using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswDDL
    {
        Dictionary<string, CswTableDdlOp> _DdlOps = new Dictionary<string, CswTableDdlOp>();
        private CswConstraintDdlOps _CswConstraintDdlOps = null;
        private CswNbtResources _CswNbtResources;

        /// <summary>
        /// Constructor
        /// </summary>
        public CswDDL( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _CswConstraintDdlOps = new CswConstraintDdlOps( _CswNbtResources );
        }//ctor


        public void clear()
        {
            _DdlOps.Clear();
            _CswConstraintDdlOps.clear();
        }//_clear()


        //private bool _ManageConstraints = true;
        //public bool ManageConstraints
        //{
        //    set
        //    {
        //        _ManageConstraints = value;
        //        foreach ( CswTableDdlOp CurrentOp in _DdlOps.Values )
        //        {
        //            CurrentOp.ManageConstraints = value;
        //        }
        //    }

        //    get { return ( _ManageConstraints ); }

        //}//ManageConstraints

        private CswTableDdlOp _makeTableDdlOp( CswConstraintDdlOps CswConstraintDdlOps, string TableName )
        {
            CswTableDdlOp ReturnVal = new CswTableDdlOp( _CswNbtResources, CswConstraintDdlOps, TableName );
            return ( ReturnVal );
        }//

        public void addTable( string TableName, string FkColumnName )
        {

            if( _DdlOps.ContainsKey( TableName ) && CswEnumDdlTableOpType.Drop == _DdlOps[TableName].DdlTableOpType )
                throw ( new CswDniException( "Table " + TableName + " cannot be added because it is already being dropped " ) );

            if( _CswNbtResources.CswResources.isTableDefinedInMetaData( TableName ) )
                throw ( new CswDniException( "Cannot add table " + TableName + " because a table by that name already exists" ) );

            if( !_DdlOps.ContainsKey( TableName ) )
            {
                _DdlOps.Add( TableName, _makeTableDdlOp( _CswConstraintDdlOps, TableName ) );

                _DdlOps[TableName].DdlTableOpType = CswEnumDdlTableOpType.Add;
                _DdlOps[TableName].PkColumnName = FkColumnName;
            }
            else
            {
                if( CswEnumDdlTableOpType.Column == _DdlOps[TableName].DdlTableOpType )
                    _DdlOps[TableName].DdlTableOpType = CswEnumDdlTableOpType.Add;
            }//

            _DdlOps[TableName].apply();




        }//addTable()

        public void dropTable( string TableName )
        {
            if( _DdlOps.ContainsKey( TableName ) && CswEnumDdlTableOpType.Add == _DdlOps[TableName].DdlTableOpType )
            {
                throw ( new CswDniException( "Table " + TableName + " cannot be dropped because it is already being added" ) );
            }

            if( !_CswNbtResources.CswResources.isTableDefinedInMetaData( TableName ) )
            {
                throw ( new CswDniException( "Cannot drop table " + TableName + " because no table by that name exists" ) );
            }

            if( !_DdlOps.ContainsKey( TableName ) )
            {
                _DdlOps.Add( TableName, _makeTableDdlOp( _CswConstraintDdlOps, TableName ) );
                _DdlOps[TableName].DdlTableOpType = CswEnumDdlTableOpType.Drop;
            }
            else
            {
                if( CswEnumDdlTableOpType.Column == _DdlOps[TableName].DdlTableOpType )
                {
                    _DdlOps[TableName].DdlTableOpType = CswEnumDdlTableOpType.Add;
                }
            }

            _DdlOps[TableName].apply();

        }//dropTable()

        private void _verifyOrCreateTableForColumnOp( string TableName )
        {
            if( _DdlOps.ContainsKey( TableName ) )
            {
                if( CswEnumDdlTableOpType.Drop == _DdlOps[TableName].DdlTableOpType )
                    throw ( new CswDniException( "Table " + TableName + " is being dropped; you cannot add columns to it" ) );
            }
            else
            {
                if( !_CswNbtResources.CswResources.isTableDefinedInMetaData( TableName ) )
                    throw ( new CswDniException( "No such table: " + TableName ) );

                _DdlOps.Add( TableName, _makeTableDdlOp( _CswConstraintDdlOps, TableName ) );
                _DdlOps[TableName].DdlTableOpType = CswEnumDdlTableOpType.Column;

                _DdlOps[TableName].apply();

            }//if-else ddl op is defined for table

        }//_verifyTableForColumnOp()

        public void addColumn( string columnname, CswEnumDataDictionaryColumnType columntype, Int32 datatypesize, Int32 dblprecision,
                               string defaultvalue, string description, string foreignkeycolumn, string foreignkeytable, bool constrainfkref, bool isview,
                               string lowerrangevalue, bool lowerrangevalueinclusive, CswEnumDataDictionaryPortableDataType portabledatatype, bool ReadOnly,
                               bool Required, string tablename, CswEnumDataDictionaryUniqueType uniquetype, bool uperrangevalueinclusive, string upperrangevalue )
        {
            _verifyOrCreateTableForColumnOp( tablename );
            _DdlOps[tablename].addColumn( columnname, columntype, datatypesize, dblprecision,
                                          defaultvalue, description, foreignkeycolumn, foreignkeytable, constrainfkref, isview,
                                          lowerrangevalue, lowerrangevalueinclusive, portabledatatype, ReadOnly,
                                          Required, uniquetype, uperrangevalueinclusive, upperrangevalue );
            _DdlOps[tablename].apply();

            // case 29565 - also add new column to shadow table
            if( _CswNbtResources.CswResources.isTableAuditable( tablename ) && false == isview )
            {
                string audittablename = _CswAuditMetaData.makeAuditTableName( tablename );
                _verifyOrCreateTableForColumnOp( audittablename );
                _DdlOps[audittablename].addColumn( columnname, columntype, datatypesize, dblprecision,
                                                   "", description, "", "", false, isview,
                                                   "", false, portabledatatype, false,
                                                   false, audittablename, false, "" );
                _DdlOps[audittablename].apply();
            }
        }//addColumn()

        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();

        public void dropColumn( string TableName, string ColumnName )
        {
            _verifyOrCreateTableForColumnOp( TableName );

            _DdlOps[TableName].dropColumn( ColumnName );

            _DdlOps[TableName].apply();

        }//addColumn()


        public void renameColumn( string TableName, string OriginalColumnName, string NewColumnName )
        {
            _verifyOrCreateTableForColumnOp( TableName );
            _DdlOps[TableName].renameColumn( OriginalColumnName, NewColumnName );
            _DdlOps[TableName].apply();

            // case 29565 - also rename the column in the shadow table
            if( _CswNbtResources.CswResources.isTableAuditable( TableName ) )
            {
                string audittablename = _CswAuditMetaData.makeAuditTableName( TableName );
                _verifyOrCreateTableForColumnOp( audittablename );
                _DdlOps[audittablename].renameColumn( OriginalColumnName, NewColumnName );
                _DdlOps[audittablename].apply();
            }
        }//renameColumn() 


        public void indexColumn( string TableName, string ColumnName, string IndexNameIn = null )
        {
        }

        public bool doesS4Exist( string S4Name )
        {
            bool Ret = false;

            CswTableSelect S4Ts = _CswNbtResources.makeCswTableSelect( "unqique_static_sql_selects_queryids", "static_sql_selects" );
            DataTable Data = S4Ts.getTable( " where lower(queryid) = '" + S4Name.ToLower().Trim() + "' " );
            if( Data.Rows.Count > 0 )
            {
                Ret = true;
            }
            return Ret;
        }

        struct Constraint
        {
            public string ReferencingTable;
            public string ReferencingColumn;
            public string ConstraintName;
        }
        private List<Constraint> _Cosntraints = new List<Constraint>();
        public string makeConstraint( string ReferencingTableNameName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, bool AddDdData )
        {
            string ReturnVal = string.Empty;

            _CswConstraintDdlOps.add( ReferencingTableNameName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, AddDdData );
            _CswConstraintDdlOps.apply( ReferencingTableNameName, ReferencingColumnName );

            ReturnVal = _CswConstraintDdlOps.getConstraintName( ReferencingTableNameName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName );

            if( string.Empty == ReturnVal )
                throw ( new CswDniException( "A constraint name was not reported" ) );


            Constraint AddedConstraint = new Constraint();
            AddedConstraint.ReferencingTable = ReferencedTableName;
            AddedConstraint.ReferencingColumn = ReferencedColumnName;
            AddedConstraint.ConstraintName = ReturnVal;


            _Cosntraints.Add( AddedConstraint );

            return ( ReturnVal );

        }//makeConstraint()


        public void removeConstraint( string ReferencingTableNameName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName, string ConstraintName )
        {
            _CswConstraintDdlOps.removeConstraint( ReferencingTableNameName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName, ConstraintName );
            _CswConstraintDdlOps.apply( ReferencingTableNameName, string.Empty );

            Constraint RemovedConstraint = new Constraint();
            RemovedConstraint.ReferencingTable = ReferencedTableName;
            RemovedConstraint.ReferencingColumn = ReferencedColumnName;
            _Cosntraints.Add( RemovedConstraint );

        }//removeConstraint()

        //public void removeConstraint( string ReferencingTableName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName )
        //{
        //    _CswConstraintDdlOps.removeConstraint( ReferencingTableName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName );
        //    _CswConstraintDdlOps.apply( ReferencingTableName, ReferencingColumnName );
        //}//removeConstraint()


        public bool doesConstraintExistInDb( string ConstraintName )
        {
            return ( _CswConstraintDdlOps.doesFkConstraintExistInDb( ConstraintName ) );
        }//doesConstraintExist()

        public List<CswTableConstraint> getConstraints( string ReferencingTableNameName, string ReferencingColumnName, string ReferencedTableName, string ReferencedColumnName )
        {
            return ( _CswConstraintDdlOps.getConstraints( ReferencingTableNameName, ReferencingColumnName, ReferencedTableName, ReferencedColumnName ) );
        }


        public void confirm()
        {
            foreach( CswTableDdlOp CurrentTableDdlOp in _DdlOps.Values )
            {
                CurrentTableDdlOp.confirm();
            }//iterate ops
        }//apply() 

        public void revert()
        {


            foreach( Constraint CurrentAddedContraint in _Cosntraints )
            {
                _CswConstraintDdlOps.revert( CurrentAddedContraint.ReferencingTable, CurrentAddedContraint.ReferencingColumn );
            }

            foreach( CswTableDdlOp CurrentTableDdlOp in _DdlOps.Values )
            {
                CurrentTableDdlOp.revert();
            }//iterate ops

        }//revert() 



    }//class CswDDL

}//ChemSW.Nbt.Schema
