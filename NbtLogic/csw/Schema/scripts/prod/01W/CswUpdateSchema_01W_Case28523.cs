using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.LandingPage;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28523
    /// </summary>
    public class CswUpdateSchema_01W_Case28523 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28523; }
        }

        public override void update()
        {
            CswTableUpdate LandingPageUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28523_LandingPageUpdate", "landingpage" );
            DataTable LandingPageTable = LandingPageUpdate.getTable();

            // Clear bogus existing landing page icons
            foreach( DataRow LandingPageRow in LandingPageTable.Rows )
            {
                LandingPageRow["buttonicon"] = "";
            }

            // Set default icons for actions
            CswTableUpdate ActionsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28523_ActionsUpdate", "actions" );
            DataTable ActionsTable = ActionsUpdate.getTable();

            foreach( DataRow ActionRow in ActionsTable.Rows )
            {
                // Set action icon
                string IconFileName = string.Empty;
                CswNbtActionName ActionName = CswNbtAction.ActionNameStringToEnum( ActionRow["actionname"].ToString() );
                Int32 ActionId = CswConvert.ToInt32( ActionRow["actionid"] );
                
                switch( ActionName)
                {
                    case CswNbtActionName.Create_Inspection         : IconFileName = "clipboardcheck.png"; break;
                    case CswNbtActionName.Create_Material           : IconFileName = "atomplus.png";       break;
                    case CswNbtActionName.Design                    : IconFileName = "wrench.png";         break;
                    case CswNbtActionName.DispenseContainer         : IconFileName = "flask.png";          break;
                    case CswNbtActionName.DisposeContainer          : IconFileName = "trash.png";          break;
                    case CswNbtActionName.Edit_View                 : IconFileName = "options.png";        break;
                    case CswNbtActionName.Future_Scheduling         : IconFileName = "calendarstack.png";  break;
                    case CswNbtActionName.HMIS_Reporting            : IconFileName = "doc.png";            break;
                    case CswNbtActionName.KioskMode                 : IconFileName = "barcode.png";        break;
                    case CswNbtActionName.Modules                   : IconFileName = "options.png";        break;
                    case CswNbtActionName.Multi_Edit                : IconFileName = "largeicons.png";     break;
                    case CswNbtActionName.Quotas                    : IconFileName = "plus.png";           break;
                    case CswNbtActionName.Receiving                 : IconFileName = "bottlebox.png";      break;
                    case CswNbtActionName.Reconciliation            : IconFileName = "bottlecalendar.png"; break;
                    case CswNbtActionName.Sessions                  : IconFileName = "personclock.png";    break;
                    case CswNbtActionName.Submit_Request            : IconFileName = "cartplus.png";       break;
                    case CswNbtActionName.Subscriptions             : IconFileName = "envelopes.png";      break;
                    case CswNbtActionName.Tier_II_Reporting         : IconFileName = "doc.png";            break;
                    case CswNbtActionName.UndisposeContainer        : IconFileName = "flask.png";          break;
                    case CswNbtActionName.Upload_Legacy_Mobile_Data : IconFileName = "up.png";             break;
                    case CswNbtActionName.View_Scheduled_Rules      : IconFileName = "clock.png";          break;
                    default: break;
                }
                ActionRow["iconfilename"] = IconFileName;

                //// Fix existing landing page icons
                //foreach( DataRow LandingPageRow in from DataRow LandingPageRow in LandingPageTable.Rows 
                //                                    let LandingPageActionId = CswConvert.ToInt32( LandingPageRow["to_actionid"] ) 
                //                                  where ActionId == LandingPageActionId 
                //                                 select LandingPageRow )
                //{
                //    LandingPageRow["buttonicon"] = IconFileName;
                //}
            } // foreach( DataRow ActionRow in ActionsTable.Rows )

            ActionsUpdate.update( ActionsTable );
            LandingPageUpdate.update( LandingPageTable );

            // update getActiveActions s4
            _CswNbtSchemaModTrnsctn.UpdateS4( "getActiveActions", @"select a.actionid, a.actionname, a.showinlist, a.url, a.category, a.iconfilename, lower(a.actionname) mssqlorder
  from actions a
 where exists (select m.moduleid
          from modules m, jct_modules_actions jma
         where jma.actionid = a.actionid
           and jma.moduleid = m.moduleid
           and m.enabled = '1')
       or not exists (select m.moduleid
          from modules m, jct_modules_actions jma
         where jma.actionid = a.actionid
           and jma.moduleid = m.moduleid)
 order by lower(a.actionname)" );

        } // update()

    }//class CswUpdateSchema_01V_Case28523

}//namespace ChemSW.Nbt.Schema