using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31893_Supplies : CswUpdateSchemaTo
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
            get { return "CAF: Supply Bindings"; }
        }

        public override string AppendToScriptName()
        {
            return "F";
        }

        public override void update()
        {
            // CAF bindings definitions for Supplies
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );
            ImpMgr.CAFimportOrder( "Supply", "packages", "supplies_view", "packageid" );

            //Simple Props
            ImpMgr.importBinding( "materialid", CswNbtPropertySetMaterial.PropertyName.LegacyMaterialId, "" );
            ImpMgr.importBinding( "productdescription", "Description", "" );
            ImpMgr.importBinding( "materialname", CswNbtPropertySetMaterial.PropertyName.TradeName, "" );
            ImpMgr.importBinding( "productno", CswNbtPropertySetMaterial.PropertyName.PartNumber, "" );
            ImpMgr.importBinding( "approved_trans", CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving, "" );

            //Relationships
            ImpMgr.importBinding( "vendorid", CswNbtPropertySetMaterial.PropertyName.Supplier, CswEnumNbtSubFieldName.NodeID.ToString() );

            //LOBs
            ImpMgr.importBinding( "struct_pict", CswNbtObjClassNonChemical.PropertyName.Picture, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName: "materials", LobDataPkColOverride: "materialid" );

            ImpMgr.finalize();
        } // update()

    }

}//namespace ChemSW.Nbt.Schema