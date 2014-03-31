using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateSchemaDDL_02M_CIS52772 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52772; }
        }

        public override string Title
        {
            get { return "Adjust layout of Container NT" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );

                foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeTab firstTab = ContainerNT.getFirstNodeTypeTab();

                    CswNbtMetaDataNodeTypeTab identityTab = ContainerNT.getIdentityTab();

                    CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode);
                    BarcodeNTP.removeFromAllLayouts();
                    BarcodeNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, identityTab.TabId, 1, 1 );

                    CswNbtMetaDataNodeTypeProp MaterialNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );
                    MaterialNTP.removeFromAllLayouts();
                    MaterialNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, identityTab.TabId, 2, 1 );

                    CswNbtMetaDataNodeTypeProp QuantityNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity);
                    QuantityNTP.removeFromAllLayouts();
                    QuantityNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, identityTab.TabId, 3, 1 );



                    CswNbtMetaDataNodeTypeProp OwnerNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner );
                    OwnerNTP.removeFromAllLayouts();
                    OwnerNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 1, 1 );

                    CswNbtMetaDataNodeTypeProp LocationNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Location );
                    LocationNTP.removeFromAllLayouts();
                    LocationNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 2, 1 );
                    CswNbtMetaDataNodeTypeProp ExpDateNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate );
                    ExpDateNTP.removeFromAllLayouts();
                    ExpDateNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 3, 1 );

                    CswNbtMetaDataNodeTypeProp DateCreatedNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.DateCreated );
                    DateCreatedNTP.removeFromAllLayouts();
                    DateCreatedNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 4, 1 );

                    CswNbtMetaDataNodeTypeProp LabelFormatNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );
                    LabelFormatNTP.removeFromAllLayouts();
                    LabelFormatNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 7, 1 );

                    CswNbtMetaDataNodeTypeProp SaveChangesNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Save );
                    SaveChangesNTP.removeFromAllLayouts();
                    SaveChangesNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 9, 1 );

                    CswNbtMetaDataNodeTypeProp MissingNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Missing );
                    MissingNTP.removeFromAllLayouts();
                    MissingNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 2, 2 );

                    CswNbtMetaDataNodeTypeProp StatusNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Status );
                    StatusNTP.removeFromAllLayouts();
                    StatusNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 3, 2 );

                    CswNbtMetaDataNodeTypeProp RecieptLotNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ReceiptLot );
                    RecieptLotNTP.removeFromAllLayouts();
                    RecieptLotNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 4, 2 );

                    CswNbtMetaDataNodeTypeProp ContainerGroupNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ContainerGroup );
                    ContainerGroupNTP.removeFromAllLayouts();
                    ContainerGroupNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 5, 2 );

                    CswNbtMetaDataNodeTypeProp ContainerFamilyNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ContainerFamily );
                    ContainerFamilyNTP.removeFromAllLayouts();
                    ContainerFamilyNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 6, 2 );

                    CswNbtMetaDataNodeTypeProp DisposeContainerNTP = ContainerNT.getNodeTypePropByObjectClassProp
( CswNbtObjClassContainer.PropertyName.Dispose );
                    DisposeContainerNTP.removeFromAllLayouts();
                    DisposeContainerNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, firstTab.TabId, 8, 2 );
                }
            }
        }
    }
}


