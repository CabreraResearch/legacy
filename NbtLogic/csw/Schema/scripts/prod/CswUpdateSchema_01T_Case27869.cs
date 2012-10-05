using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27869
    /// </summary>
    public class CswUpdateSchema_01T_Case27869 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MethodOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MethodClass );
            if( null != MethodOc )
            {
                CswNbtMetaDataNodeType MethodNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Method" );
                if( null == MethodNt )
                {
                    //Create new NodeType
                    MethodNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( MethodOc.ObjectClassId, "Method", "MLM" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, MethodNt.NodeTypeId );
                    MethodNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMethod.PropertyName.MethodNo ) );

                    //Create Demo Data
                    CswNbtObjClassMethod MethodNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MethodNt.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    MethodNode.MethodNo.Text = "000001";
                    MethodNode.MethodDescription.Text = "Demo Method";
                    MethodNode.IsDemo = true;
                    MethodNode.postChanges( false );
                }

                CswNbtView MethodView = _CswNbtSchemaModTrnsctn.restoreView( "Methods" );
                if( null == MethodView )
                {
                    //Create new View
                    MethodView = _CswNbtSchemaModTrnsctn.makeNewView( "Methods", NbtViewVisibility.Global );
                    MethodView.Category = "MLM (demo)";
                    MethodView.IsDemo = true;
                    MethodView.ViewMode = NbtViewRenderingMode.Tree;
                    MethodView.AddViewRelationship( MethodOc, true );
                    MethodView.save();
                }
            }
        } //Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27869; }
        }

    }//class CswUpdateSchema_01T_Case27869

}//namespace ChemSW.Nbt.Schema