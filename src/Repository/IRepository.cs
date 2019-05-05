using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Repository
{
    public interface IRepository : IDisposable
    {
        bool CreateCatalog<T>() where T : IEntity, new();

        T NewEntity<T>(params object[] args) where T : IEntity, new();

        int Insert<T>(T entity) where T : IEntity, new();
        int Insert<T>(IEnumerable<T> entities) where T : IEntity, new();

        int Update<T>(T entity) where T : IEntity, new();
        int Update<T>(IEnumerable<T> entities) where T : IEntity, new();

        T Read<T>(int id) where T : IEntity, new();
        T[] Read<T>() where T : IEntity, new();
        T[] Read<T>(Expression<Func<T, bool>> predicate) where T : IEntity, new();

        int Delete<T>() where T : IEntity, new();
        int Delete<T>(int id) where T : IEntity, new();
        int Delete<T>(T entity) where T : IEntity, new();
        int Delete<T>(Expression<Func<T, bool>> predicate) where T : IEntity, new();

        void Shrink();
    }
}

