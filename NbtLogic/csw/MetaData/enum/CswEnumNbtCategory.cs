using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.MetaData
{
    public sealed class CswEnumNbtCategory : IEquatable<CswEnumNbtCategory>, IComparable<CswEnumNbtCategory>
    {

        private static Dictionary<string, string> _Enums = new Dictionary<string, string>( StringComparer.OrdinalIgnoreCase )
            {
                {CISProConfiguration, CISProConfiguration}, 
                {Containers, Containers},
                {Dispenses, Dispenses}, 
                {Equipment, Equipment},
                {ForReports, ForReports}, 
                {Inspections, Inspections}, 
                {LabSafetyDemo, LabSafetyDemo},
                {Locations, Locations}, 
                {Materials, Materials}, 
                {MLM, MLM},
                {MLMDemo, MLMDemo},
                {Problems, Problems}, 
                {Requests, Requests},
                {System, System},
                {Tasks, Tasks},
                {Units, Units}
            };

        public readonly string Value;

        private static string _Parse( string Val )
        {
            string ret = CswNbtResources.UnknownEnum;
            if( _Enums.ContainsKey( Val ) )
            {
                ret = _Enums[Val];
            }
            return ret;
        }

        public CswEnumNbtCategory( string ItemName = CswNbtResources.UnknownEnum )
        {
            Value = _Parse( ItemName );
        }

        public static implicit operator CswEnumNbtCategory( string Val )
        {
            return new CswEnumNbtCategory( Val );
        }

        public static implicit operator string( CswEnumNbtCategory item )
        {
            return item.Value;
        }

        public override string ToString()
        {
            return Value;
        }
        
        public const string CISProConfiguration = "CISPro Configuration";
        public const string Containers = "Containers";
        public const string Dispenses = "Dispenses";
        public const string Equipment = "Equipment";
        public const string ForReports = "For Reports";
        public const string Inspections = "Inspections";
        public const string LabSafetyDemo = "Lab Safety (demo)";
        public const string Locations = "Locations";
        public const string Materials = "Materials";
        public const string MLM = "MLM";
        public const string MLMDemo = "MLM (demo)";
        public const string Problems = "Problems";
        public const string Requests = "Requests";
        public const string System = "System";
        public const string Tasks = "Tasks";
        public const string Units = "Units";

        #region IEquatable (NbtFieldType)

        public static bool operator ==( CswEnumNbtCategory ft1, CswEnumNbtCategory ft2 )
        {
            //do a string comparison on the fieldtypes
            return ft1.ToString() == ft2.ToString();
        }

        public static bool operator !=( CswEnumNbtCategory ft1, CswEnumNbtCategory ft2 )
        {
            return !( ft1 == ft2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswEnumNbtCategory ) )
                return false;
            return this == (CswEnumNbtCategory) obj;
        }

        public bool Equals( CswEnumNbtCategory obj )
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

        #endregion IEquatable (NbtFieldType)

        #region IComparable (NbtFieldType)

        public int CompareTo( CswEnumNbtCategory other )
        {
            return this.ToString().CompareTo( other.ToString() );
        }

        #endregion IComparable (NbtFieldType)
    }
}
