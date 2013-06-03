using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29798
    /// </summary>
    public class CswUpdateSchema_02B_Case29798 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 29798; }
        }

        public override void update()
        {
            // Yes, this is NT and NTP specific logic, but there are 5 NTs under the DocumentOC and we needed to work specifically with the SDS Document NT.
            CswNbtMetaDataNodeType SDSDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
            if( null != SDSDocumentNT )
            {
                CswNbtMetaDataNodeTypeProp TitleOCP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
                TitleOCP.DefaultValue.AsText.Text = "SDS";
                TitleOCP.removeFromLayout( CswEnumNbtLayoutType.Add );
            }

        } // update()

    }//class CswUpdateSchema_02B_Case29798

}//namespace ChemSW.Nbt.Schema