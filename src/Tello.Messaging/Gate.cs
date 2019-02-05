using System;
using System.Threading;

namespace Tello.Messaging
{
    public sealed class Gate
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public T WithReadLock<T>(Func<T> func)
        {
            _lock.EnterReadLock();
            try
            {
                return func.Invoke();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void WithReadLock(Action action)
        {
            _lock.EnterReadLock();
            try
            {
                action.Invoke();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public T WithUpgradeableReadLock<T>(Func<T> func)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                return func.Invoke();
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public void WithUpgradeableReadLock(Action action)
        {
            _lock.EnterUpgradeableReadLock();
            try
            {
                action.Invoke();
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public void WithWriteLock(Action action)
        {
            _lock.EnterWriteLock();
            try
            {
                action.Invoke();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public T WithWriteLock<T>(Func<T> func)
        {
            _lock.EnterWriteLock();
            try
            {
                return func.Invoke();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
