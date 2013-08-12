using System;
using System.Data;
using ChemSW.Core;
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
    } // class CswNbt2DRowRelationship
} // namespace
