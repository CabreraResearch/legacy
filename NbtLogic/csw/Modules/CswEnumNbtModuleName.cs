using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT Module
    /// </summary>
    public sealed class CswEnumNbtModuleName : IEquatable<CswEnumNbtModuleName>, IComparable<CswEnumNbtModuleName>
    {
        #region Internals

        public static IEnumerable<CswEnumNbtModuleName> All
        {
            get { return _Enums.Select( KeyValuePair => KeyValuePair.Key ).Select( Key => (CswEnumNbtModuleName) Key ); }
        }

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
        {
            {C3                       , C3},
            {CISPro                   , CISPro},
            {Containers               , Containers},
            {Dev                      , Dev},
            {FireCode                 , FireCode},
            {FireDbSync               , FireDbSync},
            {IMCS                     , IMCS},
            {LOLISync                 , LOLISync },
            {ManufacturerLotInfo      , ManufacturerLotInfo},
            {MLM                      , MLM},
            {MultiInventoryGroup      , MultiInventoryGroup},
            {MultiSite                , MultiSite},
            {NBTManager               , NBTManager},
            {PCIDSync                 , PCIDSync},
            {RegulatoryLists          , RegulatoryLists},
            {SDS                      , SDS},
            {SI                       , SI}
        };

        /// <summary>
        /// The string value of the current instance
        /// </summary>
        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        /// <summary>
        /// The enum constructor
        /// </summary>
        public CswEnumNbtModuleName( string ItemName = CswResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        /// <summary>
        /// Implicit cast to Enum
        /// </summary>
        public static implicit operator CswEnumNbtModuleName( string Val )
        {
            return new CswEnumNbtModuleName( Val );
        }

        /// <summary>
        /// Override of ToString
        /// </summary>
        public override string ToString()
        {
            return Value;
        }

        #endregion

        #region Enum members

        public const string Unknown = "Unknown";

        /// <summary>
        /// Chemical Inventory
        /// </summary>
        public const string CISPro = "CISPro";
        /// <summary>
        /// Development
        /// </summary>
        public const string Dev = "Dev";
        /// <summary>
        /// Material Life-cycle Management
        /// </summary>
        public const string MLM = "MLM";
        /// <summary>
        /// Instrument Maintenance and Calibration
        /// </summary>
        public const string IMCS = "IMCS";
        /// <summary>
        /// NBT Management Application
        /// </summary>
        public const string NBTManager = "NBTManager";
        /// <summary>
        /// Site Inspection
        /// </summary>
        public const string SI = "SI";
        /// <summary>
        /// ChemCatCentral
        /// </summary>
        public const string C3 = "C3";
        /// <summary>
        /// Containers
        /// </summary>
        public const string Containers = "Containers";
        /// <summary>
        /// Regulatory Lists
        /// </summary>
        public const string RegulatoryLists = "Regulatory Lists";
        /// <summary>
        /// Module that allows fire and hazard reporting on containers
        /// </summary>
        public const string FireCode = "Fire Code";
        /// <summary>
        /// Module that allows safety data sheets on materials
        /// </summary>
        public const string SDS = "SDS";
        /// <summary>
        /// Multiple Site NodeTypes
        /// </summary>
        public const string MultiSite = "Multi Site";
        /// <summary>
        /// Multiple Site NodeTypes
        /// </summary>
        public const string MultiInventoryGroup = "Multi Inventory Group";
        /// <summary>
        /// Module that syncs FireDb data with ChemCatCentral.
        /// </summary>
        public const string FireDbSync = "FireDb Sync";
        /// <summary>
        /// Module that syncs PCID data with ChemCatCentral.
        /// </summary>
        public const string PCIDSync = "PCID Sync";
        /// <summary>
        /// Manufacturer Lot Info
        /// </summary>
        public const string ManufacturerLotInfo = "Manufacturer Lot Info";
        /// <summary>
        /// Module that syncs Regulatory List data with LOLI.
        /// </summary>
        public const string LOLISync = "LOLI Sync";

        #endregion

        #region IComparable

        public int CompareTo( CswEnumNbtModuleName other )
        {
            return string.Compare( ToString(), other.ToString(), StringComparison.Ordinal );
        }

        #endregion IComparable

        #region IEquatable (CswEnumNbtModuleName)

        /// <summary>
        /// == Equality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator ==( CswEnumNbtModuleName ft1, CswEnumNbtModuleName ft2 )
        {
            //do a string comparison on the fieldtypes
            return CswConvert.ToString( ft1 ) == CswConvert.ToString( ft2 );
        }

        /// <summary>
        ///  != Inequality operator guarantees we're evaluating instance values
        /// </summary>
        public static bool operator !=( CswEnumNbtModuleName ft1, CswEnumNbtModuleName ft2 )
        {
            return !( ft1 == ft2 );
        }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtModuleName ) )
            {
                return false;
            }
            return this == (CswEnumNbtModuleName) obj;
        }

        /// <summary>
        /// Equals
        /// </summary>
        public bool Equals( CswEnumNbtModuleName obj )
        {
            return this == obj;
        }

        /// <summary>
        /// Get Hash Code
        /// </summary>
        public override int GetHashCode()
        {
            int ret = 23, prime = 37;
            ret = ( ret * prime ) + Value.GetHashCode();
            ret = ( ret * prime ) + _Enums.GetHashCode();
            return ret;
        }

        #endregion IEquatable (CswEnumNbtModuleName)


    } // class CswNbtModule
}// namespace ChemSW.Nbt
