using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web.SessionState;
using System.Xml;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswGridData
	{
		private CswNbtResources _CswNbtResources;
		public CswGridData( CswNbtResources Resources )
		{
			_CswNbtResources = Resources;
			PageSize = _CswNbtResources.CurrentNbtUser.PageSize;
		}

		public string PkColumn = string.Empty;
		public bool HidePkColumn = true;
		public Int32 PageSize;

		public JObject DataTableToJSON( DataTable Data )
		{
			// Columns
			JArray JColumnNames = new JArray();
			JArray JColumnDefs = new JArray();
			foreach( DataColumn Column in Data.Columns )
			{
				bool IsPrimaryKey = false;
				foreach( DataColumn PkCol in Data.PrimaryKey )
				{
					if( PkCol == Column )
						IsPrimaryKey = true;
				}

				JColumnNames.Add( Column.ColumnName );
				JObject ThisColumnDef = new JObject();
				ThisColumnDef.Add( new JProperty( "name", Column.ColumnName ) );
				ThisColumnDef.Add( new JProperty( "index", Column.ColumnName ) );
				if( Column.ColumnName.ToLower() == PkColumn.ToLower() )
				{
					ThisColumnDef.Add( new JProperty( "key", "true" ) );
					// This is bugged...
					//if( HidePkColumn )
					//    ThisColumnDef.Add( new JProperty( "hidden", "true" ) );
				}
				JColumnDefs.Add( ThisColumnDef );
			} // foreach( DataColumn Column in Data.Columns )

			// Rows
			JArray JRows = new JArray();
			foreach( DataRow Row in Data.Rows )
			{
				JObject RowObj = new JObject();
				foreach( DataColumn Column in Data.Columns )
				{
					RowObj.Add( new JProperty( Column.ColumnName, Row[Column].ToString() ) );
				}
				JRows.Add( RowObj );
			} 

			return new JObject(
				new JProperty( "datatype", "local" ),
				new JProperty( "colNames", JColumnNames ),
				new JProperty( "colModel", JColumnDefs ),
				new JProperty( "data", JRows ),
				new JProperty( "rowNum", PageSize )//,
				//new JProperty( "rowList", new JArray( 10, 25, 50 ) ) 
				);

			//new JObject(
				//    new JProperty( "total", "1" ),
				//    new JProperty( "page", "1" ),
				//    new JProperty( "records", Data.Rows.Count ),
				//    new JProperty( "data", JRows ) ) ) );

		} // _mapDataTable()


	} // class CswGridData

} // namespace ChemSW.Nbt.WebServices
