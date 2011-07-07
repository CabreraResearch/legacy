using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.DB;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-57
	/// </summary>
	public class CswUpdateSchemaTo01H57 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;
		private CswProdUpdtRsrc _CswProdUpdtRsrc = null;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 57 ); } }
		public string Description { get { return ( _CswProdUpdtRsrc.makeTestCaseDescription( SchemaVersion ) ); } }

		public CswUpdateSchemaTo01H57( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
			_CswProdUpdtRsrc = new CswProdUpdtRsrc( _CswNbtSchemaModTrnsctn );
		}

		public void update()
		{

			// case 22603
			// Move 'Status' to 'Action' tab to clarify why Finished doesn't stay checked

			CswNbtMetaDataObjectClass InspectionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );

			Collection<CswNbtMetaDataNodeType> InspectionNTs = new Collection<CswNbtMetaDataNodeType>();
			foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.NodeTypes )
			{
                if( InspectionNT.IsLatestVersion )
                {
                    InspectionNTs.Add( InspectionNT );
                }
			}

			foreach( CswNbtMetaDataNodeType InspectionNT in InspectionNTs )
			{
				CswNbtMetaDataNodeTypeProp StatusNTP = InspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.StatusPropertyName );
				CswNbtMetaDataNodeTypeProp FinishedNTP = InspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.FinishedPropertyName );
				CswNbtMetaDataNodeTypeProp CancelledNTP = InspectionNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionDesign.CancelledPropertyName );

				CswNbtMetaDataNodeTypeTab ActionTab = InspectionNT.getNodeTypeTab( "Action" );
				if( ActionTab != null )
				{
					StatusNTP.NodeTypeTab = ActionTab;
					StatusNTP.DisplayRow = 1;
					StatusNTP.DisplayColumn = 1;

					FinishedNTP.DisplayRow = 2;
					FinishedNTP.DisplayColumn = 1;

					CancelledNTP.DisplayRow = 3;
					CancelledNTP.DisplayColumn = 1;
				} // if( ActionTab != null )
			} // foreach( CswNbtMetaDataNodeType InspectionNT in InspectionOC.NodeTypes )


			// case 22591
			// Insert empty records in jct_nodes_props for all node properties
			foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )
			{
				foreach( CswNbtNode Node in NodeType.getNodes( true, true ) )
				{
					foreach( CswNbtNodePropWrapper Prop in Node.Properties )
					{
						Prop.makePropRow();
					}
					Node.postChanges( true );

				} // foreach(CswNbtNode Node in NodeType.getNodes(true, true)
			} // foreach( CswNbtMetaDataNodeType NodeType in _CswNbtSchemaModTrnsctn.MetaData.NodeTypes )

		} // update()


	}//class CswUpdateSchemaTo01H57

}//namespace ChemSW.Nbt.Schema

