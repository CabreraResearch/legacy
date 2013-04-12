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
        public override void OnEnable()
        {
            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                if( null != UserJurisdictionNTP )
                {
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
                    UserJurisdictionNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, UserNT.getFirstNodeTypeTab().TabId );
                }
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
        }

        public override void OnDisable()
        {
            //Disable dependent modules
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.Containers ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.Containers );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.SDS ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.SDS );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.RegulatoryLists ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.RegulatoryLists );
            }
            if( _CswNbtResources.Modules.IsModuleEnabled( CswEnumNbtModuleName.C3 ) )
            {
                _CswNbtResources.Modules.DisableModule( CswEnumNbtModuleName.C3 );
            }

            CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            foreach( CswNbtMetaDataNodeType UserNT in UserOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp UserJurisdictionNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.Jurisdiction );
                UserJurisdictionNTP.removeFromAllLayouts();
            }

            //Case 27862 - hide...
            //   All CISPro roles and users
            //   Unit of measure and work units views
            _CswNbtResources.Modules.ToggleRoleNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleUserNodes( true, "cispro" );
            _CswNbtResources.Modules.ToggleView( true, "Units of Measurement", CswEnumNbtViewVisibility.Global );
        } // OnDisable()

    } // class CswNbtModuleCISPro
}// namespace ChemSW.Nbt
