using System;
using System.Collections.Generic;
using System.Text;

namespace ChemSW.Nbt.Schema
{
    public interface ICswUpdateSchemaTo
    {
        void update();
        CswSchemaVersion SchemaVersion { get; }

    }
}
