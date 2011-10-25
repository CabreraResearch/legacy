using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01J-09
	/// </summary>
	public class CswUpdateSchemaTo01J09 : CswUpdateSchemaTo
	{
		public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 09 ); } }
		public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

		public override void update()
		{
			// case 23996
			// Set the 'My Tasks' view to use assembly tasks as well
			CswNbtMetaDataObjectClass TaskOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.TaskClass );
			if( TaskOC.NodeTypes.Count > 0 )
			{
				List<CswNbtView> MyTasksViews = _CswNbtSchemaModTrnsctn.restoreViews( "My Tasks" );
				foreach( CswNbtView MyTasksView in MyTasksViews )
				{
					if( MyTasksView.Visibility == NbtViewVisibility.Global )
					{
						MyTasksView.Root.ChildRelationships.Clear();

						foreach( CswNbtMetaDataNodeType TaskNT in TaskOC.NodeTypes )
						{
							// Relationship
							CswNbtViewRelationship TaskRel = MyTasksView.AddViewRelationship( TaskNT, true );

							// Properties
							CswNbtViewProperty DueDateViewProp = MyTasksView.AddViewProperty( TaskRel, TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.DueDatePropertyName ) );
							CswNbtMetaDataNodeTypeProp CompletedNTP = TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.CompletedPropertyName );
							CswNbtViewProperty CompletedViewProp = MyTasksView.AddViewProperty( TaskRel, CompletedNTP );
							CswNbtViewProperty SummaryViewProp = MyTasksView.AddViewProperty( TaskRel, TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.SummaryPropertyName ) );
							CswNbtViewProperty OwnerViewProp = MyTasksView.AddViewProperty( TaskRel, TaskNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassTask.OwnerPropertyName ) );
							DueDateViewProp.Order = 1;
							CompletedViewProp.Order = 2;
							SummaryViewProp.Order = 3;
							OwnerViewProp.Order = 4;

							CswNbtMetaDataNodeTypeProp LocationNTP = TaskNT.getNodeTypeProp( "Location" );
							if( LocationNTP != null )
							{
								CswNbtViewProperty LocationViewProp = MyTasksView.AddViewProperty( TaskRel, LocationNTP );
								LocationViewProp.Order = 5;
							}
							CswNbtMetaDataNodeTypeProp TechnicianNTP = TaskNT.getNodeTypeProp( "Technician" );
							CswNbtViewProperty TechnicianViewProp = null;
							if( LocationNTP != null )
							{
								TechnicianViewProp = MyTasksView.AddViewProperty( TaskRel, TechnicianNTP );
								TechnicianViewProp.Order = 6;
							}

							// Filters
							MyTasksView.AddViewPropertyFilter( CompletedViewProp, CompletedNTP.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, Tristate.False.ToString(), false );
							if( TechnicianNTP != null )
							{
								MyTasksView.AddViewPropertyFilter( TechnicianViewProp, TechnicianNTP.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, "me", false );
							}
						} // foreach( CswNbtMetaDataNodeType TaskNT in TaskOC.NodeTypes )
						MyTasksView.save();

					} // if( MyTasksView.Visibility == NbtViewVisibility.Global )
				} // foreach( CswNbtView MyTasksView in MyTasksViews )
			} // if( TaskOC.NodeTypes.Count > 0 )

		}//Update()

	}//class CswUpdateSchemaTo01J09

}//namespace ChemSW.Nbt.Schema


