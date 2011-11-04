using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

        public JObject getNodeTypes( CswNbtMetaDataObjectClass ObjectClass )
        {
            JObject ReturnVal = new JObject();
            if( null == ObjectClass )
            {
                foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
                {
                    _addNodeTypeAttributes( NodeType, ReturnVal );
                } // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
            }
            else
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
                {
                    if( NodeType.IsLatestVersion )
                    {
                        _addNodeTypeAttributes( NodeType, ReturnVal );
                    }
                }
            }
            return ReturnVal;
        } // getNodeTypes()

        private void _addNodeTypeAttributes( CswNbtMetaDataNodeType NodeType, JObject ReturnVal )
        {
            string NtName = "nodetype_" + NodeType.NodeTypeId;
            ReturnVal[NtName] = new JObject();
            ReturnVal[NtName]["id"] = NodeType.NodeTypeId;
            ReturnVal[NtName]["name"] = NodeType.NodeTypeName;
            ReturnVal[NtName]["objectclass"] = NodeType.ObjectClass.ObjectClass.ToString();
            ReturnVal[NtName]["objectclassid"] = NodeType.ObjectClass.ObjectClassId.ToString();
        }

        public JObject createMetaDataCollectionByVerticalDesign( CswNbtMetaDataObjectClass ObjectClass, string NodeTypeName, string Category )
        {
            JObject Ret = new JObject();
            if( _CswNbtResources.CurrentNbtUser.Username == CswNbtObjClassUser.ChemSWAdminUsername )
            {
                switch( ObjectClass.ObjectClass )
                {
                    case CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass:
                        _createInspectionCollection( NodeTypeName, Category, Ret );
                        break;
                    default:
                        throw new CswDniException( ErrorType.Warning, "This operation is only permitted for Inspection Targets", "Only the InspectionTargetClass implements this feature" );

                }
            }
            else
            {
                throw new CswDniException( ErrorType.Warning, "This operation is only permitted for the chemsw_admin user", "Cannot create MetaData collection for non-cswadmin users" );
            }
            return Ret;
        }

        private void _createInspectionCollection( string NodeTypeName, string Category, JObject Ret )
        {
            if( false == string.IsNullOrEmpty( NodeTypeName ) )
            {
                string InspectionTargetName = _checkUniqueNodeType( NodeTypeName.Trim() );
                string InspectionGroupName = _checkUniqueNodeType( InspectionTargetName + " Group" );
                string InspectionName = _checkUniqueNodeType( InspectionTargetName + " Inspection" );
                string InspectionScheduleName = _checkUniqueNodeType( InspectionTargetName + " Schedule" );
                string InspectionRouteName = _checkUniqueNodeType( InspectionTargetName + " Route" );

                //if we're here, we're validated
                CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                CswNbtMetaDataObjectClass InspectionTargetGroupOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass );
                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtMetaDataObjectClass InspectionRouteOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionRouteClass );
                CswNbtMetaDataObjectClass GeneratorOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );

                //Create the new NodeTypes
                CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetOc.ObjectClassId, InspectionTargetName, Category );
                CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionTargetGroupOc.ObjectClassId, InspectionGroupName, Category );
                CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionDesignOc.ObjectClassId, InspectionName, Category );
                CswNbtMetaDataNodeType InspectionRouteNt = _CswNbtResources.MetaData.makeNewNodeType( InspectionRouteOc.ObjectClassId, InspectionRouteName, Category );
                CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.makeNewNodeType( GeneratorOc.ObjectClassId, InspectionScheduleName, Category );

                //Set new InspectionTarget Props and Tabs
                CswNbtMetaDataNodeTypeProp ItInspectionGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                ItInspectionGroupNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp ItRouteNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetNt, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Route", InspectionTargetNt.getFirstNodeTypeTab().TabId );
                ItRouteNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionRouteNt.NodeTypeId );
                CswNbtMetaDataNodeTypeTab InspectionsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetNt, InspectionName, 2 );
                CswNbtMetaDataNodeTypeProp ItInspectionsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionName, InspectionsTab.TabId );

                //Set InspectionTargetGroup Props and Tabs
                _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Text, "Description", InspectionTargetGroupNt.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeTab ItgLocationsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Locations", 2 );
                CswNbtMetaDataNodeTypeProp ItgLocationsNtp = _CswNbtResources.MetaData.makeNewProp( InspectionTargetGroupNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionTargetName + " Locations", ItgLocationsTab.TabId );

                //Set InspectionDesign Props and Tabs
                CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                IdTargetNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp IdGeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
                IdGeneratorNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), GeneratorNt.NodeTypeId );

                //Set InspectionRoute Props and Tabs
                CswNbtMetaDataNodeTypeProp IrInspectorNtp = _CswNbtResources.MetaData.makeNewProp( InspectionRouteNt, CswNbtMetaDataFieldType.NbtFieldType.Relationship, "Inspector", InspectionRouteNt.getFirstNodeTypeTab().TabId );
                IrInspectorNtp.SetFK( CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass ).ObjectClassId );
                CswNbtMetaDataNodeTypeTab IrTargetTab = _CswNbtResources.MetaData.makeNewTab( InspectionRouteNt, InspectionTargetName, 2 );
                CswNbtMetaDataNodeTypeProp IrTargetNtp = _CswNbtResources.MetaData.makeNewProp( InspectionRouteNt, CswNbtMetaDataFieldType.NbtFieldType.Grid, InspectionTargetName, IrTargetTab.TabId );

                //Set Generator Props
                CswNbtMetaDataNodeTypeProp GnOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                GnOwnerNtp.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
                CswNbtMetaDataNodeTypeProp GnParentTypeNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentTypePropertyName );
                GnParentTypeNtp._DataRow[CswNbtMetaDataNodeTypeProp._Element_DefaultValue.ToString()] = InspectionTargetNt.NodeTypeId.ToString();
                CswNbtMetaDataNodeTypeProp GnTargetTypeNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.TargetTypePropertyName );
                GnTargetTypeNtp._DataRow[CswNbtMetaDataNodeTypeProp._Element_DefaultValue.ToString()] = InspectionDesignNt.NodeTypeId.ToString();
                CswNbtMetaDataNodeTypeProp GnParentViewNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.ParentViewPropertyName );

                //Views


            }

        }

        private string _checkUniqueNodeType( string NewNodeTypeName )
        {
            string Ret = string.Empty;
            CswNbtMetaDataNodeType NewNodeType = _CswNbtResources.MetaData.getNodeType( NewNodeTypeName );
            if( null != NewNodeType )
            {
                throw new CswDniException( ErrorType.Error, "Cannot create duplicate target.", "Cannot Create a new Inspection Target of name: " + NewNodeTypeName + ", because Target: " + NewNodeType.NodeTypeName + " already exists." );
            }
            else
            {
                Ret = NewNodeTypeName;
            }
            return Ret;
        }

    } // class CswNbtWebServiceMetaData

} // namespace ChemSW.Nbt.WebServices
