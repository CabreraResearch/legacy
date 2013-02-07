using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Security;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCustomer : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string IPFilterRegex = "IP Filter Regex";
            public const string SubscriptionExpirationDate = "Subscription Expiration Date";
            public const string Deactivated = "Deactivated";
            public const string CompanyID = "Company ID";
            public const string UserCount = "User Count";
            public const string ModulesEnabled = "Modules Enabled";
            public const string Login = "Login";
            public const string SchemaName = "Schema Name";
            public const string SchemaVersion = "Schema Version";
            public const string PendingFeedbackCount = "Pending Feedback Count";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassCustomer( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.CustomerClass ); }
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
            ICswUser RetUser = new CswNbtSystemUser( _CswNbtResources, SystemUserNames.SysUsr_ObjClassCustomer );
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
            if( null != Node && _Validate( Node, NbtObjectClass.CustomerClass ) )
            {
                ret = (CswNbtObjClassCustomer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

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
                    Collection<CswNbtModuleName> ModulesToEnable = new Collection<CswNbtModuleName>();
                    Collection<CswNbtModuleName> ModulesToDisable = new Collection<CswNbtModuleName>();

                    foreach( string ModuleName in ModulesEnabled.YValues )
                    {
                        CswNbtModuleName Module = ModuleName;
                        //Enum.TryParse( ModuleName, true, out Module );
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
                    OtherResources.Modules.UpdateModules( ModulesToEnable, ModulesToDisable );
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

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }

        protected override void afterPopulateProps()
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
                foreach( CswNbtModuleName ModuleName in CswNbtModuleName._All )
                {
                    if( CswNbtModuleName.Unknown != ModuleName )
                    {
                        YValues.Add( ModuleName.ToString() );
                    }
                }
                ModulesEnabled.YValues = YValues;
                ModulesEnabled.XValues = new CswCommaDelimitedString() { ModulesEnabledXValue };

                // get data from target schema
                //string OriginalAccessId = _CswNbtResources.AccessId;
                //_CswNbtResources.AccessId = CompanyID.Text;
                CswNbtResources OtherResources = makeOtherResources();

                Collection<CswNbtModuleName> Modules = new Collection<CswNbtModuleName>();
                foreach( CswNbtModuleName Module in OtherResources.Modules.ModulesEnabled() )
                {
                    Modules.Add( Module );
                }

                // case 25960
                string OtherSchemaVersion = OtherResources.ConfigVbls.getConfigVariableValue( CswNbtResources.ConfigurationVariables.schemaversion.ToString() );

                //case 28079 - count the number of pending feedback nodes
                int count = 0;
                CswNbtMetaDataObjectClass feedbackOC = OtherResources.MetaData.getObjectClass( NbtObjectClass.FeedbackClass );
                foreach( CswNbtObjClassFeedback feedbackNode in feedbackOC.getNodes( false, false ) )
                {
                    if( feedbackNode.Status.Value.Equals( CswNbtObjClassFeedback.Statuses.PendingReview ) )
                    {
                        count++;
                    }
                }
                PendingFeedbackCount.Value = count;

                // reconnect to original schema
                //_CswNbtResources.AccessId = OriginalAccessId;
                finalizeOtherResources( OtherResources );

                foreach( CswNbtModuleName ModuleName in CswNbtModuleName._All )
                {
                    if( CswNbtModuleName.Unknown != ModuleName )
                    {
                        ModulesEnabled.SetValue( ModulesEnabledXValue, ModuleName.ToString(), Modules.Contains( ModuleName ) );
                    }
                }

                this.SchemaVersion.StaticText = OtherSchemaVersion;
            }

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public static string ModulesEnabledXValue = "Enabled";

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {



            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                if( PropertyName.Login == OCP.PropName )
                {
                    ButtonData.Action = NbtButtonAction.reauthenticate;
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
                return ( _CswNbtNode.Properties[PropertyName.IPFilterRegex] );
            }
        }
        public CswNbtNodePropDateTime SubscriptionExpirationDate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SubscriptionExpirationDate] );
            }
        }
        public CswNbtNodePropLogical Deactivated
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Deactivated] );
            }
        }
        public CswNbtNodePropText CompanyID
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.CompanyID] );
            }
        }
        public CswNbtNodePropNumber UserCount
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.UserCount] );
            }
        }
        public CswNbtNodePropLogicalSet ModulesEnabled
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.ModulesEnabled] );
            }
        }
        public CswNbtNodePropButton Login
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Login] );
            }
        }
        public CswNbtNodePropStatic SchemaVersion
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SchemaVersion] );
            }
        }
        public CswNbtNodePropStatic SchemaName
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SchemaName] );
            }
        }
        public CswNbtNodePropNumber PendingFeedbackCount
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.PendingFeedbackCount] );
            }
        }

        #endregion


    }//CswNbtObjClassCustomer

}//namespace ChemSW.Nbt.ObjClasses
