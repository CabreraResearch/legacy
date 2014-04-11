using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52097A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.AE; }
        }

        public override int CaseNo
        {
            get { return 52097; }
        }

        public override string Title
        {
            get { return "Adding a grid property named containers to the Receipt Lot OC" + CaseNo; }
        }

        public override string AppendToScriptName()
        {
            return "AE";
        }

        public override void update()
        {
            

            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass);

            CswNbtMetaDataObjectClassProp ContainerNTP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ReceiptLotOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassReceiptLot.PropertyName.Containers,
                    FieldType = CswEnumNbtFieldType.Grid
                } );

            CswNbtMetaDataObjectClassProp ReceiptLotPropOnContainer = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( ContainerOC.ObjectClassId, CswNbtObjClassContainer.PropertyName.ReceiptLot);

            CswNbtView ContainersView = _CswNbtSchemaModTrnsctn.makeSafeView("GridPropContainerReceiptLot", CswEnumNbtViewVisibility.Property);
            ContainersView.SetViewMode( CswEnumNbtViewRenderingMode.Grid );

            CswNbtViewRelationship ReceiptLotRel = ContainersView.AddViewRelationship( ReceiptLotOC, false);
            CswNbtViewRelationship ContainerRel = ContainersView.AddViewRelationship( ReceiptLotRel, CswEnumNbtViewPropOwnerType.Second, ReceiptLotPropOnContainer, true);

            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode ), 1 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Material), 2 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner), 3 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.ExpirationDate), 4 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status), 5 );
            ContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity), 6);

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ContainerNTP, CswEnumNbtObjectClassPropAttributes.viewxml, ContainersView.ToString() );
        }
    }
}


