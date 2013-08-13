using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02E_Case30445 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30445; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            CswNbtObjClassWorkUnit DefaultWorkUnit = null;
            CswNbtObjClassWorkUnit FakeDefaultWorkUnit = null;
            foreach( CswNbtObjClassWorkUnit WorkUnitNode in WorkUnitOC.getNodes( false, false, IncludeHiddenNodes: true ) )
            {
                if( WorkUnitNode.Name.Text.Contains( "Default Work Unit" ) )
                {
                    WorkUnitNode.Name.Text = "Default Work Unit";
                    WorkUnitNode.IsDemo = false;
                    WorkUnitNode.postChanges( false );
                    if( null == DefaultWorkUnit )
                    {
                        DefaultWorkUnit = WorkUnitNode;
                    }
                    else
                    {
                        FakeDefaultWorkUnit = WorkUnitNode;
                    }
                }
            }
            if( null == DefaultWorkUnit )
            {
                CswNbtMetaDataNodeType WorkUnitNT = WorkUnitOC.FirstNodeType;
                if( null != WorkUnitNT )
                {
                    DefaultWorkUnit = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( WorkUnitNT.NodeTypeId, CswEnumNbtMakeNodeOperation.DoNothing );
                    DefaultWorkUnit.Name.Text = "Default Work Unit";
                    DefaultWorkUnit.postChanges( false );
                }
            }
            if( null != DefaultWorkUnit )
            {
                CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
                foreach( CswNbtObjClassUser UserNode in UserOC.getNodes( false, false, IncludeHiddenNodes: true ) )
                {
                    if( null == UserNode.WorkUnitProperty.RelatedNodeId ||
                        ( null != FakeDefaultWorkUnit && UserNode.WorkUnitProperty.RelatedNodeId == FakeDefaultWorkUnit.NodeId ) )
                    {
                        UserNode.WorkUnitProperty.RelatedNodeId = DefaultWorkUnit.NodeId;
                        UserNode.postChanges( false );
                    }
                }
                if( null != FakeDefaultWorkUnit )
                {
                    FakeDefaultWorkUnit.Node.delete( false, true );
                }
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema