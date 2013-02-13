using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28659
    /// </summary>
    public class CswUpdateSchema_01W_Case28659 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28659; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );
            CswNbtMetaDataObjectClassProp UnitCountOCP = SizeOC.getObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( UnitCountOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, false );
        }//Update()
    }//class CswUpdateSchemaCase_01W_28659
}//namespace ChemSW.Nbt.Schema