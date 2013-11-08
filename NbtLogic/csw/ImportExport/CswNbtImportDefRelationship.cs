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
        /// Add new Relationship entries to a definition (for use by CswNbtImporter)
        /// </summary>
        public static void addRelationshipEntries( CswNbtResources CswNbtResources, DataTable RelationshipsDataTable, Dictionary<string, Int32> DefIdsBySheetName )
        {
            CswTableUpdate importRelationshipsUpdate = CswNbtResources.makeCswTableUpdate( "storeDefinition_Relationships_update", CswNbtImportTables.ImportDefRelationships.TableName );

            foreach( DataRow RelRow in RelationshipsDataTable.Rows )
            {

                            //set blank instances to min value
                            if( RelRow["instance"] == DBNull.Value || String.IsNullOrEmpty( RelRow["instance"].ToString() ) )
                            {
                                RelRow["instance"] = Int32.MinValue;
                            }

                string NodeTypeName = RelRow["nodetypename"].ToString();
                string RelationshipName = RelRow["relationship"].ToString();
                CswNbtMetaDataNodeType NodeType = CswNbtResources.MetaData.getNodeType( NodeTypeName );
                CswNbtMetaDataNodeTypeProp Relationship = NodeType.getNodeTypeProp( RelationshipName );
                if( null == NodeType )
                            {
                    throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid NodeType defined in 'Relationships' sheet: " + NodeTypeName );
                            }
                else if( null == Relationship )
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "Error reading bindings", "Invalid Relationship defined in 'Relationships' sheet: " + RelRow["relationship"].ToString() + " (nodetype: " + NodeTypeName + ")" );
                        }

            } // foreach( DataRow RelRow in RelationshipsDataTable.Rows )

            importRelationshipsUpdate.update( RelationshipsDataTable );

        } // addRelationshipEntries()

    } // class CswNbt2DRowRelationship
} // namespace
