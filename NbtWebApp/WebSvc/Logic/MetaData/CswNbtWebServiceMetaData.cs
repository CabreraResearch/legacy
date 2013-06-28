using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        /// Get a list of all Object Classes
        /// </summary>
        public JObject getObjectClasses()
        {
            JObject ReturnVal = new JObject();
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClasses().OrderBy( ObjectClass => ObjectClass.ObjectClass.ToString() ) )
            {
                string ObjectClassName = ObjectClass.ObjectClass.ToString();
                ReturnVal[ObjectClassName] = new JObject();
                ReturnVal[ObjectClassName]["objectclass"] = ObjectClassName;
                ReturnVal[ObjectClassName]["objectclassid"] = ObjectClass.ObjectClassId.ToString();
                ReturnVal[ObjectClassName]["iconfilename"] = CswNbtMetaDataObjectClass.IconPrefix16 + ObjectClass.IconFileName;
            }
            return ReturnVal;
        } // getObjectClasses()


        /// <summary>
        /// Get a list of all NodeTypes, optionally limited according to supplied parameters
        /// </summary>
        /// <param name="ObjectClass">(Optional) An Object Class to constrain results.</param>
        /// <param name="ExcludeNodeTypeIds">(Optional) A comma-delimited string of NodeTypeIds to exclude from the return.</param>
        /// <param name="RelationshipTargetNodeTypeId">(Optional [Requires RelationshipObjectClassPropName]) 
        /// <para>A related NodeTypeId to further constrain the results to nodetypes whose relationship targets the supplied RelationshipTargetNodeTypeId</para>
        /// <para>Use case: get all nodetypes of Size object class whose Material relationships target Chemicals.</para>
        /// </param>
        /// <param name="RelationshipObjectClassPropName">(Optional [Requires RelationshipObjectClassPropName]) 
        /// <para>The name of the Object Class Prop which defines the relationship to RelationshipTargetNodeTypeId</para>
        /// <param name="FilterToCreate">(Optional) 
        /// <para>When set to true, only gets NodeTypes user has permission to create</para>
        /// </param>
        /// <param name="Searchable">If true, only include searchable nodetypes</param>
        /// <returns></returns>
        public JObject getNodeTypes( CswNbtMetaDataPropertySet PropertySet = null,
                                     CswNbtMetaDataObjectClass ObjectClass = null,
                                     string ExcludeNodeTypeIds = "",
                                     Int32 RelationshipTargetNodeTypeId = Int32.MinValue,
                                     string RelationshipObjectClassPropName = "",
                                     Int32 RelationshipNodeTypePropId = Int32.MinValue,
                                     CswEnumNbtNodeTypePermission FilterToPermission = null,
                                     bool Searchable = false )
        {
            JObject ReturnVal = new JObject();

            // We default the Permission type to 'View' otherwise, if FilterTOPermission is null or empty,
            // the default becomes 'Unknown' and the User will not have permission to do anything.
            if( string.IsNullOrEmpty( FilterToPermission ) )
            {
                FilterToPermission = CswEnumNbtNodeTypePermission.View;
            }

            CswCommaDelimitedString ExcludedNodeTypes = new CswCommaDelimitedString();
            Collection<Int32> ExcludedIds = new Collection<Int32>();
            if( false == string.IsNullOrEmpty( ExcludeNodeTypeIds ) )
            {
                ExcludedNodeTypes.FromString( ExcludeNodeTypeIds );
                ExcludedIds = ExcludedNodeTypes.ToIntCollection();
            }

            IEnumerable<CswNbtMetaDataNodeType> NodeTypes;
            if( Int32.MinValue != RelationshipNodeTypePropId )
            {
                CswNbtMetaDataNodeTypeProp RelationshipProp = _CswNbtResources.MetaData.getNodeTypeProp( RelationshipNodeTypePropId );
                NodeTypes = _CswNbtResources.MetaData.getNodeTypes().Where( nt => RelationshipProp.FkMatches( nt ) );
            }
            else if( null != PropertySet )
            {
                List<CswNbtMetaDataNodeType> NTs = new List<CswNbtMetaDataNodeType>();
                foreach( CswNbtMetaDataObjectClass OC in PropertySet.getObjectClasses() )
                {
                    NTs.AddRange( OC.getLatestVersionNodeTypes() );
                }
                NodeTypes = NTs;
            }
            else if( null == ObjectClass )
            {
                NodeTypes = _CswNbtResources.MetaData.getNodeTypesLatestVersion();
            }
            else
            {
                NodeTypes = ObjectClass.getLatestVersionNodeTypes();
                ReturnVal["objectClassId"] = ObjectClass.ObjectClassId;
            }

            Int32 NodeTypeCount = 0;

            foreach( CswNbtMetaDataNodeType RetNodeType in NodeTypes
                                                            .Where( _RetNodeType => ( false == Searchable || CswNbtMetaDataObjectClass.NotSearchableValue != _RetNodeType.SearchDeferPropId ) )
                                                            .OrderBy( _RetNodeType => _RetNodeType.NodeTypeName ) )
            {
                bool AddThisNodeType = false;
                if( false == ExcludedIds.Contains( RetNodeType.NodeTypeId ) )
                {
                    AddThisNodeType = true;
                    if( Int32.MinValue != RelationshipTargetNodeTypeId &&
                         false == string.IsNullOrEmpty( RelationshipObjectClassPropName ) )
                    {
                        CswNbtMetaDataNodeTypeProp RelationshipNtp = RetNodeType.getNodeTypePropByObjectClassProp( RelationshipObjectClassPropName );
                        if( null != RelationshipNtp &&
                             RelationshipNtp.getFieldTypeValue() == CswEnumNbtFieldType.Relationship )
                        {
                            CswNbtMetaDataNodeType RelatedNodeType = _CswNbtResources.MetaData.getNodeType( RelationshipTargetNodeTypeId );
                            if( null == RelatedNodeType ||
                                false == RelationshipNtp.FkMatches( RelatedNodeType, true ) )
                            //false == ( ( RelationshipNtp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() &&
                            //              RelationshipNtp.FKValue == RelatedNodeType.FirstVersionNodeTypeId ) ||
                            //            ( RelationshipNtp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                            //              RelationshipNtp.FKValue == RelatedNodeType.ObjectClassId ) ) )
                            {
                                AddThisNodeType = false;
                            }
                        }
                    }
                    if( false == _userHasPermission( FilterToPermission, RetNodeType ) )
                    {
                        AddThisNodeType = false;
                    }
                }

                if( AddThisNodeType )
                {
                    _addNodeTypeAttributes( RetNodeType, ReturnVal );
                    NodeTypeCount += 1;
                }
            }


            ReturnVal["count"] = NodeTypeCount;
            return ReturnVal;
        } // getNodeTypes()

        public JObject getNodeTypeTabs( string NodeTypeName, string NodeTypeId, string FilterToPermission )
        {
            JObject ReturnVal = new JObject();
            CswNbtMetaDataNodeType NodeType;
            if( false == String.IsNullOrEmpty( NodeTypeName ) )
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeName );
            }
            else
            {
                NodeType = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( NodeTypeId ) );
            }
            if( null != NodeType )
            {
                IEnumerable<CswNbtMetaDataNodeTypeTab> Tabs = NodeType.getNodeTypeTabs();
                foreach( CswNbtMetaDataNodeTypeTab Tab in Tabs )
                {
                    if( _userHasTabPermission( FilterToPermission, NodeType, Tab ) )
                    {
                        string TabName = "tab_" + Tab.TabId;
                        ReturnVal[TabName] = new JObject();
                        ReturnVal[TabName]["id"] = Tab.TabId;
                        ReturnVal[TabName]["name"] = Tab.TabName;
                    }
                }
            }
            return ReturnVal;
        }

        private bool _userHasTabPermission( CswEnumNbtNodeTypePermission PermissionType, CswNbtMetaDataNodeType NodeType, CswNbtMetaDataNodeTypeTab Tab )
        {
            bool hasPermission = true;
            hasPermission = _CswNbtResources.Permit.canTab( PermissionType, NodeType, Tab );
            return hasPermission;
        }

        private bool _userHasPermission( CswEnumNbtNodeTypePermission PermissionType, CswNbtMetaDataNodeType RetNodeType )
        {
            bool hasPermission = true;
            if( PermissionType == CswEnumNbtNodeTypePermission.Create )
            {
                hasPermission = hasPermission && RetNodeType.getObjectClass().CanAdd;
            }
            hasPermission = hasPermission && _CswNbtResources.Permit.canNodeType( PermissionType, RetNodeType );

            return hasPermission;
        }

        private void _addNodeTypeAttributes( CswNbtMetaDataNodeType NodeType, JObject ReturnVal )
        {
            CswNbtMetaDataObjectClass ObjectClass = NodeType.getObjectClass();
            string NtName = "nodetype_" + NodeType.NodeTypeId;
            ReturnVal[NtName] = new JObject();
            ReturnVal[NtName]["id"] = NodeType.NodeTypeId;
            ReturnVal[NtName]["name"] = NodeType.NodeTypeName;
            ReturnVal[NtName]["iconfilename"] = CswNbtMetaDataObjectClass.IconPrefix16 + NodeType.IconFileName;
            ReturnVal[NtName]["objectclass"] = ObjectClass.ObjectClass.ToString();
            ReturnVal[NtName]["objectclassid"] = ObjectClass.ObjectClassId.ToString();

            switch( ObjectClass.ObjectClass )
            {
                case CswEnumNbtObjectClass.InspectionDesignClass:
                    CswNbtMetaDataNodeTypeProp InspectionTargetNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                    ReturnVal[NtName]["targetnodetypeid"] = InspectionTargetNTP.FKValue.ToString();
                    if( InspectionTargetNTP.FKType == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() )
                    {
                        ReturnVal[NtName]["targetnodetypeid"] = InspectionTargetNTP.FKValue.ToString();
                    }
                    break;
            }

        }

        public CswNbtResources makeSystemUserResources( string AccessId, bool ExcludeDisabledModules = true, bool IsDeleteModeLogical = true )
        {
            CswNbtResources NbtSystemResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtWeb, ExcludeDisabledModules, IsDeleteModeLogical, new CswSuperCycleCacheDefault() );
            NbtSystemResources.AccessId = AccessId;
            NbtSystemResources.InitCurrentUser = _InitSystemUser;
            return NbtSystemResources;
        }

        private ICswUser _InitSystemUser( ICswResources Resources )
        {
            return new CswNbtSystemUser( Resources, CswEnumSystemUserNames.SysUsr_NbtWebSvcMgr );
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
