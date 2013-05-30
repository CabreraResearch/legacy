using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CofA Module
    /// </summary>
    public class CswNbtModuleRuleCofA : CswNbtModuleRule
    {
        public CswNbtModuleRuleCofA( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.CofA; } }

        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.MLM ) )
            {
                _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.MLM );
            }

            //Show the following ReceiptLot properties...
            //   Assigned CofA
            //   View CofA
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( CswNbtMetaDataNodeType ReceiptLotNT in ReceiptLotOC.getNodeTypes() )
            {
                _CswNbtResources.Modules.AddPropToTab( ReceiptLotNT.NodeTypeId, "Assigned C of A", ReceiptLotNT.getFirstNodeTypeTab(), 7, 1 );
                _CswNbtResources.Modules.AddPropToTab( ReceiptLotNT.NodeTypeId, "View C of A", ReceiptLotNT.getIdentityTab(), 1, 1 );
            }

            //Show the following Container properties...
            //   View SDS
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(CswEnumNbtObjectClass.ContainerClass);
                foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
                {
                    _CswNbtResources.Modules.AddPropToTab( ContainerNT.NodeTypeId, "View C of A", ContainerNT.getIdentityTab(), 3, 1 );
                }
            }
        }

        public override void OnDisable()
        {
            //Hide the following ReceiptLot properties...
            //   Assigned CofA
            //   View CofA
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            foreach( int ReceiptLotId in ReceiptLotOC.getNodeTypeIds() )
            {
                _CswNbtResources.Modules.HideProp( ReceiptLotId, "Assigned C of A" );
                _CswNbtResources.Modules.HideProp( ReceiptLotId, "View C of A" );
            }

            //Hide the following Container properties...
            //   View CofA
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtResources.MetaData.getObjectClass(CswEnumNbtObjectClass.ContainerClass);
                foreach( int ContainerNTId in ContainerOC.getNodeTypeIds() )
                {
                    _CswNbtResources.Modules.HideProp( ContainerNTId, "View C of A" );
                }
            }
        } // OnDisable()
    } // class CswNbtModuleRuleCofA
}// namespace ChemSW.Nbt
