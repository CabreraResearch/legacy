using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-07
    /// </summary>
    public class CswUpdateSchemaTo01H07 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 07 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H07( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

        public void update()
        {
            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp EquipmentStatusOCP = EquipmentOC.getObjectClassProp( CswNbtObjClassEquipment.StatusPropertyName );

            // BZ 10454
            // Add retired filter to existing views
            DataTable ViewsTable = _CswNbtSchemaModTrnsctn.getAllViews();
            foreach( DataRow ViewRow in ViewsTable.Rows )
            {
                CswNbtView ThisView = _CswNbtSchemaModTrnsctn.restoreView( CswConvert.ToInt32( ViewRow["nodeviewid"] ) );
                _SetDefaultEquipmentFilter( ThisView, ThisView.Root );
            }

            // Make view of retired equipment
            CswNbtView RetiredEquipView = _CswNbtSchemaModTrnsctn.makeView();
            RetiredEquipView.makeNew( "Retired Equipment", NbtViewVisibility.Global, null, null, null );
            RetiredEquipView.ViewMode = NbtViewRenderingMode.List;
            RetiredEquipView.Category = "Equipment";
            CswNbtViewRelationship EquipmentRel = RetiredEquipView.AddViewRelationship( EquipmentOC, false );   // false is important here
            CswNbtViewProperty StatusViewProp = RetiredEquipView.AddViewProperty( EquipmentRel, EquipmentStatusOCP );
            CswNbtViewPropertyFilter StatusRetiredFilter = RetiredEquipView.AddViewPropertyFilter( StatusViewProp,
                                                                    EquipmentStatusOCP.FieldTypeRule.SubFields.Default.Name,
                                                                    CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                                    CswNbtObjClassEquipment.StatusOptionToDisplayString( CswNbtObjClassEquipment.StatusOption.Retired ),
                                                                    false );
            RetiredEquipView.save();



            // BZ 10425
            CswNbtMetaDataNodeType DefaultMailReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Mail Report" );
            if( DefaultMailReportNT != null )
            {
                CswNbtMetaDataNodeTypeProp StatusNTP = DefaultMailReportNT.getNodeTypeProp( "Status" );
                if( StatusNTP != null )
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeProp( StatusNTP );

                CswNbtMetaDataNodeTypeProp RecipientsNTP = DefaultMailReportNT.getNodeTypeProp( "Recipients" );
                CswNbtMetaDataNodeTypeProp DueDateIntervalNTP = DefaultMailReportNT.getNodeTypeProp( "Due Date Interval" );
                CswNbtMetaDataNodeTypeProp TypeNTP = DefaultMailReportNT.getNodeTypeProp( "Type" );
                CswNbtMetaDataNodeTypeProp WarningDaysNTP = DefaultMailReportNT.getNodeTypeProp( "Warning Days" );
                CswNbtMetaDataNodeTypeProp ReportNTP = DefaultMailReportNT.getNodeTypeProp( "Report" );
                CswNbtMetaDataNodeTypeProp ReportViewNTP = DefaultMailReportNT.getNodeTypeProp( "Report View" );
                CswNbtMetaDataNodeTypeProp MessageNTP = DefaultMailReportNT.getNodeTypeProp( "Message" );
                CswNbtMetaDataNodeTypeProp RunTimeNTP = DefaultMailReportNT.getNodeTypeProp( "Run Time" );
                CswNbtMetaDataNodeTypeProp NoDataNotificationNTP = DefaultMailReportNT.getNodeTypeProp( "No Data Notification" );
                CswNbtMetaDataNodeTypeProp FinalDueDateNTP = DefaultMailReportNT.getNodeTypeProp( "Final Due Date" );
                CswNbtMetaDataNodeTypeProp EnabledNTP = DefaultMailReportNT.getNodeTypeProp( "Enabled" );
                CswNbtMetaDataNodeTypeProp NextDueDateNTP = DefaultMailReportNT.getNodeTypeProp( "Next Due Date" );
                CswNbtMetaDataNodeTypeProp LastProcessedNTP = DefaultMailReportNT.getNodeTypeProp( "Last Processed" );
                CswNbtMetaDataNodeTypeProp RunStatusNTP = DefaultMailReportNT.getNodeTypeProp( "Run Status" );

                //Recipients                  Due Date Interval
                //Type                        Warning Days
                //    Report
                //    Report View
                //Message                     Run Time
                //No Data Notification        Final Due Date
                //Enabled                     Next Due Date
                //                            Last Processed
                //                            Run Status

                if( RecipientsNTP != null )
                {
                    RecipientsNTP.DisplayRow = 1;
                    RecipientsNTP.DisplayColumn = 1;
                    RecipientsNTP.DisplayRowAdd = 1;
                    RecipientsNTP.DisplayColAdd = 1;
                }
                if( DueDateIntervalNTP != null )
                {
                    DueDateIntervalNTP.DisplayRow = 1;
                    DueDateIntervalNTP.DisplayColumn = 2;
                    DueDateIntervalNTP.DisplayRowAdd = 1;
                    DueDateIntervalNTP.DisplayColAdd = 2;
                }
                if( TypeNTP != null )
                {
                    TypeNTP.DisplayRow = 2;
                    TypeNTP.DisplayColumn = 1;
                    TypeNTP.DisplayRowAdd = 2;
                    TypeNTP.DisplayColAdd = 1;
                }
                if( WarningDaysNTP != null )
                {
                    WarningDaysNTP.DisplayRow = 2;
                    WarningDaysNTP.DisplayColumn = 2;
                    WarningDaysNTP.DisplayRowAdd = 2;
                    WarningDaysNTP.DisplayColAdd = 2;
                }
                if( MessageNTP != null )
                {
                    MessageNTP.DisplayRow = 3;
                    MessageNTP.DisplayColumn = 1;
                    MessageNTP.DisplayRowAdd = 3;
                    MessageNTP.DisplayColAdd = 1;
                }
                if( RunTimeNTP != null )
                {
                    RunTimeNTP.DisplayRow = 3;
                    RunTimeNTP.DisplayColumn = 2;
                    RunTimeNTP.DisplayRowAdd = 3;
                    RunTimeNTP.DisplayColAdd = 2;
                }
                if( NoDataNotificationNTP != null )
                {
                    NoDataNotificationNTP.DisplayRow = 4;
                    NoDataNotificationNTP.DisplayColumn = 1;
                    NoDataNotificationNTP.DisplayRowAdd = 4;
                    NoDataNotificationNTP.DisplayColAdd = 1;
                }
                if( FinalDueDateNTP != null )
                {
                    FinalDueDateNTP.DisplayRow = 4;
                    FinalDueDateNTP.DisplayColumn = 2;
                    FinalDueDateNTP.DisplayRowAdd = 4;
                    FinalDueDateNTP.DisplayColAdd = 2;
                }
                if( EnabledNTP != null )
                {
                    EnabledNTP.DisplayRow = 5;
                    EnabledNTP.DisplayColumn = 1;
                    EnabledNTP.DisplayRowAdd = 5;
                    EnabledNTP.DisplayColAdd = 1;
                }
                if( NextDueDateNTP != null )
                {
                    NextDueDateNTP.DisplayRow = 5;
                    NextDueDateNTP.DisplayColumn = 2;
                    NextDueDateNTP.DisplayRowAdd = 5;
                    NextDueDateNTP.DisplayColAdd = 2;
                }
                if( LastProcessedNTP != null )
                {
                    LastProcessedNTP.DisplayRow = 7;
                    LastProcessedNTP.DisplayColumn = 2;
                    LastProcessedNTP.DisplayRowAdd = 7;
                    LastProcessedNTP.DisplayColAdd = 2;
                }
                if( RunStatusNTP != null )
                {
                    RunStatusNTP.DisplayRow = 7;
                    RunStatusNTP.DisplayColumn = 2;
                    RunStatusNTP.DisplayRowAdd = 7;
                    RunStatusNTP.DisplayColAdd = 2;
                }

                // conditionals have their own layouttable                
                if( ReportNTP != null )
                {
                    ReportNTP.DisplayRow = 1;
                    ReportNTP.DisplayColumn = 1;
                    ReportNTP.DisplayRowAdd = 1;
                    ReportNTP.DisplayColAdd = 1;
                }
                if( ReportViewNTP != null )
                {
                    ReportViewNTP.DisplayRow = 2;
                    ReportViewNTP.DisplayColumn = 1;
                    ReportViewNTP.DisplayRowAdd = 2;
                    ReportViewNTP.DisplayColAdd = 1;
                }
            } // if( DefaultMailReportNT != null)

        }//Update()


        public void _SetDefaultEquipmentFilter( CswNbtView View, CswNbtViewNode ParentViewNode )
        {
            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp EquipmentStatusOCP = EquipmentOC.getObjectClassProp( CswNbtObjClassEquipment.StatusPropertyName );

            foreach( CswNbtViewRelationship ChildRelationship in ParentViewNode.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
            {
                bool IsEquipmentRelationship = false;
                if( ChildRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType NT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( ChildRelationship.SecondId );
                    IsEquipmentRelationship = ( NT.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
                }
                else if( ChildRelationship.SecondType == CswNbtViewRelationship.RelatedIdType.ObjectClassId )
                {
                    IsEquipmentRelationship = ( ChildRelationship.SecondId == EquipmentOC.ObjectClassId );
                }

                if( IsEquipmentRelationship )
                {
                    bool AlreadyHasRetiredFilter = false;
                    foreach( CswNbtViewProperty ViewProp in ChildRelationship.Properties )
                    {
                        if( ( ViewProp.NodeTypeProp != null && ViewProp.NodeTypeProp.ObjectClassPropId == EquipmentStatusOCP.PropId ) ||
                            ( ViewProp.ObjectClassProp != null && ViewProp.ObjectClassProp.PropId == EquipmentStatusOCP.PropId ) )
                        {
                            foreach( CswNbtViewPropertyFilter Filter in ViewProp.Filters )
                            {
                                AlreadyHasRetiredFilter = ( Filter.SubfieldName == EquipmentStatusOCP.FieldTypeRule.SubFields.Default.Name &&
                                                            Filter.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals &&
                                                            Filter.Value == CswNbtObjClassEquipment.StatusOptionToDisplayString( CswNbtObjClassEquipment.StatusOption.Retired ) );
                            }
                        }
                    }

                    if( !AlreadyHasRetiredFilter )
                    {
                        CswNbtObjClassEquipment ObjClassEquipment = (CswNbtObjClassEquipment) _CswNbtSchemaModTrnsctn.makeObjClass( EquipmentOC );
                        ObjClassEquipment.addDefaultViewFilters( ChildRelationship );
                        View.save();
                    }
                } // if( IsEquipmentRelationship )
            } // foreach( CswNbtViewRelationship ChildRelationship in ParentViewNode.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship ) )
        } // _SetDefaultEquipmentFilter()


    }//class CswUpdateSchemaTo01H07

}//namespace ChemSW.Nbt.Schema


