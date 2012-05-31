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
        public static string SchemaNamePropertyName { get { return "Schema Name"; } }
        public static string SchemaVersionPropertyName { get { return "Schema Version"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassCustomer( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
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
            OtherResources.release();
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassCustomer
        /// </summary>
        public static implicit operator CswNbtObjClassCustomer( CswNbtNode Node )
        {
            CswNbtObjClassCustomer ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass ) )
            {
                ret = (CswNbtObjClassCustomer) Node.ObjClass;
            }
            return ret;
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


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _checkForConfigFileUpdate();
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }

        public override void afterWriteNode()
        {
            _doConfigFileUpdate();

            if( _CompanyIDDefined() )
            {
                if( ModulesEnabled.WasModified )
                {
                    Collection<CswNbtResources.CswNbtModule> ModulesToEnable = new Collection<CswNbtResources.CswNbtModule>();
                    Collection<CswNbtResources.CswNbtModule> ModulesToDisable = new Collection<CswNbtResources.CswNbtModule>();

                    foreach( string ModuleName in ModulesEnabled.YValues )
                    {
                        CswNbtResources.CswNbtModule Module;
                        Enum.TryParse( ModuleName, true, out Module );
                        if( ModulesEnabled.CheckValue( ModulesEnabledXValue, ModuleName ) )
                        {
                            ModulesToEnable.Add( Module );
                        }
                        else
                        {
                            ModulesToDisable.Add( Module );
                        }
                    }

                    // switch to target schema
                    CswNbtResources OtherResources = makeOtherResources();
                    OtherResources.UpdateModules( ModulesToEnable, ModulesToDisable );
                    finalizeOtherResources( OtherResources );

                } // if( ModulesEnabled.WasModified )
            } // if( _CompanyIDDefined() )

            _CswNbtObjClassDefault.afterWriteNode();
        } // afterWriteNode()

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

        public override void beforeDeleteNode(bool DeleteAllRequiredRelatedNodes = false)
        {
            _CswNbtObjClassDefault.beforeDeleteNode(DeleteAllRequiredRelatedNodes);
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

                // case 25960
                this.SchemaName.StaticText = _CswNbtResources.CswDbCfgInfo.CurrentUserName;

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

                // case 25960
                string OtherSchemaVersion = OtherResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.schemaversion.ToString() );

                // reconnect to original schema
                //_CswNbtResources.AccessId = OriginalAccessId;
                finalizeOtherResources( OtherResources );

                foreach( CswNbtResources.CswNbtModule Module in Enum.GetValues( typeof( CswNbtResources.CswNbtModule ) ) )
                {
                    ModulesEnabled.SetValue( ModulesEnabledXValue, Module.ToString(), Modules.Contains( Module ) );
                }

                this.SchemaVersion.StaticText = OtherSchemaVersion;
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public static string ModulesEnabledXValue = "Enabled";

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                if( LoginPropertyName == OCP.PropName )
                {
                    ButtonAction = NbtButtonAction.reauthenticate;
                }
            }
            return true;
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
        public CswNbtNodePropStatic SchemaVersion
        {
            get
            {
                return ( _CswNbtNode.Properties[SchemaVersionPropertyName].AsStatic );
            }
        }
        public CswNbtNodePropStatic SchemaName
        {
            get
            {
                return ( _CswNbtNode.Properties[SchemaNamePropertyName].AsStatic );
            }
        }


        #endregion


    }//CswNbtObjClassCustomer

}//namespace ChemSW.Nbt.ObjClasses
