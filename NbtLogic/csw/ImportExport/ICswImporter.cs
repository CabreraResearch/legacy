
namespace ChemSW.Nbt.ImportExport
{

    public enum ImportMode
    {
        Unknown,
        /// <summary>
        /// Make changes to existing data.  Unmatched data is ignored.
        /// </summary>
        Update,
        /// <summary>
        /// Make new data.  Matched data is ignored.
        /// </summary>
        Duplicate,
        /// <summary>
        /// Make changes to existing data, and make new when unmatched.
        /// </summary>
        Overwrite

    } // ImportMode

    public enum ImportAlgorithm { Legacy, DbTableBased }

    public interface ICswImporter
    {

        void ImportXml( ImportMode IMode, ref string ViewXml, ref string ResultXml, ref string ErrorLog );
        void reset(); 
        void stop(); 

    } // ICswImporter
} // namespace ChemSW.Nbt
