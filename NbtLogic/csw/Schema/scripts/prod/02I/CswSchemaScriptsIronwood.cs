using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsIronwood : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02I_CaseXXXXX()
                    new CswUpdateDDL_02I_Case31057(),
                    new CswUpdateDDL_02I_Case31061A(),
                    new CswUpdateDDL_02I_Case31142(),
                    new CswUpdateDDL_02I_Case31091()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02I_CaseXXXXX()
                    new CswUpdateMetaData_02I_Case31072(),
                    new CswUpdateMetaData_02I_Case31114A(),
                    new CswUpdateMetaData_02I_Case31074()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02I_CaseXXXXX()
                    new CswUpdateSchema_02I_Case31040(),
                    new CswUpdateSchema_02I_Case31072(),
                    new CswUpdateSchema_02I_Case31061B(),
                    new CswUpdateSchema_02I_Case31114B(),
                    new CswUpdateSchema_02I_Case31041(),
                    new CswUpdateSchema_02I_Case31055(),
                    new CswUpdateSchema_02I_Case31074B(),
                    new CswUpdateSchema_02I_Case31025()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsHickory
}//namespace ChemSW.Nbt.Schema