using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ChemSW.Nbt.Schema
{
    public interface ICswSchemaScripts
    {
        Collection<CswUpdateSchemaTo> _DDLScripts();
        Collection<CswUpdateSchemaTo> _MetaDataScripts();
        Collection<CswUpdateSchemaTo> _SchemaScripts();
    }
}//namespace ChemSW.Nbt.Schema        
