using System;
using ChemSW.Config;
using ChemSW.Nbt;
using ChemSW.Nbt.csw.ImportExport;

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
                string CAFSchema = args[1];
                string CAFPassword = args[2];
                string CAFDatabase = args[3];


                //create the NbtResources and assign the DB Resources using the access id
                CswNbtResources NbtResources = CswNbtResourcesFactory.makeCswNbtResources( CswEnumAppType.Nbt, CswEnumSetupMode.NbtExe, true );
                NbtResources.AccessId = AccessId;

                CswNbtImportTools.startCAFImportImpl( NbtResources, CAFDatabase, CAFSchema, CAFPassword, CswEnumSetupMode.NbtExe );

            }//if correct number of args
        }//Main

    }//Class
}//Namespace
