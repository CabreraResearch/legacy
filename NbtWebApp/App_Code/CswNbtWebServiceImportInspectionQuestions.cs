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
    public class CswNbtWebServiceImportInspectionQuestions
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceImportInspectionQuestions(CswNbtResources CswNbtResources)
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

        private static ImportColumns ImportColumnsFromDisplayString(string Column)
        {
            return (ImportColumns)Enum.Parse(typeof(ImportColumns), Column.Replace(' ', '_'), true);
        }

        /// <summary>
        /// Reads an Excel file from the file system and converts it into a ADO.NET data table
        /// </summary>
        /// <param name="FullPathAndFileName"></param>
        /// <returns></returns>
        public DataTable ConvertExcelFileToDataTable(string FullPathAndFileName)
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
            } // try
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + "\r\n" + ex.StackTrace);
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

        public void CreateNodes(DataTable ExcelDataTable)
        {
            CswNbtMetaDataNodeType NewInspectionDesignNodeType = _CswNbtResources.MetaData.makeNewNodeType("InspectionDesignClass", "Inspection Design " + DateTime.Now.ToString("MMddyy_HHmmss"), string.Empty);
 
            foreach (DataRow Row in ExcelDataTable.Rows)
            {
                string Section = Row[ImportColumnsToDisplayString(ImportColumns.Section)].ToString();
                string Question = Row[ImportColumnsToDisplayString(ImportColumns.Question)].ToString();
                string HelpText = Row[ImportColumnsToDisplayString(ImportColumns.Help_Text)].ToString();

                //string AllowedAnswers = Row[ImportColumnsToDisplayString(ImportColumns.Allowed_Answers)].ToString();
                CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                PossibleAnswers.FromString(Row[ImportColumnsToDisplayString(ImportColumns.Allowed_Answers)].ToString());

                //string CompliantAnswers = Row[ImportColumnsToDisplayString(ImportColumns.Compliant_Answers)].ToString();
                CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
                CompliantAnswers.FromString(Row[ImportColumnsToDisplayString(ImportColumns.Compliant_Answers)].ToString());

                CswNbtMetaDataNodeTypeProp QuestionProperty =  _CswNbtResources.MetaData.makeNewProp(NewInspectionDesignNodeType, CswNbtMetaDataFieldType.NbtFieldType.Question, Question, 0);

                // For mapping of question subfields to question node type properties
                // See lines 800 - 850 and lines 1908 - 1954 in design.aspx.cs
                QuestionProperty.ListOptions = PossibleAnswers.ToString();
                QuestionProperty.ValueOptions = CompliantAnswers.ToString();
                QuestionProperty.HelpText = HelpText;
            }
        }
    }
}