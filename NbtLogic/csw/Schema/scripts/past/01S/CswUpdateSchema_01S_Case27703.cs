using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27703
    /// </summary>
    public class CswUpdateSchema_01S_Case27703 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass requestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtDoomedObjectClasses.RequestItemClass );
            CswNbtMetaDataNodeType requestItemNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Request Item" );

            #region PART 1 - BUTTON NAMING AND LIST OPTIONS

            //change materials Request button to a menu button
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp requestOCP = materialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Request );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( requestOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.extended, CswNbtNodePropButton.ButtonMode.menu );

            //change Material "request" prop on all material nodes to have the request by list options
            string opts = CswNbtObjClassRequestItem.RequestsBy.Options.ToString();
            foreach( CswNbtObjClassMaterial materialNode in materialOC.getNodes( false, false ) )
            {
                materialNode.Request.MenuOptions = opts;
                materialNode.Request.State = CswNbtObjClassRequestItem.RequestsBy.Size;
                materialNode.postChanges( false );
            }

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            string fulFillOpts = CswNbtObjClassRequestItem.FulfillMenu.Options.ToString();
            foreach( CswNbtObjClassContainer containerNode in containerOC.getNodes( false, false ) )
            {
                containerNode.updateRequestMenu();
                containerNode.postChanges( false );
            }

            //change Request Items "Request By" prop to use the newly named list options
            CswNbtMetaDataObjectClassProp requestByOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RequestBy );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( requestByOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, opts );

            //make Request Items "request by" server managed
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( requestByOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            //remove the filters on "Count," "Size," and "Quantity" - these will now show/hide based on Materials "Request" button click
            CswNbtMetaDataObjectClassProp countOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Count );
            CswNbtMetaDataObjectClassProp sizeOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( countOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filter, "" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( countOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filterpropid, "" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( sizeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filter, "" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( sizeOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filterpropid, "" );

            CswNbtMetaDataObjectClassProp quantityOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Quantity );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( quantityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filter, "" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( quantityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filterpropid, "" );

            #endregion

            #region PART 2 - ADD NAME PROPERTY TO REQUEST ITEM

            // moved to RunBeforeEveryExecutionOfUpdater_01OC
            //CswNbtMetaDataObjectClassProp nameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( requestItemOC )
            //{
            //    PropName = "Name",
            //    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
            //    ServerManaged = true
            //} );
            
            CswNbtMetaDataObjectClassProp nameOCP = requestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Name );

            string newNameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassRequestItem.PropertyName.Name );
            if( null != requestItemNT )
            {
                requestItemNT.setNameTemplateText( newNameTemplate );
                CswNbtMetaDataNodeTypeProp nameNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( requestItemNT.NodeTypeId, nameOCP.ObjectClassPropId );
                nameNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

            #endregion

        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27703; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema