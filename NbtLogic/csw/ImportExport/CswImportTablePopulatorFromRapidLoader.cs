
namespace ChemSW.Nbt.ImportExport
{

    public class CswImportTablePopulatorFromRapidLoader : ICswImportTablePopulator
    {


        private bool _Stop = false;
        public bool Stop
        {
            set { _Stop = value; }
            get { return ( _Stop ); }
        }


        public bool loadImportTables( ref string Msg )
        {
            bool ReturnVal = true;

            return ( ReturnVal );

        }//loadImportTables


    } // CswImportTablePopulatorFromRapidLoader

} // namespace ChemSW.Nbt
