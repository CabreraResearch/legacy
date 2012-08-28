
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27622
    /// </summary>
    public class CswUpdateSchemaCase27622 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataNodeType ChemicalNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNt )
            {
                ChemicalNt.setNameTemplateText(
                    CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.PropertyName.Tradename ) + " " +
                    CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.PropertyName.Supplier ) + " " +
                    CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.PropertyName.PartNumber )
                    );
            }
        }//Update()

    }//class CswUpdateSchemaCase27622

}//namespace ChemSW.Nbt.Schema