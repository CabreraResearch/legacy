using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Audit;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-41
	/// </summary>
	public class CswUpdateSchemaTo01H41 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 41 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H41( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{
			CswNbtMetaDataNodeType EquipmentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Equipment" );
			if( EquipmentNT != null )
			{

				// case 21215
				CswNbtMetaDataNodeTypeProp EquipmentIdNTP = EquipmentNT.getNodeTypeProp( "Equipment Id" );
				CswNbtMetaDataNodeTypeProp TypeNTP = EquipmentNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassEquipment.TypePropertyName );
				CswNbtMetaDataNodeTypeProp ManufacturerNTP = EquipmentNT.getNodeTypeProp( "Manufacturer" );
				CswNbtMetaDataNodeTypeProp PropertyNoNTP = EquipmentNT.getNodeTypeProp( "Property No" );
				CswNbtMetaDataNodeTypeProp SerialNoNTP = EquipmentNT.getNodeTypeProp( "Serial No" );
				CswNbtMetaDataNodeTypeProp ModelNTP = EquipmentNT.getNodeTypeProp( "Model" );
				CswNbtMetaDataNodeTypeProp LocationNTP = EquipmentNT.getNodeTypeProp( "Location" );
				CswNbtMetaDataNodeTypeProp DepartmentNTP = EquipmentNT.getNodeTypeProp( "Department" );
				CswNbtMetaDataNodeTypeProp ResponsibleNTP = EquipmentNT.getNodeTypeProp( "Responsible" );

				CswNbtMetaDataNodeTypeProp EquipLocationNTP = EquipmentNT.getNodeTypeProp( "Location" );

				CswNbtView FindEquipmentView = _CswNbtSchemaModTrnsctn.restoreView( "Find Equipment" );
				if( FindEquipmentView != null )
				{
					FindEquipmentView.Root.ChildRelationships.Clear();

					CswNbtViewRelationship EquipRel = FindEquipmentView.AddViewRelationship( EquipmentNT, true );
					if( EquipmentIdNTP != null )
					{
						CswNbtViewProperty EquipmentIdViewProp = FindEquipmentView.AddViewProperty( EquipRel, EquipmentIdNTP );
						FindEquipmentView.AddViewPropertyFilter( EquipmentIdViewProp, EquipmentIdNTP.FieldTypeRule.SubFields.Default.Name, EquipmentIdNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( TypeNTP != null )
					{
						CswNbtViewProperty TypeViewProp = FindEquipmentView.AddViewProperty( EquipRel, TypeNTP );
						FindEquipmentView.AddViewPropertyFilter( TypeViewProp, TypeNTP.FieldTypeRule.SubFields.Default.Name, TypeNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( ManufacturerNTP != null )
					{
						CswNbtViewProperty ManufacturerViewProp = FindEquipmentView.AddViewProperty( EquipRel, ManufacturerNTP );
						FindEquipmentView.AddViewPropertyFilter( ManufacturerViewProp, ManufacturerNTP.FieldTypeRule.SubFields.Default.Name, ManufacturerNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( PropertyNoNTP != null )
					{
						CswNbtViewProperty PropertyNoViewProp = FindEquipmentView.AddViewProperty( EquipRel, PropertyNoNTP );
						FindEquipmentView.AddViewPropertyFilter( PropertyNoViewProp, PropertyNoNTP.FieldTypeRule.SubFields.Default.Name, PropertyNoNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( SerialNoNTP != null )
					{
						CswNbtViewProperty SerialNoViewProp = FindEquipmentView.AddViewProperty( EquipRel, SerialNoNTP );
						FindEquipmentView.AddViewPropertyFilter( SerialNoViewProp, SerialNoNTP.FieldTypeRule.SubFields.Default.Name, SerialNoNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( ModelNTP != null )
					{
						CswNbtViewProperty ModelViewProp = FindEquipmentView.AddViewProperty( EquipRel, ModelNTP );
						FindEquipmentView.AddViewPropertyFilter( ModelViewProp, ModelNTP.FieldTypeRule.SubFields.Default.Name, ModelNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( LocationNTP != null )
					{
						CswNbtViewProperty LocationViewProp = FindEquipmentView.AddViewProperty( EquipRel, LocationNTP );
						FindEquipmentView.AddViewPropertyFilter( LocationViewProp, LocationNTP.FieldTypeRule.SubFields.Default.Name, LocationNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( DepartmentNTP != null )
					{
						CswNbtViewProperty DepartmentViewProp = FindEquipmentView.AddViewProperty( EquipRel, DepartmentNTP );
						FindEquipmentView.AddViewPropertyFilter( DepartmentViewProp, DepartmentNTP.FieldTypeRule.SubFields.Default.Name, DepartmentNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					if( ResponsibleNTP != null )
					{
						CswNbtViewProperty ResponsibleViewProp = FindEquipmentView.AddViewProperty( EquipRel, ResponsibleNTP );
						FindEquipmentView.AddViewPropertyFilter( ResponsibleViewProp, ResponsibleNTP.FieldTypeRule.SubFields.Default.Name, ResponsibleNTP.FieldTypeRule.SubFields.Default.DefaultFilterMode, string.Empty, false );
					}
					FindEquipmentView.save();

				} // if( FindEquipmentView != null )

				// case 21216
				CswNbtMetaDataNodeType BuildingNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Building" );
				if( BuildingNT != null )
				{
					CswNbtView EquipByLocView = _CswNbtSchemaModTrnsctn.makeView();
					EquipByLocView.makeNew( "Equipment By Location", NbtViewVisibility.Global, null, null, null );
					EquipByLocView.Category = "Equipment";

					//CswNbtMetaDataNodeTypeProp BuildingLocationNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
					CswNbtMetaDataNodeTypeProp BuildingNameNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
					CswNbtMetaDataNodeTypeProp BuildingBarcodeNTP = BuildingNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );

					CswNbtViewRelationship BuildingRel = EquipByLocView.AddViewRelationship( BuildingNT, true );
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

					CswNbtMetaDataNodeType FloorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Floor" );
					CswNbtViewRelationship FloorRel = null;
					if( FloorNT != null )
					{
						CswNbtMetaDataNodeTypeProp FloorLocationNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
						CswNbtMetaDataNodeTypeProp FloorNameNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
						CswNbtMetaDataNodeTypeProp FloorBarcodeNTP = FloorNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );

						FloorRel = EquipByLocView.AddViewRelationship( BuildingRel, CswNbtViewRelationship.PropOwnerType.Second, FloorLocationNTP, true );
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
					} // if( FloorNT != null )

					CswNbtMetaDataNodeType RoomNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Room" );
					if( RoomNT != null )
					{
						CswNbtMetaDataNodeTypeProp RoomLocationNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.LocationPropertyName );
						CswNbtMetaDataNodeTypeProp RoomNameNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.NamePropertyName );
						CswNbtMetaDataNodeTypeProp RoomBarcodeNTP = RoomNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassLocation.BarcodePropertyName );

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

						if( FloorRel != null )
						{
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
						}
					} // if( RoomNT != null )

					EquipByLocView.save();
				} // if( BuildingNT != null )

			} // if( EquipmentNT != null)
		} // update()

	}//class CswUpdateSchemaTo01H41

}//namespace ChemSW.Nbt.Schema

