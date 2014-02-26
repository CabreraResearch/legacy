using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29311
    /// </summary>
    public class CswUpdateSchema_02K_Case31509 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31509; }
        }

        public override void update()
        {
            // Fix any remnants of the old Design Mode (Design.aspx)
            CswTableUpdate ActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31509_Action_Update", "actions" );
            DataTable ActionTable = ActionUpdate.getTable( "where actionname = '" + CswEnumNbtActionName.Design + "'" );
            if( ActionTable.Rows.Count > 0 )
            {
                DataRow ActionRow = ActionTable.Rows[0];

                // Fix the action row
                ActionRow["showinlist"] = CswConvert.ToDbVal( false );
                ActionRow["url"] = DBNull.Value;
                ActionUpdate.update( ActionTable );

                // Remove button from landing pages
                Int32 DesignActionId = CswConvert.ToInt32( ActionRow["actionid"] );
                if( DesignActionId != Int32.MinValue )
                {
                    CswTableUpdate LandingPageUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "31509_LandingPage_update", "landingpage" );
                    DataTable LandingPageTable = LandingPageUpdate.getTable( "to_actionid", DesignActionId );
                    foreach( DataRow LandingPageRow in LandingPageTable.Rows )
                    {
                        LandingPageRow.Delete();
                    }
                    LandingPageUpdate.update( LandingPageTable );
                }
            } // if( ActionTable.Rows.Count > 0 )
        } // update()


    }//class CswUpdateSchema_02K_Case31509

}//namespace ChemSW.Nbt.Schema