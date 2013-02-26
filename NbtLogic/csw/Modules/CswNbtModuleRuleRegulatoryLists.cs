using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Actions;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the Regulatory Lists Module
    /// </summary>
    public class CswNbtModuleRuleRegulatoryLists : CswNbtModuleRule
    {
        public CswNbtModuleRuleRegulatoryLists( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswNbtModuleName ModuleName { get { return CswNbtModuleName.RegulatoryLists; } }
        public override void OnEnable()
        {
            if( false == _CswNbtResources.Modules.IsModuleEnabled( CswNbtModuleName.CISPro ) )
            {
                _CswNbtResources.Modules.EnableModule( CswNbtModuleName.CISPro );
            }


        }

        public override void OnDisable()
        {

        } // OnDisable()

    } // class CswNbtModuleRegulatoryLists
}// namespace ChemSW.Nbt
