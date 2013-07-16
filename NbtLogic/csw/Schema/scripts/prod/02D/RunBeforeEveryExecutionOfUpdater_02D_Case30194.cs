using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for OC changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30194 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: Case 30194";

        #region Blame Logic

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30194; }
        }

        #endregion Blame Logic

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass WorkUnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.WorkUnitClass );
            _CswNbtSchemaModTrnsctn.deleteModuleObjectClassJunction( CswEnumNbtModuleName.Containers, WorkUnitOC.ObjectClassId );

            CswNbtObjClassWorkUnit DefaultWorkUnit = null;
            foreach( CswNbtObjClassWorkUnit WorkUnitNode in WorkUnitOC.getNodes( false, false, IncludeHiddenNodes: true ) )
            {
                if( WorkUnitNode.Name.Text == "Default Work Unit" )
                {
                    WorkUnitNode.IsDemo = false;
                    WorkUnitNode.postChanges( false );
                    DefaultWorkUnit = WorkUnitNode;
                }
            }
            if( null == DefaultWorkUnit )
            {
                CswNbtMetaDataNodeType WorkUnitNT = WorkUnitOC.FirstNodeType;
                if( null != WorkUnitNT )
                {
                    DefaultWorkUnit = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( WorkUnitNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            ( (CswNbtObjClassWorkUnit) NewNode ).Name.Text = "Default Work Unit";
                        } );
                }
            }

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtObjClassUser UserNode in UserOC.getNodes( false, false, IncludeHiddenNodes: true ) )
            {
                if( null == UserNode.WorkUnitProperty.RelatedNodeId && null != DefaultWorkUnit )
                {
                    UserNode.WorkUnitProperty.RelatedNodeId = DefaultWorkUnit.NodeId;
                    UserNode.postChanges( false );
                }
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_02D_Case30194
}//namespace ChemSW.Nbt.Schema


