using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 31501
    /// </summary>
    public class CswUpdateSchema_02K_Case31501 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 31501; }
        }

        private bool _unitExists( CswNbtMetaDataNodeType UnitNT, string UnitName, string AliasName )
        {
            CswNbtMetaDataNodeTypeProp UnitNameNTP = UnitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
            CswNbtMetaDataNodeTypeProp UnitAliasesNTP = UnitNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Aliases );
            CswNbtView View = _CswNbtSchemaModTrnsctn.makeView();
            View.ViewName = "31501_unitview";
            CswNbtViewRelationship rel1 = View.AddViewRelationship( UnitNT, false );
            // name equals UnitName
            View.AddViewPropertyAndFilter( rel1,
                                           UnitNameNTP,
                                           SubFieldName: CswNbtFieldTypeRuleText.SubFieldName.Text,
                                           FilterMode: CswEnumNbtFilterMode.Equals,
                                           Value: UnitName );
            // ... or name equals AliasName
            View.AddViewPropertyAndFilter( rel1,
                                           UnitNameNTP,
                                           Conjunction: CswEnumNbtFilterConjunction.Or,
                                           SubFieldName: CswNbtFieldTypeRuleText.SubFieldName.Text,
                                           FilterMode: CswEnumNbtFilterMode.Equals,
                                           Value: AliasName );
            // ... or alias contains UnitName
            View.AddViewPropertyAndFilter( rel1,
                                           UnitAliasesNTP,
                                           Conjunction: CswEnumNbtFilterConjunction.Or,
                                           SubFieldName: CswNbtFieldTypeRuleText.SubFieldName.Text,
                                           FilterMode: CswEnumNbtFilterMode.Contains,
                                           Value: UnitName );
            // ... or alias contains AliasName
            View.AddViewPropertyAndFilter( rel1,
                                           UnitAliasesNTP,
                                           Conjunction: CswEnumNbtFilterConjunction.Or,
                                           SubFieldName: CswNbtFieldTypeRuleText.SubFieldName.Text,
                                           FilterMode: CswEnumNbtFilterMode.Contains,
                                           Value: AliasName );
            ICswNbtTree Tree = _CswNbtSchemaModTrnsctn.getTreeFromView( View, true );
            return Tree.getChildNodeCount() > 0;
        }

        public override void update()
        {
            // New UOMs: pint, quart, and ug
            CswNbtMetaDataNodeType Unit_VolumeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit_Volume" );
            if( null != Unit_VolumeNT )
            {
                if( false == _unitExists( Unit_VolumeNT, "pint", "pt" ) )
                {
                    _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( Unit_VolumeNT.NodeTypeId, delegate( CswNbtNode newNode )
                        {
                            CswNbtObjClassUnitOfMeasure newUnit = newNode;
                            newUnit.Name.Text = "pint";
                            newUnit.Aliases.Text = "pt";
                            newUnit.ConversionFactor.Base = 4.73176; //pint to Liters
                            newUnit.ConversionFactor.Exponent = -1;
                            newUnit.Fractional.Checked = CswEnumTristate.True;
                        } );
                }
                if( false == _unitExists( Unit_VolumeNT, "quart", "qt" ) )
                {
                    _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( Unit_VolumeNT.NodeTypeId, delegate( CswNbtNode newNode )
                        {
                            CswNbtObjClassUnitOfMeasure newUnit = newNode;
                            newUnit.Name.Text = "quart";
                            newUnit.Aliases.Text = "qt";
                            newUnit.ConversionFactor.Base = 9.46353; //quart to Liters
                            newUnit.ConversionFactor.Exponent = -1;
                            newUnit.Fractional.Checked = CswEnumTristate.True;
                        } );
                }
            }
            CswNbtMetaDataNodeType Unit_WeightNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit_Weight" );
            if( null != Unit_WeightNT )
            {
                if( false == _unitExists( Unit_VolumeNT, "ug", "microg" ) )
                {
                    _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( Unit_WeightNT.NodeTypeId, delegate( CswNbtNode newNode )
                        {
                            CswNbtObjClassUnitOfMeasure newUnit = newNode;
                            newUnit.Name.Text = "ug";
                            newUnit.Aliases.Text = "microgram,microg";
                            newUnit.ConversionFactor.Base = 1; //ug to kg
                            newUnit.ConversionFactor.Exponent = -9;
                            newUnit.Fractional.Checked = CswEnumTristate.True;
                        } );
                }
            }
        } // update()


    }//class CswUpdateSchema_02K_Case31501

}//namespace ChemSW.Nbt.Schema