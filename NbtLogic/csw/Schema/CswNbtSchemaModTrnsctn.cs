﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ChemSW.Audit;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Log;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ImportExport;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.Search;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.RscAdo;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswNbtSchemaModTrnsctn
    {
        private CswNbtResources _CswNbtResources;

        private CswDDL _CswDdl = null;
        CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();

        public ICswDbCfgInfo CswDbCfgInfo
        {
            get
            {
                return ( _CswNbtResources.CswDbCfgInfo );
            }
        }

        public string ConfigFileLocation
        {
            get
            {
                return ( CswFilePath.getConfigurationFilePath( _CswNbtResources.SetupVbls.SetupMode ) );
            }
        }

        public Int32 UpdtShellWaitMsec
        {
            get
            {
                Int32 ReturnVal = 30000;
                if( _CswNbtResources.SetupVbls.doesSettingExist( CswEnumSetupVariableNames.UpdtShellWaitMsec ) )
                {
                    ReturnVal = CswConvert.ToInt32( _CswNbtResources.SetupVbls.readSetting( CswEnumSetupVariableNames.UpdtShellWaitMsec ) );
                }

                return ( ReturnVal );
            }
        }


        /// <summary>
        /// Encapsulate data acces mechanics for schmema updater so that a schema treats transactions consistently
        /// and can accurately rollback to the previous data state. The idea is essentially that schema updater 
        /// should see everything in dbresources, meta data, and nodes _except_ for the methods that deal with 
        /// transaction control (begin-end transaction, finalize(), etc.) and let this class do that work.
        /// 
        /// </summary>
        /// <param name="CswNbtResources"></param>
        public CswNbtSchemaModTrnsctn( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtResources.TransactionMode = CswEnumTransactionMode.Atomistic;
            _CswDdl = new CswDDL( _CswNbtResources );
            //            _CswNbtSequenceManager = new CswNbtSequenceManager( _CswNbtResources );
        }//ctor

        /// <summary>
        /// Send a Schema Update error to the log.
        /// </summary>
        public void logError( string Message )
        {
            CswStatusMessage Msg = new CswStatusMessage
            {
                AppType = CswEnumAppType.SchemUpdt,
                ContentType = CswEnumContentType.Error
            };
            Msg.Attributes.Add( CswEnumLegalAttribute.exoteric_message, Message );
            CswLogger.send( Msg );
        }

        public ICswLogger CswLogger { get { return ( _CswNbtResources.CswLogger ); } }

        public CswNbtActInspectionDesignWiz getCswNbtActInspectionDesignWiz()
        {
            return ( new CswNbtActInspectionDesignWiz( _CswNbtResources, CswEnumNbtViewVisibility.Global, null, true ) );
        }

        private CswNbtLandingPage _LandingPage = null;
        /// <summary>
        /// A library of functions for adding, moving, modifying, and deleting LandingPage Items
        /// </summary>
        public CswNbtLandingPage LandingPage
        {
            get
            {
                if( _LandingPage == null && _CswNbtResources.IsInitializedForDbAccess )
                {
                    _LandingPage = new CswNbtLandingPage( _CswNbtResources );
                }
                return _LandingPage;
            }
        }

        #region TransactionManagement
        public void beginTransaction()
        {
            _CswNbtResources.CswResources.beginTransaction();
        }

        /// <summary>
        /// This removes the cache of CswTableCaddies from CswResources.  Consider using Rollback() if you are trying to revert changes.
        /// </summary>
        public void clearUpdates()
        {
            _CswNbtResources.CswResources.clearUpdates();
        }

        public void commitTransaction()
        {
            _CswDdl.confirm();
            _CswNbtResources.finalize( true );
            _CswDdl.clear();
        }

        public void rollbackTransaction()
        {
            _CswNbtResources.Rollback();
            _CswNbtResources.refreshDataDictionary();
            _CswDdl.revert();
            _CswDdl.clear();
        }

        public void refreshDataDictionary()
        {
            _CswNbtResources.refreshDataDictionary();
        }

        #endregion

        #region CswDbResources
        //These items here commented out are the ones that the schema update process should _not_ have access 
        //to
        //public bool InTransaction { }
        //public void beginTransaction()
        //public void commitTransaction()
        //public void rollbackTransaction()

        //public void setMetaData( DataTable DataTable, string TableName, string IncludeOnlyCols ) { _CswDataDictionaryInfo.setMetaData( DataTable, TableName, IncludeOnlyCols ); }//
        //public void setMetaData( ICswDmlOpLogic CswDmlOpLogic ) { _CswDataDictionaryInfo.setMetaData( CswDmlOpLogic ); }//setMetaData() 
        //        public IDbConnection getConnection() { return ( _CswResourcesForTableCaddy.getConnection() ); }
        //        public IDbConnection getConnection() { return ( _CswResourcesForTableCaddy.Connection ); }
        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public void addAdapter( string AdapterName ) { _CswResourcesForTableCaddy.addAdapter( AdapterName ); }
        //public bool isAdapterDefined( string AdapterName ) { return ( _CswResourcesForTableCaddy.isAdapterDefined<CswDataAdapter>( AdapterName ) ); }
        //public void setIsDeleteModeLogical( bool IsDeleteModeLogical ) { _CswResourcesForTableCaddy.setIsDeleteModeLogical( IsDeleteModeLogical ); }

        public bool isTableDefinedInMetaData( string TableName ) { return ( _CswNbtResources.CswResources.isTableDefinedInMetaData( TableName ) ); }
        public bool isColumnDefinedInMetaData( string TableName, string ColumnName ) { return ( _CswNbtResources.CswResources.isColumnDefinedInMetaData( TableName, ColumnName ) ); }

        public bool isTableDefinedInDataBase( string TableName ) { return ( _CswNbtResources.CswResources.isTableDefinedInDataBase( TableName ) ); }
        public bool isColumnDefinedInDataBase( string TableName, string ColumnName ) { return ( _CswNbtResources.CswResources.isColumnDefinedInDataBase( TableName, ColumnName ) ); }

        public bool isTableDefined( string TableName ) { return ( _CswNbtResources.CswResources.isTableDefined( TableName ) ); }
        public bool isColumnDefined( string TableName, string ColumnName ) { return ( _CswNbtResources.CswResources.isColumnDefined( TableName, ColumnName ) ); }


        //bz # 9116: Centralize access to dbresources for easier datacaching
        //public CswMetaDataReader CswMetaDataReader { get { return ( _CswNbtResources.CswResources.CswMetaDataReader ); } }
        public ICswDataDictionaryReader CswDataDictionary { get { return ( _CswNbtResources.CswResources.DataDictionary ); } }

        //limit usage of CswDbResources public interfaces as per bz # 9136
        //        public IDbConnection DbConnection { get { return ( _CswResourcesForTableCaddy.getConnection() ); } }
        //        public IDbConnection DbConnection { get { return ( _CswResourcesForTableCaddy.Connection ); } }
        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public bool isValid( ref string ErrorMesage ) { return ( _CswResourcesForTableCaddy.isValid( ref ErrorMesage ) ); }

        //bz # 9116: Centralize access to dbresources for easier datacaching
        //public CswDataAdapterColl<CswDataAdapter> DataAdapters { get { return ( _CswResourcesForTableCaddy.DataAdapters ); } }//DataAdapters
        //public CswDataAdapter DataAdapter { get { return ( _CswResourcesForTableCaddy.DataAdapter ); } }

        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public void loadNamedSelectSql( string AdapterName, string SelectSqlname ) { _CswResourcesForTableCaddy.loadNamedSelectSql( AdapterName, SelectSqlname ); }
        //public void loadNamedSelectSql( string SelectSqlname ) { _CswResourcesForTableCaddy.loadNamedSelectSql( SelectSqlname ); }

        // We should use CswArbitrarySelect instead of these.  1/25/2010 sds
        //public void setSqlSelectText( string SqlSelectText ) { _CswNbtResources.CswResources.setSqlSelectText( SqlSelectText ); }
        //public void setSqlSelectText( string AdapterName, string SqlSelectText ) { _CswNbtResources.CswResources.setSqlSelectText( AdapterName, SqlSelectText ); }
        //public void setSqlSelectText( string SqlSelectText, Int32 PageLowerBoundExclusive, Int32 PageUpperBoundInclusive ) { _CswNbtResources.CswResources.setSqlSelectText( SqlSelectText, PageLowerBoundExclusive, PageUpperBoundInclusive ); }
        //public void setSqlSelectText( string AdapterName, string SqlSelectText, Int32 PageLowerBoundExclusive, Int32 PageUpperBoundInclusive ) { _CswNbtResources.CswResources.setSqlSelectText( AdapterName, SqlSelectText, PageLowerBoundExclusive, PageUpperBoundInclusive ); }

        /// <summary>
        /// Executes arbitrary sql.  It's your job to make sure it's platform neutral.
        /// You should *strongly* consider using CswArbitrarySelect, CswTableSelect, or CswTableUpdate instead of this.
        /// </summary>
        /// <returns>Number of rows affected</returns>
        public Int32 execArbitraryPlatformNeutralSql( string SqlText ) { return _CswNbtResources.CswResources.execArbitraryPlatformNeutralSql( SqlText ); }


        /// <summary>
        /// This method executes the sql passed into it in a transaction that is separate from the 
        /// one that is controlled by the beginTransaction() and commitTransaction() methods.
        /// Moreover, the connection it uses is returned to the connection pool immediately. 
        /// It should be used only when you have a need to perform a sql operation that 
        /// is separate from the main transaction. This is almost always _not_ 
        /// what you want to do. Caveat emptor!
        /// </summary>
        /// <param name="SqlText">The SQL to be executed</param>
        /// <returns></returns>
        public Int32 execArbitraryPlatformNeutralSqlInItsOwnTransaction( string SqlText ) { return ( _CswNbtResources.CswResources.execArbitraryPlatformNeutralSqlInItsOwnTransaction( SqlText ) ); }

        /// <summary>
        /// Executes arbitrary sql.  It's your job to make sure it's platform neutral.
        /// You should *strongly* consider using CswArbitrarySelect, CswTableSelect, or CswTableUpdate instead of this.
        /// </summary>
        /// <returns>DataTable of results</returns>
        /// 
        public DataTable execArbitraryPlatformNeutralSqlSelect( string UniqueName, string SqlText ) { return _CswNbtResources.CswResources.execArbitraryPlatformNeutralSqlSelect( UniqueName, SqlText ); }

        //limit usage of CswDbResources public interfaces as per bz # 9136
        //        public string getSqlText( string AdapterName, SqlType SqlType ) { return ( _CswResourcesForTableCaddy.getSqlText( AdapterName, SqlType ) ); }// 
        public string getSqlText( CswEnumSqlType SqlType ) { return ( _CswNbtResources.CswResources.getSqlText( SqlType ) ); }//
        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public void setSqlDmlText( SqlType SqlType, string DmlTable, string SqlDmlText, bool LoadParamsFromSql ) { _CswResourcesForTableCaddy.setSqlDmlText( SqlType, DmlTable, SqlDmlText, LoadParamsFromSql ); }
        //        public void setSqlDmlText( string AdapterName, SqlType SqlType, string DmlTable, string SqlDmlText, bool LoadParamsFromSql ) { _CswResourcesForTableCaddy.setSqlDmlText( AdapterName, SqlType, DmlTable, SqlDmlText, LoadParamsFromSql ); }
        public void setParameterValue( CswEnumSqlType SqlType, CswStaticParam StaticParam ) { _CswNbtResources.CswResources.setParameterValue( SqlType, StaticParam ); }
        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public void setParameterValue( string AdapterName, ChemSW.RscAdo.SqlType SqlType, string parameterName, object val ) { _CswResourcesForTableCaddy.setParameterValue( AdapterName, SqlType, parameterName, val ); }
        //public void clearCommand( SqlType SqlType ) { _CswResourcesForTableCaddy.clearCommand( SqlType ); }
        //public void clearCommand( string AdapterName, SqlType SqlType ) { _CswResourcesForTableCaddy.clearCommand( AdapterName, SqlType ); }
        //        public ICswDbNativeDate getCswDbNativeDate() { return ( _CswResourcesForTableCaddy.getCswDbNativeDate() ); }// 
        //        public void prepareDmlOp( SqlType SqlType, DataTable DataTable ) { _CswResourcesForTableCaddy.prepareDmlOp( SqlType, DataTable ); }
        public void prepareDmlOp( string AdapterName, CswEnumSqlType SqlType, DataTable DataTable ) { _CswNbtResources.CswResources.prepareDmlOp( AdapterName, SqlType, DataTable ); }
        public string getPrimeKeyColName( string TableName ) { return ( _CswNbtResources.CswResources.getPrimeKeyColName( TableName ) ); }
        public int getNewPrimeKey( string TableName ) { return ( _CswNbtResources.CswResources.getNewPrimeKey( TableName ) ); }
        public bool doesS4Exist( string S4Name )
        {
            return ( _CswDdl.doesS4Exist( S4Name ) );
        }

        public bool IsDbLinkConnectionHealthy( string DbLink, ref string ErrorMessage ) { return ( _CswNbtResources.CswResources.IsDbLinkConnectionHealthy( DbLink, ref ErrorMessage ) ); }

        public void makeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, bool AddDdData )
        {
            _CswDdl.makeConstraint( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, AddDdData );
        }

        public void removeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, string ConstraintName )
        {
            _CswDdl.removeConstraint( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, ConstraintName );
        }

        public List<CswTableConstraint> getConstraints( string PkTableName, string PkColumnName, string FkTableName, string FkColumnName )
        {
            return ( _CswDdl.getConstraints( PkTableName, PkColumnName, FkTableName, FkColumnName ) );
        }

        public int purgeTableRecords( string TableName ) { return ( _CswNbtResources.CswResources.purgeTableRecords( TableName ) ); }//purgeTableRecords()
        public void copyTable( string SourceTableName, string CopyToTableName, bool TableIsTemporary = true ) { _CswNbtResources.CswResources.copyTable( SourceTableName, CopyToTableName, TableIsTemporary ); }//copyTable()

        public void addTable( string TableName, string PkColumnName )
        {
            _CswDdl.addTable( TableName, PkColumnName );
        }

        public void dropTable( string TableName ) { _CswDdl.dropTable( TableName ); }

        public void addColumn( string columnname, CswEnumDataDictionaryColumnType columntype, Int32 datatypesize, Int32 dblprecision,
                               string defaultvalue, string description, string foreignkeycolumn, string foreignkeytable, bool constrainfkref, bool isview,
                               string lowerrangevalue, bool lowerrangevalueinclusive, CswEnumDataDictionaryPortableDataType portabledatatype, bool ReadOnly,
                               bool Required, string tablename, CswEnumDataDictionaryUniqueType uniquetype, bool uperrangevalueinclusive, string upperrangevalue )
        {
            _CswDdl.addColumn( columnname, columntype, datatypesize, dblprecision,
                               defaultvalue, description, foreignkeycolumn, foreignkeytable, constrainfkref, isview,
                               lowerrangevalue, lowerrangevalueinclusive, portabledatatype, ReadOnly,
                               Required, tablename, uniquetype, uperrangevalueinclusive, upperrangevalue );
        }

        public void renameColumn( string TableName, string OriginalColumnName, string NewColumnName )
        {
            _CswDdl.renameColumn( TableName, OriginalColumnName, NewColumnName );
        }

        public void dropColumn( string TableName, string ColumnName ) { _CswDdl.dropColumn( TableName, ColumnName ); }
        public void changeColumnDataType( string TableName, string ColumnName, CswEnumDataDictionaryPortableDataType NewDataType, Int32 DataTypeSize ) { _CswNbtResources.CswResources.changeColumnDataType( TableName, ColumnName, NewDataType, DataTypeSize ); }

        public void indexColumn( string TableName, string ColumnName, string IndexNameIn = null ) { _CswNbtResources.CswResources.indexColumn( TableName, ColumnName, IndexNameIn ); }
        public void updateIndex( string TableName, string ColumnName, string IndexName ) { _CswNbtResources.CswResources.updateIndex( TableName, ColumnName, IndexName ); }

        public DataTable getAllViews() { return _CswNbtResources.ViewSelect.getAllViews(); }

        public string Accessid { get { return ( _CswNbtResources.AccessId ); } }
        public string ServerId { get { return ( _CswNbtResources.CswResources.ServerId ); } }
        public string UserName { get { return ( _CswNbtResources.CswResources.UserName ); } }
        #endregion

        #region Metadata, table, and Nodes
        public CswStaticSelect makeCswStaticSelect( string UniqueName, string QueryName ) { return ( _CswNbtResources.makeCswStaticSelect( UniqueName, QueryName ) ); }
        public CswArbitrarySelect makeCswArbitrarySelect( string UniqueName, string QueryText ) { return ( _CswNbtResources.makeCswArbitrarySelect( UniqueName, QueryText ) ); }
        public CswTableSelect makeCswTableSelect( string UniqueName, string TableName ) { return ( _CswNbtResources.makeCswTableSelect( UniqueName, TableName ) ); }
        public CswTableUpdate makeCswTableUpdate( string UniqueName, string TableName ) { return ( _CswNbtResources.makeCswTableUpdate( UniqueName, TableName ) ); }

        private CswNbtMetaDataForSchemaUpdater _CswNbtMetaDataForSchemaUpdater = null;
        /// <summary>
        /// A CswNbtMetaData collection for schema updater only
        /// </summary>
        public CswNbtMetaDataForSchemaUpdater MetaData
        {
            get
            {
                if( _CswNbtMetaDataForSchemaUpdater == null && _CswNbtResources.IsInitializedForDbAccess )
                {
                    _CswNbtMetaDataForSchemaUpdater = new CswNbtMetaDataForSchemaUpdater( _CswNbtResources, _CswNbtResources.MetaData._CswNbtMetaDataResources, this );
                    //_CswNbtResources.assignMetaDataEvents( _CswNbtMetaDataForSchemaUpdater );
                }
                return _CswNbtMetaDataForSchemaUpdater;
            }
        }

        public void makeMissingAuditTablesAndColumns()
        {
            DataTable DataTable = execArbitraryPlatformNeutralSqlSelect( "query for all datadatable names", "select distinct tablename from data_dictionary" );

            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                string CurrentTableName = CurrentRow["tablename"].ToString().ToLower();
                makeTableAuditable( CurrentTableName );
            }
        }

        public bool makeTableAuditable( string TableName )
        {
            bool shouldBeAudited = _CswAuditMetaData.shouldBeAudited( TableName );
            if( shouldBeAudited )
            {
                if( false == _CswNbtResources.CswResources.DataDictionary.isColumnDefined( TableName, _CswAuditMetaData.AuditLevelColName ) )
                {
                    addStringColumn( TableName, _CswAuditMetaData.AuditLevelColName, _CswAuditMetaData.AuditLevelColDescription, _CswAuditMetaData.AuditLevelColIsRequired, _CswAuditMetaData.AuditLevelColLength );
                }

                string AuditTableName = _CswAuditMetaData.makeAuditTableName( TableName );

                //create the audit table if necessary
                if( false == _CswNbtResources.CswResources.DataDictionary.isTableDefined( AuditTableName ) )
                {
                    copyTable( TableName, AuditTableName, false );
                    addStringColumn( AuditTableName, _CswAuditMetaData.AuditEventTypeColName, _CswAuditMetaData.AuditEventTypeColDescription, true, _CswAuditMetaData.AuditEventTypeColLength );
                    addForeignKeyColumn( AuditTableName, _CswAuditMetaData.AuditTransactionIdColName, "fk to audittransactions table", true, _CswAuditMetaData.AuditTransactionTableName, _CswAuditMetaData.AuditTransactionIdColName );
                    addDateColumn( AuditTableName, _CswAuditMetaData.AuditRecordCreatedColName, _CswAuditMetaData.AuditRecordCreatedColDescription, true );
                    addLongColumn( AuditTableName, _CswNbtResources.DataDictionary.getPrimeKeyColumn( TableName ), "prime key of audited record", true );

                }
                else //if it does exist, maybe the target table has new columns to be added to the audit table?
                {
                    string[] TargetTableColumnNameArray = new string[_CswNbtResources.DataDictionary.getColumnNames( TableName ).Count];
                    _CswNbtResources.DataDictionary.getColumnNames( TableName ).CopyTo( TargetTableColumnNameArray, 0 );
                    List<string> TargetColumnNames = new List<string>( TargetTableColumnNameArray );

                    string[] AuditTableColumnNameArray = new string[_CswNbtResources.DataDictionary.getColumnNames( AuditTableName ).Count];
                    _CswNbtResources.DataDictionary.getColumnNames( AuditTableName ).CopyTo( AuditTableColumnNameArray, 0 );
                    List<string> AuditColumnNames = new List<string>( AuditTableColumnNameArray );

                    List<string> MissingAuditTableColumnNames = new List<string>();

                    foreach( string CurrentTargetColumnName in TargetColumnNames )
                    {
                        if( ( _CswAuditMetaData.AuditLevelColName != CurrentTargetColumnName.ToLower() ) &&
                            ( false == AuditColumnNames.Contains( CurrentTargetColumnName ) ) )
                        {
                            MissingAuditTableColumnNames.Add( CurrentTargetColumnName );
                        }
                    }

                    if( MissingAuditTableColumnNames.Count > 0 )
                    {
                        foreach( string CurrentMissingColumnName in MissingAuditTableColumnNames )
                        {
                            _CswNbtResources.DataDictionary.setCurrentColumn( TableName, CurrentMissingColumnName );
                            addColumn( CurrentMissingColumnName, _CswNbtResources.DataDictionary.ColumnType, _CswNbtResources.DataDictionary.DataTypeSize, _CswNbtResources.DataDictionary.DblPrecision, _CswNbtResources.DataDictionary.DefaultValue, _CswNbtResources.DataDictionary.Description, _CswNbtResources.DataDictionary.ForeignKeyColumn, _CswNbtResources.DataDictionary.ForeignKeyTable, false, _CswNbtResources.DataDictionary.IsView, _CswNbtResources.DataDictionary.LowerRangeValue, _CswNbtResources.DataDictionary.LowerRangeValueInclusive, _CswNbtResources.DataDictionary.PortableDataType, _CswNbtResources.DataDictionary.ReadOnly, _CswNbtResources.DataDictionary.Required, AuditTableName, _CswNbtResources.DataDictionary.UniqueType, _CswNbtResources.DataDictionary.UpperRangeValueInclusive, _CswNbtResources.DataDictionary.UpperRangeValue );
                        }

                    }//if the audit table is missing columns                                                                  

                }//if-else the audit table did not yet exist

            }//if-else it's an audited table

            return shouldBeAudited;
        }//makeTableAuditable() 

        public void makeTableNotAuditable( string TableName )
        {
            if( _CswNbtResources.CswResources.isColumnDefined( TableName, _CswAuditMetaData.AuditLevelColName ) )
            {
                dropColumn( TableName, _CswAuditMetaData.AuditLevelColName );
            }

            string AuditTableName = _CswAuditMetaData.makeAuditTableName( TableName );

            if( _CswNbtResources.CswResources.isTableDefined( AuditTableName ) )
            {
                dropTable( AuditTableName );
            }//if the audit table does not yet exist

        }//makeTableAuditable() 

        public bool isTableAuditable( string TableName ) { return ( _CswNbtResources.CswResources.isTableAuditable( TableName ) ); }

        public void setTableAuditLevel( string TableName, string WhereClause, CswEnumAuditLevel AuditLevel )
        {
            if( isTableAuditable( TableName ) )
            {
                CswTableUpdate CswTableUpdate = makeCswTableUpdate( "setTableAuditLevel", TableName );

                DataTable DataTable;

                if( string.Empty == WhereClause )
                {
                    DataTable = CswTableUpdate.getTable();
                }
                else
                {
                    DataTable = CswTableUpdate.getTable( WhereClause );
                }//if-else we have a where clause

                foreach( DataRow CurrentRow in DataTable.Rows )
                {
                    CurrentRow[_CswAuditMetaData.AuditLevelColName] = AuditLevel;
                }

                CswTableUpdate.update( DataTable );

            }//if table is auditable

        }//setTableAuditLevel() 

        public CswNbtNodeCollection Nodes { get { return ( _CswNbtResources.Nodes ); } }
        public CswNbtActionCollection Actions { get { return _CswNbtResources.Actions; } }

        /// <summary>
        /// For making ObjClass files without a Node
        /// </summary>
        public CswNbtObjClass makeObjClass( CswNbtMetaDataObjectClass ObjectClass )
        {
            return CswNbtObjClassFactory.makeObjClass( _CswNbtResources, ObjectClass );
        }

        public CswNbtViewSelect ViewSelect { get { return _CswNbtResources.ViewSelect; } }

        /// <summary>
        /// Returns a new CswNbtView object. Does not save the view
        /// </summary>
        public CswNbtView makeView() { return ( new CswNbtView( _CswNbtResources ) ); }

        /// <summary>
        /// Creates and persists a new CswNbtView object.
        /// STRONGLY RECOMMEND USING makeSafeView() if you think this view may conflict with existing views in the system
        /// </summary>
        public CswNbtView makeNewView( string ViewName, CswEnumNbtViewVisibility Visibility, CswPrimaryKey RoleId = null, CswPrimaryKey UserId = null, Int32 CopyViewId = Int32.MinValue )
        {
            CswNbtView Ret = new CswNbtView( _CswNbtResources );
            Ret.saveNew( ViewName, Visibility, RoleId, UserId, CopyViewId );
            return Ret;
        }
        public CswNbtView restoreView( CswNbtViewId ViewId ) { return ViewSelect.restoreView( ViewId ); }
        public CswNbtView restoreViewString( string ViewAsString ) { return ViewSelect.restoreView( ViewAsString ); }
        public CswNbtView restoreView( string ViewName, CswEnumNbtViewVisibility Visibility = null ) { return ViewSelect.restoreView( ViewName, Visibility ); }

        /// <summary>
        /// Creates a new view and clears/deletes any existing views that violate uniqueness with the new view
        /// </summary>
        public CswNbtView makeSafeView( string ViewName, CswEnumNbtViewVisibility Visibility, CswPrimaryKey RoleId = null, CswPrimaryKey UserId = null, Int32 CopyViewId = Int32.MinValue )
        {
            CswNbtView Ret = ViewSelect.restoreView( ViewName, Visibility );
            if( null == Ret && ( Visibility == CswEnumNbtViewVisibility.Role || Visibility == CswEnumNbtViewVisibility.User ) )
            {
                //If we're making a new Role/User view, override any existing global views that match
                Ret = ViewSelect.restoreView( ViewName, CswEnumNbtViewVisibility.Global );
            }
            if( null != Ret )
            {
                Ret.Root.ChildRelationships.Clear();
            }
            else
            {
                if( Visibility == CswEnumNbtViewVisibility.Global )
                {
                    //If we're making a new Global view, delete any existing Role/User views that match
                    List<CswNbtView> MatchingViews = ViewSelect.restoreViews( ViewName );
                    foreach( CswNbtView MatchingView in MatchingViews )
                    {
                        if( MatchingView.Visibility == CswEnumNbtViewVisibility.Role || MatchingView.Visibility == CswEnumNbtViewVisibility.User )
                        {
                            MatchingView.Delete();
                        }
                    }
                }
                Ret = new CswNbtView( _CswNbtResources );
                Ret.saveNew( ViewName, Visibility, RoleId, UserId, CopyViewId );
            }
            return Ret;
        }

        public ICswNbtTree getTreeFromView( CswNbtView View, bool IncludeSystemNodes ) { return _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, IncludeSystemNodes, false ); }
        public List<CswNbtView> restoreViews( string ViewName )
        {
            List<CswNbtView> ReturnVal = new List<CswNbtView>();

            CswTableSelect ViewSelect = makeCswTableSelect( "SchemaModTrnsctn_restoreViews_select", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( "nodeviewid" );
            DataTable ViewTable = ViewSelect.getTable( SelectCols, string.Empty, Int32.MinValue, " where viewname='" + ViewName + "'", false );
            foreach( DataRow CurrentRow in ViewTable.Rows )
            {
                ReturnVal.Add( _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) ) );
            }

            return ( ReturnVal );

        }//restoreViews()

        /// <summary>
        /// Restore all views matching ViewMode
        /// </summary>
        public List<CswNbtView> restoreAllViewsOfMode( CswEnumNbtViewRenderingMode ViewMode )
        {
            List<CswNbtView> ReturnVal = new List<CswNbtView>();

            CswTableSelect ViewSelect = makeCswTableSelect( "SchemaModTrnsctn_restoreViews_select", "node_views" );
            CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
            SelectCols.Add( "nodeviewid" );
            DataTable ViewTable = ViewSelect.getTable( SelectCols, string.Empty, Int32.MinValue, " where viewmode='" + ViewMode + "'", false );
            foreach( DataRow CurrentRow in ViewTable.Rows )
            {
                ReturnVal.Add( _CswNbtResources.ViewSelect.restoreView( new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) ) ) );
            }

            return ( ReturnVal );

        }//restoreAllViewsOfMode()

        public void deleteView( string ViewName, bool DeleteAllInstances )
        {
            List<CswNbtView> Views = restoreViews( ViewName );

            if( !DeleteAllInstances && Views.Count > 1 )
                throw ( new CswDniException( "There is more than one instance of the specified view: " + ViewName ) );

            foreach( CswNbtView CurrentView in Views )
                CurrentView.Delete();

        }//deleteView()

        public CswNbtPermit Permit { get { return _CswNbtResources.Permit; } }

        /// <summary>
        /// Convenience function for making new Action
        /// </summary>
        public Int32 createAction( CswEnumNbtActionName Name, bool ShowInList, string URL, string Category )
        {
            // Create the Action
            CswTableUpdate ActionsTable = makeCswTableUpdate( "SchemaModTrnsctn_ActionUpdate", "actions" );
            DataTable ActionsDataTable = ActionsTable.getEmptyTable();
            DataRow ActionRow = ActionsDataTable.NewRow();
            ActionRow["actionname"] = CswNbtAction.ActionNameEnumToString( Name );
            ActionRow["showinlist"] = CswConvert.ToDbVal( ShowInList ); //Probably needs to be off by default.  Leaving on for development.
            ActionRow["url"] = URL;
            ActionRow["category"] = Category;
            ActionsDataTable.Rows.Add( ActionRow );
            Int32 NewActionId = CswConvert.ToInt32( ActionRow["actionid"] );
            ActionsTable.update( ActionsDataTable );

            // Grant permission to Administrator
            CswNbtNode RoleNode = Nodes.makeRoleNodeFromRoleName( "Administrator" );
            if( RoleNode != null )
            {
                _CswNbtResources.Permit.set( Name, (CswNbtObjClassRole) RoleNode, true );
            }
            CswNbtNode RoleNode2 = Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            if( RoleNode2 != null )
            {
                _CswNbtResources.Permit.set( Name, (CswNbtObjClassRole) RoleNode2, true );
            }
            _CswNbtResources.ClearActionsCache();
            return NewActionId;
        } // createAction()

        public void deleteAction( string ActionName )
        {
            // Remove from jct_modules_actions
            CswTableUpdate JctUpdate = this.makeCswTableUpdate( "28562D_Jct_Update", "jct_modules_actions" );
            DataTable JctTable = JctUpdate.getTable( "where actionid in (select actionid from actions where actionname = '" + ActionName + "')" );
            if( JctTable.Rows.Count > 0 )
            {
                foreach( DataRow JctRow in JctTable.Rows )
                {
                    JctRow.Delete();
                }
            }

            // Remove from actions
            CswTableUpdate ActionUpdate = this.makeCswTableUpdate( "28562D_Action_Update", "actions" );
            DataTable ActionTable = ActionUpdate.getTable( "where actionname = '" + ActionName + "'" );
            if( ActionTable.Rows.Count > 0 )
            {
                foreach( DataRow ActionRow in ActionTable.Rows )
                {
                    ActionRow.Delete();
                }
            }

            // Fix action permissions
            CswNbtMetaDataObjectClass RoleOC = this.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtObjClassRole Role in RoleOC.getNodes( false, true ) )
            {
                if( Role.ActionPermissions.CheckValue( ActionName ) )
                {
                    Role.ActionPermissions.RemoveValue( ActionName );
                    Role.postChanges( false );
                }
            }
        } // deleteAction()

        /// <summary>
        /// Convenience function for making new Configuration Variable
        /// </summary>
        public void createConfigurationVariable( CswEnumConfigurationVariableNames Name, string Description, string VariableValue, bool IsSystem )
        {
            // Create the Configuration Variable
            if( Name != CswEnumConfigurationVariableNames.Unknown )
            {
                createConfigurationVariable( Name.ToString().ToLower(), Description, VariableValue, IsSystem );
            }
        }

        /// <summary>
        /// Convenience function for making new Configuration Variable
        /// </summary>
        public void createConfigurationVariable( CswEnumNbtConfigurationVariables Name, string Description, string VariableValue, bool IsSystem )
        {
            // Create the Configuration Variable
            if( Name != CswEnumNbtConfigurationVariables.unknown )
            {
                createConfigurationVariable( Name.ToString().ToLower(), Description, VariableValue, IsSystem );
            }
        }

        /// <summary>
        /// Convenience function for making new Configuration Variable
        /// </summary>
        public void createConfigurationVariable( String Name, string Description, string VariableValue, bool IsSystem )
        {
            // Create the Configuration Variable
            CswTableUpdate ConfigVarTable = makeCswTableUpdate( "SchemaModTrnsctn_ConfigVarUpdate", "configuration_variables" );
            DataTable ConfigVarDataTable = ConfigVarTable.getEmptyTable();
            DataRow ConfigVarRow = ConfigVarDataTable.NewRow();
            ConfigVarRow["variablename"] = Name.ToLower();
            ConfigVarRow["description"] = Description;
            ConfigVarRow["variablevalue"] = VariableValue;
            ConfigVarRow["issystem"] = CswConvert.ToDbVal( IsSystem );
            ConfigVarDataTable.Rows.Add( ConfigVarRow );
            ConfigVarTable.update( ConfigVarDataTable );
        }

        /// <summary>
        /// Convenience function for deleting a Configuration Variable
        /// </summary>
        public void deleteConfigurationVariable( CswEnumNbtConfigurationVariables Name )
        {
            deleteConfigurationVariable( Name.ToString() );
        }

        /// <summary>
        /// Convenience function for deleting a Configuration Variable
        /// </summary>
        /// <param name="Name"></param>
        public void deleteConfigurationVariable( String Name )
        {
            CswTableUpdate ConfigVarTable = makeCswTableUpdate( "SchemaModTrnsctn_ConfigVarUpdate", "configuration_variables" );
            DataTable ConfigVarDataTable = ConfigVarTable.getTable( "where lower(variablename)='" + Name.ToLower() + "'", true );
            foreach( DataRow Row in ConfigVarDataTable.Rows )
            {
                Row.Delete();
            }
            ConfigVarTable.update( ConfigVarDataTable );
        }

        public Int32 getActionId( CswEnumNbtActionName ActionName )
        {
            Int32 RetActionId = Int32.MinValue;
            if( null != Actions[ActionName] )
            {
                RetActionId = Actions[ActionName].ActionId;
            }
            return RetActionId;
        }

        #region Create Junctions
        /// <summary>
        /// Convenience function for making new jct_module_actions records
        /// </summary>
        public void createFieldTypesSubFieldsJunction( CswNbtMetaDataFieldType FieldType, CswEnumNbtPropColumn Column,
            CswEnumNbtSubFieldName SubField, bool IsReportable, bool IsDefault = false )
        {
            CswTableUpdate JctFtSfUpdate = makeCswTableUpdate( "SchemaModTrnsctn_FieldTypeSubFieldJunction", "field_types_subfields" );
            DataTable UpdateAsDataTable = JctFtSfUpdate.getEmptyTable();
            DataRow JctRow = UpdateAsDataTable.NewRow();
            JctRow["fieldtypeid"] = FieldType.FieldTypeId;
            JctRow["propcolname"] = Column.ToString();
            JctRow["subfieldname"] = SubField.ToString().ToLower();
            JctRow["reportable"] = CswConvert.ToDbVal( IsReportable );
            JctRow["is_default"] = CswConvert.ToDbVal( IsDefault );
            UpdateAsDataTable.Rows.Add( JctRow );
            JctFtSfUpdate.update( UpdateAsDataTable );
        }

        public void createModuleActionJunction( CswEnumNbtModuleName Module, CswEnumNbtActionName ActionName )
        {
            Int32 ModuleId = Modules.GetModuleId( Module );
            Int32 ActionId = getActionId( ActionName );
            createModuleActionJunction( ModuleId, ActionId );
        }

        /// <summary>
        /// Convenience function for making new jct_module_actions records
        /// </summary>
        public void createModuleActionJunction( Int32 ModuleId, Int32 ActionId )
        {
            CswTableUpdate JctModulesATable = makeCswTableUpdate( "SchemaModTrnsctn_ModuleJunctionUpdate", "jct_modules_actions" );
            DataTable JctModulesADataTable = JctModulesATable.getEmptyTable();
            DataRow JctRow = JctModulesADataTable.NewRow();
            JctRow["actionid"] = ActionId.ToString();
            JctRow["moduleid"] = ModuleId.ToString();
            JctModulesADataTable.Rows.Add( JctRow );
            JctModulesATable.update( JctModulesADataTable );
        }

        /// <summary>
        /// Convenience function for making new jct_module_objectclass records
        /// </summary>
        public void createModuleObjectClassJunction( CswEnumNbtModuleName Module, Int32 ObjectClassId )
        {
            Int32 ModuleId = Modules.GetModuleId( Module );
            createModuleObjectClassJunction( ModuleId, ObjectClassId );
        }

        /// <summary>
        /// Convenience function for making new jct_module_objectclass records
        /// </summary>
        public void createModuleObjectClassJunction( Int32 ModuleId, Int32 ObjectClassId )
        {
            CswTableUpdate JctModulesOCTable = makeCswTableUpdate( "SchemaModTrnsctn_ModuleJunctionUpdate", "jct_modules_objectclass" );
            DataTable JctModulesOCDataTable = JctModulesOCTable.getEmptyTable();
            DataRow JctRow = JctModulesOCDataTable.NewRow();
            JctRow["deleted"] = CswConvert.ToDbVal( false );
            JctRow["moduleid"] = ModuleId.ToString();
            JctRow["objectclassid"] = ObjectClassId.ToString();
            JctModulesOCDataTable.Rows.Add( JctRow );
            JctModulesOCTable.update( JctModulesOCDataTable );
        }

        /// <summary>
        /// Convenience function for making new jct_module_nodetype records
        /// </summary>
        public void createModuleNodeTypeJunction( CswEnumNbtModuleName Module, Int32 NodeTypeId )
        {
            Int32 ModuleId = Modules.GetModuleId( Module );
            createModuleNodeTypeJunction( ModuleId, NodeTypeId );
        }

        /// <summary>
        /// Convenience function for making new jct_module_nodetypes records
        /// </summary>
        public void createModuleNodeTypeJunction( Int32 ModuleId, Int32 NodeTypeId )
        {
            CswTableUpdate JctModulesNTTable = makeCswTableUpdate( "SchemaModTrnsctn_ModuleJunctionUpdate", "jct_modules_nodetypes" );
            DataTable JctModulesNTDataTable = JctModulesNTTable.getEmptyTable();
            DataRow JctRow = JctModulesNTDataTable.NewRow();
            JctRow["moduleid"] = ModuleId.ToString();
            JctRow["nodetypeid"] = NodeTypeId.ToString();
            JctModulesNTDataTable.Rows.Add( JctRow );
            JctModulesNTTable.update( JctModulesNTDataTable );
        }

        #endregion Create Junctions

        #region Change Junctions


        private void _changeJunctionModuleId( Int32 ChangeModuleId, Int32 ToModuleId, string TableName, string Fk )
        {
            CswCommaDelimitedString FksAlreadyOnToModuleId = new CswCommaDelimitedString();
            CswTableSelect JctModSelect = makeCswTableSelect( "SchemaModTrnsctn_ModuleJunctionUpdate_" + TableName + "_select", TableName );
            DataTable JctModSelectTable = JctModSelect.getTable( "moduleid", ToModuleId );
            foreach( DataRow JctRow in JctModSelectTable.Rows )
            {
                FksAlreadyOnToModuleId.Add( CswConvert.ToString( JctRow[Fk] ) );
            }

            CswTableUpdate JctModUpdate = makeCswTableUpdate( "SchemaModTrnsctn_ModuleJunctionUpdate_" + TableName + "_update", TableName );
            DataTable JctModUpdateTable = JctModUpdate.getTable( "moduleid", ChangeModuleId );
            foreach( DataRow JctRow in JctModUpdateTable.Rows )
            {
                string FkId = CswConvert.ToString( JctRow[Fk] );
                if( false == FksAlreadyOnToModuleId.Contains( FkId ) )
                {
                    JctRow["moduleid"] = CswConvert.ToDbVal( ToModuleId );
                }
                else
                {
                    JctRow.Delete();
                }
            }
            JctModUpdate.update( JctModUpdateTable );
        }

        /// <summary>
        /// Dereferences a moduleid from the appropriate jct tables and replaces with a new moduleid when necessary.
        /// </summary>
        public void changeJunctionModuleId( Int32 OldModuleId, Int32 NewModuleId )
        {
            _changeJunctionModuleId( OldModuleId, NewModuleId, "jct_modules_actions", "actionid" );
            _changeJunctionModuleId( OldModuleId, NewModuleId, "jct_modules_nodetypes", "nodetypeid" );
            _changeJunctionModuleId( OldModuleId, NewModuleId, "jct_modules_objectclass", "objectclassid" );
        }

        #endregion  Change Junctions

        #region Getters

        /// <summary>
        /// For manipulating modules
        /// </summary>
        public CswNbtModuleManager Modules
        {
            get
            {
                return _CswNbtResources.Modules;
            }
        }

        /// <summary>
        /// Convenience function for getting Object Class ID by name (usually for the purpose of deleting because the Enum has been removed)
        /// </summary>
        public Int32 getObjectClassId( string ObjectClassName )
        {
            Int32 Ret = Int32.MinValue;
            string OcName = ObjectClassName.ToLower();
            if( false == OcName.EndsWith( "class" ) )
            {
                OcName += "class";
            }

            CswTableSelect ObjectClassTableSelect = makeCswTableSelect( "SchemaModTrnsctn_ObjectClassUpdate", "object_class" );
            DataTable ObjectClassTable = ObjectClassTableSelect.getTable( "where lower(objectclass)='" + OcName + "'", true );
            if( ObjectClassTable.Rows.Count == 1 )
            {
                Ret = CswConvert.ToInt32( ObjectClassTable.Rows[0]["objectclassid"] );
            }

            return Ret;
        }

        #endregion Getters

        #region Create Schema/Meta Data

        /// <summary>
        /// Convenience function for making new Module. 
        /// Default behavior for enabled: true if master, false otherwise
        /// </summary>
        public Int32 createModule( string Description, string Name )
        {
            return createModule( Description, Name, Enabled: isMaster() );
        }

        /// <summary>
        /// Convenience function for making new Module
        /// </summary>
        public Int32 createModule( string Description, string Name, bool Enabled )
        {
            CswTableUpdate ModulesTable = makeCswTableUpdate( "SchemaModTrnsctn_ModuleUpdate", "modules" );
            DataTable ModulesDataTable = ModulesTable.getEmptyTable();
            DataRow ModuleRow = ModulesDataTable.NewRow();
            ModuleRow["deleted"] = CswConvert.ToDbVal( false );
            ModuleRow["description"] = Description;
            ModuleRow["name"] = Name;
            ModuleRow["enabled"] = CswConvert.ToDbVal( Enabled );
            ModulesDataTable.Rows.Add( ModuleRow );
            Int32 NewModuleId = CswConvert.ToInt32( ModuleRow["moduleid"] );
            ModulesTable.update( ModulesDataTable );
            _CswNbtResources.Modules.ClearModulesCache();
            return NewModuleId;
        }

        /// <summary>
        /// Convenience function for making new Scheduled Rule
        /// </summary>
        public Int32 createScheduledRule( CswEnumNbtScheduleRuleNames RuleName, CswEnumRecurrence Recurrence, Int32 Interval )
        {
            Int32 RetRuleId = Int32.MinValue;
            if( null != Recurrence &&
                Recurrence != CswNbtResources.UnknownEnum &&
                CswNbtResources.UnknownEnum != RuleName )
            {
                //TODO - Come back some day and make this dundant-proof
                //if we ever have to shift scripts around to accomodate DDL, these helper methods will not be so helpful
                CswTableUpdate RulesUpdate = makeCswTableUpdate( "SchemaModTrnsctn_ScheduledRuleUpdate", "scheduledrules" );
                DataTable RuleTable = RulesUpdate.getTable( WhereClause: " where lower(rulename)='" + RuleName.ToString().ToLower() + "' " );
                if( 0 == RuleTable.Rows.Count )
                {
                    DataRow NewRuleRow = RuleTable.NewRow();
                    NewRuleRow["recurrence"] = CswConvert.ToDbVal( Recurrence.ToString() );
                    NewRuleRow["interval"] = CswConvert.ToDbVal( Interval );
                    NewRuleRow["maxruntimems"] = CswConvert.ToDbVal( 300000 );
                    NewRuleRow["reprobatethreshold"] = CswConvert.ToDbVal( 3 );
                    NewRuleRow["disabled"] = CswConvert.ToDbVal( false );
                    NewRuleRow["rulename"] = CswConvert.ToDbVal( RuleName.ToString() );
                    RuleTable.Rows.Add( NewRuleRow );

                    RetRuleId = CswConvert.ToInt32( NewRuleRow["scheduledruleid"] );
                    RulesUpdate.update( RuleTable );
                }
                else
                {
                    this.logError( "Scheduled Rule name {" + RuleName + "} already exists." );
                }
            }
            return RetRuleId;
        }

        /// <summary>
        /// Convenience function for making new Object Classes
        /// </summary>
        public CswNbtMetaDataObjectClass createObjectClass( CswEnumNbtObjectClass ObjectClass, string IconFileName, bool AuditLevel )
        {
            if( ObjectClass == CswNbtResources.UnknownEnum )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot create an ObjectClass of Unknown.", "The provided Object Class name was not defined." );
            }
            CswNbtMetaDataObjectClass NewObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClass );
            if( null == NewObjectClass )
            {
                CswTableUpdate ObjectClassTableUpdate = makeCswTableUpdate( "SchemaModTrnsctn_ObjectClassUpdate",
                                                                            "object_class" );
                DataTable NewObjectClassTable = ObjectClassTableUpdate.getEmptyTable();
                DataRow NewOCRow = NewObjectClassTable.NewRow();
                NewOCRow["objectclass"] = ObjectClass.ToString();
                NewOCRow["iconfilename"] = IconFileName;
                NewOCRow["auditlevel"] = CswConvert.ToDbVal( AuditLevel );
                NewOCRow["nodecount"] = 0;

                NewOCRow["oraviewname"] = CswFormat.MakeOracleCompliantIdentifier( ObjectClass.ToString() );

                NewObjectClassTable.Rows.Add( NewOCRow );
                Int32 NewObjectClassId = CswConvert.ToInt32( NewOCRow["objectclassid"] );
                ObjectClassTableUpdate.update( NewObjectClassTable );
                NewObjectClass = _CswNbtResources.MetaData.getObjectClass( NewObjectClassId );

                // Case 27923
                createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NewObjectClass )
                {
                    PropName = CswNbtObjClass.PropertyName.Save,
                    FieldType = CswEnumNbtFieldType.Button,
                    Extended = CswNbtNodePropButton.ButtonMode.button
                } );

                // Case 30813
                createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NewObjectClass )
                {
                    PropName = CswNbtObjClass.PropertyName.LegacyId,
                    FieldType = CswEnumNbtFieldType.Text,
                    ServerManaged = true
                } );

            }
            return NewObjectClass;
        }

        /// <summary>
        /// Convenience wrapper for creating an Object Class Prop
        /// </summary>
        public CswNbtMetaDataObjectClassProp createObjectClassProp( CswNbtMetaDataObjectClass ObjectClass, CswNbtWcfMetaDataModel.ObjectClassProp OcpModel )
        {
            OcpModel.ObjectClass = ObjectClass;
            return createObjectClassProp( OcpModel );
        }

        /// <summary>
        /// Convenience wrapper for creating an Object Class Prop
        /// </summary>
        public CswNbtMetaDataObjectClassProp createObjectClassProp( CswNbtWcfMetaDataModel.ObjectClassProp OcpModel )
        {
            CswNbtMetaDataObjectClassProp RetProp = null;
            if( null != OcpModel.ObjectClass && OcpModel.ObjectClass.ObjectClass != CswNbtResources.UnknownEnum )
            {
                RetProp = OcpModel.ObjectClass.getObjectClassProp( OcpModel.PropName );
                if( null == RetProp )
                {
                    CswTableUpdate ObjectClassPropUpdate = makeCswTableUpdate( "SchemaModTrnsctn_ObjectClassUpdate", "object_class_props" );
                    DataTable UpdateTable = ObjectClassPropUpdate.getEmptyTable();
                    _addObjectClassPropRow( UpdateTable,
                                            OcpModel );

                    ObjectClassPropUpdate.update( UpdateTable );
                    MetaData.refreshAll();
                    RetProp = OcpModel.ObjectClass.getObjectClassProp( OcpModel.PropName );
                }
            }
            return RetProp;
        }

        #endregion Create Schema/Meta Data

        #region Delete Schema/Meta Data

        public void deleteModule( string ModuleName )
        {
            Int32 ModuleId = Modules.GetModuleId( ModuleName );
            deleteModuleNodeTypeJunction( ModuleId, NodeTypeId: Int32.MinValue );
            deleteAllModuleObjectClassJunctions( ModuleId );

            CswTableUpdate ModulesTU = makeCswTableUpdate( "SchemaModTrnsctn_DeleteModuleNTJunction", "modules" );
            DataTable ModulesDT = ModulesTU.getTable( "where moduleid = " + ModuleId );

            if( 1 == ModulesDT.Rows.Count ) //There can only be one Module/id
            {
                ModulesDT.Rows[0].Delete();
            }
            ModulesTU.update( ModulesDT );
        }

        #endregion Delete Schema/Meta Data

        #region Delete Junctions

        /// <summary>
        /// Delete all Module junctions on either this Module, this ActionName or both
        /// </summary>
        public void deleteModuleActionJunction( CswEnumNbtModuleName Module, CswEnumNbtActionName Action )
        {
            Int32 ModuleId = Modules.GetModuleId( Module );
            Int32 ActionId = getActionId( Action );
            deleteModuleActionJunction( ModuleId, ActionId );
        }

        /// <summary>
        /// Delete all Module junctions on either this ModuleId, this ActionId or both
        /// </summary>
        public void deleteModuleActionJunction( Int32 ModuleId, Int32 ActionId )
        {
            if( Int32.MinValue != ModuleId && Int32.MinValue != ActionId )
            {
                CswTableUpdate jct_modules_actionTU = makeCswTableUpdate( "SchemaModTrnsctn_DeleteModuleActionJunction", "jct_modules_actions" );
                string WhereSql = "";
                if( Int32.MinValue != ModuleId )
                {
                    WhereSql = " moduleid = " + ModuleId;
                }
                if( WhereSql.Length > 0 )
                {
                    WhereSql += " and ";
                }
                if( Int32.MinValue != ActionId )
                {
                    WhereSql += " actionid = " + ActionId;
                }
                DataTable jct_modules_actionDT = jct_modules_actionTU.getTable( "where " + WhereSql );
                Int32 RowCount = jct_modules_actionDT.Rows.Count;
                for( Int32 R = 0; R < RowCount; R += 1 )
                {
                    jct_modules_actionDT.Rows[R].Delete();
                }
                jct_modules_actionTU.update( jct_modules_actionDT );
            }
        }

        public void deleteModuleNodeTypeJunction( CswEnumNbtModuleName Module, Int32 NodeTypeId )
        {
            Int32 ModuleId = Modules.GetModuleId( Module );
            deleteModuleNodeTypeJunction( ModuleId, NodeTypeId );
        }

        /// <summary>
        /// Delete all Module junctions on either this ModuleId, this NodeTypeId or both
        /// </summary>
        public void deleteModuleNodeTypeJunction( Int32 ModuleId, Int32 NodeTypeId )
        {
            _deleteModuleJunction( ModuleId, NodeTypeId, false );
        }

        public void deleteModuleObjectClassJunction( CswEnumNbtModuleName Module, Int32 ObjectClassId )
        {
            Int32 ModuleId = Modules.GetModuleId( Module );
            _deleteModuleJunction( ModuleId, ObjectClassId, true );
        }

        public void deleteModuleObjectClassJunction( Int32 ModuleId, Int32 ObjectClassId )
        {
            _deleteModuleJunction( ModuleId, ObjectClassId, true );
        }

        private void _deleteModuleJunction( Int32 ModuleId, Int32 ItemId, bool IsObjClassId )
        {
            if( Int32.MinValue != ModuleId && Int32.MinValue != ItemId )
            {
                string tableName = IsObjClassId ? "jct_modules_objectclass" : "jct_modules_nodetypes";
                CswTableUpdate jct_modules_TU = makeCswTableUpdate( "SchemaModTrnsctn_DeleteModuleJunction", tableName );
                string WhereSql = "";
                if( Int32.MinValue != ModuleId )
                {
                    WhereSql = " moduleid = " + ModuleId;
                }
                if( WhereSql.Length > 0 )
                {
                    WhereSql += " and ";
                }
                if( Int32.MinValue != ItemId )
                {
                    string colname = IsObjClassId ? "objectclassid" : "nodetypeid";
                    WhereSql += colname + " = " + ItemId;
                }
                DataTable jct_modules_DT = jct_modules_TU.getTable( "where " + WhereSql );
                Int32 RowCount = jct_modules_DT.Rows.Count;
                for( Int32 R = 0; R < RowCount; R += 1 )
                {
                    jct_modules_DT.Rows[R].Delete();
                }
                jct_modules_TU.update( jct_modules_DT );
            }
        }

        /// <summary>
        /// Delete all Modules tied to this Object Class or any of its Node Types
        /// </summary>
        public void deleteAllModuleObjectClassJunctions( CswNbtMetaDataObjectClass ObjectClass )
        {
            foreach( Int32 NodeTypeId in ObjectClass.getNodeTypeIds().Keys )
            {
                deleteModuleNodeTypeJunction( ModuleId: Int32.MinValue, NodeTypeId: NodeTypeId );
            }

            CswTableUpdate jct_modules_objectclassTU = makeCswTableUpdate( "SchemaModTrnsctn_DeleteAllModuleOCJunction", "jct_modules_objectclass" );
            DataTable jct_modules_objectclassDT = jct_modules_objectclassTU.getTable( "where objectclassid = " + ObjectClass.ObjectClassId );
            Int32 RowCount = jct_modules_objectclassDT.Rows.Count;
            for( Int32 R = 0; R < RowCount; R += 1 )
            {
                jct_modules_objectclassDT.Rows[R].Delete();
            }
            jct_modules_objectclassTU.update( jct_modules_objectclassDT );
        }

        /// <summary>
        /// Delete all junctions tied to this ModuleId
        /// </summary>
        public void deleteAllModuleObjectClassJunctions( Int32 ModuleId )
        {
            CswTableUpdate jct_modules_objectclassTU = makeCswTableUpdate( "SchemaModTrnsctn_DeleteAllModuleOCJunction", "jct_modules_objectclass" );
            DataTable jct_modules_objectclassDT = jct_modules_objectclassTU.getTable( "where moduleid = " + ModuleId );
            Int32 RowCount = jct_modules_objectclassDT.Rows.Count;
            for( Int32 R = 0; R < RowCount; R += 1 )
            {
                jct_modules_objectclassDT.Rows[R].Delete();
            }
            jct_modules_objectclassTU.update( jct_modules_objectclassDT );
        }

        #endregion Delete Junctions

        #region DML
        /// <summary>
        /// (Deprecated) Convenience function for making new Object Class Props
        /// </summary>
        public DataRow addObjectClassPropRow( DataTable ObjectClassPropsTable, Int32 ObjectClassId, string PropName,
                                              CswEnumNbtFieldType FieldType, Int32 DisplayColAdd, Int32 DisplayRowAdd )
        {
            DataRow OCPRow = ObjectClassPropsTable.NewRow();
            OCPRow["propname"] = PropName;
            OCPRow["fieldtypeid"] = MetaData.getFieldType( FieldType ).FieldTypeId.ToString();
            OCPRow["isbatchentry"] = CswConvert.ToDbVal( false );
            OCPRow["isfk"] = CswConvert.ToDbVal( false );
            OCPRow["fktype"] = "";
            OCPRow["fkvalue"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["isrequired"] = CswConvert.ToDbVal( false );
            OCPRow["isunique"] = CswConvert.ToDbVal( false );
            OCPRow["isglobalunique"] = CswConvert.ToDbVal( false );
            OCPRow["objectclassid"] = ObjectClassId.ToString();
            OCPRow["servermanaged"] = CswConvert.ToDbVal( false );
            OCPRow["listoptions"] = "";
            OCPRow["valueoptions"] = "";
            OCPRow["viewxml"] = "";
            OCPRow["multi"] = CswConvert.ToDbVal( false );
            OCPRow["defaultvalueid"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["readonly"] = CswConvert.ToDbVal( false );
            if( DisplayRowAdd != Int32.MinValue )
            {
                OCPRow["display_col_add"] = CswConvert.ToDbVal( DisplayColAdd );
                OCPRow["display_row_add"] = CswConvert.ToDbVal( DisplayRowAdd );
                OCPRow["setvalonadd"] = CswConvert.ToDbVal( true );
            }
            else
            {
                OCPRow["display_col_add"] = CswConvert.ToDbVal( Int32.MinValue );
                OCPRow["display_row_add"] = CswConvert.ToDbVal( Int32.MinValue );
                OCPRow["setvalonadd"] = CswConvert.ToDbVal( false );
            }
            OCPRow["valuefieldid"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["numberprecision"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["numberminvalue"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["numbermaxvalue"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["statictext"] = "";
            OCPRow["filter"] = "";
            OCPRow["filterpropid"] = CswConvert.ToDbVal( Int32.MinValue );
            ObjectClassPropsTable.Rows.Add( OCPRow );
            return OCPRow;
        }

        private bool _validateFkType( string FkType )
        {
            bool RetIsValid = false;

            CswEnumNbtViewPropIdType PropIdType = FkType;
            if( PropIdType != CswEnumNbtViewPropIdType.Unknown )
            {
                RetIsValid = true;
            }
            else
            {
                CswEnumNbtViewRelatedIdType RelatedIdType = FkType;
                if( RelatedIdType != CswEnumNbtViewRelatedIdType.Unknown )
                {
                    RetIsValid = true;
                }
            }
            return RetIsValid;
        }

        /// <summary>
        /// (Deprecated) Convenience function for making new Object Class Props with more granular control
        /// </summary>
        private DataRow _addObjectClassPropRow( DataTable ObjectClassPropsTable, CswNbtMetaDataObjectClass ObjectClass, string PropName,
                                             CswEnumNbtFieldType FieldType, bool IsBatchEntry, bool ReadOnly,
                                             bool IsFk, string FkType, Int32 FkValue, bool IsRequired, bool IsUnique, bool IsGlobalUnique,
                                             bool ServerManaged, string ListOptions, Int32 DisplayColAdd, Int32 DisplayRowAdd, Int32 NumberPrecision )
        {
            DataRow OCPRow = ObjectClassPropsTable.NewRow();
            OCPRow["propname"] = PropName;
            OCPRow["fieldtypeid"] = CswConvert.ToDbVal( MetaData.getFieldType( FieldType ).FieldTypeId );
            OCPRow["isbatchentry"] = CswConvert.ToDbVal( IsBatchEntry );
            OCPRow["isfk"] = CswConvert.ToDbVal( IsFk );
            if( IsFk &&
                Int32.MinValue != FkValue &&
                _validateFkType( FkType ) )
            {
                OCPRow["fktype"] = FkType;
                OCPRow["fkvalue"] = CswConvert.ToDbVal( FkValue );
            }
            else
            {
                OCPRow["fktype"] = "";
                OCPRow["fkvalue"] = CswConvert.ToDbVal( Int32.MinValue );
            }
            OCPRow["isrequired"] = CswConvert.ToDbVal( IsRequired );
            OCPRow["isunique"] = CswConvert.ToDbVal( IsUnique );
            OCPRow["isglobalunique"] = CswConvert.ToDbVal( IsGlobalUnique );
            OCPRow["objectclassid"] = ObjectClass.ObjectClassId.ToString();
            OCPRow["servermanaged"] = CswConvert.ToDbVal( ServerManaged );
            OCPRow["listoptions"] = ListOptions;
            OCPRow["valueoptions"] = "";
            OCPRow["viewxml"] = "";
            OCPRow["multi"] = CswConvert.ToDbVal( false );
            OCPRow["defaultvalueid"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["readonly"] = CswConvert.ToDbVal( ReadOnly );
            OCPRow["display_col_add"] = CswConvert.ToDbVal( DisplayColAdd );
            OCPRow["display_row_add"] = CswConvert.ToDbVal( DisplayRowAdd );
            if( DisplayRowAdd != Int32.MinValue )
            {
                OCPRow["setvalonadd"] = CswConvert.ToDbVal( true );
            }
            else
            {
                OCPRow["setvalonadd"] = CswConvert.ToDbVal( false );
            }
            OCPRow["valuefieldid"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["numberprecision"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["numberminvalue"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["numbermaxvalue"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow["statictext"] = "";
            OCPRow["filter"] = "";
            OCPRow["filterpropid"] = CswConvert.ToDbVal( Int32.MinValue );
            ObjectClassPropsTable.Rows.Add( OCPRow );
            return OCPRow;
        }

        /// <summary>
        /// Convenience function for making new Object Class Props with more granular control
        /// </summary>
        private void _addObjectClassPropRow( DataTable ObjectClassPropsTable, CswNbtWcfMetaDataModel.ObjectClassProp OcpModel )
        {
            DataRow OCPRow = ObjectClassPropsTable.NewRow();
            OCPRow[CswEnumNbtObjectClassPropAttributes.propname.ToString()] = OcpModel.PropName;
            OCPRow[CswEnumNbtObjectClassPropAttributes.fieldtypeid.ToString()] = CswConvert.ToDbVal( MetaData.getFieldType( OcpModel.FieldType ).FieldTypeId );
            OCPRow[CswEnumNbtObjectClassPropAttributes.isbatchentry.ToString()] = CswConvert.ToDbVal( OcpModel.IsBatchEntry );
            if( OcpModel.IsFk ||
                ( Int32.MinValue != OcpModel.FkValue &&
                _validateFkType( OcpModel.FkType ) ) )
            {
                OCPRow[CswEnumNbtObjectClassPropAttributes.isfk.ToString()] = CswConvert.ToDbVal( true );
                OCPRow[CswEnumNbtObjectClassPropAttributes.fktype.ToString()] = OcpModel.FkType;
                OCPRow[CswEnumNbtObjectClassPropAttributes.fkvalue.ToString()] = CswConvert.ToDbVal( OcpModel.FkValue );
                if( Int32.MinValue != OcpModel.ValuePropId &&
                    CswNbtResources.UnknownEnum != OcpModel.ValuePropType )
                {
                    OCPRow[CswEnumNbtObjectClassPropAttributes.valuepropid.ToString()] = CswConvert.ToDbVal( OcpModel.ValuePropId );
                    OCPRow[CswEnumNbtObjectClassPropAttributes.valueproptype.ToString()] = CswConvert.ToDbVal( OcpModel.ValuePropType );
                }
            }
            else
            {
                OCPRow[CswEnumNbtObjectClassPropAttributes.isfk.ToString()] = CswConvert.ToDbVal( false );
                OCPRow[CswEnumNbtObjectClassPropAttributes.fktype.ToString()] = "";
                OCPRow[CswEnumNbtObjectClassPropAttributes.fkvalue.ToString()] = CswConvert.ToDbVal( Int32.MinValue );
            }
            OCPRow[CswEnumNbtObjectClassPropAttributes.isrequired.ToString()] = CswConvert.ToDbVal( OcpModel.IsRequired );
            OCPRow[CswEnumNbtObjectClassPropAttributes.isunique.ToString()] = CswConvert.ToDbVal( OcpModel.IsUnique );
            OCPRow[CswEnumNbtObjectClassPropAttributes.isglobalunique.ToString()] = CswConvert.ToDbVal( OcpModel.IsGlobalUnique );
            OCPRow["objectclassid"] = OcpModel.ObjectClass.ObjectClassId.ToString();
            OCPRow[CswEnumNbtObjectClassPropAttributes.servermanaged.ToString()] = CswConvert.ToDbVal( OcpModel.ServerManaged );
            OCPRow[CswEnumNbtObjectClassPropAttributes.listoptions.ToString()] = OcpModel.ListOptions;
            OCPRow[CswEnumNbtObjectClassPropAttributes.valueoptions.ToString()] = OcpModel.ValueOptions;
            OCPRow[CswEnumNbtObjectClassPropAttributes.viewxml.ToString()] = OcpModel.ViewXml;
            OCPRow[CswEnumNbtObjectClassPropAttributes.multi.ToString()] = CswConvert.ToDbVal( OcpModel.Multi );
            OCPRow["defaultvalueid"] = CswConvert.ToDbVal( Int32.MinValue );
            OCPRow[CswEnumNbtObjectClassPropAttributes.readOnly.ToString()] = CswConvert.ToDbVal( OcpModel.ReadOnly );
            OCPRow[CswEnumNbtObjectClassPropAttributes.display_col_add.ToString()] = CswConvert.ToDbVal( OcpModel.DisplayColAdd );
            OCPRow[CswEnumNbtObjectClassPropAttributes.display_row_add.ToString()] = CswConvert.ToDbVal( OcpModel.DisplayRowAdd );
            if( OcpModel.DisplayRowAdd != Int32.MinValue || OcpModel.SetValOnAdd )
            {
                OCPRow[CswEnumNbtObjectClassPropAttributes.setvalonadd.ToString()] = CswConvert.ToDbVal( true );
            }
            else
            {
                OCPRow[CswEnumNbtObjectClassPropAttributes.setvalonadd.ToString()] = CswConvert.ToDbVal( false );
            }
            OCPRow[CswEnumNbtObjectClassPropAttributes.valuefieldid.ToString()] = CswConvert.ToDbVal( OcpModel.ValueFieldId );
            if( OcpModel.FieldType == CswEnumNbtFieldType.Number )
            {
                OCPRow[CswEnumNbtObjectClassPropAttributes.numberprecision.ToString()] =
                    CswConvert.ToDbVal( OcpModel.NumberPrecision );
                OCPRow[CswEnumNbtObjectClassPropAttributes.numberminvalue.ToString()] =
                    CswConvert.ToDbVal( OcpModel.NumberMinValue );
                OCPRow[CswEnumNbtObjectClassPropAttributes.numbermaxvalue.ToString()] =
                    CswConvert.ToDbVal( OcpModel.NumberMaxValue );
            }
            OCPRow[CswEnumNbtObjectClassPropAttributes.statictext.ToString()] = OcpModel.StaticText;
            OCPRow[CswEnumNbtObjectClassPropAttributes.extended.ToString()] = OcpModel.Extended;
            OCPRow[CswEnumNbtObjectClassPropAttributes.textareacols.ToString()] = CswConvert.ToDbVal( OcpModel.TextAreaColumns );
            OCPRow[CswEnumNbtObjectClassPropAttributes.textarearows.ToString()] = CswConvert.ToDbVal( OcpModel.TextAreaRows );

            OCPRow[CswEnumNbtObjectClassPropAttributes.filter.ToString()] = OcpModel.Filter;
            OCPRow[CswEnumNbtObjectClassPropAttributes.filterpropid.ToString()] = CswConvert.ToDbVal( OcpModel.FilterPropId );
            OCPRow[CswEnumNbtObjectClassPropAttributes.auditlevel.ToString()] = CswConvert.ToDbVal( OcpModel.AuditLevel );

            OCPRow["oraviewcolname"] = CswFormat.MakeOracleCompliantIdentifier( OcpModel.PropName );

            ObjectClassPropsTable.Rows.Add( OCPRow );
        }

        #endregion DML

        #region DDL

        /// <summary>
        /// Convenience function for adding a new boolean column to the database schema
        /// </summary>
        public void addBooleanColumn( string tablename, string columnname, string description, bool required )//, Int32 NodeTypePropId, string SubFieldName )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.Boolean, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, false, string.Empty );//, NodeTypePropId, SubFieldName );
        }

        /// <summary>
        /// Convenience function for adding a new foreign key column to the database schema
        /// </summary>
        public void addForeignKeyColumn( string tablename, string columnname, string description, bool required, string foreignkeytable, string foreignkeycolumn )//, Int32 NodeTypePropId, string SubFieldName )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Fk, Int32.MinValue, Int32.MinValue, string.Empty, description, foreignkeycolumn, foreignkeytable,
                       false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.Long, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, false, string.Empty );//, NodeTypePropId, SubFieldName );
        }

        /// <summary>
        /// Convenience function for adding a new string column to the database schema
        /// </summary>
        public void addStringColumn( string tablename, string columnname, string description, bool required, Int32 datatypesize )//, Int32 NodeTypePropId, string SubFieldName )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, datatypesize, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.String, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, false, string.Empty );//, NodeTypePropId, SubFieldName );
        }

        /// <summary>
        /// Convenience function for adding a new long column to the database schema
        /// </summary>
        public void addLongColumn( string tablename, string columnname, string description, bool required )
        {
            addLongColumn( tablename, columnname, description, required, string.Empty, false, string.Empty, false );
        }

        /// <summary>
        /// Convenience function for adding a new long column to the database schema
        /// </summary>
        public void addLongColumn( string tablename, string columnname, string description, bool required,
                                   string LowerRangeValue, bool LowerRangeValueInclusive, string UpperRangeValue, bool UpperRangeValueInclusive )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, LowerRangeValue, LowerRangeValueInclusive, CswEnumDataDictionaryPortableDataType.Long, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, UpperRangeValueInclusive, UpperRangeValue );
        }

        /// <summary>
        /// Convenience function for adding a new number column to the database schema
        /// </summary>
        public void addNumberColumn( string tablename, string columnname, string description, bool required,
                                   string LowerRangeValue = "", bool LowerRangeValueInclusive = false, string UpperRangeValue = "", bool UpperRangeValueInclusive = false )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, LowerRangeValue, LowerRangeValueInclusive, CswEnumDataDictionaryPortableDataType.Number, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, UpperRangeValueInclusive, UpperRangeValue );
        }

        /// <summary>
        /// Convenience function for adding a new double column to the database schema
        /// </summary>
        public void addDoubleColumn( string tablename, string columnname, string description, bool required, Int32 DblPrecision )
        {
            addDoubleColumn( tablename, columnname, description, required, DblPrecision, string.Empty, false, string.Empty, false );
        }

        /// <summary>
        /// Convenience function for adding a new double column to the database schema
        /// </summary>
        public void addDoubleColumn( string tablename, string columnname, string description, bool required, Int32 DblPrecision,
                                     string LowerRangeValue, bool LowerRangeValueInclusive, string UpperRangeValue, bool UpperRangeValueInclusive )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, DblPrecision, string.Empty, description, string.Empty, string.Empty,
                       false, false, LowerRangeValue, LowerRangeValueInclusive, CswEnumDataDictionaryPortableDataType.Double, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, UpperRangeValueInclusive, UpperRangeValue );
        }

        /// <summary>
        /// Convenience function for adding a new date column to the database schema
        /// </summary>
        public void addDateColumn( string tablename, string columnname, string description, bool required )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.Datetime, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, false, string.Empty );
        }

        /// <summary>
        /// Convenience function for adding a new date column to the database schema
        /// </summary>
        public void addBlobColumn( string tablename, string columnname, string description, bool required )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.Blob, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, false, string.Empty );
        }

        /// <summary>
        /// Convenience function for adding a new date column to the database schema
        /// </summary>
        public void addClobColumn( string tablename, string columnname, string description, bool required )
        {
            addColumn( columnname, CswEnumDataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, string.Empty, false, CswEnumDataDictionaryPortableDataType.Clob, false,
                       required, tablename, CswEnumDataDictionaryUniqueType.None, false, string.Empty );
        }

        #endregion DDL

        #endregion

        /// <summary>
        /// Replaces one property for another in all Views.
        /// If the new property's name ends with _TMP, the suffix is removed.
        /// </summary>
        public void ReplacePropertyInViews( CswTableUpdate ViewsUpdate, CswNbtMetaDataNodeTypeProp OldProp, CswNbtMetaDataNodeTypeProp NewProp )
        {
            if( OldProp != null )
            {
                string WhereClause = "where viewxml like '%" + OldProp.PropId.ToString() + "%'";
                CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
                SelectCols.Add( "nodeviewid" );
                SelectCols.Add( "viewxml" );
                DataTable ReportedByViewsTable = ViewsUpdate.getTable( SelectCols, "", Int32.MinValue, WhereClause, false );
                foreach( DataRow CurrentRow in ReportedByViewsTable.Rows )
                {
                    CswNbtView CurrentView = this.makeView();
                    CurrentView.LoadXml( CurrentRow["viewxml"].ToString() );
                    CurrentView.ViewId = new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) );

                    // Swap old property for new
                    this.ReplaceViewProperty( CurrentView, OldProp, NewProp );
                    CurrentView.save();
                }
                ViewsUpdate.update( ReportedByViewsTable );
            }
        }

        /// <summary>
        /// Replaces one property for another in a View.
        /// If the new property's name ends with _TMP, the suffix is removed.
        /// </summary>
        public void ReplaceViewProperty( CswNbtView View, CswNbtMetaDataNodeTypeProp ExistingProperty, CswNbtMetaDataNodeTypeProp NewProperty )
        {
            foreach( CswNbtViewRelationship Relationship in View.Root.ChildRelationships )
            {
                _ReplaceViewPropertyRecursive( View, Relationship, ExistingProperty, NewProperty );
            }
        }
        private void _ReplaceViewPropertyRecursive( CswNbtView View, CswNbtViewRelationship Rel, CswNbtMetaDataNodeTypeProp ExistingProperty, CswNbtMetaDataNodeTypeProp NewProperty )
        {
            Collection<CswNbtViewProperty> ViewPropsToRemove = new Collection<CswNbtViewProperty>();
            foreach( CswNbtViewProperty ViewProp in Rel.Properties )
            {
                if( ViewProp.NodeTypePropId == ExistingProperty.PropId )
                {
                    // Mark old prop for swap
                    ViewPropsToRemove.Add( ViewProp );
                }
            }

            foreach( CswNbtViewProperty OldProp in ViewPropsToRemove )
            {
                // Add new prop
                CswNbtViewProperty NewViewProp = View.AddViewProperty( Rel, NewProperty );
                NewViewProp.SortBy = OldProp.SortBy;
                NewViewProp.SortMethod = OldProp.SortMethod;

                // Copy filters
                foreach( CswNbtViewPropertyFilter ViewFilter in OldProp.Filters )
                {
                    NewViewProp.addFilter( ViewFilter );
                }

                // Remove old prop
                Rel.removeProperty( OldProp );

                // Fix the name
                if( NewProperty.PropName.EndsWith( "_TMP" ) )
                    NewViewProp.Name = NewProperty.PropName.Substring( 0, NewProperty.PropName.IndexOf( "_TMP" ) );
            }

            // Recurse
            foreach( CswNbtViewRelationship Relationship in Rel.ChildRelationships )
            {
                _ReplaceViewPropertyRecursive( View, Relationship, ExistingProperty, NewProperty );
            }
        }

        public void InsertS4( string QueryId, string QueryText, string MainTableName )
        {
            string PkColumnName = _CswNbtResources.DataDictionary.getPrimeKeyColumn( MainTableName );
            _CswNbtResources.DataDictionary.setCurrentColumn( MainTableName, PkColumnName );
            Int32 PkTableColId = _CswNbtResources.DataDictionary.TableColId;

            CswTableUpdate S4Update = makeCswTableUpdate( "CswNbtSchemaModTrnsctn_InsertS4", "static_sql_selects" );
            DataTable S4Table = S4Update.getEmptyTable();
            DataRow NewRow = S4Table.NewRow();
            NewRow["datadictionaryid"] = CswConvert.ToDbVal( PkTableColId );
            NewRow["dd_columnname"] = PkColumnName;
            NewRow["dd_tablename"] = MainTableName.ToLower();
            NewRow["deleted"] = CswConvert.ToDbVal( false );
            NewRow["queryid"] = QueryId;
            NewRow["querytext"] = QueryText;
            S4Table.Rows.Add( NewRow );
            S4Update.update( S4Table );

            // Clear cached S4s
            _CswNbtResources.CswResources.ClearCache();
        }

        /// <summary>
        /// Convenience function for updating S4s
        /// </summary>
        public void UpdateS4( string QueryId, string QueryText )
        {
            CswTableUpdate S4Update = makeCswTableUpdate( "CswNbtSchemaModTrnsctn_UpdateS4", "static_sql_selects" );
            DataTable S4Table = S4Update.getTable( "where lower(queryid) = '" + QueryId.ToLower() + "'" );
            if( S4Table.Rows.Count < 0 )
                throw new CswDniException( CswEnumErrorType.Error, "No Match for S4 QueryId: " + QueryId, "CswNbtSchemaModTrnsctn::UpdateS4() returned 0 rows for S4 queryid: " + QueryId );
            S4Table.Rows[0]["querytext"] = QueryText;
            S4Update.update( S4Table );

            // Clear cached S4s
            _CswNbtResources.CswResources.ClearCache();
        }

        /// <summary>
        /// Reading of values located in the configuration_variables table
        /// </summary>
        public CswConfigurationVariables ConfigVbls { get { return ( _CswNbtResources.ConfigVbls ); } }

        /// <summary>
        /// Convenience function for setting value of a configuration variable
        /// </summary>
        public void setConfigVariableValue( String VariableName, String VariableValue )
        {
            if( !String.IsNullOrEmpty( VariableValue ) && !String.IsNullOrEmpty( VariableName ) )
            {
                _CswNbtResources.ConfigVbls.setConfigVariableValue( VariableName, VariableValue );
            }
        }

        public string getConfigVariableValue( String VariableName )
        {
            return ( _CswNbtResources.ConfigVbls.getConfigVariableValue( VariableName ) );
        }

        public CswNbtActUpdatePropertyValue getCswNbtActUpdatePropertyValue() { return ( new CswNbtActUpdatePropertyValue( _CswNbtResources ) ); }

        public void execStoredProc( string StoredProcName, List<CswStoredProcParam> Params ) { _CswNbtResources.execStoredProc( StoredProcName, Params ); }

        public void getNextSchemaDumpFileInfo( ref string PhysicalDirectoryPath, ref string NameOfCurrentDump )
        {
            if( _CswNbtResources.SetupVbls.doesSettingExist( CswEnumSetupVariableNames.DumpFileDirectoryId ) )
            {
                string StatusMsg = string.Empty;
                if( false == _CswNbtResources.getNextSchemaDumpFileInfo( _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.DumpFileDirectoryId], ref PhysicalDirectoryPath, ref NameOfCurrentDump, ref StatusMsg ) )
                {
                    throw ( new CswDniException( "Unable to take retrieve dump file information: " + StatusMsg ) );
                }
            }
            else
            {
                throw ( new CswDniException( "Unable to get dump file information: there is no " + CswEnumSetupVariableNames.DumpFileDirectoryId + " setup variable" ) );
            }//if-else the dump setup setting exists

        }//getNextSchemaDumpFileInfo() 

        public void takeADump( ref string DumpFileName, ref string StatusMessage )
        {
            if( _CswNbtResources.SetupVbls.doesSettingExist( CswEnumSetupVariableNames.DumpFileDirectoryId ) )
            {
                if( false == _CswNbtResources.takeADump( _CswNbtResources.SetupVbls[CswEnumSetupVariableNames.DumpFileDirectoryId], ref DumpFileName, ref StatusMessage ) )
                {
                    throw ( new CswDniException( "Unable to take a dump: " + StatusMessage ) );
                }
            }
            else
            {
                throw ( new CswDniException( "Unable to take a dump: there is no " + CswEnumSetupVariableNames.DumpFileDirectoryId + " setup variable" ) );

            }//if-else the dump setup setting exists

        }//takeADump() 

        public string makeUniqueConstraint( string TableName, string ColumnName )
        {
            return ( _CswNbtResources.makeUniqueConstraint( TableName, ColumnName ) );
        }//makeUniqueConstraintInDb() 

        public string makeUniqueConstraint( string TableName, string ColumnName, bool AddDdData )
        {
            return ( _CswNbtResources.makeUniqueConstraint( TableName, ColumnName, AddDdData ) );
        }

        public bool doesFkConstraintExistInDb( string ConstraintName ) { return ( _CswNbtResources.doesFkConstraintExistInDb( ConstraintName ) ); }
        public bool doesUniqueConstraintExistInDb( string ConstraintName ) { return ( _CswNbtResources.doesUniqueConstraintExistInDb( ConstraintName ) ); }
        public string getUniqueConstraintName( string TableName, string ColumName ) { return ( _CswNbtResources.getUniqueConstraintName( TableName, ColumName ) ); }

        /// <summary>
        /// (Deprecated: vendor neutrality is no longer a requirement)
        /// <para>Run an external SQL script stored in Resources</para>
        /// </summary>
        /// <param name="SqlFileName">Name of file</param>
        /// <param name="ResourceSqlFile">File contents from Resources</param>
        public void runExternalSqlScript( string SqlFileName, byte[] ResourceSqlFile )
        {
            string FileLocations = Application.StartupPath;
            string BatchFilePath = FileLocations + "\\runscript.bat";
            string SqlFilePath = FileLocations + "\\" + SqlFileName;
            try
            {
                //Retrieve files from resource
                File.WriteAllBytes( BatchFilePath, ChemSW.Nbt.Properties.Resources.runscript_bat );
                File.WriteAllBytes( SqlFilePath, ResourceSqlFile );

                while( ( false == File.Exists( BatchFilePath ) ) && ( false == File.Exists( SqlFilePath ) ) )
                {
                    Thread.Sleep( 100 );
                }

                CswDbCfgInfo.makeConfigurationCurrent( Accessid );
                string serverName = CswDbCfgInfo.CurrentServerName;
                string userName = CswDbCfgInfo.CurrentUserName;
                string passWord = CswDbCfgInfo.CurrentPlainPwd;

                // Start external process
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = BatchFilePath;
                p.StartInfo.Arguments = " " + serverName + " " + userName + " " + passWord + " " + FileLocations + " " + SqlFileName;
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;

                Process SpawnedProcess = System.Diagnostics.Process.Start( p.StartInfo );
                if( false == SpawnedProcess.WaitForExit( UpdtShellWaitMsec ) )
                {
                    CswLogger.reportAppState( "Timed out will running " + SqlFileName + " prior to updates." );
                }
            }
            finally
            {
                if( File.Exists( BatchFilePath ) )
                {
                    File.Delete( BatchFilePath );
                }
                if( File.Exists( SqlFilePath ) )
                {
                    File.Delete( SqlFilePath );
                }
            }
        } // runExternalSqlScript

        /// <summary>
        /// Returns true if the current schema is an unmodified master
        /// </summary>
        public bool isMaster()
        {
            //
            CswTableSelect LicenseAcceptance = new CswTableSelect( _CswNbtResources.CswResources, "license acceptance", "license_accept" );
            return ( LicenseAcceptance.getRecordCount() == 0 );
        }

        /// <summary>
        /// Create a Rate Interval instance
        /// </summary>
        public CswRateInterval makeRateInterval()
        {
            return new CswRateInterval( _CswNbtResources );
        }

        private CswNbtSearch _search = null;
        public CswNbtSearch CswNbtSearch
        {
            get
            {
                if( null == _search )
                {
                    _search = new CswNbtSearch( _CswNbtResources );
                }

                return ( _search );
            }
        }

        private CswNbtSdBlobData _sdBlobData = null;
        public CswNbtSdBlobData CswNbtSdBlobData
        {
            get
            {
                if( null == _sdBlobData )
                {
                    _sdBlobData = new CswNbtSdBlobData( _CswNbtResources );
                }
                return _sdBlobData;
            }
        }

        //public ICswNbtTree getArbitraryNodes( string SearchTerm, CswNbtMetaDataObjectClass ObjClass = null, CswNbtMetaDataNodeType NodeType = null )
        //{
        //    if( null == _search )
        //    {
        //        _search = new CswNbtSearch( _CswNbtResources ) { SearchTerm = SearchTerm  }; 
        //    }


        //    if( null != NodeType )
        //    {
        //        _search.addFilter( NodeType, false );
        //    }

        //    if( null != ObjClass )
        //    {
        //        _search.addFilter( ObjClass , false ); 
        //    }

        //    return( _search.Results() ); 

        //}//doSearch()

        public Dictionary<string, int> createImportDefinitionEntries(string ImportDefinitionName, DataTable DefDataTable)
        {
            return CswNbtImportDef.addDefinitionEntries( _CswNbtResources, ImportDefinitionName, DefDataTable );
        }

    }//class CswNbtSchemaModTrnsctn

}//ChemSW.Nbt.Schema
