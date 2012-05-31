using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

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

            CswNbtMetaDataObjectClassProp NameOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestOc,
                                                 new CswNbtWcfMetaDataModel.ObjectClassProp
                                                 {
                                                     PropName = CswNbtObjClassRequest.PropertyName.Name.ToString(),
                                                     FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                                                 } );

            CswNbtMetaDataObjectClassProp RequestorOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestOc,
                                                     new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.Requestor.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = UserOc.ObjectClassId,
                            SetValOnAdd = true,
                            IsRequired = true
                        } );

            CswNbtMetaDataObjectClassProp InventoryGroupOcp =
               _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestOc,
                                                  new CswNbtWcfMetaDataModel.ObjectClassProp
                                                  {
                                                      PropName = CswNbtObjClassRequest.PropertyName.InventoryGroup.ToString(),
                                                      FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                      IsFk = true,
                                                      FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                      FkValue = InventoryGroupOc.ObjectClassId,
                                                      SetValOnAdd = true
                                                  } );

            CswNbtMetaDataObjectClassProp SubmittedDateOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestOc,
                                                             new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                            ServerManaged = true
                        } );

            CswNbtMetaDataObjectClassProp CompletedDateOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestOc,
                                                              new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequest.PropertyName.CompletedDate.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                            ServerManaged = true
                        } );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, RequestOc.ObjectClassId );

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
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                                  new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Request.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = RequestOc.ObjectClassId,
                            SetValOnAdd = true,
                            IsRequired = true,
                            ReadOnly = true
                        } );

            CswNbtMetaDataObjectClassProp TypeOcp =
                 _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                                 new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Type.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                            ListOptions = CswNbtObjClassRequestItem.TypeOptions.ToString(),
                            SetValOnAdd = true,
                            IsRequired = true,
                            ReadOnly = true
                        } );

            CswNbtMetaDataObjectClassProp StatusOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Status.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                        ListOptions = CswNbtObjClassRequestItem.StatusOptions.ToString(),
                        ServerManaged = true
                    }
                 );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOcp, CswNbtSubField.SubFieldName.Value, CswNbtObjClassRequestItem.Statuses.Pending.ToString() );

            CswNbtMetaDataObjectClassProp CountOcp =
               _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                   new CswNbtWcfMetaDataModel.ObjectClassProp
                   {
                       PropName = CswNbtObjClassRequestItem.PropertyName.Quantity.ToString(),
                       FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                       NumberPrecision = 0,
                       NumberMinValue = 1,
                       SetValOnAdd = true
                   }
               );

            CswNbtMetaDataObjectClassProp NumberOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Number.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                        IsUnique = true,
                        IsGlobalUnique = true,
                        SetValOnAdd = true
                    }
                 );

            CswNbtMetaDataObjectClassProp QuantityOcp =
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                   new CswNbtWcfMetaDataModel.ObjectClassProp
                                   {
                                       PropName = CswNbtObjClassRequestItem.PropertyName.Quantity.ToString(),
                                       FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                                       SetValOnAdd = true
                                   } );

            CswNbtMetaDataObjectClassProp ExternalOrderNumberOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                        SetValOnAdd = true
                    }
                 );

            CswNbtMetaDataObjectClassProp CommentsOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Comments.ToString(),
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments
                    }
                 );

            CswNbtMetaDataObjectClassProp SizeOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                               new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Size.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = SizeOc.ObjectClassId,
                            SetValOnAdd = true
                        } );

            /* Conditional properties don't support multiple filters, so this won't work.
            char FilterDelimiter = '|';
            string FileFilterString = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.Equals + FilterDelimiter + CswNbtObjClassRequestItem.Types.Dispense;
            */

            CswNbtMetaDataObjectClassProp MaterialOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                           new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Material.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = MaterialOc.ObjectClassId,
                            SetValOnAdd = true
                        }
                 );


            CswNbtMetaDataObjectClassProp ContainerOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                    new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Container.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = ContainerOc.ObjectClassId,
                            SetValOnAdd = true
                        }
                );

            char FilterDelimiter = '|';
            string LocationFilterString = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.NotEquals + FilterDelimiter + CswNbtObjClassRequestItem.Types.Dispose;
            CswNbtMetaDataObjectClassProp LocationOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                        {
                            PropName = CswNbtObjClassRequestItem.PropertyName.Location.ToString(),
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                            IsFk = true,
                            FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = LocationOc.ObjectClassId,
                            FilterPropId = TypeOcp.PropId,
                            Filter = LocationFilterString,
                            SetValOnAdd = true
                        }
                 );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, RequestItemOc.ObjectClassId );

            #endregion Request Item
        }//Update()

    }//class CswUpdateSchemaCase24514ObjectClass

}//namespace ChemSW.Nbt.Schema