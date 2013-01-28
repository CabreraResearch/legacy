using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28560
    /// </summary>
    public class CswUpdateSchema_01W_Case28560 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28560; }
        }

        public override void update()
        {

            //set the menu opts
            CswNbtMetaDataObjectClass containerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( CswNbtObjClassContainer containerNode in containerOC.getNodes( false, false, false, true ) )
            {
                containerNode.ViewSDS.State = CswNbtObjClassContainer.PropertyName.ViewSDS;
                containerNode.ViewSDS.MenuOptions = CswNbtObjClassContainer.PropertyName.ViewSDS + ",View Other";
                containerNode.postChanges( false );
            }

            foreach( CswNbtMetaDataNodeType containerNT in containerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp viewSDS_NTP = containerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.ViewSDS );
                CswNbtMetaDataNodeTypeProp barcodeNTP = containerNT.getBarcodeProperty();

                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, viewSDS_NTP, barcodeNTP, true );
            }

        } //Update()

    }//class CswUpdateSchema_01W_Case28560

}//namespace ChemSW.Nbt.Schema