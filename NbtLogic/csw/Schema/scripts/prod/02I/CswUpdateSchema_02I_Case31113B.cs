using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case31113B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31113; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override string Title
        {
            get { return "Organize MaterialComponent NodeType Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );
            foreach( CswNbtMetaDataNodeType MaterialComponentNT in MaterialComponentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PercentageNTP = MaterialComponentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Percentage );
                PercentageNTP.Hidden = true;
                PercentageNTP.removeFromAllLayouts();
            }

            foreach( CswNbtObjClassMaterialComponent MaterialComponentNode in MaterialComponentOC.getNodes( false, false, false ) )
            {
                MaterialComponentNode.TargetPercentageValue.Value = MaterialComponentNode.Percentage.Value;
                MaterialComponentNode.HighPercentageValue.Value = MaterialComponentNode.Percentage.Value;
                MaterialComponentNode.postChanges( false );
            }

            CswNbtSchemaUpdateLayoutMgr AddLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.MaterialComponentClass, LayoutType: CswEnumNbtLayoutType.Add );
            AddLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            AddLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            AddLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.LowPercentageValue );
            AddLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.TargetPercentageValue );
            AddLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HighPercentageValue );
            AddLayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Active );
            AddLayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting );

            CswNbtSchemaUpdateLayoutMgr EditLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.MaterialComponentClass, LayoutType: CswEnumNbtLayoutType.Edit );
            EditLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            EditLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            EditLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.LowPercentageValue );
            EditLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.TargetPercentageValue );
            EditLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HighPercentageValue );
            EditLayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Active );
            EditLayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting );
        }
    }
}