using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30902: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30902; }
        }

        public override string ScriptName
        {
            get { return "02G_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Fix Existing Inspection Schedules Parent View Prop"; }
        }

        public override void update()
        {
            const string ViewName = "PI Schedule ParentView";

            CswNbtMetaDataNodeType InspectionGeneratorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Inspection Schedule" );
            if( null != InspectionGeneratorNT )
            {
                CswNbtMetaDataNodeTypeProp ParentViewNTP = InspectionGeneratorNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.ParentView );
                CswNbtView ExistingView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( ParentViewNTP.DefaultValue.AsViewReference.ViewId );
                if( null != ExistingView && ExistingView.ViewName == ViewName )
                {
                    foreach( CswNbtObjClassGenerator InspectionSchedNode in InspectionGeneratorNT.getNodes( false, true, false, true ) )
                    {
                        CswNbtView ParentView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( InspectionSchedNode.ParentView.ViewId );
                        if( ParentView.IsEmpty() )
                        {
                            ParentView.CopyFromView( ExistingView );
                            ParentView.save();
                            InspectionSchedNode.postChanges( false );
                        }

                    }
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Default Value for Inspection Schedule Parent View is invalid", "InspectionSchedule.ParentView NTP default value got an unexpected view" );
                }
            }
        } // update()

    }

}//namespace ChemSW.Nbt.Schema