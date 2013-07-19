
using ChemSW.Config;
using ChemSW.Core;
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
#if DEBUG 
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswEnumConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswEnumConfigurationVariableNames.Logging_Level.ToString(), "Info" );
            }
            _CswNbtResources.SetupVbls.writeSetting( CswEnumSetupVariableNames.LogOutputToLoggly, "true" );
            _CswNbtResources.SetupVbls.writeSetting( CswEnumSetupVariableNames.ShowFullExceptions, "true" );

            CswNbtMetaDataNodeType FieldTypeNt = _CswNbtResources.MetaData.getNodeType( "Csw Dev FieldType Test" );
            if( null == FieldTypeNt )
            {
                FieldTypeNt = _CswNbtResources.MetaData.makeNewNodeType( CswEnumNbtObjectClass.GenericClass.ToString(), "Csw Dev FieldType Test", "Csw Dev" );

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
                            _CswNbtResources.MetaData.makeNewProp( FieldTypeNt, FieldType.FieldType, FieldType.FieldType.ToString(), SimpleTab.TabId );
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
                            _CswNbtResources.MetaData.makeNewProp( FieldTypeNt, FieldType.FieldType, FieldType.FieldType.ToString(), LessSimpleTab.TabId );
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
                            _CswNbtResources.MetaData.makeNewProp( FieldTypeNt, FieldType.FieldType, FieldType.FieldType.ToString(), ComplexTab.TabId );
                            break;
                    }
                }

                CswNbtView FieldTypeView = new CswNbtView( _CswNbtResources );
                FieldTypeView.saveNew( "Field Types", CswEnumNbtViewVisibility.User, null, _CswNbtResources.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername ).NodeId );
                FieldTypeView.AddViewRelationship( FieldTypeNt, false );
                FieldTypeView.Category = "Csw Dev";
                FieldTypeView.save();


                CswNbtNode Node1 = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                CswNbtNode Node2 = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                Node1.IsDemo = true;
                Node1.postChanges( ForceUpdate: false );
                Node2.IsDemo = true;
                Node2.postChanges( ForceUpdate: false );
            }

            _CswNbtResources.Modules.EnableModule( CswEnumNbtModuleName.NBTManager );

            CswNbtMetaDataNodeType CustomerNt = _CswNbtResources.MetaData.getNodeType( "Csw Dev Customers" );
            if( null == CustomerNt )
            {
                CustomerNt = _CswNbtResources.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CustomerClass ) )
                {
                    NodeTypeName = "Csw Dev Customers"
                } );

                CswNbtView CustomersView = new CswNbtView( _CswNbtResources );
                CustomersView.saveNew( "Csw Dev Customers", CswEnumNbtViewVisibility.Global );
                CustomersView.Category = "Csw Dev";
                CustomersView.AddViewRelationship( CustomerNt, IncludeDefaultFilters: true );
                CustomersView.save();

                foreach( string AccessId in _CswNbtResources.CswDbCfgInfo.AccessIds )
                {
                    CswNbtObjClassCustomer Cust = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( CustomerNt.NodeTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                    Cust.CompanyID.Text = AccessId;
                    Cust.postChanges( ForceUpdate: false );
                }
            }
        }
#endif
        protected override void OnDisable()
        {
#if DEBUG
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( CswEnumConfigurationVariableNames.Logging_Level ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( CswEnumConfigurationVariableNames.Logging_Level.ToString(), "None" );
            }
            _CswNbtResources.SetupVbls.writeSetting( CswEnumSetupVariableNames.LogOutputToLoggly, "false" );
            _CswNbtResources.SetupVbls.writeSetting( CswEnumSetupVariableNames.ShowFullExceptions, "false" );
#endif
        }
    } // class CswNbtModuleRuleDev
}// namespace ChemSW.Nbt
