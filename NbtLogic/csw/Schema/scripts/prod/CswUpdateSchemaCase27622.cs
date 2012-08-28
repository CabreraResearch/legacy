
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
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                if( MaterialNt.NodeTypeName == "Chemical" )
                {
                    MaterialNt.setNameTemplateText(
                        CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.PropertyName.Tradename ) + " " +
                        CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.PropertyName.Supplier ) + " " +
                        CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterial.PropertyName.PartNumber )
                        );
                }
            }
        }//Update()

    }//class CswUpdateSchemaCase27622

}//namespace ChemSW.Nbt.Schema