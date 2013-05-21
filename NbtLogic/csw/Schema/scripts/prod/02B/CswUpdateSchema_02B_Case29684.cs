using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29684
    /// </summary>
    public class CswUpdateSchema_02B_Case29684 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29684; }
        }

        private CswNbtMetaDataNodeType AssemblyScheduleNT;
        private CswNbtMetaDataNodeType EquipmentScheduleNT;
        private CswNbtMetaDataNodeType AssemblyTaskNT;
        private CswNbtMetaDataNodeType EquipmentTaskNT;

        public override void update()
        {
            AssemblyScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Schedule" );
            EquipmentScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Schedule" );
            AssemblyTaskNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Assembly Task" );
            EquipmentTaskNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment Task" );

            if( null != AssemblyScheduleNT && null != EquipmentScheduleNT && null != AssemblyTaskNT && null != EquipmentTaskNT )
            {
                _fixBadScheudlesAndTasks();
            }
        } // update()

        private void _fixBadScheudlesAndTasks()
        {
            ICswNbtTree BadSchedulesTree = _CswNbtSchemaModTrnsctn.getTreeFromView( _getBadSchedulesAndTasksView(), false );
            BadSchedulesTree.goToRoot();
            for( int i = 0; i < BadSchedulesTree.getChildNodeCount(); i++ )
            {
                BadSchedulesTree.goToNthChild( i );

                CswNbtObjClassGenerator ScheduleNode = BadSchedulesTree.getNodeForCurrentPosition();
                int TargetTypeId = ScheduleNode.NodeTypeId == AssemblyScheduleNT.NodeTypeId ? AssemblyTaskNT.NodeTypeId : EquipmentTaskNT.NodeTypeId;
                ScheduleNode.TargetType.SelectedNodeTypeIds.Clear();
                ScheduleNode.TargetType.SelectedNodeTypeIds.Add( CswConvert.ToString( TargetTypeId ) );
                ScheduleNode.postChanges( false );

                for( int j = 0; j < BadSchedulesTree.getChildNodeCount(); j++ )
                {
                    BadSchedulesTree.goToNthChild( j );

                    CswNbtNode OldTaskNode = BadSchedulesTree.getNodeForCurrentPosition();
                    CswNbtObjClassTask NewTaskNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( TargetTypeId, CswEnumNbtMakeNodeOperation.WriteNode );
                    NewTaskNode.Node.copyPropertyValues( OldTaskNode );
                    NewTaskNode.Owner.RelatedNodeId = ScheduleNode.Owner.RelatedNodeId;
                    OldTaskNode.delete();
                    NewTaskNode.postChanges( false );

                    BadSchedulesTree.goToParentNode();
                }

                BadSchedulesTree.goToParentNode();
            }
        }

        private CswNbtView _getBadSchedulesAndTasksView()
        {
            CswNbtView BadSchedulesView = _CswNbtSchemaModTrnsctn.makeNewView( "29684 - Bad Schedules", CswEnumNbtViewVisibility.Property );
            BadSchedulesView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
            BadSchedulesView.Width = 100;

            CswNbtMetaDataNodeTypeProp AssemblyTargetTypeNTP = AssemblyScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );
            CswNbtMetaDataNodeTypeProp EquipmentTargetTypeNTP = EquipmentScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );
            CswNbtMetaDataNodeTypeProp AssemblyScheduleNTP = AssemblyTaskNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PropertyName.Generator );
            CswNbtMetaDataNodeTypeProp EquipmentScheduleNTP = EquipmentTaskNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PropertyName.Generator );

            CswNbtViewRelationship AssemblySchedRel = BadSchedulesView.AddViewRelationship( AssemblyScheduleNT, true );
            CswNbtViewRelationship EquipmentSchedRel = BadSchedulesView.AddViewRelationship( EquipmentScheduleNT, true );
            BadSchedulesView.AddViewRelationship( EquipmentSchedRel, CswEnumNbtViewPropOwnerType.Second, AssemblyScheduleNTP, true );
            BadSchedulesView.AddViewRelationship( AssemblySchedRel, CswEnumNbtViewPropOwnerType.Second, EquipmentScheduleNTP, true );            
            
            CswNbtViewProperty AssemblyTargetTypeVP = BadSchedulesView.AddViewProperty( AssemblySchedRel, AssemblyTargetTypeNTP );
            BadSchedulesView.AddViewPropertyFilter( AssemblyTargetTypeVP,
                CswEnumNbtFilterConjunction.And,
                CswEnumNbtFilterResultMode.Unknown,
                CswEnumNbtSubFieldName.NodeType,
                CswEnumNbtFilterMode.Contains,
                "Equipment" );
            
            CswNbtViewProperty EquipmentTargetTypeVP = BadSchedulesView.AddViewProperty( EquipmentSchedRel, EquipmentTargetTypeNTP );
            BadSchedulesView.AddViewPropertyFilter( EquipmentTargetTypeVP,
                CswEnumNbtFilterConjunction.And,
                CswEnumNbtFilterResultMode.Unknown,
                CswEnumNbtSubFieldName.NodeType,
                CswEnumNbtFilterMode.Contains,
                "Assembly" );          

            return BadSchedulesView;
        }

    }//class CswUpdateSchema_02B_Case29684

}//namespace ChemSW.Nbt.Schema