using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswWebServiceInspectionDesign
    {
        private CswNbtResources _CswNbtResources;

        public CswWebServiceInspectionDesign( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private readonly CswCommaDelimitedString _ColumnNames = new CswCommaDelimitedString()
                                                           {
                                                               "Section",
                                                               "Question",
                                                               "Allowed Answers",
                                                               "Compliant Answers",
                                                               "Help Text"
                                                           };

        public void AddPrimaryKeys( ref DataTable myDataTable )
        {
            if( myDataTable.Columns["RowNumber"] == null )
            {
                myDataTable.Columns.Add( "RowNumber" );
                for( int RowIndex = 0; RowIndex < myDataTable.Rows.Count; RowIndex++ )
                {
                    myDataTable.Rows[RowIndex]["RowNumber"] = RowIndex;
                }
            }
        }

        /// <summary>
        /// Reads an Excel file from the file system and converts it into a ADO.NET data table
        /// </summary>
        /// <param name="FullPathAndFileName"></param>
        /// <returns></returns>
        public DataTable ConvertExcelFileToDataTable( string FullPathAndFileName, ref string ErrorMessage, ref string WarningMessage )
        {
            DataTable ExcelDataTable = null;
            OleDbConnection ExcelConn = null;

            try
            {
                // Microsoft JET engine knows how to read Excel files as a database
                // Problem is - it is old OLE technology - not newer ADO.NET
                string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FullPathAndFileName + ";Extended Properties=Excel 8.0;";
                ExcelConn = new OleDbConnection( ConnStr );
                ExcelConn.Open();

                DataTable ExcelSchemaDT = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
                string FirstSheetName = ExcelSchemaDT.Rows[0]["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
                DataAdapter.SelectCommand = SelectCommand;

                ExcelDataTable = new DataTable();
                DataAdapter.Fill( ExcelDataTable );

                foreach( string ColumnName in _ColumnNames )
                {
                    if( ExcelDataTable.Columns[ColumnName] == null )
                    {
                        ErrorMessage += "Column named '" + ColumnName + "' was not found.  ";
                    }
                }
                for( int ColumnIndex = 0; ColumnIndex < ExcelDataTable.Columns.Count; ColumnIndex++ )
                {
                    if( false == _ColumnNames.Contains( ExcelDataTable.Columns[ColumnIndex].ColumnName ) )
                    {
                        WarningMessage += "Column named '" + ExcelDataTable.Columns[ColumnIndex].ColumnName + "' was not used.  ";
                    }
                }
            } // try
            catch( Exception ex )
            {
                _CswNbtResources.CswLogger.reportError( ex );
            }
            finally
            {
                if( ExcelConn != null )
                {
                    ExcelConn.Close();
                    ExcelConn.Dispose();
                }
            }
            return ExcelDataTable;
        } // ConvertExcelFileToDataTable()

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionName, string InspectionTargetName )
        {
            JObject RetObj = new JObject();
            Int32 PropsWithoutError = 0;
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();
            CswCommaDelimitedString PropsWithError = new CswCommaDelimitedString();

            InspectionName = InspectionName.Trim();

            JArray GridArray = JArray.Parse( GridArrayString );
            CswNbtMetaDataNodeType InspectionTargetNT = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            if( null != InspectionTargetNT &&
                null != GridArray &&
                GridArray.Count > 0 )
            {
                RetObj["totalrows"] = GridArray.Count.ToString();

                CswNbtMetaDataNodeType NewInspectionNodeType = _CswNbtResources.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass.ToString(), InspectionName, InspectionTargetNT.Category );

                string FkType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
                Int32 FkValue = InspectionTargetNT.NodeTypeId;
                CswNbtMetaDataNodeTypeProp TargetProperty = NewInspectionNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                TargetProperty.SetFK( FkType, FkValue );

                //Get distinct tabs
                Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();

                for( Int32 Index = 0; Index < GridArray.Count; Index += 1 )
                {
                    if( GridArray[Index].Type == JTokenType.Object )
                    {
                        JObject ThisRow = (JObject) GridArray[Index];
                        string TabName = CswConvert.ToString( ThisRow["Section"] );
                        if( string.IsNullOrEmpty( TabName ) )
                        {
                            TabName = "Section 1";
                        }
                        if( false == Tabs.ContainsKey( TabName ) )
                        {
                            CswNbtMetaDataNodeTypeTab ThisTab = NewInspectionNodeType.getNodeTypeTab( TabName );
                            if( null == ThisTab )
                            {
                                ThisTab = _CswNbtResources.MetaData.makeNewTab( NewInspectionNodeType, TabName, NewInspectionNodeType.NodeTypeTabs.Count );
                            }
                            Tabs.Add( TabName, ThisTab );
                        }
                    }
                }

                for( Int32 Index = 0; Index < GridArray.Count; Index += 1 )
                {
                    if( GridArray[Index].Type == JTokenType.Object )
                    {
                        JObject ThisRow = (JObject) GridArray[Index];
                        string TabName = CswConvert.ToString( ThisRow["Section"] );
                        if( string.IsNullOrEmpty( TabName ) )
                        {
                            TabName = "Section 1";
                        }
                        string Question = CswConvert.ToString( ThisRow["Question"] );
                        if( false == string.IsNullOrEmpty( Question ) )
                        {
                            string AllowedAnswers = CswConvert.ToString( ThisRow["Allowed Answers"] );
                            string CompliantAnswers = CswConvert.ToString( ThisRow["Compliant Answers"] );
                            string HelpText = CswConvert.ToString( ThisRow["Help Text"] );

                            CswNbtMetaDataNodeTypeTab ThisTab;
                            Tabs.TryGetValue( TabName, out ThisTab );
                            Int32 ThisTabId = Int32.MinValue;
                            if( null != ThisTab )
                            {
                                ThisTabId = ThisTab.TabId;
                            }
                            CswNbtMetaDataNodeTypeProp ThisQuestion = _CswNbtResources.MetaData.makeNewProp( NewInspectionNodeType, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, ThisTabId );

                            if( null != ThisQuestion )
                            {
                                if( false == string.IsNullOrEmpty( AllowedAnswers ) &&
                                    false == string.IsNullOrEmpty( CompliantAnswers ) )
                                {
                                    PropsWithoutError += 1;
                                }
                                else
                                {
                                    PropsWithError.Add( Question );
                                }
                                ThisQuestion.ListOptions = AllowedAnswers;
                                ThisQuestion.ValueOptions = CompliantAnswers;
                                ThisQuestion.HelpText = HelpText;
                            }

                        }
                        else
                        {
                            GridRowsSkipped.Add( Index.ToString() );
                        }
                    }
                }
            }

            RetObj["rownumbersskipped"] = new JArray( GridRowsSkipped.ToString() );
            RetObj["questionswitherrors"] = new JArray( PropsWithError.ToString() );
            RetObj["countsucceeded"] = PropsWithoutError.ToString();

            return RetObj;
        }

        public JObject getNodesForInspectionTarget( string InspectionTargetName, bool IncludeSchedules = false )
        {
            JObject RetObj = new JObject();
            CswNbtMetaDataNodeType InspectionTargetNT = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            if( null == InspectionTargetNT )
            {
                //This is a new InspectionTarget
                RetObj["groupcount"] = "0";
                RetObj["succeeded"] = "true";
            }
            else
            {
                RetObj["succeeded"] = "false";
                if( InspectionTargetNT.ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass )
                {
                    throw new CswDniException( ErrorType.Error, "Cannot use " + InspectionTargetName + " as an Inspection Target.", "Attempted to use a nodetype of class " + InspectionTargetNT.ObjectClass.ObjectClass.ToString() + " as an InspectionTargetClass." );
                }
                CswNbtView InspectionScheduleView = _getViewForInspectionTarget( InspectionTargetNT, IncludeSchedules );
                if( null != InspectionScheduleView )
                {
                    RetObj["succeeded"] = "true";
                    ICswNbtTree GroupTree = _CswNbtResources.Trees.getTreeFromView( InspectionScheduleView, true, true, true, false );
                    RetObj["groupcount"] = GroupTree.getChildNodeCount().ToString();

                    JArray NodeNames = new JArray();
                    RetObj["groupnodenames"] = NodeNames;
                    if( GroupTree.getChildNodeCount() > 0 )
                    {
                        for( Int32 Child = 0; Child < GroupTree.getChildNodeCount(); Child += 1 )
                        {
                            GroupTree.goToNthChild( Child );
                            CswNbtNode GroupNode = GroupTree.getNodeForCurrentPosition();
                            if( null != GroupNode )
                            {
                                NodeNames.Add( GroupNode.NodeName );
                                if( IncludeSchedules )
                                {
                                    JArray Schedules = new JArray();
                                    RetObj[GroupNode.NodeName] = Schedules;
                                    for( Int32 SubChild = 0; SubChild < GroupTree.getChildNodeCount(); SubChild += 1 )
                                    {
                                        GroupTree.goToNthChild( SubChild );
                                        CswNbtNode ScheduleNode = GroupTree.getNodeForCurrentPosition();
                                        Schedules.Add( ScheduleNode.NodeName );
                                        GroupTree.goToParentNode();
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return RetObj;
        }

        private CswNbtView _getViewForInspectionTarget( CswNbtMetaDataNodeType InspectionTargetNT, bool IncludeSchedules )
        {
            CswNbtView RetView = null;
            CswNbtMetaDataNodeTypeProp InTargetGroupNTP = InspectionTargetNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            if( InTargetGroupNTP.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
            {
                CswNbtMetaDataNodeType InTargetGroupNT = _CswNbtResources.MetaData.getNodeType( InTargetGroupNTP.FKValue );
                if( null != InTargetGroupNT && InTargetGroupNT.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass )
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InTargetGroupNT, false );

                    if( IncludeSchedules )
                    {
                        CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                        foreach( CswNbtMetaDataNodeType GeneratorNt in GeneratorOC.NodeTypes )
                        {
                            CswNbtMetaDataNodeTypeProp OwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                            if( OwnerNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                                OwnerNtp.FKValue == InTargetGroupNT.NodeTypeId )
                            {
                                RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, OwnerNtp, false );
                            }
                        }
                    }
                }
            }
            else if( InTargetGroupNTP.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
            {
                //tough cookies for now
            }
            return RetView;
        }
    }
}