using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents the CISPro Module
    /// </summary>
    public class CswNbtModuleRuleCISPro : CswNbtModuleRule
    {
        public CswNbtModuleRuleCISPro( CswNbtResources CswNbtResources ) :
            base( CswNbtResources )
        {
        }
        public override CswEnumNbtModuleName ModuleName { get { return CswEnumNbtModuleName.CISPro; } }
        protected override void OnEnable()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                _CswNbtResources.Modules.ShowProp( UserJurisdictionNTP );
            }

            //Case 27862 - show...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            //_CswNbtResources.Modules.ToggleRoleNodes()
            _CswNbtResources.Modules.ToggleRoleNodes( false, "cispro" );
            _CswNbtResources.Modules.ToggleUserNodes( false, "cispro" );
            _CswNbtResources.Modules.ToggleView( false, "Units of Measurement", CswEnumNbtViewVisibility.Global );

            //CISPro_Request_Fulfiller Role/User gets turned on by the above line, but should only be on if Containers is enabled
            _CswNbtResources.Modules.ToggleRoleNodes( true, "request_fulfiller" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "request_fulfiller" );
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.ExtChemDataSync, Disabled: false );
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.TierII, Disabled: false );

            //Mail Reports
            _CswNbtResources.Modules.ToggleNode( false, "Deficient Inventory Levels", CswEnumNbtObjectClass.MailReportClass );
        }

        protected override void OnDisable()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                _CswNbtResources.Modules.HideProp( UserJurisdictionNTP );
            }

            //Case 27862 - hide...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            _CswNbtResources.Modules.ToggleRoleNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleView( true, "Units of Measurement", CswEnumNbtViewVisibility.Global );
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.ExtChemDataSync, Disabled: true );
            _CswNbtResources.Modules.ToggleScheduledRule( CswEnumNbtScheduleRuleNames.TierII, Disabled: true );

            //Mail Reports
            _CswNbtResources.Modules.ToggleNode( true, "Deficient Inventory Levels", CswEnumNbtObjectClass.MailReportClass );
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
