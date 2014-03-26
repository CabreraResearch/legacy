using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31893_MaterialComps : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31893; }
        }

        public override string Title
        {
            get { return "CAF: Material Component Bindings"; }
        }

        public override string AppendToScriptName()
        {
            return "E";
        }

        public override void update()
        {
            bool DefinitionExists = CswNbtImportDef.checkForDefinitionEntries( _CswNbtSchemaModTrnsctn, CswScheduleLogicNbtCAFImport.DefinitionName );
            if( DefinitionExists )
            {
            // CAF bindings definitions for Biologicals
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            ImpMgr.CAFimportOrder( "Material Component", "component_casnos", "materialcomps_view", "legacyid" );

            //Simple Props
            ImpMgr.importBinding( "quantity", CswNbtObjClassMaterialComponent.PropertyName.TargetPercentageValue, "" );
            ImpMgr.importBinding( "quantity", CswNbtObjClassMaterialComponent.PropertyName.HighPercentageValue, "" );

            //Relationships
            ImpMgr.importBinding( "packageid", CswNbtObjClassMaterialComponent.PropertyName.Mixture, CswEnumNbtSubFieldName.NodeID.ToString() );
            ImpMgr.importBinding( "constituentid", CswNbtObjClassMaterialComponent.PropertyName.Constituent, CswEnumNbtSubFieldName.NodeID.ToString() );

            ImpMgr.finalize();
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema