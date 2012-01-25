using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-18
    /// </summary>
    public class CswUpdateSchemaTo01L18 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 18 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region Case 24786

            CswTableUpdate TableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( SchemaVersion.ToString() + "_jct_nodes_props_update", "jct_nodes_props" );
            string UpdateWhere = @"where jctnodepropid in ( 
                                        select n.defaultvalueid from nodetype_props n 
                                        join field_types f on n.fieldtypeid=f.fieldtypeid
                                        where lower(f.fieldtype)='question' 
                                  )";
            DataTable UpdateTable = TableUpdate.getTable( UpdateWhere, false );

            foreach( DataRow Row in UpdateTable.Rows )
            {
                Row["field1_date"] = DBNull.Value;
            }
            TableUpdate.update( UpdateTable );


            #endregion Case 24786

            #region Case 24806

            CswNbtMetaDataObjectClass CustomerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.CustomerClass );

            CswCommaDelimitedString UniqueCustomers = new CswCommaDelimitedString();
            foreach( CswNbtNode CustomerNode in CustomerOc.getNodes( true, false ) )
            {
                CswNbtObjClassCustomer NodeAsCustomer = CswNbtNodeCaster.AsCustomer( CustomerNode );
                string CustomerId = NodeAsCustomer.CompanyID.Text.Trim().ToLower();
                if( false == UniqueCustomers.Contains( CustomerId, false ) )
                {
                    UniqueCustomers.Add( CustomerId );
                }
                else
                {
                    CustomerNode.delete();
                }
            }

            CswNbtMetaDataObjectClassProp CustomerIdOcp = CustomerOc.getObjectClassProp( CswNbtObjClassCustomer.CompanyIDPropertyName );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerIdOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isunique, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerIdOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CustomerIdOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            #endregion Case 24806

        }//Update()

    }//class CswUpdateSchemaTo01L18

}//namespace ChemSW.Nbt.Schema


