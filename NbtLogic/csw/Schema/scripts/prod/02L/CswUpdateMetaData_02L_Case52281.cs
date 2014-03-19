using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52281: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52281; }
        }

        public override string Title
        {
            get { return "Create Manufacturer ObjectClass"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            CswNbtMetaDataObjectClass ManufacturerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ManufacturerClass );
            if( null == ManufacturerOC )
            {
                ManufacturerOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.ManufacturerClass, "box.png", true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.MLM, ManufacturerOC.ObjectClassId );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ManufacturerOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ManufacturerOC )
                {
                    PropName = CswNbtObjClassManufacturer.PropertyName.Material,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.PropertySetId.ToString(),
                    FkValue = MaterialPS.PropertySetId,
                    IsRequired = true
                } );
                CswNbtView ManufacturingSiteView = _CswNbtSchemaModTrnsctn.makeView();
                CswNbtViewRelationship parent = ManufacturingSiteView.AddViewRelationship( VendorOC, true );
                CswNbtMetaDataObjectClassProp VendorTypeOCP = VendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorTypeName );
                ManufacturingSiteView.AddViewPropertyAndFilter( parent,
                                                        MetaDataProp : VendorTypeOCP,
                                                        Value : CswNbtObjClassVendor.VendorTypes.Manufacturing,
                                                        FilterMode : CswEnumNbtFilterMode.Equals );
                ManufacturingSiteView.Visibility = CswEnumNbtViewVisibility.Property;
                ManufacturingSiteView.ViewName = "Manufacturing Site";
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ManufacturerOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ManufacturerOC )
                {
                    PropName = CswNbtObjClassManufacturer.PropertyName.ManufacturingSite,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = VendorOC.ObjectClassId,
                    IsRequired = true,
                    ViewXml = ManufacturingSiteView.ToString()
                } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( ManufacturerOC, new CswNbtWcfMetaDataModel.ObjectClassProp( ManufacturerOC )
                {
                    PropName = CswNbtObjClassManufacturer.PropertyName.Qualified,
                    FieldType = CswEnumNbtFieldType.Logical
                } );
            }
            CswNbtMetaDataNodeType ManufacturerNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( ManufacturerOC )
                                                                                                          {
                                                                                                              NodeTypeName = "Manufacturer",
                                                                                                              Category = "MLM"
                                                                                                          } );
        } // update()
    }
}//namespace ChemSW.Nbt.Schema