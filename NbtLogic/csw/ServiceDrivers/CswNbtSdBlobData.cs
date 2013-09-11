using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
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


        public static void FixOrientation( ref Image image )
        {
            // 0x0112 is the EXIF byte address for the orientation tag
            bool quit = true;
            for( int i = 0; i < image.PropertyItems.Length; ++i )
            {
                if( image.PropertyItems[i].Id == 0x0112 )
                {
                    quit = false;
                }
            }
            if( quit )
            {
                return;
            }

            // get the first byte from the orientation tag and convert it to an integer
            var orientationNumber = image.GetPropertyItem( 0x0112 ).Value[0];

            switch( orientationNumber )
            {
                // up is pointing to the right
                case 8:
                    image.RotateFlip( RotateFlipType.Rotate270FlipNone );
                    break;
                // up is pointing to the bottom (image is upside-down)
                case 3:
                    image.RotateFlip( RotateFlipType.Rotate180FlipNone );
                    break;
                // up is pointing to the left
                case 6:
                    image.RotateFlip( RotateFlipType.Rotate90FlipNone );
                    break;
                // up is pointing up (correct orientation)
                case 1:
                    break;
            }
        }

        public int saveFile( string PropIdAttr, byte[] BlobData, string ContentType, string FileName, out string Href, int BlobDataId = Int32.MinValue, bool PostChanges = true )
        {
            CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );

            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
            CswNbtNode Node = _CswNbtResources.Nodes[PropId.NodeId];
            CswNbtNodePropWrapper FileProp = Node.Properties[MetaDataProp];

            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.Edit, MetaDataProp.getNodeType(), _CswNbtResources.CurrentNbtUser ) &&
                _CswNbtResources.Permit.isPropWritable( CswEnumNbtNodeTypePermission.Edit, MetaDataProp, null, FileProp ) )
            {
                if( Int32.MinValue == FileProp.JctNodePropId )
                {
                    FileProp.makePropRow(); //if we don't have a jct_node_prop row for this prop, we do now
                    if( PostChanges )
                    {
                        Node.postChanges( true );
                    }
                }

                if( FileProp.getFieldType().FieldType == CswEnumNbtFieldType.Image )
                {
                    //case 29692: support EXIF image rotation metadata to properly orient photos
                    bool img_ok = false;
                    MemoryStream ms = new MemoryStream( BlobData, 0, BlobData.Length );
                    ms.Write( BlobData, 0, BlobData.Length );
                    System.Drawing.Image img = null;

                    try
                    {
                        img = Image.FromStream( ms, true );
                        img_ok = true;
                    }
                    catch
                    {
                    }

                    if( img_ok == true )
                    {
                        FixOrientation( ref img );
                        ImageConverter converter = new ImageConverter();
                        BlobData = (byte[]) converter.ConvertTo( img, typeof( byte[] ) );
                    }
                }



                //Save the file to blob_data
                CswTableUpdate BlobUpdate = _CswNbtResources.makeCswTableUpdate( "saveBlob", "blob_data" );
                string whereClause = "where jctnodepropid = " + FileProp.JctNodePropId;
                if( Int32.MinValue != BlobDataId )
                {
                    whereClause += " and blobdataid = " + BlobDataId;
                }
                DataTable BlobTbl = BlobUpdate.getTable( whereClause );
                if( BlobTbl.Rows.Count > 0 &&
                    ( Int32.MinValue != BlobDataId ||
                      FileProp.getFieldTypeValue() == CswEnumNbtFieldType.File ||
                      FileProp.getFieldTypeValue() == CswEnumNbtFieldType.MOL ) )
                {
                    BlobTbl.Rows[0]["blobdata"] = BlobData;
                    BlobTbl.Rows[0]["contenttype"] = ContentType;
                    BlobTbl.Rows[0]["filename"] = FileName;
                    BlobTbl.Rows[0]["auditlevel"] = MetaDataProp.AuditLevel;
                    BlobDataId = CswConvert.ToInt32( BlobTbl.Rows[0]["blobdataid"] );
                }
                else
                {
                    DataRow NewRow = BlobTbl.NewRow();
                    NewRow["jctnodepropid"] = FileProp.JctNodePropId;
                    NewRow["blobdata"] = BlobData;
                    NewRow["contenttype"] = ContentType;
                    NewRow["filename"] = FileName;
                    NewRow["auditlevel"] = MetaDataProp.AuditLevel;
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

                SetLastModified( FileProp );
                FileProp.SyncGestalt();
                if( PostChanges )
                {
                    Node.postChanges( false );
                }

                Href = CswNbtNodePropBlob.getLink( FileProp.JctNodePropId, PropId.NodeId, BlobDataId );

            } //canNodeType() && isPropWritable()
            else
            {
                Href = string.Empty; //To satifsy the "ref string Href"
                throw new CswDniException( CswEnumErrorType.Error, "You do not have sufficient priveledges to save files", "User " + _CswNbtResources.CurrentNbtUser.UserId + " attemped to save blobdata on JctNodeProp " + FileProp.JctNodePropId );
            }
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

        public void saveMol( string MolString, string PropId, out string Href, out string FormattedMolString, bool PostChanges = true )
        {
            CswPropIdAttr PropIdAttr = new CswPropIdAttr( PropId );
            CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropIdAttr.NodeTypePropId );

            //Case 29769 - enforce correct mol file format
            FormattedMolString = MoleculeBuilder.FormatMolFile( MolString );

            Href = "";
            CswNbtNode Node = _CswNbtResources.Nodes[PropIdAttr.NodeId];
            if( null != Node )
            {
                CswNbtNodePropMol molProp = Node.Properties[MetaDataProp];
                if( null != molProp )
                {
                    //Save the mol text to _CswNbtResources
                    CswTableUpdate JctUpdate = _CswNbtResources.makeCswTableUpdate( "Clobber_save_update", "jct_nodes_props" );
                    DataTable JctTable = null;

                    if( Int32.MinValue != molProp.JctNodePropId )
                    {
                        JctTable = JctUpdate.getTable( "jctnodepropid", molProp.JctNodePropId );
                        JctTable.Rows[0]["clobdata"] = FormattedMolString;
                    }
                    else
                    {
                        JctTable = JctUpdate.getEmptyTable();
                        DataRow JRow = JctTable.NewRow();
                        JRow["nodetypepropid"] = CswConvert.ToDbVal( PropIdAttr.NodeTypePropId );
                        JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                        JRow["nodeidtablename"] = Node.NodeId.TableName;
                        JRow["clobdata"] = FormattedMolString;
                        JctTable.Rows.Add( JRow );
                    }

                    //Save the mol image to blob_data
                    byte[] molImage = CswStructureSearch.GetImage( FormattedMolString );

                    CswNbtSdBlobData SdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                    Href = CswNbtNodePropMol.getLink( molProp.JctNodePropId, Node.NodeId );

                    SdBlobData.saveFile( PropId, molImage, CswNbtNodePropMol.MolImgFileContentType, CswNbtNodePropMol.MolImgFileName, out Href, Int32.MinValue, PostChanges );
                    JctUpdate.update( JctTable );

                    //case 28364 - calculate fingerprint and save it
                    _CswNbtResources.StructureSearchManager.InsertFingerprintRecord( PropIdAttr.NodeId.PrimaryKey, FormattedMolString );
                }
            }
        }

        /// <summary>
        /// Gets a collection of all images for a property
        /// </summary>
        public Collection<CswNbtBlob> GetImages( CswPrimaryKey NodeId, Int32 JctNodePropId, string Date = "" )
        {
            Collection<CswNbtBlob> images = new Collection<CswNbtBlob>();
            DataTable blobDataTbl = null;
            if( string.IsNullOrEmpty( Date ) )
            {
                CswTableSelect blobDataTS = _CswNbtResources.makeCswTableSelect( "NodePropImage.getFileNames", "blob_data" );
                blobDataTbl = blobDataTS.getTable( "where jctnodepropid = " + JctNodePropId );

            }
            else //fetch blob content
            {
                CswArbitrarySelect blobDataAuditTS = GetBlobAuditSelect( _CswNbtResources, Date, JctNodePropId );
                blobDataTbl = blobDataAuditTS.getTable();
            }

            foreach( DataRow row in blobDataTbl.Rows )
            {
                Int32 BlobDataId = CswConvert.ToInt32( row["blobdataid"] );
                CswNbtBlob img = new CswNbtBlob()
                {
                    FileName = row["filename"].ToString(),
                    ContentType = row["contenttype"].ToString(),
                    BlobDataId = BlobDataId,
                    BlobUrl = CswNbtNodePropImage.getLink( JctNodePropId, NodeId, BlobDataId ),
                    Caption = row["caption"].ToString()
                };
                images.Add( img );
            }


            if( images.Count == 0 ) //add default placeholder
            {
                CswNbtBlob placeHolderImg = new CswNbtBlob()
                {
                    FileName = "empty.jpg",
                    ContentType = "image/gif",
                    BlobDataId = Int32.MinValue,
                    BlobUrl = CswNbtNodePropImage.getLink( JctNodePropId, NodeId )
                };
                images.Add( placeHolderImg );
            }

            return images;
        }

        /// <summary>
        /// Set the filename for a file property
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="JctNodePropId"></param>
        public void SetFileName( string FileName, CswNbtNodePropBlob BlobProp )
        {
            CswTableUpdate ts = _CswNbtResources.makeCswTableUpdate( "getFirstFileName", "blob_data" );
            DataTable dt = ts.getTable( "where jctnodepropid = " + BlobProp.JctNodePropId );
            if( dt.Rows.Count > 0 )
            {
                dt.Rows[0]["filename"] = FileName;
            }
            ts.update( dt );
        }

        /// <summary>
        /// Get the filename for a file property
        /// </summary>
        /// <param name="JctNodePropId"></param>
        /// <returns></returns>
        public string GetFileName( CswNbtNodePropBlob BlobProp )
        {
            string ret = "";
            CswTableSelect ts = _CswNbtResources.makeCswTableSelect( "getFirstFileName", "blob_data" );
            DataTable dt = ts.getTable( "where jctnodepropid = " + BlobProp.JctNodePropId );
            if( dt.Rows.Count > 0 )
            {
                ret = dt.Rows[0]["filename"].ToString();
            }
            return ret;
        }

        public int GetMolPropJctNodePropId( CswPrimaryKey NodeId )
        {
            Int32 ret = Int32.MinValue;

            string sql = @"select bd.jctnodepropid from blob_data bd
                              join jct_nodes_props jnp on jnp.jctnodepropid = bd.jctnodepropid
                           where jnp.nodeid = :nodeid ";

            CswArbitrarySelect arbSelect = _CswNbtResources.makeCswArbitrarySelect( "getBlobJctNodePropId", sql );
            arbSelect.addParameter( "nodeid", NodeId.PrimaryKey.ToString() );

            DataTable dt = arbSelect.getTable();

            if( dt.Rows.Count > 0 ) //there's only one mol img per node
            {
                ret = CswConvert.ToInt32( dt.Rows[0]["jctnodepropid"] );
            }

            return ret;
        }

        /// <summary>
        /// Gets the first BlobDataId for a given prop
        /// </summary>
        public int GetBlobDataId( int JctNodePropId )
        {
            int ret = Int32.MinValue;

            string sql = @"select blobdataid from blob_data where jctnodepropid = :jctnodepropid ";

            CswArbitrarySelect arbSelect = _CswNbtResources.makeCswArbitrarySelect( "getBlobJctNodePropId", sql );
            arbSelect.addParameter( "jctnodepropid", JctNodePropId.ToString() );

            DataTable dt = arbSelect.getTable();

            if( dt.Rows.Count > 0 )
            {
                ret = CswConvert.ToInt32( dt.Rows[0]["blobdataid"] );
            }

            return ret;
        }

        public void SetLastModified( CswNbtNodePropWrapper BlobProp )
        {
            BlobProp.SetPropRowValue( CswEnumNbtPropColumn.Field2_Date, DateTime.Now );
        }

        public static CswArbitrarySelect GetBlobAuditSelect( CswNbtResources NbtResources, string Date, int JctNodePropId, int BlobDataId = Int32.MinValue )
        {
            string sql = @"select * from blob_data_audit bda where bda.blobdataauditid in (select max(bd.blobdataauditid) from blob_data_audit bd
                                    join audit_transactions audt on audt.audittransactionid = bd.audittransactionid
                                    join jct_nodes_props_audit jnp on jnp.audittransactionid = audt.audittransactionid
                                where jnp.recordcreated <= to_date(:blobdate, 'MM/DD/YYYY HH:MI:SS PM') and jnp.jctnodepropid = :jctnodepropid";
            if( Int32.MinValue != BlobDataId )
            {
                sql += " and bda.blobdataid = :blobdataid";
            }
            sql += " group by bd.blobdataid ) order by bda.blobdataid";

            CswArbitrarySelect BlobAuditSelect = NbtResources.makeCswArbitrarySelect( "SdBlobData.GetBlobAuditSelect", sql );
            BlobAuditSelect.addParameter( "blobdate", Date );
            BlobAuditSelect.addParameter( "jctnodepropid", JctNodePropId.ToString() );
            if( Int32.MinValue != BlobDataId )
            {
                BlobAuditSelect.addParameter( "blobdataid", BlobDataId.ToString() );
            }

            return BlobAuditSelect;
        }

        [DataContract]
        public class CswNbtBlob
        {
            [DataMember]
            public string FileName = string.Empty;

            [DataMember]
            public string ContentType = string.Empty;

            [DataMember]
            public string BlobUrl = string.Empty;

            [DataMember]
            public int BlobDataId = Int32.MinValue;

            [DataMember]
            public string Caption = string.Empty;
        }

    }
}
