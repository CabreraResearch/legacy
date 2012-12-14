
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

            _createTierIITable( CswDeveloper.BV, 28247 );

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

        private void _createTierIITable( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( "tier2" ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( "tier2", "tier2id" );
                _CswNbtSchemaModTrnsctn.getNewPrimeKey("tier2");
            }

            if( _CswNbtSchemaModTrnsctn.isTableDefined( "tier2" ) )
            {
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "dateadded" ) )
                {
                    _CswNbtSchemaModTrnsctn.addDateColumn( "tier2", "dateadded", "Date added", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "locationid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "tier2", "locationid", "PK of the Location", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "parentlocationid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "tier2", "parentlocationid", "PK of the parent Location", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "materialid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "tier2", "materialid", "PK of the Material", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "casno" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( "tier2", "casno", "Material's CASNo", false, false, 20 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "quantity" ) )
                {
                    _CswNbtSchemaModTrnsctn.addDoubleColumn( "tier2", "quantity", "Quantity of the Material in the given Location", false, false, 6 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "tier2", "totalquantity" ) )
                {
                    _CswNbtSchemaModTrnsctn.addDoubleColumn( "tier2", "totalquantity", "Quantity of the Material in the given Location and all child locations", false, false, 6 );
                }
            }

            _resetBlame();
        }

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


