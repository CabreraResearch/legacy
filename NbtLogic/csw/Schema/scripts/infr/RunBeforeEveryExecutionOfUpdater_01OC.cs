using System;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo = 0;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        #region Ursula Methods

        public void _makeContainerGroup()
        {
            #region 27866 - Container Group
            _acceptBlame( CswDeveloper.MB, 27866 );

            CswNbtMetaDataObjectClass containerGoupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass );
            if( null == containerGoupOC )
            {
                containerGoupOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.ContainerGroupClass, "barcode.png", false );

                CswNbtMetaDataObjectClassProp containerGroupNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );

                //this barcode prop has to start with a "G" - sequence it set on the NTP and thus in a schemascript
                CswNbtMetaDataObjectClassProp containerGroupBarcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.Barcode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Barcode
                } );

                CswNbtMetaDataObjectClassProp containerGroupSyncLocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.SyncLocation,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( containerGroupSyncLocationOCP, false );

                CswNbtMetaDataObjectClassProp containerGroupLocationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerGoupOC )
                {
                    PropName = CswNbtObjClassContainerGroup.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location
                } );

                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, containerGoupOC.ObjectClassId );

            }
            _resetBlame();
            #endregion
        }

        public void _newContainerProperties27866()
        {
            #region Case 27866 part 2 - new container properties
            _acceptBlame( CswDeveloper.MB, 27866 );

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

            CswNbtMetaDataObjectClass receiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
            CswNbtMetaDataObjectClassProp receiptLotOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ReceiptLot,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = receiptLotOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                ServerManaged = true
            } );

            CswNbtMetaDataObjectClassProp lotControlledOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.LotControlled,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( lotControlledOCP, false );

            CswNbtMetaDataObjectClassProp requisitionableOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.Requisitionable,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( requisitionableOCP, false );

            CswNbtMetaDataObjectClass containerGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass );
            CswNbtMetaDataObjectClassProp containerGroupOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ContainerGroup,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = containerGroupOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString()
            } );

            CswNbtMetaDataObjectClass printLabelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp labelFormatOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.LabelFormat,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = printLabelOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString()
            } );

            CswNbtMetaDataObjectClass userOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp reservedForOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( containerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ReservedFor,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkValue = userOC.ObjectClassId,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString()
            } );

            _resetBlame();
            #endregion
        }

        private void _destroyRequestItemOc( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( "RequestItemClass" );
            if( null != RequestItemOc )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClass( RequestItemOc );
            }
            _resetBlame();
        }

        private void _addRequestFavorite( CswDeveloper Dev, Int32 CaseNo )
        {
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClassProp IsFavoriteOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestOc )
            {
                PropName = CswNbtObjClassRequest.PropertyName.IsFavorite,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                SetValOnAdd = false,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IsFavoriteOcp, false );

            CswNbtMetaDataObjectClassProp RequestorOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestorOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestorOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_col_add, Int32.MinValue );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestorOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_row_add, Int32.MinValue );

            CswNbtMetaDataObjectClassProp NameOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_col_add, 1 );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.display_row_add, 1 );

        }

        private CswNbtMetaDataObjectClass _createRequestItemBase( NbtObjectClass ObjectClass, Int32 StartAddRowAt = 1 )
        {
            CswNbtMetaDataObjectClass Ret = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjectClass );
            if( null == Ret )
            {
                CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClass InventoryGroupOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupClass );
                CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                Ret = _CswNbtSchemaModTrnsctn.createObjectClass( ObjectClass, NbtIcon.cart, AuditLevel: true );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.RequestedFor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.InventoryGroup,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = InventoryGroupOc.ObjectClassId,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.NeededBy,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = StartAddRowAt
                } );
                StartAddRowAt += 1;

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Name,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Description,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Static
                } );

                CswNbtMetaDataObjectClassProp StatusOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Status,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( StatusOcp, CswNbtPropertySetRequestItem.Statuses.Pending );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Type,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Requestor,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp PriorityOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Priority,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                    NumberPrecision = 1,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( PriorityOcp, 0 );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Number,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    ServerManaged = true,
                    SetValOnAdd = false,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Request,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RequestOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.ExternalOrderNumber,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = false
                } );



                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.AssignedTo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Comments,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Comments,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( Ret )
                {
                    PropName = CswNbtPropertySetRequestItem.PropertyName.Fulfill,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                    Extended = CswNbtNodePropButton.ButtonMode.menu,
                    StaticText = CswNbtPropertySetRequestItem.FulfillMenu.Complete,
                    SetValOnAdd = false
                } );

            }
            return Ret;
        }

        private void _createRequestContainerDispense( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestContainerDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerDispenseClass );
            if( null == RequestContainerDispenseOc )
            {
                CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

                CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );

                RequestContainerDispenseOc = _createRequestItemBase( NbtObjectClass.RequestContainerDispenseClass );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Container,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ContainerOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestContainerDispense.FulfillMenu.Dispense );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.FulfillMenu.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Quantity,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    IsRequired = true,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 5
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.Size,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SizeOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.Statuses.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerDispenseOc )
                {
                    PropName = CswNbtObjClassRequestContainerDispense.PropertyName.TotalDispensed,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestContainerDispenseOc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.Types.ContainerDispense );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtObjClassRequestContainerDispense.Types.ContainerDispense );
            }
            _resetBlame();
        }

        private void _createRequestContainerUpdate( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestContainerUpdateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestContainerUpdateClass );
            if( null == RequestContainerUpdateOc )
            {
                CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );

                RequestContainerUpdateOc = _createRequestItemBase( NbtObjectClass.RequestContainerUpdateClass );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestContainerUpdateOc )
                {
                    PropName = CswNbtObjClassRequestContainerUpdate.PropertyName.Container,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ContainerOc.ObjectClassId,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestContainerUpdate.FulfillMenu.Dispose );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerUpdate.FulfillMenu.DisposeOptions.ToString() );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerUpdate.Types.Options.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtObjClassRequestContainerUpdate.Types.Dispose );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestContainerUpdateOc.getObjectClassProp( CswNbtObjClassRequestContainerUpdate.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerUpdate.Statuses.Options.ToString() );
            }
            _resetBlame();
        }

        private void _createRequestMaterialDispense( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestMaterialDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialDispenseClass );
            if( null == RequestMaterialDispenseOc )
            {
                CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );

                RequestMaterialDispenseOc = _createRequestItemBase( NbtObjectClass.RequestMaterialDispenseClass );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestMaterialDispense.FulfillMenu.Order );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialDispense.FulfillMenu.Options.ToString() );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialDispense.Types.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Quantity,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    IsRequired = true,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 5
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Count,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                    SetValOnAdd = true,
                    IsRequired = true,
                    NumberPrecision = 0,
                    NumberMinValue = 1,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 6
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Size,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SizeOc.ObjectClassId,
                    IsRequired = true,
                    SetValOnAdd = true,
                    DisplayRowAdd = 7,
                    DisplayColAdd = 1
                } );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestContainerDispense.Statuses.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.TotalDispensed,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp TotalMovedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.TotalMoved,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                    ServerManaged = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TotalMovedOcp, 0 );

                CswNbtMetaDataObjectClassProp ReorderOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Reorder,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    SetValOnAdd = false,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ReorderOcp, false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.ReorderFrequency,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.TimeInterval,
                    SetValOnAdd = false,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RequestMaterialDispenseOc.ObjectClassId,
                    FilterPropId = ReorderOcp.PropId,
                    Filter = CswNbtMetaDataObjectClassProp.makeFilter( ReorderOcp, CswNbtPropFilterSql.PropertyFilterMode.Equals, true )
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.NextReorderDate,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                    SetValOnAdd = false,
                    ServerManaged = true,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = RequestMaterialDispenseOc.ObjectClassId,
                    FilterPropId = ReorderOcp.PropId,
                    Filter = CswNbtMetaDataObjectClassProp.makeFilter( ReorderOcp, CswNbtPropFilterSql.PropertyFilterMode.Equals, true )
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Level,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp IsBatchOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.IsBatch,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IsBatchOcp, false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.Batch,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    SetValOnAdd = false
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                 {
                     PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.ReceiptLotsReceived,
                     FieldType = CswNbtMetaDataFieldType.NbtFieldType.Grid,
                     SetValOnAdd = false,
                     Extended = CswNbtNodePropGrid.GridPropMode.Link.ToString(),

                 } );

                CswNbtMetaDataObjectClassProp GoodsReceivedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.GoodsReceived,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true,
                    SetValOnAdd = false
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( GoodsReceivedOcp, false );

                CswNbtMetaDataObjectClass ReceiptLotOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReceiptLotClass );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.ReceiptLotToDispense,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = ReceiptLotOc.ObjectClassId,
                    SetValOnAdd = false
                } );

                CswNbtMetaDataObjectClassProp ReceiptLotRequestOcp = ReceiptLotOc.getObjectClassProp( CswNbtObjClassReceiptLot.PropertyName.RequestItem );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReceiptLotRequestOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ReceiptLotRequestOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, RequestMaterialDispenseOc.ObjectClassId );
                //We should fix ContainerDispenseTransaction too, but PropertySets aren't in the database. So we have to fix the relationship view on the NodeTypeProp.

                CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestClass );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialDispenseOc )
                {
                    PropName = CswNbtObjClassRequestMaterialDispense.PropertyName.IsFavorite,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.PropertyReference,
                    FkType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    FkValue = RequestMaterialDispenseOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.Request ).PropId,
                    ValuePropId = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.IsFavorite ).PropId,
                    ValuePropType = NbtViewPropIdType.ObjectClassPropId.ToString(),
                    SetValOnAdd = false
                } );
            }
            _resetBlame();
        }

        private void _createRequestMaterialCreate( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass RequestMaterialCreateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.RequestMaterialCreateClass );
            if( null == RequestMaterialCreateOc )
            {
                CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                CswNbtMetaDataObjectClass SupplierOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );

                RequestMaterialCreateOc = _createRequestItemBase( NbtObjectClass.RequestMaterialCreateClass, 5 );

                CswNbtMetaDataObjectClassProp FulfillOcp = RequestMaterialCreateOc.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Fulfill );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.statictext, CswNbtObjClassRequestMaterialCreate.FulfillMenu.Create );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FulfillOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialCreate.FulfillMenu.Options.ToString() );

                CswNbtMetaDataObjectClassProp TypeOcp = RequestMaterialCreateOc.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Type );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialCreate.Types.Options.ToString() );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TypeOcp, CswNbtObjClassRequestMaterialCreate.Types.Create );

                CswNbtMetaDataObjectClassProp StatusOcp = RequestMaterialCreateOc.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.Status );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StatusOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestMaterialCreate.Statuses.Options.ToString() );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.Material,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId,
                    SetValOnAdd = false,
                    ServerManaged = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialType,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = MaterialOc.ObjectClassId,
                    Multi = false,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 1,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialTradename,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 2,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialSupplier,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = SupplierOc.ObjectClassId,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 3,
                    IsRequired = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RequestMaterialCreateOc )
                {
                    PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.NewMaterialPartNo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    SetValOnAdd = true,
                    DisplayColAdd = 1,
                    DisplayRowAdd = 4
                } );
            }
            _resetBlame();
        }
        public void _addGeneratorTargetCreatedDate()
        {
            _acceptBlame( CswDeveloper.SS, 28069 );

            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.TaskClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TaskOC )
            {
                PropName = CswNbtPropertySetGeneratorTarget.PropertyName.CreatedDate,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                ServerManaged = true
            } );

            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( InspectionDesignOC )
            {
                PropName = CswNbtPropertySetGeneratorTarget.PropertyName.CreatedDate,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                ServerManaged = true
            } );

            _resetBlame();
        }


        private void _createaNewMaterialComponentProp( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );

            CswNbtMetaDataObjectClassProp activeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialComponentOC )
            {
                PropName = CswNbtObjClassMaterialComponent.PropertyName.Active,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( activeOCP, false );

            _resetBlame();
        }

        private void _createUNCodeNodeType( CswDeveloper Dev, Int32 CaseNo )
        {
            CswNbtMetaDataObjectClass GenericOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GenericClass );
            if( null != GenericOc )
            {
                //LQNo NodeType
                CswNbtMetaDataNodeType LQNoNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "LQNo" );
                if( null == LQNoNt )
                {
                    LQNoNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "LQNo", "MLM" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, LQNoNt.NodeTypeId );
                    CswNbtMetaDataNodeTypeProp LQNoLQNoNtp = _createNewProp( LQNoNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Text );
                    LQNoLQNoNtp.setIsUnique( true );
                    CswNbtMetaDataNodeTypeProp LQNoLimitNtp = _createNewProp( LQNoNt, "Limit", CswNbtMetaDataFieldType.NbtFieldType.Quantity );
                    LQNoLimitNtp.IsRequired = true;
                    CswNbtMetaDataNodeType WeightNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Weight)" );
                    if( null != WeightNt )
                    {
                        LQNoLimitNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), WeightNt.NodeTypeId );
                    }
                    LQNoNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "LQNo" ) );

                    //UNCode NodeType
                    CswNbtMetaDataNodeType UNCodeNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "UN Code" );
                    if( null == UNCodeNt )
                    {
                        UNCodeNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "UN Code", "MLM" );
                        _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, UNCodeNt.NodeTypeId );
                        CswNbtMetaDataNodeTypeProp UNCodeUNCodeNtp = _createNewProp( UNCodeNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Text );
                        UNCodeUNCodeNtp.setIsUnique( true );
                        CswNbtMetaDataNodeTypeProp UNCodeLQNoNtp = _createNewProp( UNCodeNt, "LQNo", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                        UNCodeLQNoNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), LQNoNt.NodeTypeId );
                        UNCodeNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "UN Code" ) );

                        //Create Demo Data
                        if( null != WeightNt )
                        {
                            CswPrimaryKey kgNodeId = null;
                            foreach( CswNbtObjClassUnitOfMeasure WeightNode in WeightNt.getNodes( false, false ) )
                            {
                                if( "kg" == WeightNode.Name.Text )
                                {
                                    kgNodeId = WeightNode.NodeId;
                                }
                            }
                            CswNbtNode LQNoNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( LQNoNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                            LQNoNode.Properties[LQNoLQNoNtp].AsText.Text = "1 Metric Ton";
                            LQNoNode.Properties[LQNoLimitNtp].AsQuantity.Quantity = 1000;
                            LQNoNode.Properties[LQNoLimitNtp].AsQuantity.UnitId = kgNodeId;
                            LQNoNode.IsDemo = true;
                            LQNoNode.postChanges( false );

                            CswNbtNode UNCodeNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UNCodeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                            UNCodeNode.Properties[UNCodeLQNoNtp].AsRelationship.RelatedNodeId = LQNoNode.NodeId;
                            UNCodeNode.Properties[UNCodeUNCodeNtp].AsText.Text = "US ITH";
                            UNCodeNode.IsDemo = true;
                            UNCodeNode.postChanges( false );
                        }

                        //Create demo Views
                        CswNbtView UNCodeView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes", NbtViewVisibility.Global );
                        UNCodeView.Category = "MLM (demo)";
                        UNCodeView.IsDemo = true;
                        UNCodeView.ViewMode = NbtViewRenderingMode.Tree;
                        UNCodeView.AddViewRelationship( UNCodeNt, true );
                        UNCodeView.save();

                        CswNbtView LQNoView = _CswNbtSchemaModTrnsctn.makeNewView( "UN Codes by LQNo", NbtViewVisibility.Global );
                        LQNoView.Category = "MLM (demo)";
                        LQNoView.IsDemo = true;
                        LQNoView.ViewMode = NbtViewRenderingMode.Tree;
                        CswNbtViewRelationship LQNoRelationship = LQNoView.AddViewRelationship( LQNoNt, true );
                        LQNoView.AddViewRelationship( LQNoRelationship, NbtViewPropOwnerType.Second, UNCodeLQNoNtp, false );
                        LQNoView.save();

                        //Update Chemical to include UN Code
                        CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
                        if( null != ChemicalNt )
                        {
                            CswNbtMetaDataNodeTypeProp ChemUNCodeNtp = _createNewProp( ChemicalNt, "UN Code", CswNbtMetaDataFieldType.NbtFieldType.Relationship, false );
                            ChemUNCodeNtp.SetFK( NbtViewRelatedIdType.NodeTypeId.ToString(), UNCodeNt.NodeTypeId );
                        }
                    }
                }
            }
        }

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswNbtMetaDataFieldType.NbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop.PropId,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop.PropId,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        private void _createNewMaterialProps( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            CswNbtMetaDataObjectClassProp materialIdOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.MaterialId,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Sequence,
                ServerManaged = true,
                IsUnique = true
            } );

            CswNbtMetaDataObjectClassProp approvedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.Approved,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( approvedOCP, false );

            CswNbtMetaDataObjectClass vendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.VendorClass );
            CswNbtMetaDataObjectClassProp vendorNameOCP = vendorOC.getObjectClassProp( CswNbtObjClassVendor.PropertyName.VendorName );
            CswNbtView supplierView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship supplierParent = supplierView.AddViewRelationship( vendorOC, true );
            supplierView.AddViewPropertyAndFilter( supplierParent,
                MetaDataProp: vendorNameOCP,
                Value: "Corporate",
                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );

            CswNbtMetaDataObjectClassProp supplierOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( supplierOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, supplierView.ToXml().ToString() );

            CswNbtMetaDataNodeType unCodeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "UN Code" );
            if( null != unCodeNT )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
                {
                    PropName = CswNbtObjClassMaterial.PropertyName.UNCode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.NodeTypeId.ToString(),
                    FkValue = unCodeNT.NodeTypeId
                } );
            }

            CswNbtMetaDataObjectClass mepOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ManufacturerEquivalentPartClass );
            CswNbtMetaDataObjectClassProp manufacturerOCP = mepOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Manufacturer );
            CswNbtMetaDataObjectClassProp materialOCP = mepOC.getObjectClassProp( CswNbtObjClassManufacturerEquivalentPart.PropertyName.Material );

            CswNbtView manufacturingSitesView = _CswNbtSchemaModTrnsctn.makeNewView( CswNbtObjClassMaterial.PropertyName.ManufacturingSites, NbtViewVisibility.Property );
            CswNbtViewRelationship parent = manufacturingSitesView.AddViewRelationship( materialOC, true );
            CswNbtViewRelationship parent2 = manufacturingSitesView.AddViewRelationship( parent, NbtViewPropOwnerType.Second, materialOCP, false );
            manufacturingSitesView.AddViewProperty( parent2, manufacturerOCP );
            manufacturingSitesView.SetViewMode( NbtViewRenderingMode.Grid );
            manufacturingSitesView.save();

            CswNbtMetaDataObjectClassProp manufacturingSitesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.ManufacturingSites,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Grid
            } );

            _resetBlame();
        }

        private void _makeContainerLocationOc()
        {
            #region Case 24489 - ContainerLocation ObjectClass
            _acceptBlame( CswDeveloper.BV, 24489 );

            CswNbtMetaDataObjectClass ContainerLocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerLocationClass );
            if( null == ContainerLocationOc )
            {
                //Create new ObjectClass
                ContainerLocationOc = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.ContainerLocationClass, "barcode.png", true );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, ContainerLocationOc.ObjectClassId );
            }
            //Create ObjectClassProps
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.Container,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = ContainerOc.ObjectClassId,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.Location,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.Type,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = String.Join( ",", CswNbtObjClassContainerLocation.TypeOptions._All ),
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.ContainerScan,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.LocationScan,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.ScanDate,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.Status,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = String.Join( ",", CswNbtObjClassContainerLocation.StatusOptions._All ),
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.Action,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = String.Join( ",", CswNbtObjClassContainerLocation.ActionOptions._All ),
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.ActionApplied,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                ServerManaged = true
            } );
            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerLocationOc )
            {
                PropName = CswNbtObjClassContainerLocation.PropertyName.User,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = UserOc.ObjectClassId,
                ServerManaged = true
            } );
            //Container DateCreated
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOc )
            {
                PropName = CswNbtObjClassContainer.PropertyName.DateCreated,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                ServerManaged = true
            } );

            _resetBlame();
            #endregion Case 24489 - ContainerLocation ObjectClass
        }

        private void _replaceMaterialCASNoProp( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            CswNbtMetaDataFieldType CASNoFT = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.CASNo );
            if( null == CASNoFT )
            {
                _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswNbtMetaDataFieldType.NbtFieldType.CASNo, CswNbtMetaDataFieldType.DataType.TEXT );
            }

            //drop the existing CASNo prop (a text ft) and replace it with the new CASNo prop
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp oldCASNoOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.CasNo );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( oldCASNoOCP, true );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( materialOC )
            {
                PropName = CswNbtObjClassMaterial.PropertyName.CasNo,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.CASNo,
                SetValOnAdd = false
            } );

            _resetBlame();
        }

        private void _makePendingFeedbackCountProp( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            CswNbtMetaDataObjectClass customerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CustomerClass );
            CswNbtMetaDataObjectClassProp pendingFeedbackCount = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( customerOC )
            {
                PropName = CswNbtObjClassCustomer.PropertyName.PendingFeedbackCount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( pendingFeedbackCount, 0 );

            _resetBlame();
        }

        #endregion Ursula Methods

        #region Viola Methods

        #region Case 28283

        private void _addFireClassExemptAmountProps( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            #region FireClassExemptAmountSet

            CswNbtMetaDataObjectClass FireClassExemptAmountSetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountSetClass );
            if( null == FireClassExemptAmountSetOC )
            {
                FireClassExemptAmountSetOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.FireClassExemptAmountSetClass, "explode.png", false );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, FireClassExemptAmountSetOC.ObjectClassId );
            }
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountSetOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmountSet.PropertyName.SetName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                IsUnique = true
            } );

            #endregion FireClassExemptAmountSet

            #region FireClassExemptAmount

            CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
            if( null == FireClassExemptAmountOC )
            {
                FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.FireClassExemptAmountClass, "explode.png", false );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, FireClassExemptAmountOC.ObjectClassId );
            }
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.SetName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = FireClassExemptAmountSetOC.ObjectClassId,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 1
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.SortOrder,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 2
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                SetValOnAdd = true,
                IsRequired = true,
                ListOptions = String.Join(",", CswNbtObjClassFireClassExemptAmount.FireHazardClassTypes._All ),
                DisplayColAdd = 1,
                DisplayRowAdd = 3
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.HazardType,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                SetValOnAdd = true,
                ListOptions = "Physical,Health",
                DisplayColAdd = 1,
                DisplayRowAdd = 4
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.Material,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 5
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );

            #endregion FireClassExemptAmount

            _resetBlame();
        }

        #endregion Case 28283

        #region Case 28281
        private void _addHazardousReoprtingProp( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp HazardousReportingOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialComponentOC )
            {
                PropName = CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( HazardousReportingOCP, false );

            _resetBlame();
        }
        
        private void _addContainerFireReportingProps( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.StoragePressure,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "1 = Atmospheric,2 = Pressurized,3 = Subatmospheric",
                SetValOnAdd = false
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.StorageTemperature,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "4 = Room Temperature,5 = Greater than Room Temperature,6 = Less than Room Temperature,7 = Cryogenic",
                SetValOnAdd = false
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.UseType,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "Storage,Use Closed,Use Open",
                SetValOnAdd = false
            } );

            _resetBlame();
        }
        #endregion Case 28281

        #region Case 28282

        private void _addControlZoneNT( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass GenericOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GenericClass );
            if( null != GenericOc )
            {
                //ControlZone NodeType
                CswNbtMetaDataNodeType ControlZoneNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
                if( null == ControlZoneNt )
                {
                    ControlZoneNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "Control Zone", "Materials" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, ControlZoneNt.NodeTypeId );                    

                    CswNbtMetaDataNodeTypeProp NameNTP = _createNewProp( ControlZoneNt, "Name", CswNbtMetaDataFieldType.NbtFieldType.Text );
                    NameNTP.IsRequired = true;
                    CswNbtMetaDataNodeTypeProp MAQOffsetNTP = _createNewProp( ControlZoneNt, "MAQ Offset %", CswNbtMetaDataFieldType.NbtFieldType.Number, false );
                    MAQOffsetNTP.DefaultValue.AsNumber.Value = 100;
                    CswNbtMetaDataObjectClass FCEASOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountSetClass );
                    CswNbtMetaDataNodeTypeProp FCEASNameNTP = _createNewProp( ControlZoneNt, "Fire Class Set Name", CswNbtMetaDataFieldType.NbtFieldType.Relationship );
                    FCEASNameNTP.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), FCEASOC.ObjectClassId );

                    ControlZoneNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Name" ) );

                    //Update Location to include Control Zone
                    CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( LocationOC )
                    {
                        PropName = CswNbtObjClassLocation.PropertyName.ControlZone,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                        IsFk = true,
                        FkType = NbtViewRelatedIdType.NodeTypeId.ToString(),
                        FkValue = ControlZoneNt.NodeTypeId
                    } );
                }
            }

            _resetBlame();
        }

        #endregion Case 28282

        #endregion Viola Methods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region URSULA

            _destroyRequestItemOc( CswDeveloper.CF, 27942 );

            _addRequestFavorite( CswDeveloper.CF, 27695 ); //This needs to happen before we create the Items
            _createRequestContainerDispense( CswDeveloper.CF, 27942 );
            _createRequestContainerUpdate( CswDeveloper.CF, 27942 );
            _createRequestMaterialDispense( CswDeveloper.CF, 27942 );
            _createRequestMaterialCreate( CswDeveloper.CF, 27871 );

            _makeContainerGroup();
            _newContainerProperties27866();
            _makeContainerLocationOc();
            _createaNewMaterialComponentProp( CswDeveloper.MB, 27864 );
            _createUNCodeNodeType( CswDeveloper.MB, 27872 );
            _createNewMaterialProps( CswDeveloper.MB, 27864 );
            _replaceMaterialCASNoProp( CswDeveloper.MB, 27876 );

            _addGeneratorTargetCreatedDate();

            _makePendingFeedbackCountProp( CswDeveloper.MB, 28079 );

            #endregion URSULA

            #region VIOLA

            _addFireClassExemptAmountProps( CswDeveloper.BV, 28283 );
            _addHazardousReoprtingProp( CswDeveloper.BV, 28281 );
            _addContainerFireReportingProps( CswDeveloper.BV, 28281 );
            _addControlZoneNT( CswDeveloper.BV, 28282 );

            #endregion VIOLA


            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01OC

}//namespace ChemSW.Nbt.Schema


