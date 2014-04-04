using System.Data;
using System.Threading;
using ChemSW.DB;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the ACCL Direct Structure Search Module
    /// </summary>
    /// <remarks>
    /// Enables structure search and image rendering using the Accelrys Direct cartridge
    /// </remarks>
    public class CswNbtModuleRuleDirectStructureSearch: CswNbtModuleRule
    {
        public CswNbtModuleRuleDirectStructureSearch( CswNbtResources CswNbtResources ) : base( CswNbtResources ) { }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.DirectStructureSearch; } }

        /// <summary>
        /// Returns whether the Direct Structure Search Module can be enabled. Checks for the prescence of the c$direct90 user.
        /// </summary>
        public override string CanModuleBeEnabled()
        {
            string ret = string.Empty;
            if( false == _CswNbtResources.AcclDirect.IsDirectInstalled() )
            {
                ret = "Direct is not installed, cannot enable the Direct Structure Search Module";
            }
            return ret;
        }

        protected override void OnEnable()
        {
            if( false == _CswNbtResources.AcclDirect.IsDirectInitialized() )
            {
                //Init direct
                _setupDirect();

                //Create the mol_data_molidx if it's not already present
                if( false == _doesTblIdxExist( "mol_data_molidx" ) )
                {
                    /* Users can run structure searches without this index. The index just makes searches faster.
                     * When a user enables the module we know it's safe to use ACCL Direct functions, so it's a 
                     * good time to create the index if it doesn't already exist and we don't really care when 
                     * the index is finished being created so we can spin it off into it's own thread
                     */
                    Thread setupDirectThread = new Thread( _createMolIdx );
                    setupDirectThread.Start();
                }
            }
        }

        protected override void OnDisable()
        {
            if( _CswNbtResources.AcclDirect.IsDirectInstalled() && _CswNbtResources.AcclDirect.IsDirectInitialized() )
            {
                _unsetupDirect();
            }
        } // OnDisable()

        private void _setupDirect()
        {
            //this inits Direct for the user, it it safe to run over and over again
            _CswNbtResources.execArbitraryPlatformNeutralSql(
                    @"begin
                        c$direct90.mdlauxop.setup;
                      end;" );
        }

        private void _unsetupDirect()
        {
            //this un-inits Direct for the user, it it safe to run over and over again
            _CswNbtResources.execArbitraryPlatformNeutralSql(
                    @"begin
                        c$direct90.mdlauxop.unsetup;
                      end;" );
        }

        private void _createMolIdx()
        {
            CswNbtResources tempResources = CswNbtResourcesFactory.makeCswNbtResources( _CswNbtResources );
            tempResources.AccessId = _CswNbtResources.AccessId;
            tempResources.execArbitraryPlatformNeutralSql( @"create index mol_data_molidx
                                                                        on mol_data(ctab)
                                                                        indextype is c$direct90.mxixmdl" );
        }

        private bool _doesTblIdxExist( string IdxName )
        {
            CswArbitrarySelect idxSelect = _CswNbtResources.makeCswArbitrarySelect( "ModuleRuleDirectStructureSearch.doesIdxExist", "select * from user_indexes ui where lower(ui.index_name) = '" + IdxName + "'" );
            DataTable indexes = idxSelect.getTable();
            return indexes.Rows.Count > 0;
        }

    } // class CswNbtModuleRuleDirectStructureSearch
}// namespace ChemSW.Nbt
