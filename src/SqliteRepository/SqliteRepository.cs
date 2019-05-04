using Repository;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SqliteRepository
{
    public class SqliteRepository : IRepository
    {
        private readonly SQLiteConnection _sqlite;

        #region ctor
        public SqliteRepository()
            : this((null, null))
        { }

        public SqliteRepository((string databasePath, string databaseName) settings)
        {
            var databaseName = String.IsNullOrEmpty(settings.databaseName)
                ? "tello.sqlite"
                : settings.databaseName;

            var connectionString = String.IsNullOrEmpty(settings.databasePath)
                ? databaseName
                : $"{settings.databasePath}/{databaseName}";

            _sqlite = _sqlite ??
                new SQLiteConnection(
                    connectionString,
                    SQLiteOpenFlags.Create
                        | SQLiteOpenFlags.ReadWrite
                        | SQLiteOpenFlags.FullMutex,
                    true)
                { BusyTimeout = TimeSpan.FromSeconds(10) };
        }
        #endregion

        public bool CreateCatalog<T>()
        {
            return _sqlite.CreateTable<T>() == CreateTableResult.Created;
        }

        public int Delete<T>() where T : IEntity, new()
        {
            return _sqlite.DeleteAll<T>();
        }

        public int Delete<T>(int id) where T : IEntity, new()
        {
            return _sqlite.Delete<T>(id);
        }

        public int Delete<T>(T entity) where T : IEntity, new()
        {
            return Delete<T>(entity.Id);
        }

        public int Delete<T>(Expression<Func<T, bool>> predicate) where T : IEntity, new()
        {
            return _sqlite.Delete(predicate);
        }

        public T Read<T>(int id) where T : IEntity, new()
        {
            return _sqlite.Find<T>(id);
        }

        public T[] Read<T>() where T : IEntity, new()
        {
            return _sqlite
                .Table<T>()
                .ToArray();
        }

        public T[] Read<T>(Expression<Func<T, bool>> predicate) where T : IEntity, new()
        {
            return _sqlite
                .Table<T>()
                .Where(predicate).
                ToArray();
        }

        public int Insert<T>(T entity) where T : IEntity, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return _sqlite.Insert(entity);
        }

        public int Insert<T>(IEnumerable<T> entities) where T : IEntity, new()
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return entities.Sum(e => Insert(e));
        }

        public int Update<T>(T entity) where T : IEntity, new()
        {
            return _sqlite.Update(entity);
        }

        public int Update<T>(IEnumerable<T> entities) where T : IEntity, new()
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return entities.Sum(e => Update(e));
        }
    }
}
