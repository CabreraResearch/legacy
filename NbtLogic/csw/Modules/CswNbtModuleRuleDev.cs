
using ChemSW.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using CswNbtMetaDataNodeType = ChemSW.Nbt.MetaData.CswNbtMetaDataNodeType;
using CswNbtMetaDataNodeTypeTab = ChemSW.Nbt.MetaData.CswNbtMetaDataNodeTypeTab;
using CswNbtMetaDataObjectClass = ChemSW.Nbt.MetaData.CswNbtMetaDataObjectClass;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Dev Module
    /// </summary>
    public class CswNbtModuleRuleDev : CswNbtModuleRule
    {
        public CswNbtModuleRuleDev( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.Dev; } }
        public override void OnEnable()
        {
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level.ToString(), "Info" );
            }

            CswNbtMetaDataNodeType FieldTypeNt = _CswNbtResources.MetaData.getNodeType( "Csw Dev FieldType Test" );
            if( null == FieldTypeNt )
            {
                FieldTypeNt = _CswNbtResources.MetaData.makeNewNodeType( NbtObjectClass.GenericClass.ToString(), "Csw Dev FieldType Test", "Csw Dev" );

                CswNbtMetaDataNodeTypeTab SimpleTab = FieldTypeNt.getNodeTypeTab( "Csw Dev FieldType Test" );
                if( null != SimpleTab )
                {
                    SimpleTab.TabName = "Simple";
                }
                else
                {
                    SimpleTab = _CswNbtResources.MetaData.makeNewTab( FieldTypeNt, "Simple", 1 );
                }
                CswNbtMetaDataNodeTypeTab LessSimpleTab = _CswNbtResources.MetaData.makeNewTab( FieldTypeNt, "Less Simple", 2 );
                CswNbtMetaDataNodeTypeTab ComplexTab = _CswNbtResources.MetaData.makeNewTab( FieldTypeNt, "Complex", 3 );

                foreach( CswNbtMetaDataFieldType FieldType in _CswNbtResources.MetaData.getFieldTypes() )
                {
                    switch( FieldType.FieldType )
                    {
                        case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                        case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                        case CswNbtMetaDataFieldType.NbtFieldType.Image:
                        case CswNbtMetaDataFieldType.NbtFieldType.List:
                        case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                        case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                        case CswNbtMetaDataFieldType.NbtFieldType.Number:
                        case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                        case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                        case CswNbtMetaDataFieldType.NbtFieldType.Static:
                        case CswNbtMetaDataFieldType.NbtFieldType.Text:
                            _CswNbtResources.MetaData.makeNewProp( FieldTypeNt, FieldType.FieldType, FieldType.FieldType.ToString(), SimpleTab.TabId );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Comments:
                        case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                        case CswNbtMetaDataFieldType.NbtFieldType.File:
                        case CswNbtMetaDataFieldType.NbtFieldType.ImageList:
                        case CswNbtMetaDataFieldType.NbtFieldType.Link:
                        case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                        case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                        case CswNbtMetaDataFieldType.NbtFieldType.Password:
                        case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                        case CswNbtMetaDataFieldType.NbtFieldType.Scientific:
                        case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                            _CswNbtResources.MetaData.makeNewProp( FieldTypeNt, FieldType.FieldType, FieldType.FieldType.ToString(), LessSimpleTab.TabId );
                            break;

                        case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                        case CswNbtMetaDataFieldType.NbtFieldType.Location:
                        case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                        case CswNbtMetaDataFieldType.NbtFieldType.MultiList:
                        case CswNbtMetaDataFieldType.NbtFieldType.Question:
                        case CswNbtMetaDataFieldType.NbtFieldType.NFPA:
                        case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                        case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                        case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                        case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                        case CswNbtMetaDataFieldType.NbtFieldType.UserSelect:
                            _CswNbtResources.MetaData.makeNewProp( FieldTypeNt, FieldType.FieldType, FieldType.FieldType.ToString(), ComplexTab.TabId );
                            break;
                    }
                }

                CswNbtView FieldTypeView = new CswNbtView( _CswNbtResources );
                FieldTypeView.saveNew( "Field Types", NbtViewVisibility.User, null, _CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername ).NodeId );
                FieldTypeView.AddViewRelationship( FieldTypeNt, false );
                FieldTypeView.Category = "Csw Dev";
                FieldTypeView.save();

                CswNbtNode Node1 = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtNode Node2 = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                Node1.IsDemo = true;
                Node1.postChanges( ForceUpdate: false );
                Node2.IsDemo = true;
                Node2.postChanges( ForceUpdate: false );
            }

            _CswNbtResources.Modules.ToggleView( false, "Containers", NbtViewVisibility.Global );
        }

        public override void OnDisable()
        {
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level.ToString(), "None" );
            }

            _CswNbtResources.Modules.ToggleView( true, "Containers", NbtViewVisibility.Global );
        }

    } // class CswNbtModuleRuleDev
}// namespace ChemSW.Nbt
