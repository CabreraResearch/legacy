
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

            // case 26827
            _CswNbtSchemaModTrnsctn.addLongColumn( "object_class", "searchdeferpropid", "Defer to the target of this property in search results", false, false );
            _CswNbtSchemaModTrnsctn.addLongColumn( "nodetypes", "searchdeferpropid", "Defer to the target of this property in search results", false, false );

            
            // case 25495
            _acceptBlame( CswDeveloper.SS, 25495 );
            string SearchTableName = "search";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( SearchTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( SearchTableName , "searchid" );
                _CswNbtSchemaModTrnsctn.addStringColumn( SearchTableName, "category", "category for view selector", false, false, 40 );
                _CswNbtSchemaModTrnsctn.addStringColumn( SearchTableName, "name", "name of search", false, false, 80 );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( SearchTableName , "userid", "owner of search", false, false, "nodes", "nodeid" );
                _CswNbtSchemaModTrnsctn.addClobColumn( SearchTableName , "searchdata", "data for building this search", false, false );
            }
            _resetBlame();

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


