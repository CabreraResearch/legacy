using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

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

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            #region YORICK

            _createExcludeInQuotaBarColumns( CswDeveloper.MB, 28752 );

            #endregion YORICK

        }//Update()

        #region Yorick

        private void _createExcludeInQuotaBarColumns( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string ExcludeInQuotaBarColName = "excludeinquotabar";

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class", ExcludeInQuotaBarColName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class", ExcludeInQuotaBarColName, "Whether this ObjectClass is excluded when determining the quota bar size", false, true );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", ExcludeInQuotaBarColName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", ExcludeInQuotaBarColName, "Whether this NodeType is excluded when determining the quota bar size", false, true );
            }

            _resetBlame();
        }
        
        #endregion Yorick

    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


