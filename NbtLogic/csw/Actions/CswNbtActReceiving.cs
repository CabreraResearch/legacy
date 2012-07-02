using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActReceiving
    {


        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtMetaDataObjectClass _ContainerOc = null;
        private CswNbtMetaDataObjectClass _MaterialOc = null;
        private CswNbtMetaDataObjectClass _SizeOc = null;
        private CswPrimaryKey _MaterialId = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActReceiving( CswNbtResources CswNbtResources, CswNbtMetaDataObjectClass MaterialOc, CswPrimaryKey MaterialNodeId )
        {
            _CswNbtResources = CswNbtResources;
            if( false == _CswNbtResources.IsModuleEnabled( CswNbtResources.CswNbtModule.CISPro ) )
            {
                throw new CswDniException( ErrorType.Error, "Cannot use the Submit Request action without the required module.", "Attempted to constuct CswNbtActReceiving without the required module." );
            }

            _MaterialOc = MaterialOc;
            _MaterialId = MaterialNodeId;
            _ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            _SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
        }

        #endregion Constructor

        #region Public methods and props

        public CswNbtView SizesView
        {
            get
            {
                CswNbtView SizeView = new CswNbtView( _CswNbtResources );
                SizeView.Visibility = NbtViewVisibility.Property;
                SizeView.ViewMode = NbtViewRenderingMode.Grid;

                CswNbtViewRelationship MaterialRel = SizeView.AddViewRelationship( _MaterialOc, true );
                CswNbtMetaDataObjectClass SizeOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
                CswNbtMetaDataObjectClassProp CapacityOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CapacityPropertyName );
                CswNbtMetaDataObjectClassProp MaterialOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.MaterialPropertyName );
                CswNbtMetaDataObjectClassProp CatalogNoOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.CatalogNoPropertyName );
                CswNbtMetaDataObjectClassProp DispensableOcp = SizeOc.getObjectClassProp( CswNbtObjClassSize.DispensablePropertyName );

                CswNbtViewRelationship SizeRel = SizeView.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, MaterialOcp, true );
                SizeView.AddViewProperty( SizeRel, CapacityOcp );
                CswNbtViewProperty DispensableVp = SizeView.AddViewProperty( SizeRel, DispensableOcp );
                DispensableVp.ShowInGrid = false;
                SizeView.AddViewPropertyFilter( DispensableVp, DispensableOcp.getFieldTypeRule().SubFields.Default.Name, Value: Tristate.True.ToString() );
                SizeView.AddViewProperty( SizeRel, CatalogNoOcp );
                SizeView.SaveToCache( false );
                return SizeView;
            }
        }

        /// <summary>
        /// Instance a new container according to Object Class rules. Note: this does not get the properties.
        /// </summary>
        public CswNbtObjClassContainer makeContainer( CswNbtMetaDataNodeType ContainerNt = null )
        {
            CswNbtObjClassContainer RetAsContainer = null;
            CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );

            ContainerNt = ContainerNt ?? _ContainerOc.getLatestVersionNodeTypes().FirstOrDefault();
            if( null != ContainerNt )
            {
                RetAsContainer = PropsAction.getAddNode( ContainerNt );
                if( null == RetAsContainer )
                {
                    throw new CswDniException( ErrorType.Error, "Could not create a new container.", "Failed to create a new Container node." );
                }
                RetAsContainer.Material.RelatedNodeId = _MaterialId;
                RetAsContainer.Material.Hidden = true;
                RetAsContainer.Size.Hidden = true;
            }
            return RetAsContainer;
        }

        /// <summary>
        /// Get the Add Layout properties for a container
        /// </summary>
        public JObject getContainerAddProps( CswNbtObjClassContainer Container )
        {
            JObject Ret = new JObject();
            if( null != Container )
            {
                CswNbtSdTabsAndProps PropsAction = new CswNbtSdTabsAndProps( _CswNbtResources );
                _CswNbtResources.EditMode = NodeEditMode.Add;
                Ret = PropsAction.getProps( Container.Node, "", null, CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
            }
            return Ret;
        }

        #endregion Public methods and props
    }


}
