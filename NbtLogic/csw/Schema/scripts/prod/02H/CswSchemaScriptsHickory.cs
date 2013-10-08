using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsHickory : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    //new CswUpdateDDL_02H_CaseXXXXX()
                    //Add DDL Scripts Here
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02H_CaseXXXXX()
                    new CswUpdateMetaData_02H_Case30879(),
                    new CswUpdateMetaData_02H_Case30537A(),
                    new CswUpdateMetaData_02H_Case30537B(),
                    new CswUpdateMetaData_02H_Case30764(),
                    new CswUpdateMetaData_02H_Case30130()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02H_CaseXXXXX()
                    new CswUpdateSchema_02H_Case30537C()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsHickory
}//namespace ChemSW.Nbt.Schema