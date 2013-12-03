using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case30989: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30989; }
        }

        public override string Title
        {
            get { return "Make Users Available Work Units prop required"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp AvailableWorkUnitsOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.AvailableWorkUnits );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AvailableWorkUnitsOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema