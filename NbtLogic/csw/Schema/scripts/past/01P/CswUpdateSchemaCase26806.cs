using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26806
    /// </summary>
    public class CswUpdateSchemaCase26806 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass materialCompnentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass );

            CswNbtMetaDataObjectClassProp mixtureOCP = materialCompnentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.MixturePropertyName );
            CswNbtMetaDataObjectClassProp constituentOCP = materialCompnentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.ConstituentPropertyName );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constituentOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.iscompoundunique, true );


        }//Update()

    }//class CswUpdateSchemaCase26806

}//namespace ChemSW.Nbt.Schema