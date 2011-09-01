using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01I-02
    /// </summary>
    public class CswUpdateSchemaTo01I02 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'I', 02 ); } }

		public override void update()
        {

			CswTableUpdate OCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I-02_OCP_Update", "object_class_props" );

			// case 7728
			// Change Date and Time to DateTime
			// This is also in script 01I-08
			CswTableUpdate FieldTypesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I-02_FT_Update", "field_types" );
			DataTable FTTable = FieldTypesUpdate.getTable( "where fieldtype = 'Date' or fieldtype = 'Time'" );
			if( FTTable.Rows.Count > 0 )
			{
				Int32 TimeFTId = Int32.MinValue;
				Int32 DateFTId = Int32.MinValue;
				foreach( DataRow FTRow in FTTable.Rows )
				{
					if( FTRow["fieldtype"].ToString() == "Date" )
					{
						DateFTId = CswConvert.ToInt32( FTRow["fieldtypeid"] );
						FTRow["fieldtype"] = CswNbtMetaDataFieldType.NbtFieldType.DateTime.ToString();
					}
					if( FTRow["fieldtype"].ToString() == "Time" )
					{
						TimeFTId = CswConvert.ToInt32( FTRow["fieldtypeid"] );
						FTRow.Delete();
					}
				} // foreach( DataRow FTRow in FTTable.Rows )

				// update props first
				if( TimeFTId != Int32.MinValue && DateFTId != Int32.MinValue )
				{
					CswTableUpdate NTPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I-02_NTP_Update", "nodetype_props" );

					// date
					DataTable DateOCPTable = OCPUpdate.getTable( "where fieldtypeid = " + DateFTId.ToString() );
					foreach( DataRow DateOCPRow in DateOCPTable.Rows )
					{
						DateOCPRow["extended"] = CswNbtNodePropDateTime.DateDisplayMode.Date.ToString();
					}
					OCPUpdate.update( DateOCPTable );

					DataTable DateNTPTable = NTPUpdate.getTable( "where fieldtypeid = " + DateFTId.ToString() );
					foreach( DataRow DateNTPRow in DateNTPTable.Rows )
					{
						DateNTPRow["extended"] = CswNbtNodePropDateTime.DateDisplayMode.Date.ToString();
					}
					NTPUpdate.update( DateNTPTable );
		
					// time
					DataTable TimeOCPTable = OCPUpdate.getTable( "where fieldtypeid = " + TimeFTId.ToString() );
					foreach( DataRow TimeOCPRow in TimeOCPTable.Rows )
					{
						TimeOCPRow["fieldtypeid"] = CswConvert.ToDbVal( DateFTId );
						TimeOCPRow["extended"] = CswNbtNodePropDateTime.DateDisplayMode.Time.ToString();
					}
					OCPUpdate.update( TimeOCPTable );

					DataTable TimeNTPTable = NTPUpdate.getTable( "where fieldtypeid = " + TimeFTId.ToString() );
					foreach( DataRow TimeNTPRow in TimeNTPTable.Rows )
					{
						TimeNTPRow["fieldtypeid"] = CswConvert.ToDbVal( DateFTId );
						TimeNTPRow["extended"] = CswNbtNodePropDateTime.DateDisplayMode.Time.ToString();
					}
					NTPUpdate.update( TimeNTPTable );
				} // if( TimeFTId != Int32.MinValue && DateFTId != Int32.MinValue )

				// now commit field types change
				FieldTypesUpdate.update( FTTable );
			} // if( FTTable.Rows.Count > 0 )
	
			// Fix views
			CswTableUpdate ViewsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01I-02_Views_Update", "node_views" );
			DataTable ViewsTable = ViewsUpdate.getTable( "where viewxml like '%\"Date\"%' or viewxml like '%\"Time\"%'" );
			foreach( DataRow ViewsRow in ViewsTable.Rows )
			{
				string ViewXml = ViewsRow["viewxml"].ToString();
				ViewXml = ViewXml.Replace( "\"Date\"", "\"DateTime\"" );
				ViewXml = ViewXml.Replace( "\"Time\"", "\"DateTime\"" );
				ViewsRow["viewxml"] = ViewXml;
			}
			ViewsUpdate.update( ViewsTable );


			// case 9943
			// Add "Date Format" property to User
			
			CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass);
			
			DataTable OCPTable = OCPUpdate.getEmptyTable();
			_CswNbtSchemaModTrnsctn.addObjectClassPropRow( OCPTable, UserOC, CswNbtObjClassUser.DateFormatPropertyName, CswNbtMetaDataFieldType.NbtFieldType.List, 
														   false, false, false, string.Empty, Int32.MinValue, false, false, false, false, 
														   "MM/dd/yyyy,dd-MM-yyyy,yyyy/MM/dd,dd MMM yyyy", Int32.MinValue, Int32.MinValue );
			OCPUpdate.update( OCPTable );

			_CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

        }//Update()

    }//class CswUpdateSchemaTo01I02

}//namespace ChemSW.Nbt.Schema


