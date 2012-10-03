
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27670
    /// </summary>
    public class CswUpdateSchema_01S_Case27670 : CswUpdateSchemaTo
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
            
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27670; }
        }

        //Update()

    }//class CswUpdateSchemaCase27670

}//namespace ChemSW.Nbt.Schema