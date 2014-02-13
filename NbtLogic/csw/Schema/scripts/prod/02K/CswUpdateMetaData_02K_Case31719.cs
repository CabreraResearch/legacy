using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02K_Case31719 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31719; }
        }

        public override string Title
        {
            get { return "Request button should always show Request as text"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp RequestOCP = ContainerOC.getObjectClassProp( "Request" );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RequestOCP, CswEnumNbtObjectClassPropAttributes.statictext, "Request" );
            // This is a placeholder script that does nothing.
        } // update()

    }

}//namespace ChemSW.Nbt.Schema