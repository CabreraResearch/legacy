using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27071RequestFulfillment
    /// </summary>
    public class CswUpdateSchemaCase27071RequestFulfillment : CswUpdateSchemaTo
    {
        public override void update()
        {
            //_CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Fulfill_Request, true, "", "Materials" );
            //_CswNbtSchemaModTrnsctn.createModuleActionJunction(CswNbtResources.CswNbtModule.CISPro, CswNbtActionName.Fulfill_Request);

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass RequestItemOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
            CswNbtMetaDataObjectClassProp AssgnedToOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtObjClassRequestItem.PropertyName.AssignedTo,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                    IsFk = true,
                    FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = UserOc.ObjectClassId
                } );

            CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
            CswNbtMetaDataObjectClassProp StatusOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status );
            CswNbtMetaDataFieldType ListFt = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, ListFt.FieldTypeId );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestItem.Types.Options.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            CswNbtView DispenseRequestsView = _CswNbtSchemaModTrnsctn.makeNewView( "Dispense Requests: Open", NbtViewVisibility.Global );
            DispenseRequestsView.ViewMode = NbtViewRenderingMode.Tree;
            DispenseRequestsView.Category = "Requests";
            CswNbtViewRelationship RequestItemsVr = DispenseRequestsView.AddViewRelationship( RequestItemOc, true );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, AssgnedToOcp, "me" );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, TypeOcp, CswNbtObjClassRequestItem.Types.Dispense );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestItem.Statuses.Completed, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestItem.Statuses.Cancelled, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.AddViewPropertyAndFilter( RequestItemsVr, StatusOcp, CswNbtObjClassRequestItem.Statuses.Dispensed, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            DispenseRequestsView.save();
        }//Update()

    }//class CswUpdateSchemaCase27071RequestFulfillment

}//namespace ChemSW.Nbt.Schema