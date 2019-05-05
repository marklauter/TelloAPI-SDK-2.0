using SQLite;
using System;
using System.Linq.Expressions;

namespace Tello.Observations.Sqlite
{
    public class TrackingSession : ITrackingSession
    {
        private readonly string _databaseName = "tello.sqlite";
        private readonly string _connectionString;
        private static SQLiteConnection _sqliteConnection;

        public string SessionId { get; }

        public TrackingSession(string sessionPrefix, string databaseName = null)
        {
            SessionId = $"{sessionPrefix}-{Guid.NewGuid()}-{DateTime.UtcNow.ToString("o")}";

            var path = Environment.GetEnvironmentVariable("sqlite_connectionstring_path", EnvironmentVariableTarget.Machine);
            _databaseName = String.IsNullOrEmpty(databaseName)
                ? _databaseName
                : databaseName;
            _connectionString = path != null
                ? $"{path}/{_databaseName}"
                : _databaseName;

            _sqliteConnection = _sqliteConnection ?? 
                new SQLiteConnection(
                    _connectionString, 
                    SQLiteOpenFlags.Create 
                        | SQLiteOpenFlags.ReadWrite 
                        | SQLiteOpenFlags.FullMutex,
                    true);
        }

        public void Write<T>(T observation) where T : IObservation, new()
        {
            if (observation == null)
            {
                throw new ArgumentNullException(nameof(observation));
            }

            observation.SessionId = SessionId;
            _sqliteConnection.CreateTable<T>();
            _sqliteConnection.Insert(observation);
        }

        public T[] Read<T>() where T : IObservation, new()
        {
            _sqliteConnection.CreateTable<T>();
            return _sqliteConnection.Table<T>()
                .ToArray();
        }

        public T[] Read<T>(Expression<Func<T, bool>> predicate) where T : IObservation, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            _sqliteConnection.CreateTable<T>();
            return _sqliteConnection.Table<T>()
                .Where(predicate)
                .ToArray();
        }

        public T[] Read<T>(TimeSpan age) where T : IObservation, new()
        {
            _sqliteConnection.CreateTable<T>();
            return _sqliteConnection
                .Table<T>()
                .Where(o => o.Timestamp >= DateTime.UtcNow - age)
                .ToArray();
        }

        public void Delete<T>() where T : IObservation, new()
        {
            _sqliteConnection.CreateTable<T>();
            _sqliteConnection.DeleteAll<T>();
        }

        public void Delete<T>(TimeSpan age) where T : IObservation, new()
        {
            var oldestDate = DateTime.UtcNow - age;
            _sqliteConnection.CreateTable<T>();
            var mapping = _sqliteConnection.GetMapping<T>();
            var query = $"delete from {mapping.TableName} where Timestamp <= ?";
            _sqliteConnection.Execute(query, oldestDate);
        }
    }
}
