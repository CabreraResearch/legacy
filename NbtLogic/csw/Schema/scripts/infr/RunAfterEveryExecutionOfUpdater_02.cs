using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Post-schema PL/SQL compile script
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {
        #region Blame Logic

        private CswDeveloper _Author = CswDeveloper.NBT;
        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;
        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public static string Title = "Compile Oracle Objects";

        public override void update()
        {
            #region Synonyms

            //Add Synonyms here

            #endregion Synonyms

            #region Triggers

            //Add Triggers here

            #endregion Triggers

            #region Functions

            foreach( CswUpdateSchemaPLSQLFunctions.Functions Function in CswUpdateSchemaPLSQLFunctions.Functions._All )
            {
                _acceptBlame( Function._Dev, Function._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( Function.ToString() );
                _resetBlame();
            }

            #endregion Functions

            #region Procedures

            foreach( CswUpdateSchemaPLSQLProcedures.Procedures Procedure in CswUpdateSchemaPLSQLProcedures.Procedures._All )
            {
                _acceptBlame( Procedure._Dev, Procedure._CaseNo);
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( Procedure.ToString() );
                _resetBlame();
            }

            #endregion Procedures

            #region Types

            //Because Nested Tables are dependant upon types, they must be dropped before they can be recompiled
            //Normally we would drop types explicitly based on the order of their dependencies, 
            //but that can't be done generically. So instead, we use "force" to avoid ORA-02303.
            #region Drop Types

            foreach( CswUpdateSchemaPLSQLTypes.NestedTables NestedTable in CswUpdateSchemaPLSQLTypes.NestedTables._All )
            {
                _acceptBlame( NestedTable._Dev, NestedTable._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( 
                    @"declare
                      object_not_exists EXCEPTION;
                      PRAGMA EXCEPTION_INIT(object_not_exists, -04043);
                    begin
                      execute immediate 'drop type " + NestedTable._Title + @" force';
                    exception
                      when object_not_exists then null;
                    end;" 
                );
                _resetBlame();
            }

            foreach( CswUpdateSchemaPLSQLTypes.TypeHeaders TypeHeader in CswUpdateSchemaPLSQLTypes.TypeHeaders._All )
            {
                _acceptBlame( TypeHeader._Dev, TypeHeader._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    @"declare
                      object_not_exists EXCEPTION;
                      PRAGMA EXCEPTION_INIT(object_not_exists, -04043);
                    begin
                      execute immediate 'drop type " + TypeHeader._Title + @" force';
                    exception
                      when object_not_exists then null;
                    end;"
                );
                _resetBlame();
            }            

            #endregion Drop Types

            #region Type Headers

            foreach( CswUpdateSchemaPLSQLTypes.TypeHeaders TypeHeader in CswUpdateSchemaPLSQLTypes.TypeHeaders._All )
            {
                _acceptBlame( TypeHeader._Dev, TypeHeader._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( TypeHeader.ToString() );
                _resetBlame();
            }

            #endregion Type Headers

            #region Type Bodies

            //Add Type Bodies here

            #endregion Type Bodies

            #region Nested Tables

            foreach( CswUpdateSchemaPLSQLTypes.NestedTables NestedTable in CswUpdateSchemaPLSQLTypes.NestedTables._All )
            {
                _acceptBlame( NestedTable._Dev, NestedTable._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( NestedTable.ToString() );
                _resetBlame();
            }

            #endregion Nested Tables

            #endregion Types

            #region Package Headers

            foreach( CswUpdateSchemaPLSQLPackages.PackageHeaders PackageHead in CswUpdateSchemaPLSQLPackages.PackageHeaders._All )
            {
                _acceptBlame( PackageHead._Dev, PackageHead._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( PackageHead.ToString() );
                _resetBlame();
            }

            #endregion Package Headers

            #region Package Bodies

            foreach( CswUpdateSchemaPLSQLPackages.PackageBodies PackageBodies in CswUpdateSchemaPLSQLPackages.PackageBodies._All )
            {
                _acceptBlame( PackageBodies._Dev, PackageBodies._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( PackageBodies.ToString() );
                _resetBlame();
            }

            #endregion Package Bodies

            #region Views

            foreach( CswUpdateSchemaPLSQLViews.Views View in CswUpdateSchemaPLSQLViews.Views._All )
            {
                _acceptBlame( View._Dev, View._CaseNo );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( View.ToString() );
                _resetBlame();
            }

            #endregion Views
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_02

}//namespace ChemSW.Nbt.Schema


