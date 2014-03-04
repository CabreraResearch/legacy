using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.WebServices;
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
            public CswNbtViewId ViewId;
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
                foreach( Int32 PropId in Params.PropIds )
                {
                    CswNbtMetaDataNodeTypeProp Prop = NodeType.getNodeTypeProp( PropId );
                    ret.CsvData.Columns.Add( Prop.PropName );  // danger?
                }

                CswNbtView View = NbtResources.ViewSelect.restoreView( Params.ViewId );
                ICswNbtTree Tree = NbtResources.Trees.getTreeFromView( View, RequireViewPermissions: true, IncludeSystemNodes: false, IncludeHiddenNodes: false );
                _recurseBatchEditData( NodeType, Tree, ret, Params );
            }
        }

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
        }

        private static void _addNodeToData( CswNbtMetaDataNodeType NodeType, CswNbtNode Node, BatchEditDownload ret, BatchEditParams Params )
        {
            DataRow row = ret.CsvData.NewRow();
            foreach( Int32 PropId in Params.PropIds )
            {
                CswNbtMetaDataNodeTypeProp Prop = NodeType.getNodeTypeProp( PropId );
                row[Prop.PropName] = Node.Properties[Prop].Gestalt;
            }
            ret.CsvData.Rows.Add( row );
        }

        public static void UploadBatchEditData( ICswResources CswResources, BatchEditReturn ret, BatchEditUpload Params )
        {

        }

    } // class CswNbtWebServiceBatchEdit
}// namespace ChemSW.Nbt.Actions

