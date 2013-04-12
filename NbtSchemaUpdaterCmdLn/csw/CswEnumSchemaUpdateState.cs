using System;
using System.Threading;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public enum CswEnumSchemaUpdateState
    {
        Idle,
        Running,
        Succeeded,
        Failed
    };
} //ChemSW.Nbt.Schema.CmdLn
