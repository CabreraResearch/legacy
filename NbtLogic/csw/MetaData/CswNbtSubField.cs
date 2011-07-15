using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChemSW.Nbt.MetaData
{


    public class CswNbtSubField
    {

        public enum PropColumn
        {
            Field1,
            Field1_FK,
            Field1_Date,
            Field1_Numeric,
            Field2,
            Field2_Date,
            Field3,
            Field4,
            Field5,
            Gestalt, //bz # 6628
            ClobData,
            ReadOnly,
            PendingUpdate,
            Unknown
        };

        /// <summary>
        /// Names of SubFields of Properties
        /// There are SERIOUS repercussions if these are changed!  Beware!
        /// </summary>
        public enum SubFieldName
        {
            Unknown, // generates an exception when run
            AllowedAnswers,
            Answer,
            Barcode,
            Blob,
            Checked,
            Column,
            Comments,
            CompliantAnswers,
            ContentType,
            CorrectiveAction,
            DateAnswered,
            DateCorrected,
            Href,
            Image,
            Interval,
            IsCompliant,
            Mol,
            Name,
            NodeID,
            NodeType,
            Number,
            Password,
            Path,
            Required,
            Row,
            Sequence,
            StartDateTime,
            Text,
            Units,
            Value,
            ViewID,
            ChangedDate
        }

        public SubFieldName Name = SubFieldName.Value;
        public string Table = string.Empty;
        public PropColumn Column = PropColumn.Unknown;
        public string RelationalTable = string.Empty;
        public string RelationalColumn = string.Empty;
        private CswNbtFieldResources _CswNbtFieldResources;

        public CswNbtSubField( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp, PropColumn DefaultColumn, SubFieldName SubFieldName )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            Name = SubFieldName;
            Table = "jct_nodes_props";  // default
            Column = DefaultColumn;

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

        public CswNbtPropFilterSql.PropertyFilterMode FilterModes;
        public CswNbtPropFilterSql.PropertyFilterMode DefaultFilterMode
        {
            get
            {
                return SupportedFilterModes.First(); //CswNbtPropFilterSql.PropertyFilterMode.Begins;      
            }
        }

        public Collection<CswNbtPropFilterSql.PropertyFilterMode> SupportedFilterModes
        {
            get
            {
                Collection<CswNbtPropFilterSql.PropertyFilterMode> ReturnVal = new Collection<CswNbtPropFilterSql.PropertyFilterMode>();

                Type enumType = typeof( CswNbtPropFilterSql.PropertyFilterMode );
                Array AllFilterModes = Enum.GetValues( enumType );

                foreach( CswNbtPropFilterSql.PropertyFilterMode CurrentFilterMode in AllFilterModes )
                {
                    if( CurrentFilterMode == ( CurrentFilterMode & FilterModes ) )
                    {
                        ReturnVal.Add( CurrentFilterMode );
                    }
                }//iterate all filter modes

                return ( ReturnVal );
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
