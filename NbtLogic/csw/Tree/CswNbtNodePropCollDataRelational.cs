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

        public void update( Int32 NodeTypeId, CswPrimaryKey RelationalId, DataTable PropsTable )
        {
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
                string TableName = NodeType.TableName;
                if( TableName != "nodes" )
                {
                    string PkColumnName = _CswNbtResources.getPrimeKeyColName( TableName );
                    CswTableUpdate CswTableUpdate = null;
                    DataTable DataTable = null;
                    DataRow DataRow = null;

                    // horrible special case for Design
                    // We need to use CswNbtMetaDataResources objects, or else we have dirty-write problems
                    if( NodeType.TableName.StartsWith( "nodetype" ) )
                    {
                        switch( NodeType.TableName )
                        {
                            case "nodetypes":
                                CswTableUpdate = _CswNbtResources.MetaData._CswNbtMetaDataResources.NodeTypeTableUpdate;
                                CswNbtMetaDataNodeType relatedNT = _CswNbtResources.MetaData.getNodeType( RelationalId.PrimaryKey );
                                DataTable = relatedNT._DataRow.Table;
                                DataRow = relatedNT._DataRow;
                                break;
                            case "nodetype_tabset":
                                CswTableUpdate = _CswNbtResources.MetaData._CswNbtMetaDataResources.NodeTypeTabTableUpdate;
                                CswNbtMetaDataNodeTypeTab relatedNTT = _CswNbtResources.MetaData.getNodeTypeTab( RelationalId.PrimaryKey );
                                DataTable = relatedNTT._DataRow.Table;
                                DataRow = relatedNTT._DataRow;
                                break;
                            case "nodetype_props":
                                CswTableUpdate = _CswNbtResources.MetaData._CswNbtMetaDataResources.NodeTypePropTableUpdate;
                                CswNbtMetaDataNodeTypeProp relatedNTP = _CswNbtResources.MetaData.getNodeTypeProp( RelationalId.PrimaryKey );
                                DataTable = relatedNTP._DataRow.Table;
                                DataRow = relatedNTP._DataRow;
                                break;
                        }
                    } // if( NodeType.TableName.StartsWith( "nodetype" ) )

                    if( null == DataTable || null == CswTableUpdate )
                    {
                        CswTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtNodePropCollDataRelational_update", TableName );
                        DataTable = CswTableUpdate.getTable( null, PkColumnName, RelationalId.PrimaryKey, string.Empty, false );
                        DataRow = DataTable.Rows[0];
                    }

                    if( null != DataRow )
                    {
                        CswTableSelect MappingSelect = _CswNbtResources.makeCswTableSelect( "PropCollDataRelational_mapping", "jct_dd_ntp" );
                        DataTable MappingTable = MappingSelect.getTable( "where nodetypepropid in (select nodetypepropid from nodetype_props where nodetypeid =" + NodeTypeId.ToString() + ")" );

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
                                                DataRow[_CswNbtResources.DataDictionary.ColumnName] = DBNull.Value;
                                            }
                                            else if( _CswNbtResources.DataDictionary.ColumnName == "defaultvalueid" )
                                            {
                                                // TODO: Fix defaultvalueid syncing
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

                                                DataRow[_CswNbtResources.DataDictionary.ColumnName] = value; //CurrentRow[CurrentSubField.Column.ToString()];
                                            }
                                        } // if( _CswNbtResources.DataDictionary.ColumnName != string.Empty )
                                    } // if( null != MappingRow )
                                } // foreach( CswNbtSubField CurrentSubField in thisNTP.getFieldTypeRule().SubFields )
                            } // if( null != thisNTP )
                        } // foreach( DataRow CurrentRow in PropsTable.Rows )

                        CswTableUpdate.update( DataTable );
                    } // if( null != DataRow )
                } // if( TableName != "nodes" )
            } // if( CswTools.IsPrimaryKey( RelationalId ) )
        }//update() 


    }//CswNbtNodePropCollDataNative


}//namespace ChemSW.Nbt
