using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29125
    /// </summary>
    public class CswUpdateSchema_01Y_Case29125B : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 29125; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType InspectionScheduleNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeFirstVersion( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            if( null != InspectionScheduleNT )
            {
                CswNbtMetaDataNodeTypeTab SettingsTab = InspectionScheduleNT.getNodeTypeTab( "Settings" );
                if( null != SettingsTab )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeTypeTab( SettingsTab );
                }
                CswNbtMetaDataNodeTypeProp SummaryNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Summary );
                SummaryNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, InspectionScheduleNT.getIdentityTab().TabId );
                CswNbtMetaDataNodeTypeProp InspectionGroupNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
                InspectionGroupNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, InspectionScheduleNT.getFirstNodeTypeTab().TabId, 1, 2 );
                CswNbtMetaDataNodeTypeProp InspectionTypeNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );
                InspectionTypeNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, InspectionScheduleNT.getFirstNodeTypeTab().TabId, 2, 2 );             
                CswNbtMetaDataNodeTypeProp InspectionTargetTypeNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.ParentType );
                InspectionTargetTypeNTP.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp InspectionTargetViewNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.ParentView );
                InspectionTargetViewNTP.removeFromAllLayouts();
            }
        } //Update()
    }//class CswUpdateSchema_01Y_Case29125B
}//namespace ChemSW.Nbt.Schema