using System;
using ChemSW.Audit;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02M_CIS52299B : CswUpdateSchemaTo
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
            get { return "switch Component to use NumericRange fieldtype"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public const string oldLowPercentageValue = "Low % Value";
        public const string oldTargetPercentageValue = "Target % Value";
        public const string oldHighPercentageValue = "High % Value";
        public const string oldPercentageValue = "Percentage";

        public override void update()
        {
            CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );

            CswNbtMetaDataObjectClassProp oldLowOCP = ComponentOC.getObjectClassProp( oldLowPercentageValue );
            CswNbtMetaDataObjectClassProp oldTargetOCP = ComponentOC.getObjectClassProp( oldTargetPercentageValue );
            CswNbtMetaDataObjectClassProp oldHighOCP = ComponentOC.getObjectClassProp( oldHighPercentageValue );
            CswNbtMetaDataObjectClassProp oldPercentageOCP = ComponentOC.getObjectClassProp( oldPercentageValue );


            // Make new NumericRange property on MaterialComponent
            CswNbtMetaDataObjectClassProp PercentageRangeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( ComponentOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    PropName = CswNbtObjClassMaterialComponent.PropertyName.PercentageRange,
                    FieldType = CswEnumNbtFieldType.NumericRange,
                    NumberMinValue = 0,
                    NumberMaxValue = 100,
                    NumberPrecision = 3,
                    IsRequired = true,
                    SetValOnAdd = true,
                    AuditLevel = CswEnumAuditLevel.PlainAudit
                } );
            
            
            // Copy values from existing Low/Target/High to new PercentageRange
            CswNbtView CompFixView = _CswNbtSchemaModTrnsctn.makeView();
            CompFixView.ViewName = "52299_compfixview";
            CswNbtViewRelationship rel1 = CompFixView.AddViewRelationship( ComponentOC, false );
            CompFixView.AddViewPropertyAndFilter( rel1, oldLowOCP, Conjunction: CswEnumNbtFilterConjunction.Or, FilterMode: CswEnumNbtFilterMode.NotNull );
            CompFixView.AddViewPropertyAndFilter( rel1, oldTargetOCP, Conjunction: CswEnumNbtFilterConjunction.Or, FilterMode: CswEnumNbtFilterMode.NotNull );
            CompFixView.AddViewPropertyAndFilter( rel1, oldHighOCP, Conjunction: CswEnumNbtFilterConjunction.Or, FilterMode: CswEnumNbtFilterMode.NotNull );
            
            ICswNbtTree CompFixTree = _CswNbtSchemaModTrnsctn.getTreeFromView( CompFixView, IncludeSystemNodes: true );
            while( CompFixTree.getChildNodeCount() > 0 )
            {
                for( Int32 c = 0; c < CompFixTree.getChildNodeCount(); c++ )
                {
                    CompFixTree.goToNthChild( c );

                    CswNbtObjClassMaterialComponent CompNode = CompFixTree.getCurrentNode();
                    CompNode.PercentageRange.Lower = CompNode.Node.Properties[oldLowPercentageValue].AsNumber.Value;
                    CompNode.PercentageRange.Target = CompNode.Node.Properties[oldTargetPercentageValue].AsNumber.Value;
                    CompNode.PercentageRange.Upper = CompNode.Node.Properties[oldHighPercentageValue].AsNumber.Value;
                    CompNode.PercentageRange.Units = "%";

                    CompNode.Node.Properties[oldLowPercentageValue].AsNumber.Value = Double.NaN;
                    CompNode.Node.Properties[oldTargetPercentageValue].AsNumber.Value = Double.NaN;
                    CompNode.Node.Properties[oldHighPercentageValue].AsNumber.Value = Double.NaN;
                    
                    CompNode.postChanges( false );

                    CompFixTree.goToParentNode();
                } // for( Int32 c = 0; c < CompFixTree.getChildNodeCount(); c++ )

                // next iteration
                CompFixTree = _CswNbtSchemaModTrnsctn.getTreeFromView( CompFixView, IncludeSystemNodes: true );
            } // while( CompFixTree.getChildNodeCount() > 0 )


            // Delete existing Low/Target/High
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( oldLowOCP, true );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( oldTargetOCP, true );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( oldHighOCP, true );
            _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( oldPercentageOCP, true );

        }
    }
}