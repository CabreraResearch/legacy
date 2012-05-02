using System.Collections.Generic;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25041
    /// </summary>
    public class CswUpdateSchemaCase25041 : CswUpdateSchemaTo
    {
        public override void update()
        {
            string OOCStatus = "OOC";
            string DeficientStatus = "Deficient";
            string StatusListOptions = "Not Inspected,OK,Deficient";

            //Update InspectionTarget Status ObjClassProp (rename 'OOC' in Status list to 'Deficient')
            CswNbtMetaDataObjectClass InspectionTargetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp(
                            InspectionTargetOC.getObjectClassProp( CswNbtObjClassInspectionTarget.StatusPropertyName ),
                            CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions,
                            StatusListOptions );

            //Update 'OOC Inspections' Action
            CswTableUpdate ActionUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "Action_Update", "actions" );
            DataTable ActionTable = ActionUpdate.getTable( "where actionname = 'OOC Inspections'" );
            foreach( DataRow ActionRow in ActionTable.Rows )
            {
                ActionRow["actionname"] = "Deficient Inspections";
            }
            ActionUpdate.update( ActionTable );

            //Update all nodes with Inspection Status 'OOC'
            IEnumerable<CswNbtNode> InspectionTargetNodes = InspectionTargetOC.getNodes( false, true );
            foreach( CswNbtNode Node in InspectionTargetNodes )
            {
                CswNbtObjClassInspectionTarget InspectionTargetNode = CswNbtNodeCaster.AsInspectionTarget( Node );
                if( InspectionTargetNode.Status.Value == OOCStatus )
                {
                    InspectionTargetNode.Status.Value = DeficientStatus;
                    InspectionTargetNode.postChanges( true );
                }
            }

            //Update 'OOC Issues last 30 days' Report
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            IEnumerable<CswNbtNode> ReportNodes = ReportOC.getNodes( false, true );
            foreach( CswNbtNode Node in ReportNodes )
            {
                string nodeName = CswNbtNodeCaster.AsReport( Node ).ReportName.Text;
                if( nodeName.Contains( OOCStatus ) )
                {
                    //Node.NodeName = Node.NodeName.Replace( OOCStatus, DeficientStatus ); //Not Necessary
                    CswNbtNodeCaster.AsReport( Node ).ReportName.Text = CswNbtNodeCaster.AsReport( Node ).ReportName.Text.Replace( OOCStatus, DeficientStatus );
                    Node.postChanges( true );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25041

}//namespace ChemSW.Nbt.Schema