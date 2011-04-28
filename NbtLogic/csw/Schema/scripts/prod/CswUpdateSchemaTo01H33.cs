using System;
using System.Data;
using System.Threading;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01H-33
    /// </summary>
    public class CswUpdateSchemaTo01H33 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 33 ); } }
        public CswUpdateSchemaTo01H33( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
        }

        public void update()
        {
            Thread.Sleep( 3000 );


        } // update()

    }//class CswUpdateSchemaTo01H33

}//namespace ChemSW.Nbt.Schema

