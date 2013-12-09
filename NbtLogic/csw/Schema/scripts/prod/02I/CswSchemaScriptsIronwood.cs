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
                    new CswUpdateDDL_02I_Case31091(),
                    new CswUpdateDDL_02I_Case31056()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02I_CaseXXXXX()
                    new CswUpdateMetaData_02I_Case31072(),
                    new CswUpdateMetaData_02I_Case31114A(),
                    new CswUpdateMetaData_02I_Case31210(),
                    new CswUpdateMetaData_02I_Case31074(),
                    new CswUpdateMetaData_02I_Case31243(),
                    new CswUpdateMetaData_02I_Case31234A(),
                    new CswUpdateMetaData_02I_Case31090A(),
                    new CswUpdateMetaData_02I_Case30533A(),
                    new CswUpdateMetaData_02I_Case30533B(),
                    new CswUpdateMetaData_02I_Case31113A()
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
                    new CswUpdateSchema_02I_Case31223(),
                    new CswUpdateSchema_02I_Case31025(),
                    new CswUpdateSchema_02I_Case31210(),
                    new CswUpdateSchema_02I_Case31234B(),
                    new CswUpdateSchema_02I_Case31090B(),
                    new CswUpdateSchema_02I_Case31236A(),
                    new CswUpdateSchema_02I_Case30969(),
                    new CswUpdateSchema_02I_Case30533C(),
                    new CswUpdateSchema_02I_Case30941(),
                    new CswUpdateSchema_02I_Case30989(),
                    new CswUpdateSchema_02I_Case31113B(),
                    new CswUpdateSchema_02I_Case31312(),
                    new CswUpdateSchema_02I_Case31292(),
                    new CswUpdateSchema_02I_Case31056B(),
                    new CswUpdateSchema_02I_Case31086()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsHickory
}//namespace ChemSW.Nbt.Schema