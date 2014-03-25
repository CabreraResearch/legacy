using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02L_Case52990: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 52990; }
        }

        public override string Title
        {
            get { return "Add Expiration props to Supply and Biological"; }
        }

        public override void update()
        {
           CswNbtMetaDataObjectClass BiologicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BiologicalClass );
           CswNbtMetaDataObjectClass SupplyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );
            CswNbtMetaDataNodeType TimeUoM_OC = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit Time" );

            CswTableUpdate JctPropSetOCP = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "52990_JCT_PS_OCP", "jct_propertyset_ocprop" );
            DataTable JctTable = JctPropSetOCP.getEmptyTable();

            foreach( CswNbtMetaDataObjectClass MaterialOC in new[] {BiologicalOC, SupplyOC} )
            {

                CswNbtMetaDataObjectClassProp ExpireInterval = _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtPropertySetMaterial.PropertyName.ExpirationInterval,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsFk = true,
                    FkType = "NodeTypeId",
                    FkValue = TimeUoM_OC.NodeTypeId
                } );

                CswNbtMetaDataObjectClassProp OpenExpireInterval = _CswNbtSchemaModTrnsctn.createObjectClassProp( MaterialOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                {
                    PropName = CswNbtPropertySetMaterial.PropertyName.OpenExpireInterval,
                    FieldType = CswEnumNbtFieldType.Quantity,
                    IsFk = true,
                    FkType = "NodeTypeId",
                    FkValue = TimeUoM_OC.NodeTypeId
                } );

                DataRow Row = JctTable.NewRow();
                Row["propertysetid"] = MaterialOC.getPropertySet().PropertySetId;
                Row["objectclasspropid"] = ExpireInterval.ObjectClassPropId;
                JctTable.Rows.Add( Row );

                Row = JctTable.NewRow();
                Row["propertysetid"] = MaterialOC.getPropertySet().PropertySetId;
                Row["objectclasspropid"] = OpenExpireInterval.ObjectClassPropId;
                JctTable.Rows.Add( Row );
            }

            JctPropSetOCP.update( JctTable );
        } // update()
        
    }

}//namespace ChemSW.Nbt.Schema