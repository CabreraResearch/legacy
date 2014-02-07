using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31783 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31783; }
        }

        public override string Title
        {
            get { return "Migrate Layout Data Prep - Fix FireClassExemptAmount Layout"; }
        }

        public override string AppendToScriptName()
        {
            return "Pre";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass FCEAOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountClass );
            foreach( CswNbtMetaDataNodeType FCEANt in FCEAOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 7 + ", display_column = " + 1 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 8 + ", display_column = " + 1 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 9 + ", display_column = " + 1 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 10 + ", display_column = " + 1 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 11 + ", display_column = " + 1 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 12 + ", display_column = " + 1 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 7 + ", display_column = " + 2 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 8 + ", display_column = " + 2 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 9 + ", display_column = " + 2 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 10 + ", display_column = " + 2 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 11 + ", display_column = " + 2 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 12 + ", display_column = " + 2 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 7 + ", display_column = " + 3 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 8 + ", display_column = " + 3 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 9 + ", display_column = " + 3 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
                ExemptAmountNTP = FCEANt.getNodeTypePropByObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptFootnotes );
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql(
                    "update nodetype_layout set display_row = " + 10 + ", display_column = " + 3 + " where nodetypepropid = " + ExemptAmountNTP.PropId );
            }
        } // update()

    } // class CswUpdateSchema_02K_Case31783
} // namespace