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
	/// Updates the schema to version 01H-52
	/// </summary>
	public class CswUpdateSchemaTo01H52 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 52 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H52( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{
			// case 22339

			// New Site nodetype

			CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
			CswNbtMetaDataNodeType SiteNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( LocationOC.ObjectClassId, "Site", "Locations" );
			SiteNT.NameTemplateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassLocation.NamePropertyName );
			SiteNT.IconFileName = "flag.gif";

			// Add permissions
			CswNbtObjClassRole AdministratorRoleNode = CswNbtNodeCaster.AsRole( _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "Administrator" ) );
			CswNbtObjClassRole ChemSWAdministratorRoleNode = CswNbtNodeCaster.AsRole( _CswNbtSchemaModTrnsctn.Nodes.makeRoleNodeFromRoleName( "chemsw_admin_role" ) );

			_CswNbtSchemaModTrnsctn.Permit.set( new CswNbtPermit.NodeTypePermission[] { 
													CswNbtPermit.NodeTypePermission.Create,
													CswNbtPermit.NodeTypePermission.View,
													CswNbtPermit.NodeTypePermission.Edit,
													CswNbtPermit.NodeTypePermission.Delete },
												SiteNT,
												AdministratorRoleNode,
												true );

			_CswNbtSchemaModTrnsctn.Permit.set( new CswNbtPermit.NodeTypePermission[] { 
													CswNbtPermit.NodeTypePermission.Create,
													CswNbtPermit.NodeTypePermission.View,
													CswNbtPermit.NodeTypePermission.Edit,
													CswNbtPermit.NodeTypePermission.Delete },
												SiteNT,
												ChemSWAdministratorRoleNode,
												true );

			// Add a default site node: Site 1
			CswNbtNode SiteNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SiteNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
			CswNbtObjClassLocation SiteNodeAsSite = CswNbtNodeCaster.AsLocation( SiteNode );
			SiteNodeAsSite.Name.Text = "Site 1";
			SiteNode.postChanges( true );

			// Set all buildings to be in Site 1
			CswNbtMetaDataNodeType BuildingNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
			if( BuildingNT != null )
			{
				foreach( CswNbtNode BuildingNode in BuildingNT.getNodes( false, true ) )
				{
					CswNbtObjClassLocation BuildingNodeAsLocation = CswNbtNodeCaster.AsLocation( BuildingNode );
					BuildingNodeAsLocation.Location.SelectedNodeId = SiteNode.NodeId;
					BuildingNodeAsLocation.Location.RefreshNodeName();
					BuildingNode.postChanges( false );
				}
			}

			
			
			// update Location views
			
			CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Floor" );
			CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
			CswNbtMetaDataNodeType CabinetNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Cabinet" );
			CswNbtMetaDataNodeType ShelfNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Shelf" );
			CswNbtMetaDataNodeType BoxNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Box" );

			//Locations
			CswNbtView LocationsView = _CswNbtSchemaModTrnsctn.restoreView( "Locations" );
			if( LocationsView != null )
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


			//Equipment By Location
			CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
			if( EquipmentNT != null )
			{
				CswNbtMetaDataNodeTypeProp EquipLocationNTP = EquipmentNT.getNodeTypeProp( "Location" );

				CswNbtView EquipByLocView = _CswNbtSchemaModTrnsctn.restoreView( "Equipment By Location" );
				if( EquipByLocView != null )
				{
					EquipByLocView.Root.ChildRelationships.Clear();
					
					CswNbtViewRelationship SiteRel = _addLocationToView( EquipByLocView, null, SiteNT );
					CswNbtViewRelationship BuildingRel = _addLocationToView( EquipByLocView, SiteRel, BuildingNT );
					CswNbtViewRelationship FloorRel = _addLocationToView( EquipByLocView, BuildingRel, FloorNT );
					CswNbtViewRelationship RoomRel = _addLocationToView( EquipByLocView, FloorRel, RoomNT );
					CswNbtViewRelationship CabinetRel = _addLocationToView( EquipByLocView, RoomRel, CabinetNT );
					CswNbtViewRelationship ShelfRel = _addLocationToView( EquipByLocView, CabinetRel, ShelfNT );
					CswNbtViewRelationship BoxRel = _addLocationToView( EquipByLocView, ShelfRel, BoxNT );

					CswNbtViewRelationship RoomRel2 = _addLocationToView( EquipByLocView, BuildingRel, RoomNT );
					CswNbtViewRelationship CabinetRel2 = _addLocationToView( EquipByLocView, RoomRel2, CabinetNT );
					CswNbtViewRelationship ShelfRel2 = _addLocationToView( EquipByLocView, CabinetRel2, ShelfNT );
					CswNbtViewRelationship BoxRel2 = _addLocationToView( EquipByLocView, ShelfRel2, BoxNT );

					
					if( EquipLocationNTP != null )
					{
						EquipByLocView.AddViewRelationship( SiteRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( RoomRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( CabinetRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( ShelfRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( RoomRel2, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( CabinetRel2, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( ShelfRel2, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						EquipByLocView.AddViewRelationship( BoxRel2, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
					}
					
					EquipByLocView.save();
					
				} // if( EquipByLocView != null )

			} // if( EquipmentNT != null )


			//Inspection Points by Location
			CswNbtMetaDataNodeType InspectionPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString(CswSchemaUpdater.HamletNodeTypes.FE_Inspection_Point ));
			if( InspectionPointNT != null )
			{
				CswNbtMetaDataNodeTypeProp InspPointLocationNTP = InspectionPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );

				CswNbtView InspPointsByLocView = _CswNbtSchemaModTrnsctn.restoreView( "Inspection Points by Location" );
				if( InspPointsByLocView != null )
				{
					InspPointsByLocView.Root.ChildRelationships.Clear();
					
					CswNbtViewRelationship SiteRel = _addLocationToView( InspPointsByLocView, null, SiteNT );
					CswNbtViewRelationship BuildingRel = _addLocationToView( InspPointsByLocView, SiteRel, BuildingNT );
					CswNbtViewRelationship FloorRel = _addLocationToView( InspPointsByLocView, BuildingRel, FloorNT );
					CswNbtViewRelationship RoomRel = _addLocationToView( InspPointsByLocView, FloorRel, RoomNT );
					CswNbtViewRelationship CabinetRel = _addLocationToView( InspPointsByLocView, RoomRel, CabinetNT );
					CswNbtViewRelationship ShelfRel = _addLocationToView( InspPointsByLocView, CabinetRel, ShelfNT );
					CswNbtViewRelationship BoxRel = _addLocationToView( InspPointsByLocView, ShelfRel, BoxNT );

					CswNbtViewRelationship RoomRel2 = _addLocationToView( InspPointsByLocView, BuildingRel, RoomNT );
					CswNbtViewRelationship CabinetRel2 = _addLocationToView( InspPointsByLocView, RoomRel2, CabinetNT );
					CswNbtViewRelationship ShelfRel2 = _addLocationToView( InspPointsByLocView, CabinetRel2, ShelfNT );
					CswNbtViewRelationship BoxRel2 = _addLocationToView( InspPointsByLocView, ShelfRel2, BoxNT );

					
					if( InspPointLocationNTP != null )
					{
						InspPointsByLocView.AddViewRelationship( SiteRel, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( RoomRel, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( CabinetRel, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( ShelfRel, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( RoomRel2, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( CabinetRel2, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( ShelfRel2, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
						InspPointsByLocView.AddViewRelationship( BoxRel2, CswNbtViewRelationship.PropOwnerType.Second, InspPointLocationNTP, true );
					}
					
					InspPointsByLocView.save();
					
				} // if( InspPointsByLocView != null )

			} // if( InspectionPointNT != null )
	
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


	}//class CswUpdateSchemaTo01H52

}//namespace ChemSW.Nbt.Schema

