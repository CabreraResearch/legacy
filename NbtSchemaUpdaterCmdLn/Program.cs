using ChemSW.Nbt.Schema.CmdLn;

namespace NbtSchemaUpdaterCmdLn
{
    class Program
    {
        private static CswSchemaUpdaterConsole _CswSchemaUpdaterConsole = null;
        static void Main( string[] args )
        {
            _CswSchemaUpdaterConsole = new CswSchemaUpdaterConsole();
            _CswSchemaUpdaterConsole.process( args );
        }
    }
}
