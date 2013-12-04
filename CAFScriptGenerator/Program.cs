using System;
using System.IO;
using ChemSW;
using ChemSW.Config;
using ChemSW.Nbt;
using ChemSW.Nbt.csw.ImportExport;
using ChemSW.Nbt.WebServices;
using ChemSW.RscAdo;

namespace CAFScriptGenerator
{
    class Program
    {
        static void Main( string[] args )
        {
            if( args.Length != 4 )
            {
                Console.Write( "\nUsage:\n\n" +
                               "CAFSqlGenerator.exe <NBT AccessId> <CAF DB Username> <CAF DB Password> <CAF DB server>\n\n" +
                               "For example, if you were setting up a CAF import from cis0001/userpass@CAFDB to nbt_master, you would type " +
                               "\"CAFSqlGenerator.exe nbt_master cis0001 userpass CAFDB\"\n" );
            }
            else
            {
                //assign the command line params to readable variables
                string AccessId = args[0];
                string CAFUser = args[1];
                string CAFPwd = args[2];
                string CAFSrv = args[3];

                //create the NbtResources and assign the DB Resources using the access id
                CswNbtResources NbtResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtExe, true );
                NbtResources.AccessId = AccessId;

                //open a connection to the CAF server using the remaining command line arguments
                CswDbVendorOpsOracle CAFConnection = new CswDbVendorOpsOracle( "CAFImport", CAFSrv, CAFUser, CAFPwd, (CswDataDictionary) NbtResources.DataDictionary, NbtResources.CswLogger, CswEnumPooledConnectionState.Open, "" );
                


                string CAFSql = fetchCAFSql( NbtResources );

                //there is no clean solution for running the contents of .SQL file from inside C#, but I was instructed
                //to make it work, so please forgive the horrible hacks that follow.
                //Assumptions made here: 
                //   the only PL/SQL blocks are the deletes at the top of the script and the triggers at the bottom, 
                //   the / at the end of PL/SQL is always at the beginning of a line, 
                //   triggers always have two lines of spaces before them, except the very first trigger, which has 3


                //add a / before the first trigger and split the file into an array of strings on / chars (breaking off potential PL/SQL blocks)
                string[] SQLCommands = CAFSql
                    .Replace( ");\r\n\r\n\r\ncreate or replace trigger", ");\r\n\r\n\r\n/\r\ncreate or replace trigger" )
                    .Replace( "create or replace procedure", "\r\n/\r\ncreate or replace procedure" )
                    .Split( new[] { "\r\n/" }, StringSplitOptions.RemoveEmptyEntries );

                foreach( string SQLCommand in SQLCommands )
                {   //if the string starts with either of these, it's a PL/SQL block and can be sent as-is
                    if( SQLCommand.Trim().StartsWith( "begin" ) || SQLCommand.Trim().StartsWith( "create or replace trigger" ) || SQLCommand.Trim().StartsWith( "create or replace procedure" ))
                    {
                        CAFConnection.execArbitraryPlatformNeutralSql( SQLCommand );
                    }
                    //otherwise, we need to further split out each command on ; chars
                    else
                    {
                        foreach( string SingleCommand in SQLCommand.Split( ';' ) )
                        {
                            if( SingleCommand.Trim() != String.Empty )
                            {
                                CAFConnection.execArbitraryPlatformNeutralSql( SingleCommand.Trim() );
                            }
                        }
                    }
                }//foreach PL/SQL block in CAF.sql

                //Create all custom props and set up import bindings
                CswNbtImportTools.CreateAllCAFProps( NbtResources, CswEnumSetupMode.NbtExe );

            }//if correct number of args
        }//Main



        private static string fetchCAFSql( CswNbtResources NbtResources )
        {
            //run the function behind the Generate CAF Sql button, and attach a StreamReader to parse the resulting data file
            CswNbtImportWcf.GenerateSQLReturn Return = new CswNbtImportWcf.GenerateSQLReturn();
            CswNbtWebServiceImport.generateCAFSql( NbtResources, Return, "CAF" );
            StreamReader OutputScript = new StreamReader( Return.stream );

            return OutputScript.ReadToEnd();
        }

    }//Class
}//Namespace
