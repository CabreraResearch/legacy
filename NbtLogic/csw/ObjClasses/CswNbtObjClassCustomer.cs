using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;


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

        #region Inherited Events

        public override void beforeCreateNode()
        {
            _checkForConfigFileUpdate();
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _doConfigFileUpdate();
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()


        public override void beforeWriteNode()
        {
            _checkForConfigFileUpdate();
            _CswNbtObjClassDefault.beforeWriteNode();
        }

        public override void afterWriteNode()
        {
            _doConfigFileUpdate();
            _CswNbtObjClassDefault.afterWriteNode();
        }

        bool UpdateConfigFile = false;
        private void _checkForConfigFileUpdate()
        {
            if( ( Deactivated.WasModified || IPFilterRegex.WasModified || UserCount.WasModified ) &&
                ( CompanyID.Text != string.Empty && _CswNbtResources.CswDbCfgInfo.ConfigurationExists( CompanyID.Text ) ) )
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
            if( CompanyID.Text != string.Empty && _CswNbtResources.CswDbCfgInfo.ConfigurationExists( CompanyID.Text ) )
            {
                // get data from DbConfig file
                _CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CompanyID.Text );
                if( _CswNbtResources.CswDbCfgInfo.CurrentDeactivated )
                    Deactivated.Checked = Tristate.True;
                else
                    Deactivated.Checked = Tristate.False;
                IPFilterRegex.Text = _CswNbtResources.CswDbCfgInfo.CurrentIPFilterRegex;
                if( CswTools.IsInteger( _CswNbtResources.CswDbCfgInfo.CurrentUserCount ) )
                    UserCount.Value = Convert.ToInt32( _CswNbtResources.CswDbCfgInfo.CurrentUserCount );
                else
                    UserCount.Value = Double.NaN;

                // get data from target schema
                string OriginalAccessId = _CswNbtResources.AccessId;
                //Commmit no longer necessary with bz # 8576 fixed
                //                _CswNbtResources.CswDbResources.commitTransaction();//KLUDGE ALERT: bz # 8591 && 8576
                _CswNbtResources.AccessId = CompanyID.Text;
                //                _CswNbtResources.CswDbResources.beginTransaction();

                string ModuleString = string.Empty;
                foreach( CswNbtResources.CswNbtModule Module in _CswNbtResources.ModulesEnabled() )
                {
                    if( ModuleString != string.Empty )
                        ModuleString += ", ";
                    ModuleString += Module.ToString();
                }

                // reconnect to original schema
                //_CswNbtResources.CswDbResources.commitTransaction(); //KLUDGE ALERT: bz # 8591 && 8576
                // finalize does commitTransaction():
                _CswNbtResources.finalize();
                _CswNbtResources.AccessId = OriginalAccessId;
                // Setting AccessId does this for us:
                //_CswNbtResources.CswDbResources.beginTransaction();

                ModulesEnabled.StaticText = ModuleString;
            }

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
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
        public CswNbtNodePropDate SubscriptionExpirationDate
        {
            get
            {
                return ( _CswNbtNode.Properties[SubscriptionExpirationDatePropertyName].AsDate );
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
        public CswNbtNodePropStatic ModulesEnabled
        {
            get
            {
                return ( _CswNbtNode.Properties[ModulesEnabledPropertyName].AsStatic );
            }
        }

        #endregion

    }//CswNbtObjClassCustomer

}//namespace ChemSW.Nbt.ObjClasses
