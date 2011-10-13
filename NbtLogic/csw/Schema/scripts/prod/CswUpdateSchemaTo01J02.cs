using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Actions;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 02J-02
    /// </summary>
    public class CswUpdateSchemaTo01J02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 02 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 20970 - Add 'locked' column to nodes
			// These are in 01J-01 as well
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodes", "locked" ) )
			{
				_CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes", "locked", "Prevents access to a node, for nodes beyond subscription level", false, false );
			}
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "nodes_audit", "locked" ) )
			{
				_CswNbtSchemaModTrnsctn.addBooleanColumn( "nodes_audit", "locked", "Prevents access to a node, for nodes beyond subscription level", false, false );
			}

			// case 20970 - Set 'locked' to false for all current nodes
			CswTableUpdate NodesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01J02_Nodes_Update", "nodes" );
			DataTable NodesTable = NodesUpdate.getTable();
			foreach( DataRow NodesRow in NodesTable.Rows )
			{
				NodesRow["locked"] = CswConvert.ToDbVal( false );
			}
			NodesUpdate.update( NodesTable );

			//CswTableUpdate NodesAuditUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01J02_NodesAudit_Update", "nodes_audit" );
			//DataTable NodesAuditTable = NodesAuditUpdate.getTable();
			//foreach( DataRow NodesAuditRow in NodesAuditTable.Rows )
			//{
			//    NodesAuditRow["locked"] = CswConvert.ToDbVal( false );
			//}
			//NodesAuditUpdate.update( NodesAuditTable );


			// case 20970 - Add 'quota' column to object_class
			// This is in 01J-01 as well
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class", "quota" ) )
			{
				_CswNbtSchemaModTrnsctn.addLongColumn( "object_class", "quota", "Sets the subscription count for nodes of this object class", false, false );
			}
			if( false == _CswNbtSchemaModTrnsctn.isColumnDefinedInDataBase( "object_class_audit", "quota" ) )
			{
				_CswNbtSchemaModTrnsctn.addLongColumn( "object_class_audit", "quota", "Sets the subscription count for nodes of this object class", false, false );
			}

			// case 20970 - Add an action for viewing/editing object class quotas
			// permissions are granted in next script
			_CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Quotas, true, "", "System" );

		}//Update()

    }//class CswUpdateSchemaTo01J02

}//namespace ChemSW.Nbt.Schema


