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
            CswTableUpdate JctPSOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_jct_update", "jct_propertyset_objectclass" );
            DataTable JctPSOCTable = JctPSOCUpdate.getEmptyTable();
            DataRow NewJctPSOCRow = JctPSOCTable.NewRow();
            NewJctPSOCRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.MaterialClass );
            NewJctPSOCRow["propertysetid"] = CswConvert.ToDbVal( MaterialPS.PropertySetId );
            JctPSOCTable.Rows.Add( NewJctPSOCRow );
            JctPSOCUpdate.update( JctPSOCTable );

            //Update jct_propertyset_ocprop
            CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_jct2_update", "jct_propertyset_ocprop" );
            DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in ObjectClass.getObjectClassProps() )
            {
                bool doInsert = ( ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.MaterialId ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.TradeName ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Supplier ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.PartNumber ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.PhysicalState ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.SpecificGravity ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.StorageCompatibility ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.ExpirationInterval ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Receive ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Request );
                if( doInsert )
                {
                    DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                    NewJctPSOCPRow["objectclasspropid"] = ObjectClassProp.PropId;
                    NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( MaterialPS.PropertySetId );
                    JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
                }
            }
            JctPSOCPUpdate.update( JctPSOCPTable );
        } // update()
    }//class CswUpdateSchema_02B_Case28690A
}//namespace ChemSW.Nbt.Schema