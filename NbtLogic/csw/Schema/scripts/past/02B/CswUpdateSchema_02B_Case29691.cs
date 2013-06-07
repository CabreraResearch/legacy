using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29691
    /// Based off of 01P/CswUpdateSchemaCase26834
    /// </summary>
    public class CswUpdateSchema_02B_Case29691 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override Int32 CaseNo
        {
            get { return 26961; }
        }

        public override void update()
        {
            // Fix graphics URLs for Storage Compatibility (remove "/NbtWebApp")

            CswDelimitedString ImageUrls = new CswDelimitedString( '\n' )
                {
                    "Images/cispro/0w.gif",
                    "Images/cispro/1o.gif",
                    "Images/cispro/2y.gif",
                    "Images/cispro/3g.gif",
                    "Images/cispro/4b.gif",
                    "Images/cispro/5l.gif",
                    "Images/cispro/6p.gif",
                    "Images/cispro/7r.gif"
                };

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );

            CswNbtMetaDataObjectClassProp ChemStorCompatOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.StorageCompatibility );
            CswNbtMetaDataObjectClassProp LocStorCompatOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.StorageCompatibility );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ChemStorCompatOCP, CswEnumNbtObjectClassPropAttributes.valueoptions, ImageUrls.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocStorCompatOCP, CswEnumNbtObjectClassPropAttributes.valueoptions, ImageUrls.ToString() );

        }//Update()


    }//class CswUpdateSchema_02B_Case29691

}//namespace ChemSW.Nbt.Schema