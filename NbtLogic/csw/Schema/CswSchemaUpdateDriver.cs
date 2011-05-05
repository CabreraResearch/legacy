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
        CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        ICswUpdateSchemaTo _CswUpdateSchemaTo = null;
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
        /// <param name="CswNbtSchemaModTrnsctn"></param>
        /// <param name="CswUpdateSchemaTo"></param>
        public CswSchemaUpdateDriver( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, ICswUpdateSchemaTo CswUpdateSchemaTo )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswUpdateSchemaTo = CswUpdateSchemaTo;
        }//ctor

        public string Description
        {
            get
            {
                return ( _CswUpdateSchemaTo.Description );
            }
        }

        public CswSchemaVersion SchemaVersion { get { return ( _CswUpdateSchemaTo.SchemaVersion ); } }

        public void update()
        {
            try
            {
                _CswNbtSchemaModTrnsctn.refreshDataDictionary();
                _CswNbtSchemaModTrnsctn.beginTransaction();
                _CswUpdateSchemaTo.update();
                _CswNbtSchemaModTrnsctn.commitTransaction();
            }

            catch( CswDniExceptionIgnoreDeliberately CswDniExceptionIgnoreDeliberately )
            {
                _UpdateSucceeded = true;

                try
                {
                    _CswNbtSchemaModTrnsctn.rollbackTransaction();
                }

                catch( Exception CommitException )
                {
                    _RollbackSucceeded = false;
                    _Message += "Rollback failed: " + CommitException.Message + " at " + CommitException.StackTrace.ToString();
                }//
            }//catch

            catch( Exception Exception )
            {
                _Message = Exception.Message + " at: " + Exception.StackTrace.ToString();
                _UpdateSucceeded = false;

                try
                {
                    _CswNbtSchemaModTrnsctn.rollbackTransaction();
                }

                catch( Exception CommitException )
                {
                    _RollbackSucceeded = false;
                    _Message += "Rollback failed: " + CommitException.Message + " at " + CommitException.StackTrace.ToString();
                }//
            }//

        }//beginUpdate()

    }//class CswSchemaUpdateDriver

}//ChemSW.Nbt.Schema
