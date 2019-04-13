using SQLite;
using Sumo.Retry.Policies;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tello.Repository
{
    public interface IRepository
    {
        void Write<T>(T observation) where T : IObservation, new();

        T[] Read<T>() where T : IObservation, new();
        T[] Read<T>(Expression<Func<T, bool>> predicate) where T : IObservation, new();
        T[] Read<T>(TimeSpan age) where T : IObservation, new();

        void Clear<T>() where T : IObservation, new();
        void Clear<T>(TimeSpan age) where T : IObservation, new();
    }

    public class ObservationRepository : IRepository
    {
        private readonly RetryPolicy _retryPolicy = new SqliteTransientRetryPolicy(600, TimeSpan.FromMinutes(3), TimeSpan.FromMilliseconds(100));
        private readonly string _databaseName = "tello.sqlite";
        private readonly string _connectionString;
        private static SQLiteConnection _sqliteConnection;

        public ObservationRepository(string databaseName = null)
        {
            var path = Environment.GetEnvironmentVariable("sqlite_connectionstring_path", EnvironmentVariableTarget.Machine);
            _databaseName = String.IsNullOrEmpty(databaseName)
                ? _databaseName
                : databaseName;
            _connectionString = path != null
                ? $"{path}/{_databaseName}"
                : _databaseName;

            _sqliteConnection = _sqliteConnection ?? new SQLiteConnection(_connectionString, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);
        }

        private void InvokeDbOperation(Action<SQLiteConnection> action)
        {
            _sqliteConnection.BeginTransaction();
            try
            {
                action(_sqliteConnection);
                _sqliteConnection.Commit();
            }
            catch
            {
                _sqliteConnection.Rollback();
                throw;
            }
        }

        public void Write<T>(T observation) where T : IObservation, new()
        {
            if (observation == null)
            {
                throw new ArgumentNullException(nameof(observation));
            }

            InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                connection.Insert(observation);
            });
        }

        public T[] Read<T>() where T : IObservation, new()
        {
            var result = default(T[]);
            InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                result = connection.Table<T>()
                    .ToArray();
            });
            return result;
        }

        public T[] Read<T>(Expression<Func<T, bool>> predicate) where T : IObservation, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var result = default(T[]);
            InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                result = connection.Table<T>()
                    .Where(predicate)
                    .ToArray();
            });
            return result;
        }

        public T[] Read<T>(TimeSpan age) where T : IObservation, new()
        {
            var result = default(T[]);
            InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                result = connection
                    .Table<T>()
                    .Where(o => o.Timestamp >= DateTime.UtcNow - age)
                    .ToArray();
            });
            return result;
        }

        public void Clear<T>() where T : IObservation, new()
        {
            InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                connection.DeleteAll<T>();
            });
        }

        public void Clear<T>(TimeSpan age) where T : IObservation, new()
        {
            var oldestDate = DateTime.UtcNow - age;
            InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                var mapping = connection.GetMapping<T>();
                var query = $"delete from {mapping.TableName} where Timestamp <= ?";
                connection.Execute(query, oldestDate);
            });
        }
    }
}
