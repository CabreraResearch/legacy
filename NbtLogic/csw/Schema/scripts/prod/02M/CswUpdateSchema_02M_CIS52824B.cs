using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52824B: CswUpdateSchemaTo
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
            get { return "Create Equipment CAF bindings"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            if( CswNbtImportDef.checkForDefinitionEntries( _CswNbtSchemaModTrnsctn, "CAF" ) )
            {
                CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( _CswNbtSchemaModTrnsctn, "CAF" );

                ImpMgr.CAFimportOrder( "Equipment", "materials", "equipment_view", "legacyid", createLegacyId: true );

                //Simple props
                ImpMgr.importBinding( "serialno", CswNbtObjClassEquipment.PropertyName.SerialNo, "" );
                ImpMgr.importBinding( "barcodeid", CswNbtObjClassEquipment.PropertyName.EquipmentId, "" );
                ImpMgr.importBinding( "expirationdate", CswNbtObjClassEquipment.PropertyName.ServiceEndsOn, "" );
                ImpMgr.importBinding( "model", CswNbtObjClassEquipment.PropertyName.Model, "" );
                ImpMgr.importBinding( "manufacturer", CswNbtObjClassEquipment.PropertyName.Manufacturer, "" );
                ImpMgr.importBinding( "description", CswNbtObjClassEquipment.PropertyName.Description, "" );

                //Relationships and Locations
                ImpMgr.importBinding( "ownerid", CswNbtObjClassEquipment.PropertyName.User, CswEnumNbtSubFieldName.NodeID.ToString() );
                ImpMgr.importBinding( "locationid", CswNbtObjClassEquipment.PropertyName.Location, CswEnumNbtSubFieldName.NodeID.ToString() );
                ImpMgr.importBinding( "materialsubclassid", CswNbtObjClassEquipment.PropertyName.Type, CswEnumNbtSubFieldName.NodeID.ToString() );

                //LOB data
                ImpMgr.importBinding( "struct_pict", CswNbtObjClassEquipment.PropertyName.Picture, CswEnumNbtSubFieldName.Blob.ToString(), BlobTableName: "materials", LobDataPkColOverride: "materialid" );

                ImpMgr.finalize();
            }
        }
    }
}