using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the ManufacturerLotInfo Module
    /// </summary>
    public class CswNbtModuleRuleManufacturerLotInfo: CswNbtModuleRule
    {
        public CswNbtModuleRuleManufacturerLotInfo( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.ManufacturerLotInfo; } }

        protected override void OnEnable()
        {
            //Show the following ReceiptLot properties...
            //   Manufacturer
            //   Manufacturer Lot No
            //   Manufactured Date
            //   Assigned CofA
            //   View CofA
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.ShowProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.Manufacturer );
                _CswNbtResources.Modules.ShowProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.ManufacturerLotNo );
                _CswNbtResources.Modules.ShowProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate );
                _CswNbtResources.Modules.ShowProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.AssignedCofA );
                _CswNbtResources.Modules.ShowProp( ReceiptLotNT.NodeTypeId, CswNbtObjClassReceiptLot.PropertyName.ViewCofA );
            }

            //Show the following Container properties...
            //   View CofA
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.ShowProp( ContainerNT.NodeTypeId, CswNbtObjClassContainer.PropertyName.ViewCofA );
            }
        }

        protected override void OnDisable()
        {
            //Hide the following ReceiptLot properties...
            //   Manufacturer
            //   Manufacturer Lot No
            //   Manufactured Date
            //   Assigned CofA
            //   View CofA
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( int ReceiptLotId in ReceiptLotOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( ReceiptLotId, CswNbtObjClassReceiptLot.PropertyName.Manufacturer );
                _CswNbtResources.Modules.HideProp( ReceiptLotId, CswNbtObjClassReceiptLot.PropertyName.ManufacturerLotNo );
                _CswNbtResources.Modules.HideProp( ReceiptLotId, CswNbtObjClassReceiptLot.PropertyName.ManufacturedDate );
                _CswNbtResources.Modules.HideProp( ReceiptLotId, CswNbtObjClassReceiptLot.PropertyName.AssignedCofA );
                _CswNbtResources.Modules.HideProp( ReceiptLotId, CswNbtObjClassReceiptLot.PropertyName.ViewCofA );
            }

            //Hide the following Container properties...
            //   View CofA
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( ContainerNTId, CswNbtObjClassContainer.PropertyName.ViewCofA );
            }
        } // OnDisable()
    } // class CswNbtModuleRuleCofA
}// namespace ChemSW.Nbt
