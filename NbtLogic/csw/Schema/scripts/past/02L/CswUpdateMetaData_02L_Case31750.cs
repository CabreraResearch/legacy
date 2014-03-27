using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    public class CswUpdateMetaData_02L_Case31750 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31750; }
        }

        public override string Title
        {
            get { return "Unit name is now unique on the Unit OC"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp NameOCP = UnitOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOCP, CswEnumNbtObjectClassPropAttributes.isglobalunique, true );
        } // update()

    }//class CswUpdateMetaData_02L_Case31750

}//namespace ChemSW.Nbt.Schema