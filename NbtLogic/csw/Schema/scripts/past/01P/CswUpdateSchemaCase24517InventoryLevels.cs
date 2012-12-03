using ChemSW.Config;
using ChemSW.Log;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24517InventoryLevels
    /// </summary>
    public class CswUpdateSchemaCase24517InventoryLevels : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InventoryLevelOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InventoryLevelClass, "docs.gif", true, false );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp IlMaterialOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.Material,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = MaterialOc.ObjectClassId,
                IsRequired = true,
                IsCompoundUnique = true
            } );

            CswNbtMetaDataObjectClassProp IlLocationOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.Location,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location,
                IsRequired = true,
                IsCompoundUnique = true
            } );

            CswNbtMetaDataObjectClassProp IlTypeOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.Type,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = CswNbtObjClassInventoryLevel.Types.Options.ToString(),
                IsRequired = true,
                IsCompoundUnique = true
            } );

            CswNbtMetaDataObjectClassProp IlLevelOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.Level,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity,
                IsRequired = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.Subscribe,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.UserSelect,
                SetValOnAdd = true
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.LastNotified,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                ServerManaged = true
            } );

            CswNbtMetaDataObjectClassProp IlStatusOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryLevelOc, new CswNbtWcfMetaDataModel.ObjectClassProp
            {
                PropName = CswNbtObjClassInventoryLevel.PropertyName.Status,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = CswNbtObjClassInventoryLevel.Statuses.Options.ToString(),
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IlStatusOcp, IlStatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtObjClassInventoryLevel.Statuses.Ok );

            _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtResources.CswNbtModule.CISPro, InventoryLevelOc.ObjectClassId );

            CswNbtMetaDataNodeType InventoryLevelNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inventory Level" );
            if( null != InventoryLevelNt && InventoryLevelNt.ObjectClassId != InventoryLevelOc.ObjectClassId )
            {
                InventoryLevelNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "CISPro Inventory Level" );
            }
            if( null != InventoryLevelNt && InventoryLevelNt.ObjectClassId != InventoryLevelOc.ObjectClassId )
            {
                CswStatusMessage Msg = new CswStatusMessage
                                            {
                                                AppType = AppType.SchemUpdt,
                                                ContentType = ContentType.Error
                                            };
                Msg.Attributes.Add( ChemSW.Log.LegalAttribute.exoteric_message, "Nodetypes 'Inventory Level' and 'CISPro Inventory Level' already exist and are not of the InventoryLevel Object Class." );
                _CswNbtSchemaModTrnsctn.CswLogger.send( Msg );
            }
            if( null == InventoryLevelNt )
            {
                InventoryLevelNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( InventoryLevelOc )
                                                                         {
                                                                             NodeTypeName = "Inventory Level",
                                                                             Category = "Materials",
                                                                         } );
                CswNbtMetaDataNodeTypeProp Material = InventoryLevelNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
                CswNbtMetaDataNodeTypeProp Level = InventoryLevelNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Level );
                CswNbtMetaDataNodeTypeProp Type = InventoryLevelNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Type );
                InventoryLevelNt.addNameTemplateText( Material.PropName );
                InventoryLevelNt.addNameTemplateText( Type.PropName );
                InventoryLevelNt.addNameTemplateText( Level.PropName );
            }

            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeTab PhysicalTab = MaterialNt.getNodeTypeTab( "Physical Tab" );
                if( null != PhysicalTab )
                {
                    PhysicalTab.TabName = "Physical";
                }
                else
                {
                    PhysicalTab = MaterialNt.getNodeTypeTab( "Physical" );
                    if( null == PhysicalTab )
                    {
                        PhysicalTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( MaterialNt, "Physical", MaterialNt.getNodeTypeTabIds().Count );
                    }
                }
                CswNbtMetaDataNodeTypeProp LevelsNtp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( MaterialNt, _CswNbtSchemaModTrnsctn.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Grid ), "Inventory Levels" )
                {
                    TabId = PhysicalTab.TabId
                } );
                LevelsNtp.Extended = CswNbtNodePropGrid.GridPropMode.Link.ToString();

                CswNbtView LevelsView = _CswNbtSchemaModTrnsctn.restoreView( LevelsNtp.ViewId );
                if( null == LevelsView )
                {
                    LevelsView = _CswNbtSchemaModTrnsctn.makeView();
                    LevelsNtp.ViewId = LevelsView.ViewId;
                }

                LevelsView.Root.ChildRelationships.Clear();
                LevelsView.ViewMode = NbtViewRenderingMode.Grid;
                LevelsView.Visibility = NbtViewVisibility.Property;

                CswNbtViewRelationship RootRel = LevelsView.AddViewRelationship( MaterialNt, true );
                CswNbtViewRelationship LevelRel = LevelsView.AddViewRelationship( RootRel, NbtViewPropOwnerType.Second, IlMaterialOcp, true );
                LevelsView.AddViewProperty( LevelRel, IlTypeOcp );
                LevelsView.AddViewProperty( LevelRel, IlLevelOcp );
                LevelsView.AddViewProperty( LevelRel, IlLocationOcp );
                LevelsView.AddViewProperty( LevelRel, IlStatusOcp );
                LevelsView.save();
            }

        }//Update()

    }//class CswUpdateSchemaCase24517InventoryLevels

}//namespace ChemSW.Nbt.Schema