using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ChemSW.Exceptions;


namespace ChemSW.Nbt.Schema.CmdLn
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaUpdaterConsole
    {

        private const string NuLine = "\n\r";
        private const string ParamSpacer = "- ";
        private const string _Help = NuLine + "Usage: " +
                                     NuLine + "NbtSchemaUpdate <AccessId> | all" +
                                     NuLine + ParamSpacer + "<AccessId>: The AccessId, as per CswDbConfig.xml, of the schema to be updated" +
                                     NuLine + ParamSpacer + "all: appdate all schemata specified CswDbConfig.xml";

        private string[] _args;
        public CswSchemaUpdaterConsole( string[] args )
        {
            _args = args;

        }

        public string process()
        {
            string ReturnVal = string.Empty;

            if( 1 == _args.Length )
            {
                if( "all" != _args[0].ToString().ToLower() )
                {
                }
                else
                {
                }
            }
            else
            {
                ReturnVal = _Help; 
            }//


            return ( ReturnVal );
        }

    }//CswSchemaUpdaterConsole

}//ChemSW.Nbt.Schema.CmdLn
