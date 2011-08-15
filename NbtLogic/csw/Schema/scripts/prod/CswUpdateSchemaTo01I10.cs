using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-10
    /// </summary>
    public class CswUpdateSchemaTo01I10 : ICswUpdateSchemaTo
    {
        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswProdUpdtRsrc _CswProdUpdtRsrc = null;
        public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 10 ); } }
        public CswUpdateSchemaTo01I10( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
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
            Int32 NewUnitTypeClassId = _CswNbtSchemaModTrnsctn.createObjectClass("UnitTypeClass", "scales.gif", false, false);

            // refresh so that meta data has the new class
            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            // get an updateable table so we can add properties to the classes
            CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate("01I-04_OCP_Update", "object_class_props");
            DataTable OCPTable = OCPUpdate.getEmptyTable();

            // pull back the newly created Unit Type class
            CswNbtMetaDataObjectClass UnitTypeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.UnitTypeClass);

            // get UnitOfMeasure class
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass);

            // add properties to UnitType class
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitTypeOC.ObjectClassId, CswNbtObjClassUnitType.NamePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Text, Int32.MinValue, Int32.MinValue);
            //_CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitTypeOC.ObjectClassId, CswNbtObjClassUnitType.BaseUnitPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, Int32.MinValue, Int32.MinValue);
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitTypeOC, CswNbtObjClassUnitType.BaseUnitPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), UnitOfMeasureOC.ObjectClassId, false, false, false, false, string.Empty, Int32.MinValue, Int32.MinValue);

            // add properties to UnitOfMeasure class
            //_CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitOfMeasureOC.ObjectClassId, CswNbtObjClassUnitOfMeasure.UnitTypePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, Int32.MinValue, Int32.MinValue);
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitOfMeasureOC, CswNbtObjClassUnitOfMeasure.UnitTypePropertyName, CswNbtMetaDataFieldType.NbtFieldType.Relationship, false, false, true, CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), UnitTypeOC.ObjectClassId, false, false, false, false, string.Empty, Int32.MinValue, Int32.MinValue);
            _CswNbtSchemaModTrnsctn.addObjectClassPropRow(OCPTable, UnitOfMeasureOC.ObjectClassId, CswNbtObjClassUnitOfMeasure.ConversionFactorPropertyName, CswNbtMetaDataFieldType.NbtFieldType.Scientific, Int32.MinValue, Int32.MinValue);

            // save changes to properties
            OCPUpdate.update(OCPTable);

            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

            // create the UnitType node type
            CswNbtMetaDataNodeType UnitTypeNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType("UnitTypeClass", "Unit Type", string.Empty);

            // Have the Node name based on the Name property
            UnitTypeNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry(CswNbtObjClassUnitType.NamePropertyName);

            // get Generic class to swap ForeignKey
            CswNbtMetaDataObjectClass GenericOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass);

            // We have to set the property type again
            CswNbtMetaDataNodeTypeProp BaseUnitNTP = UnitTypeNT.getNodeTypePropByObjectClassPropName(CswNbtObjClassUnitType.BaseUnitPropertyName);
            CswNbtMetaDataNodeType UnitOfMeasureNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType("Unit of Measure");
            CswNbtMetaDataNodeTypeProp UnitTypeNTP = UnitOfMeasureNT.getNodeTypePropByObjectClassPropName(CswNbtObjClassUnitOfMeasure.UnitTypePropertyName);

            // We have to toggle the Foreign Key - so first set it to the wrong value
            BaseUnitNTP.SetFK(CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), UnitOfMeasureNT.NodeTypeId, string.Empty, Int32.MinValue);
            // now set it to the correct value
            BaseUnitNTP.SetFK(CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), UnitOfMeasureOC.ObjectClassId, string.Empty, Int32.MinValue);

            // We have to toggle the Foreign Key - so first set it to the wrong value
            UnitTypeNTP.SetFK(CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), UnitTypeNT.NodeTypeId, string.Empty, Int32.MinValue);
            // now set it to the correct value
            UnitTypeNTP.SetFK(CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), UnitTypeOC.ObjectClassId, string.Empty, Int32.MinValue);

            //_CswNbtSchemaModTrnsctn.Permit.set(
            // Grant permission to Administrator
            CswNbtNode RoleNode = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName("Administrator");
            _CswNbtSchemaModTrnsctn.Permit.set(new CswNbtPermit.NodeTypePermission[] {
												CswNbtPermit.NodeTypePermission.Delete, 
												CswNbtPermit.NodeTypePermission.Create, 
												CswNbtPermit.NodeTypePermission.Edit, 
												CswNbtPermit.NodeTypePermission.View },
                                        UnitTypeNT,
                                        CswNbtNodeCaster.AsRole(RoleNode),
                                        true);
            CswNbtNode RoleNode2 = _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName("chemsw_admin_role");
            _CswNbtSchemaModTrnsctn.Permit.set(new CswNbtPermit.NodeTypePermission[] {
												CswNbtPermit.NodeTypePermission.Delete, 
												CswNbtPermit.NodeTypePermission.Create, 
												CswNbtPermit.NodeTypePermission.Edit, 
												CswNbtPermit.NodeTypePermission.View },
                                        UnitTypeNT,
                                        CswNbtNodeCaster.AsRole(RoleNode2),
                                        true);


            // case 20939
            // Steve says this is no longer required
            //_CswNbtSchemaModTrnsctn.createAction(CswNbtActionName.Create_Inspection, true, string.Empty, "Inspections");
            _CswNbtSchemaModTrnsctn.Permit.set(CswNbtActionName.Create_Inspection,
                                        CswNbtNodeCaster.AsRole(RoleNode),
                                        true);
            _CswNbtSchemaModTrnsctn.Permit.set(CswNbtActionName.Create_Inspection,
                                        CswNbtNodeCaster.AsRole(RoleNode2),
                                        true);
        }//Update()

    }//class CswUpdateSchemaTo01I10

}//namespace ChemSW.Nbt.Schema


