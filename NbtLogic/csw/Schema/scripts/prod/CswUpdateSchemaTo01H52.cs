using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
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

			// Add a default site node: Site 1
			CswNbtNode SiteNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( SiteNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.JustSetPk );
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
					BuildingNode.postChanges( false );
				}
			}

			// update Location views
			CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Floor" );
			CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
			if( SiteNT != null && BuildingNT != null && RoomNT != null && FloorNT != null )
			{
				CswNbtMetaDataNodeTypeProp SiteLocationNTP = SiteNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
				CswNbtMetaDataNodeTypeProp SiteNameNTP = SiteNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
				CswNbtMetaDataNodeTypeProp SiteBarcodeNTP = SiteNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );
				CswNbtMetaDataNodeTypeProp BuildingLocationNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
				CswNbtMetaDataNodeTypeProp BuildingNameNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
				CswNbtMetaDataNodeTypeProp BuildingBarcodeNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );
				CswNbtMetaDataNodeTypeProp FloorLocationNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
				CswNbtMetaDataNodeTypeProp FloorNameNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
				CswNbtMetaDataNodeTypeProp FloorBarcodeNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );
				CswNbtMetaDataNodeTypeProp RoomLocationNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
				CswNbtMetaDataNodeTypeProp RoomNameNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
				CswNbtMetaDataNodeTypeProp RoomBarcodeNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );

				//Equipment By Location
				CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
				if( EquipmentNT != null )
				{
					CswNbtMetaDataNodeTypeProp EquipLocationNTP = EquipmentNT.getNodeTypeProp( "Location" );

					CswNbtView EquipByLocView = _CswNbtSchemaModTrnsctn.restoreView( "Equipment By Location" );
					if( EquipByLocView != null )
					{
						EquipByLocView.Root.ChildRelationships.Clear();

						CswNbtViewRelationship SiteRel = EquipByLocView.AddViewRelationship( SiteNT, true );
						if( SiteNameNTP != null )
						{
							CswNbtViewProperty SiteNameViewProp = EquipByLocView.AddViewProperty( SiteRel, SiteNameNTP );
							EquipByLocView.AddViewPropertyFilter( SiteNameViewProp, SiteNameNTP.FieldTypeRule.SubFields.Default.Name, SiteNameNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( SiteBarcodeNTP != null )
						{
							CswNbtViewProperty SiteBarcodeViewProp = EquipByLocView.AddViewProperty( SiteRel, SiteBarcodeNTP );
							EquipByLocView.AddViewPropertyFilter( SiteBarcodeViewProp, SiteBarcodeNTP.FieldTypeRule.SubFields.Default.Name, SiteBarcodeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( EquipLocationNTP != null )
						{
							EquipByLocView.AddViewRelationship( SiteRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						}

						CswNbtViewRelationship BuildingRel = EquipByLocView.AddViewRelationship( SiteRel, CswNbtViewRelationship.PropOwnerType.Second, BuildingLocationNTP, true );
						if( BuildingNameNTP != null )
						{
							CswNbtViewProperty BuildingNameViewProp = EquipByLocView.AddViewProperty( BuildingRel, BuildingNameNTP );
							EquipByLocView.AddViewPropertyFilter( BuildingNameViewProp, BuildingNameNTP.FieldTypeRule.SubFields.Default.Name, BuildingNameNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( BuildingBarcodeNTP != null )
						{
							CswNbtViewProperty BuildingBarcodeViewProp = EquipByLocView.AddViewProperty( BuildingRel, BuildingBarcodeNTP );
							EquipByLocView.AddViewPropertyFilter( BuildingBarcodeViewProp, BuildingBarcodeNTP.FieldTypeRule.SubFields.Default.Name, BuildingBarcodeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( EquipLocationNTP != null )
						{
							EquipByLocView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						}

						CswNbtViewRelationship FloorRel = EquipByLocView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, FloorLocationNTP, true );
						if( FloorNameNTP != null )
						{
							CswNbtViewProperty FloorNameViewProp = EquipByLocView.AddViewProperty( FloorRel, FloorNameNTP );
							EquipByLocView.AddViewPropertyFilter( FloorNameViewProp, FloorNameNTP.FieldTypeRule.SubFields.Default.Name, FloorNameNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( FloorBarcodeNTP != null )
						{
							CswNbtViewProperty FloorBarcodeViewProp = EquipByLocView.AddViewProperty( FloorRel, FloorBarcodeNTP );
							EquipByLocView.AddViewPropertyFilter( FloorBarcodeViewProp, FloorBarcodeNTP.FieldTypeRule.SubFields.Default.Name, FloorBarcodeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( EquipLocationNTP != null )
						{
							EquipByLocView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						}

						CswNbtViewRelationship RoomRel1 = EquipByLocView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, true );
						if( RoomNameNTP != null )
						{
							CswNbtViewProperty RoomNameViewProp1 = EquipByLocView.AddViewProperty( RoomRel1, RoomNameNTP );
							EquipByLocView.AddViewPropertyFilter( RoomNameViewProp1, RoomNameNTP.FieldTypeRule.SubFields.Default.Name, RoomNameNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( RoomBarcodeNTP != null )
						{
							CswNbtViewProperty RoomBarcodeViewProp1 = EquipByLocView.AddViewProperty( RoomRel1, RoomBarcodeNTP );
							EquipByLocView.AddViewPropertyFilter( RoomBarcodeViewProp1, RoomBarcodeNTP.FieldTypeRule.SubFields.Default.Name, RoomBarcodeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( EquipLocationNTP != null )
						{
							EquipByLocView.AddViewRelationship( RoomRel1, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						}

						CswNbtViewRelationship RoomRel2 = EquipByLocView.AddViewRelationship( FloorRel, CswNbtViewRelationship.PropOwnerType.Second, RoomLocationNTP, true );
						if( RoomNameNTP != null )
						{
							CswNbtViewProperty RoomNameViewProp2 = EquipByLocView.AddViewProperty( RoomRel2, RoomNameNTP );
							EquipByLocView.AddViewPropertyFilter( RoomNameViewProp2, RoomNameNTP.FieldTypeRule.SubFields.Default.Name, RoomNameNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( RoomBarcodeNTP != null )
						{
							CswNbtViewProperty RoomBarcodeViewProp2 = EquipByLocView.AddViewProperty( RoomRel2, RoomBarcodeNTP );
							EquipByLocView.AddViewPropertyFilter( RoomBarcodeViewProp2, RoomBarcodeNTP.FieldTypeRule.SubFields.Default.Name, RoomBarcodeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
						}
						if( EquipLocationNTP != null )
						{
							EquipByLocView.AddViewRelationship( RoomRel2, CswNbtViewRelationship.PropOwnerType.Second, EquipLocationNTP, true );
						}

						EquipByLocView.save();
					} // if( EquipByLocView != null )
				} // if( EquipmentNT != null )
			} // if( SiteNT != null && BuildingNT != null && RoomNT != null && FloorNT != null )

			//Locations

			//Inspection Points by Location


		} // update()

	}//class CswUpdateSchemaTo01H52

}//namespace ChemSW.Nbt.Schema

