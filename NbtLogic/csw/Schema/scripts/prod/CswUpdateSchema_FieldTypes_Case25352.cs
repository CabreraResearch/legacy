using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27479
    /// </summary>
    public class CswUpdateSchema_FieldTypes_Case25352 : CswUpdateSchemaTo
    {

        public override void update()
        {
            CswNbtMetaDataNodeType FieldTypeNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Csw Dev FieldType Test" );
            if( null == FieldTypeNt )
            {
                FieldTypeNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass.ToString(), "Csw Dev FieldType Test", "Csw Dev" );
                _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.Dev, FieldTypeNt.NodeTypeId );

                CswNbtMetaDataNodeTypeTab SimpleTab = FieldTypeNt.getNodeTypeTab( "Csw Dev FieldType Test" );
                if( null != SimpleTab )
                {
                    SimpleTab.TabName = "Simple";
                }
                else
                {
                    SimpleTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( FieldTypeNt, "Simple", 1 );
                }
                CswNbtMetaDataNodeTypeTab LessSimpleTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( FieldTypeNt, "Less Simple", 2 );
                CswNbtMetaDataNodeTypeTab ComplexTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( FieldTypeNt, "Complex", 3 );

                foreach( CswNbtMetaDataFieldType CswNbtMetaDataFieldType in _CswNbtSchemaModTrnsctn.MetaData.getFieldTypes() )
                {
                    switch( CswNbtMetaDataFieldType.FieldType )
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
                            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FieldTypeNt, CswNbtMetaDataFieldType.FieldType, CswNbtMetaDataFieldType.FieldType, SimpleTab.TabId );
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
                            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FieldTypeNt, CswNbtMetaDataFieldType.FieldType, CswNbtMetaDataFieldType.FieldType, LessSimpleTab.TabId );
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
                            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( FieldTypeNt, CswNbtMetaDataFieldType.FieldType, CswNbtMetaDataFieldType.FieldType, ComplexTab.TabId );
                            break;
                    }
                }

                CswNbtView FieldTypeView = _CswNbtSchemaModTrnsctn.makeNewView( "Field Types", NbtViewVisibility.User, null, _CswNbtSchemaModTrnsctn.Nodes.makeUserNodeFromUsername( CswNbtObjClassUser.ChemSWAdminUsername ).NodeId );
                FieldTypeView.AddViewRelationship( FieldTypeNt, false );
                FieldTypeView.Category = "Csw Dev";
                FieldTypeView.save();

                CswNbtNode Node1 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtNode Node2 = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( FieldTypeNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                Node1.IsDemo = true;
                Node1.postChanges( ForceUpdate: false );
                Node2.IsDemo = true;
                Node2.postChanges( ForceUpdate: false );

            }
            // This is a placeholder script that does nothing.
        }//Update()

    }//class CswUpdateSchemaCase_GridsAndButtons_27479

}//namespace ChemSW.Nbt.Schema