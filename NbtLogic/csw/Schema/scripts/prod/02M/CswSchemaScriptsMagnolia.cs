using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// List of schema scripts for the labeled milestone
    /// </summary>
    public class CswSchemaScriptsMagnolia : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02M_CISXXXXX()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02M_CISXXXXX()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02M_CISXXXXX()
                };
        } // _SchemaScripts()

    }
}