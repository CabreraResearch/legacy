using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28363
    /// </summary>
    public class CswUpdateSchema_01W_Case28363 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28363; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            //update the View SDS menu opts with the correct values
            foreach( CswNbtObjClassMaterial materialNode in materialOC.getNodes( false, false, false, true ) )
            {
                materialNode.ViewSDS.State = CswNbtObjClassMaterial.PropertyName.ViewSDS;
                materialNode.UpdateViewSDSButtonOpts();
                materialNode.postChanges( false );
            }

            //Move the View SDS button to the Identity tab for Chemicals and hide it for everything else
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp viewSDS_NTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.ViewSDS );
                if( materialNT.NodeTypeName.Equals( "Chemical" ) )
                {
                    CswNbtMetaDataNodeTypeProp receiveNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Receive );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, viewSDS_NTP, receiveNTP, true );
                }
                else
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( viewSDS_NTP );
                }
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28363

}//namespace ChemSW.Nbt.Schema