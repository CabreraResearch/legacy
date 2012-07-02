using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24446Container
    /// </summary>
    public class CswUpdateSchemaCase24446Container : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp LocationOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            CswNbtMetaDataObjectClassProp ExpirationDateOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.ExpirationDatePropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ExpirationDateOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp OwnerOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( ContainerOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                                                                    {
                                                                                                                        PropName = CswNbtObjClassContainer.OwnerPropertyName,
                                                                                                                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                                                                                        IsFk = true,
                                                                                                                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                                                                                        FkValue = UserOc.ObjectClassId,
                                                                                                                        IsRequired = true,
                                                                                                                        SetValOnAdd = true
                                                                                                                    } );
            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswNbtResources.ConfigurationVariables.container_receipt_limit, "Limit the number of containers allowed at receipt.", "25", false );
        }//Update()

    }//class CswUpdateSchemaCase24446Container

}//namespace ChemSW.Nbt.Schema