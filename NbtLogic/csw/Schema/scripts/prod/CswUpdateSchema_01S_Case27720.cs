using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27720
    /// </summary>
    public class CswUpdateSchema_01S_Case27720 : CswUpdateSchemaTo
    {
        public override void update()
        {

            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )
            {
                // remove 'Nodes to Report' from all layouts
                CswNbtMetaDataNodeTypeProp NodesToReportNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.NodesToReport );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( NodesToReportNTP );

                // remove Run Status from Add layout
                CswNbtMetaDataNodeTypeProp RunStatusNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.RunStatus );
                RunStatusNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );

                // set Target Type and Event to be conditional on Type = View
                CswNbtMetaDataNodeTypeProp TypeNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Type );
                CswNbtMetaDataNodeTypeProp TargetTypeNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.TargetType );
                CswNbtMetaDataNodeTypeProp EventNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Event );
                TargetTypeNTP.setFilter( TypeNTP, TypeNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassMailReport.TypeOptionView );
                EventNTP.setFilter( TypeNTP, TypeNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassMailReport.TypeOptionView );

                // add help text to Report View
                CswNbtMetaDataNodeTypeProp ReportViewNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.ReportView );
                ReportViewNTP.HelpText = "For 'Exists' events, a report is generated if the view returns any results that match the Target Type.  For 'Edit' events, a report is only generated if one of the properties in the view was modified.";

                // set target type and event for existing mail reports
                foreach( CswNbtObjClassMailReport MailReportNode in MailReportNT.getNodes( false, true ) )
                {
                    if( CswNbtObjClassMailReport.TypeOptionView == MailReportNode.Type.Value &&
                        0 == MailReportNode.TargetType.SelectedNodeTypeIds.Count )
                    {
                        CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( MailReportNode.ReportView.ViewId );
                        if( View.Root.ChildRelationships.Count > 0 )
                        {
                            CswNbtViewRelationship RootRel = View.Root.ChildRelationships[0];
                            if( RootRel.SecondType == NbtViewRelatedIdType.NodeTypeId )
                            {
                                MailReportNode.TargetType.SelectedNodeTypeIds.Add( RootRel.SecondId.ToString() );
                            }
                            else
                            {
                                CswNbtMetaDataObjectClass RootOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( RootRel.SecondId.ToString() );
                                if( null != RootOC.FirstNodeType )
                                {
                                    MailReportNode.TargetType.SelectedNodeTypeIds.Add( RootOC.FirstNodeType.NodeTypeId.ToString() );
                                }
                            }
                        }
                    }

                    if( MailReportNode.Event.Empty )
                    {
                        MailReportNode.Event.Value = CswNbtObjClassMailReport.EventOption.Exists.ToString();
                    }
                    MailReportNode.postChanges( false );

                } // foreach( CswNbtObjClassMailReport MailReportNode in MailReportNT.getNodes( false, true ) )
            } // foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )

        }//Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 27720; }
        }

    }//class CswUpdateSchemaCase27720

}//namespace ChemSW.Nbt.Schema