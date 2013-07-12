using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30022
    /// </summary>
    public class CswUpdateSchema_02C_Case30022 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30022; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GeneratorClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GeneratorClass );
            foreach( CswNbtObjClassGenerator GeneratorNode in GeneratorClass.getNodes( false, false ) )
            {
                if( GeneratorNode.Enabled.Checked == CswEnumTristate.True )
                {
                    if( GeneratorNode.NextDueDate.DateTimeValue != DateTime.MinValue )
                    {
                        CswNbtView GeneratorTasksView = GeneratorNode.GetGeneratedTargetsView( DateTime.MinValue );
                        ICswNbtTree TasksTree = _CswNbtSchemaModTrnsctn.getTreeFromView( GeneratorTasksView, false );
                        int NumTasks = TasksTree.getChildNodeCount();
                        if( NumTasks > 0 )
                        {
                            DateTime LastTaskDueDate = DateTime.MinValue;
                            for( int i = 0; i < NumTasks; i++ )
                            {
                                TasksTree.goToNthChild(i);
                                CswNbtObjClassTask TaskNode = TasksTree.getNodeForCurrentPosition();
                                if( TaskNode.DueDate.DateTimeValue > LastTaskDueDate )
                                {
                                    LastTaskDueDate = TaskNode.DueDate.DateTimeValue;
                                }
                                TasksTree.goToParentNode();
                            }
                            DateTime ExpectedNextDueDate = GeneratorNode.DueDateInterval.getNextOccuranceAfter( LastTaskDueDate );
                            if( ExpectedNextDueDate != GeneratorNode.NextDueDate.DateTimeValue )
                            {
                                GeneratorNode.NextDueDate.DateTimeValue = ExpectedNextDueDate;
                                GeneratorNode.postChanges( true );
                            }
                        }
                        else
                        {
                            GeneratorNode.NextDueDate.DateTimeValue = DateTime.MinValue;
                            GeneratorNode.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                            GeneratorNode.postChanges( true );
                        }
                    }
                    else
                    {
                        GeneratorNode.updateNextDueDate( ForceUpdate: true, DeleteFutureNodes: false );
                        GeneratorNode.postChanges( true );
                    }
                }
            }
        } // update()

    }//class CswUpdateSchema_02C_Case30022

}//namespace ChemSW.Nbt.Schema