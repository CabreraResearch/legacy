
using ChemSW.Config;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using CswNbtMetaDataNodeType = ChemSW.Nbt.MetaData.CswNbtMetaDataNodeType;
using CswNbtMetaDataNodeTypeTab = ChemSW.Nbt.MetaData.CswNbtMetaDataNodeTypeTab;

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
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.Dev; } }
        protected override void OnEnable()
        {
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswEnumConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswEnumConfigurationVariableNames.Logging_Level.ToString(), "Info" );
            }

            CswNbtMetaDataNodeType FieldTypeNt = _CswNbtResources.MetaData.getNodeType( "Csw Dev FieldType Test" );
            if( null == FieldTypeNt )
            {
                FieldTypeNt = _CswNbtResources.MetaData.makeNewNodeTypeNew(
                    new CswNbtWcfMetaDataModel.NodeType( _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ) )
                        {
                            NodeTypeName = "Csw Dev FieldType Test",
                            Category = "Csw Dev"
                        } );

                CswNbtMetaDataNodeTypeTab SimpleTab = FieldTypeNt.getNodeTypeTab( "Csw Dev FieldType Test" );
                if( null != SimpleTab )
                {
                    //SimpleTab.TabName = "Simple";
                    SimpleTab.DesignNode.TabName.Text = "Simple";
                    SimpleTab.DesignNode.postChanges( false );
                }
                else
                {
                    SimpleTab = _CswNbtResources.MetaData.makeNewTabNew( FieldTypeNt, "Simple", 1 );
                }
                CswNbtMetaDataNodeTypeTab LessSimpleTab = _CswNbtResources.MetaData.makeNewTabNew( FieldTypeNt, "Less Simple", 2 );
                CswNbtMetaDataNodeTypeTab ComplexTab = _CswNbtResources.MetaData.makeNewTabNew( FieldTypeNt, "Complex", 3 );

                foreach( CswNbtMetaDataFieldType FieldType in _CswNbtResources.MetaData.getFieldTypes() )
                {
                    switch( FieldType.FieldType )
                    {
                        case CswEnumNbtFieldType.Barcode:
                        case CswEnumNbtFieldType.DateTime:
                        case CswEnumNbtFieldType.Image:
                        case CswEnumNbtFieldType.List:
                        case CswEnumNbtFieldType.Logical:
                        case CswEnumNbtFieldType.Memo:
                        case CswEnumNbtFieldType.Number:
                        case CswEnumNbtFieldType.PropertyReference:
                        case CswEnumNbtFieldType.Sequence:
                        case CswEnumNbtFieldType.Static:
                        case CswEnumNbtFieldType.Text:
                            _CswNbtResources.MetaData.makeNewPropNew( new CswNbtWcfMetaDataModel.NodeTypeProp( FieldTypeNt, FieldType, FieldType.FieldType.ToString() )
                                {
                                    TabId = SimpleTab.TabId
                                } );
                            break;

                        case CswEnumNbtFieldType.Comments:
                        case CswEnumNbtFieldType.Composite:
                        case CswEnumNbtFieldType.File:
                        case CswEnumNbtFieldType.ImageList:
                        case CswEnumNbtFieldType.Link:
                        case CswEnumNbtFieldType.MOL:
                        case CswEnumNbtFieldType.MTBF:
                        case CswEnumNbtFieldType.Password:
                        case CswEnumNbtFieldType.Quantity:
                        case CswEnumNbtFieldType.Scientific:
                        case CswEnumNbtFieldType.ViewReference:
                            _CswNbtResources.MetaData.makeNewPropNew( new CswNbtWcfMetaDataModel.NodeTypeProp( FieldTypeNt, FieldType, FieldType.FieldType.ToString() )
                                {
                                    TabId = LessSimpleTab.TabId
                                } );
                            break;

                        case CswEnumNbtFieldType.Grid:
                        case CswEnumNbtFieldType.Location:
                        case CswEnumNbtFieldType.LogicalSet:
                        case CswEnumNbtFieldType.MultiList:
                        case CswEnumNbtFieldType.Question:
                        case CswEnumNbtFieldType.NFPA:
                        case CswEnumNbtFieldType.NodeTypeSelect:
                        case CswEnumNbtFieldType.Relationship:
                        case CswEnumNbtFieldType.TimeInterval:
                        case CswEnumNbtFieldType.ViewPickList:
                        case CswEnumNbtFieldType.UserSelect:
                            _CswNbtResources.MetaData.makeNewPropNew( new CswNbtWcfMetaDataModel.NodeTypeProp( FieldTypeNt, FieldType, FieldType.FieldType.ToString() )
                                {
                                    TabId = ComplexTab.TabId
                                } );
                            break;
                    }
                }

                CswNbtView FieldTypeView = new CswNbtView( _CswNbtResources );
                FieldTypeView.saveNew( "Field Types", CswEnumNbtViewVisibility.User, null, _CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername ).NodeId );
                FieldTypeView.AddViewRelationship( FieldTypeNt, false );
                FieldTypeView.Category = "Csw Dev";
                FieldTypeView.save();

                CswNbtNode Node1 = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId );
                CswNbtNode Node2 = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId );
                Node1.IsDemo = true;
                Node1.postChanges( ForceUpdate: false );
                Node2.IsDemo = true;
                Node2.postChanges( ForceUpdate: false );
            }
        }

        protected override void OnDisable()
        {
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswEnumConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswEnumConfigurationVariableNames.Logging_Level.ToString(), "None" );
            }
        }
    } // class CswNbtModuleRuleDev
}// namespace ChemSW.Nbt
