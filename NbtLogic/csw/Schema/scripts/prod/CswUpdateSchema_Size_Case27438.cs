using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27438
    /// </summary>
    public class CswUpdateSchema_Size_Case27438 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.SizeClass );
            CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;

            if( SizeNt != null )
            {
                //Hide Dispensable and QuantityEditable properties
                CswNbtMetaDataNodeTypeProp DispensableNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Dispensable );
                DispensableNtp.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp QuantityEditableNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.QuantityEditable );
                QuantityEditableNtp.removeFromAllLayouts();
            }

        }//Update()

    }//class CswUpdateSchemaCase27438

}//namespace ChemSW.Nbt.Schema