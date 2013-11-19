using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case31198 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CM; }
        }

        public override int CaseNo
        {
            get { return 31198; }
        }

        public override string Title
        {
            get { return "Fix Inventory Levels View: OCP"; }
        }

        public override string AppendToScriptName()
        {
            return "A";
        }

        public override void update()
        {
            // Recreate the Inventory Level Grid view on Location OC
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp InventoryLevelsOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryLevels );

            CswNbtMetaDataObjectClass InventoryLvlsOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryLevelClass );
            CswNbtMetaDataObjectClassProp LocationOCP = InventoryLvlsOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Location );

            CswNbtView InventoryLevelsView = _CswNbtSchemaModTrnsctn.makeView();
            InventoryLevelsView.ViewName = CswNbtObjClassLocation.PropertyName.InventoryLevels;
            InventoryLevelsView.SetViewMode( CswEnumNbtViewRenderingMode.Grid );

            CswNbtViewRelationship LocationRel = InventoryLevelsView.AddViewRelationship( LocationOC, false );
            CswNbtViewRelationship InventoryLvlsRel = InventoryLevelsView.AddViewRelationship( LocationRel, CswEnumNbtViewPropOwnerType.Second, LocationOCP, true );
            InventoryLevelsView.AddViewProperty( InventoryLvlsRel, InventoryLvlsOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantity ), 1 );
            InventoryLevelsView.AddViewProperty( InventoryLvlsRel, InventoryLvlsOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Level ), 2 );
            InventoryLevelsView.AddViewProperty( InventoryLvlsRel, InventoryLvlsOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material ), 3 );
            InventoryLevelsView.AddViewProperty( InventoryLvlsRel, InventoryLvlsOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Status ), 4 );
            InventoryLevelsView.AddViewProperty( InventoryLvlsRel, InventoryLvlsOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Type ), 5 );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( InventoryLevelsOCP, CswEnumNbtObjectClassPropAttributes.viewxml, InventoryLevelsView.ToString() );

        } // update()
    }

}//namespace ChemSW.Nbt.Schema