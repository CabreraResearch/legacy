using System;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropCollDataRelational //: ICswNbtNodePropCollData
    {
        private CswNbtResources _CswNbtResources = null;
        public CswNbtNodePropCollDataRelational( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor



        //private DataTable _PropsTable = null;
        //public DataTable PropsTable
        //{
        //    get
        //    {

        //        if( null == _PropsTable )
        //        {
        //            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
        //            CswTableSelect CswTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodePropCollDataRelational_PropsTable", NodeType.TableName );
        //            string FilterColumn = _CswNbtResources.getPrimeKeyColName( NodeType.TableName );
        //            CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
        //            foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.getNodeTypeProps() )
        //            {
        //                foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.getFieldTypeRule().SubFields )
        //                {
        //                    if( CurrentSubField.RelationalColumn != string.Empty )
        //                        SelectColumns.Add( CurrentSubField.RelationalColumn.ToLower() );
        //                }
        //            }//iterate node type props to set up select columns

        //            DataTable DataTable = null;
        //            if( _NodePk != null )
        //                DataTable = CswTableSelect.getTable( SelectColumns, FilterColumn, _NodePk.PrimaryKey, string.Empty, false );
        //            else
        //                DataTable = CswTableSelect.getEmptyTable();

        //            _PropsTable = makeEmptyJctNodesProps();
        //            foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.getNodeTypeProps() )
        //            {
        //                DataRow NewRow = _PropsTable.NewRow();
        //                NewRow["nodetypepropid"] = CurrentNodeTypeProp.PropId.ToString();
        //                if( _NodePk != null )
        //                {
        //                    NewRow["nodeid"] = CswConvert.ToDbVal( _NodePk.PrimaryKey );
        //                    NewRow["nodeidtablename"] = _NodePk.TableName;
        //                }
        //                foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.getFieldTypeRule().SubFields )
        //                {
        //                    if( DataTable.Rows.Count > 0 && CurrentSubField.RelationalColumn != string.Empty )
        //                        NewRow[CurrentSubField.Column.ToString()] = DataTable.Rows[0][CurrentSubField.RelationalColumn];
        //                }
        //                _PropsTable.Rows.Add( NewRow );
        //            }//iterate node type props

        //        } // if( null == _PropsTable )

        //        return ( _PropsTable );
        //    }//get
        //}//PropsTable

        //Int32 _NodeTypeId = Int32.MinValue;
        //public Int32 NodeTypeId
        //{
        //    set
        //    {
        //        _NodeTypeId = value;
        //    }

        //    get
        //    {
        //        return ( _NodeTypeId );
        //    }
        //}


        //CswPrimaryKey _NodePk = null;
        //public CswPrimaryKey NodePk
        //{
        //    set
        //    {
        //        _NodePk = value;
        //    }

        //    get
        //    {
        //        return ( _NodePk );
        //    }
        //}//NodeKey


        //private DataTable makeEmptyJctNodesProps()
        //{
        //    return ( _CswNbtResources.makeCswTableSelect( "makeEmptyJctNodesProps_select", "jct_nodes_props" ).getEmptyTableNoEvents() );
        //}//makeEmptyJctNodesProps

        //This table has to look exactly like the jct_nodes_props table because it 
        //serves as the core of the property collection and individual property 
        //data. But this class will have to transmogrify said table's data into 
        //the format of the 


        //public bool IsTableEmpty { get { return ( null == _PropsTable ); } }

        //public void refreshTable()
        //{
        //    _PropsTable = null;

        //}//refreshTable()

        //void fillFromCswPrimaryKey( CswPrimaryKey CswPrimaryKey, Int32 NodeTypeId )
        //{
        //}//fillFromCswPrimaryKey()




        public void update( Int32 NodeTypeId, CswPrimaryKey RelationalId, DataTable PropsTable )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswTableUpdate CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtNodePropCollDataRelational_update", NodeType.TableName );
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( NodeType.TableName );

            CswCommaDelimitedString SelectColumns = null; // new CswCommaDelimitedString();
            //foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.getNodeTypeProps() )
            //{
            //    foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.getFieldTypeRule().SubFields )
            //    {
            //        if( CurrentSubField.RelationalColumn != string.Empty )
            //            SelectColumns.Add( CurrentSubField.RelationalColumn );
            //    }
            //}//iterate node type props to set up select columns

            DataTable DataTable = CswTableUpdate.getTable( SelectColumns, PkColumnName, RelationalId.PrimaryKey, string.Empty, false );

            CswTableSelect MappingSelect = _CswNbtResources.makeCswTableSelect( "PropCollDataRelational_mapping", "jct_dd_ntp" );
            DataTable MappingTable = MappingSelect.getTable( "where nodetypepropid in (select nodetypepropid from nodetype_props where nodetypeid =" + NodeTypeId.ToString() + ")" );

            ////test
            //string ColumnNames = string.Empty;
            //foreach ( DataColumn CurrentColumn in DataTable.Columns )
            //{
            //    ColumnNames += CurrentColumn.ColumnName + "\r\n";
            //}

            foreach( DataRow CurrentRow in PropsTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp thisNTP = NodeType.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );
                if( null != thisNTP )
                {
                    foreach( CswNbtSubField CurrentSubField in thisNTP.getFieldTypeRule().SubFields )
                    {
                        DataRow MappingRow = MappingTable.Rows.Cast<DataRow>()
                                                         .FirstOrDefault( r => CswConvert.ToInt32( r["nodetypepropid"] ) == thisNTP.PropId &&
                                                                               r["subfieldname"].ToString() == CurrentSubField.Name.ToString() );
                        if( null != MappingRow )
                        {
                            _CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( MappingRow["datadictionaryid"] ) );
                            if( _CswNbtResources.DataDictionary.ColumnName != string.Empty )
                            {
                                if( CurrentRow[CurrentSubField.Column.ToString()].ToString() == string.Empty )
                                {
                                    DataTable.Rows[0][_CswNbtResources.DataDictionary.ColumnName] = DBNull.Value;
                                }
                                else
                                {
                                    object value = CurrentRow[CurrentSubField.Column.ToString()];

                                    // Special case for booleans and tristates
                                    if( thisNTP.getFieldTypeValue() == CswEnumNbtFieldType.Logical )
                                    {
                                        value = CswConvert.ToDbVal( CswConvert.ToTristate( CurrentRow[CurrentSubField.Column.ToString()] ) );
                                    }
                                    // Special case for relationships and locations, if the related entity is also relational
                                    if( CurrentSubField.Name == CswEnumNbtSubFieldName.NodeID &&
                                        ( thisNTP.getFieldTypeValue() == CswEnumNbtFieldType.Relationship ||
                                          thisNTP.getFieldTypeValue() == CswEnumNbtFieldType.Location ) )
                                    {
                                        CswNbtNode RelatedNode = _CswNbtResources.Nodes[new CswPrimaryKey( "nodes", CswConvert.ToInt32( value ) )];
                                        if( null != RelatedNode && RelatedNode.getNodeType().DoRelationalSync )
                                        {
                                            // Remap the foreign key reference to the relational primary key
                                            value = RelatedNode.RelationalId.PrimaryKey;
                                        }
                                    }

                                    DataTable.Rows[0][_CswNbtResources.DataDictionary.ColumnName] = value; //CurrentRow[CurrentSubField.Column.ToString()];
                                }
                            }
                        }
                    }
                }
            }

            CswTableUpdate.update( DataTable );

        }//update() 


    }//CswNbtNodePropCollDataNative


}//namespace ChemSW.Nbt
