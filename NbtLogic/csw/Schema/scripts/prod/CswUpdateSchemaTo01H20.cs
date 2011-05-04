using System;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;
using System.IO;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-20
	/// </summary>
	public class CswUpdateSchemaTo01H20 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
        private CswProdUpdtRsrc _CswProdUpdtRsrc = null; 

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 20 ); } }

        public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

        public CswUpdateSchemaTo01H20( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
        }

		public void update()
		{
			CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
			foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.NodeTypes )
			{
				CswNbtMetaDataNodeType LatestVersionInspectionNT = InspectionNT.LatestVersionNodeType;
				if( !LatestVersionInspectionNT.IsLocked )
				{
					// case 20951
					CswNbtMetaDataNodeTypeTab ActionTab = LatestVersionInspectionNT.getNodeTypeTab( "Action" );
					if( ActionTab == null )
					{
                                               ActionTab = _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( LatestVersionInspectionNT, "Action", 9 );
					}

					CswNbtMetaDataNodeTypeProp FinishedProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
					FinishedProp.NodeTypeTab = ActionTab;
					FinishedProp.DisplayRow = 1;
					FinishedProp.DisplayColumn = 1;

					CswNbtMetaDataNodeTypeProp CancelledProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelledPropertyName );
					CancelledProp.NodeTypeTab = ActionTab;
					CancelledProp.DisplayRow = 2;
					CancelledProp.DisplayColumn = 1;

					CswNbtMetaDataNodeTypeProp CancelReasonProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelReasonPropertyName );
					CancelReasonProp.NodeTypeTab = ActionTab;
					CancelReasonProp.DisplayRow = 3;  // even though webapp interprets this independently, Mobile needs this to be 3
					CancelReasonProp.DisplayColumn = 1;


					// case 20955
					CswNbtMetaDataNodeTypeProp GeneratorProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.GeneratorPropertyName );
					CswNbtMetaDataNodeTypeProp IsFutureProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.IsFuturePropertyName );
					CswNbtMetaDataNodeTypeProp RouteProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( "Route" );
					CswNbtMetaDataNodeTypeProp RouteOrderProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( "Route Order" );

					GeneratorProp.HideInMobile = true;
					IsFutureProp.HideInMobile = true;
					RouteProp.HideInMobile = true;
					RouteOrderProp.HideInMobile = true;

					// case 20948
					CswNbtMetaDataNodeTypeProp DueDateProp = LatestVersionInspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.DatePropertyName );
					DueDateProp.DisplayRow = 4;
					
					// case 20956
					var PiSetupTab = LatestVersionInspectionNT.getNodeTypeTab( "Setup" );
					if( null != PiSetupTab )
					{
					    PiSetupTab.TabName = "Details";
					    PiSetupTab.TabOrder = 10;
					}

				} // if( !InspectionNT.IsLocked )
			} // foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.NodeTypes )



		} // update()

	}//class CswUpdateSchemaTo01H20

}//namespace ChemSW.Nbt.Schema


