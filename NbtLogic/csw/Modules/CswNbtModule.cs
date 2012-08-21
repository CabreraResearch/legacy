using System;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module
    /// </summary>
    public sealed class CswNbtModule : CswEnum<CswNbtModule>
    {
        private CswNbtModule( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtModule> _All { get { return All; } }
        public static implicit operator CswNbtModule( string str )
        {
            CswNbtModule ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswNbtModule Unknown = new CswNbtModule( "Unknown" );

        /// <summary>
        /// BioSafety
        /// </summary>
        public static readonly CswNbtModule BioSafety = new CswNbtModule( "BioSafety" );
        /// <summary>
        /// Control Chart Pro
        /// </summary> 
        public static readonly CswNbtModule CCPro = new CswNbtModule( "CCPro" );
        /// <summary>
        /// Chemical Inventory
        /// </summary>
        public static readonly CswNbtModule CISPro = new CswNbtModule( "CISPro" );
        /// <summary>
        /// Mobile
        /// </summary>
        public static readonly CswNbtModule Mobile = new CswNbtModule( "Mobile" );
        /// <summary>
        /// Instrument Maintenance and Calibration
        /// </summary>
        public static readonly CswNbtModule IMCS = new CswNbtModule( "IMCS" );
        /// <summary>
        /// NBT Management Application
        /// </summary>
        public static readonly CswNbtModule NBTManager = new CswNbtModule( "NBTManager" );
        /// <summary>
        /// Site Inspection
        /// </summary>
        public static readonly CswNbtModule SI = new CswNbtModule( "SI" );
        /// <summary>
        /// Sample Tracking
        /// </summary>
        public static readonly CswNbtModule STIS = new CswNbtModule( "STIS" );
    
    } // class CswNbtModule
}// namespace ChemSW.Nbt
