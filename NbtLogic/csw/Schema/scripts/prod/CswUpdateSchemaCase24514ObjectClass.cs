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
                                                      FkValue = InventoryGroupOc.ObjectClassId
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

            CswNbtMetaDataObjectClassProp MaterialOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                           new CswNbtWcfMetaDataModel.ObjectClassProp
                           {
                               PropName = CswNbtObjClassRequestItem.PropertyName.Material,
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
                                        PropName = CswNbtObjClassRequestItem.PropertyName.Container,
                                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                        IsFk = true,
                                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                        FkValue = ContainerOc.ObjectClassId,
                                        SetValOnAdd = true
                                    }
                );

            CswNbtMetaDataObjectClassProp TypeOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                     new CswNbtWcfMetaDataModel.ObjectClassProp
                                     {
                                         PropName = CswNbtObjClassRequestItem.PropertyName.Type,
                                         FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                                         SetValOnAdd = true,
                                         ListOptions = CswNbtObjClassRequestItem.Types.Options.ToString(),
                                         ReadOnly = true
                                     } );

            CswNbtMetaDataObjectClassProp LocationOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Location,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location,
                        IsFk = true,
                        FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = LocationOc.ObjectClassId,
                        SetValOnAdd = true,
                        IsRequired = true
                    }
                 );

            CswNbtMetaDataObjectClassProp RequestOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                                  new CswNbtWcfMetaDataModel.ObjectClassProp
                                                  {
                                                      PropName = CswNbtObjClassRequestItem.PropertyName.Request,
                                                      FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                      IsFk = true,
                                                      FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                      FkValue = RequestOc.ObjectClassId,
                                                      ServerManaged = true
                                                  } );

            CswNbtMetaDataObjectClassProp NumberOcp =
                 _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                     new CswNbtWcfMetaDataModel.ObjectClassProp
                     {
                         PropName = CswNbtObjClassRequestItem.PropertyName.Number,
                         FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                         IsUnique = true,
                         IsGlobalUnique = true,
                         ServerManaged = true
                     }
                  );

            CswNbtMetaDataObjectClassProp StatusOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Status,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                        ListOptions = CswNbtObjClassRequestItem.Statuses.Options.ToString(),
                        ServerManaged = true
                    }
                 );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOcp, CswNbtSubField.SubFieldName.Value, CswNbtObjClassRequestItem.Statuses.Pending );

            CswNbtMetaDataObjectClassProp RequestByOcp =
                 _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                                 new CswNbtWcfMetaDataModel.ObjectClassProp
                                                 {
                                                     PropName = CswNbtObjClassRequestItem.PropertyName.RequestBy,
                                                     FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                                                     ListOptions = CswNbtObjClassRequestItem.RequestsBy.Options.ToString(),
                                                     IsRequired = true,
                                                     SetValOnAdd = true
                                                 } );

            char FilterDelimiter = '|';
            CswNbtMetaDataObjectClassProp CountOcp =
               _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                   new CswNbtWcfMetaDataModel.ObjectClassProp
                   {
                       PropName = CswNbtObjClassRequestItem.PropertyName.Count,
                       FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                       NumberPrecision = 0,
                       NumberMinValue = 1,
                       SetValOnAdd = true,
                       Filter = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.Equals + FilterDelimiter + CswNbtObjClassRequestItem.RequestsBy.Size,
                       FilterPropId = RequestByOcp.PropId
                   }
               );

            CswNbtMetaDataObjectClassProp SizeOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                               new CswNbtWcfMetaDataModel.ObjectClassProp
                               {
                                   PropName = CswNbtObjClassRequestItem.PropertyName.Size,
                                   FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                   IsFk = true,
                                   FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                   FkValue = SizeOc.ObjectClassId,
                                   SetValOnAdd = true,
                                   Filter = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.Equals + FilterDelimiter + CswNbtObjClassRequestItem.RequestsBy.Size,
                                   FilterPropId = RequestByOcp.PropId
                               } );

            CswNbtMetaDataObjectClassProp QuantityOcp =
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                                   new CswNbtWcfMetaDataModel.ObjectClassProp
                                   {
                                       PropName = CswNbtObjClassRequestItem.PropertyName.Quantity,
                                       FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                                       SetValOnAdd = true,
                                       Filter = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.NotEquals + FilterDelimiter + CswNbtObjClassRequestItem.RequestsBy.Size,
                                       FilterPropId = RequestByOcp.PropId
                                   } );

            CswNbtMetaDataObjectClassProp ExternalOrderNumberOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                    }
                 );

            CswNbtMetaDataObjectClassProp CommentsOcp =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc,
                    new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassRequestItem.PropertyName.Comments,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments,
                        SetValOnAdd = false
                    }
                 );





            CswNbtMetaDataObjectClassProp AssgnedToOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassRequestItem.PropertyName.AssignedTo,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UserOc.ObjectClassId
            } );

            CswNbtMetaDataObjectClassProp RequestGroupOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.InventoryGroup.ToString() );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestItemOc )
            {
                PropName = CswNbtObjClassRequestItem.PropertyName.InventoryGroup,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
                IsFk = true,
                FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                FkValue = RequestOcp.PropId,
                ValuePropId = RequestGroupOcp.PropId,
                ValuePropType = NbtViewPropIdType.ObjectClassPropId.ToString()
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestItemOc )
            {
                PropName = CswNbtObjClassRequestItem.PropertyName.TotalDispensed,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                ServerManaged = true
            } );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, RequestItemOc.ObjectClassId );

            #endregion Request Item
        }//Update()

    }//class CswUpdateSchemaCase24514ObjectClass

}//namespace ChemSW.Nbt.Schema