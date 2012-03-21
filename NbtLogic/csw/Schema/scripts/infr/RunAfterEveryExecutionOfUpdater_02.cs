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
    public class RunAfterEveryExecutionOfUpdater_02 : CswUpdateSchemaTo
    {

        public override void update()
        {
            //***************  ADD your own code
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Running nbt_finalize_ora.bat after updates." ); //this one doesn't blocks        
            //"Initialize" is only for updateSequences()!!!
            _CswNbtSchemaModTrnsctn.CswDbCfgInfo.makeConfigurationCurrent( _CswNbtSchemaModTrnsctn.Accessid );
            string serverName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentServerName;
            string userName = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentUserName;
            string passWord = _CswNbtSchemaModTrnsctn.CswDbCfgInfo.CurrentPlainPwd;


            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.FileName = _CswNbtSchemaModTrnsctn.ConfigFileLocation + "\\nbt_finalize_ora.bat";
            p.StartInfo.Arguments = " " + serverName + " " + userName + " " + passWord + " " + _CswNbtSchemaModTrnsctn.ConfigFileLocation;
            p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = false;

            System.Diagnostics.Process.Start( p.StartInfo );
            //we don't wait 
            _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( "Left nbt_finalize_ora.bat after updates without waiting." ); //this one doesn't block


        }//Update()

    }//class CswUpdateSchema_Infr_TakeDump

}//namespace ChemSW.Nbt.Schema


