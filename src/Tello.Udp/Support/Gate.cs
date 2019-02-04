using System;
using System.Threading;

namespace Tello.Udp
{
    internal sealed class Gate
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        internal T WithReadLock<T>(Func<T> func)
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

        internal void WithReadLock(Action action)
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

        internal T WithUpgradeableReadLock<T>(Func<T> func)
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

        internal void WithUpgradeableReadLock(Action action)
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

        internal void WithWriteLock(Action action)
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

        internal T WithWriteLock<T>(Func<T> func)
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
