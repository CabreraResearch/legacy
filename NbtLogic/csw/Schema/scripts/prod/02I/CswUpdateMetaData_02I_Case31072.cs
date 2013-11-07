using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31072 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31072; }
        }

        public override string Title
        {
            get { return "Make UnitOfMeasure BaseUnit ServerManaged"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UoMOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp BaseUnitOCP = UoMOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.BaseUnit );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( BaseUnitOCP, CswEnumNbtObjectClassPropAttributes.servermanaged, true );
        } // update()

    }

}//namespace ChemSW.Nbt.Schema