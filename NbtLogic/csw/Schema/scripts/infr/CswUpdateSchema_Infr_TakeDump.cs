
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-01
    /// </summary>
    public class CswUpdateSchema_Infr_TakeDump : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 00,'A',00 ); } }
        public override string Description
        {
            get
            {
                string ReturnVal = string.Empty;

                string DumpDirectory = string.Empty;
                string DumpFileName = string.Empty;
                string StatusMsg = string.Empty;

                _CswNbtSchemaModTrnsctn.getNextSchemaDumpFileInfo( ref DumpDirectory, ref DumpFileName );
                ReturnVal = "Creating dumpfile " + DumpFileName + " in " + DumpDirectory;

                return ( ReturnVal );
            }
        }

        public override void update()
        {

            string DumpDirectory = string.Empty;
            string DumpFileName = string.Empty;
            string StatusMsg = string.Empty;

            _CswNbtSchemaModTrnsctn.takeADump( ref DumpFileName, ref StatusMsg ); 

        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


