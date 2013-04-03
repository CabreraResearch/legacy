using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29125
    /// </summary>
    public class CswUpdateSchema_01Y_Case29125 : CswUpdateSchemaTo
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
                CswNbtMetaDataNodeTypeProp InspectionGroupNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
                InspectionGroupNTP.ServerManaged = false;
                InspectionGroupNTP.ReadOnly = true;
                CswNbtMetaDataNodeTypeProp InspectionTargetTypeNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.ParentType );
                InspectionTargetTypeNTP.ServerManaged = true;
                InspectionTargetTypeNTP.Multi = PropertySelectMode.Single;
                InspectionTargetTypeNTP.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp InspectionTargetViewNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.ParentView );
                InspectionTargetViewNTP.ServerManaged = true;
                InspectionTargetViewNTP.removeFromAllLayouts();
                CswNbtMetaDataNodeTypeProp InspectionTypeNTP = InspectionScheduleNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );
                InspectionTypeNTP.ServerManaged = false;
                InspectionTypeNTP.ReadOnly = false;
                InspectionTypeNTP.Multi = PropertySelectMode.Multiple;
            }

        } //Update()

    }//class CswUpdateSchema_01Y_Case29125

}//namespace ChemSW.Nbt.Schema