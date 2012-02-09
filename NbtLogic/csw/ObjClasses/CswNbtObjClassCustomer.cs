using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCustomer : CswNbtObjClass
    {
        public static string IPFilterRegexPropertyName { get { return "IP Filter Regex"; } }
        public static string SubscriptionExpirationDatePropertyName { get { return "Subscription Expiration Date"; } }
        public static string DeactivatedPropertyName { get { return "Deactivated"; } }
        public static string CompanyIDPropertyName { get { return "Company ID"; } }
        public static string UserCountPropertyName { get { return "User Count"; } }
        public static string ModulesEnabledPropertyName { get { return "Modules Enabled"; } }
        public static string LoginPropertyName { get { return "Login"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassCustomer( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public CswNbtObjClassCustomer( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass ); }
        }

        private bool _CompanyIDDefined()
        {
            return ( CompanyID.Text != string.Empty && _CswNbtResources.CswDbCfgInfo.ConfigurationExists( CompanyID.Text ) );
        }

        private CswNbtResources makeOtherResources()
        {
            CswNbtResources OtherResources = CswNbtResourcesFactory.makeCswNbtResources( _CswNbtResources );
            OtherResources.AccessId = CompanyID.Text;
            OtherResources.InitCurrentUser = InitUser;
            return OtherResources;
        }
        public ICswUser InitUser( ICswResources Resources )
        {
            ICswUser RetUser = new CswNbtSystemUser( _CswNbtResources, "CswNbtObjClassCustomer_SystemUser" );
            return RetUser;
        }

        private void finalizeOtherResources( CswNbtResources OtherResources )
        {
            OtherResources.finalize();
        }


        #region Inherited Events

        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _checkForConfigFileUpdate();
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _doConfigFileUpdate();
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()


        public override void beforeWriteNode( bool OverrideUniqueValidation )
        {
            _checkForConfigFileUpdate();
            _CswNbtObjClassDefault.beforeWriteNode( OverrideUniqueValidation );
        }

        public override void afterWriteNode()
        {
            _doConfigFileUpdate();

            if( _CompanyIDDefined() )
            {

                if( ModulesEnabled.WasModified )
                {
                    CswCommaDelimitedString NewModulesEnabled = new CswCommaDelimitedString();
                    if( ModulesEnabled.WasModified )
                    {
                        foreach( string ModuleName in ModulesEnabled.YValues )
                        {
                            if( ModulesEnabled.CheckValue( ModulesEnabledXValue, ModuleName ) )
                            {
                                NewModulesEnabled.Add( ModuleName );
                            }
                        }
                    }

                    // switch to target schema
                    //string OriginalAccessId = _CswNbtResources.AccessId;
                    //_CswNbtResources.AccessId = CompanyID.Text;
                    CswNbtResources OtherResources = makeOtherResources();

                    if( NewModulesEnabled.Count > 0 )
                    {
                        CswTableUpdate ModulesUpdate = OtherResources.makeCswTableUpdate( "CswNbtObjClassCustomer_modules_update", "modules" );
                        DataTable ModulesTable = ModulesUpdate.getTable();
                        foreach( DataRow ModulesRow in ModulesTable.Rows )
                        {
                            ModulesRow["enabled"] = CswConvert.ToDbVal( NewModulesEnabled.Contains( ModulesRow["name"].ToString() ) );
                        }
                        ModulesUpdate.update( ModulesTable );
                    }

                    // reconnect to original schema
                    //_CswNbtResources.AccessId = OriginalAccessId;
                    finalizeOtherResources( OtherResources );
                } // if( ChemSWAdminPassword.WasModified || ModulesEnabled.WasModified )
            } // if( _CompanyIDDefined() )

            _CswNbtObjClassDefault.afterWriteNode();
        }

        bool UpdateConfigFile = false;
        private void _checkForConfigFileUpdate()
        {
            if( ( Deactivated.WasModified || IPFilterRegex.WasModified || UserCount.WasModified ) && _CompanyIDDefined() )
            {
                UpdateConfigFile = true;
            }
        }

        private void _doConfigFileUpdate()
        {
            if( UpdateConfigFile )
            {
                // Update the value of Deactivated and IP Filter Regex in the DbConfig file
                _CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CompanyID.Text );
                _CswNbtResources.CswDbCfgInfo.CurrentDeactivated = ( Deactivated.Checked == Tristate.True );
                _CswNbtResources.CswDbCfgInfo.CurrentIPFilterRegex = IPFilterRegex.Text;
                if( !Double.IsNaN( UserCount.Value ) )   // BZ 7822
                    _CswNbtResources.CswDbCfgInfo.CurrentUserCount = UserCount.Value.ToString();
                UpdateConfigFile = false;
            }
        }

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();
        }

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }

        public override void afterPopulateProps()
        {
            // Set property values according to the value in the DbConfig file or from the target schema
            if( _CompanyIDDefined() )
            {
                // get data from DbConfig file
                _CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CompanyID.Text );
                if( _CswNbtResources.CswDbCfgInfo.CurrentDeactivated )
                    Deactivated.Checked = Tristate.True;
                else
                    Deactivated.Checked = Tristate.False;
                IPFilterRegex.Text = _CswNbtResources.CswDbCfgInfo.CurrentIPFilterRegex;
                if( CswTools.IsInteger( _CswNbtResources.CswDbCfgInfo.CurrentUserCount ) )
                    UserCount.Value = CswConvert.ToInt32( _CswNbtResources.CswDbCfgInfo.CurrentUserCount );
                else
                    UserCount.Value = Double.NaN;


                CswCommaDelimitedString YValues = new CswCommaDelimitedString();
                foreach( CswNbtResources.CswNbtModule Module in Enum.GetValues( typeof( CswNbtResources.CswNbtModule ) ) )
                {
                    YValues.Add( Module.ToString() );
                }
                ModulesEnabled.YValues = YValues;
                ModulesEnabled.XValues = new CswCommaDelimitedString() { ModulesEnabledXValue };

                // get data from target schema
                //string OriginalAccessId = _CswNbtResources.AccessId;
                //_CswNbtResources.AccessId = CompanyID.Text;
                CswNbtResources OtherResources = makeOtherResources();

                Collection<CswNbtResources.CswNbtModule> Modules = new Collection<CswNbtResources.CswNbtModule>();
                foreach( CswNbtResources.CswNbtModule Module in OtherResources.ModulesEnabled() )
                {
                    Modules.Add( Module );
                }

                // reconnect to original schema
                //_CswNbtResources.AccessId = OriginalAccessId;
                finalizeOtherResources( OtherResources );

                foreach( CswNbtResources.CswNbtModule Module in Enum.GetValues( typeof( CswNbtResources.CswNbtModule ) ) )
                {
                    ModulesEnabled.SetValue( ModulesEnabledXValue, Module.ToString(), Modules.Contains( Module ) );
                }
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public static string ModulesEnabledXValue = "Enabled";

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ActionObj )
        {
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                if( LoginPropertyName == OCP.PropName )
                {
                    ActionObj["action"] = CswNbtMetaDataObjectClass.OnButtonClickEvents.reauthenticate.ToString();
                }
            }
        }


        #endregion

        #region Object class specific properties

        public CswNbtNodePropText IPFilterRegex
        {
            get
            {
                return ( _CswNbtNode.Properties[IPFilterRegexPropertyName].AsText );
            }
        }
        public CswNbtNodePropDateTime SubscriptionExpirationDate
        {
            get
            {
                return ( _CswNbtNode.Properties[SubscriptionExpirationDatePropertyName].AsDateTime );
            }
        }
        public CswNbtNodePropLogical Deactivated
        {
            get
            {
                return ( _CswNbtNode.Properties[DeactivatedPropertyName].AsLogical );
            }
        }
        public CswNbtNodePropText CompanyID
        {
            get
            {
                return ( _CswNbtNode.Properties[CompanyIDPropertyName].AsText );
            }
        }
        public CswNbtNodePropNumber UserCount
        {
            get
            {
                return ( _CswNbtNode.Properties[UserCountPropertyName].AsNumber );
            }
        }
        public CswNbtNodePropLogicalSet ModulesEnabled
        {
            get
            {
                return ( _CswNbtNode.Properties[ModulesEnabledPropertyName].AsLogicalSet );
            }
        }
        public CswNbtNodePropButton Login
        {
            get
            {
                return ( _CswNbtNode.Properties[LoginPropertyName].AsButton );
            }
        }

        #endregion


    }//CswNbtObjClassCustomer

}//namespace ChemSW.Nbt.ObjClasses
