using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24517InventoryLevelsViews
    /// </summary>
    public class CswUpdateSchemaCase24517InventoryLevelsViews : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryLevelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryLevelClass );
            CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp IlMaterialOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
            CswNbtMetaDataObjectClassProp IlLocationOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Location );
            CswNbtMetaDataObjectClassProp IlTypeOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Type );
            CswNbtMetaDataObjectClassProp IlLevelOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Level );
            CswNbtMetaDataObjectClassProp IlStatusOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Status );
            CswNbtMetaDataObjectClassProp IlCurrentQuantityOcp = InventoryLevelOc.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantity );

            foreach( CswNbtMetaDataNodeType LocationNt in LocationOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab LevelTab = LocationNt.getNodeTypeTab( "Inventory Levels" );
                if( null == LevelTab )
                {
                    LevelTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( LocationNt, "Inventory Levels", LocationNt.getNodeTypeTabIds().Count );
                }
                CswNbtMetaDataNodeTypeProp LevelsNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( LocationNt, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ), "Inventory Levels" )
                {
                    TabId = LevelTab.TabId
                } );

                CswNbtView LevelsView = _CswNbtSchemaModTrnsctn.restoreView( LevelsNtp.ViewId );
                if( null == LevelsView )
                {
                    LevelsView = _CswNbtSchemaModTrnsctn.makeView();
                    LevelsNtp.ViewId = LevelsView.ViewId;
                }

                LevelsView.Root.ChildRelationships.Clear();
                LevelsView.ViewMode = NbtViewRenderingMode.Grid;
                LevelsView.Visibility = NbtViewVisibility.Property;

                CswNbtViewRelationship RootRel = LevelsView.AddViewRelationship( LocationNt, true );
                CswNbtViewRelationship LevelRel = LevelsView.AddViewRelationship( RootRel, NbtViewPropOwnerType.Second, IlLocationOcp, true );
                LevelsView.AddViewProperty( LevelRel, IlMaterialOcp );
                LevelsView.AddViewProperty( LevelRel, IlTypeOcp );
                LevelsView.AddViewProperty( LevelRel, IlLevelOcp );
                LevelsView.AddViewProperty( LevelRel, IlStatusOcp );
                LevelsView.AddViewProperty( LevelRel, IlCurrentQuantityOcp );
                LevelsView.save();
            }

            CswNbtView AboveMinimumInventoryView = _CswNbtSchemaModTrnsctn.makeView();
            AboveMinimumInventoryView.makeNew( "Above Maximum Inventory", NbtViewVisibility.Global );
            AboveMinimumInventoryView.ViewMode = NbtViewRenderingMode.Grid;
            AboveMinimumInventoryView.Category = "Materials";
            CswNbtViewRelationship AmivRel = AboveMinimumInventoryView.AddViewRelationship( InventoryLevelOc, true );
            AboveMinimumInventoryView.AddViewProperty( AmivRel, IlMaterialOcp );
            AboveMinimumInventoryView.AddViewProperty( AmivRel, IlLocationOcp );
            AboveMinimumInventoryView.AddViewPropertyAndFilter( AmivRel, IlTypeOcp, CswNbtObjClassInventoryLevel.Types.Maximum );
            AboveMinimumInventoryView.AddViewProperty( AmivRel, IlLevelOcp );
            AboveMinimumInventoryView.AddViewPropertyAndFilter( AmivRel, IlStatusOcp, CswNbtObjClassInventoryLevel.Statuses.Above );
            AboveMinimumInventoryView.AddViewProperty( AmivRel, IlCurrentQuantityOcp );
            AboveMinimumInventoryView.save();

            CswNbtView BelowMinimumInventoryView = _CswNbtSchemaModTrnsctn.makeView();
            BelowMinimumInventoryView.makeNew( "Below Minimum Inventory", NbtViewVisibility.Global );
            BelowMinimumInventoryView.ViewMode = NbtViewRenderingMode.Grid;
            BelowMinimumInventoryView.Category = "Materials";
            CswNbtViewRelationship BmivRel = BelowMinimumInventoryView.AddViewRelationship( InventoryLevelOc, true );
            BelowMinimumInventoryView.AddViewProperty( BmivRel, IlMaterialOcp );
            BelowMinimumInventoryView.AddViewProperty( BmivRel, IlLocationOcp );
            BelowMinimumInventoryView.AddViewPropertyAndFilter( BmivRel, IlTypeOcp, CswNbtObjClassInventoryLevel.Types.Minimum ); 
            BelowMinimumInventoryView.AddViewProperty( BmivRel, IlLevelOcp );
            BelowMinimumInventoryView.AddViewPropertyAndFilter( BmivRel, IlStatusOcp, CswNbtObjClassInventoryLevel.Statuses.Below );
            BelowMinimumInventoryView.AddViewProperty( BmivRel, IlCurrentQuantityOcp );
            BelowMinimumInventoryView.save();



        }//Update()                  

    }//class CswUpdateSchemaCase24517InventoryLevelsViews

}//namespace ChemSW.Nbt.Schema