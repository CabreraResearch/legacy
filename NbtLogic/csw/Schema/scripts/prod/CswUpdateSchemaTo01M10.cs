using System;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01M-10
    /// </summary>
    public class CswUpdateSchemaTo01M10 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'M', 10 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            
        }//Update()

    }//class CswUpdateSchemaTo01M10

}//namespace ChemSW.Nbt.Schema