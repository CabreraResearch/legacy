using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Nbt.Schema.CmdLn;

namespace NbtSchemaUpdaterCmdLn
{
    class Program
    {
        private static CswSchemaUpdaterConsole _CswSchemaUpdaterConsole = null; 
        static void Main( string[] args )
        {
            _CswSchemaUpdaterConsole = new CswSchemaUpdaterConsole( args );
            Console.WriteLine( _CswSchemaUpdaterConsole.process() ); 
        }
    }
}
