using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case30533A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30533; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override string Title
        {
            get { return "Create new Request Item ObjectClass"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass RequestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            CswNbtMetaDataObjectClass EnterprisePartOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EnterprisePartClass );
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            if( null == RequestItemOC )
            {
                RequestItemOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RequestItemClass, "cart.png", true );
                #region Core OCPs
                CswNbtMetaDataObjectClassProp StatusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Status,
                    FieldType = CswEnumNbtFieldType.List,
                    ServerManaged = true,
                    ListOptions = CswNbtObjClassRequestItem.Statuses.Options.ToString()
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOCP, CswNbtObjClassRequestItem.Statuses.Pending, CswEnumNbtSubFieldName.Value );
                CswNbtMetaDataObjectClassProp TypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Type,
                    FieldType = CswEnumNbtFieldType.List,
                    ServerManaged = true,
                    ListOptions = CswNbtObjClassRequestItem.Types.Options.ToString()
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOCP, CswNbtObjClassRequestItem.Types.MaterialCreate, CswEnumNbtSubFieldName.Value );
                CswNbtMetaDataObjectClassProp RequestOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Request,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RequestOC.ObjectClassId,
                    ServerManaged = true
                } );
                CswNbtMetaDataObjectClassProp RequestNameOCP = RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name );
                CswNbtMetaDataObjectClassProp NameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Name,
                    FieldType = CswEnumNbtFieldType.PropertyReference,
                    IsFk = true,
                    FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = RequestOCP.ObjectClassPropId,
                    ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                    ValueFieldId = RequestNameOCP.PropId
                } );
                CswNbtMetaDataObjectClassProp ItemNumberOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.ItemNumber,
                    FieldType = CswEnumNbtFieldType.Sequence,
                    ServerManaged = true
                } );
                CswNbtMetaDataObjectClassProp DescriptionOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Description,
                    FieldType = CswEnumNbtFieldType.Static
                } );
                CswNbtMetaDataObjectClassProp RequestorOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Requestor,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClassProp RequestedForOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.RequestedFor,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOC.ObjectClassId,
                    SetValOnAdd = true
                } );
                CswNbtMetaDataObjectClassProp NeededByOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.NeededBy,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    SetValOnAdd = true
                } );
                CswNbtMetaDataObjectClassProp ExternalOrderNumberOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber,
                    FieldType = CswEnumNbtFieldType.Text,
                    SetValOnAdd = true
                } );
                CswNbtMetaDataObjectClassProp AssignedToOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.AssignedTo,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClassProp CommentsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Comments,
                    FieldType = CswEnumNbtFieldType.Comments
                } );
                CswNbtMetaDataObjectClassProp PriorityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Priority,
                    FieldType = CswEnumNbtFieldType.Number,
                    NumberMinValue = 0
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( PriorityOCP, 0, CswEnumNbtSubFieldName.Value );
                CswNbtMetaDataObjectClassProp FulfillmentHistoryOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.FulfillmentHistory,
                    FieldType = CswEnumNbtFieldType.Comments,
                    ServerManaged = true
                } );
                CswNbtMetaDataObjectClassProp FulfillOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Fulfill,
                    FieldType = CswEnumNbtFieldType.Button,
                    Extended = "menu",
                    StaticText = "Fulfill",
                    ListOptions = CswNbtObjClassRequestItem.FulfillMenu.Options.ToString()
                } );
                CswNbtMetaDataObjectClassProp LocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Location,
                    FieldType = CswEnumNbtFieldType.Location,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = LocationOC.ObjectClassId,
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp InventoryGroupOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.InventoryGroup,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = InventoryGroupOC.ObjectClassId,
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                #endregion Core OCPs
                #region Target Properties
                CswNbtMetaDataObjectClassProp EnterprisePartOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.EnterprisePart,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = EnterprisePartOC.ObjectClassId,
                    ServerManaged = true
                } );
                CswNbtMetaDataObjectClassProp MaterialOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Material,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.PropertySetId.ToString(),
                    FkValue = MaterialPS.PropertySetId,
                    ServerManaged = true
                } );
                CswNbtMetaDataObjectClassProp ContainerOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Container,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ContainerOC.ObjectClassId,
                    ServerManaged = true
                } );
                #endregion Target Properties
                #region Amount Properties
                CswNbtMetaDataObjectClassProp QuantityOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Quantity,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(),
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp TotalDispensedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.TotalDispensed,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(),
                    ServerManaged = true
                } );
                CswNbtMetaDataObjectClassProp SizeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.Size,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SizeOC.ObjectClassId,
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp SizeCountOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.SizeCount,
                    FieldType = CswEnumNbtFieldType.Number,
                    NumberMinValue = 1,
                    NumberPrecision = 0
                } );
                CswNbtMetaDataObjectClassProp TotalMovedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.TotalMoved,
                    FieldType = CswEnumNbtFieldType.Number
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TotalMovedOCP, 0, CswEnumNbtSubFieldName.Value );
                #endregion Amount Properties
                #region Create Material Properties
                CswNbtMetaDataObjectClassProp NewMaterialTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.NewMaterialType,
                    FieldType = CswEnumNbtFieldType.NodeTypeSelect,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.PropertySetId.ToString(),
                    FkValue = MaterialPS.PropertySetId,
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp NewMaterialTradenameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename,
                    FieldType = CswEnumNbtFieldType.Text,
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp NewMaterialSupplierOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier,
                    FieldType = CswEnumNbtFieldType.Text,
                    SetValOnAdd = true,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp NewMaterialPartNoOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo,
                    FieldType = CswEnumNbtFieldType.Text,
                    SetValOnAdd = true
                } );
                #endregion Create Material Properties
                #region Placeholder Properties
                CswNbtMetaDataObjectClassProp RequestIsFavoriteOCP = RequestOC.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsFavorite );
                CswNbtMetaDataObjectClassProp IsFavoriteOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.IsFavorite,
                    FieldType = CswEnumNbtFieldType.PropertyReference,
                    IsFk = true,
                    FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = RequestOCP.ObjectClassPropId,
                    ValuePropType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                    ValueFieldId = RequestIsFavoriteOCP.PropId
                } );
                CswNbtMetaDataObjectClassProp IsRecurringOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.IsRecurring,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true,
                    ServerManaged = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IsRecurringOCP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked );
                CswNbtMetaDataObjectClassProp RecurringFrequencyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.RecurringFrequency,
                    FieldType = CswEnumNbtFieldType.TimeInterval
                } );
                CswNbtMetaDataObjectClassProp NextReorderDateOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.NextReorderDate,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    ServerManaged = true
                } );
                #endregion Placeholder Properties
                #region MLM Properties
                CswNbtMetaDataObjectClassProp CertificationLevelOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.CertificationLevel,
                    FieldType = CswEnumNbtFieldType.List,
                    ListOptions = CswEnumNbtMaterialRequestApprovalLevel.All.ToString()
                } );
                CswNbtMetaDataObjectClassProp IsBatchOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.IsBatch,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IsBatchOCP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked );
                CswNbtMetaDataObjectClassProp BatchNumberOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.BatchNumber,
                    FieldType = CswEnumNbtFieldType.Text
                } );
                CswNbtMetaDataObjectClassProp GoodsReceivedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.GoodsReceived,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( GoodsReceivedOCP, CswEnumTristate.False, CswEnumNbtSubFieldName.Checked );
                CswNbtMetaDataObjectClassProp ReceiptLotToDispenseOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.ReceiptLotToDispense,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ReceiptLotOC.ObjectClassId
                } );
                CswNbtMetaDataObjectClassProp ReceiptLotsReceivedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived,
                    FieldType = CswEnumNbtFieldType.Grid
                } );
                _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( RequestItemOC.ObjectClassId, "Request Item", "Requests" );

                #endregion MLM Properties
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema