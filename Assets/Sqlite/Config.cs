
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

    public enum SQLiteResult
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
        IOErrorRead = (SQLiteResult.IOError | (1 << 8)),
        IOErrorShortRead = (SQLiteResult.IOError | (2 << 8)),
        IOErrorWrite = (SQLiteResult.IOError | (3 << 8)),
        IOErrorFsync = (SQLiteResult.IOError | (4 << 8)),
        IOErrorDirFSync = (SQLiteResult.IOError | (5 << 8)),
        IOErrorTruncate = (SQLiteResult.IOError | (6 << 8)),
        IOErrorFStat = (SQLiteResult.IOError | (7 << 8)),
        IOErrorUnlock = (SQLiteResult.IOError | (8 << 8)),
        IOErrorRdlock = (SQLiteResult.IOError | (9 << 8)),
        IOErrorDelete = (SQLiteResult.IOError | (10 << 8)),
        IOErrorBlocked = (SQLiteResult.IOError | (11 << 8)),
        IOErrorNoMem = (SQLiteResult.IOError | (12 << 8)),
        IOErrorAccess = (SQLiteResult.IOError | (13 << 8)),
        IOErrorCheckReservedLock = (SQLiteResult.IOError | (14 << 8)),
        IOErrorLock = (SQLiteResult.IOError | (15 << 8)),
        IOErrorClose = (SQLiteResult.IOError | (16 << 8)),
        IOErrorDirClose = (SQLiteResult.IOError | (17 << 8)),
        IOErrorSHMOpen = (SQLiteResult.IOError | (18 << 8)),
        IOErrorSHMSize = (SQLiteResult.IOError | (19 << 8)),
        IOErrorSHMLock = (SQLiteResult.IOError | (20 << 8)),
        IOErrorSHMMap = (SQLiteResult.IOError | (21 << 8)),
        IOErrorSeek = (SQLiteResult.IOError | (22 << 8)),
        IOErrorDeleteNoEnt = (SQLiteResult.IOError | (23 << 8)),
        IOErrorMMap = (SQLiteResult.IOError | (24 << 8)),
        LockedSharedcache = (SQLiteResult.Locked | (1 << 8)),
        BusyRecovery = (SQLiteResult.Busy | (1 << 8)),
        CannottOpenNoTempDir = (SQLiteResult.CannotOpen | (1 << 8)),
        CannotOpenIsDir = (SQLiteResult.CannotOpen | (2 << 8)),
        CannotOpenFullPath = (SQLiteResult.CannotOpen | (3 << 8)),
        CorruptVTab = (SQLiteResult.Corrupt | (1 << 8)),
        ReadonlyRecovery = (SQLiteResult.ReadOnly | (1 << 8)),
        ReadonlyCannotLock = (SQLiteResult.ReadOnly | (2 << 8)),
        ReadonlyRollback = (SQLiteResult.ReadOnly | (3 << 8)),
        AbortRollback = (SQLiteResult.Abort | (2 << 8)),
        ConstraintCheck = (SQLiteResult.Constraint | (1 << 8)),
        ConstraintCommitHook = (SQLiteResult.Constraint | (2 << 8)),
        ConstraintForeignKey = (SQLiteResult.Constraint | (3 << 8)),
        ConstraintFunction = (SQLiteResult.Constraint | (4 << 8)),
        ConstraintNotNull = (SQLiteResult.Constraint | (5 << 8)),
        ConstraintPrimaryKey = (SQLiteResult.Constraint | (6 << 8)),
        ConstraintTrigger = (SQLiteResult.Constraint | (7 << 8)),
        ConstraintUnique = (SQLiteResult.Constraint | (8 << 8)),
        ConstraintVTab = (SQLiteResult.Constraint | (9 << 8)),
        NoticeRecoverWAL = (SQLiteResult.Notice | (1 << 8)),
        NoticeRecoverRollback = (SQLiteResult.Notice | (2 << 8))
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
