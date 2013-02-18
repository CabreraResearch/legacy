using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28779
    /// </summary>
    public class CswUpdateSchema_01W_Case28779 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28779; }
        }

        public override void update()
        {
            //Set Size NTs name template to "[{Unit Count}] x {Initial Quantity} {Catalog No}"
            CswNbtMetaDataObjectClass sizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            foreach( CswNbtMetaDataNodeType sizeNT in sizeOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp unitCountNTP = sizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount );
                CswNbtMetaDataNodeTypeProp initQuantNTP = sizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
                CswNbtMetaDataNodeTypeProp catalogNoNTP = sizeNT.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );

                string unitCountTmp = CswNbtMetaData.MakeTemplateEntry( unitCountNTP.PropName );
                string initQuantTmp = CswNbtMetaData.MakeTemplateEntry( initQuantNTP.PropName );
                string catNoTmp = CswNbtMetaData.MakeTemplateEntry( catalogNoNTP.PropName );

                sizeNT.setNameTemplateText( "[" + unitCountTmp + "] x " + initQuantTmp + " " + catNoTmp );
            }

        } //Update()

    }//class CswUpdateSchema_01V_Case28779

}//namespace ChemSW.Nbt.Schema