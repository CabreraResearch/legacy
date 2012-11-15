using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28069
    /// </summary>
    public class CswUpdateSchema_01U_Case28069 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.TaskClass );
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );

            // Set layout to be next to Due Date
            foreach( CswNbtMetaDataNodeType TaskNT in TaskOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp TaskCreatedDateNTP = TaskNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PropertyName.CreatedDate );
                CswNbtMetaDataNodeTypeProp TaskDueDateNTP = TaskNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PropertyName.DueDate );
                TaskCreatedDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, TaskDueDateNTP, true );
            }
            foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp InspCreatedDateNTP = InspectionNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PropertyName.CreatedDate );
                CswNbtMetaDataNodeTypeProp InspDueDateNTP = InspectionNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTask.PropertyName.DueDate );
                InspCreatedDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, InspDueDateNTP, true );
            }

            // Set creation date for all existing tasks and inspection designs to be 1/1/2000
            foreach( CswNbtPropertySetGeneratorTarget TaskNode in TaskOC.getNodes( false, true, false, true ) )
            {
                if( DateTime.MinValue == TaskNode.CreatedDate.DateTimeValue )
                {
                    TaskNode.CreatedDate.DateTimeValue = new DateTime( 2000, 1, 1 );
                    TaskNode.postChanges( false );
                }
            }

            foreach( CswNbtPropertySetGeneratorTarget InspNode in InspectionOC.getNodes( false, true, false, true ) )
            {
                if( DateTime.MinValue == InspNode.CreatedDate.DateTimeValue )
                {
                    InspNode.CreatedDate.DateTimeValue = new DateTime( 2000, 1, 1 );
                    InspNode.postChanges( false );
                }
            }
        } // Update()

    }//class CswUpdateSchema_01U_Case28069

}//namespace ChemSW.Nbt.Schema