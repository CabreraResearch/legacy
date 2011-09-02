using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for the table of components on the Welcome page
    /// </summary>
    public class CswNbtWebServiceInspections
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceInspections( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public JObject getInspectionStatusGrid()
        {
            CswNbtMetaDataObjectClass InspectionOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp InspectionStatusOCP = InspectionOC.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );

            DataTable InspectionData = new DataTable();
            InspectionData.Columns.Add( "rownum" );
            InspectionData.Columns.Add( "nodeid" );
            InspectionData.Columns.Add( "nodepk" );
            InspectionData.Columns.Add( "Inspection" );
            InspectionData.Columns.Add( "Inspection Point" );
            InspectionData.Columns.Add( "Due" );
            InspectionData.Columns.Add( "Status" );
            InspectionData.Columns.Add( "OOC Question" );
            InspectionData.Columns.Add( "OOC Answer" );
            InspectionData.Columns.Add( "Date Answered" );
            InspectionData.Columns.Add( "Comments" );

            // get OOC inspections
            CswNbtView OOCView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship InspectionRel = OOCView.AddViewRelationship( InspectionOC, false );
            CswNbtViewProperty StatusViewProp = OOCView.AddViewProperty( InspectionRel, InspectionStatusOCP );
            CswNbtViewPropertyFilter StatusOOCFilter = OOCView.AddViewPropertyFilter(
                StatusViewProp,
                InspectionStatusOCP.FieldTypeRule.SubFields.Default.Name,
                CswNbtPropFilterSql.PropertyFilterMode.Equals,
                CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required ),
                false );

            ICswNbtTree OOCTree = _CswNbtResources.Trees.getTreeFromView( OOCView, false, true, false, false );
            for( Int32 i = 0; i < OOCTree.getChildNodeCount(); i++ )
            {
                OOCTree.goToNthChild( i );

                CswNbtNode InspectionNode = OOCTree.getNodeForCurrentPosition();
                CswNbtObjClassInspectionDesign NodeAsInspection = CswNbtNodeCaster.AsInspectionDesign( InspectionNode );
                CswNbtPropEnmrtrFiltered QuestionProps = InspectionNode.Properties[CswNbtMetaDataFieldType.NbtFieldType.Question];
                foreach( CswNbtNodePropWrapper QuestionProp in QuestionProps )
                {
                    if( !QuestionProp.AsQuestion.IsCompliant )
                    {
                        DataRow Row = InspectionData.NewRow();
                        Row["rownum"] = CswConvert.ToDbVal( InspectionData.Rows.Count + 1 );
                        Row["nodeid"] = CswConvert.ToDbVal( InspectionNode.NodeId.PrimaryKey );
                        Row["nodepk"] = InspectionNode.NodeId.ToString();
                        Row["Inspection"] = InspectionNode.NodeName;
                        Row["Inspection Point"] = NodeAsInspection.Target.CachedNodeName;
                        Row["Due"] = NodeAsInspection.Date.DateTimeValue.ToShortDateString();
                        Row["Status"] = NodeAsInspection.Status.Value;
                        Row["OOC Question"] = QuestionProp.NodeTypeProp.PropNameWithQuestionNo;
                        Row["OOC Answer"] = QuestionProp.AsQuestion.Answer;
                        Row["Date Answered"] = QuestionProp.AsQuestion.DateAnswered.ToShortDateString();
                        Row["Comments"] = QuestionProp.AsQuestion.Comments;
                        InspectionData.Rows.Add( Row );

                    } // if(!QuestionProp.AsQuestion.IsCompliant)
                } // foreach(CswNbtNodePropWrapper QuestionProp  in QuestionProps )

                OOCTree.goToParentNode();
            } // for( Int32 i = 0; i < OOCTree.getChildNodeCount(); i++ )

            CswGridData gd = new CswGridData( _CswNbtResources );
            gd.PkColumn = "rownum";
            return gd.DataTableToJSON( InspectionData );

        } // getInspectionStatusGrid

    } // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

