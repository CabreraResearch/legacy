using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebAppServices.Response;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514ObjectClass : CswUpdateSchemaTo
    {
        public override void update()
        {
            #region Request

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                          IconFileName: "docs.gif",
                                                          AuditLevel: true,
                                                          UseBatchEntry: false );
            CswNbtMetaDataObjectClassProp RequestorOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                     new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.Requestor.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = UserOc.ObjectClassId,
                            SetValOnAdd = true,
                            IsRequired = true
                        } );

            CswNbtMetaDataObjectClassProp NameOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                             new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.Name.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                            SetValOnAdd = true
                        } );

            CswNbtMetaDataObjectClassProp SubmittedDateOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                             new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime
                        } );

            CswNbtMetaDataObjectClassProp CompletedDateOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                              new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.CompletedDate.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime
                        } );

            CswNbtMetaDataObjectClassProp InventoryGroupOcp =
               _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                  new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.InventoryGroup.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = InventoryGroupOc.ObjectClassId,
                            SetValOnAdd = true
                        } );

            #endregion Request
            #region Request Item
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                                                          IconFileName: "docs.gif",
                                                          AuditLevel: true,
                                                          UseBatchEntry: false );

            CswNbtMetaDataObjectClassProp RequestOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                                                  new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Request.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = RequestOc.ObjectClassId,
                            SetValOnAdd = true
                        } );

            CswNbtMetaDataObjectClassProp TypeOcp =
                 _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                                                 new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Type.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                            ListOptions = CswNbtObjClassRequestItem.TypeOptions.ToString(),
                            SetValOnAdd = true,
                            IsRequired = true
                        } );

            CswNbtMetaDataObjectClassProp QuantityOcp =
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                                   new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Quantity.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                            SetValOnAdd = true,
                            IsRequired = true
                        } );

            CswNbtMetaDataObjectClassProp SizeOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                               new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Size.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = SizeOc.ObjectClassId,
                            SetValOnAdd = true,
                            IsRequired = true
                        } );

            CswNbtMetaDataObjectClassProp MaterialOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                           new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Material.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = MaterialOc.ObjectClassId
                        }
                 );


            CswNbtMetaDataObjectClassProp ContainerOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Container.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = ContainerOc.ObjectClassId
                        }
                );


            CswNbtMetaDataObjectClassProp LocationOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Location.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = LocationOc.ObjectClassId
                        }
                 );

            CswNbtMetaDataObjectClassProp CountOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Quantity.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                        NumberPrecision = 0,
                        NumberMinValue = 1
                    }
                );

            CswNbtMetaDataObjectClassProp CommentsOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Comments.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments
                    }
                 );

            CswNbtMetaDataObjectClassProp StatusOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Status.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                        ListOptions = CswNbtObjClassRequestItem.StatusOptions.ToString()
                    }
                 );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOcp, CswNbtSubField.SubFieldName.Value, CswNbtObjClassRequestItem.Statuses.Pending.ToString() );

            CswNbtMetaDataObjectClassProp NumberOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Number.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                        IsUnique = true,
                        IsGlobalUnique = true
                    }
                 );

            CswNbtMetaDataObjectClassProp ExternalOrderNumberOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass,
                    new CswNbtWcfObjectClassDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                    }
                 );

            #endregion Request Item
        }//Update()

    }//class CswUpdateSchemaCase24514ObjectClass

}//namespace ChemSW.Nbt.Schema