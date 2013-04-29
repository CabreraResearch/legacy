using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtSubField
    {
        public CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Value;
        public string Table = string.Empty;
        public CswEnumNbtPropColumn Column = CswEnumNbtPropColumn.Unknown;
        //public string RelationalTable = string.Empty;
        //public string RelationalColumn = string.Empty;
        private CswNbtFieldResources _CswNbtFieldResources;
        public bool isReportable;

        //public CswNbtSubField( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp, PropColumn DefaultColumn, SubFieldName SubFieldName )
        public CswNbtSubField( CswNbtFieldResources CswNbtFieldResources, CswEnumNbtPropColumn DefaultColumn, CswEnumNbtSubFieldName SubFieldName, bool Reportable = false )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            Name = SubFieldName;
            Table = "jct_nodes_props";  // default
            Column = DefaultColumn;
            isReportable = Reportable;

            //BZ 9139 - CswNbtMetaDataResources handles this now
            //if( MetaDataProp is CswNbtMetaDataNodeTypeProp )
            //{
            //    CswNbtMetaDataNodeTypeProp NodeTypeProp = (CswNbtMetaDataNodeTypeProp) MetaDataProp;
            //    if( NodeTypeProp.PropId != Int32.MinValue )
            //    {
            //        // This is a candidate for performance refactoring...later.
            //        CswQueryCaddy Caddy = CswNbtFieldResources.CswNbtResources.makeCswQueryCaddy( "getTableAndColumnForProp" );
            //        Caddy.S4Parameters.Add( "nodetypepropid", NodeTypeProp.PropId.ToString() );
            //        Caddy.S4Parameters.Add( "subfieldname", Name.ToString() );
            //        DataTable JctTable = Caddy.Table;
            //        if( JctTable.Rows.Count > 0 )
            //        {
            //            RelationalTable = JctTable.Rows[0]["tablename"].ToString();
            //            RelationalColumn = JctTable.Rows[0]["columnname"].ToString();
            //        }
            //    }
            //}
        }

        public Collection<CswEnumNbtFilterMode> _FilterModes = new Collection<CswEnumNbtFilterMode>();
        public CswEnumNbtFilterMode DefaultFilterMode
        {
            get
            {
                return SupportedFilterModes.First(); //CswEnumNbtFilterMode.Begins;      
            }
        }

        public Collection<CswEnumNbtFilterMode> SupportedFilterModes
        {
            get
            {
                return _FilterModes;
                //Collection<CswEnumNbtFilterMode> ReturnVal = new Collection<CswEnumNbtFilterMode>();

                //Type enumType = typeof( CswEnumNbtFilterMode );
                //Array AllFilterModes = Enum.GetValues( enumType );

                //foreach( CswEnumNbtFilterMode CurrentFilterMode in AllFilterModes )
                //{
                //    if( CurrentFilterMode == ( CurrentFilterMode & FilterModes ) )
                //    {
                //        ReturnVal.Add( CurrentFilterMode );
                //    }
                //}//iterate all filter modes

                //return ( ReturnVal );
            }//get

        }//FilterModes

        public string ToXmlNodeName( bool ToLower = false )
        {
            // case 20371 - In the NBT property importer, need to distinguish between NodeID (the subfield) and nodeid (the pk column)
            //return this.Name.ToString().ToLower();
            string ret = this.Name.ToString();
            if( ToLower )
            {
                ret = ret.ToLower();
            }
            return ret;
        }

    }//CswNbtSubField

}//namespace ChemSW.Nbt.MetaData
