using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26840
    /// </summary>
    public class CswUpdateSchema_01W_Case26840 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 26840; }
        }

        public override void update()
        {
            #region OC Props

            CswNbtMetaDataObjectClass InventoryLevelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryLevelClass );
            CswNbtMetaDataObjectClassProp MaterialProp = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Material );
            CswNbtMetaDataObjectClassProp LocationProp = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Location );
            CswNbtMetaDataObjectClassProp TypeProp = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Type );
            CswNbtMetaDataObjectClassProp LevelProp = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Level );
            CswNbtMetaDataObjectClassProp CurrentQuantityProp = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.CurrentQuantity );
            CswNbtMetaDataObjectClassProp StatusProp = InventoryLevelOC.getObjectClassProp( CswNbtObjClassInventoryLevel.PropertyName.Status );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp LocInventoryGroupProp = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.InventoryGroup );

            CswNbtMetaDataObjectClass InventoryGroupPermissionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.InventoryGroupPermissionClass );
            CswNbtMetaDataObjectClassProp IGPInventoryGroupProp = InventoryGroupPermissionOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.InventoryGroup );
            CswNbtMetaDataObjectClassProp IGPWorkUnitProp = InventoryGroupPermissionOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.WorkUnit );
            CswNbtMetaDataObjectClassProp IGPRoleProp = InventoryGroupPermissionOC.getObjectClassProp( CswNbtObjClassInventoryGroupPermission.PropertyName.Role );

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp WorkUnitProp = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.WorkUnit );
            CswNbtMetaDataObjectClassProp RoleProp = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Role );

            #endregion OC Props

            #region SQL Query Template

            String SqlText = @"
              select 
                il.op_{0} as Material, 
                il.op_{1} as LOCATION, 
                il.op_{2} as TYPE, 
                il.op_{3} as inventorylevel, 
                il.op_{4} as CurrentQuantity, 
                il.op_{5} as Status
              from ocinventorylevelclass il
                inner join oclocationclass l on l.nodeid = il.op_{1}_fk
                inner join ocinventorygroupclass ig on ig.nodeid = l.op_{6}_fk
                inner join (select * from ocinventorygrouppermissioncl) igp on igp.op_{7}_fk = ig.nodeid
                inner join ocworkunitclass wu on wu.nodeid = igp.op_{8}_fk
                inner join (select * from ocuserclass) u on u.op_{9}_fk = wu.nodeid
              where il.op_{5} != 'Ok'
                and u.op_{11}_fk = igp.op_{10}_fk
                and u.nodeid = {12}";

            #endregion SQL Query Template

            String SelectText = String.Format( SqlText,
                MaterialProp.PropId,
                LocationProp.PropId,
                TypeProp.PropId,
                LevelProp.PropId,
                CurrentQuantityProp.PropId,
                StatusProp.PropId,
                LocInventoryGroupProp.PropId,
                IGPInventoryGroupProp.PropId,
                IGPWorkUnitProp.PropId,
                WorkUnitProp.PropId,
                IGPRoleProp.PropId,
                RoleProp.PropId,
                "{userid}"
            );

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataNodeType ReportNT = ReportOC.FirstNodeType;
            if( null != ReportNT )
            {
                CswNbtObjClassReport ReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                ReportNode.ReportName.Text = "Deficient Inventory Levels";
                ReportNode.Category.Text = "Containers";
                ReportNode.SQL.Text = SelectText;
                ReportNode.postChanges( false );

                CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MailReportClass );
                CswNbtMetaDataNodeType MailReportNT = MailReportOC.FirstNodeType;
                if( null != MailReportNT )
                {
                    CswNbtMetaDataNodeTypeProp MailReportNameNTP = MailReportNT.getNodeTypeProp( "Name" );
                    CswNbtObjClassMailReport MailReportNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( MailReportNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    if( null != MailReportNameNTP )
                    {
                        MailReportNode.Node.Properties[MailReportNameNTP].AsText.Text = "Deficient Inventory Levels";
                    }
                    MailReportNode.OutputFormat.Value = "link";
                    MailReportNode.Type.Value = CswNbtObjClassMailReport.TypeOptionReport;
                    MailReportNode.Report.RelatedNodeId = ReportNode.NodeId;
                    MailReportNode.Message.Text = "The following levels are above maximum inventory or below mnimum inventory:";
                    MailReportNode.Enabled.Checked = Tristate.True;

                    CswRateInterval DailyRate = _CswNbtSchemaModTrnsctn.makeRateInterval();
                    DailyRate.setHourly( 24, DateTime.Today );
                    MailReportNode.DueDateInterval.RateInterval = DailyRate;

                    MailReportNode.postChanges( true );
                }
            }            
        }//Update()
    }//class CswUpdateSchemaCase_01W_26840
}//namespace ChemSW.Nbt.Schema