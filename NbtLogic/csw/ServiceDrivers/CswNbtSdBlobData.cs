using System;
using System.Data;
using System.IO;
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

            if( Node.getObjectClass().ObjectClass == NbtObjectClass.ReportClass )
            {
                CswNbtObjClassReport Report = Node;
                CswFilePath FilePathTools = new CswFilePath( _CswNbtResources );
                string ReportPath = FilePathTools.getFullReportFilePath( Report.RPTFile.JctNodePropId.ToString() );
                _createReportFile( ReportPath, Report.RPTFile.JctNodePropId, BlobData );
            }

            Node.postChanges( false );

            Href = CswNbtNodePropBlob.getLink( FileProp.JctNodePropId, PropId.NodeId, FileProp.JctNodePropId );
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
                    Href = CswNbtNodePropMol.getLink( molProp.JctNodePropId, Node.NodeId, PropIdAttr.NodeTypePropId );

                    SdBlobData.saveFile( PropId, molImage, "image/jpeg", "Mol.jpeg", out Href );

                    //case 28364 - calculate fingerprint and save it
                    _CswNbtResources.StructureSearchManager.InsertFingerprintRecord( PropIdAttr.NodeId.PrimaryKey, MolString );
                }
            }
        }

    }
}
