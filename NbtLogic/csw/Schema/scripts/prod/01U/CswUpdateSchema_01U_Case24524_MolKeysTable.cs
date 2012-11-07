using ChemSW.Nbt.csw.Dev;
using ChemSW.StructureSearch;
using ChemSW.DB;
using ChemSW.Nbt.Sched;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24524
    /// </summary>
    public class CswUpdateSchema_01U_Case24524_MolKeysTable : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 24524; }
        }

        public override void update()
        {
            #region Create fingerprint table
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( "mol_keys" ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( "mol_keys", "nodeid" );

                for( int i = 0; i < CswStructureSearch.keySize; i++ )
                {
                    _CswNbtSchemaModTrnsctn.addNumberColumn( "mol_keys", "key" + i, "", false, false, string.Empty, false, string.Empty, false );
                }

                _CswNbtSchemaModTrnsctn.addNumberColumn( "mol_keys", "atomcount", "", false, false, string.Empty, false, string.Empty, false );
            }
            #endregion

            #region Fingerprinting scheduled task

            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.MolFingerprints, Recurrence.NSeconds, 15 );

            #endregion
        }

        //Update()

    }//class CswUpdateSchemaCase24524

}//namespace ChemSW.Nbt.Schema