﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Threading;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceInspectionDesign
    {
        #region ctor

        private CswNbtResources _CswNbtResources;
        private readonly ICswNbtUser _CurrentUser;
        private readonly TextInfo _TextInfo;
        public CswNbtWebServiceInspectionDesign( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;

            if( _CswNbtResources.CurrentNbtUser.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName )
            {
                throw new CswDniException( ErrorType.Error, "Only the ChemSW Admin role can access the Inspection Design wizard.", "Attempted to access the Inspection Design wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            _CurrentUser = _CswNbtResources.CurrentNbtUser;
            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }

        #endregion ctor


        #region Public

        /// <summary>
        /// Reads an Excel file from the file system and converts it into a ADO.NET data table
        /// </summary>
        /// <param name="FullPathAndFileName"></param>
        /// <returns></returns>
        public DataTable convertExcelFileToDataTable( string FullPathAndFileName, ref string ErrorMessage, ref string WarningMessage )
        {
            DataTable RetDataTable = new DataTable();
            OleDbConnection ExcelConn = null;

            try
            {
                // Microsoft JET engine knows how to read Excel files as a database
                // Problem is - it is old OLE technology - not newer ADO.NET
                string ConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FullPathAndFileName + ";Extended Properties=Excel 8.0;";
                ExcelConn = new OleDbConnection( ConnStr );
                ExcelConn.Open();

                DataTable InspectionDt = ExcelConn.GetOleDbSchemaTable( OleDbSchemaGuid.Tables, null );
                if( null == InspectionDt )
                {
                    throw new CswDniException( ErrorType.Error, "Could not process the uploaded file: " + FullPathAndFileName, "GetOleDbSchemaTable failed to parse a valid XLS file." );
                }

                string FirstSheetName = InspectionDt.Rows[0]["TABLE_NAME"].ToString();

                OleDbDataAdapter DataAdapter = new OleDbDataAdapter();
                OleDbCommand SelectCommand = new OleDbCommand( "SELECT * FROM [" + FirstSheetName + "]", ExcelConn );
                DataAdapter.SelectCommand = SelectCommand;

                DataTable UploadDataTable = new DataTable();

                DataAdapter.Fill( UploadDataTable );

                //we have finished deserializing the XLS file by now...
                CswNbtActInspectionDesignWiz wiz = new CswNbtActInspectionDesignWiz( _CswNbtResources );
                RetDataTable =  wiz.prepareDataTable( UploadDataTable );

            } // try
            catch( Exception Exception )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
            }
            finally
            {
                if( ExcelConn != null )
                {
                    ExcelConn.Close();
                    ExcelConn.Dispose();
                }
            }
            return RetDataTable;
        } // convertExcelFileToDataTable()

        public JObject recycleInspectionDesign( string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswNbtActInspectionDesignWiz wiz = new CswNbtActInspectionDesignWiz( _CswNbtResources );
            return( wiz.recycleInspectionDesign(InspectionDesignName,InspectionTargetName,Category) );
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswNbtActInspectionDesignWiz wiz = new CswNbtActInspectionDesignWiz( _CswNbtResources );
            return ( wiz.createInspectionDesignTabsAndProps( GridArrayString, InspectionDesignName, InspectionTargetName, Category ) );

        }

        #endregion Public

    }
}