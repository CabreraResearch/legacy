using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01: CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        #region Blame Logic

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;

        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!
            
            #region CEDAR

            _deleteNodeTypeTabSetIdColumn( CswEnumDeveloper.BV, 29898 );

            #endregion CEDAR

            #region DOGWOOD

            #endregion DOGWOOD

        }//Update()        

        #region CEDAR Methods

        private void _deleteNodeTypeTabSetIdColumn( CswEnumDeveloper BlameMe, Int32 CaseNum )
        {
            _acceptBlame( BlameMe, CaseNum );

            const string NodeTypeTabSetId = "nodetypetabsetid";
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", NodeTypeTabSetId ) )
            {
                _CswNbtSchemaModTrnsctn.dropColumn( "nodetype_props", NodeTypeTabSetId );
            }

            _resetBlame();
        }

        #endregion CEDAR Methods
        
        #region DOGWOOD Methods

        #endregion DOGWOOD Methods


    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


