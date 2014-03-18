using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using ChemSW.Core;
using ChemSW.Nbt.Batch;
using ChemSW.Nbt.csw.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.Actions
{
    /// <summary>
    /// Holds logic for handling arbitrary batch operations
    /// </summary>
    public class CswNbtWebServiceBulkEdit
    {
        [DataContract]
        public class BulkEditProperties : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            public Collection<BulkEditProperty> Data;
        }
        [DataContract]
        public class BulkEditProperty
        {
            [DataMember( IsRequired = true )]
            public Int32 id;
            [DataMember( IsRequired = true )]
            public string name;
        }

        [DataContract]
        public class BulkEditParams
        {
            [DataMember( IsRequired = true )]
            public string ViewId;
            [DataMember( IsRequired = true )]
            public Int32 NodeTypeId;
            [DataMember( IsRequired = true )]
            public Collection<Int32> PropIds;
        }

        [DataContract]
        public class BulkEditDownload
        {
            [DataMember( IsRequired = true )]
            [Description( "Excel file content for bulk edit" )]
            public DataTable CsvData;
        }

        [DataContract]
        public class BulkEditUpload
        {
            [DataMember( IsRequired = true )]
            [Description( "Excel file content for bulk edit" )]
            public HttpPostedFile PostedFile;
        }

        [DataContract]
        public class BulkEditReturn : CswWebSvcReturn
        {
            [DataMember( IsRequired = true )]
            [Description( "ViewId" )]
            public string ViewId;
        }

        public static void getBulkEditProperties( ICswResources CswResources, BulkEditProperties ret, BulkEditParams Params )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Params.NodeTypeId );
            if( null != NodeType )
            {
                ret.Data = new Collection<BulkEditProperty>();
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
                        ret.Data.Add( new BulkEditProperty()
                            {
                                id = Prop.PropId,
                                name = Prop.PropNameWithQuestionNo
                            } );
                    }
                }
            }
        } // getBulkEditProperties()


        public static void DownloadBulkEditData( ICswResources CswResources, BulkEditDownload ret, BulkEditParams Params )
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
                _recurseBulkEditData( NodeType, Tree, ret, Params );
            }
        } // DownloadBulkEditData()

        private static void _recurseBulkEditData( CswNbtMetaDataNodeType NodeType, ICswNbtTree Tree, BulkEditDownload ret, BulkEditParams Params )
        {
            for( Int32 n = 0; n < Tree.getChildNodeCount(); n++ )
            {
                Tree.goToNthChild( n );

                if( Tree.getNodeKeyForCurrentPosition().NodeTypeId == Params.NodeTypeId )
                {
                    _addNodeToData( NodeType, Tree.getCurrentNode(), ret, Params );
                }
                _recurseBulkEditData( NodeType, Tree, ret, Params );

                Tree.goToParentNode();
            }
        } // _recurseBulkEditData()

        private static void _addNodeToData( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, BulkEditDownload ret, BulkEditParams Params )
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

        public static void UploadBulkEditData( ICswResources CswResources, BulkEditReturn ret, BulkEditUpload Params )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswTempFile temp = new CswTempFile( CswResources );
            string tempPath = temp.saveToTempFile( Params.PostedFile.InputStream, CswResources.AccessId + "_bulkedit_" + DateTime.Now.Ticks.ToString() );

            DataSet uploadDataSet = CswNbtImportTools.ReadExcel( tempPath );
            if( uploadDataSet.Tables.Count > 0 )
            {
                DataTable uploadTable = uploadDataSet.Tables[0];

                CswNbtBatchOpBulkEdit batch = new CswNbtBatchOpBulkEdit( NbtResources );
                CswNbtObjClassBatchOp batchNode = batch.makeBatchOp( uploadTable );

                CswNbtView BatchOpsView = new CswNbtView( (CswNbtResources) CswResources );
                BatchOpsView.ViewName = "New Batch Operations";
                BatchOpsView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                CswNbtViewRelationship BatchRel = BatchOpsView.AddViewRelationship( batchNode.NodeType, false );
                BatchRel.NodeIdsToFilterIn.Add( batchNode.NodeId );

                BatchOpsView.SaveToCache( true );
                ret.ViewId = BatchOpsView.SessionViewId.ToString();

            } // if( uploadDataSet.Tables.Count > 0 )
        } // UploadBulkEditData()

    } // class CswNbtWebServiceBulkEdit
}// namespace ChemSW.Nbt.Actions

