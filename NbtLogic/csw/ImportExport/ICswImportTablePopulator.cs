
namespace ChemSW.Nbt.ImportExport
{

    public enum ImportTablePopulationMode
    {
        Unknown,

        /// <summary>
        /// Input is the canonical Xml format of post-IMCS ripper/Generic spreadsheet via XSL transform.
        /// See:
        ///      ~/Nbt/NbtImport/XSL
        ///      ~/Nbt/NbtImport/SpreadsheetExamples/Generic
        /// </summary>
        FromXml,

        /// <summary>
        /// Input is essentially the rapid-loader spreadsheet, 
        /// See .
        /// ~/Nbt/NbtImport/SpreadsheetExamples/RapidLoader
        /// </summary>
        FromRapidLoaderXls

    } // ImportMode

    public interface ICswImportTablePopulator
    {

        bool loadImportTables( ref string Msg );

    } // ICswImportTablePopulator

} // namespace ChemSW.Nbt
