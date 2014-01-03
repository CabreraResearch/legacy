using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case30665: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30665; }
        }

        public override string Title
        {
            get { return "Create Receiving Langing Page"; }
        }

        public override void update()
        {
            CswNbtObjClassRole AdminRole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );

            CswNbtViewId MyBatchOperationsViewId = null;
            CswNbtView MyBatchOperationsView = null;
            DataTable Views = _CswNbtSchemaModTrnsctn.ViewSelect.getView( "My Batch Operations", CswEnumNbtViewVisibility.Global, null, null );
            if( Views.Rows.Count > 0 )
            {
                MyBatchOperationsViewId = new CswNbtViewId( CswConvert.ToInt32( Views.Rows[0]["nodeviewid"] ) );
                MyBatchOperationsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( MyBatchOperationsViewId );
            }

            if( null == MyBatchOperationsView )
            {
                throw new CswDniException( CswEnumErrorType.Error, "My Batch Operations View does not exist", "The My Batch Operations View is needed to configure a landing page for the Receiving Wizard" );
            }
            if( null == AdminRole )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Administrator Role does not exist", "The Administrator Role is needed to configure a landing page for the Receiving Wizard" );
            }

            _CswNbtSchemaModTrnsctn.LandingPage.addLandingPageItem( CswEnumNbtLandingPageItemType.Link, new LandingPageData.Request()
                {
                    ActionId = _CswNbtSchemaModTrnsctn.Actions[CswEnumNbtActionName.Receiving].ActionId.ToString(),
                    NodeViewId = MyBatchOperationsViewId.ToString(),   //I think we have to set PkValue and not NodeViewId...but idk why
                    PkValue = MyBatchOperationsViewId.ToString(),
                    ViewType = CswEnumNbtViewType.View.ToString(),
                    RoleId = AdminRole.NodeId.ToString(),
                    Text = "My Batch Operations",
                    NewColumn = 1,
                    NewRow = 1
                } );


            CswNbtMetaDataObjectClass ChemicalClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp ReceiveOCP = ChemicalClass.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Receive );
            _CswNbtSchemaModTrnsctn.LandingPage.addLandingPageItem( CswEnumNbtLandingPageItemType.Button, new LandingPageData.Request()
                {
                    ActionId = _CswNbtSchemaModTrnsctn.Actions[CswEnumNbtActionName.Receiving].ActionId.ToString(),
                    PkValue = ReceiveOCP.PropId.ToString(),
                    RoleId = AdminRole.NodeId.ToString(),
                    Text = "Receive more of this Material",
                    NewColumn = 2,
                    NewRow = 1
                } );


        } // update()

    }

}//namespace ChemSW.Nbt.Schema