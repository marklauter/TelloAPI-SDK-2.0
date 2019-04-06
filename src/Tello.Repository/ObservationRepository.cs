using SQLite;
using Sumo.Retry;
using Sumo.Retry.Policies;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Tello.Repository
{
    public interface IRepository
    {
        Task Write<T>(T observation) where T : IObservation, new();

        Task<T[]> Read<T>() where T : IObservation, new();
        Task<T[]> Read<T>(Expression<Func<T, bool>> predicate) where T : IObservation, new();
        Task<T[]> Read<T>(TimeSpan age) where T : IObservation, new();

        Task Clear<T>() where T : IObservation, new();
        Task Clear<T>(TimeSpan age) where T : IObservation, new();
    }

    public class ObservationRepository : IRepository
    {
        private readonly RetryPolicy _retryPolicy = new SqliteTransientRetryPolicy(600, TimeSpan.FromMinutes(3), TimeSpan.FromMilliseconds(100));
        private readonly string _databaseName = "tello.sqlite";
        private readonly string _connectionString;

        public ObservationRepository(string databaseName = null)
        {
            var path = Environment.GetEnvironmentVariable("sqlite_connectionstring_path", EnvironmentVariableTarget.Machine);
            _databaseName = String.IsNullOrEmpty(databaseName)
                ? _databaseName
                : databaseName;
            _connectionString = path != null
                ? $"{path}/{_databaseName}"
                : _databaseName;
        }

        private async Task InvokeDbOperation(Action<SQLiteConnection> action)
        {
            await WithRetry.InvokeAsync(_retryPolicy, () =>
            {
                //todo: look into SQLiteOpenFlags - the mobile app is encountering db locked exceptions and I think SQLiteOpenFlags might be the solution
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.BeginTransaction();
                    try
                    {
                        action(connection);
                        connection.Commit();
                    }
                    catch
                    {
                        connection.Rollback();
                        throw;
                    }
                }
            });
        }

        public async Task Write<T>(T observation) where T : IObservation, new()
        {
            if (observation == null)
            {
                throw new ArgumentNullException(nameof(observation));
            }

            await InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                connection.Insert(observation);
            });
        }

        public async Task<T[]> Read<T>() where T : IObservation, new()
        {
            var result = default(T[]);
            await InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                result = connection.Table<T>()
                    .ToArray();
            });
            return result;
        }

        public async Task<T[]> Read<T>(Expression<Func<T, bool>> predicate) where T : IObservation, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var result = default(T[]);
            await InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                result = connection.Table<T>()
                    .Where(predicate)
                    .ToArray();
            });
            return result;
        }

        public async Task<T[]> Read<T>(TimeSpan age) where T : IObservation, new()
        {
            var result = default(T[]);
            await InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                result = connection
                    .Table<T>()
                    .Where(o => o.Timestamp >= DateTime.UtcNow - age)
                    .ToArray();
            });
            return result;
        }

        public async Task Clear<T>() where T : IObservation, new()
        {
            await InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                connection.DeleteAll<T>();
            });
        }

        public async Task Clear<T>(TimeSpan age) where T : IObservation, new()
        {
            var oldestDate = DateTime.UtcNow - age;
            var query = $"delete from {typeof(T).Name} where Timestamp <= ?";
            await InvokeDbOperation((connection) =>
            {
                connection.CreateTable<T>();
                connection.Execute(query, oldestDate);
            });
        }
    }
}
