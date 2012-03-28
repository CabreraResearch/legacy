using System;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using System.Diagnostics;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Runs the initialize script
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {
        public static string Title = FileName;

        private const string FileName = "nbt_initialize_ora.sql";

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.runExternalSqlScript( FileName, ChemSW.Nbt.Properties.Resources.nbt_initialize_ora_sql );
        }//update()

    }//class RunBeforeEveryExecutionOfUpdater_02

}//namespace ChemSW.Nbt.Schema
