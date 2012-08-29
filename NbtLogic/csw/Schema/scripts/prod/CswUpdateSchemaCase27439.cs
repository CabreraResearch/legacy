using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                IsRequired = false,
            } );

            CswNbtMetaDataObjectClassProp vendorTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( vendorOC )
            {
                PropName = "Vendor Type",
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "Sales,Corporate,Technical,Manufacturing",
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( vendorTypeOCP, vendorTypeOCP.getFieldTypeRule().SubFields.Default.Name, "Sales" );

            /* Get the Vendor NT and hide the newly created properties */
            cswnbtmetadatanodetype vendornt = _cswnbtschemamodtrnsctn.metadata.getnodetype( "vendor" );
            if( null != vendornt )
            {
                cswnbtmetadatanodetypeprop corporateentityntp = _cswnbtschemamodtrnsctn.metadata.getnodetypepropbyobjectclassprop( vendornt.nodetypeid, corporateentityocp.objectclasspropid );
                corporateentityntp.removefromalllayouts();

                cswnbtmetadatanodetypeprop vendortypentp = _cswnbtschemamodtrnsctn.metadata.getnodetypepropbyobjectclassprop( vendornt.nodetypeid, vendortypeocp.objectclasspropid );
                vendortypentp.removefromalllayouts();
            }

        }//Update()

    }

}//namespace ChemSW.Nbt.Schema