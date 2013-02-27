using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module
    /// </summary>
    public sealed class CswNbtModuleName : CswEnum<CswNbtModuleName>
    {
        private CswNbtModuleName( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtModuleName> _All { get { return All; } }
        public static implicit operator CswNbtModuleName( string str )
        {
            CswNbtModuleName ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswNbtModuleName Unknown = new CswNbtModuleName( "Unknown" );

        /// <summary>
        /// BioSafety
        /// </summary>
        public static readonly CswNbtModuleName BioSafety = new CswNbtModuleName( "BioSafety" );
        /// <summary>
        /// Control Chart Pro
        /// </summary> 
        public static readonly CswNbtModuleName CCPro = new CswNbtModuleName( "CCPro" );
        /// <summary>
        /// Chemical Inventory
        /// </summary>
        public static readonly CswNbtModuleName CISPro = new CswNbtModuleName( "CISPro" );
        /// <summary>
        /// Development
        /// </summary>
        public static readonly CswNbtModuleName Dev = new CswNbtModuleName( "Dev" );
        /// <summary>
        /// Material Life-cycle Management
        /// </summary>
        public static readonly CswNbtModuleName MLM = new CswNbtModuleName( "MLM" );
        /// <summary>
        /// Instrument Maintenance and Calibration
        /// </summary>
        public static readonly CswNbtModuleName IMCS = new CswNbtModuleName( "IMCS" );
        /// <summary>
        /// NBT Management Application
        /// </summary>
        public static readonly CswNbtModuleName NBTManager = new CswNbtModuleName( "NBTManager" );
        /// <summary>
        /// Site Inspection
        /// </summary>
        public static readonly CswNbtModuleName SI = new CswNbtModuleName( "SI" );
        /// <summary>
        /// Sample Tracking
        /// </summary>
        public static readonly CswNbtModuleName STIS = new CswNbtModuleName( "STIS" );
        /// <summary>
        /// ChemCatCentral
        /// </summary>
        public static readonly CswNbtModuleName C3 = new CswNbtModuleName( "C3" );
        /// <summary>
        /// Containers
        /// </summary>
        public static readonly CswNbtModuleName Containers = new CswNbtModuleName( "Containers" );
        /// <summary>
        /// Regulatory Lists
        /// </summary>
        public static readonly CswNbtModuleName RegulatoryLists = new CswNbtModuleName( "Regulatory Lists" );
        /// <summary>
        /// Multiple Site NodeTypes
        /// </summary>
        public static readonly CswNbtModuleName MultiSite = new CswNbtModuleName( "Multi Site" );

    } // class CswNbtModule
}// namespace ChemSW.Nbt
