using System.Collections.Generic;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Data;
using ChemSW.DB;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
	/// <summary>
	/// Updates the schema to version 01H-26
	/// </summary>
	public class CswUpdateSchemaTo01H26 : ICswUpdateSchemaTo
	{
		private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

		public CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'H', 26 ); } }
		public CswUpdateSchemaTo01H26( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
		{
			_CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
		}

		public void update()
		{
			// case 21364
			// fill new viewmode column on node_views

			CswTableUpdate ViewsUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01H26_update_views", "node_views" );
			DataTable ViewsTable = ViewsUpdate.getTable();
			foreach( DataRow ViewsRow in ViewsTable.Rows )
			{
				CswNbtView ThisView = _CswNbtSchemaModTrnsctn.restoreView( CswConvert.ToInt32( ViewsRow["nodeviewid"] ) );
				ViewsRow["viewmode"] = ThisView.ViewMode.ToString();
			}
			ViewsUpdate.update( ViewsTable );

			// update S4s to include new viewmode column
			_CswNbtSchemaModTrnsctn.UpdateS4( "getViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid, r.nodename rolename, u.nodename username
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where nodeviewid = :getviewid
order by lower(v.viewname)" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getAllViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid,
v.userid, v.category, r.nodename rolename, u.nodename username
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where visibility != 'Property'
order by lower(v.viewname)" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getVisibleViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid,
v.category, r.nodename rolename, u.nodename username, v.viewxml
,lower(NVL(v.category, v.viewname)) mssqlorder
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where ((visibility = 'Global') or
       (visibility = 'Role' and roleid = :getroleid) or
       (visibility = 'User' and userid = :getuserid))
       #addclause
order by #orderbyclause" );

			_CswNbtSchemaModTrnsctn.UpdateS4( "getUserViewInfo", @"select v.nodeviewid, v.viewname, v.viewmode, v.visibility, v.roleid, v.userid, v.category, r.nodename rolename, u.nodename username
from node_views v
left outer join nodes r on v.roleid = r.nodeid
left outer join nodes u on v.userid = u.nodeid
where (visibility = 'User' and userid = :getuserid)
order by lower(NVL(v.category, v.viewname)), lower(v.viewname)" );




			// case 20871
			List<CswNbtView> AllMountPoints = _CswNbtSchemaModTrnsctn.restoreViews( "All FE Inspection Points" );
			CswNbtMetaDataNodeType MountPointNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( CswSchemaUpdater.HamletNodeTypesAsString( CswSchemaUpdater.HamletNodeTypes.FE_Inspection_Point ) );
			if( null != MountPointNT )
			{
				CswNbtMetaDataNodeTypeProp BarcodeNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.BarcodePropertyName );
				CswNbtMetaDataNodeTypeProp DescriptionNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.DescriptionPropertyName );
				CswNbtMetaDataNodeTypeProp StatusNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.StatusPropertyName );
				CswNbtMetaDataNodeTypeProp TypeNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.TypePropertyName );
				CswNbtMetaDataNodeTypeProp LocationNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.LocationPropertyName );
				CswNbtMetaDataNodeTypeProp MountPointGroupNtp = MountPointNT.getNodeTypePropByObjectClassPropName( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
				foreach( CswNbtView View in AllMountPoints )
				{
					View.Root.ChildRelationships.Clear();
					CswNbtViewRelationship MountPointVr = View.AddViewRelationship( MountPointNT, false );
					CswNbtViewProperty BarcodeVp = View.AddViewProperty( MountPointVr, BarcodeNtp );
					View.AddViewPropertyFilter( BarcodeVp, CswNbtSubField.SubFieldName.Barcode, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

					CswNbtViewProperty DescriptionVp = View.AddViewProperty( MountPointVr, DescriptionNtp );
					View.AddViewPropertyFilter( DescriptionVp, CswNbtSubField.SubFieldName.Text, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

					CswNbtViewProperty StatusVp = View.AddViewProperty( MountPointVr, StatusNtp );
					View.AddViewPropertyFilter( StatusVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

					CswNbtViewProperty TypeVp = View.AddViewProperty( MountPointVr, TypeNtp );
					View.AddViewPropertyFilter( TypeVp, CswNbtSubField.SubFieldName.Value, CswNbtPropFilterSql.PropertyFilterMode.Equals, string.Empty, false );

					CswNbtViewProperty LocationVp = View.AddViewProperty( MountPointVr, LocationNtp );
					View.AddViewPropertyFilter( LocationVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );

					CswNbtViewProperty MountPointGroupVp = View.AddViewProperty( MountPointVr, MountPointGroupNtp );
					View.AddViewPropertyFilter( MountPointGroupVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, string.Empty, false );
					View.save();
				}
			}

            // case 20924
            CswNbtView MyProblems = _CswNbtSchemaModTrnsctn.restoreView( "My Problems" );
            if( null != MyProblems )
            {
                MyProblems.Root.ChildRelationships.Clear();

                CswNbtMetaDataObjectClass ProblemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass );
                if( 0 < ProblemOC.NodeTypes.Count )
                {
                    foreach( CswNbtMetaDataNodeType Problem in ProblemOC.NodeTypes )
                    {
                        CswNbtViewRelationship ProblemRelationship = MyProblems.AddViewRelationship( Problem, false );

                        CswNbtMetaDataNodeTypeProp ReportedByNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ReportedByPropertyName );
                        CswNbtViewProperty ReportedByVp = MyProblems.AddViewProperty( ProblemRelationship, ReportedByNtp );
                        ReportedByVp.Order = 1;

                        CswNbtMetaDataNodeTypeProp ClosedNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.ClosedPropertyName );
                        CswNbtViewProperty ClosedVp = MyProblems.AddViewProperty( ProblemRelationship, ClosedNtp );
                        ClosedVp.Order = 2;
                        MyProblems.AddViewPropertyFilter( ClosedVp, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "false", false );

                        CswNbtMetaDataNodeTypeProp DateOpenedNtp = Problem.getNodeTypePropByObjectClassPropName( CswNbtObjClassProblem.DateOpenedPropertyName );
                        CswNbtViewProperty DateOpenedVp = MyProblems.AddViewProperty( ProblemRelationship, DateOpenedNtp );
                        DateOpenedVp.Order = 3;

                        CswNbtMetaDataNodeTypeProp EquipmentNtp = Problem.getNodeTypeProp( "Equipment" );
                        if( null != EquipmentNtp )
                        {
                            CswNbtViewProperty EquipmentVp = MyProblems.AddViewProperty( ProblemRelationship, EquipmentNtp );
                            EquipmentVp.Order = 4;
                        }

                        CswNbtMetaDataNodeTypeProp AssemblyNtp = Problem.getNodeTypeProp( "Assembly" );
                        if( null != AssemblyNtp )
                        {
                            CswNbtViewProperty AssemblyVp = MyProblems.AddViewProperty( ProblemRelationship, AssemblyNtp );
                            AssemblyVp.Order = 5;
                        }

                        CswNbtMetaDataNodeTypeProp LocationNtp = Problem.getNodeTypeProp( "Location" );
                        if( null != LocationNtp )
                        {
                            CswNbtViewProperty LocationVp = MyProblems.AddViewProperty( ProblemRelationship, LocationNtp );
                            LocationVp.Order = 6;
                        }

                        CswNbtMetaDataNodeTypeProp SummaryNtp = Problem.getNodeTypeProp( "Summary" );
                        if( null != SummaryNtp )
                        {
                            CswNbtViewProperty SummaryVp = MyProblems.AddViewProperty( ProblemRelationship, SummaryNtp );
                            SummaryVp.Order = 7;
                        }

                        CswNbtMetaDataNodeTypeProp TechnicianNtp = Problem.getNodeTypeProp( "Technician" );
                        if( null != TechnicianNtp )
                        {
                            CswNbtViewProperty TechnicianVp = MyProblems.AddViewProperty( ProblemRelationship, TechnicianNtp );
                            TechnicianVp.Order = 8;
                            MyProblems.AddViewPropertyFilter( TechnicianVp, CswNbtSubField.SubFieldName.Name, CswNbtPropFilterSql.PropertyFilterMode.Equals, "me", false );
                        }

                    }
                    MyProblems.save();
                }

            }


			//No else, just leave the OC-based views
		} // update()

	}//class CswUpdateSchemaTo01H26

}//namespace ChemSW.Nbt.Schema

