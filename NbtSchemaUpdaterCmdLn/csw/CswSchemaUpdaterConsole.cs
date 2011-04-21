using System;
using System.Threading;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt.Config;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdaterConsole
    {

        private const string NuLine = "\n\r";
        private const string ParamSpacer = "- ";

        private string _VersionInfo
        {
            get
            {
                string ReturnVal = string.Empty;

                ReturnVal = NuLine + "ChemSW NBT SchemaUpdater version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();

                if( null != _CswSchemaUpdater )
                {
                    ReturnVal += NuLine + NuLine + "Minimum schema version: " + _CswSchemaUpdater.MinimumVersion;
                    ReturnVal += NuLine + "Latest schema version: " + _CswSchemaUpdater.TargetVersion;
                }

                return ( ReturnVal );
            }
        }

        private string _Help
        {
            get
            {

                string ReturnVal = string.Empty;

                ReturnVal = _VersionInfo;




                ReturnVal += NuLine + NuLine + "Usage: " +
                                            NuLine + "NbtSchemaUpdate <AccessId> | all" +
                                            NuLine + ParamSpacer + "<AccessId>: The AccessId, as per CswDbConfig.xml, of the schema to be updated" +
                                            NuLine + ParamSpacer + "all: appdate all schemata specified CswDbConfig.xml";
                return ( ReturnVal );
            }
        }


        private CswDbCfgInfoNbt _CswDbCfgInfoNbt = null;
        private CswSchemaUpdater _CswSchemaUpdater = null;
        private CswNbtResources _CswNbtResources = null;
        private CswSchemaUpdateThread _CswSchemaUpdateThread = null;



        private string[] _args;
        public CswSchemaUpdaterConsole( string[] args )
        {
            _args = args;
            _CswDbCfgInfoNbt = new CswDbCfgInfoNbt( SetupMode.Executable );
            _CswNbtResources = CswNbtResourcesFactory.makeCswNbtResources( AppType.Nbt, new CswSetupVblsNbt( SetupMode.Executable ), _CswDbCfgInfoNbt, CswTools.getConfigurationFilePath( SetupMode.Executable ), false, false );
            _CswSchemaUpdater = new CswSchemaUpdater( _CswNbtResources );
            _CswSchemaUpdateThread = new CswSchemaUpdateThread( _CswSchemaUpdater );

        }//ctor

        public string process()
        {
            string ReturnVal = string.Empty;

            if( 1 == _args.Length )
            {
                string Arg1 = _args[0].ToString();

                if( "all" != Arg1.ToLower() )
                {
                    if( _CswDbCfgInfoNbt.AccessIds.Contains( Arg1 ) )
                    {
                        ReturnVal = _updateAccessId( Arg1 );
                    }
                    else
                    {
                        ReturnVal = "AccessId " + Arg1 + " unknown";
                    }
                }
                else
                {

                    ReturnVal = "This is not implemented yet";
                }
            }
            else
            {
                ReturnVal = _Help;
            }//


            return ( ReturnVal );
        }

        private string _updateAccessId( string AccessId )
        {
            string ReturnVal = string.Empty;

            _CswNbtResources.AccessId = AccessId;

            if( _CswSchemaUpdater.LatestVersion != _CswSchemaUpdater.TargetVersion )
            {
                Console.WriteLine( "Updating AccessId " + AccessId + " to schema " + _CswSchemaUpdater.TargetVersion.ToString() );
                _CswSchemaUpdateThread.start();
                while( UpdateState.Complete != _CswSchemaUpdateThread.UpdateState )
                {
                    Console.Write( " ." );
                    Thread.Sleep( 1000 );
                }

                ReturnVal = _CswSchemaUpdateThread.Message;

            }
            else
            {
                ReturnVal = NuLine + _VersionInfo + NuLine + NuLine + "AccessId " + AccessId + ": Schema version -- " + _CswSchemaUpdater.LatestVersion.ToString() + "-- is current";
            }

            return ( ReturnVal );

        }//_updateAccessId() 

    }//CswSchemaUpdaterConsole

}//ChemSW.Nbt.Schema.CmdLn
