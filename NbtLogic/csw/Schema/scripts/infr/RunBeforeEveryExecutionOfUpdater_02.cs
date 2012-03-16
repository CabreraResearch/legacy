using System;
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

        public override void update()
        {
            //***************  ADD your own code
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState("Running nbt_initialize_ora.bat prior to updates."); //this one blocks        
            //"Initialize" is only for updateSequences()!!!
            _CswNbtSchemaModTrnsctn.CswDbCfgInfo.makeConfigurationCurrent( _CswNbtSchemaModTrnsctn.Accessid );
            string serverName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentServerName;
            string userName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentUserName;
            string passWord = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentPlainPwd;


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = _CswNbtSchemaModTrnsctn.ConfigFileLocation + "\\nbt_initialize_ora.bat";
            p.StartInfo.Arguments = " " + serverName + " " + userName + " " + passWord + " " + _CswNbtSchemaModTrnsctn.ConfigFileLocation;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.UseShellExecute=false;
            p.StartInfo.RedirectStandardOutput = false;

            System.Diagnostics.Process.Start( p.StartInfo );
            p.WaitForExit( 30000 );

            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Finished nbt_initialize_ora.bat prior to updates." ); //this one doesn't block

        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


