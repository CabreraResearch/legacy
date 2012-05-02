using System.Collections.Generic;
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
            string ActionUpdate = "update actions set actionname = 'Deficient Inspections' where actionname = 'OOC Inspections'";
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( ActionUpdate );

            //Update all nodes with Inspection Status 'OOC'
            IEnumerable<CswNbtNode> InspectionTargetNodes = InspectionTargetOC.getNodes( false, true );
            foreach( CswNbtNode Node in InspectionTargetNodes )
            {
                string nodeStatus = Node.Properties[CswNbtObjClassInspectionTarget.StatusPropertyName].AsList.Value;
                if( nodeStatus == OOCStatus )
                {
                    Node.Properties[CswNbtObjClassInspectionTarget.StatusPropertyName].AsList.Value = DeficientStatus;
                    Node.postChanges( true );
                }
            }

            //Update 'OOC Issues last 30 days' Report
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            IEnumerable<CswNbtNode> ReportNodes = ReportOC.getNodes( false, true );
            foreach( CswNbtNode Node in ReportNodes )
            {
                string nodeName = Node.Properties[CswNbtObjClassReport.ReportNamePropertyName].AsText.Text;
                if( nodeName.Contains( OOCStatus ) )
                {
                    Node.NodeName = Node.NodeName.Replace( OOCStatus, DeficientStatus );
                    Node.Properties[CswNbtObjClassReport.ReportNamePropertyName].AsText.Text =
                        Node.Properties[CswNbtObjClassReport.ReportNamePropertyName].AsText.Text.Replace( OOCStatus, DeficientStatus );
                    Node.postChanges( true );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25041

}//namespace ChemSW.Nbt.Schema