using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.DB;

namespace ChemSW.Nbt.Batch
{
    /// <summary>
    /// Enum: Batch Operation Name
    /// </summary>
    public sealed class NbtBatchOpName : CswEnum<NbtBatchOpName>
    {
        private NbtBatchOpName( string Name ) : base( Name ) { }
        public static IEnumerable<NbtBatchOpName> _All { get { return CswEnum<NbtBatchOpName>.All; } }
        public static explicit operator NbtBatchOpName( string str )
        {
            NbtBatchOpName ret = Parse( str );
            return ( ret != null ) ? ret : NbtBatchOpName.Unknown;
        }
        public static readonly NbtBatchOpName Unknown = new NbtBatchOpName( "Unknown" );

        public static readonly NbtBatchOpName FutureNodes = new NbtBatchOpName( "FutureNodes" );
    }

    /// <summary>
    /// Enum: Batch Operation Status
    /// </summary>
    public sealed class NbtBatchOpStatus : CswEnum<NbtBatchOpStatus>
    {
        private NbtBatchOpStatus( string Name ) : base( Name ) { }
        public static IEnumerable<NbtBatchOpStatus> _All { get { return CswEnum<NbtBatchOpStatus>.All; } }
        public static explicit operator NbtBatchOpStatus( string str )
        {
            NbtBatchOpStatus ret = Parse( str );
            return ( ret != null ) ? ret : NbtBatchOpStatus.Unknown;
        }
        public static readonly NbtBatchOpStatus Unknown = new NbtBatchOpStatus( "Unknown" );

        public static readonly NbtBatchOpStatus Pending = new NbtBatchOpStatus( "Pending" );
        public static readonly NbtBatchOpStatus Processing = new NbtBatchOpStatus( "Processing" );
        public static readonly NbtBatchOpStatus Completed = new NbtBatchOpStatus( "Completed" );
        public static readonly NbtBatchOpStatus Error = new NbtBatchOpStatus( "Error" );
    }

} // namespace ChemSW.Nbt.Batch
