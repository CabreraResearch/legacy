using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdBlobData
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtSdBlobData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void saveFile( string PropIdAttr, byte[] BlobData, string ContentType, string FileName, out string Href )
        {
            CswNbtSdTabsAndProps tabsAndProps = new CswNbtSdTabsAndProps( _CswNbtResources );

            CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
            CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];

            //Save the attribute data to jct_nodes_props
            CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Blobber_save_update", "jct_nodes_props" );
            DataTable JctTable = JctUpdate.getTable( "jctnodepropid", FileProp.JctNodePropId );
            JctTable.Rows[0]["field1"] = FileName;
            JctTable.Rows[0]["field2"] = ContentType;
            JctUpdate.update( JctTable );

            //Save the file to blob_data
            CswTableUpdate BlobUpdate = _CswNbtResources.makeCswTableUpdate( "saveBlob", "blob_data" );
            DataTable BlobTbl = BlobUpdate.getTable( "where jctnodepropid = " + FileProp.JctNodePropId );
            if( BlobTbl.Rows.Count > 0 )
            {
                BlobTbl.Rows[0]["blobdata"] = BlobData;
            }
            else
            {
                DataRow NewRow = BlobTbl.NewRow();
                NewRow["jctnodepropid"] = FileProp.JctNodePropId;
                NewRow["blobdata"] = BlobData;
                BlobTbl.Rows.Add( NewRow );
            }
            BlobUpdate.update( BlobTbl );

            Node.postChanges( false );

            Href = CswNbtNodePropBlob.getLink( FileProp.JctNodePropId, PropId.NodeId, FileProp.JctNodePropId );
        }

    }
}
