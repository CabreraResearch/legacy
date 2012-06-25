using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26834
    /// </summary>
    public class CswUpdateSchemaCase26834 : CswUpdateSchemaTo
    {
        public override void update()
        {
            // Add graphics to storage compatibility property on Materials.
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp StorCompatOCP = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.StorageCompatibilityPropertyName );

            CswDelimitedString ImageNames = new CswDelimitedString( '\n' ) {
                "0 - None",
		        "1 - Inorganic Acids",
		        "2 - Organic Acids",
		        "3 - Bases",
		        "4 - Oxidizing Inorganic Acids",
		        "5 - Oxidizers",
		        "6 - Toxics",
		        "7 - Flammables"
            };

            CswDelimitedString ImageUrls = new CswDelimitedString( '\n' ) {
                "/NbtWebApp/Images/cispro/0w.gif",
                "/NbtWebApp/Images/cispro/1o.gif",
                "/NbtWebApp/Images/cispro/2y.gif",
                "/NbtWebApp/Images/cispro/3g.gif",
                "/NbtWebApp/Images/cispro/4b.gif",
                "/NbtWebApp/Images/cispro/5l.gif",
                "/NbtWebApp/Images/cispro/6p.gif",
                "/NbtWebApp/Images/cispro/7r.gif"
            };

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StorCompatOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, ImageNames.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StorCompatOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.valueoptions, ImageUrls.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StorCompatOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.textarearows, 50 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StorCompatOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.textareacols, 50 );

        }//Update()

    }//class CswUpdateSchemaCase26834

}//namespace ChemSW.Nbt.Schema