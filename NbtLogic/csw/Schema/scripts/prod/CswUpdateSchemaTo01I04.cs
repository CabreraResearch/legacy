using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-04
    /// </summary>
    public class CswUpdateSchemaTo01I04 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 04 ); } }
        public CswUpdateSchemaTo01I04( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }


        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }


        public void update()
        {
			// case 7608
			// 1. Following the steps in the Wiki for creating a new object class - Implement a new class - Unit Type class - using CswNbtObjClassUnitOfMeasure as a model
            // 2. Add properties to the new Class: (UnitType)Name, BaseUnit.  Base Unit points to a unit of measure that is the base unit for that Unit Type
            // 3. Implement a new NodeType called Unit Type - based on the new Unit Type class 
            // 4. Add a property "Conversion Factor" to Unit Of Measure, which is a Scientific fieldtype value, that scales the quantity to the "Base Unit".  Each Unit of measure will have a conversion factor that converts its value and unit to a value in base units.  That way all values in a unit type, but different units of measure, can be compared

            // create the class
            Int32 NewObjectClassId = _CswNbtSchemaModTrnsctn.createObjectClass("UnitTypeClass", "scales.gif", false, false);

            // refresh so that meta data has the new class
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            // create the node type
            CswNbtMetaDataNodeType NewNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType("UnitTypeClass", "Unit Type", string.Empty);

            // get an updateable table so we can add properties to the classes
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("01I-04_OCP_Update", "object_class_props");
			DataTable OCPTable = OCPUpdate.getEmptyTable();

            // pull back the newly created Unit Type class
            CswNbtMetaDataObjectClass UnitTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.UnitTypeClass);

            // add properties to UnitType class
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitTypeOC.ObjectClassId, CswNbtObjClassUnitType.NamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue);
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitTypeOC.ObjectClassId, CswNbtObjClassUnitType.BaseUnitPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, Int32.MinValue, Int32.MinValue);

            // we have to add properties to Unit Of Measure class -s o get UnitOfMeasure class
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass);

            // add properties to UnitOfMeasure class
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitOfMeasureOC.ObjectClassId, CswNbtObjClassUnitOfMeasure.UnitTypePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, Int32.MinValue, Int32.MinValue);
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitOfMeasureOC.ObjectClassId, CswNbtObjClassUnitOfMeasure.ConversionFactorPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Scientific, Int32.MinValue, Int32.MinValue);

            // save changes to properties
            OCPUpdate.update(OCPTable);

			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            //_CswNbtSchemaModTrnsctn.Permit.set(
            // Grant permission to Administrator
            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" );
            _CswNbtSchemaModTrnsctn.Permit.set(new CswNbtPermit.NodeTypePermission[] {
												CswNbtPermit.NodeTypePermission.Delete, 
												CswNbtPermit.NodeTypePermission.Create, 
												CswNbtPermit.NodeTypePermission.Edit, 
												CswNbtPermit.NodeTypePermission.View },
                                        NewNodeType,
                                        CswNbtNodeCaster.AsRole(RoleNode),
                                        true);
            CswNbtNode RoleNode2 = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName("chemsw_admin_role");
            _CswNbtSchemaModTrnsctn.Permit.set(new CswNbtPermit.NodeTypePermission[] {
												CswNbtPermit.NodeTypePermission.Delete, 
												CswNbtPermit.NodeTypePermission.Create, 
												CswNbtPermit.NodeTypePermission.Edit, 
												CswNbtPermit.NodeTypePermission.View },
                                        NewNodeType,
                                        CswNbtNodeCaster.AsRole(RoleNode2),
                                        true);

        }//Update()

    }//class CswUpdateSchemaTo01I02

}//namespace ChemSW.Nbt.Schema


