using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28508
    /// </summary>
    public class CswUpdateSchema_01W_Case28508 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28508; }
        }

        public override void update()
        {
            // Set container name template to include Disposed
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            foreach( CswNbtMetaDataNodeType ContainerNT in ContainerOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp BarcodeNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode );
                CswNbtMetaDataNodeTypeProp DisposedNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed );
                ContainerNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( BarcodeNTP.PropName ) +
                                                 " " +
                                                 CswNbtMetaData.MakeTemplateEntry( DisposedNTP.PropName ) );
            }
        } //update()

    }//class CswUpdateSchema_01V_Case28508

}//namespace ChemSW.Nbt.Schema