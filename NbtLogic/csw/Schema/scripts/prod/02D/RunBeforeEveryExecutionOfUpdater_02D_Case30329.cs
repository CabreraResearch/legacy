using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30329
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30329 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 30329; }
        }

        public override void update()
        {
            // Regulatory List List Code 'LOLI List Name' property should be required
            CswNbtMetaDataObjectClass RegListListCodeOC =
                _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
            CswNbtMetaDataObjectClassProp LOLIListNameOCP =
                RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LOLIListNameOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );

        } //Update()

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30329
}//namespace ChemSW.Nbt.Schema


