using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28690
    /// </summary>
    public class CswUpdateSchema_02B_Case28690A : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28690; }
        }

        public override void update()
        {
            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropertySet( CswEnumNbtPropertySetName.MaterialSet, "atom.png" );

            //Update jct_propertyset_objectclass
            CswTableUpdate JctPSOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_jctpsoc_update", "jct_propertyset_objectclass" );
            DataTable JctPSOCTable = JctPSOCUpdate.getEmptyTable();
            _addObjClassToPropertySetMaterial( JctPSOCTable, CswEnumNbtObjectClass.ChemicalClass, MaterialPS.PropertySetId );
            _addObjClassToPropertySetMaterial( JctPSOCTable, CswEnumNbtObjectClass.NonChemicalClass, MaterialPS.PropertySetId );
            JctPSOCUpdate.update( JctPSOCTable );

            //Update jct_propertyset_ocprop
            CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_jctpsocp_update", "jct_propertyset_ocprop" );
            DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();
            _addObjClassPropsToPropertySetMaterial( JctPSOCPTable, CswEnumNbtObjectClass.ChemicalClass, MaterialPS.PropertySetId );
            _addObjClassPropsToPropertySetMaterial( JctPSOCPTable, CswEnumNbtObjectClass.NonChemicalClass, MaterialPS.PropertySetId );
            JctPSOCPUpdate.update( JctPSOCPTable );
        } // update()

        private void _addObjClassToPropertySetMaterial( DataTable JctPSOCTable, string ObjClassName, int PropertySetId )
        {
            DataRow NewJctPSOCRow = JctPSOCTable.NewRow();
            NewJctPSOCRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( ObjClassName );
            NewJctPSOCRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
            JctPSOCTable.Rows.Add( NewJctPSOCRow );
        }

        private void _addObjClassPropsToPropertySetMaterial( DataTable JctPSOCPTable, string ObjClassName, int PropertySetId )
        {
            CswNbtMetaDataObjectClass MaterialObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in MaterialObjectClass.getObjectClassProps() )
            {
                bool doInsert = ( ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.MaterialId ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.TradeName ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Supplier ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.PartNumber ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Receive ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Request ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.C3ProductId ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.C3SyncDate );
                if( doInsert )
                {
                    DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                    NewJctPSOCPRow["objectclasspropid"] = ObjectClassProp.PropId;
                    NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
                    JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
                }
            }
        }

    }//class CswUpdateSchema_02B_Case28690A
}//namespace ChemSW.Nbt.Schema