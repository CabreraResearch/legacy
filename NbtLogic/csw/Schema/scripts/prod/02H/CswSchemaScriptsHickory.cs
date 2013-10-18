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
                    // new CswUpdateMetaData_02H_CaseXXXXX()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02H_CaseXXXXX()
                    new CswUpdateMetaData_02H_Case30879(),
                    new CswUpdateMetaData_02H_Case30046(),
                    new CswUpdateMetaData_02H_Case30130(),
                    new CswUpdateMetaData_02H_Case30537A(),
                    new CswUpdateMetaData_02H_Case30537B(),
                    new CswUpdateMetaData_02H_Case30690A(),
                    new CswUpdateMetaData_02H_Case30764(),
                    new CswUpdateMetaData_02H_Case30130(),
                    new CswUpdateMetaData_02H_Case30400(),
                    new CswUpdateMetaData_02H_Case28562(),
                    new CswUpdateMetaData_02H_Case30590(),
                    new CswUpdateMetaData_02H_Case30772(),
                    new CswUpdateMetaData_02H_Case30723()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02H_CaseXXXXX()
                    new CswUpdateSchema_02H_Case30537C(),
                    new CswUpdateSchema_02H_Case30400(),
                    new CswUpdateSchema_02H_Case30690B(),
                    new CswUpdateSchema_02H_Case28562B(),
                    new CswUpdateSchema_02H_Case28562C(),
                    new CswUpdateSchema_02H_Case28562D(),
                    new CswUpdateSchema_02H_Case28518A(),
                    new CswUpdateSchema_02H_Case28518B()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsHickory
}//namespace ChemSW.Nbt.Schema