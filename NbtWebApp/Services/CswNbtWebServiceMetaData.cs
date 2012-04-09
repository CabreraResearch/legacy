using System;
using System.Collections.ObjectModel;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceMetaData
    {
        private readonly CswNbtResources _CswNbtResources;

        public CswNbtWebServiceMetaData( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        } //ctor

        public JObject getNodeTypes( CswNbtMetaDataObjectClass ObjectClass, string ExcludeNodeTypeIds, Int32 RelatedNodeTypeId )
        {
            JObject ReturnVal = new JObject();
            CswCommaDelimitedString ExcludedNodeTypes = new CswCommaDelimitedString();
            ExcludedNodeTypes.FromString( ExcludeNodeTypeIds );
            Collection<Int32> ExcludedIds = ExcludedNodeTypes.ToIntCollection();
            Int32 NodeTypeCount = 0;
            if( null == ObjectClass )
            {
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
                {
                    if( false == ExcludedIds.Contains( NodeType.NodeTypeId ) )
                    {
                        NodeTypeCount += _filterNodeTypesByObjectClassPropRelationship( NodeType, RelatedNodeTypeId, ReturnVal );
                    }
                } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            }
            else
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
                {
                    if( NodeType.IsLatestVersion() && false == ExcludedIds.Contains( NodeType.NodeTypeId ) )
                    {
                        NodeTypeCount += _filterNodeTypesByObjectClassPropRelationship( NodeType, RelatedNodeTypeId, ReturnVal );
                    }
                }
            }
            ReturnVal["count"] = NodeTypeCount;
            return ReturnVal;
        } // getNodeTypes()

        private Int32 _filterNodeTypesByObjectClassPropRelationship( CswNbtMetaDataNodeType NodeType, Int32 RelatedNodeTypeId, JObject RetObj )
        {
            Int32 NodeTypesAdded = 0;
            if( null != NodeType )
            {
                switch( NodeType.getObjectClass().ObjectClass )
                {
                    case CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass:
                        if( Int32.MinValue != RelatedNodeTypeId )
                        {
                            CswNbtMetaDataNodeTypeProp MaterialNtp = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
                            if( MaterialNtp.FKType != NbtViewRelatedIdType.ObjectClassId.ToString() )
                            {
                                CswNbtMetaDataNodeType RelatedMaterialNt = _CswNbtResources.MetaData.getNodeType( RelatedNodeTypeId );
                                if( null != RelatedMaterialNt && MaterialNtp.FKValue == RelatedMaterialNt.getFirstVersionNodeType().NodeTypeId )
                                {
                                    _addNodeTypeAttributes( NodeType, RetObj );
                                    NodeTypesAdded = 1;
                                }
                            }
                            else
                            {
                                _addNodeTypeAttributes( NodeType, RetObj );
                                NodeTypesAdded = 1;
                            }
                        }
                        else
                        {
                            _addNodeTypeAttributes( NodeType, RetObj );
                            NodeTypesAdded = 1;
                        }
                        break;

                    default:
                        _addNodeTypeAttributes( NodeType, RetObj );
                        NodeTypesAdded = 1;
                        break;
                }
            }
            return NodeTypesAdded;
        }

        private void _addNodeTypeAttributes( CswNbtMetaDataNodeType NodeType, JObject ReturnVal )
        {
            CswNbtMetaDataObjectClass ObjectClass = NodeType.getObjectClass();
            string NtName = "nodetype_" + NodeType.NodeTypeId;
            ReturnVal[NtName] = new JObject();
            ReturnVal[NtName]["id"] = NodeType.NodeTypeId;
            ReturnVal[NtName]["name"] = NodeType.NodeTypeName;
            ReturnVal[NtName]["objectclass"] = ObjectClass.ObjectClass.ToString();
            ReturnVal[NtName]["objectclassid"] = ObjectClass.ObjectClassId.ToString();

            switch( ObjectClass.ObjectClass )
            {
                case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass:
                    CswNbtMetaDataNodeTypeProp InspectionTargetNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
                    ReturnVal[NtName]["targetnodetypeid"] = InspectionTargetNTP.FKValue.ToString();
                    if( InspectionTargetNTP.FKType == NbtViewPropIdType.NodeTypePropId.ToString() )
                    {
                        ReturnVal[NtName]["targetnodetypeid"] = InspectionTargetNTP.FKValue.ToString();
                    }
                    break;
            }

        }

        public CswNbtResources makeSystemUserResources( string AccessId, bool ExcludeDisabledModules = true, bool IsDeleteModeLogical = true )
        {
            CswNbtResources NbtSystemResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, SetupMode.NbtWeb, ExcludeDisabledModules, IsDeleteModeLogical, new CswSuperCycleCacheDefault() );
            NbtSystemResources.AccessId = AccessId;
            NbtSystemResources.InitCurrentUser = _InitSystemUser;
            return NbtSystemResources;
        }

        private ICswUser _InitSystemUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, "CswNbtWebServiceNbtManager_SystemUser" );
        }

        public void finalizeOtherResources( CswNbtResources NbtOtherResources )
        {
            if( null != NbtOtherResources )
            {
                NbtOtherResources.logMessage( "WebServices: Session Ended (_deInitResources called)" );
                NbtOtherResources.finalize();
                NbtOtherResources.release();
            }
        } //finalizeOtherResources

    } // class CswNbtWebServiceMetaData

} // namespace ChemSW.Nbt.WebServices
