
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27670
    /// </summary>
    public class CswUpdateSchemaCase27670 : CswUpdateSchemaTo
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
                if( null != InitialQuantityNtp )
                { 
                    InitialQuantityNtp.setIsCompoundUnique( true );
                }
                CswNbtMetaDataNodeTypeProp CatalogNoNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.CatalogNo );
                if( null != CatalogNoNtp )
                {
                    CatalogNoNtp.setIsCompoundUnique( true );
                }
                CswNbtMetaDataNodeTypeProp MaterialNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.Material );
                if( null != MaterialNtp )
                {
                    MaterialNtp.setIsCompoundUnique( true );
                }
            }
            
        }//Update()

    }//class CswUpdateSchemaCase27670

}//namespace ChemSW.Nbt.Schema