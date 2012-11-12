using System;
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
                    _Message += "Rollback failed: " + CommitException.Message + " at " + CommitException.StackTrace.ToString() + ". \n";
                }//
            }//catch

            catch( Exception Exception )
            {
                _Message += "Script for case " + _CswUpdateSchemaTo.getCaseLink() + " authored by " + _CswUpdateSchemaTo.Author + " failed. \n";
                _Message += Exception.Message + " at: " + Exception.StackTrace.ToString() + ". \n"; ;
                _UpdateSucceeded = false;

                try
                {
                    _CswNbtSchemaModTrnsctn.rollbackTransaction();
                }

                catch( Exception CommitException )
                {
                    _RollbackSucceeded = false;
                    _Message += "Rollback failed: " + CommitException.Message + " at " + CommitException.StackTrace.ToString() + ". \n";
                }//
            }//

        }//beginUpdate()

    }//class CswSchemaUpdateDriver

}//ChemSW.Nbt.Schema
