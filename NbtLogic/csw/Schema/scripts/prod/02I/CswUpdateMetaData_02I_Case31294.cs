using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02I_Case31294 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31294; }
        }

        public override string Title
        {
            get { return "Enforce uniqueness on balance name"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass BalanceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
            CswNbtMetaDataObjectClassProp NameProp = BalanceOC.getObjectClassProp( CswNbtObjClassBalance.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameProp, CswEnumNbtObjectClassPropAttributes.isunique, CswConvert.ToDbVal( true ) );
        } // update()
    }

}//namespace ChemSW.Nbt.Schema