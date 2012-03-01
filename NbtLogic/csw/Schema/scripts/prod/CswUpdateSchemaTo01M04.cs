using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;


namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-04
    /// </summary>
    public class CswUpdateSchemaTo01M04 : CswUpdateSchemaTo
    {
//        //public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 04 ); } }
//        public override string Description { set { ; } get { return "Update to schema version " + SchemaVersion.ToString(); } }



        public override void update()
        {


            #region //case 24979

            _CswNbtSchemaModTrnsctn.addBooleanColumn( "object_class_props",
                "iscompoundunique",
                "all compound unique columns on an instance are validated for uniqueness", false, false );

            _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_props",
                "iscompoundunique",
                "all compound unique columns on an instance are validated for uniqueness", false, false );


            #endregion


        }//Update()

    }//class CswUpdateSchemaTo01M04

}//namespace ChemSW.Nbt.Schema