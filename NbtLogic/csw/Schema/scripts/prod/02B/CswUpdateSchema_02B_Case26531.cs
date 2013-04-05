using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26531
    /// </summary>
    public class CswUpdateSchema_02B_Case26531: CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 26531; }
        }

        public override void update()
        {

            //Move blob data from jct_nodes_props to blob_data
            CswNbtMetaDataFieldType molFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.MOL );
            CswNbtMetaDataFieldType imageFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Image );
            CswNbtMetaDataFieldType fileFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.File );
            string sql = "select distinct nodetypeid from nodetype_props where fieldtypeid in (" + molFT.FieldTypeId + "," + imageFT.FieldTypeId + "," + fileFT.FieldTypeId + ")";

            CswArbitrarySelect arbSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "findBlobDataNTs", sql );
            DataTable nodeTypes = arbSelect.getTable();
            foreach( DataRow Row in nodeTypes.Rows )
            {
                int NodeTypeId = CswConvert.ToInt32( Row["nodetypeid"].ToString() );
                CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( NodeTypeId );
                if( null != NodeType )
                {
                    foreach( CswNbtNode Node in NodeType.getNodes( false, true, false, true ) )
                    {
                        foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps() )
                        {
                            CswNbtMetaDataFieldType.NbtFieldType Type = Prop.getFieldTypeValue();

                            if( Type == CswNbtMetaDataFieldType.NbtFieldType.MOL ||
                                Type == CswNbtMetaDataFieldType.NbtFieldType.File ||
                                Type == CswNbtMetaDataFieldType.NbtFieldType.Image )
                            {
                                CswNbtNodePropWrapper propWrapper = Node.Properties[Prop];
                                _moveBlobData( propWrapper, Node );
                            }
                        }
                    }
                }
            }

        } // update()

        private void _moveBlobData( CswNbtNodePropWrapper PropWrapper, CswNbtNode Node )
        {
            CswNbtResources NbtResources = _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources;
            CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( NbtResources );

            CswPropIdAttr PropId = new CswPropIdAttr( Node.NodeId, PropWrapper.NodeTypePropId );
            byte[] BlobData = new byte[0];
            string ContentType = string.Empty;
            string FileName = string.Empty;

            CswTableSelect JctTS = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "getBlobData", "jct_nodes_props" );
            DataTable JctDT = JctTS.getTable( "jctnodepropid", PropWrapper.JctNodePropId );
            foreach( DataRow Row in JctDT.Rows ) //should only be one
            {
                BlobData = Row["blobdata"] as byte[];
                FileName = Row["field1"].ToString();
                ContentType = Row["field2"].ToString();
            }

            if( null != BlobData )
            {
                string Href;
                SdBlobData.saveFile( PropId.ToString(), BlobData, ContentType, FileName, out Href );
            }

        }

    }//class CswUpdateSchema_02B_Case26531

}//namespace ChemSW.Nbt.Schema