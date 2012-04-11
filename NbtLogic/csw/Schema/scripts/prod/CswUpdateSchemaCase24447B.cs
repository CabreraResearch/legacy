using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24488B
    /// </summary>
    public class CswUpdateSchemaCase24488B : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            
            // set up Container Location FK
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp ContainerLocationOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ContainerLocationOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ContainerLocationOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ContainerLocationOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, LocationOC.ObjectClassId );

            // set up User Default Location FK
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp UserDefLocOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.DefaultLocationPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserDefLocOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserDefLocOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UserDefLocOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, LocationOC.ObjectClassId );

        }//Update()

    }//class CswUpdateSchemaCase24488B

}//namespace ChemSW.Nbt.Schema