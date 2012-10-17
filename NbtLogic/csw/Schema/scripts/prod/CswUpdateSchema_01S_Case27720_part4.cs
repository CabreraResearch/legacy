using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27720_part4
    /// </summary>
    public class CswUpdateSchema_01S_Case27720_part4 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataNodeType MailReportNT = MailReportOC.FirstNodeType;
            CswNbtMetaDataNodeTypeProp MailReportNameNTP = MailReportNT.getNodeTypeProp( "Name" );

            if( null != MailReportNT )
            {
                if( _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.SI ) )
                {
                    // Notification for Inspection status = Action Required

                    CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                    foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.getNodeTypes() )
                    {
                        CswNbtMetaDataNodeTypeProp InspectionStatusNTP = InspectionNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status );

                        // Make new mail report for old notification
                        CswNbtObjClassMailReport MailReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                        if( null != MailReportNameNTP )
                        {
                            MailReportNode.Node.Properties[MailReportNameNTP].AsText.Text = InspectionNT.NodeTypeName + " Action Required Notification";
                        }
                        MailReportNode.OutputFormat.Value = "link";

                        MailReportNode.Type.Value = CswNbtObjClassMailReport.TypeOptionView;
                        MailReportNode.Event.Value = CswNbtObjClassMailReport.EventOption.Edit.ToString();
                        MailReportNode.TargetType.SelectedNodeTypeIds.Add( InspectionNT.NodeTypeId.ToString() );
                        MailReportNode.Message.Text = "The following inspections have been marked Action Required:";
                        MailReportNode.Enabled.Checked = Tristate.True;
                        MailReportNode.IsDemo = true;

                        CswRateInterval HourlyRate = _CswNbtSchemaModTrnsctn.makeRateInterval();
                        HourlyRate.setHourly( 2, DateTime.Now );
                        MailReportNode.DueDateInterval.RateInterval = HourlyRate;

                        if( "scu" == _CswNbtSchemaModTrnsctn.Accessid )
                        {
                            // Subscribe existing user (id: 24917)
                            MailReportNode.Recipients.AddUser( new CswPrimaryKey( "nodes", 24917 ) );
                            MailReportNode.Enabled.Checked = Tristate.True;
                        }
                        else
                        {
                            MailReportNode.Enabled.Checked = Tristate.False;
                        }

                        MailReportNode.postChanges( true );

                        CswNbtView ReportView = _CswNbtSchemaModTrnsctn.restoreView( MailReportNode.ReportView.ViewId );
                        ReportView.Root.ChildRelationships.Clear();
                        CswNbtViewRelationship InspRel = ReportView.AddViewRelationship( InspectionNT, false );
                        ReportView.AddViewPropertyAndFilter( InspRel, InspectionStatusNTP, CswNbtObjClassInspectionDesign.InspectionStatus.ActionRequired.ToString() );
                        ReportView.save();

                    } // foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.getNodeTypes() )
                } // if(_CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.SI) )


                if( _CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
                {
                    // Notification for new Problems

                    CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
                    foreach( CswNbtMetaDataNodeType ProblemNT in ProblemOC.getNodeTypes() )
                    {
                        CswNbtMetaDataNodeTypeProp ProblemClosedNTP = ProblemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassProblem.PropertyName.Closed );

                        // Make new mail report for old notification
                        CswNbtObjClassMailReport MailReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                        if( null != MailReportNameNTP )
                        {
                            MailReportNode.Node.Properties[MailReportNameNTP].AsText.Text = "New " + ProblemNT.NodeTypeName + " Notification";
                        }
                        MailReportNode.OutputFormat.Value = "link";

                        MailReportNode.Type.Value = CswNbtObjClassMailReport.TypeOptionView;
                        MailReportNode.Event.Value = CswNbtObjClassMailReport.EventOption.Edit.ToString();
                        MailReportNode.TargetType.SelectedNodeTypeIds.Add( ProblemNT.NodeTypeId.ToString() );
                        MailReportNode.Message.Text = "The following problems have been opened:";
                        MailReportNode.IsDemo = true;

                        CswRateInterval HourlyRate = _CswNbtSchemaModTrnsctn.makeRateInterval();
                        HourlyRate.setHourly( 2, DateTime.Now );
                        MailReportNode.DueDateInterval.RateInterval = HourlyRate;

                        if( "cabot" == _CswNbtSchemaModTrnsctn.Accessid )
                        {
                            // Subscribe existing user (id: 24786)
                            MailReportNode.Recipients.AddUser( new CswPrimaryKey( "nodes", 24786 ) );
                            MailReportNode.Enabled.Checked = Tristate.True;
                        }
                        else
                        {
                            MailReportNode.Enabled.Checked = Tristate.False;
                        }
                        MailReportNode.postChanges( true );

                        CswNbtView ReportView = _CswNbtSchemaModTrnsctn.restoreView( MailReportNode.ReportView.ViewId );
                        ReportView.Root.ChildRelationships.Clear();
                        CswNbtViewRelationship ProbRel = ReportView.AddViewRelationship( ProblemNT, false );
                        ReportView.AddViewPropertyAndFilter( ProbRel, ProblemClosedNTP, Tristate.False.ToString() );
                        ReportView.save();

                    } // foreach( CswNbtMetaDataNodeType ProblemNT in ProblemOC.getNodeTypes() )
                } // else if(_CswNbtSchemaModTrnsctn.Modules.IsModuleEnabled( CswNbtModuleName.IMCS) )
            } // if( null != MailReportNT )
        }//Update()

    }//class CswUpdateSchema_01S_Case27720_part4

}//namespace ChemSW.Nbt.Schema