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

        public JObject copyInspectionDesign( string CopyFromInspectionDesign, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JObject RetObj = new JObject();
            return RetObj;
        }

        private Dictionary<string, CswNbtMetaDataNodeTypeTab> _getTabsForInspection( JArray Grid, CswNbtMetaDataNodeType NodeType )
        {
            Dictionary<string, CswNbtMetaDataNodeTypeTab> RetDict = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = CswConvert.ToString( ThisRow["Section"] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = "Section 1";
                    }
                    if( false == RetDict.ContainsKey( TabName ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab = NodeType.getNodeTypeTab( TabName );
                        if( null == ThisTab )
                        {
                            ThisTab = _CswNbtResources.MetaData.makeNewTab( NodeType, TabName, NodeType.NodeTypeTabs.Count );
                        }
                        RetDict.Add( TabName, ThisTab );
                    }
                }
            }
            return RetDict;
        }

        private Int32 _createInspectionProps( JArray Grid, CswNbtMetaDataNodeType InspectionDesignNt,
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs, CswCommaDelimitedString GridRowsSkipped )
        {
            Int32 RetCount = 0;
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = CswConvert.ToString( ThisRow["Section"] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = "Section 1";
                    }
                    string Question = CswConvert.ToString( ThisRow["Question"] );
                    string AllowedAnswers = CswConvert.ToString( ThisRow["Allowed Answers"] );
                    string CompliantAnswers = CswConvert.ToString( ThisRow["Compliant Answers"] );
                    string HelpText = CswConvert.ToString( ThisRow["Help Text"] );

                    if( false == string.IsNullOrEmpty( Question ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab;
                        Tabs.TryGetValue( TabName, out ThisTab );
                        Int32 ThisTabId = Int32.MinValue;
                        if( null != ThisTab )
                        {
                            ThisTabId = ThisTab.TabId;
                        }
                        CswNbtMetaDataNodeTypeProp ThisQuestion = _CswNbtResources.MetaData.makeNewProp( InspectionDesignNt, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, ThisTabId );

                        if( null != ThisQuestion )
                        {
                            if( false == string.IsNullOrEmpty( AllowedAnswers ) )
                            {
                                ThisQuestion.ListOptions = AllowedAnswers;
                            }
                            if( false == string.IsNullOrEmpty( CompliantAnswers ) )
                            {
                                ThisQuestion.ValueOptions = CompliantAnswers;
                            }
                            if( false == string.IsNullOrEmpty( HelpText ) )
                            {
                                ThisQuestion.HelpText = HelpText;
                            }
                            RetCount += 1;
                        }

                    }
                    else
                    {
                        GridRowsSkipped.Add( Index.ToString() );
                    }
                }
            }
            return RetCount;
        }

        private void _createInspectionDesignRelationships( CswNbtMetaDataNodeType InspectionDesignNt, string InspectionTargetName )
        {
            CswNbtMetaDataNodeType InspectionTargetNT = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            string FkType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
            Int32 FkValue = InspectionTargetNT.NodeTypeId;
            CswNbtMetaDataNodeTypeProp TargetProperty = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
            TargetProperty.SetFK( FkType, FkValue );
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JObject RetObj = new JObject();
            Int32 PropsWithoutError = 0;
            Int32 TotalRows = 0;
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();

            InspectionDesignName = InspectionDesignName.Trim();

            JArray GridArray = JArray.Parse( GridArrayString );

            if( null != GridArray &&
                GridArray.Count > 0 )
            {
                TotalRows = GridArray.Count;

                //Build a _getCategory() method
                //InspectionTargetNT.Category: 
                CswNbtMetaDataNodeType NewInspectionNodeType = _CswNbtResources.MetaData.makeNewNodeType( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass.ToString(), InspectionDesignName, Category );

                //Get distinct tabs
                Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs = _getTabsForInspection( GridArray, NewInspectionNodeType );
                //Create the props
                PropsWithoutError = _createInspectionProps( GridArray, NewInspectionNodeType, Tabs, GridRowsSkipped );
                //Build the MetaData
                _createInspectionDesignRelationships( NewInspectionNodeType, InspectionTargetName );
            }

            RetObj["totalrows"] = TotalRows.ToString();
            RetObj["rownumbersskipped"] = new JArray( GridRowsSkipped.ToString() );
            RetObj["countsucceeded"] = PropsWithoutError.ToString();

            return RetObj;
        }

        public JObject getScheduleNodesForInspection( string InspectionTargetName, string CopyInspectionDesignName )
        {
            JObject RetObj = new JObject();
            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( CopyInspectionDesignName );
            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            if( null == InspectionDesignNt && null == InspectionTargetNt )
            {
                //This is a new InspectionDesign on a new InspectionTarget
                RetObj["groupcount"] = "0";
                RetObj["succeeded"] = "true";
            }
            else
            {
                RetObj["succeeded"] = "false";
                CswNbtView InspectionScheduleView = null;
                if( null != InspectionDesignNt && InspectionDesignNt.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                {
                    InspectionScheduleView = _getSchedulesViewFromInspectionDesign( InspectionDesignNt );
                }
                else
                {
                    if( null == InspectionTargetNt || InspectionTargetNt.ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot use " + InspectionTargetName + " as an Inspection Target.", "Attempted to use a nodetype as an InspectionTargetClass." );
                    }
                    InspectionScheduleView = _getSchedulesViewFromInspectionTarget( InspectionTargetNt );
                }
                if( null != InspectionScheduleView )
                {
                    RetObj["succeeded"] = "true";
                    ICswNbtTree GroupTree = _CswNbtResources.Trees.getTreeFromView( InspectionScheduleView, true, true, true, false );
                    RetObj["groupcount"] = GroupTree.getChildNodeCount().ToString();

                    JArray NodeNames = new JArray();
                    RetObj["groupnodenames"] = NodeNames;
                    if( GroupTree.getChildNodeCount() > 0 )
                    {
                        for( Int32 GroupChild = 0; GroupChild < GroupTree.getChildNodeCount(); GroupChild += 1 )
                        {
                            GroupTree.goToNthChild( GroupChild );
                            CswNbtNode GroupNode = GroupTree.getNodeForCurrentPosition();
                            if( null != GroupNode )
                            {
                                NodeNames.Add( GroupNode.NodeName );
                                JArray Schedules = new JArray();
                                RetObj[GroupNode.NodeName] = Schedules;
                                for( Int32 SchedChild = 0; SchedChild < GroupTree.getChildNodeCount(); SchedChild += 1 )
                                {
                                    GroupTree.goToNthChild( SchedChild );
                                    CswNbtNode ScheduleNode = GroupTree.getNodeForCurrentPosition();
                                    Schedules.Add( ScheduleNode.NodeName );
                                    GroupTree.goToParentNode();
                                }
                            }
                            GroupTree.goToParentNode();
                        }
                    }
                }
            }
            return RetObj;
        }

        private CswNbtView _getSchedulesViewFromInspectionDesign( CswNbtMetaDataNodeType InspectionDesignNt )
        {
            CswNbtView RetView = null;
            CswNbtMetaDataNodeTypeProp GeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
            if( GeneratorNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
            {
                CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( GeneratorNtp.FKValue );
                if( null != GeneratorNt && GeneratorNt.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass )
                {
                    CswNbtMetaDataNodeTypeProp GenOwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                    if( GenOwnerNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                    {
                        CswNbtMetaDataNodeType InspectionGroupNt = _CswNbtResources.MetaData.getNodeType( GenOwnerNtp.FKValue );
                        if( null != InspectionGroupNt && InspectionGroupNt.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass )
                        {
                            RetView = new CswNbtView( _CswNbtResources );
                            CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionGroupNt, false );
                            RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, GenOwnerNtp, false );
                        }
                    }
                }
            }
            else if( GeneratorNtp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create a Schedule view", "Cannot use Object Class relationships to construct a view." );
            }
            return RetView;
        }

        private CswNbtView _getSchedulesViewFromInspectionTarget( CswNbtMetaDataNodeType InspectionTargetNt )
        {
            CswNbtView RetView = null;
            CswNbtMetaDataNodeTypeProp InTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
            if( InTargetGroupNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
            {
                CswNbtMetaDataNodeType InTargetGroupNt = _CswNbtResources.MetaData.getNodeType( InTargetGroupNtp.FKValue );
                if( null != InTargetGroupNt && InTargetGroupNt.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass )
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InTargetGroupNt, false );

                    CswNbtMetaDataObjectClass GeneratorOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                    foreach( CswNbtMetaDataNodeType GeneratorNt in GeneratorOc.NodeTypes )
                    {
                        CswNbtMetaDataNodeTypeProp OwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                        if( OwnerNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                            OwnerNtp.FKValue == InTargetGroupNt.NodeTypeId )
                        {
                            RetView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, OwnerNtp, false );
                        }
                    }
                }
            }
            else if( InTargetGroupNtp.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
            {
                throw new CswDniException( ErrorType.Warning, "Cannot create a Schedule view", "Cannot use Object Class relationships to construct a view." );
            }
            return RetView;
        }
    }
}