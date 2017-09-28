
using System;

namespace szn
{
    [Flags]
    public enum SQLiteOpenFlags
    {
        ReadOnly = 1, ReadWrite = 2, Create = 4,
        NoMutex = 0x8000, FullMutex = 0x10000,
        SharedCache = 0x20000, PrivateCache = 0x40000,
        ProtectionComplete = 0x00100000,
        ProtectionCompleteUnlessOpen = 0x00200000,
        ProtectionCompleteUntilFirstUserAuthentication = 0x00300000,
        ProtectionNone = 0x00400000
    }

    [Flags]
    public enum CreateFlags
    {
        None = 0,
        ImplicitPK = 1,    // create a primary key for field called 'Id' (Orm.ImplicitPkName)
        ImplicitIndex = 2, // create an index for fields ending in 'Id' (Orm.ImplicitIndexSuffix)
        AllImplicit = 3,   // do both above
        AutoIncPK = 4      // force PK field to be auto inc
    }

    public enum SQLite3Result
    {
        OK = 0,
        Error = 1,
        Internal = 2,
        Perm = 3,
        Abort = 4,
        Busy = 5,
        Locked = 6,
        NoMem = 7,
        ReadOnly = 8,
        Interrupt = 9,
        IOError = 10,
        Corrupt = 11,
        NotFound = 12,
        Full = 13,
        CannotOpen = 14,
        LockErr = 15,
        Empty = 16,
        SchemaChngd = 17,
        TooBig = 18,
        Constraint = 19,
        Mismatch = 20,
        Misuse = 21,
        NotImplementedLFS = 22,
        AccessDenied = 23,
        Format = 24,
        Range = 25,
        NonDBFile = 26,
        Notice = 27,
        Warning = 28,
        Row = 100,
        Done = 101
    }

    public enum ExtendedResult
    {
        IOErrorRead = (SQLite3Result.IOError | (1 << 8)),
        IOErrorShortRead = (SQLite3Result.IOError | (2 << 8)),
        IOErrorWrite = (SQLite3Result.IOError | (3 << 8)),
        IOErrorFsync = (SQLite3Result.IOError | (4 << 8)),
        IOErrorDirFSync = (SQLite3Result.IOError | (5 << 8)),
        IOErrorTruncate = (SQLite3Result.IOError | (6 << 8)),
        IOErrorFStat = (SQLite3Result.IOError | (7 << 8)),
        IOErrorUnlock = (SQLite3Result.IOError | (8 << 8)),
        IOErrorRdlock = (SQLite3Result.IOError | (9 << 8)),
        IOErrorDelete = (SQLite3Result.IOError | (10 << 8)),
        IOErrorBlocked = (SQLite3Result.IOError | (11 << 8)),
        IOErrorNoMem = (SQLite3Result.IOError | (12 << 8)),
        IOErrorAccess = (SQLite3Result.IOError | (13 << 8)),
        IOErrorCheckReservedLock = (SQLite3Result.IOError | (14 << 8)),
        IOErrorLock = (SQLite3Result.IOError | (15 << 8)),
        IOErrorClose = (SQLite3Result.IOError | (16 << 8)),
        IOErrorDirClose = (SQLite3Result.IOError | (17 << 8)),
        IOErrorSHMOpen = (SQLite3Result.IOError | (18 << 8)),
        IOErrorSHMSize = (SQLite3Result.IOError | (19 << 8)),
        IOErrorSHMLock = (SQLite3Result.IOError | (20 << 8)),
        IOErrorSHMMap = (SQLite3Result.IOError | (21 << 8)),
        IOErrorSeek = (SQLite3Result.IOError | (22 << 8)),
        IOErrorDeleteNoEnt = (SQLite3Result.IOError | (23 << 8)),
        IOErrorMMap = (SQLite3Result.IOError | (24 << 8)),
        LockedSharedcache = (SQLite3Result.Locked | (1 << 8)),
        BusyRecovery = (SQLite3Result.Busy | (1 << 8)),
        CannottOpenNoTempDir = (SQLite3Result.CannotOpen | (1 << 8)),
        CannotOpenIsDir = (SQLite3Result.CannotOpen | (2 << 8)),
        CannotOpenFullPath = (SQLite3Result.CannotOpen | (3 << 8)),
        CorruptVTab = (SQLite3Result.Corrupt | (1 << 8)),
        ReadonlyRecovery = (SQLite3Result.ReadOnly | (1 << 8)),
        ReadonlyCannotLock = (SQLite3Result.ReadOnly | (2 << 8)),
        ReadonlyRollback = (SQLite3Result.ReadOnly | (3 << 8)),
        AbortRollback = (SQLite3Result.Abort | (2 << 8)),
        ConstraintCheck = (SQLite3Result.Constraint | (1 << 8)),
        ConstraintCommitHook = (SQLite3Result.Constraint | (2 << 8)),
        ConstraintForeignKey = (SQLite3Result.Constraint | (3 << 8)),
        ConstraintFunction = (SQLite3Result.Constraint | (4 << 8)),
        ConstraintNotNull = (SQLite3Result.Constraint | (5 << 8)),
        ConstraintPrimaryKey = (SQLite3Result.Constraint | (6 << 8)),
        ConstraintTrigger = (SQLite3Result.Constraint | (7 << 8)),
        ConstraintUnique = (SQLite3Result.Constraint | (8 << 8)),
        ConstraintVTab = (SQLite3Result.Constraint | (9 << 8)),
        NoticeRecoverWAL = (SQLite3Result.Notice | (1 << 8)),
        NoticeRecoverRollback = (SQLite3Result.Notice | (2 << 8))
    }

    public enum ConfigOption
    {
        SingleThread = 1,
        MultiThread = 2,
        Serialized = 3
    }

    public enum ColType
    {
        Integer = 1,
        Float = 2,
        Text = 3,
        Blob = 4,
        Null = 5
    }
    
    public class Config
    {
    }
}
