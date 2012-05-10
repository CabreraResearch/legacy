using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25981
    /// </summary>
    public class CswUpdateSchemaCase25981 : CswUpdateSchemaTo
    {
        public override void update()
        {
            //remove the obsolete SI_Example stuff
            //delete the old views
            _CswNbtSchemaModTrnsctn.deleteView( "Groups, SI_protocol: SI_target", true );
            _CswNbtSchemaModTrnsctn.deleteView( "Inspections, SI_protocol: SI_target", true );
            _CswNbtSchemaModTrnsctn.deleteView( "Scheduling, SI_protocol: SI_target", true );

            //delete the old nodetypes (and nodes by auto-cascade)           
            MetaData.CswNbtMetaDataNodeType nt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_protocol" );
            if( null != nt )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( nt );
            }
            nt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_target" );
            if( null != nt )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( nt );
            }
            nt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SI_target Group" );
            if( null != nt )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( nt );
            }

            //Lab Safety (**new and improved**)
            //create inspection wizard for Lab Safety (example)
            CswNbtActInspectionDesignWiz wiz = _CswNbtSchemaModTrnsctn.getCswNbtActInspectionDesignWiz();
            DataTable qTable = new DataTable();
            qTable.Columns.Add( "SECTION", typeof( string ) );
            qTable.Columns.Add( "QUESTION", typeof( string ) );
            qTable.Columns.Add( "ALLOWED_ANSWERS", typeof( string ) );
            qTable.Columns.Add( "COMPLIANT_ANSWERS", typeof( string ) );
            qTable.Columns.Add( "HELP_TEXT", typeof( string ) );

            // Section1: Q1.1 
            DataRow dr1 = qTable.NewRow();
            dr1["SECTION"] = "Safety";
            dr1["QUESTION"] = "Is door placarded properly?";
            dr1["ALLOWED_ANSWERS"] = "Yes,No";
            dr1["COMPLIANT_ANSWERS"] = "Yes";
            dr1["HELP_TEXT"] = "There should be proper safety placards for PROP65 and Radioactivity as necessary.";
            qTable.Rows.Add( dr1 );

            // Section1: Q1.2 
            DataRow dr2 = qTable.NewRow();
            dr2["SECTION"] = "Safety";
            dr2["QUESTION"] = "All chemical containers labeled?";
            dr2["ALLOWED_ANSWERS"] = "Yes,No,n/a";
            dr2["COMPLIANT_ANSWERS"] = "Yes";
            dr2["HELP_TEXT"] = "All containers must have an identity label and safety info.";
            qTable.Rows.Add( dr2 );

            // Section1: Q1.3
            DataRow dr3 = qTable.NewRow();
            dr3["SECTION"] = "Safety";
            dr3["QUESTION"] = "First aid kit in good order?";
            dr3["ALLOWED_ANSWERS"] = "Yes,No";
            dr3["COMPLIANT_ANSWERS"] = "Yes";
            dr3["HELP_TEXT"] = "Check that the kit is properly stocked.";
            qTable.Rows.Add( dr3 );

            string newCat = "Lab Safety";
            JObject newObj = wiz.createInspectionDesignTabsAndProps( qTable, newCat + " Checklist", newCat, newCat );
            CswNbtMetaDataNodeType schedNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );



            //create some nodes...

            //locations
            Int32 bldgNTID = Int32.MinValue;
            CswPrimaryKey locNRL = null;
            CswPrimaryKey locLab1 = null;
            foreach( CswNbtNode anode in _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass ).getNodes( false, false ) )
            {
                if( anode.NodeName == "North Research Lab" )
                {
                    locNRL = anode.NodeId;
                    bldgNTID = anode.NodeTypeId;
                }
                if( anode.NodeName == "Lab 1" )
                {
                    locLab1 = anode.NodeId;
                }
            }
            //Lab 2 in North Research Lab
            CswPrimaryKey locLab2 = null;
            CswNbtMetaDataNodeType roomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
            if( locNRL != null )
            {
                CswNbtNode lab2Node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( roomNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassLocation lab2NodeAsLocation = CswNbtNodeCaster.AsLocation( lab2Node );
                lab2NodeAsLocation.Location.NodeId = locNRL;
                lab2Node.IsDemo = true;
                lab2NodeAsLocation.postChanges( true );
                locLab2 = lab2Node.NodeId;
            }

            //ExampleGroup
            CswNbtNode groupNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.GroupNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassInspectionTargetGroup groupNodeAsGroup = CswNbtNodeCaster.AsInspectionTargetGroup( groupNode );
            groupNodeAsGroup.Name.Text = "Lab Safety Group1";
            groupNode.IsDemo = true;
            groupNodeAsGroup.postChanges( true );

            if( null != schedNT )
            {
                //ExampleMonthlyInspectionSchedule (disabled)
                CswNbtNode schedNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( schedNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                CswNbtObjClassGenerator schedNodeAsGenerator = CswNbtNodeCaster.AsGenerator( schedNode );
                schedNodeAsGenerator.Owner.RelatedNodeId = groupNode.NodeId;
                schedNodeAsGenerator.TargetType.SelectedNodeTypeIds.Add( wiz.DesignNtId.ToString() );
                schedNodeAsGenerator.ParentType.SelectedNodeTypeIds.Add( wiz.TargetNtId.ToString() );
                schedNodeAsGenerator.Summary.Text = "Monthly Lab Safety Schedule";
                schedNode.IsDemo = true;
                schedNodeAsGenerator.postChanges( true );
            }

            // Lab1:IP1
            CswNbtNode itemNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.TargetNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassInspectionTarget itemNodeAsTarget = CswNbtNodeCaster.AsInspectionTarget( itemNode );
            itemNodeAsTarget.Description.Text = "IP1";
            if( null != locLab1 ) itemNodeAsTarget.Location.SelectedNodeId = locLab1;
            itemNodeAsTarget.InspectionTargetGroup.RelatedNodeId = groupNode.NodeId;
            itemNode.IsDemo = true;
            itemNodeAsTarget.postChanges( true );
            // Lab2:IP2
            CswNbtNode item2Node = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.TargetNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassInspectionTarget item2NodeAsTarget = CswNbtNodeCaster.AsInspectionTarget( item2Node );
            item2NodeAsTarget.Description.Text = "IP2";
            item2Node.IsDemo = true;
            if( null != locLab2 ) item2NodeAsTarget.Location.SelectedNodeId = locLab2;
            item2NodeAsTarget.InspectionTargetGroup.RelatedNodeId = groupNode.NodeId;
            item2NodeAsTarget.postChanges( true );

            //NO route yet (save for future release)

            //ExampleInspection (manual)
            CswNbtNode inspectNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( wiz.DesignNtId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            //set target
            CswNbtObjClassInspectionDesign inspectNodeAsDesign = CswNbtNodeCaster.AsInspectionDesign( inspectNode );
            inspectNodeAsDesign.Owner.RelatedNodeId = itemNode.NodeId;
            inspectNode.IsDemo = true;
            inspectNodeAsDesign.Date.DateTimeValue = new DateTime( 2022, 1, 1 );
            inspectNodeAsDesign.postChanges( true );


            //report and mailer
            CswNbtMetaDataNodeType rptNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );
            CswNbtNode rptNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( rptNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
            CswNbtObjClassReport rptNodeAsReport = CswNbtNodeCaster.AsReport( rptNode );
            Int32 locPropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( wiz.DesignNtId, "Location" );
            Int32 inspnamePropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( wiz.DesignNtId, "Name" );
            Int32 inspdatePropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( wiz.DesignNtId, "Inspection Date" );
            Int32 targPropId = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropId( wiz.DesignNtId, "Target" );



            rptNodeAsReport.Category.Text = "Lab Safety";
            rptNodeAsReport.ReportName.Text = "Lab 1 Deficiencies";
            rptNodeAsReport.SQL.Text = @"select des.P" + inspdatePropId.ToString() + @" as InspectionDate, 
                                      des.P" + inspnamePropId.ToString() + @" as InspectionName,
                                      des.P" + locPropId.ToString() + @" as Location,
                                      CASE nvl(q.correctiveaction,'NULL')
                                       WHEN 'NULL' then 'NO'
                                       ELSE 'yes'
                                      END as Resolved,
                                     q.questionno,
                                      q.question,
                                      q.answer,
                                      q.correctiveaction,
                                      q.comments
                                      from ntlabsafetychecklist des
                                      join ntlabsafety targ on targ.nodeid=des.P" + targPropId.ToString() + @"_labsafety_ntfk
                                    join vwquestiondetail q on q.nodeid = des.nodeid
                                          where (q.iscompliant = '0' or q.correctiveaction is not null)
                                           and des.P" + locPropId.ToString() + @" like '%> Lab 1'
                                      order by des.P" + locPropId.ToString() + @", des.P" + inspdatePropId.ToString() + @", q.questionno";
            rptNode.IsDemo = true;
            rptNodeAsReport.postChanges( true );

            CswNbtMetaDataNodeType mailNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Mail Report" );
            CswNbtNode mailNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( mailNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );

            CswNbtObjClassMailReport mailNodeAsMailReport = CswNbtNodeCaster.AsMailReport( mailNode );
            mailNodeAsMailReport.Type.Value = CswNbtObjClassMailReport.TypeOptionReport;
            mailNodeAsMailReport.Enabled.Checked = Tristate.True;
            mailNodeAsMailReport.DueDateInterval.RateInterval.setMonthlyByDate( 1, 1, 1, 2020 );
            mailNodeAsMailReport.Report.RelatedNodeId = rptNode.NodeId;
            mailNode.IsDemo = true;
            mailNodeAsMailReport.postChanges( true );

            //create views
            //name=Lab Safety By Location; cat=Lab Safety; mode=tree, global; struct=Building>Room>Lab Safety
            CswNbtMetaDataNodeType bldgNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( bldgNTID );
            CswNbtMetaDataNodeType targNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( wiz.TargetNtId );

            CswNbtView LocView = _CswNbtSchemaModTrnsctn.makeView();
            LocView.makeNew( "Lab Safety By Location", NbtViewVisibility.Global );
            LocView.ViewMode = NbtViewRenderingMode.Tree;
            CswNbtViewRelationship bldgRelationship = LocView.AddViewRelationship( bldgNT, true );
            //add room
            CswNbtMetaDataNodeTypeProp roomLocProp = roomNT.getNodeTypePropByObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );
            CswNbtViewRelationship roomRelationship = LocView.AddViewRelationship( bldgRelationship, NbtViewPropOwnerType.Second, roomLocProp, true );
            //add inspection target
            CswNbtMetaDataNodeTypeProp targetLocProp = targNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.LocationPropertyName );
            LocView.AddViewRelationship( roomRelationship, NbtViewPropOwnerType.Second, targetLocProp, true );
            LocView.save();

            //Due Lab Inspections
            CswNbtMetaDataNodeType inspectNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( wiz.DesignNtId );
            CswNbtMetaDataNodeTypeProp dueDateProp = inspectNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.DatePropertyName );
            CswNbtMetaDataNodeTypeProp statusProp = inspectNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
            CswNbtMetaDataNodeTypeProp nameProp = inspectNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            CswNbtMetaDataNodeTypeProp targetProp = inspectNT.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );

            CswNbtView view = _CswNbtSchemaModTrnsctn.makeView();
            view.makeNew( "Due Lab Safety Inspections", NbtViewVisibility.Global );
            view.ForMobile = true;
            view.ViewMode = NbtViewRenderingMode.Grid;
            CswNbtViewRelationship ParentRelationship = view.AddViewRelationship( inspectNT, true );
            view.AddViewProperty( ParentRelationship, dueDateProp );
            CswNbtViewProperty statusviewProp = view.AddViewProperty( ParentRelationship, statusProp );
            view.AddViewProperty( ParentRelationship, nameProp );
            view.AddViewProperty( ParentRelationship, targetProp );

            //copy
            CswNbtView CompletedView = _CswNbtSchemaModTrnsctn.makeView();
            CompletedView.makeNew( "Completed Lab Safety Inspections", NbtViewVisibility.Global, null, null, view );
            CompletedView.save();

            CswNbtView ArView = _CswNbtSchemaModTrnsctn.makeView();
            ArView.makeNew( "Action Required Lab Safety Inspections", NbtViewVisibility.Global, null, null, view );

            ArView.save();


            //filters
            view.AddViewPropertyFilter(
                                             statusviewProp,
                                             CswNbtSubField.SubFieldName.Value,
                                             CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                             "Pending",
                                             false );
            view.save();

            CompletedView.AddViewPropertyFilter(
                CompletedView.findPropertyByName( CswNbtObjClassInspectionDesign.StatusPropertyName ),
                                             CswNbtSubField.SubFieldName.Value,
                                             CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                             "Completed",
                                             false );
            CompletedView.save();

            ArView.AddViewPropertyFilter(
                ArView.findPropertyByName( CswNbtObjClassInspectionDesign.StatusPropertyName ),
                                             CswNbtSubField.SubFieldName.Value,
                                             CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                             "Action Required",
                                             false );
            ArView.save();


            //change permissions on roles
            CswNbtMetaDataNodeType roleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Role" );
            CswNbtMetaDataNodeType groupNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( wiz.GroupNtId );
            CswNbtMetaDataNodeType targetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( wiz.TargetNtId );

            CswNbtNode arole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( null != arole )
            {
                CswNbtObjClassRole roleNode = CswNbtNodeCaster.AsRole( arole );
                bool canDelete = true;
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Deficient_Inspections, roleNode, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Edit_View, roleNode, true );
                _setNtPermitsForRole( roleNode, inspectNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, targetNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, groupNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, schedNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, mailNT, true, true, true, canDelete );
            }

            arole = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Inspector" );
            if( null != arole )
            {
                CswNbtObjClassRole roleNode = CswNbtNodeCaster.AsRole( arole );
                bool canDelete = false;
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Deficient_Inspections, roleNode, true );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Edit_View, roleNode, true );
                _setNtPermitsForRole( roleNode, inspectNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, targetNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, groupNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, schedNT, true, true, true, canDelete );
                _setNtPermitsForRole( roleNode, mailNT, true, true, true, canDelete );

                //welcome items for inspector
                ChemSW.Nbt.Welcome.CswNbtWelcomeTable aWelcomeTbl = _CswNbtSchemaModTrnsctn.getWelcomeTable();
                aWelcomeTbl.DeleteAllWelcomeItemsForRole( roleNode.NodeId.ToString() );

                //Due Inspections view r1c1 calendar.gif
                aWelcomeTbl.AddWelcomeItem( Welcome.CswNbtWelcomeTable.WelcomeComponentType.Link,
                    CswNbtView.ViewType.View, view.ViewId.ToString(), int.MinValue, "Due Inspections", 1, 1,
                    "calendar.gif", arole.NodeId.ToString() );

                //Add Inspections (inspection points by location view) r1c2 feinspection.png
                aWelcomeTbl.AddWelcomeItem( Welcome.CswNbtWelcomeTable.WelcomeComponentType.Link,
                    CswNbtView.ViewType.View, LocView.ViewId.ToString(), int.MinValue, "Add Inspection", 1, 2,
                    "feinspection.png", arole.NodeId.ToString() );

                //Completed Inspection view r1c3 clipboard.gif
                aWelcomeTbl.AddWelcomeItem( Welcome.CswNbtWelcomeTable.WelcomeComponentType.Link,
                    CswNbtView.ViewType.View, CompletedView.ViewId.ToString(), int.MinValue, "Completed Inspections", 1, 3,
                    "clipboard.gif", arole.NodeId.ToString() );

                //Scheduling Inspections view r2c1 (no icon)
                if( null != wiz.SchedulingViewId )
                {
                    aWelcomeTbl.AddWelcomeItem( Welcome.CswNbtWelcomeTable.WelcomeComponentType.Link,
                        CswNbtView.ViewType.View, wiz.SchedulingViewId.ToString(), int.MinValue, "Scheduling Inspections", 2, 1,
                        "blank.gif", arole.NodeId.ToString() );
                }

                //Action Required view r2c2 warning.gif
                aWelcomeTbl.AddWelcomeItem( Welcome.CswNbtWelcomeTable.WelcomeComponentType.Link,
                    CswNbtView.ViewType.View, ArView.ViewId.ToString(), int.MinValue, "Action Required", 2, 2,
                    "warning.gif", arole.NodeId.ToString() );

                //Lab 1 Deficiencies (report, no icon) r2c3
                //REPORT LINKS DON'T WORK on welcome page yet...
            }




        }//Update()

        void _setNtPermitsForRole( CswNbtObjClassRole arole, CswNbtMetaDataNodeType nt, bool canView, bool canCreate,
            bool canEdit, bool canDelete )
        {
            _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.View, nt, arole, canView );
            _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.Create, nt, arole, canCreate );
            _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.Edit, nt, arole, canEdit );
            _CswNbtSchemaModTrnsctn.Permit.set( Security.CswNbtPermit.NodeTypePermission.Delete, nt, arole, canDelete );
        }

    }//class CswUpdateSchemaCase25981

}//namespace ChemSW.Nbt.Schema