
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27665
    /// </summary>
    public class CswUpdateSchemaCase27665 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp InitialQuantityNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
                InitialQuantityNtp.Attribute1 = CswConvert.ToDbVal( true ).ToString();
            }
        }//Update()

    }

}//namespace ChemSW.Nbt.Schema