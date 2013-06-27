using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module
    /// </summary>
    public sealed class CswEnumNbtModuleName : CswEnum<CswEnumNbtModuleName>
    {
        private CswEnumNbtModuleName( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtModuleName> _All { get { return All; } }
        public static implicit operator CswEnumNbtModuleName( string str )
        {
            CswEnumNbtModuleName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtModuleName Unknown = new CswEnumNbtModuleName( "Unknown" );

        /// <summary>
        /// BioSafety
        /// </summary>
        public static readonly CswEnumNbtModuleName BioSafety = new CswEnumNbtModuleName( "BioSafety" );
        /// <summary>
        /// Control Chart Pro
        /// </summary> 
        public static readonly CswEnumNbtModuleName CCPro = new CswEnumNbtModuleName( "CCPro" );
        /// <summary>
        /// Chemical Inventory
        /// </summary>
        public static readonly CswEnumNbtModuleName CISPro = new CswEnumNbtModuleName( "CISPro" );
        /// <summary>
        /// Development
        /// </summary>
        public static readonly CswEnumNbtModuleName Dev = new CswEnumNbtModuleName( "Dev" );
        /// <summary>
        /// Material Life-cycle Management
        /// </summary>
        public static readonly CswEnumNbtModuleName MLM = new CswEnumNbtModuleName( "MLM" );
        /// <summary>
        /// Instrument Maintenance and Calibration
        /// </summary>
        public static readonly CswEnumNbtModuleName IMCS = new CswEnumNbtModuleName( "IMCS" );
        /// <summary>
        /// NBT Management Application
        /// </summary>
        public static readonly CswEnumNbtModuleName NBTManager = new CswEnumNbtModuleName( "NBTManager" );
        /// <summary>
        /// Site Inspection
        /// </summary>
        public static readonly CswEnumNbtModuleName SI = new CswEnumNbtModuleName( "SI" );
        /// <summary>
        /// Sample Tracking
        /// </summary>
        public static readonly CswEnumNbtModuleName STIS = new CswEnumNbtModuleName( "STIS" );
        /// <summary>
        /// ChemCatCentral
        /// </summary>
        public static readonly CswEnumNbtModuleName C3 = new CswEnumNbtModuleName( "C3" );
        /// <summary>
        /// Containers
        /// </summary>
        public static readonly CswEnumNbtModuleName Containers = new CswEnumNbtModuleName( "Containers" );
        /// <summary>
        /// Regulatory Lists
        /// </summary>
        public static readonly CswEnumNbtModuleName RegulatoryLists = new CswEnumNbtModuleName( "Regulatory Lists" );
        /// <summary>
        /// Module that allows fire and hazard reporting on containers
        /// </summary>
        public static readonly CswEnumNbtModuleName FireCode = new CswEnumNbtModuleName( "Fire Code" );
        /// <summary>
        /// Module that allows safety data sheets on materials
        /// </summary>
        public static readonly CswEnumNbtModuleName SDS = new CswEnumNbtModuleName( "SDS" );
        /// <summary>
        /// Multiple Site NodeTypes
        /// </summary>
        public static readonly CswEnumNbtModuleName MultiSite = new CswEnumNbtModuleName( "Multi Site" );
        /// <summary>
        /// Multiple Site NodeTypes
        /// </summary>
        public static readonly CswEnumNbtModuleName MultiInventoryGroup = new CswEnumNbtModuleName( "Multi Inventory Group" );
        /// <summary>
        /// Module that syncs FireDb data with ChemCatCentral.
        /// </summary>
        public static readonly CswEnumNbtModuleName FireDbSync = new CswEnumNbtModuleName( "FireDb Sync" );
        /// <summary>
        /// Module that syncs PCID data with ChemCatCentral.
        /// </summary>
        public static readonly CswEnumNbtModuleName PCIDSync = new CswEnumNbtModuleName( "PCID Sync" );
        /// <summary>
        /// Certificate of Analysis
        /// </summary>
        public static readonly CswEnumNbtModuleName CofA = new CswEnumNbtModuleName( "C of A" );
        /// <summary>
        /// Module that syncs Regulatory List data with LOLI.
        /// </summary>
        public static readonly CswEnumNbtModuleName LOLISync = new CswEnumNbtModuleName( "LOLI Sync" );


    } // class CswNbtModule
}// namespace ChemSW.Nbt
