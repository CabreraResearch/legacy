using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27439
    /// </summary>
    public class CswUpdateSchemaCase27439 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.VendorClass );

            /* Create the new properties Corporate Entity and Vendor Type */
            CswNbtMetaDataObjectClassProp corporateEntityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( vendorOC )
            {
                PropName = "Corporate Entity",
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );

            CswNbtMetaDataObjectClassProp vendorTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( vendorOC )
            {
                PropName = "Vendor Type",
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "Sales,Corporate,Technical,Manufacturing"
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( vendorTypeOCP, vendorTypeOCP.getFieldTypeRule().SubFields.Default.Name, "Sales" );

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema