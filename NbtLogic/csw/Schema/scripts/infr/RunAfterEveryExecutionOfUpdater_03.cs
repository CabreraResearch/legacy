using System;
using System.Threading;
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
    /// Applies indexes
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_03 : CswUpdateSchemaTo
    {
        public static string Title = FileName;

        private const string FileName = "indexes.sql";

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.runExternalSqlScript( FileName, ChemSW.Nbt.Properties.Resources.indexes_sql );
        }//Update()

    }//class RunAfterEveryExecutionOfUpdater_03

}//namespace ChemSW.Nbt.Schema


