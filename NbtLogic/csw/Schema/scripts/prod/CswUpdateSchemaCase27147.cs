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

            string valueOpts = "/NbtWebApp/Images/cispro/0w.gif\n/NbtWebApp/Images/cispro/1o.gif\n/NbtWebApp/Images/cispro/2y.gif\n/NbtWebApp/Images/cispro/3g.gif\n/NbtWebApp/Images/cispro/4b.gif\n/NbtWebApp/Images/cispro/5l.gif\n/NbtWebApp/Images/cispro/6p.gif\n/NbtWebApp/Images/cispro/7r.gif";
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( storageCompatibilityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.valueoptions, valueOpts );

        }//Update()

    }//class CswUpdateSchemaCase27147

}//namespace ChemSW.Nbt.Schema