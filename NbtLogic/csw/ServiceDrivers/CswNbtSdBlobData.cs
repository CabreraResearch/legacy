using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.StructureSearch;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdBlobData
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtSdBlobData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public int saveFile( string PropIdAttr, byte[] BlobData, string ContentType, string FileName, out string Href, int BlobDataId = Int32.MinValue )
        {
            CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
            CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];
            FileProp.makePropRow(); //if we don't have a jct_node_prop row for this prop, we do now

            //Save the file to blob_data
            CswTableUpdate BlobUpdate = _CswNbtResources.makeCswTableUpdate( "saveBlob", "blob_data" );
            string whereClause = "where jctnodepropid = " + FileProp.JctNodePropId;
            if( Int32.MinValue != BlobDataId )
            {
                whereClause += " and blobdataid = " + BlobDataId;
            }
            DataTable BlobTbl = BlobUpdate.getTable( whereClause );
            if( BlobTbl.Rows.Count > 0 && Int32.MinValue != BlobDataId )
            {
                BlobTbl.Rows[0]["blobdata"] = BlobData;
                BlobTbl.Rows[0]["contenttype"] = ContentType;
                BlobTbl.Rows[0]["filename"] = FileName;
                BlobDataId = CswConvert.ToInt32( BlobTbl.Rows[0]["blobdataid"] );
            }
            else
            {
                DataRow NewRow = BlobTbl.NewRow();
                NewRow["jctnodepropid"] = FileProp.JctNodePropId;
                NewRow["blobdata"] = BlobData;
                NewRow["contenttype"] = ContentType;
                NewRow["filename"] = FileName;
                BlobDataId = CswConvert.ToInt32( NewRow["blobdataid"] );
                BlobTbl.Rows.Add( NewRow );
            }
            BlobUpdate.update( BlobTbl );

            if( Node.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ReportClass )
            {
                CswNbtObjClassReport Report = Node;
                CswFilePath FilePathTools = new CswFilePath( _CswNbtResources );
                string ReportPath = FilePathTools.getFullReportFilePath( Report.RPTFile.JctNodePropId.ToString() );
                _createReportFile( ReportPath, Report.RPTFile.JctNodePropId, BlobData );
            }

            Node.postChanges( false );

            Href = CswNbtNodePropBlob.getLink( FileProp.JctNodePropId, PropId.NodeId, BlobDataId );
            return BlobDataId;
        }

        private void _createReportFile( string ReportTempFileName, int NodePropId, byte[] BlobData )
        {
            ( new FileInfo( ReportTempFileName ) ).Directory.Create();
            FileMode fileMode = File.Exists( ReportTempFileName ) ? FileMode.Truncate : FileMode.CreateNew;
            FileStream fs = new FileStream( ReportTempFileName, fileMode );
            BinaryWriter BWriter = new BinaryWriter( fs, System.Text.Encoding.Default );
            BWriter.Write( BlobData );
        }

        public void saveMol( string MolString, string PropId, out string Href )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropId );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );

            Href = "";
            CswNbtNode Node = _CswNbtResources.Nodes[PropIdAttr.NodeId];
            if( null != Node )
            {
                CswNbtNodePropMol molProp = Node.Properties[MetaDataProp];
                if( null != molProp )
                {
                    //Save the mol text to _CswNbtResources
                    CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Clobber_save_update", "jct_nodes_props" );

                    if( Int32.MinValue != molProp.JctNodePropId )
                    {
                        DataTable JctTable = JctUpdate.getTable( "jctnodepropid", molProp.JctNodePropId );
                        JctTable.Rows[0]["clobdata"] = MolString;
                        JctUpdate.update( JctTable );
                    }
                    else
                    {
                        DataTable JctTable = JctUpdate.getEmptyTable();
                        DataRow JRow = JctTable.NewRow();
                        JRow["nodetypepropid"] = CswConvert.ToDbVal( PropIdAttr.NodeTypePropId );
                        JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                        JRow["nodeidtablename"] = Node.NodeId.TableName;
                        JRow["clobdata"] = MolString;
                        JctTable.Rows.Add( JRow );
                        JctUpdate.update( JctTable );
                    }

                    //Save the mol image to blob_data
                    byte[] molImage = CswStructureSearch.GetImage( MolString );

                    CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                    Href = CswNbtNodePropMol.getLink( molProp.JctNodePropId, Node.NodeId );

                    SdBlobData.saveFile( PropId, molImage, CswNbtNodePropMol.MolImgFileContentType, CswNbtNodePropMol.MolImgFileName, out Href );

                    //case 28364 - calculate fingerprint and save it
                    _CswNbtResources.StructureSearchManager.InsertFingerprintRecord( PropIdAttr.NodeId.PrimaryKey, MolString );
                }
            }
        }

        /// <summary>
        /// Gets a collection of all images for a property
        /// </summary>
        public Collection<CswNbtImage> GetImages( CswPrimaryKey NodeId, Int32 JctNodePropId )
        {
            Collection<CswNbtImage> images = new Collection<CswNbtImage>();
            CswTableSelect blobDataTS = _CswNbtResources.makeCswTableSelect( "NodePropImage.getFileNames", "blob_data" );
            DataTable blobDataTbl = blobDataTS.getTable( "where jctnodepropid = " + JctNodePropId );
            foreach( DataRow row in blobDataTbl.Rows )
            {
                Int32 BlobDataId = CswConvert.ToInt32( row["blobdataid"] );
                CswNbtImage img = new CswNbtImage()
                {
                    FileName = row["filename"].ToString(),
                    ContentType = row["contenttype"].ToString(),
                    BlobDataId = BlobDataId,
                    ImageUrl = CswNbtNodePropImage.getLink( JctNodePropId, NodeId, BlobDataId ),
                    Caption = row["caption"].ToString()
                };
                images.Add( img );
            }

            if( images.Count == 0 ) //add default placeholder
            {
                CswNbtImage placeHolderImg = new CswNbtImage()
                {
                    FileName = "empty",
                    ContentType = "image/gif",
                    BlobDataId = Int32.MinValue,
                    ImageUrl = CswNbtNodePropImage.getLink( JctNodePropId, NodeId )
                };
                images.Add( placeHolderImg );
            }

            return images;
        }

        [DataContract]
        public class CswNbtImage
        {
            [DataMember]
            public string FileName = string.Empty;

            [DataMember]
            public string ContentType = string.Empty;

            [DataMember]
            public string ImageUrl = string.Empty;

            [DataMember]
            public int BlobDataId = Int32.MinValue;

            [DataMember]
            public string Caption = string.Empty;
        }

    }
}
