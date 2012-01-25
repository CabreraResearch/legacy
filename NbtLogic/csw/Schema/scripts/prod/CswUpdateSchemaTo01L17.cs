using System;
using System.Data;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-17
    /// </summary>
    public class CswUpdateSchemaTo01L17 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 17 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            #region case 24587

            // The FKType and FKValue of all Location properties on Location nodetypes should be the Location object class in the master.
            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass);
            foreach(CswNbtMetaDataNodeType LocationNT in LocationOC.NodeTypes)
            {
                CswNbtMetaDataNodeTypeProp LocationProp = LocationNT.getNodeTypePropByObjectClassPropName(CswNbtObjClassLocation.LocationPropertyName);
                LocationProp.SetFK(CswNbtViewRelationship.RelatedIdType.ObjectClassId.ToString(), LocationOC.ObjectClassId);
            }
            
            #endregion case 24587

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


       


        }//Update()

    }//class CswUpdateSchemaTo01L17

}//namespace ChemSW.Nbt.Schema


