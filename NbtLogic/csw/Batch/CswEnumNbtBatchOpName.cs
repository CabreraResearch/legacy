﻿using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.Batch
{
    /// <summary>
    /// Enum: Batch Operation Name
    /// </summary>
    public sealed class CswEnumNbtBatchOpName : CswEnum<CswEnumNbtBatchOpName>
    {
        private CswEnumNbtBatchOpName( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtBatchOpName> _All { get { return All; } }
        public static implicit operator CswEnumNbtBatchOpName( string str )
        {
            CswEnumNbtBatchOpName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtBatchOpName Unknown = new CswEnumNbtBatchOpName( "Unknown" );

        public static readonly CswEnumNbtBatchOpName BatchEdit = new CswEnumNbtBatchOpName( "BatchEdit" );
        public static readonly CswEnumNbtBatchOpName FutureNodes = new CswEnumNbtBatchOpName( "FutureNodes" );
        public static readonly CswEnumNbtBatchOpName MultiEdit = new CswEnumNbtBatchOpName( "MultiEdit" );
        public static readonly CswEnumNbtBatchOpName MultiButtonClick = new CswEnumNbtBatchOpName( "MultiButtonClick" );
        public static readonly CswEnumNbtBatchOpName MultiDelete = new CswEnumNbtBatchOpName( "MultiDelete" );
        public static readonly CswEnumNbtBatchOpName InventoryLevel = new CswEnumNbtBatchOpName( "InventoryLevel" );
        public static readonly CswEnumNbtBatchOpName SyncLocation = new CswEnumNbtBatchOpName( "SyncLocation" );
        public static readonly CswEnumNbtBatchOpName MobileMultiOpUpdates = new CswEnumNbtBatchOpName( "MobileMultiOpUpdates" );
        public static readonly CswEnumNbtBatchOpName Receiving = new CswEnumNbtBatchOpName( "Receiving" );
    }

} // namespace ChemSW.Nbt.Batch
