using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27866
    /// </summary>
    public class CswUpdateSchema_01U_Case27866 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27866; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass containerGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerGroupClass );

            CswNbtMetaDataNodeType containerGroupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Container Group" );
            if( null == containerGroupNT )
            {
                containerGroupNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( containerGroupOC.ObjectClassId, "Container Group", "MLM (demo)" );
            }

            CswNbtMetaDataNodeTypeProp barcodeNTP = containerGroupNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerGroup.PropertyName.Barcode );
            if( null != barcodeNTP )
            {
                //set the sequence id of the NTP
                if( Int32.MinValue == barcodeNTP.SequenceId )
                {
                    int containerGroupSequenceId = _CswNbtSchemaModTrnsctn.makeSequence( new CswSequenceName( "ContainerGroup" ), "G", "", 6, 0 );
                    barcodeNTP.setSequence( containerGroupSequenceId );
                }
            }

            CswNbtMetaDataNodeTypeProp nameNTP = containerGroupNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainerGroup.PropertyName.Name );
            nameNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, null, false );

            string template = CswNbtMetaData.MakeTemplateEntry( nameNTP.PropName );
            template += " " + CswNbtMetaData.MakeTemplateEntry( barcodeNTP.PropName );
            containerGroupNT.setNameTemplateText( template );

            //create demo view
            CswNbtView containerGroupView = _CswNbtSchemaModTrnsctn.restoreView( "Container Groups" );
            if( null == containerGroupView )
            {
                containerGroupView = _CswNbtSchemaModTrnsctn.makeNewView( "Container Groups", NbtViewVisibility.Global );
                containerGroupView.Category = "MLM (demo)";
                containerGroupView.SetViewMode( NbtViewRenderingMode.Tree );
                containerGroupView.IsDemo = true;
                containerGroupView.AddViewRelationship( containerGroupNT, true );
                containerGroupView.save();
            }

        }

        //Update()

    }//class CswUpdateSchemaCase27866

}//namespace ChemSW.Nbt.Schema