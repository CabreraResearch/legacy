using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Batch
{
    /// <summary>
    /// Enum: Batch Operation Name
    /// </summary>
    public sealed class NbtBatchOpName : CswEnum<NbtBatchOpName>
    {
        private NbtBatchOpName( string Name ) : base( Name ) { }
        public static IEnumerable<NbtBatchOpName> _All { get { return All; } }
        public static implicit operator NbtBatchOpName( string str )
        {
            NbtBatchOpName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtBatchOpName Unknown = new NbtBatchOpName( "Unknown" );

        public static readonly NbtBatchOpName FutureNodes = new NbtBatchOpName( "FutureNodes" );
        public static readonly NbtBatchOpName MultiEdit = new NbtBatchOpName( "MultiEdit" );
        public static readonly NbtBatchOpName MultiDelete = new NbtBatchOpName( "MultiDelete" );
        public static readonly NbtBatchOpName InventoryLevel = new NbtBatchOpName( "InventoryLevel" );
        public static readonly NbtBatchOpName MailReport = new NbtBatchOpName( "MailReport" );
        public static readonly NbtBatchOpName UpdateRegulatoryLists = new NbtBatchOpName( "UpdateRegulatoryLists" );
        public static readonly NbtBatchOpName UpdateRegulatoryListsForMaterials = new NbtBatchOpName( "UpdateRegulatoryListsForMaterials" );
        public static readonly NbtBatchOpName ExpiredContainers = new NbtBatchOpName( "ExpiredContainers" );
        public static readonly NbtBatchOpName MolFingerprints = new NbtBatchOpName( "MolFingerprints" );
        public static readonly NbtBatchOpName SyncLocation = new NbtBatchOpName( "SyncLocation" );
    }

    /// <summary>
    /// Enum: Batch Operation Status
    /// </summary>
    public sealed class NbtBatchOpStatus : CswEnum<NbtBatchOpStatus>
    {
        private NbtBatchOpStatus( string Name ) : base( Name ) { }
        public static IEnumerable<NbtBatchOpStatus> _All { get { return All; } }
        public static implicit operator NbtBatchOpStatus( string str )
        {
            NbtBatchOpStatus ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly NbtBatchOpStatus Unknown = new NbtBatchOpStatus( "Unknown" );

        public static readonly NbtBatchOpStatus Pending = new NbtBatchOpStatus( "Pending" );
        public static readonly NbtBatchOpStatus Processing = new NbtBatchOpStatus( "Processing" );
        public static readonly NbtBatchOpStatus Completed = new NbtBatchOpStatus( "Completed" );
        public static readonly NbtBatchOpStatus Error = new NbtBatchOpStatus( "Error" );
    }

} // namespace ChemSW.Nbt.Batch
