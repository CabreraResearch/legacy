using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-53
	/// </summary>
	public class CswUpdateSchemaTo01H53 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 53 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H53( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{
			// case 22339
			// this failed in 01H-52
			// update Location views
			CswNbtMetaDataNodeType SiteNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Site" );
			CswNbtMetaDataNodeType BuildingNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
			CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Floor" );
			CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
			CswNbtMetaDataNodeType CabinetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Cabinet" );
			CswNbtMetaDataNodeType ShelfNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Shelf" );
			CswNbtMetaDataNodeType BoxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );

			//Locations
			foreach( CswNbtView LocationsView in _CswNbtSchemaModTrnsctn.restoreViews( "Locations" ) )
			{
				LocationsView.Root.ChildRelationships.Clear();

				CswNbtViewRelationship SiteRel = _addLocationToView( LocationsView, null, SiteNT );
				CswNbtViewRelationship BuildingRel = _addLocationToView( LocationsView, SiteRel, BuildingNT );
				CswNbtViewRelationship FloorRel = _addLocationToView( LocationsView, BuildingRel, FloorNT );
				CswNbtViewRelationship RoomRel = _addLocationToView( LocationsView, FloorRel, RoomNT );
				CswNbtViewRelationship CabinetRel = _addLocationToView( LocationsView, RoomRel, CabinetNT );
				CswNbtViewRelationship ShelfRel = _addLocationToView( LocationsView, CabinetRel, ShelfNT );
				CswNbtViewRelationship BoxRel = _addLocationToView( LocationsView, ShelfRel, BoxNT );

				CswNbtViewRelationship RoomRel2 = _addLocationToView( LocationsView, BuildingRel, RoomNT );
				CswNbtViewRelationship CabinetRel2 = _addLocationToView( LocationsView, RoomRel2, CabinetNT );
				CswNbtViewRelationship ShelfRel2 = _addLocationToView( LocationsView, CabinetRel2, ShelfNT );
				CswNbtViewRelationship BoxRel2 = _addLocationToView( LocationsView, ShelfRel2, BoxNT );

				LocationsView.save();
			} // if( LocationsView != null )

			// fix category of 'Inspection Points by Location' view

			CswNbtView InspPointsByLocView = _CswNbtSchemaModTrnsctn.restoreView( "Inspection Points by Location" );
			if( InspPointsByLocView != null )
			{
				InspPointsByLocView.Category = "Inspections";
				InspPointsByLocView.save();
			}


			// case 22176

			//New Inspection Status action
			_CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Inspection_Status, true, "", "Inspections" );

			// Add permissions
			CswNbtObjClassRole AdministratorRoleNode = CswNbtNodeCaster.AsRole( _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" ) );
			CswNbtObjClassRole ChemSWAdministratorRoleNode = CswNbtNodeCaster.AsRole( _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "chemsw_admin_role" ) );
			_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Inspection_Status, AdministratorRoleNode, true );
			_CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Inspection_Status, ChemSWAdministratorRoleNode, true );

		} // update()



		private CswNbtViewRelationship _addLocationToView( CswNbtView View, CswNbtViewRelationship ParentRel, CswNbtMetaDataNodeType LocationNodeType )
		{
			CswNbtViewRelationship Rel = null;
			if( LocationNodeType != null )
			{
				CswNbtMetaDataNodeTypeProp LocationNTP = LocationNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
				CswNbtMetaDataNodeTypeProp NameNTP = LocationNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
				CswNbtMetaDataNodeTypeProp BarcodeNTP = LocationNodeType.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );

				if( ParentRel == null )
				{
					Rel = View.AddViewRelationship( LocationNodeType, true );
				}
				else
				{
					Rel = View.AddViewRelationship( ParentRel, CswNbtViewRelationship.PropOwnerType.Second, LocationNTP, true );
				}

				if( NameNTP != null )
				{
					CswNbtViewProperty NameViewProp = View.AddViewProperty( Rel, NameNTP );
					View.AddViewPropertyFilter( NameViewProp,
												NameNTP.FieldTypeRule.SubFields.Default.Name,
												NameNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode,
												string.Empty,
												false );
				}
				if( BarcodeNTP != null )
				{
					CswNbtViewProperty BarcodeViewProp = View.AddViewProperty( Rel, BarcodeNTP );
					View.AddViewPropertyFilter( BarcodeViewProp,
												BarcodeNTP.FieldTypeRule.SubFields.Default.Name,
												BarcodeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode,
												string.Empty,
												false );
				}
			} // if( LocationNodetype != null )
			return Rel;
		} // _addLocationToView()

	}//class CswUpdateSchemaTo01H53

}//namespace ChemSW.Nbt.Schema

