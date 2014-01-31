using System;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceDesign
    {

        #region DataContracts

        /// <summary>
        /// Request object
        /// </summary>
        [DataContract]
        public class CswNbtDesignRequest
        {
            //[DataMember]
            //public string 
        }

        /// <summary>
        /// Response object
        /// </summary>
        [DataContract]
        public class CswNbtDesignReturn : CswWebSvcReturn
        {
            public CswNbtDesignReturn()
            {
                Data = new DesignResponse();
            }

            [DataMember]
            public DesignResponse Data;
        }

        [DataContract]
        public class DesignResponse
        {
            [DataMember]
            public string NodePk = string.Empty;

            [DataMember]
            public string NodeKey = string.Empty;

            [DataMember]
            public Int32 NodeTypeId = Int32.MinValue;

            [DataMember]
            public string NodeTypeName = string.Empty;
            
            [DataMember]
            public string IconFileName = string.Empty;

            [DataMember]
            public Int32 ObjectClassId = Int32.MinValue;
        }

        #endregion

        public static void getDesignNodeType( ICswResources CswResources, CswNbtDesignReturn Return, string NodeTypeId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            CswPrimaryKey NodeTypePk = new CswPrimaryKey( "nodetypes", CswConvert.ToInt32( NodeTypeId ) );
            if( CswTools.IsPrimaryKey( NodeTypePk ) )
            {
                CswNbtObjClassDesignNodeType DesignNodeType = _CswNbtResources.Nodes.getNodeByRelationalId( NodeTypePk );
                if( null != DesignNodeType )
                {
                    DesignResponse DesignResponse = new DesignResponse();
                    DesignResponse.NodePk = CswConvert.ToString( DesignNodeType.NodeId );
                    DesignResponse.NodeKey = DesignNodeType.Node.NodeLink;
                    DesignResponse.NodeTypeId = DesignNodeType.NodeTypeId;
                    DesignResponse.NodeTypeName = DesignNodeType.NodeTypeName.Text;
                    DesignResponse.IconFileName = CswNbtMetaDataObjectClass.IconPrefix18 + DesignNodeType.IconFileName.Value.ToString();
                    DesignResponse.ObjectClassId = DesignNodeType.ObjectClass.ObjectClassId;
                    Return.Data = DesignResponse;
                }
            }

        }// getDesignNodeType()

        public static void getDesignNodeTypePropDefinition( ICswResources CswResources, CswNbtDesignReturn Return, string FieldTypeId )
        {
            CswNbtResources _CswNbtResources = (CswNbtResources) CswResources;
            Int32 FieldTypePk = CswConvert.ToInt32( FieldTypeId );
            CswNbtMetaDataFieldType FT = _CswNbtResources.MetaData.getFieldType( FieldTypePk );
            CswNbtMetaDataNodeType FieldTypeNT = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassDesignNodeTypeProp.getNodeTypeName( FT.FieldType ) );
            if( null != FieldTypeNT )
            {
                DesignResponse DesignResponse = new DesignResponse();
                DesignResponse.NodeTypeId = FieldTypeNT.NodeTypeId;
                DesignResponse.ObjectClassId = FieldTypeNT.ObjectClassId;
                Return.Data = DesignResponse;
            }
        }

    }//CswNbtWebServiceDesign class
}//namespace