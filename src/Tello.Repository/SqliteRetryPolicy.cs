using SQLite;
using Sumo.Retry.Policies;
using System;
using System.Collections.Generic;

namespace Tello.Repository
{
    /// <summary>
    /// https://www.activesphere.com/blog/2018/12/24/understanding-sqlite-busy
    /// https://sqlite.org/rescode.html
    /// </summary>
    internal sealed class SqliteTransientRetryPolicy : CustomRetryPolicy
    {
        public SqliteTransientRetryPolicy(int maxAttempts, TimeSpan timeout, TimeSpan? initialInterval = null) : base(maxAttempts, timeout, initialInterval)
        {
        }

        public SqliteTransientRetryPolicy(RetryPolicy retryPolicy) : base(retryPolicy)
        {
        }

        // https://sqlite.org/rescode.html
        private static HashSet<int> _transientErrors = new HashSet<int>(new int[]
        {
            (int)SQLite3.Result.Busy,
            (int)SQLite3.Result.Locked,
            (int)SQLite3.Result.NoMem,
            (int)SQLite3.Result.Interrupt,
            (int)SQLite3.Result.IOError,
            (int)SQLite3.Result.LockErr,
            (int)SQLite3.Result.SchemaChngd,
            (int)SQLite3.Result.Notice,
            (int)SQLite3.Result.Warning,
            (int)SQLite3.ExtendedResult.BusyRecovery,
            (int)SQLite3.ExtendedResult.LockedSharedcache,
            (int)SQLite3.ExtendedResult.ReadonlyRecovery,
            (int)SQLite3.ExtendedResult.IOErrorRead,
            (int)SQLite3.ExtendedResult.IOErrorShortRead,
            (int)SQLite3.ExtendedResult.NoticeRecoverWAL,
            513, // SQLITE_ERROR_RETRY
            517, // SQLITE_BUSY_SNAPSHOT
            (int)SQLite3.ExtendedResult.IOErrorWrite,
            (int)SQLite3.ExtendedResult.IOErrorFsync,
            (int)SQLite3.ExtendedResult.IOErrorDirFSync,
            (int)SQLite3.ExtendedResult.IOErrorTruncate,
            (int)SQLite3.ExtendedResult.IOErrorFStat,
            (int)SQLite3.ExtendedResult.IOErrorUnlock,
            (int)SQLite3.ExtendedResult.IOErrorRdlock,
            (int)SQLite3.ExtendedResult.IOErrorDelete,
            (int)SQLite3.ExtendedResult.IOErrorBlocked,
            (int)SQLite3.ExtendedResult.IOErrorNoMem,
            (int)SQLite3.ExtendedResult.IOErrorAccess,
            (int)SQLite3.ExtendedResult.IOErrorCheckReservedLock,
            (int)SQLite3.ExtendedResult.IOErrorLock,
            (int)SQLite3.ExtendedResult.IOErrorClose,
            (int)SQLite3.ExtendedResult.IOErrorDirClose,
            (int)SQLite3.ExtendedResult.IOErrorSHMOpen,
            (int)SQLite3.ExtendedResult.IOErrorSHMSize,
            (int)SQLite3.ExtendedResult.IOErrorSHMLock,
            (int)SQLite3.ExtendedResult.IOErrorSHMMap,
            (int)SQLite3.ExtendedResult.IOErrorSeek,
            (int)SQLite3.ExtendedResult.IOErrorDeleteNoEnt,
            (int)SQLite3.ExtendedResult.IOErrorMMap
        });

        public override bool IsRetryAllowed(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            return ex is SQLiteException && _transientErrors.Contains((int)(ex as SQLiteException).Result);
        }
    }

}
