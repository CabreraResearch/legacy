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
    /// Updates the schema to version 01J-01
    /// </summary>
    public class RunAfterEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {

        public override void update()
        {
            //***************  ADD your own code
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Running nbt_finalize_ora.bat after updates." ); //this one doesn't blocks        
            //"Initialize" is only for updateSequences()!!!

            string FileLocations = Application.StartupPath;
            string BatchFilePath = FileLocations + "\\nbt_finalize_ora.bat";
            string SqlFilePath = FileLocations + "\\nbt_finalize_ora.sql";
            File.WriteAllBytes( BatchFilePath, ChemSW.Nbt.Properties.Resources.nbt_initialize_ora_bat );
            File.WriteAllBytes( SqlFilePath, ChemSW.Nbt.Properties.Resources.nbt_initialize_ora_sql );


            _CswNbtSchemaModTrnsctn.CswDbCfgInfo.makeConfigurationCurrent( _CswNbtSchemaModTrnsctn.Accessid );
            string serverName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentServerName;
            string userName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentUserName;
            string passWord = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentPlainPwd;


            System.Diagnostics.Process Process = new System.Diagnostics.Process();
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.FileName = BatchFilePath;
            Process.StartInfo.Arguments = " " + serverName + " " + userName + " " + passWord + " " + FileLocations;
            Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardOutput = false;

            System.Diagnostics.Process.Start( Process.StartInfo );
            //we don't wait 
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Left nbt_finalize_ora.bat after updates without waiting." ); //this one doesn't block


        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


