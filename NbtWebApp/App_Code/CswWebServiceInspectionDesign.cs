using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
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

        private enum ImportColumns
        {
            Section,
            Question,
            Allowed_Answers,
            Compliant_Answers,
            Help_Text
        }

        public string GetExcelTemplate()
        {
            CswDelimitedString CSVTemplate = new CswDelimitedString( '\t' );
            foreach( ImportColumns Col in Enum.GetValues( typeof( ImportColumns ) ) )
            {
                CSVTemplate.Add( _stripUnderscores( Col ) );
            }

            return CSVTemplate.ToString();
        }

        private static string _stripUnderscores( Object Name )
        {
            return CswConvert.ToString( Name ).Replace( '_', ' ' );
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

                // Try to check for all the problems here - before we start creating database objects
                string[] ExpectedColumnNames = Enum.GetNames( typeof( ImportColumns ) );
                for( int ColumnNameIndex = 0; ColumnNameIndex < ExpectedColumnNames.Length; ColumnNameIndex++ )
                {
                    ExpectedColumnNames[ColumnNameIndex] = _stripUnderscores( ExpectedColumnNames[ColumnNameIndex] );
                }
                foreach( string myColumnName in ExpectedColumnNames )
                {
                    if( ExcelDataTable.Columns[myColumnName] == null )
                    {
                        ErrorMessage += "Column named '" + myColumnName + "' was not found.  ";
                    }
                }
                for( int ColumnIndex = 0; ColumnIndex < ExcelDataTable.Columns.Count; ColumnIndex++ )
                {
                    if( !ExpectedColumnNames.Contains( ExcelDataTable.Columns[ColumnIndex].ColumnName ) )
                    {
                        WarningMessage += "Column named '" + ExcelDataTable.Columns[ColumnIndex].ColumnName + "' was not used.  ";
                    }
                }
                if( string.IsNullOrEmpty( ExcelDataTable.Rows[0][_stripUnderscores( ImportColumns.Section )].ToString() ) )
                {
                    ErrorMessage += "User must supply a Section Name in the first row.  ";
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

        public int CreateNodes( DataTable ExcelDataTable, string NewInspectionName, string TargetName, ref string ErrorMessage, ref string WarningMessage )
        {
            int NumRowsImported = 0;

            try
            {
                if( ExcelDataTable == null )
                {
                    throw new CswDniException( ErrorType.Warning, "Excel data table empty.", "Excel Table was null" );
                }
                if( ExcelDataTable.Rows.Count == 0 )
                {
                    throw new CswDniException( ErrorType.Warning, "Excel data table did not have any rows in it.", "Excel table row count was 0." );
                }
                if( string.IsNullOrEmpty( NewInspectionName ) )
                {
                    throw new CswDniException( ErrorType.Warning, "Inspection must have a name", "New Inspection Name was null or empty." );
                }
                NewInspectionName = NewInspectionName.Trim();
                // Save the new Inspection
                CswNbtMetaDataObjectClass InspectionObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtMetaDataNodeType NewInspectionNodeType = _CswNbtResources.MetaData.makeNewNodeType( InspectionObjectClass.ObjectClassId, NewInspectionName, "Fire Extinguisher" );

                // Get rid of the automatically created Section in this case
                _CswNbtResources.MetaData.DeleteNodeTypeTab( NewInspectionNodeType.getNodeTypeTab( "Section 1" ) );

                // Set the target nodeType of the Target relationship property
                // For now - we are setting the target relationship type to FE Inspection Point
                CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( TargetName );
                string NewFKType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
                Int32 NewFKValue = TargetNodeType.NodeTypeId;
                CswNbtMetaDataNodeTypeProp TargetProperty = NewInspectionNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.TargetPropertyName );
                TargetProperty.SetFK( NewFKType, NewFKValue, string.Empty, Int32.MinValue );

                // Setup Sections
                if( string.IsNullOrEmpty( ExcelDataTable.Rows[0][_stripUnderscores( ImportColumns.Section )].ToString() ) )
                {
                    throw new CswDniException( ErrorType.Warning, "User must supply a Section Name in the first row.", "" );
                }
                // The TabOrder also effects the question number.  Question number = TabIndex + Question Number
                Int32 CurrentTabOrder = 1;

                string Section = ExcelDataTable.Rows[0][_stripUnderscores( ImportColumns.Section )].ToString();
                string Question = string.Empty;
                string HelpText = string.Empty;
                CswNbtMetaDataNodeTypeTab CurrentTab = null;
                for( int RowIndex = 0; RowIndex < ExcelDataTable.Rows.Count; RowIndex++ )
                {
                    // IF the row has a new section THEN update to a new section
                    if( !string.IsNullOrEmpty( ExcelDataTable.Rows[RowIndex][_stripUnderscores( ImportColumns.Section )].ToString() ) )
                    {
                        Section = ExcelDataTable.Rows[RowIndex][_stripUnderscores( ImportColumns.Section )].ToString();
                        CurrentTab = NewInspectionNodeType.getNodeTypeTab( Section );
                        if( CurrentTab == null )
                        {
                            CurrentTab = _CswNbtResources.MetaData.makeNewTab( NewInspectionNodeType, Section, CurrentTabOrder );
                            CurrentTabOrder++;
                        }
                    }
                    Question = ExcelDataTable.Rows[RowIndex][_stripUnderscores( ImportColumns.Question )].ToString();
                    HelpText = ExcelDataTable.Rows[RowIndex][_stripUnderscores( ImportColumns.Help_Text )].ToString();

                    CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                    PossibleAnswers.FromString( ExcelDataTable.Rows[RowIndex][_stripUnderscores( ImportColumns.Allowed_Answers )].ToString() );

                    CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                    CompliantAnswers.FromString( ExcelDataTable.Rows[RowIndex][_stripUnderscores( ImportColumns.Compliant_Answers )].ToString() );

                    // Make sure the row is not empty
                    if( ( !string.IsNullOrEmpty( Question ) ) || ( !string.IsNullOrEmpty( PossibleAnswers.ToString() ) ) || ( !string.IsNullOrEmpty( CompliantAnswers.ToString() ) ) || ( !string.IsNullOrEmpty( HelpText ) ) )
                    {
                        // There is something in the row - make sure all the fields we require are present
                        if( ( !string.IsNullOrEmpty( Question ) ) && ( !string.IsNullOrEmpty( PossibleAnswers.ToString() ) ) && ( !string.IsNullOrEmpty( CompliantAnswers.ToString() ) ) )
                        {
                            CswNbtMetaDataNodeTypeProp QuestionProperty = _CswNbtResources.MetaData.makeNewProp( NewInspectionNodeType, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, CurrentTab.TabId );

                            if( QuestionProperty != null )
                            {
                                // For mapping of question subfields to question node type properties
                                // See lines 800 - 850 and lines 1908 - 1954 in design.aspx.cs
                                QuestionProperty.ListOptions = PossibleAnswers.ToString();
                                QuestionProperty.ValueOptions = CompliantAnswers.ToString();
                                QuestionProperty.HelpText = HelpText;
                                NumRowsImported++;
                            }
                            else
                            {
                                ErrorMessage += "Did not create Question Node Type Property.";
                            }
                        }
                        else
                        {
                            WarningMessage += "Some rows may not have been imported because they were missing data.";
                        }
                    }
                }

            }
            catch( Exception ex )
            {
                ErrorMessage += "Exception: " + ex.Message;
            }
            return NumRowsImported;
        }

        public JObject getInspectionTargetGroupView( string InspectionTargetName )
        {
            JObject RetObj = new JObject();
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( ErrorType.Warning, "Inspection Target's name was not specified", "InspectionTargetName was null or empty" );
            }

            CswNbtView InspectionScheduleView = null;

            RetObj["succeeded"] = "false";
            CswNbtMetaDataNodeType InspectionTargetNT = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            if( null == InspectionTargetNT )
            {
                //we'll create it on Finish, we'll build the schedules manually client-side
                RetObj["succeeded"] = "true";
                RetObj["noview"] = "true";
            }
            else
            {
                if( InspectionTargetNT.ObjectClass.ObjectClass != CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass )
                {
                    throw new CswDniException( ErrorType.Error, "Cannot create an inspection on a " + InspectionTargetName, "Attempted to create an inspection on a nodetype of class " + InspectionTargetNT.ObjectClass.ObjectClass.ToString() );
                }
                CswNbtMetaDataNodeTypeProp InTargetGroupNTP = InspectionTargetNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                if( InTargetGroupNTP.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() )
                {
                    CswNbtMetaDataNodeType InTargetGroupNT = _CswNbtResources.MetaData.getNodeType( InTargetGroupNTP.FKValue );
                    if( null != InTargetGroupNT && InTargetGroupNT.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass )
                    {
                        CswNbtMetaDataObjectClass GeneratorOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
                        foreach( CswNbtMetaDataNodeType GeneratorNt in GeneratorOC.NodeTypes )
                        {
                            CswNbtMetaDataNodeTypeProp OwnerNtp = GeneratorNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassGenerator.OwnerPropertyName );
                            if( OwnerNtp.FKType == CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString() &&
                                OwnerNtp.FKValue == InTargetGroupNT.NodeTypeId )
                            {
                                InspectionScheduleView = new CswNbtView( _CswNbtResources );
                                InspectionScheduleView.Visibility = NbtViewVisibility.User;
                                InspectionScheduleView.VisibilityUserId = _CswNbtResources.CurrentNbtUser.UserId;
                                InspectionScheduleView.ViewName = InspectionTargetName + " Schedule View";
                                InspectionScheduleView.ViewMode = NbtViewRenderingMode.Tree;

                                CswNbtViewRelationship IpGroupRelationship = InspectionScheduleView.AddViewRelationship( InTargetGroupNT, false );
                                InspectionScheduleView.AddViewRelationship( IpGroupRelationship, CswNbtViewRelationship.PropOwnerType.Second, OwnerNtp, false );

                                InspectionScheduleView.SaveToCache( false );

                                RetObj["succeeded"] = "true";
                                RetObj["viewid"] = InspectionScheduleView.SessionViewId.ToString();
                            }
                        }
                    }
                }
                else if( InTargetGroupNTP.FKType == CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString() )
                {
                    //tough cookies for now
                }
            }
            return RetObj;
        }

    }
}