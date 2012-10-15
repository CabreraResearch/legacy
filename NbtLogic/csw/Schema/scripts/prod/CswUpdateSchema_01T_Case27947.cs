using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case27947
    /// </summary>
    public class CswUpdateSchema_01T_Case27947 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp statusOCP = containerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status );

            //set the list options for Status and maker it server managed
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( statusOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, CswNbtObjClassContainer.Statuses.Options.ToString() );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( statusOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            //Status is on the Container tab
            CswNbtMetaDataNodeType containerNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container" );
            if( null != containerNT )
            {
                CswNbtMetaDataNodeTypeProp statusNTP = containerNT.getNodeTypePropByObjectClassProp( statusOCP.PropId );
                CswNbtMetaDataNodeTypeTab containerNTT = containerNT.getFirstNodeTypeTab();
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                    NodeTypeId: containerNT.NodeTypeId,
                    PropId: statusNTP.PropId,
                    DoMove: true,
                    TabId: containerNTT.TabId );
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27947; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case27947

}//namespace ChemSW.Nbt.Schema