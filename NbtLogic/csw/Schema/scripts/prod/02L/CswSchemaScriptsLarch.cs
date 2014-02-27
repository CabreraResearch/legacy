using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsLarch : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02L_CaseXXXXX()
                    new CswUpdateDDL_02L_Case31907()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02L_Case29446A(),
                    new CswUpdateMetaData_02L_Case31750()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateSchema_02L_Case31883(),
                    new CswUpdateSchema_02L_Case29446B(),
                    new CswUpdateSchema_02L_Case31907()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsLarch
}//namespace ChemSW.Nbt.Schema