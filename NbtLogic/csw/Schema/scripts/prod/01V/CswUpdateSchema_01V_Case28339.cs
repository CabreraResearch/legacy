using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28339
    /// </summary>
    public class CswUpdateSchema_01V_Case28339 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28339; }
        }

        public override void update()
        {
            //All existing suppliers should have a vendor type of 'Sales'
            CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            foreach( CswNbtObjClassVendor vendorNode in vendorOC.getNodes( false, false, false, true ) )
            {
                vendorNode.VendorType.Value = CswNbtObjClassVendor.VendorTypes.Sales;
                vendorNode.postChanges( false );
            }

            //Remove the view filter on Materials
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp supplierOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );

            bool updatedOCP = false;
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp supplierNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
                CswNbtView oldSupplierView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( supplierNTP.ViewId );
                if( null != oldSupplierView )
                {
                    oldSupplierView.Delete();
                }
                CswNbtView newSupplierView = _CswNbtSchemaModTrnsctn.makeNewView( "Supplier", NbtViewVisibility.Property );
                newSupplierView.AddViewRelationship( vendorOC, true );
                newSupplierView.save();

                supplierNTP.ViewId = newSupplierView.ViewId;

                if( false == updatedOCP )
                {
                    updatedOCP = true;
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( supplierOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, newSupplierView.ToString() );
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28339

}//namespace ChemSW.Nbt.Schema