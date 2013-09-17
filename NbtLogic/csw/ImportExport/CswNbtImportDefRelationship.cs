using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDefRelationship
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly DataRow _row;

        public CswNbtImportDefRelationship( CswNbtResources CswNbtResources, DataRow RelRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = RelRow;
        }

        public Int32 ImportRelationshipId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefRelationships.importdefrelationshipid] ); }
        }
        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefRelationships.importdefid] ); }
        }
        public string NodeTypeName
        {
            get { return _row[CswNbtImportTables.ImportDefRelationships.nodetypename].ToString(); }
        }
        public string RelationshipName
        {
            get { return _row[CswNbtImportTables.ImportDefRelationships.relationship].ToString(); }
        }
        public Int32 Instance
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefRelationships.instance] ); }
        }
        public string SourceRelColumnName
        {
            get { return _row[CswNbtImportTables.ImportDefRelationships.sourcerelcolumnname].ToString(); }
        }

        public CswNbtMetaDataNodeType NodeType
        {
            get { return _CswNbtResources.MetaData.getNodeType( NodeTypeName ); }
        }

        public CswNbtMetaDataNodeTypeProp Relationship
        {
            get
            {
                CswNbtMetaDataNodeTypeProp ret = null;
                if( null != NodeType )
                {
                    ret = NodeType.getNodeTypeProp( RelationshipName );
                }
                return ret;
            }
        }


        /// <summary>
        /// Get a DataTable to fill out, for use with addRelationshipEntries()
        /// </summary>
        /// <returns></returns>
        public static DataTable getDataTableForNewRelationshipEntries()
        {
            DataTable Table = new DataTable();
            Table.Columns.Add( "sheet" );
            Table.Columns.Add( "nodetype" );
            Table.Columns.Add( "relationship" );
            Table.Columns.Add( "instance" );
            Table.Columns.Add( "sourcerelcolumnname" );
            return Table;
        }

        /// <summary>
        /// Add new Relationship entries to a definition (for use by CswNbtImporter)
        /// </summary>
        public static void addRelationshipEntries( CswNbtResources CswNbtResources, DataTable RelationshipsDataTable, Dictionary<string, Int32> DefIdsBySheetName )
        {
            CswTableUpdate importRelationshipsUpdate = CswNbtResources.makeCswTableUpdate( "storeDefinition_Relationships_update", CswNbtImportTables.ImportDefRelationships.TableName );
            DataTable importRelationshipsTable = importRelationshipsUpdate.getEmptyTable();

            foreach( DataRow RelRow in RelationshipsDataTable.Rows )
            {
                string SheetName = RelRow["sheet"].ToString();
                if( false == string.IsNullOrEmpty( SheetName ) )
                {
                    string NodeTypeName = RelRow["nodetype"].ToString();
                    CswNbtMetaDataNodeType NodeType = CswNbtResources.MetaData.getNodeType( NodeTypeName );
                    if( null != NodeType )
                    {
                        string RelationshipName = RelRow["relationship"].ToString();
                        CswNbtMetaDataNodeTypeProp Relationship = NodeType.getNodeTypeProp( RelationshipName );
                        if( null != Relationship )
                        {
                            DataRow row = importRelationshipsTable.NewRow();
                            row[CswNbtImportTables.ImportDefRelationships.importdefid] = DefIdsBySheetName[SheetName];
                            row[CswNbtImportTables.ImportDefRelationships.nodetypename] = NodeTypeName;
                            row[CswNbtImportTables.ImportDefRelationships.relationship] = RelationshipName;
                            row[CswNbtImportTables.ImportDefRelationships.instance] = CswConvert.ToDbVal( RelRow["instance"].ToString() );
                            if( RelRow.Table.Columns.Contains( "sourcerelcolumnname" ) )
                            {
                                row[CswNbtImportTables.ImportDefRelationships.sourcerelcolumnname] = RelRow["sourcerelcolumnname"].ToString();
                            }
                            importRelationshipsTable.Rows.Add( row );

                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid Relationship defined in 'Relationships' sheet: " + RelRow["relationship"].ToString() + " (nodetype: " + NodeTypeName + ")" );
                        }
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid NodeType defined in 'Relationships' sheet: " + NodeTypeName );
                    }
                }
            } // foreach( DataRow RelRow in RelationshipsDataTable.Rows )
            importRelationshipsUpdate.update( importRelationshipsTable );
        } // addRelationshipEntries()

    } // class CswNbt2DRowRelationship
} // namespace
