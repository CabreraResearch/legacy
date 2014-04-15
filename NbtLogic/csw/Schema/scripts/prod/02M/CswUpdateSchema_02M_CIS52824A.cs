using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52824A: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52824; }
        }

        public override string Title
        {
            get { return "Create Equipment Type CAF bindings"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            if( CswNbtImportDef.checkForDefinitionEntries( _CswNbtSchemaModTrnsctn, "CAF" ) )
            {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

                ImpMgr.CAFimportOrder( "Equipment Type", "materials_subclass", "equipment_type_view", "materialsubclassid" );

                //Simple props
                ImpMgr.importBinding( "subclassname", CswNbtObjClassEquipmentType.PropertyName.TypeName, "" );
                
                ImpMgr.finalize();
            }
        }
    }
}