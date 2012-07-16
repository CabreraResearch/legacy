using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27147
    /// </summary>
    public class CswUpdateSchemaCase27147 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass locationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp storageCompatibilityOCP = locationOC.getObjectClassProp( CswNbtObjClassLocation.StorageCompatabilityPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storageCompatibilityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, Tristate.True );

            string listOpts = "0 - None\n1 - Inorganic Acids\n2 - Organic Acids\n3 - Bases\n4 - Oxidizing Inorganic Acids\n5 - Oxidizers\n6 - Toxics\n7 - Flammables";
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storageCompatibilityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, listOpts );

        }//Update()

    }//class CswUpdateSchemaCase27147

}//namespace ChemSW.Nbt.Schema