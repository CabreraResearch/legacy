using ChemSW.Nbt.Schema.CmdLn;

namespace NbtSchemaUpdaterCmdLn
{
    class Program
    {
        private static CswSchemaUpdaterConsole _CswSchemaUpdaterConsole = null;

        //return 
        static int Main( string[] args )
        {
            int ReturnVal = 1;

            _CswSchemaUpdaterConsole = new CswSchemaUpdaterConsole();

            bool AllScriptsSucceded = _CswSchemaUpdaterConsole.process( args );

            if( AllScriptsSucceded )
            {
                ReturnVal = 1;
            }
            else
            {
                ReturnVal = -1;
            }

            return ( ReturnVal );
        }
    }
}
