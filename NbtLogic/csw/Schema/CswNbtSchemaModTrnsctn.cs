using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Log;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

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
            _CswNbtResources.TransactionMode = TransactionMode.Atomistic;
            _CswDdl = new CswDDL( _CswNbtResources );
            //            _CswNbtSequenceManager = new CswNbtSequenceManager( _CswNbtResources );
        }//ctor

        public ICswLogger CswLogger { get { return ( _CswNbtResources.CswLogger ); } }

        //private bool _ManageConstraints = true;
        //public bool ManageConstraints
        //{
        //    set { _CswDdl.ManageConstraints = value; }
        //    get { return ( _CswDdl.ManageConstraints ); }
        //}//ManageConstraints

        #region TransactionManagement
        public void beginTransaction()
        {
            _CswNbtResources.CswResources.beginTransaction();
        }//beginTransaction()

        /// <summary>
        /// This removes the cache of CswTableCaddies from CswResources.  Consider using Rollback() if you are trying to revert changes.
        /// </summary>
        public void clearUpdates()
        {

            _CswNbtResources.CswResources.clearUpdates();
        }//

        public void commitTransaction()
        {
            _CswDdl.confirm();
            _CswNbtResources.finalize( true );
            //_CswNbtResources.CswResources.commitTransaction();
            _CswDdl.clear();


        }//commitTransaction()

        public void rollbackTransaction()
        {
            //_CswResourcesForTableCaddy.commitTransaction();
            _CswNbtResources.Rollback();
            _CswNbtResources.refresh();
            _CswDdl.revert();
            _CswDdl.clear();

        }//rollbackTransaction()

        public void refreshDataDictionary()
        {
            _CswNbtResources.refresh();
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
        public bool isColumnDefinedInMetaData( string TableName, string ColumnName ) { return ( _CswNbtResources.CswResources.isColumnDefinedInMetaData( TableName, ColumnName ) ); }//isColumnDefinedInMetaData 

        public bool isTableDefinedInDataBase( string TableName ) { return ( _CswNbtResources.CswResources.isTableDefinedInDataBase( TableName ) ); }//isTableDefinedInDataBase() 
        public bool isColumnDefinedInDataBase( string TableName, string ColumnName ) { return ( _CswNbtResources.CswResources.isColumnDefinedInDataBase( TableName, ColumnName ) ); }//isColumnDefinedInDataBase()

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
        /// Executes arbitrary sql.  It's your job to make sure it's platform neutral.
        /// You should *strongly* consider using CswArbitrarySelect, CswTableSelect, or CswTableUpdate instead of this.
        /// </summary>
        /// <returns>DataTable of results</returns>
        public DataTable execArbitraryPlatformNeutralSqlSelect( string UniqueName, string SqlText ) { return _CswNbtResources.CswResources.execArbitraryPlatformNeutralSqlSelect( UniqueName, SqlText ); }

        //limit usage of CswDbResources public interfaces as per bz # 9136
        //        public string getSqlText( string AdapterName, SqlType SqlType ) { return ( _CswResourcesForTableCaddy.getSqlText( AdapterName, SqlType ) ); }// 
        public string getSqlText( SqlType SqlType ) { return ( _CswNbtResources.CswResources.getSqlText( SqlType ) ); }//
        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public void setSqlDmlText( SqlType SqlType, string DmlTable, string SqlDmlText, bool LoadParamsFromSql ) { _CswResourcesForTableCaddy.setSqlDmlText( SqlType, DmlTable, SqlDmlText, LoadParamsFromSql ); }
        //        public void setSqlDmlText( string AdapterName, SqlType SqlType, string DmlTable, string SqlDmlText, bool LoadParamsFromSql ) { _CswResourcesForTableCaddy.setSqlDmlText( AdapterName, SqlType, DmlTable, SqlDmlText, LoadParamsFromSql ); }
        public void setParameterValue( SqlType SqlType, CswStaticParam StaticParam ) { _CswNbtResources.CswResources.setParameterValue( SqlType, StaticParam ); }
        //limit usage of CswDbResources public interfaces as per bz # 9136
        //public void setParameterValue( string AdapterName, ChemSW.RscAdo.SqlType SqlType, string parameterName, object val ) { _CswResourcesForTableCaddy.setParameterValue( AdapterName, SqlType, parameterName, val ); }
        //public void clearCommand( SqlType SqlType ) { _CswResourcesForTableCaddy.clearCommand( SqlType ); }
        //public void clearCommand( string AdapterName, SqlType SqlType ) { _CswResourcesForTableCaddy.clearCommand( AdapterName, SqlType ); }
        //        public ICswDbNativeDate getCswDbNativeDate() { return ( _CswResourcesForTableCaddy.getCswDbNativeDate() ); }// 
        //        public void prepareDmlOp( SqlType SqlType, DataTable DataTable ) { _CswResourcesForTableCaddy.prepareDmlOp( SqlType, DataTable ); }
        public void prepareDmlOp( string AdapterName, SqlType SqlType, DataTable DataTable ) { _CswNbtResources.CswResources.prepareDmlOp( AdapterName, SqlType, DataTable ); }
        public string getPrimeKeyColName( string TableName ) { return ( _CswNbtResources.CswResources.getPrimeKeyColName( TableName ) ); }
        public int getNewPrimeKey( string TableName ) { return ( _CswNbtResources.CswResources.getNewPrimeKey( TableName ) ); }



        public Int32 makeSequence( CswSequenceName SequenceName, string Prepend, string Postpend, Int32 Pad, Int32 InitialValue )
        {

            return _CswDdl.makeSequence( SequenceName, Prepend, Postpend, Pad, InitialValue );
        }
        public DataTable getSequence( CswSequenceName SequenceName )
        {
            return _CswDdl.getSequence( SequenceName );
        }
        public DataTable getAllSequences()
        {
            return _CswDdl.getAllSequences();
        }

        public bool doesSequenceExist( CswSequenceName SequenceName )
        {
            return ( _CswDdl.doesSequenceExist( SequenceName ) );
        }

        public Int32 getSequenceValue( CswSequenceName SequenceName )
        {
            return ( _CswDdl.getSequenceValue( SequenceName ) );
        }

        public void removeSequence( CswSequenceName SequenceName )
        {
            _CswDdl.removeSequence( SequenceName );
        }


        public void makeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, bool AddDdData )
        {
            _CswDdl.makeConstraint( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, AddDdData );

        }//makeConstraint()


        public void removeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, string ConstraintName )
        {
            _CswDdl.removeConstraint( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, ConstraintName );
        }//removeConstraint()

        //public void removeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName )
        //{
        //    _CswDdl.removeConstraint( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName );
        //}//removeConstraint()


        public bool doesConstraintExistInDb( string ConstraintName )
        {
            return ( _CswDdl.doesConstraintExistInDb( ConstraintName ) );
        }//doesConstraintExist()

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

        public void dropTable( string TableName ) { _CswDdl.dropTable( TableName ); }//dropTable()

        //public void addColumn( string columnname, DataDictionaryColumnType columntype, Int32 datatypesize, Int32 dblprecision,
        //                       string defaultvalue, string description, string foreignkeycolumn, string foreignkeytable, bool constrainfkref, bool isview,
        //                       bool logicaldelete, string lowerrangevalue, bool lowerrangevalueinclusive, DataDictionaryPortableDataType portabledatatype, bool ReadOnly,
        //                       bool Required, string tablename, DataDictionaryUniqueType uniquetype, bool uperrangevalueinclusive, string upperrangevalue )
        //{
        //    addColumn( columnname, columntype, datatypesize, dblprecision,
        //               defaultvalue, description, foreignkeycolumn, foreignkeytable, constrainfkref, isview,
        //               logicaldelete, lowerrangevalue, lowerrangevalueinclusive, portabledatatype, ReadOnly,
        //               Required, tablename, uniquetype, uperrangevalueinclusive, upperrangevalue );
        //               //Int32.MinValue, string.Empty );
        //}
        public void addColumn( string columnname, DataDictionaryColumnType columntype, Int32 datatypesize, Int32 dblprecision,
                               string defaultvalue, string description, string foreignkeycolumn, string foreignkeytable, bool constrainfkref, bool isview,
                               bool logicaldelete, string lowerrangevalue, bool lowerrangevalueinclusive, DataDictionaryPortableDataType portabledatatype, bool ReadOnly,
                               bool Required, string tablename, DataDictionaryUniqueType uniquetype, bool uperrangevalueinclusive, string upperrangevalue )
        //Int32 NodeTypePropId, string SubFieldName )
        {
            _CswDdl.addColumn( columnname, columntype, datatypesize, dblprecision,
                               defaultvalue, description, foreignkeycolumn, foreignkeytable, constrainfkref, isview,
                               logicaldelete, lowerrangevalue, lowerrangevalueinclusive, portabledatatype, ReadOnly,
                               Required, tablename, uniquetype, uperrangevalueinclusive, upperrangevalue );
            //NodeTypePropId, SubFieldName );
        }//addColumn()

        public void renameColumn( string TableName, string OriginalColumnName, string NewColumnName )
        {
            _CswDdl.renameColumn( TableName, OriginalColumnName, NewColumnName );
        }


        public void dropColumn( string TableName, string ColumnName ) { _CswDdl.dropColumn( TableName, ColumnName ); }
        public void changeColumnDataType( string TableName, string ColumnName, DataDictionaryPortableDataType NewDataType, Int32 DataTypeSize ) { _CswNbtResources.CswResources.changeColumnDataType( TableName, ColumnName, NewDataType, DataTypeSize ); }
        public bool isLogicalDeleteTable( string TableName ) { return ( _CswNbtResources.isLogicalDeleteTable( TableName ) ); }

        public DataTable getAllViews() { return _CswNbtResources.ViewSelect.getAllViews(); }

        //        public DbVendorType DbVendorType { get { return ( _CswResourcesForTableCaddy.DbVendorType ); } }
        public string Accessid { get { return ( _CswNbtResources.AccessId ); } }
        public string ServerId { get { return ( _CswNbtResources.CswResources.ServerId ); } }
        public string UserName { get { return ( _CswNbtResources.CswResources.UserName ); } }
        #endregion

        #region Metadata, table, and Nodes
        public CswStaticSelect makeCswStaticSelect( string UniqueName, string QueryName ) { return ( _CswNbtResources.makeCswStaticSelect( UniqueName, QueryName ) ); }
        public CswArbitrarySelect makeCswArbitrarySelect( string UniqueName, string QueryText ) { return ( _CswNbtResources.makeCswArbitrarySelect( UniqueName, QueryText ) ); }
        public CswTableSelect makeCswTableSelect( string UniqueName, string TableName ) { return ( _CswNbtResources.makeCswTableSelect( UniqueName, TableName ) ); }
        public CswTableUpdate makeCswTableUpdate( string UniqueName, string TableName ) { return ( _CswNbtResources.makeCswTableUpdate( UniqueName, TableName ) ); }
        //public CswNbtMetaData MetaData { get { return ( _CswNbtResources.MetaData ); } }

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
                    _CswNbtMetaDataForSchemaUpdater = new CswNbtMetaDataForSchemaUpdater( _CswNbtResources, _CswNbtResources.MetaData._CswNbtMetaDataResources );
                    _CswNbtResources.assignMetaDataEvents( _CswNbtMetaDataForSchemaUpdater );
                }
                return _CswNbtMetaDataForSchemaUpdater;
            }
        }


        public void makeMissingAuditTablesAndColumns()
        {

            ICollection TableNamesColl = (ICollection) _CswNbtResources.CswResources.DataDictionary.getTableNames();
            string[] TableNames = new string[TableNamesColl.Count];
            TableNamesColl.CopyTo( TableNames, 0 );

            foreach( string CurrentTableName in TableNames )
            {
                makeTableAuditable( CurrentTableName );
            }

        }//makeMissingAuditTables()


        public void makeTableAuditable( string TableName )
        {
            if( ( false == _CswAuditMetaData.isAuditTable( TableName ) ) && ( false == _CswNbtResources.CswResources.DataDictionary.isColumnDefined( TableName, _CswAuditMetaData.AuditLevelColName ) ) )
            {
                addStringColumn( TableName, _CswAuditMetaData.AuditLevelColName, _CswAuditMetaData.AuditLevelColDescription, false, _CswAuditMetaData.AuditLevelColIsRequired, _CswAuditMetaData.AuditLevelColLength );
            }

            //datetime stamp column


            if( false == _CswAuditMetaData.isAuditTable( TableName ) )
            {
                string AuditTableName = _CswAuditMetaData.makeAuditTableName( TableName );
                if( false == _CswNbtResources.CswResources.DataDictionary.isTableDefined( AuditTableName ) )
                {
                    copyTable( TableName, AuditTableName, false );
                    addStringColumn( AuditTableName, _CswAuditMetaData.AuditEventTypeColName, _CswAuditMetaData.AuditEventTypeColDescription, false, true, _CswAuditMetaData.AuditEventTypeColLength );
                    addForeignKeyColumn( AuditTableName, _CswAuditMetaData.AuditTransactionIdColName, "fk to audittransactions table", false, true, _CswAuditMetaData.AuditTransactionTableName, _CswAuditMetaData.AuditTransactionIdColName );
                    addDateColumn( AuditTableName, _CswAuditMetaData.AuditRecordCreatedColName, _CswAuditMetaData.AuditRecordCreatedColDescription, false, true );
                    addLongColumn( AuditTableName, _CswNbtResources.DataDictionary.getPrimeKeyColumn( TableName ), "prime key of audited record", false, true );

                }//if the audit table does not yet exist

            }//if it isn't already an audit table

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

        public void setTableAuditLevel( string TableName, string WhereClause, AuditLevel AuditLevel )
        {
            if( isTableAuditable( TableName ) )
            {
                CswTableUpdate CswTableUpdate = makeCswTableUpdate( "setTableAuditLevel", TableName );

                DataTable DataTable = null;

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
                    CurrentRow[_CswAuditMetaData.AuditLevelColName] = _CswAuditMetaData.DefaultAuditLevel;
                }

                CswTableUpdate.update( DataTable );

            }//if table is auditable

        }//setTableAuditLevel() 

        public CswNbtNodeCollection Nodes { get { return ( _CswNbtResources.Nodes ); } }
        //public CswNbtTreeCache Trees { get { return ( _CswNbtResources.Trees ); } }
        public CswNbtActionCollection Actions { get { return _CswNbtResources.Actions; } }

        /// <summary>
        /// For making ObjClass files without a Node
        /// </summary>
        public CswNbtObjClass makeObjClass( CswNbtMetaDataObjectClass ObjectClass )
        {
            return CswNbtObjClassFactory.makeObjClass( _CswNbtResources, ObjectClass );
        }

        //public bool IsModuleEnabled( CswNbtResources.CswModule Module )
        //{
        //    return _CswNbtResources.IsModuleEnabled( Module );
        //}

        public CswNbtViewSelect ViewSelect { get { return _CswNbtResources.ViewSelect; } }

        public CswNbtView makeView() { return ( new CswNbtView( _CswNbtResources ) ); }
        public CswNbtView restoreView( CswNbtViewId ViewId ) { return ViewSelect.restoreView( ViewId ); }
        public CswNbtView restoreViewString( string ViewAsString ) { return ViewSelect.restoreView( ViewAsString ); }
        public CswNbtView restoreView( string ViewName )
        {
            CswNbtView ReturnVal = null;

            List<CswNbtView> Views = restoreViews( ViewName );
            if( 1 == Views.Count )
            {
                ReturnVal = Views[0];
            }
            //else if ( 0 == Views.Count )
            //{
            //    throw ( new CswDniException( "No such view: " + ViewName ) );
            //}
            //else if ( Views.Count > 1 )
            //{
            //    throw ( new CswDniException( "There are more than one views having the view name " + ViewName ) );
            //}//

            return ( ReturnVal );
        }//restoreView() 

        //public void ClearCache()
        //{
        //    _CswNbtResources.ClearCache();
        //    _CswNbtResources.CurrentUser = new CswNbtSystemUser( _CswNbtResources, "_SchemaUpdaterUser" );
        //}

        //public CswNbtView getTreeViewOfNodeType( Int32 NodeTypeId ) { return _CswNbtResources.Trees.getTreeViewOfNodeType( NodeTypeId ); }
        //public CswNbtView getTreeViewOfObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass ObjectClass ) { return _CswNbtResources.Trees.getTreeViewOfObjectClass( ObjectClass ); }

        public ICswNbtTree getTreeFromView( CswNbtView View, bool IncludeSystemNodes ) { return _CswNbtResources.Trees.getTreeFromView( View, true, true, false, IncludeSystemNodes ); }
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

        public void deleteView( string ViewName, bool DeleteAllInstances )
        {
            List<CswNbtView> Views = restoreViews( ViewName );

            if( !DeleteAllInstances && Views.Count > 1 )
                throw ( new CswDniException( "There is more than one instance of the specified view: " + ViewName ) );

            foreach( CswNbtView CurrentView in Views )
                CurrentView.Delete();

        }//deleteView()

        //public CswNbtNodeTypePermissions getNodeTypePermissions( CswNbtObjClassRole Role, CswNbtMetaDataNodeType NodeType )
        //{
        //    return new CswNbtNodeTypePermissions( Role, NodeType );
        //}

        //public CswNbtNodeTypePermissions getNodeTypePermissions( string RoleName, string NodeTypeName )
        //{
        //    CswNbtNodeTypePermissions ReturnVal = null;
        //    CswNbtNode RoleNode = Nodes.makeRoleNodeFromRoleName( RoleName );
        //    if( null == RoleNode )
        //        throw ( new CswDniException( "No such role: " + RoleName ) );
        //    CswNbtObjClassRole Role = CswNbtNodeCaster.AsRole( RoleNode );

        //    CswNbtMetaDataNodeType CswNbtMetaDataNodeType = null;
        //    if( null == ( CswNbtMetaDataNodeType = MetaData.getNodeType( NodeTypeName ) ) )
        //        throw ( new CswDniException( "No such nodetype: " + NodeTypeName ) );


        //    ReturnVal = new CswNbtNodeTypePermissions( Role, CswNbtMetaDataNodeType );

        //    return ( ReturnVal );

        //}//getNodeTypePermissions

        public CswNbtPermit Permit { get { return _CswNbtResources.Permit; } }

        /// <summary>
        /// Convenience function for making new Action
        /// </summary>
        public Int32 createAction( CswNbtActionName Name, bool ShowInList, string URL, string Category )
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
                _CswNbtResources.Permit.set( Name, CswNbtNodeCaster.AsRole( RoleNode ), true );
            }
			CswNbtNode RoleNode2 = Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            if( RoleNode2 != null )
            {
                _CswNbtResources.Permit.set( Name, CswNbtNodeCaster.AsRole( RoleNode2 ), true );
            }
            return NewActionId;
        }

        /// <summary>
        /// Convenience function for making new Action
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
            //Int32 NewJctModuleActionClassId = CswConvert.ToInt32(JctRow["jctmoduleactionid"]);
            JctModulesATable.update( JctModulesADataTable );
        }

        ///// <summary>
        ///// Deprecated in favor of SetActionPermission.  Don't use for new scripts.
        ///// </summary>
        //public void GrantActionPermission( CswNbtNode RoleNode, CswNbtActionName ActionName )
        //{
        //    _CswNbtResources.Permit.set( ActionName, RoleNode, true );
        //}

        ///// <summary>
        ///// Grants or revokes permission to an action to a role
        ///// </summary>
        //public void SetActionPermission( CswNbtNode RoleNode, CswNbtActionName ActionName, bool HasAccess )
        //{
        //    if( RoleNode != null )
        //    {
        //        CswNbtNodePropLogicalSet ActionPermissions = ( (CswNbtObjClassRole) CswNbtNodeCaster.AsRole( RoleNode ) ).ActionPermissions;
        //        ActionPermissions.SetValue( CswNbtObjClassRole.ActionPermissionsXValueName,
        //                                    CswNbtAction.ActionNameEnumToString( ActionName ),
        //                                    HasAccess );
        //        ActionPermissions.Save();
        //        RoleNode.postChanges( false );
        //    }
        //}

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
            ModuleRow["enabled"] = CswConvert.ToDbVal( Enabled ); //Probably needs to be off by default.  Leaving on for development.
            ModulesDataTable.Rows.Add( ModuleRow );
            Int32 NewModuleId = CswConvert.ToInt32( ModuleRow["moduleid"] );
            ModulesTable.update( ModulesDataTable );
            return NewModuleId;
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
            //Int32 NewJctModuleObjectClassId = CswConvert.ToInt32(ModuleRow["jctmoduleobjectclassid"]);
            JctModulesOCTable.update( JctModulesOCDataTable );
        }

        /// <summary>
        /// Convenience function for making new jct_module_nodetypes records
        /// </summary>
        public void createModuleNodeTypeJunction( Int32 ModuleId, Int32 NodeTypeId )
        {
            CswTableUpdate JctModulesNTTable = makeCswTableUpdate( "SchemaModTrnsctn_ModuleJunctionUpdate", "jct_modules_nodetypes" );
            DataTable JctModulesNTDataTable = JctModulesNTTable.getEmptyTable();
            DataRow JctRow = JctModulesNTDataTable.NewRow();
            //JctRow["deleted"] = CswConvert.ToDbVal( false );
            JctRow["moduleid"] = ModuleId.ToString();
            JctRow["nodetypeid"] = NodeTypeId.ToString();
            JctModulesNTDataTable.Rows.Add( JctRow );
            //Int32 NewModuleId = CswConvert.ToInt32(ModuleRow["jctmodulenodetypeid"]);
            JctModulesNTTable.update( JctModulesNTDataTable );
        }

        /// <summary>
        /// Convenience function for making new Object Classes
        /// </summary>
        public Int32 createObjectClass( string ObjectClassName, string IconFileName, bool AuditLevel, bool UseBatchEntry )
        {
            if( !ObjectClassName.EndsWith( "Class" ) )
                ObjectClassName += "Class";

            CswTableUpdate ObjectClassTableUpdate = makeCswTableUpdate( "SchemaModTrnsctn_ObjectClassUpdate", "object_class" );
            DataTable NewObjectClassTable = ObjectClassTableUpdate.getEmptyTable();
            DataRow NewOCRow = NewObjectClassTable.NewRow();
            NewOCRow["objectclass"] = ObjectClassName;
            NewOCRow["iconfilename"] = IconFileName;
            NewOCRow["auditlevel"] = CswConvert.ToDbVal( AuditLevel );
            NewOCRow["use_batch_entry"] = CswConvert.ToDbVal( UseBatchEntry );
            NewObjectClassTable.Rows.Add( NewOCRow );
            Int32 NewObjectClassId = CswConvert.ToInt32( NewOCRow["objectclassid"] );
            ObjectClassTableUpdate.update( NewObjectClassTable );
            return NewObjectClassId;
        }

        /// <summary>
        /// Convenience function for making new Object Class Props
        /// </summary>
        public DataRow addObjectClassPropRow( DataTable ObjectClassPropsTable, Int32 ObjectClassId, string PropName,
                                              CswNbtMetaDataFieldType.NbtFieldType FieldType, Int32 DisplayColAdd, Int32 DisplayRowAdd )
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

        /// <summary>
        /// Convenience function for making new Object Class Props with more granular control
        /// </summary>
        public DataRow addObjectClassPropRow( DataTable ObjectClassPropsTable, CswNbtMetaDataObjectClass ObjectClass, string PropName,
                                             CswNbtMetaDataFieldType.NbtFieldType FieldType, bool IsBatchEntry, bool ReadOnly,
                                             bool IsFk, string FkType, Int32 FkValue, bool IsRequired, bool IsUnique, bool IsGlobalUnique,
                                             bool ServerManaged, string ListOptions, Int32 DisplayColAdd, Int32 DisplayRowAdd )
        {
            DataRow OCPRow = ObjectClassPropsTable.NewRow();
            OCPRow["propname"] = PropName;
            OCPRow["fieldtypeid"] = CswConvert.ToDbVal( MetaData.getFieldType( FieldType ).FieldTypeId );
            OCPRow["isbatchentry"] = CswConvert.ToDbVal( IsBatchEntry );
            OCPRow["isfk"] = CswConvert.ToDbVal( IsFk );
            OCPRow["fktype"] = FkType;
            OCPRow["fkvalue"] = CswConvert.ToDbVal( FkValue );
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
        /// Convenience function for adding a new boolean column to the database schema
        /// </summary>
        public void addBooleanColumn( string tablename, string columnname, string description, bool logicaldelete, bool required )//, Int32 NodeTypePropId, string SubFieldName )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, string.Empty, false, DataDictionaryPortableDataType.Boolean, false,
                       required, tablename, DataDictionaryUniqueType.None, false, string.Empty );//, NodeTypePropId, SubFieldName );
        }

        /// <summary>
        /// Convenience function for adding a new foreign key column to the database schema
        /// </summary>
        public void addForeignKeyColumn( string tablename, string columnname, string description, bool logicaldelete, bool required, string foreignkeytable, string foreignkeycolumn )//, Int32 NodeTypePropId, string SubFieldName )
        {
            addColumn( columnname, DataDictionaryColumnType.Fk, Int32.MinValue, Int32.MinValue, string.Empty, description, foreignkeycolumn, foreignkeytable,
                       false, false, logicaldelete, string.Empty, false, DataDictionaryPortableDataType.Long, false,
                       required, tablename, DataDictionaryUniqueType.None, false, string.Empty );//, NodeTypePropId, SubFieldName );
        }

        /// <summary>
        /// Convenience function for adding a new string column to the database schema
        /// </summary>
        public void addStringColumn( string tablename, string columnname, string description, bool logicaldelete, bool required, Int32 datatypesize )//, Int32 NodeTypePropId, string SubFieldName )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, datatypesize, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, string.Empty, false, DataDictionaryPortableDataType.String, false,
                       required, tablename, DataDictionaryUniqueType.None, false, string.Empty );//, NodeTypePropId, SubFieldName );
        }

        /// <summary>
        /// Convenience function for adding a new long column to the database schema
        /// </summary>
        public void addLongColumn( string tablename, string columnname, string description, bool logicaldelete, bool required )
        {
            addLongColumn( tablename, columnname, description, logicaldelete, required, string.Empty, false, string.Empty, false );
        }

        /// <summary>
        /// Convenience function for adding a new long column to the database schema
        /// </summary>
        public void addLongColumn( string tablename, string columnname, string description, bool logicaldelete, bool required,
                                   string LowerRangeValue, bool LowerRangeValueInclusive, string UpperRangeValue, bool UpperRangeValueInclusive )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, LowerRangeValue, LowerRangeValueInclusive, DataDictionaryPortableDataType.Long, false,
                       required, tablename, DataDictionaryUniqueType.None, UpperRangeValueInclusive, UpperRangeValue );
        }

        /// <summary>
        /// Convenience function for adding a new double column to the database schema
        /// </summary>
        public void addDoubleColumn( string tablename, string columnname, string description, bool logicaldelete, bool required, Int32 DblPrecision )
        {
            addDoubleColumn( tablename, columnname, description, logicaldelete, required, DblPrecision, string.Empty, false, string.Empty, false );
        }

        /// <summary>
        /// Convenience function for adding a new double column to the database schema
        /// </summary>
        public void addDoubleColumn( string tablename, string columnname, string description, bool logicaldelete, bool required, Int32 DblPrecision,
                                     string LowerRangeValue, bool LowerRangeValueInclusive, string UpperRangeValue, bool UpperRangeValueInclusive )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, Int32.MinValue, DblPrecision, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, LowerRangeValue, LowerRangeValueInclusive, DataDictionaryPortableDataType.Double, false,
                       required, tablename, DataDictionaryUniqueType.None, UpperRangeValueInclusive, UpperRangeValue );
        }

        /// <summary>
        /// Convenience function for adding a new date column to the database schema
        /// </summary>
        public void addDateColumn( string tablename, string columnname, string description, bool logicaldelete, bool required )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, string.Empty, false, DataDictionaryPortableDataType.Datetime, false,
                       required, tablename, DataDictionaryUniqueType.None, false, string.Empty );
        }

        /// <summary>
        /// Convenience function for adding a new date column to the database schema
        /// </summary>
        public void addBlobColumn( string tablename, string columnname, string description, bool logicaldelete, bool required )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, string.Empty, false, DataDictionaryPortableDataType.Blob, false,
                       required, tablename, DataDictionaryUniqueType.None, false, string.Empty );
        }

        /// <summary>
        /// Convenience function for adding a new date column to the database schema
        /// </summary>
        public void addClobColumn( string tablename, string columnname, string description, bool logicaldelete, bool required )
        {
            addColumn( columnname, DataDictionaryColumnType.Value, Int32.MinValue, Int32.MinValue, string.Empty, description, string.Empty, string.Empty,
                       false, false, logicaldelete, string.Empty, false, DataDictionaryPortableDataType.Clob, false,
                       required, tablename, DataDictionaryUniqueType.None, false, string.Empty );
        }

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
                throw new CswDniException( ErrorType.Error, "No Match for S4 QueryId: " + QueryId, "CswNbtSchemaModTrnsctn::UpdateS4() returned 0 rows for S4 queryid: " + QueryId );
            S4Table.Rows[0]["querytext"] = QueryText;
            S4Update.update( S4Table );

            // Clear cached S4s
            _CswNbtResources.CswResources.ClearCache();
        }

        /// <summary>
        /// Convenience function for setting value of a configuration variable
        /// </summary>
        public void setConfigVariableValue( String VariableName, String VariableValue )
        {
            if( !String.IsNullOrEmpty( VariableValue ) && !String.IsNullOrEmpty( VariableName ) )
            {
                _CswNbtResources.setConfigVariableValue( VariableName, VariableValue );
            }
        }

        public string getConfigVariableValue( String VariableName )
        {
            return ( _CswNbtResources.getConfigVariableValue( VariableName ) );
        }


        public CswNbtActUpdatePropertyValue getCswNbtActUpdatePropertyValue() { return ( new CswNbtActUpdatePropertyValue( _CswNbtResources ) ); }
    }//class CswNbtSchemaModTrnsctn

}//ChemSW.Nbt.Schema
