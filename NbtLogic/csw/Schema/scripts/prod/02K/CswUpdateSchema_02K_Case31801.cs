using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31801 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 31801; }
        }

        public override string Title
        {
            get { return "Require Tab Names for New Tabs"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass );
            CswNbtMetaDataObjectClassProp TabNameOCP = TabOC.getObjectClassProp( CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TabNameOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema