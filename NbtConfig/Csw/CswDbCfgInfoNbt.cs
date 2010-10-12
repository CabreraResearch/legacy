using System;
using System.Collections;
using ChemSW.Core;
using ChemSW.Config;

namespace ChemSW.Nbt.Config
{

    public class CswDbCfgInfoNbt : ICswDbCfgInfo
    {

        private CswDbCfgInfo _CswDbCfgInfo = null;
        public CswDbCfgInfoNbt( SetupMode SetupMode )
        {
            _CswDbCfgInfo = new CswDbCfgInfo( SetupMode );
        }//ctor 

        public void fill() { _CswDbCfgInfo.fill(); }

        public void setSetupMode( SetupMode SetupMode, string SetupFilePath )
        {
            _CswDbCfgInfo.setSetupMode( SetupMode, SetupFilePath );

        }//SetConfigMode()




        public void makeNewDbInstance( string AccessId, string ServerType, string ServerName, string UserName, string PlainPwd, string UserCount, bool Deactivated, string IPFilterRegex )
        {
            _CswDbCfgInfo.makeNewDbInstance( AccessId, ServerType, ServerName, UserName, PlainPwd, UserCount, Deactivated, IPFilterRegex );

        }//makeNewDbInstance()

        public void removeDbInstance( string AccessId )
        {
            _CswDbCfgInfo.removeDbInstance( AccessId );
        }//removeDbInstance()

        public void makeConfigurationCurrent( string AccessId )
        {
            _CswDbCfgInfo.makeConfigurationCurrent( AccessId );

        }//makeConfigurationCurrent()

        public bool ConfigurationExists( string AccessId )
        {
            return _CswDbCfgInfo.ConfigurationExists( AccessId );
        }

        public string CurrentAccessId
        {
            get
            {
                return ( _CswDbCfgInfo.CurrentAccessId );
            }//get

            set
            {
                _CswDbCfgInfo.CurrentAccessId = value;
            }//set
        }//CurrentAccessId

        public string CurrentServerType
        {
            set
            {
                _CswDbCfgInfo.CurrentServerType = value;
            }//
            get
            {
                return ( _CswDbCfgInfo.CurrentServerType );
            }//get

        }//

        public string CurrentServerName
        {
            set
            {
                _CswDbCfgInfo.CurrentServerName = value;
            }//set

            get
            {
                return ( _CswDbCfgInfo.CurrentServerName );

            }//get

        }//

        public string CurrentUserName
        {
            set
            {
                _CswDbCfgInfo.CurrentUserName = value;
            }//set

            get
            {

                return ( _CswDbCfgInfo.CurrentUserName );
            }//get

        }//

        public string CurrentPlainPwd
        {
            set
            {
                _CswDbCfgInfo.CurrentPlainPwd = value;
            }//set

            get
            {

                return ( _CswDbCfgInfo.CurrentPlainPwd );
            }//get

        }//getCurrentPlainPassword()

        public string CurrentEncryptedPwd
        {

            set
            {
                _CswDbCfgInfo.CurrentEncryptedPwd = value;
            }//set

            get
            {

                return ( _CswDbCfgInfo.CurrentEncryptedPwd );
            }//set

        }//

        public string CurrentUserCount
        {
            set
            {
                _CswDbCfgInfo.CurrentUserCount = value;
            }
            get
            {

                return ( _CswDbCfgInfo.CurrentUserCount );
            }
        }

        public bool CurrentDeactivated
        {
            set
            {
                _CswDbCfgInfo.CurrentDeactivated = value;
            }
            get
            {
                return ( _CswDbCfgInfo.CurrentDeactivated );
            }
        }

        public string CurrentIPFilterRegex
        {
            set
            {
                _CswDbCfgInfo.CurrentIPFilterRegex = value;

            }
            get
            {
                return ( _CswDbCfgInfo.CurrentIPFilterRegex );
            }
        }


        public ArrayList AccessIds
        {
            get
            {
                return ( _CswDbCfgInfo.AccessIds );
            }

        }//AccessIds

        public int TotalDbInstances
        {
            get
            {
                return ( AccessIds.Count );
            }

        }

    }//CswDbCfgInfo

}//namespace ChemSW.Nbt.Config


