using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
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
        public class BatchEditProperties : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            public Collection<BatchEditProperty> Data;
        }
        [DataContract]
        public class BatchEditProperty
        {
            [DataMember( IsRequired = true )]
            public Int32 id;
            [DataMember( IsRequired = true )]
            public string name;
        }

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
            [Description( "ViewId" )]
            public string ViewId;
        }

        public static void getBatchEditProperties( ICswResources CswResources, BatchEditProperties ret, BatchEditParams Params )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Params.NodeTypeId );
            if( null != NodeType )
            {
                ret.Data = new Collection<BatchEditProperty>();
                if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NodeType ) )
                {
                    foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps()
                                                                        .Where( p => false == p.getFieldType().IsDisplayType() &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.File &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.Image &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.ImageList &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.LogicalSet &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.MOL &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.MetaDataList &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.Password &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.TimeInterval &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.ViewPickList &&
                                                                                     p.getFieldType().FieldType != CswEnumNbtFieldType.ViewReference )
                                                                        .OrderBy( p => p.PropName ) )
                    {
                        ret.Data.Add( new BatchEditProperty()
                            {
                                id = Prop.PropId,
                                name = Prop.PropNameWithQuestionNo
                            } );
                    }
                }
            }
        } // getBatchEditProperties()


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
                CswNbtObjClassBatchOp batchNode = batch.makeBatchOp( uploadTable );

                CswNbtView BatchOpsView = new CswNbtView( (CswNbtResources) CswResources );
                BatchOpsView.ViewName = "New Batch Operations";
                BatchOpsView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                CswNbtViewRelationship BatchRel = BatchOpsView.AddViewRelationship( batchNode.NodeType, false );
                BatchRel.NodeIdsToFilterIn.Add( batchNode.NodeId );

                BatchOpsView.SaveToCache( true );
                ret.ViewId = BatchOpsView.SessionViewId.ToString();

            } // if( uploadDataSet.Tables.Count > 0 )
        } // UploadBatchEditData()

    } // class CswNbtWebServiceBatchEdit
}// namespace ChemSW.Nbt.Actions

