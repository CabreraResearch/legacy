using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropCollDataRelational : ICswNbtNodePropCollData
    {
        private CswNbtResources _CswNbtResources = null;
        private CswNbtNode _Node;
        public CswNbtNodePropCollDataRelational( CswNbtResources CswNbtResources, CswNbtNode Node )
        {
            _CswNbtResources = CswNbtResources;
            _Node = Node;
        }//ctor



        private DataTable _PropsTable = null;
        public DataTable PropsTable
        {
            get
            {

                if( null == _PropsTable )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( _Node.NodeTypeId );
                    CswTableSelect CswTableSelect = _CswNbtResources.makeCswTableSelect( "CswNbtNodePropCollDataRelational_PropsTable", NodeType.TableName );
                    string FilterColumn = _CswNbtResources.getPrimeKeyColName( NodeType.TableName );
                    CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                    foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.getNodeTypeProps() )
                    {
                        foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.getFieldTypeRule().SubFields )
                        {
                            if( CurrentSubField.RelationalColumn != string.Empty )
                                SelectColumns.Add( CurrentSubField.RelationalColumn.ToLower() );
                        }
                    }//iterate node type props to set up select columns

                    DataTable DataTable = null;
                    if( _Node.NodeId != null )
                        DataTable = CswTableSelect.getTable( SelectColumns, FilterColumn, _Node.NodeId.PrimaryKey, string.Empty, false );
                    else
                        DataTable = CswTableSelect.getEmptyTable();

                    _PropsTable = makeEmptyJctNodesProps();
                    foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.getNodeTypeProps() )
                    {
                        DataRow NewRow = _PropsTable.NewRow();
                        NewRow["nodetypepropid"] = CurrentNodeTypeProp.PropId.ToString();
                        if( _Node.NodeId != null )
                        {
                            NewRow["nodeid"] = CswConvert.ToDbVal( _Node.NodeId.PrimaryKey );
                            NewRow["nodeidtablename"] = _Node.NodeId.TableName;
                        }
                        foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.getFieldTypeRule().SubFields )
                        {
                            if( DataTable.Rows.Count > 0 && CurrentSubField.RelationalColumn != string.Empty )
                                NewRow[CurrentSubField.Column.ToString()] = DataTable.Rows[0][CurrentSubField.RelationalColumn];
                        }
                        _PropsTable.Rows.Add( NewRow );
                    }//iterate node type props

                } // if( null == _PropsTable )

                return ( _PropsTable );
            }//get
        }//PropsTable



        private DataTable makeEmptyJctNodesProps()
        {
            return ( _CswNbtResources.makeCswTableSelect( "makeEmptyJctNodesProps_select", "jct_nodes_props" ).getEmptyTableNoEvents() );
        }//makeEmptyJctNodesProps

        //This table has to look exactly like the jct_nodes_props table because it 
        //serves as the core of the property collection and individual property 
        //data. But this class will have to transmogrify said table's data into 
        //the format of the 


        public bool IsTableEmpty { get { return ( null == _PropsTable ); } }

        public void refreshTable()
        {
            _PropsTable = null;

        }//refreshTable()

        //void fillFromCswPrimaryKey( CswPrimaryKey CswPrimaryKey, Int32 NodeTypeId )
        //{
        //}//fillFromCswPrimaryKey()




        public void update( bool AllowAuditing )
        {
            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( _Node.NodeTypeId );
            CswTableUpdate CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtNodePropCollDataRelational_update", NodeType.TableName );
            string PkColumnName = _CswNbtResources.getPrimeKeyColName( NodeType.TableName );

            CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
            foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.getNodeTypeProps() )
            {
                foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.getFieldTypeRule().SubFields )
                {
                    if( CurrentSubField.RelationalColumn != string.Empty )
                        SelectColumns.Add( CurrentSubField.RelationalColumn );
                }
            }//iterate node type props to set up select columns

            DataTable DataTable = CswTableUpdate.getTable( SelectColumns, PkColumnName, _Node.NodeId.PrimaryKey, string.Empty, false );


            //test
            string ColumnNames = string.Empty;
            foreach( DataColumn CurrentColumn in DataTable.Columns )
            {
                ColumnNames += CurrentColumn.ColumnName + "\r\n";
            }

            foreach( DataRow CurrentRow in _PropsTable.Rows )
            {
                CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp = NodeType.getNodeTypeProp( CswConvert.ToInt32( CurrentRow["nodetypepropid"] ) );
                foreach( CswNbtSubField CurrentSubField in CswNbtMetaDataNodeTypeProp.getFieldTypeRule().SubFields )
                {
                    if( CurrentSubField.RelationalColumn != string.Empty )
                    {
                        if( CurrentRow[CurrentSubField.Column.ToString()].ToString() == string.Empty )
                        {
                            DataTable.Rows[0][CurrentSubField.RelationalColumn] = DBNull.Value;
                        }
                        else
                        {
                            DataTable.Rows[0][CurrentSubField.RelationalColumn] = CurrentRow[CurrentSubField.Column.ToString()];
                        }
                    }
                }
            }

            CswTableUpdate.update( DataTable );

        }//update() 


    }//CswNbtNodePropCollDataNative


}//namespace ChemSW.Nbt
