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

        /// <summary>
        /// Get a list of all NodeTypes, optionally limited according to supplied parameters
        /// </summary>
        /// <param name="ObjectClass">(Optional) An Object Class to constrain results.</param>
        /// <param name="ExcludeNodeTypeIds">(Optional) A comma-delimited string of NodeTypeIds to exclude from the return.</param>
        /// <param name="RelatedNodeTypeId">(Optional) A related NodeTypeId to further constrain the results</param>
        /// <param name="RelatedObjectClassPropName">(Required if RelatedNodeTypeId is supplied) The name of the Object Class Prop which defines the relationship to RelatedNodeTypeId</param>
        /// <returns></returns>
        public JObject getNodeTypes( CswNbtMetaDataObjectClass ObjectClass = null, string ExcludeNodeTypeIds = "", Int32 RelatedNodeTypeId = Int32.MinValue, string RelatedObjectClassPropName = "" )
        {
            JObject ReturnVal = new JObject();

            CswCommaDelimitedString ExcludedNodeTypes = new CswCommaDelimitedString();
            Collection<Int32> ExcludedIds = new Collection<Int32>();
            if( false == string.IsNullOrEmpty( ExcludeNodeTypeIds ) )
            {
                ExcludedNodeTypes.FromString( ExcludeNodeTypeIds );
                ExcludedIds = ExcludedNodeTypes.ToIntCollection();
            }

            Int32 NodeTypeCount = 0;
            if( null == ObjectClass )
            {
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() )
                {
                    if( false == ExcludedIds.Contains( NodeType.NodeTypeId ) )
                    {
                        NodeTypeCount += _filterNodeTypesByObjectClassPropRelationship( NodeType, ReturnVal, RelatedNodeTypeId, RelatedObjectClassPropName );
                    }
                } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            }
            else
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
                {
                    if( NodeType.IsLatestVersion() && false == ExcludedIds.Contains( NodeType.NodeTypeId ) )
                    {
                        NodeTypeCount += _filterNodeTypesByObjectClassPropRelationship( NodeType, ReturnVal, RelatedNodeTypeId, RelatedObjectClassPropName );
                    }
                }
            }
            ReturnVal["count"] = NodeTypeCount;
            return ReturnVal;
        } // getNodeTypes()

        private Int32 _filterNodeTypesByObjectClassPropRelationship( CswNbtMetaDataNodeType NodeType, JObject RetObj, Int32 RelatedNodeTypeId = Int32.MinValue, string RelatedObjectClassPropName = "" )
        {
            Int32 NodeTypesAdded = 0;
            if( null != NodeType )
            {
                if( Int32.MinValue != RelatedNodeTypeId &&
                    false == string.IsNullOrEmpty( RelatedObjectClassPropName ) )
                {
                    /* We don't have a way (yet) to validate the prop name against the object class, so validate afterward */
                    CswNbtMetaDataNodeTypeProp RelatedNtp = NodeType.getNodeTypePropByObjectClassProp( RelatedObjectClassPropName );
                    if( null != RelatedNtp &&
                        RelatedNtp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
                        RelatedNtp.FKType != NbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        CswNbtMetaDataNodeType RelatedNodeType = _CswNbtResources.MetaData.getNodeType( RelatedNodeTypeId );
                        if( null != RelatedNodeType && RelatedNtp.FKValue == RelatedNodeType.getFirstVersionNodeType().NodeTypeId )
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
