using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Logic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Webservice for inspections
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
            InspectionData.Columns.Add( "Deficient Question" );
            InspectionData.Columns.Add( "Deficient Answer" );
            InspectionData.Columns.Add( "Date Answered" );
            InspectionData.Columns.Add( "Comments" );

            // get Deficient inspections
            CswNbtView DeficientView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship InspectionRel = DeficientView.AddViewRelationship( InspectionOC, false );
            CswNbtViewProperty StatusViewProp = DeficientView.AddViewProperty( InspectionRel, InspectionStatusOCP );
            CswNbtViewPropertyFilter StatusDeficientFilter = DeficientView.AddViewPropertyFilter(
                StatusViewProp,
                InspectionStatusOCP.getFieldTypeRule().SubFields.Default.Name,
                CswNbtPropFilterSql.PropertyFilterMode.Equals,
                CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Action_Required ),
                false );

            ICswNbtTree DeficientTree = _CswNbtResources.Trees.getTreeFromView( DeficientView, false, true, false, false );
            for( Int32 i = 0; i < DeficientTree.getChildNodeCount(); i++ )
            {
                DeficientTree.goToNthChild( i );

                bool AtLeastOneQuestion = false;
                CswNbtNode InspectionNode = DeficientTree.getNodeForCurrentPosition();
                CswNbtObjClassInspectionDesign NodeAsInspection = (CswNbtObjClassInspectionDesign) InspectionNode;
                CswNbtPropEnmrtrFiltered QuestionProps = InspectionNode.Properties[(CswNbtMetaDataFieldType.NbtFieldType) CswNbtMetaDataFieldType.NbtFieldType.Question];
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
                        if( NodeAsInspection.Date.DateTimeValue != DateTime.MinValue )
                        {
                            Row["Due"] = NodeAsInspection.Date.DateTimeValue.ToShortDateString();
                        }
                        Row["Status"] = NodeAsInspection.Status.Value;
                        Row["Deficient Question"] = QuestionProp.NodeTypeProp.PropNameWithQuestionNo;
                        Row["Deficient Answer"] = QuestionProp.AsQuestion.Answer;
                        if( NodeAsInspection.Date.DateTimeValue != DateTime.MinValue )
                        {
                            Row["Date Answered"] = QuestionProp.AsQuestion.DateAnswered.ToShortDateString();
                        }
                        Row["Comments"] = QuestionProp.AsQuestion.Comments;
                        InspectionData.Rows.Add( Row );

                        AtLeastOneQuestion = true;
                    } // if(!QuestionProp.AsQuestion.IsCompliant)
                } // foreach(CswNbtNodePropWrapper QuestionProp  in QuestionProps )

                if( false == AtLeastOneQuestion )
                {
                    // case 25501 - add a row for the inspection anyway
                    DataRow Row = InspectionData.NewRow();
                    Row["rownum"] = CswConvert.ToDbVal( InspectionData.Rows.Count + 1 );
                    Row["nodeid"] = CswConvert.ToDbVal( InspectionNode.NodeId.PrimaryKey );
                    Row["nodepk"] = InspectionNode.NodeId.ToString();
                    Row["Inspection"] = InspectionNode.NodeName;
                    Row["Inspection Point"] = NodeAsInspection.Target.CachedNodeName;
                    if( NodeAsInspection.Date.DateTimeValue != DateTime.MinValue )
                    {
                        Row["Due"] = NodeAsInspection.Date.DateTimeValue.ToShortDateString();
                    }
                    Row["Status"] = NodeAsInspection.Status.Value;
                    InspectionData.Rows.Add( Row );
                }

                DeficientTree.goToParentNode();
            } // for( Int32 i = 0; i < DeficientTree.getChildNodeCount(); i++ )

            CswNbtSdGrid gd = new CswNbtSdGrid( _CswNbtResources );
            gd.PkColumn = "rownum";
            return gd.DataTableToJSON( InspectionData );

        } // getInspectionStatusGrid

    } // class CswNbtWebServiceInspections
} // namespace ChemSW.Nbt.WebServices

