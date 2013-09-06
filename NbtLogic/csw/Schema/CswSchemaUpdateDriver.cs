using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdateDriver
    {
        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set { _CswNbtSchemaModTrnsctn = value; }
        }

        CswUpdateSchemaTo _CswUpdateSchemaTo = null;
        private string _Message = "Update Succeeded";
        public string Message { get { return ( _Message ); } }
        private bool _UpdateSucceeded = true;
        public bool UpdateSucceeded { get { return ( _UpdateSucceeded ); } }
        private bool _RollbackSucceeded = true;
        public bool RollbackSucceeded { get { return ( _RollbackSucceeded ); } }

        /// <summary>
        /// Ensure that schema update transactions are handled in a uniform way with respect to 
        /// transaction management and exception reporting, while avoiding creating a circular
        /// dependency between the schema update logic and the transaction class.
        /// </summary>
        /// <param name="CswUpdateSchemaTo"></param>
        public CswSchemaUpdateDriver( CswUpdateSchemaTo CswUpdateSchemaTo )
        {
            _CswUpdateSchemaTo = CswUpdateSchemaTo;
        }//ctor

        public string Title { get { return _CswUpdateSchemaTo.Title; } }

        public string Description
        {
            set
            {
                _CswUpdateSchemaTo.Description = value;
            }
            get
            {
                return ( _CswUpdateSchemaTo.Description );
            }
        }

        CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { set { _CswSchemaVersion = value; } get { return ( _CswSchemaVersion ); } }

        /// <summary>
        /// Returns true if the script has already been run sucessfully.
        /// </summary>
        /// <returns></returns>
        public bool AlreadyRun()
        {
            bool Ret = false;
            CswTableSelect ts = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "HasScriptAlreadyRun", "update_history" );
            DataTable dt = ts.getTable( "where scriptname = '" + _CswUpdateSchemaTo.ScriptName + "' and succeeded = 1" );
            if( dt.Rows.Count > 0 )
            {
                Ret = true;
            }

            return Ret;
        }//AlreadyRun()

        /// <summary>
        /// Returns _CswUpdateSchemaTo.ScriptName
        /// </summary>
        public string ScriptName
        {
            get { return _CswUpdateSchemaTo.ScriptName; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AlwaysRun
        {
            get { return _CswUpdateSchemaTo.AlwaysRun; }
        }

        public void update()
        {
            try
            {
                _CswNbtSchemaModTrnsctn.refreshDataDictionary();
                _CswNbtSchemaModTrnsctn.beginTransaction();

                _CswUpdateSchemaTo.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
                _CswUpdateSchemaTo.update();

                _CswUpdateSchemaTo.CswNbtSchemaModTrnsctn = null;

                _CswNbtSchemaModTrnsctn.commitTransaction();
            }

            catch( CswDniExceptionIgnoreDeliberately )
            {
                _UpdateSucceeded = true;
                try
                {
                    _CswNbtSchemaModTrnsctn.rollbackTransaction();
                }

                catch( Exception CommitException )
                {
                    _RollbackSucceeded = false;
                    _Message += "Rollback failed: " + CommitException.Message + " at " + CommitException.StackTrace.ToString() + ". \r\n";
                }//
            }//catch

            catch( Exception Exception )
            {
                _Message += "Script for case " + _CswUpdateSchemaTo.getCaseLink() + " authored by " + _CswUpdateSchemaTo.Author + " failed. \r\n";
                _Message += Exception.Message + " at: " + Exception.StackTrace.ToString() + ". \r\n";
                _UpdateSucceeded = false;

                try
                {
                    _CswNbtSchemaModTrnsctn.rollbackTransaction();
                }

                catch( Exception CommitException )
                {
                    _RollbackSucceeded = false;
                    _Message += "Rollback failed: " + CommitException.Message + " at " + CommitException.StackTrace.ToString() + ". \r\n";
                }//
            }//

        }//beginUpdate()

    }//class CswSchemaUpdateDriver

}//ChemSW.Nbt.Schema
