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

            #region Package Headers

            //Add Package Headers here

            #endregion Package Headers

            #region Package Bodies

            //Add Package Bodies here

            #endregion Package Bodies

            #region Type Headers

            //Add Type Headers here

            #endregion Type Headers

            #region Type Bodies

            //Add Type Bodies here

            #endregion Type Bodies

            #region Nested Tables

            //Add Nested Tables here

            #endregion Nested Tables

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


