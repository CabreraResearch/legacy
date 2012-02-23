using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtSubField
    {
        public sealed class PropColumn : CswEnum<PropColumn>
        {
            private PropColumn( String Name ) : base( Name ) { }
            public static IEnumerable<PropColumn> _All { get { return CswEnum<PropColumn>.All; } }
            public static explicit operator PropColumn( string str )
            {
                PropColumn ret = Parse( str );
                return ( ret != null ) ? ret : PropColumn.Unknown;
            }
            public static readonly PropColumn Unknown = new PropColumn( "Unknown" );

            public static readonly PropColumn Field1 = new PropColumn( "Field1" );
            public static readonly PropColumn Field1_FK = new PropColumn( "Field1_FK" );
            public static readonly PropColumn Field1_Date = new PropColumn( "Field1_Date" );
            public static readonly PropColumn Field1_Numeric = new PropColumn( "Field1_Numeric" );
            public static readonly PropColumn Field2_Numeric = new PropColumn( "Field2_Numeric" );
            public static readonly PropColumn Field2 = new PropColumn( "Field2" );
            public static readonly PropColumn Field2_Date = new PropColumn( "Field2_Date" );
            public static readonly PropColumn Field3 = new PropColumn( "Field3" );
            public static readonly PropColumn Field4 = new PropColumn( "Field4" );
            public static readonly PropColumn Field5 = new PropColumn( "Field5" );
            public static readonly PropColumn Gestalt = new PropColumn( "Gestalt" ); //bz # 6628
            public static readonly PropColumn ClobData = new PropColumn( "ClobData" );
            public static readonly PropColumn ReadOnly = new PropColumn( "ReadOnly" );
            public static readonly PropColumn PendingUpdate = new PropColumn( "PendingUpdate" );
        }

        /// <summary>
        /// Names of SubFields of Properties
        /// There are SERIOUS repercussions if these are changed!  Beware!  (e.g. import)
        /// </summary>
        public sealed class SubFieldName : CswEnum<SubFieldName>
        {
            private SubFieldName( String Name ) : base( Name ) { }
            public static IEnumerable<SubFieldName> _All { get { return CswEnum<SubFieldName>.All; } }
            public static explicit operator SubFieldName( string str )
            {
                SubFieldName ret = Parse( str );
                return ( ret != null ) ? ret : SubFieldName.Unknown;
            }
            public static readonly SubFieldName Unknown = new SubFieldName( "Unknown" );

            public static readonly SubFieldName AllowedAnswers = new SubFieldName( "AllowedAnswers" );
            public static readonly SubFieldName Answer = new SubFieldName( "Answer" );
            public static readonly SubFieldName Barcode = new SubFieldName( "Barcode" );
            public static readonly SubFieldName Blob = new SubFieldName( "Blob" );
            public static readonly SubFieldName Checked = new SubFieldName( "Checked" );
            public static readonly SubFieldName Column = new SubFieldName( "Column" );
            public static readonly SubFieldName Comments = new SubFieldName( "Comments" );
            public static readonly SubFieldName CompliantAnswers = new SubFieldName( "CompliantAnswers" );
            public static readonly SubFieldName ContentType = new SubFieldName( "ContentType" );
            public static readonly SubFieldName CorrectiveAction = new SubFieldName( "CorrectiveAction" );
            public static readonly SubFieldName DateAnswered = new SubFieldName( "DateAnswered" );
            public static readonly SubFieldName DateCorrected = new SubFieldName( "DateCorrected" );
            public static readonly SubFieldName Href = new SubFieldName( "Href" );
            public static readonly SubFieldName Image = new SubFieldName( "Image" );
            public static readonly SubFieldName Interval = new SubFieldName( "Interval" );
            public static readonly SubFieldName IsCompliant = new SubFieldName( "IsCompliant" );
            public static readonly SubFieldName Mol = new SubFieldName( "Mol" );
            public static readonly SubFieldName Name = new SubFieldName( "Name" );
            public static readonly SubFieldName NodeID = new SubFieldName( "NodeID" );
            public static readonly SubFieldName NodeType = new SubFieldName( "NodeType" );
            public static readonly SubFieldName Number = new SubFieldName( "Number" );
            public static readonly SubFieldName Password = new SubFieldName( "Password" );
            public static readonly SubFieldName Path = new SubFieldName( "Path" );
            public static readonly SubFieldName Required = new SubFieldName( "Required" );
            public static readonly SubFieldName Row = new SubFieldName( "Row" );
            public static readonly SubFieldName Sequence = new SubFieldName( "Sequence" );
            public static readonly SubFieldName StartDateTime = new SubFieldName( "StartDateTime" );
            public static readonly SubFieldName Text = new SubFieldName( "Text" );
            public static readonly SubFieldName Units = new SubFieldName( "Units" );
            public static readonly SubFieldName Value = new SubFieldName( "Value" );
            public static readonly SubFieldName ViewID = new SubFieldName( "ViewID" );
            public static readonly SubFieldName ChangedDate = new SubFieldName( "ChangedDate" );
            public static readonly SubFieldName Base = new SubFieldName( "Base" );
            public static readonly SubFieldName Exponent = new SubFieldName( "Exponent" );
            public static readonly SubFieldName Health = new SubFieldName( "Health" );
            public static readonly SubFieldName Flammability = new SubFieldName( "Flammability" );
            public static readonly SubFieldName Reactivity = new SubFieldName( "Reactivity" );
            public static readonly SubFieldName Special = new SubFieldName( "Special" );
        } // class SubFieldName

        public SubFieldName Name = SubFieldName.Value;
        public string Table = string.Empty;
        public PropColumn Column = PropColumn.Unknown;
        public string RelationalTable = string.Empty;
        public string RelationalColumn = string.Empty;
        private CswNbtFieldResources _CswNbtFieldResources;
        public bool isReportable;

        //public CswNbtSubField( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp, PropColumn DefaultColumn, SubFieldName SubFieldName )
        public CswNbtSubField( CswNbtFieldResources CswNbtFieldResources, PropColumn DefaultColumn, SubFieldName SubFieldName, bool Reportable = false )
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

        public Collection<CswNbtPropFilterSql.PropertyFilterMode> _FilterModes = new Collection<CswNbtPropFilterSql.PropertyFilterMode>();
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
                return _FilterModes;
                //Collection<CswNbtPropFilterSql.PropertyFilterMode> ReturnVal = new Collection<CswNbtPropFilterSql.PropertyFilterMode>();

                //Type enumType = typeof( CswNbtPropFilterSql.PropertyFilterMode );
                //Array AllFilterModes = Enum.GetValues( enumType );

                //foreach( CswNbtPropFilterSql.PropertyFilterMode CurrentFilterMode in AllFilterModes )
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
