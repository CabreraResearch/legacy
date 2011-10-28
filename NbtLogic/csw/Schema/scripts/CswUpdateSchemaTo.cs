using System;
using System.Collections.Generic;
using System.Text;

namespace ChemSW.Nbt.Schema
{
    public abstract class CswUpdateSchemaTo
    {
        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set { _CswNbtSchemaModTrnsctn = value; }
        }

        public abstract CswSchemaVersion SchemaVersion { get; }
        public abstract string Description { get; }
        public abstract void update();
    }
}
