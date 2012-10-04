
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27665
    /// </summary>
    public class CswUpdateSchema_01S_Case27665 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.SizeClass );
            CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp InitialQuantityNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.InitialQuantity );
                InitialQuantityNtp.Attribute1 = CswConvert.ToDbVal( true ).ToString();
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27665; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema