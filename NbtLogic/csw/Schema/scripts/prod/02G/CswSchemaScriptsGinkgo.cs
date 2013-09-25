using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsGinkgo : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02G_Case30557(),
                    new CswUpdateDDL_02G_Case29565(),
                    new CswUpdateDDL_02G_Case30702(),
                    new CswUpdateDDL_02G_Case30737()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02G_Case30611(),
                    new CswUpdateMetaData_02G_Case27846(),
                    new CswUpdateMetaData_02G_Case30542(),
                    new CswUpdateMetaData_02G_Case30557B(),
                    new CswUpdateMetaData_02G_Case28493A(),
                    new CswUpdateMetaData_02G_Case30666(),
                    new CswUpdateMetaData_02G_Case30564(),
                    new CswUpdateMetaData_02G_Case30480(),
                    new CswUpdateMetaData_02G_Case30575(),
                    new CswUpdateMetaData_02G_Case30680(),
                    new CswUpdateMetaData_02G_Case30743()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateSchema_02G_Case30542(),
                    new CswUpdateSchema_02G_Case30557(),
                    new CswUpdateSchema_02G_Case30679(),
                    new CswUpdateSchema_02G_Case30473(),
                    new CswUpdateSchema_02G_Case30342(),
                    new CswUpdateSchema_02G_Case28493B(),
                    new CswUpdateSchema_02G_Case30383(),
                    new CswUpdateSchema_02G_Case30379(),
                    new CswUpdateSchema_02G_Case29565B(),
                    new CswUpdateSchema_02G_Case30610(),
                    new CswUpdateSchema_02G_Case30564B(),
                    new CswUpdateSchema_02G_Case29894(),
                    new CswUpdateSchema_02G_Case28832(),
                    new CswUpdateSchema_02G_Case30480(),
                    new CswUpdateSchema_02G_Case30301(),
                    new CswUpdateSchema_02G_Case30561(),
                    new CswUpdateSchema_02G_Case30756(),
                    new CswUpdateSchema_02G_Case30702(),
                    new CswUpdateSchema_02G_Case30680(),
                    new CswUpdateSchema_02G_Case30562(),
                    new CswUpdateSchema_02G_Case30743(),
                    new CswUpdateSchema_02G_Case30743_Materials()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsGinkgo
}//namespace ChemSW.Nbt.Schema