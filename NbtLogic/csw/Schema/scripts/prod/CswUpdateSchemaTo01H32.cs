using System;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-32
	/// </summary>
	public class CswUpdateSchemaTo01H32 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 32 ); } }
		public CswUpdateSchemaTo01H32( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// case 21624
			// Relationships in use with null fktarget
			CswNbtMetaDataObjectClass AssemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );

			CswNbtMetaDataNodeType VendorNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Vendor" );
			if( VendorNT != null )
			{
				foreach( CswNbtMetaDataNodeType AssemblyNT in AssemblyOC.NodeTypes )
				{
					CswNbtMetaDataNodeTypeProp AssemblyVendorNTP = AssemblyNT.getNodeTypeProp( "Assembly Vendor" );
					if( AssemblyVendorNTP != null )
					{
						if( AssemblyVendorNTP.FKValue == Int32.MinValue )
						{
							AssemblyVendorNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), VendorNT.NodeTypeId, string.Empty, Int32.MinValue );
						}
					}

					CswNbtMetaDataNodeTypeProp AssemblyServiceVendorNTP = AssemblyNT.getNodeTypeProp( "Assembly Service Vendor" );
					if( AssemblyServiceVendorNTP != null )
					{
						if( AssemblyServiceVendorNTP.FKValue == Int32.MinValue )
						{
							AssemblyServiceVendorNTP.SetFK( CswNbtViewRelationship.RelatedIdType.NodeTypeId.ToString(), VendorNT.NodeTypeId, string.Empty, Int32.MinValue );
						}
					}

				} // foreach( CswNbtMetaDataNodeType AssemblyNT in AssemblyOC.NodeTypes )
			} // if(VendorNT != null)

		} // update()

	}//class CswUpdateSchemaTo01H32

}//namespace ChemSW.Nbt.Schema

