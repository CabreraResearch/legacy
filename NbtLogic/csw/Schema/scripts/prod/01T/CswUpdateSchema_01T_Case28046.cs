using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case CswUpdateSchema_01T_Case28046
    /// </summary>
    public class CswUpdateSchema_01T_Case28046 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtMetaDataNodeType UnitOfMeasureNT in UnitOfMeasureOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ConversionFactorNTP = UnitOfMeasureNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor );
                ConversionFactorNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, null, false );
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28046; }
        }

        //Update()

    }//class CswUpdateSchema_01T_Case28046

}//namespace ChemSW.Nbt.Schema