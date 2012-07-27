using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27045
    /// </summary>
    public class CswUpdateSchemaCase27045 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );

            // Fix the relationship view not to be "IMCS Reports" by reseting it to the default view of the target, "All ReportClass"
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp MRReportNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.ReportPropertyName );
                _setDefaultView( MRReportNTP, MRReportNTP.FKType, MRReportNTP.FKValue, false );
            }

            // Fix the two demo mail reports
            foreach( CswNbtObjClassMailReport MRNode in MailReportOC.getNodes( false, true ) )
            {
                if( null != MRNode && MRNode.IsDemo )
                {
                    if( MRNode.Node.NodeName == "Equipment Mail Report" )
                    {
                        // Equipment Mail Report - If no report or view is assigned, remove it
                        if( ( null == MRNode.Report.RelatedNodeId || Int32.MinValue == MRNode.Report.RelatedNodeId.PrimaryKey ) &&
                            ( MRNode.ReportView.SelectedViewIds.IsEmpty || "196" == MRNode.ReportView.SelectedViewIds.ToString() ) )
                        {
                            MRNode.Node.delete();
                        }
                    }
                    else
                    {
                        // Mail Report 25751 - assign "Lab 1 Deficiencies" to mail report
                        CswNbtObjClassReport Lab1Report = null;
                        foreach( CswNbtObjClassReport ReportNode in ReportOC.getNodes( false, true ) )
                        {
                            if( "Lab 1 Deficiencies" == ReportNode.ReportName.Text )
                            {
                                Lab1Report = ReportNode;
                            }
                        }
                        if( null != Lab1Report )
                        {
                            CswNbtMetaDataNodeTypeProp NameNTP = MRNode.NodeType.getNodeTypeProp("Name");
                            if(null != NameNTP)
                            {
                                MRNode.Node.Properties[NameNTP].AsText.Text = "Lab 1 Deficiencies Mail Report";
                            }
                            MRNode.Report.RelatedNodeId = Lab1Report.NodeId;
                            MRNode.postChanges( false );
                        }
                        else
                        {
                            MRNode.Node.delete();
                        }
                    }
                }
            }
        }//Update()



        // stolen and adapted from CswNbtFieldTypeRuleRelationship
        private CswNbtView _setDefaultView( CswNbtMetaDataNodeTypeProp MetaDataProp, NbtViewRelatedIdType RelatedIdType, Int32 inFKValue, bool OnlyCreateIfNull )
        {
            //CswNbtMetaDataNodeTypeProp ThisNtProp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( MetaDataProp.PropId );
            CswNbtView RetView = _CswNbtSchemaModTrnsctn.restoreView( MetaDataProp.ViewId );
            if( RelatedIdType != NbtViewRelatedIdType.Unknown &&
                ( null == RetView ||
                  RetView.Root.ChildRelationships.Count == 0 ||
                  false == OnlyCreateIfNull ) )
            {

                if( null != RetView )
                {
                    RetView.Root.ChildRelationships.Clear();
                }

                if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass TargetOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( inFKValue );
                    if( null != TargetOc )
                    {
                        RetView = TargetOc.CreateDefaultView();
                    }
                }
                else if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType TargetNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( inFKValue );
                    if( null != TargetNt )
                    {
                        RetView = TargetNt.CreateDefaultView();
                    }
                }

                RetView.ViewId = MetaDataProp.ViewId;
                RetView.Visibility = NbtViewVisibility.Property;
                RetView.ViewMode = NbtViewRenderingMode.List;
                RetView.ViewName = MetaDataProp.PropName;
                RetView.save();
            }
            return RetView;
        }
    }//class CswUpdateSchemaCase27045

}//namespace ChemSW.Nbt.Schema