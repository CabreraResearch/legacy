using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29089
    /// </summary>
    public class CswUpdateSchema_01Y_Case29089 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29089; }
        }

        public override void update()
        {
            //Add the prerequisites for modules to be activated to the DB
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.CISPro, CswNbtModuleName.SDS );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.CISPro, CswNbtModuleName.RegulatoryLists );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.CISPro, CswNbtModuleName.Containers );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.CISPro, CswNbtModuleName.C3 );

            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.Containers, CswNbtModuleName.MultiInventoryGroup );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.Containers, CswNbtModuleName.MLM );
            _CswNbtSchemaModTrnsctn.Modules.CreateModuleDependency( CswNbtModuleName.Containers, CswNbtModuleName.FireCode );

            //Make the Customer NT.modules property read-only
            CswNbtMetaDataObjectClass customerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.CustomerClass );
            CswNbtMetaDataObjectClassProp modulesOCP = customerOC.getObjectClassProp( CswNbtObjClassCustomer.PropertyName.ModulesEnabled );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( modulesOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

        } //Update()

    }//class CswUpdateSchema_01Y_Case29089

}//namespace ChemSW.Nbt.Schema