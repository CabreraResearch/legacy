using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
                    new CswUpdateMetaData_02G_Case30557()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02G_Case30611(),
                    new CswUpdateMetaData_02G_Case27846(),
                    new CswUpdateMetaData_02G_Case30542(),
                    new CswUpdateMetaData_02G_Case30557B()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateSchema_02G_Case30542(),
                    new CswUpdateSchema_02G_Case30557(),
                    new CswUpdateSchema_02G_Case30679(),
                    new CswUpdateSchema_02G_Case30473()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsGinkgo
}//namespace ChemSW.Nbt.Schema