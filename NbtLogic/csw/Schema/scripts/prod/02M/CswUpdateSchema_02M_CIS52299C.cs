using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52299C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52299; }
        }

        public override string Title
        {
            get { return "Fix MaterialComponent nodetypes for new field type"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            // Set default value of Units to %
            CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );

            foreach( CswNbtMetaDataNodeType NodeType in ComponentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PercentRangeNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
                PercentRangeNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleNumericRange.AttributeName.DefaultValue].SetSubFieldValue( CswNbtFieldTypeRuleNumericRange.SubFieldName.Units, "%" );
            }


            // Set layout on nodetypes
            CswNbtSchemaUpdateLayoutMgr AddLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.MaterialComponentClass, LayoutType: CswEnumNbtLayoutType.Add );
            AddLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            AddLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            AddLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
            AddLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Active );
            AddLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting );

            CswNbtSchemaUpdateLayoutMgr EditLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.MaterialComponentClass, LayoutType: CswEnumNbtLayoutType.Edit );
            EditLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            EditLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            EditLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
            EditLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Active );
            EditLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting );

        }
    }
}