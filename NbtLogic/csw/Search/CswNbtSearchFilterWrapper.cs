using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.Search
{

    public sealed class CswNbtSearchFilterType : CswEnum<CswNbtSearchFilterType>
    {
        private CswNbtSearchFilterType( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtSearchFilterType> _All { get { return CswEnum<CswNbtSearchFilterType>.All; } }
        public static explicit operator CswNbtSearchFilterType( string str )
        {
            CswNbtSearchFilterType ret = Parse( str );
            return ( ret != null ) ? ret : CswNbtSearchFilterType.Unknown;
        }
        public static readonly CswNbtSearchFilterType Unknown = new CswNbtSearchFilterType( "Unknown" );

        public static readonly CswNbtSearchFilterType nodetype = new CswNbtSearchFilterType( "nodetype" );
        public static readonly CswNbtSearchFilterType objectclass = new CswNbtSearchFilterType( "objectclass" );
        public static readonly CswNbtSearchFilterType propval = new CswNbtSearchFilterType( "propval" );
    }

    public class CswNbtSearchFilterWrapper : IEquatable<CswNbtSearchFilterWrapper>
    {
        private JObject _FilterObj;
        public const string BlankValue = "[blank]";

        public CswNbtSearchFilterWrapper( JObject FilterObj )
        {
            _FilterObj = FilterObj;
        }
        public CswNbtSearchFilterWrapper( string inFilterName, CswNbtSearchFilterType inType, string inFilterId, string inFilterValue, Int32 inCount, string inIcon, bool inRemoveable, CswNbtSearchPropOrder.PropOrderSourceType inSource )
        {
            _FilterObj = new JObject();

            FilterName = inFilterName;
            Type = inType;
            FilterId = inFilterId;
            FilterValue = inFilterValue;
            Count = inCount;
            Icon = inIcon;
            Removeable = inRemoveable;
            Source = inSource;
        }

        public JObject ToJObject()
        {
            return _FilterObj;
        }

        public string FilterName
        {
            get { return _FilterObj["filtername"].ToString(); }
            set { _FilterObj["filtername"] = value; }
        }
        public CswNbtSearchFilterType Type
        {
            get { return (CswNbtSearchFilterType) _FilterObj["filtertype"].ToString(); }
            set { _FilterObj["filtertype"] = value.ToString(); }
        }
        public string FilterId
        {
            get { return _FilterObj["filterid"].ToString(); }
            set { _FilterObj["filterid"] = value; }
        }
        public Int32 Count
        {
            get { return CswConvert.ToInt32( _FilterObj["count"] ); }
            set { _FilterObj["count"] = value.ToString(); }
        }
        public string Icon
        {
            get { return _FilterObj["icon"].ToString(); }
            set { _FilterObj["icon"] = value; }
        }
        public bool Removeable
        {
            get { return CswConvert.ToBoolean( _FilterObj["removeable"] ); }
            set { _FilterObj["removeable"] = value; }
        }
        public CswNbtSearchPropOrder.PropOrderSourceType Source
        {
            get { return (CswNbtSearchPropOrder.PropOrderSourceType) _FilterObj["source"].ToString(); }
            set { _FilterObj["source"] = value.ToString(); }
        }
        public string FilterValue
        {
            get { return _FilterObj["filtervalue"].ToString(); }
            set
            {
                if( value != string.Empty )
                {
                    _FilterObj["filtervalue"] = value;
                }
                else
                {
                    _FilterObj["filtervalue"] = BlankValue;
                }
            }
        } // FilterValue

        #region for NodeType Filters

        public Int32 FirstVersionId
        {
            get { return CswConvert.ToInt32( _FilterObj["firstversionid"] ); }
            set { _FilterObj["firstversionid"] = value.ToString(); }
        }

        #endregion for NodeType Filters

        #region for Object Class Filters

        public Int32 ObjectClassId
        {
            get { return CswConvert.ToInt32( _FilterObj["objectclassid"] ); }
            set { _FilterObj["objectclassid"] = value.ToString(); }
        }

        #endregion for Object Class Filters

        #region for PropVal Filters

        public Int32 FirstPropVersionId
        {
            get { return CswConvert.ToInt32( _FilterObj["firstpropversionid"] ); }
            set { _FilterObj["firstpropversionid"] = value.ToString(); }
        }

        #endregion for PropVal Filters

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtSearchFilterWrapper w1, CswNbtSearchFilterWrapper w2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( w1, w2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) w1 == null ) || ( (object) w2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( w1.Type == w2.Type &&
                w1.FilterId == w2.FilterId )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtSearchFilterWrapper w1, CswNbtSearchFilterWrapper w2 )
        {
            return !( w1 == w2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtSearchFilterWrapper ) )
                return false;
            return this == (CswNbtSearchFilterWrapper) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtSearchFilterWrapper obj )
        {
            return this == (CswNbtSearchFilterWrapper) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        #endregion IEquatable


    } // class CswNbtSearchFilterWrapper
} // namespace ChemSW.Nbt.Search

