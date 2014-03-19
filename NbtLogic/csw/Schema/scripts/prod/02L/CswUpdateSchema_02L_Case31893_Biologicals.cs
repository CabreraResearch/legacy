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
    public class CswUpdateSchema_02L_Case31893_Biologicals : CswUpdateSchemaTo
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
            get { return "CAF: Biological Bindings"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            bool DefinitionExists = CswNbtImportDef.checkForDefinitionEntries( _CswNbtSchemaModTrnsctn, CswScheduleLogicNbtCAFImport.DefinitionName );
            if( DefinitionExists )
            {
            // CAF bindings definitions for Biologicals
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            ImpMgr.CAFimportOrder( "Biological", "packages", "biologicals_view", "packageid" );

            //Simple Props
            ImpMgr.importBinding( "materialid", CswNbtPropertySetMaterial.PropertyName.LegacyMaterialId, "" );
            ImpMgr.importBinding( "refno", "Reference Number", "" );
            ImpMgr.importBinding( "type", "Type", "" );
            ImpMgr.importBinding( "species", "Species Origin", "" );
            ImpMgr.importBinding( "biosafety", "Biosafety Level", "" );
            ImpMgr.importBinding( "vectors", "Vectors", "" );
            ImpMgr.importBinding( "storage_conditions", "Storage Conditions", "" );
            ImpMgr.importBinding( "materialname", "Biological Name", "" );
            ImpMgr.importBinding( "productno", CswNbtPropertySetMaterial.PropertyName.PartNumber, "" );
            ImpMgr.importBinding( "approved_trans", CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving, "" );

            //Relationships
            ImpMgr.importBinding( "vendorid", CswNbtPropertySetMaterial.PropertyName.Supplier, CswEnumNbtSubFieldName.NodeID.ToString() );

            //LOBs
            ImpMgr.importBinding( "struct_pict", CswNbtObjClassNonChemical.PropertyName.Picture, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName: "materials", LobDataPkColOverride: "materialid" );

            ImpMgr.finalize();
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema