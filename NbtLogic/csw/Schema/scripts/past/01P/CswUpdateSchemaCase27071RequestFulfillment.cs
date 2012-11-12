using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27071RequestFulfillment
    /// </summary>
    public class CswUpdateSchemaCase27071RequestFulfillment : CswUpdateSchemaTo
    {
        public override void update()
        {
            //Part 4
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

            //Bug fix
            CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
            CswNbtMetaDataFieldType ListFt = _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.List );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fieldtypeid, ListFt.FieldTypeId );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassRequestItem.Types.Options.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, true );

            //Part 5
            CswNbtMetaDataObjectClassProp StatusOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status );
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

            //Part 6
            _CswNbtSchemaModTrnsctn.createObjectClassProp( RequestItemOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassRequestItem.PropertyName.Fulfill,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                Extended = CswNbtNodePropButton.ButtonMode.menu,
                ListOptions = CswNbtObjClassRequestItem.FulfillMenu.Options.ToString(),
                StaticText = CswNbtObjClassRequestItem.FulfillMenu.Dispense
            } );

        }//Update()

    }//class CswUpdateSchemaCase27071RequestFulfillment

}//namespace ChemSW.Nbt.Schema