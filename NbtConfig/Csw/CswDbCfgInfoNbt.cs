using ChemSW.Config;
using System;
using System.Collections.Specialized;

namespace ChemSW.Nbt.Config
{

    public class CswDbCfgInfoNbt : ICswDbCfgInfo
    {

        private CswDbCfgInfo _CswDbCfgInfo = null;
        private bool _IsMobile;

        public CswDbCfgInfoNbt( CswEnumSetupMode SetupMode, bool IsMobile )
        {
            _CswDbCfgInfo = new CswDbCfgInfo( SetupMode, IsMobile );
            _IsMobile = IsMobile;
        }//ctor 

        public void fill() { _CswDbCfgInfo.fill(); }

        public string MasterAccessId
        {
            set
            {
                _CswDbCfgInfo.MasterAccessId = value;
            }

            get
            {
                return ( _CswDbCfgInfo.MasterAccessId );
            }

        }

        public void makeNewDbInstance( string AccessId, string ServerType, string ServerName, string UserName, string PlainPwd, string UserCount, bool Deactivated, string IPFilterRegex, DateTime PasswordDisplayedDate )
        {
            _CswDbCfgInfo.makeNewDbInstance( AccessId, ServerType, ServerName, UserName, PlainPwd, UserCount, Deactivated, IPFilterRegex, PasswordDisplayedDate );

        }//makeNewDbInstance()

        public void removeDbInstance( string AccessId )
        {
            _CswDbCfgInfo.removeDbInstance( AccessId );
        }//removeDbInstance()


        public void makeConfigurationCurrent( string AccessId )
        {
            _CswDbCfgInfo.makeConfigurationCurrent( AccessId );

        }//makeConfigurationCurrent()

        public bool ConfigurationExists( string AccessId, bool OnlyIfEnabled = false )
        {
            return _CswDbCfgInfo.ConfigurationExists( AccessId, OnlyIfEnabled );
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
                if ( false == _IsMobile )
                {
                    _CswDbCfgInfo.CurrentUserCount = value;
                }
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

        public string CurrentPasswordDisplayedOn
        {
            set
            {
                _CswDbCfgInfo.CurrentPasswordDisplayedOn = value;

            }
            get
            {
                return ( _CswDbCfgInfo.CurrentPasswordDisplayedOn );
            }
        }

        public StringCollection AccessIds
        {
            get
            {
                return ( _CswDbCfgInfo.AccessIds );
            }

        }//AccessIds


        public StringCollection ActiveAccessIds
        {
            get
            {
                return ( _CswDbCfgInfo.ActiveAccessIds );
            }

        }//AccessIds

        public int TotalDbInstances
        {
            get
            {
                return ( AccessIds.Count );
            }

        }

        string ICswDbCfgInfo.this[string VariableName]
        {
            get
            {
                return _CswDbCfgInfo.getVal( VariableName );
            }
            set { _CswDbCfgInfo.setVal( VariableName, value ); }
        }

        string ICswDbCfgInfo.this[CswEnumDbConfig VariableName]
        {
            get
            {
                return _CswDbCfgInfo.getVal( VariableName );
            }
            set { _CswDbCfgInfo.setVal( VariableName, value ); }
        }

    }//CswDbCfgInfo

}//namespace ChemSW.Nbt.Config


