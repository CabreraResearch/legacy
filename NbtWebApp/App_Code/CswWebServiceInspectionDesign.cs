using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Exceptions;
using System.Xml.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web.Services;
using System.Xml;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.Config;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;
using System.Data.OleDb;

namespace ChemSW.Nbt.WebServices
{
    public class CswWebServiceInspectionDesign
    {
        private CswNbtResources _CswNbtResources;

        public CswWebServiceInspectionDesign(CswNbtResources CswNbtResources)
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
            CswDelimitedString CSVTemplate = new CswDelimitedString('\t');
            foreach (ImportColumns Col in Enum.GetValues(typeof(ImportColumns)))
            {
                CSVTemplate.Add(ImportColumnsToDisplayString(Col));
            }

            return CSVTemplate.ToString();
        }

        private static string ImportColumnsToDisplayString(ImportColumns Column)
        {
            return Column.ToString().Replace('_', ' ');
        }

        private static string ImportColumnsToDisplayString(string ColumnName)
        {
            return ColumnName.Replace('_', ' ');
        }

        private static ImportColumns ImportColumnsFromDisplayString(string Column)
        {
            return (ImportColumns)Enum.Parse(typeof(ImportColumns), Column.Replace(' ', '_'), true);
        }

        /// <summary>
        /// Reads an Excel file from the file system and converts it into a ADO.NET data table
        /// </summary>
        /// <param name="FullPathAndFileName"></param>
        /// <returns></returns>
        public DataTable ConvertExcelFileToDataTable(string FullPathAndFileName, ref string ErrorMessage, ref string WarningMessage)
        {
            DataTable ExcelDataTable = null;
            OleDbConnection ExcelConn = null;

            try
            {
                // Microsoft JET engine knows how to read Excel files as a database
                // Problem is - it is old OLE technology - not newer ADO.NET
                string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FullPathAndFileName + ";Extended Properties=Excel 8.0;";
                ExcelConn = new OleDbConnection(ConnStr);
                ExcelConn.Open();

                DataTable ExcelSchemaDT = ExcelConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string FirstSheetName = ExcelSchemaDT.Rows[0]["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand("SELECT * FROM [" + FirstSheetName + "]", ExcelConn);
                DataAdapter.SelectCommand = SelectCommand;

                ExcelDataTable = new DataTable();
                DataAdapter.Fill(ExcelDataTable);

                // Try to check for all the problems here - before we start creating database objects
                string[] ExpectedColumnNames = Enum.GetNames(typeof(ImportColumns));
                for (int ColumnNameIndex = 0; ColumnNameIndex < ExpectedColumnNames.Length; ColumnNameIndex++)
                {
                    ExpectedColumnNames[ColumnNameIndex] = ImportColumnsToDisplayString(ExpectedColumnNames[ColumnNameIndex]);
                }
                foreach (string myColumnName in ExpectedColumnNames)
                {
                    if (ExcelDataTable.Columns[myColumnName] == null)
                    {
                        ErrorMessage += "Column named '" + myColumnName + "' was not found.  ";
                    }
                }
                for (int ColumnIndex = 0; ColumnIndex < ExcelDataTable.Columns.Count; ColumnIndex++)
                {
                    if (!ExpectedColumnNames.Contains(ExcelDataTable.Columns[ColumnIndex].ColumnName))
                    {
                        WarningMessage += "Column named '" + ExcelDataTable.Columns[ColumnIndex].ColumnName + "' was not used.  ";
                    }
                }
                if (string.IsNullOrEmpty(ExcelDataTable.Rows[0][ImportColumnsToDisplayString(ImportColumns.Section)].ToString()))
                {
                    ErrorMessage += "User must supply a Section Name in the first row.  ";
                }

            } // try
            catch (Exception ex)
            {
                _CswNbtResources.CswLogger.reportError(ex);
            }
            finally
            {
                if (ExcelConn != null)
                {
                    ExcelConn.Close();
                    ExcelConn.Dispose();
                }
            }
            return ExcelDataTable;
        } // ConvertExcelFileToDataTable()

    public void AddPrimaryKeys(ref DataTable myDataTable)
    {
        if (myDataTable.Columns["RowNumber"] == null)
        {
            myDataTable.Columns.Add("RowNumber");
            for (int RowIndex = 0; RowIndex < myDataTable.Rows.Count; RowIndex++)
            {
                myDataTable.Rows[RowIndex]["RowNumber"] = RowIndex;
            }
        }
    }

        public int CreateNodes(DataTable ExcelDataTable, string NewInspectionName, string TargetName, ref string ErrorMessage, ref string WarningMessage)
        {
            int NumRowsImported = 0;

            try
            {
                if (ExcelDataTable != null)
                {
                    if (ExcelDataTable.Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(NewInspectionName))
                        {
                            NewInspectionName = NewInspectionName.Trim();
                            // Save the new Inspection
                            CswNbtMetaDataObjectClass InspectionObjectClass = _CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass);
                            CswNbtMetaDataNodeType NewInspectionNodeType = _CswNbtResources.MetaData.makeNewNodeType(InspectionObjectClass.ObjectClassId, NewInspectionName, "Fire Extinguisher");

                            // Get rid of the automatically created Section in this case
                            _CswNbtResources.MetaData.DeleteNodeTypeTab(NewInspectionNodeType.getNodeTypeTab("Section 1"));

                            // Set the target nodeType of the Target relationship property
                            // For now - we are setting the target relationship type to FE Inspection Point
                            CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType(TargetName);
                            string NewFKType = CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString();
                            Int32 NewFKValue = TargetNodeType.NodeTypeId;
                            CswNbtMetaDataNodeTypeProp TargetProperty = NewInspectionNodeType.getNodeTypePropByObjectClassPropName(CswNbtObjClassInspectionDesign.TargetPropertyName);
                            TargetProperty.SetFK(NewFKType, NewFKValue, string.Empty, Int32.MinValue);

                            // Setup Sections
                            if (!string.IsNullOrEmpty(ExcelDataTable.Rows[0][ImportColumnsToDisplayString(ImportColumns.Section)].ToString()))
                            {
                                // The TabOrder also effects the question number.  Question number = TabIndex + Question Number
                                Int32 CurrentTabOrder = 1;

                                string Section = ExcelDataTable.Rows[0][ImportColumnsToDisplayString(ImportColumns.Section)].ToString();
                                string Question = string.Empty;
                                string HelpText = string.Empty;
                                CswNbtMetaDataNodeTypeTab CurrentTab = null;
                                for (int RowIndex = 0; RowIndex < ExcelDataTable.Rows.Count; RowIndex++ )
                                {
                                    // IF the row has a new section THEN update to a new section
                                    if (!string.IsNullOrEmpty(ExcelDataTable.Rows[RowIndex][ImportColumnsToDisplayString(ImportColumns.Section)].ToString()))
                                    {
                                        Section = ExcelDataTable.Rows[RowIndex][ImportColumnsToDisplayString(ImportColumns.Section)].ToString();
                                        CurrentTab = NewInspectionNodeType.getNodeTypeTab(Section);
                                        if (CurrentTab == null)
                                        {
                                            CurrentTab = _CswNbtResources.MetaData.makeNewTab(NewInspectionNodeType, Section, CurrentTabOrder);
                                            CurrentTabOrder++;
                                        }
                                    }
                                    Question = ExcelDataTable.Rows[RowIndex][ImportColumnsToDisplayString(ImportColumns.Question)].ToString();
                                    HelpText = ExcelDataTable.Rows[RowIndex][ImportColumnsToDisplayString(ImportColumns.Help_Text)].ToString();

                                    CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                                    PossibleAnswers.FromString(ExcelDataTable.Rows[RowIndex][ImportColumnsToDisplayString(ImportColumns.Allowed_Answers)].ToString());

                                    CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                                    CompliantAnswers.FromString(ExcelDataTable.Rows[RowIndex][ImportColumnsToDisplayString(ImportColumns.Compliant_Answers)].ToString());

                                    // Make sure the row is not empty
                                    if ((!string.IsNullOrEmpty(Question)) || (!string.IsNullOrEmpty(PossibleAnswers.ToString())) || (!string.IsNullOrEmpty(CompliantAnswers.ToString())) || (!string.IsNullOrEmpty(HelpText)))
                                    {
                                        // There is something in the row - make sure all the fields we require are present
                                        if ((!string.IsNullOrEmpty(Question)) && (!string.IsNullOrEmpty(PossibleAnswers.ToString())) && (!string.IsNullOrEmpty(CompliantAnswers.ToString())))
                                        {
                                            CswNbtMetaDataNodeTypeProp QuestionProperty = _CswNbtResources.MetaData.makeNewProp(NewInspectionNodeType, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, CurrentTab.TabId);

                                            if (QuestionProperty != null)
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
                            else
                            {
                                ErrorMessage += "User must supply a Section Name in the first row.  ";
                            }
                        }
                        else
                        {
                            ErrorMessage += "New Inspection Name was null or empty.  ";
                        }
                    }
                    else
                    {
                        ErrorMessage += "Excel data table did not have any rows in it.  ";
                    }
                }
                else
                {
                    ErrorMessage += "Excel data table was null.  ";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage += "Exception: " + ex.Message;
            }
            return NumRowsImported;
        }

        public string getInspectionTargets()
        {
            System.Text.StringBuilder HtmlSelectBuilder = new System.Text.StringBuilder();
            CswNbtMetaDataObjectClass InspectionObjectClass = _CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass);

            HtmlSelectBuilder.Append("<select>");
            
            foreach (CswNbtMetaDataObjectClassProp Property in InspectionObjectClass.ObjectClassProps)
            {
                if (Property.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship)
                {
                    HtmlSelectBuilder.Append("<option value=\"" + Property.PropId.ToString() + "\">" + Property.PropName + "</option>");
                }
            }

            HtmlSelectBuilder.Append("</select>");
            return HtmlSelectBuilder.ToString();
        }

        public bool IsNodeTypeNameUnique(string NewInsepctionName)
        {
            CswNbtMetaDataNodeType aNodeType = _CswNbtResources.MetaData.getNodeType(NewInsepctionName);
            if (aNodeType == null)
                return true;
            else
                return false;
        }
    }
}