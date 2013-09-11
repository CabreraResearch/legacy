using System.Linq;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case29438 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29438; }
        }

        public override string ScriptName
        {
            get { return "02F_Case29438"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TaskClass );
            CswNbtMetaDataObjectClassProp DueDataOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.PropertyName.DueDate );
            CswNbtMetaDataObjectClassProp DoneOnOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.PropertyName.DoneOn );
            CswNbtMetaDataObjectClassProp CompletedOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.PropertyName.Completed );
            CswNbtMetaDataObjectClassProp SummaryOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.PropertyName.Summary );
            CswNbtMetaDataObjectClassProp OwnerOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.PropertyName.Owner ); //Equipment or Assembly for NTs
            CswNbtMetaDataObjectClassProp TechOCP = TaskOC.getObjectClassProp( CswNbtObjClassTask.PropertyName.Technician );
            //Location could not be upgraded to an OCP (See BV) so we just get the first Location NTP
            CswNbtMetaDataNodeTypeProp LocationNTP = null;
            CswNbtMetaDataNodeType FirstTaskNT = TaskOC.getNodeTypes().FirstOrDefault();
            if( null != FirstTaskNT )
            {
                LocationNTP = FirstTaskNT.getNodeTypeProp( "Location" );
            }

            CswNbtView CompletedTasks = _CswNbtSchemaModTrnsctn.makeSafeView( "Tasks: Completed", CswEnumNbtViewVisibility.Global );
            CompletedTasks.SetViewMode( CswEnumNbtViewRenderingMode.Grid );
            CompletedTasks.Category = "Tasks";
            CswNbtViewRelationship parent = CompletedTasks.AddViewRelationship( TaskOC, true );

            CompletedTasks.AddViewProperty( parent, DueDataOCP, 1 );
            CompletedTasks.AddViewProperty( parent, DoneOnOCP, 2 );

            CswNbtViewProperty completedVP = CompletedTasks.AddViewProperty( parent, CompletedOCP );
            CompletedTasks.AddViewPropertyFilter( completedVP, Value: true.ToString() );
            completedVP.ShowInGrid = false;

            CompletedTasks.AddViewProperty( parent, SummaryOCP, 3 );

            CompletedTasks.AddViewProperty( parent, OwnerOCP, 4 );

            if( null != LocationNTP )
            {
                CompletedTasks.AddViewProperty( parent, LocationNTP, 5 );
            }

            CompletedTasks.AddViewProperty( parent, TechOCP, 6 );

            CompletedTasks.save();


        } // update()

    }

}//namespace ChemSW.Nbt.Schema