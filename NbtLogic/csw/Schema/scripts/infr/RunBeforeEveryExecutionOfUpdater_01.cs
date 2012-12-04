
using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.StructureSearch;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo = 0;

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

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            #region URSULA

            _makeMolKeysTable();

            #endregion URSULA

            #region VIOLA

            #endregion VIOLA

        }//Update()

        private void _makeMolKeysTable()
        {
            #region Create fingerprint table
            _acceptBlame( CswDeveloper.MB, 24524 );
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( "mol_keys" ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( "mol_keys", "nodeid" );

                for( int i = 0; i < CswStructureSearch.keySize; i++ )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "mol_keys", "key" + i, "key" + i + "for the mol fingerprint", false, false );
                }

                _CswNbtSchemaModTrnsctn.addLongColumn( "mol_keys", "atomcount", "the total number of atoms in this mol fingerprint", false, false );
            }
            _resetBlame();
            #endregion
        }

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


