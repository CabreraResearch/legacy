using System;
using ChemSW.Audit;
using ChemSW.Config;
using ChemSW.MtSched.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-06
    /// </summary>
    public class CswUpdateSchemaTo01L06 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 06 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region Case 24415

            _CswNbtSchemaModTrnsctn.createScheduledRule( NbtScheduleRuleNames.DisableChemSwAdmin, Recurrence.Daily, 1 );

            CswNbtMetaDataObjectClass CustomerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CustomerOc.ObjectClass,
                                                           CswNbtObjClassCustomer.LoginPropertyName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                           false, true, false, CswNbtViewProperty.CswNbtPropType.Unknown, Int32.MinValue, false, false, false, true, string.Empty, Int32.MinValue, Int32.MinValue,
                                                           CswNbtNodePropButton.ButtonMode.button.ToString(),
                                                           false,
                                                           AuditLevel.NoAudit,
                                                           "Login"
                );


            CswNbtMetaDataObjectClassProp ChemSwAdminPasswordOcp = CustomerOc.getObjectClassProp( "ChemSW Admin Password" );
            if( null != ChemSwAdminPasswordOcp )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ChemSwAdminPasswordOcp );
            }

            #endregion Case 24415

            #region Case 24431

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
            CswNbtAction ViewRulesAction = _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.View_Scheduled_Rules];
            foreach( CswNbtNode UserNode in UserOc.getNodes( true, false ) )
            {
                CswNbtObjClassUser NodeAsUser = CswNbtNodeCaster.AsUser( UserNode );
                bool CanUse = ( NodeAsUser.Username == CswNbtObjClassUser.ChemSWAdminUsername );
                _CswNbtSchemaModTrnsctn.Permit.set( ViewRulesAction, NodeAsUser, CanUse );
            }

            #endregion Case 24431

            #region Case 24242

            _CswNbtSchemaModTrnsctn.createConfigurationVariable( CswConfigurationVariables.ConfigurationVariableNames.NotifyOnSystemFailure, "Send Email Notification on Failure Events", string.Empty, false );

            #endregion Case 24242



        }//Update()

    }//class CswUpdateSchemaTo01L06

}//namespace ChemSW.Nbt.Schema


