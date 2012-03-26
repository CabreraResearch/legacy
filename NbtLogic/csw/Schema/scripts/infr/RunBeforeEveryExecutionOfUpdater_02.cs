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
    public class RunBeforeEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {
        public static string Title = "nbt_initialize_ora.bat";

        public override void update()
        {
            //_CswNbtSchemaModTrnsctn.CswLogger.reportAppState("Running nbt_initialize_ora.bat prior to updates."); //this one blocks        
            
            //"Initialize" is only for updateSequences()!!!



            //Retrieve files from resource
            string FileLocations = Application.StartupPath;
            string BatchFilePath = FileLocations + "\\nbt_initialize_ora.bat";
            string SqlFilePath = FileLocations + "\\nbt_initialize_ora.sql";
            File.WriteAllBytes( BatchFilePath, ChemSW.Nbt.Properties.Resources.nbt_initialize_ora_bat );
            File.WriteAllBytes( SqlFilePath, ChemSW.Nbt.Properties.Resources.nbt_initialize_ora_sql );


            _CswNbtSchemaModTrnsctn.CswDbCfgInfo.makeConfigurationCurrent( _CswNbtSchemaModTrnsctn.Accessid );
            string serverName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentServerName;
            string userName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentUserName;
            string passWord = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentPlainPwd;


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = BatchFilePath;
            p.StartInfo.Arguments = " " + serverName + " " + userName + " " + passWord + " " + FileLocations;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = false;

            Process SpawnedProcess = System.Diagnostics.Process.Start( p.StartInfo );
            if( false == SpawnedProcess.WaitForExit( _CswNbtSchemaModTrnsctn.UpdtShellWaitMsec ) )
            {
                _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Timed out will running nbt_initialize_ora.bat prior to updates." );
            }
            //else
            //{
            //    _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Finished nbt_initialize_ora.bat prior to updates." );
            //}

            File.Delete( BatchFilePath );
            File.Delete( SqlFilePath );

        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


