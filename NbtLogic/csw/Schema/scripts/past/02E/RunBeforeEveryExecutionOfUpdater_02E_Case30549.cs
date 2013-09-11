using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02E_Case30549 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30549"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30549; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp ConversionFactorOCP = UoMOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.ConversionFactor );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ConversionFactorOCP, CswEnumNbtObjectClassPropAttributes.setvalonadd, true );
            foreach( CswNbtMetaDataNodeType UoMNT in UoMOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ConversionFactorNTP = UoMNT.getNodeTypePropByObjectClassProp( ConversionFactorOCP );
                CswNbtMetaDataNodeTypeProp NameNTP = UoMNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
                ConversionFactorNTP.updateLayout( CswEnumNbtLayoutType.Add, NameNTP, true );
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema