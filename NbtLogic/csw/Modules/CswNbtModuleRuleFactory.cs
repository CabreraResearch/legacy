using System;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Factory for Module Rules
    /// </summary>
    public class CswNbtModuleRuleFactory
    {
        public static CswNbtModuleRule makeModuleRule( CswNbtModuleName ModuleName )
        {
            CswNbtModuleRule ret = null;
            if( CswNbtModuleName.BioSafety == ModuleName )
            {
                ret = new CswNbtModuleRuleBioSafety();
            }
            else if( CswNbtModuleName.CCPro == ModuleName )
            {
                ret = new CswNbtModuleRuleCCPro();
            }
            else if( CswNbtModuleName.CISPro == ModuleName )
            {
                ret = new CswNbtModuleRuleCISPro();
            }
            else if( CswNbtModuleName.IMCS == ModuleName )
            {
                ret = new CswNbtModuleRuleIMCS();
            }
            else if( CswNbtModuleName.Mobile == ModuleName )
            {
                ret = new CswNbtModuleRuleMobile();
            }
            else if( CswNbtModuleName.NBTManager == ModuleName )
            {
                ret = new CswNbtModuleRuleNBTManager();
            }
            else if( CswNbtModuleName.SI == ModuleName )
            {
                ret = new CswNbtModuleRuleSI();
            }
            else if( CswNbtModuleName.STIS == ModuleName )
            {
                ret = new CswNbtModuleRuleSTIS();
            }
            else
            {
                throw new CswDniException( ErrorType.Error, 
                                           "Unhandled ModuleName: "+ ModuleName.ToString(), 
                                           "CswNbtModuleRuleFactory did not recognize module name: "+ ModuleName.ToString());
            }
            return ret;
        }
    
    } // class CswNbtModuleBioSafety
}// namespace ChemSW.Nbt
