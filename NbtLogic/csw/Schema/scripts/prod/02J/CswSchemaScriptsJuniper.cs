using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsJuniper : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02J_CaseXXXXX()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02J_CaseXXXXX()
                    new CswUpdateMetaData_02J_Case31164(),
                    new CswUpdateMetaData_02J_Case30825A(),
                    new CswUpdateMetaData_02J_Case30825B(),
                    new CswUpdateMetaData_02J_Case30825C(),
                    new CswUpdateMetaData_02J_Case30825D()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02J_CaseXXXXX()
                    new CswUpdateSchema_02J_Case31101(),
                    new CswUpdateSchema_02J_Case31076(),
                    new CswUpdateSchema_02J_Case27149(),
                    new CswUpdateSchema_02J_Case29409(),
                    new CswUpdateSchema_02J_Case30825E()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsJuniper
}//namespace ChemSW.Nbt.Schema