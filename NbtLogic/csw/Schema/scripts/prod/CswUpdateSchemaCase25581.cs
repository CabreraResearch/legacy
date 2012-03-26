﻿using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.Security;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25581
    /// </summary>
    public class CswUpdateSchemaCase25581 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // 'Due Inspections (all)' view and 'OOC Issues' report should be in the Inspections category rather than the SI_Example category.
            string InspectionsCategory = "Inspections";

            CswNbtView DueInspView = _CswNbtSchemaModTrnsctn.restoreView( "Due Inspections (all)" );
            if( DueInspView != null )
            {
                DueInspView.Category = InspectionsCategory;
                DueInspView.save();
            }

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            foreach( CswNbtNode ReportNode in ReportOC.getNodes( false, true ) )
            {
                CswNbtObjClassReport ReportNodeAsReport = CswNbtNodeCaster.AsReport( ReportNode );
                if(ReportNodeAsReport.ReportName.Text == "OOC Issues last 30 days")
                {
                    ReportNodeAsReport.Category.Text = InspectionsCategory; 
                    ReportNodeAsReport.postChanges(false);
                }
            }

            // Also the 'Create Inspection' action
            CswNbtAction CreateInspAction = _CswNbtSchemaModTrnsctn.Actions[Actions.CswNbtActionName.Create_Inspection];
            CswTableUpdate ActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "25581_Actions_Update", "actions" );
            DataTable ActionTable = ActionUpdate.getTable( "actionid", CreateInspAction.ActionId );
            if( ActionTable.Rows.Count > 0 )
            {
                ActionTable.Rows[0]["category"] = InspectionsCategory;
            }
            ActionUpdate.update( ActionTable );

        }//Update()

    }//class CswUpdateSchemaCase25581

}//namespace ChemSW.Nbt.Schema