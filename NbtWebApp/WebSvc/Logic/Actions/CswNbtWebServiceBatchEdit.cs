using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
using ChemSW.Nbt.csw.ImportExport;
using NbtWebApp.Actions.Receiving;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Actions
{
    /// <summary>
    /// Holds logic for handling arbitrary batch operations
    /// </summary>
    public class CswNbtWebServiceBatchEdit
    {
        [DataContract]
        public class BatchEditParams
        {
            [DataMember( IsRequired = true )]
            public string ViewId;
            [DataMember( IsRequired = true )]
            public Int32 NodeTypeId;
            [DataMember( IsRequired = true )]
            public Collection<Int32> PropIds;
        }

        [DataContract]
        public class BatchEditDownload
        {
            [DataMember( IsRequired = true )]
            [Description( "Excel file content for batch edit" )]
            public DataTable CsvData;
        }

        [DataContract]
        public class BatchEditUpload
        {
            [DataMember( IsRequired = true )]
            [Description( "Excel file content for batch edit" )]
            public HttpPostedFile PostedFile;
        }

        [DataContract]
        public class BatchEditReturn : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            [Description( "Batch Id" )]
            public Int32 BatchId;
        }

        public static void DownloadBatchEditData( ICswResources CswResources, BatchEditDownload ret, BatchEditParams Params )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Params.NodeTypeId );
            if( null != NodeType )
            {
                ret.CsvData = new DataTable();
                ret.CsvData.Columns.Add( "nodeid" );
                foreach( Int32 PropId in Params.PropIds )
                {
                    CswNbtMetaDataNodeTypeProp Prop = NodeType.getNodeTypeProp( PropId );
                    if( Prop.getFieldTypeRule().SubFields.Count > 1 )
                    {
                        foreach( CswNbtSubField SubField in Prop.getFieldTypeRule().SubFields )
                        {
                            ret.CsvData.Columns.Add( Prop.PropName + " " + SubField.Name );
                        }
                    }
                    else
                    {
                        ret.CsvData.Columns.Add( Prop.PropName );
                    }
                }

                CswNbtView View = NbtResources.ViewSelect.restoreView( new CswNbtViewId( Params.ViewId ) );
                ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( View, RequireViewPermissions: true, IncludeSystemNodes: false, IncludeHiddenNodes: false );
                _recurseBatchEditData( NodeType, Tree, ret, Params );
            }
        } // DownloadBatchEditData()

        private static void _recurseBatchEditData( CswNbtMetaDataNodeType NodeType, ICswNbtTree Tree, BatchEditDownload ret, BatchEditParams Params )
        {
            for( Int32 n = 0; n < Tree.getChildNodeCount(); n++ )
            {
                Tree.goToNthChild( n );

                if( Tree.getNodeKeyForCurrentPosition().NodeTypeId == Params.NodeTypeId )
                {
                    _addNodeToData( NodeType, Tree.getCurrentNode(), ret, Params );
                }
                _recurseBatchEditData( NodeType, Tree, ret, Params );

                Tree.goToParentNode();
            }
        } // _recurseBatchEditData()

        private static void _addNodeToData( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, BatchEditDownload ret, BatchEditParams Params )
        {
            DataRow row = ret.CsvData.NewRow();
            row["nodeid"] = Node.NodeId.ToString();

            foreach( Int32 PropId in Params.PropIds )
            {
                CswNbtMetaDataNodeTypeProp Prop = NodeType.getNodeTypeProp( PropId );
                if( Prop.getFieldTypeRule().SubFields.Count > 1 )
                {
                    foreach( CswNbtSubField SubField in Prop.getFieldTypeRule().SubFields )
                    {
                        row[Prop.PropName + " " + SubField.Name] = Node.Properties[Prop].GetSubFieldValue( SubField );
                    }
                }
                else
                {
                    row[Prop.PropName] = Node.Properties[Prop].GetSubFieldValue( Prop.getFieldTypeRule().SubFields.Default );
                }
            }
            ret.CsvData.Rows.Add( row );
        } // _addNodeToData()

        public static void UploadBatchEditData( ICswResources CswResources, BatchEditReturn ret, BatchEditUpload Params )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswTempFile temp = new CswTempFile( CswResources );
            string tempPath = temp.saveToTempFile( Params.PostedFile.InputStream, CswResources.AccessId + "_batchedit_" + DateTime.Now.Ticks.ToString() );

            DataSet uploadDataSet = CswNbtImportTools.ReadExcel( tempPath );
            if( uploadDataSet.Tables.Count > 0 )
            {
                DataTable uploadTable = uploadDataSet.Tables[0];
                
                CswNbtBatchOpBatchEdit batch = new CswNbtBatchOpBatchEdit( NbtResources );
                CswNbtObjClassBatchOp batchOp = batch.makeBatchOp( uploadTable );

                ret.BatchId = batchOp.NodeId.PrimaryKey;

            } // if( uploadDataSet.Tables.Count > 0 )
        } // UploadBatchEditData()

    } // class CswNbtWebServiceBatchEdit
}// namespace ChemSW.Nbt.Actions

