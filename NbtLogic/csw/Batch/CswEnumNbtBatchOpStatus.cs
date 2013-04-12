using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Batch
{
    /// <summary>
    /// Enum: Batch Operation Status
    /// </summary>
    public sealed class CswEnumNbtBatchOpStatus : CswEnum<CswEnumNbtBatchOpStatus>
    {
        private CswEnumNbtBatchOpStatus( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtBatchOpStatus> _All { get { return All; } }
        public static implicit operator CswEnumNbtBatchOpStatus( string str )
        {
            CswEnumNbtBatchOpStatus ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtBatchOpStatus Unknown = new CswEnumNbtBatchOpStatus( "Unknown" );

        public static readonly CswEnumNbtBatchOpStatus Pending = new CswEnumNbtBatchOpStatus( "Pending" );
        public static readonly CswEnumNbtBatchOpStatus Processing = new CswEnumNbtBatchOpStatus( "Processing" );
        public static readonly CswEnumNbtBatchOpStatus Completed = new CswEnumNbtBatchOpStatus( "Completed" );
        public static readonly CswEnumNbtBatchOpStatus Error = new CswEnumNbtBatchOpStatus( "Error" );
    }

} // namespace ChemSW.Nbt.Batch
