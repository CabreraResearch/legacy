using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Factory for Module Rules
    /// </summary>
    public class CswNbtModuleRuleFactory
    {
        public static CswNbtModuleRule makeModuleRule( CswNbtResources CswNbtResources, CswEnumNbtModuleName ModuleName )
        {
            CswNbtModuleRule ret = null;
            if( CswEnumNbtModuleName.CISPro == ModuleName )
            {
                ret = new CswNbtModuleRuleCISPro( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.Dev == ModuleName )
            {
                ret = new CswNbtModuleRuleDev( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.IMCS == ModuleName )
            {
                ret = new CswNbtModuleRuleIMCS( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.MLM == ModuleName )
            {
                ret = new CswNbtModuleRuleMLM( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.NBTManager == ModuleName )
            {
                ret = new CswNbtModuleRuleNBTManager( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.SI == ModuleName )
            {
                ret = new CswNbtModuleRuleSI( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.C3 == ModuleName )
            {
                ret = new CswNbtModuleRuleC3( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.Containers == ModuleName )
            {
                ret = new CswNbtModuleRuleContainers( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.FireCode == ModuleName )
            {
                ret = new CswNbtModuleRuleFireCode( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.SDS == ModuleName )
            {
                ret = new CswNbtModuleRuleSDS( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.RegulatoryLists == ModuleName )
            {
                ret = new CswNbtModuleRuleRegulatoryLists( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.MultiSite == ModuleName )
            {
                ret = new CswNbtModuleRuleMultiSite( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.MultiInventoryGroup == ModuleName )
            {
                ret = new CswNbtModuleRuleMultiInventoryGroup( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.FireDbSync == ModuleName )
            {
                ret = new CswNbtModuleRuleFireDbSync( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.PCIDSync == ModuleName )
            {
                ret = new CswNbtModuleRulePCIDSync( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.ManufacturerLotInfo == ModuleName )
            {
                ret = new CswNbtModuleRuleManufacturerLotInfo( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.LOLISync == ModuleName )
            {
                ret = new CswNbtModuleRuleLOLISync( CswNbtResources );
            }
            else if( CswEnumNbtModuleName.DSD == ModuleName )
            {
                ret = new CswNbtModuleRuleDSD( CswNbtResources );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error,
                                           "Unhandled ModuleName: " + ModuleName.ToString(),
                                           "CswNbtModuleRuleFactory did not recognize module name: " + ModuleName.ToString() );
            }
            return ret;
        }

    } // class CswNbtModuleBioSafety
}// namespace ChemSW.Nbt
