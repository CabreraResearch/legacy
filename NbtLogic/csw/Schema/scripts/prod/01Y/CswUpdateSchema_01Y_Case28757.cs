using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28757
    /// </summary>
    public class CswUpdateSchema_01Y_Case28757 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28757; }
        }

        public override void update()
        {
            _updateDocumentProps( "SDS Document" );
            _updateDocumentProps( "Material Document" );
        } //Update()

        private void _updateDocumentProps( string NodeTypeName )
        {
            CswNbtMetaDataNodeType DocNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( NodeTypeName );
            if( null != DocNT )
            {
                CswNbtMetaDataNodeTypeProp OwnerNTP = DocNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                OwnerNTP.PropName = "Material";
                OwnerNTP.ReadOnly = true;
                CswNbtMetaDataNodeTypeProp TitleNTP = DocNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
                TitleNTP.TextAreaColumns = 40;
            }
        }
    }//class CswUpdateSchema_01Y_Case28757
}//namespace ChemSW.Nbt.Schema