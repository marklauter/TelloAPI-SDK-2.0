// <copyright file="SqliteRepository.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using SQLite;

namespace Repository.Sqlite
{
    public class SqliteRepository : IRepository, IDisposable
    {
        private readonly SQLiteConnection sqlite;

        #region ctor
        public SqliteRepository()
            : this((null, null))
        {
        }

        public SqliteRepository((string databasePath, string databaseName) settings)
        {
            var databaseName = String.IsNullOrEmpty(settings.databaseName)
                ? "repository.sqlite"
                : settings.databaseName;

            var connectionString = String.IsNullOrEmpty(settings.databasePath)
                ? databaseName
                : Path.Combine(settings.databasePath, databaseName);

            this.sqlite = this.sqlite ??
                new SQLiteConnection(
                    connectionString,
                    SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex,
                    true)
                { BusyTimeout = TimeSpan.FromSeconds(10) };
        }
        #endregion

        public bool CreateCatalog<T>()
            where T : IEntity, new()
        {
            return this.sqlite.CreateTable<T>() == CreateTableResult.Created;
        }

        public T NewEntity<T>(params object[] args)
            where T : IEntity, new()
        {
            var result = (T)Activator.CreateInstance(typeof(T), args);

            // inserts fresh, updates ID with auto inc value
            if (this.Insert(result) == 1)
            {
                return result;
            }
            else
            {
                throw new Exception($"failed to insert type {typeof(T).Name}");
            }
        }

        public int Delete<T>()
            where T : IEntity, new()
        {
            return this.sqlite.DeleteAll<T>();
        }

        public int Delete<T>(int id)
            where T : IEntity, new()
        {
            return this.sqlite.Delete<T>(id);
        }

        public int Delete<T>(T entity)
            where T : IEntity, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return this.Delete<T>(entity.Id);
        }

        public int Delete<T>(Expression<Func<T, bool>> predicate)
            where T : IEntity, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var entities = this.Read(predicate);

            return entities.Sum(e => this.Delete(e));
        }

        public T Read<T>(int id)
            where T : IEntity, new()
        {
            return this.sqlite.Find<T>(id);
        }

        public T[] Read<T>()
            where T : IEntity, new()
        {
            return this.sqlite
                .Table<T>()
                .ToArray();
        }

        public T[] Read<T>(Expression<Func<T, bool>> predicate)
            where T : IEntity, new()
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return this.sqlite
                .Table<T>()
                .Where(predicate).
                ToArray();
        }

        public int Insert<T>(T entity)
            where T : IEntity, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return this.sqlite.Insert(entity);
        }

        public int Insert<T>(IEnumerable<T> entities)
            where T : IEntity, new()
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return entities.Sum(e => this.Insert(e));
        }

        public int Update<T>(T entity)
            where T : IEntity, new()
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return this.sqlite.Update(entity);
        }

        public int Update<T>(IEnumerable<T> entities)
            where T : IEntity, new()
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return entities.Sum(e => this.Update(e));
        }

        public void Shrink()
        {
            this.sqlite.Execute("vacuum");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.sqlite.Dispose();
                }

                this.disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            this.Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
