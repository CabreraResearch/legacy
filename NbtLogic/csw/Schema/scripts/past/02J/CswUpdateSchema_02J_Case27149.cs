using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case27149: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27149; }
        }

        public override string Title
        {
            get { return "Add report for Containers in invalid Locations"; }
        }
        
        public override void update()
        {
            const string ReportName = "Containers in Invalid Locations";
            string RealReportName = _getUniqueName( ReportName );

            CswNbtMetaDataNodeType ReportNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Report" );

            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassReport AsReport = NewNode;

                    #region Set SQL Text

                    AsReport.SQL.Text = @"select c.Barcode,
 c.Material,
 c.Owner,
 c.Location
from container c
       join nodes n on c.Material_id = n.nodeid
       join nodes n2 on c.Location_id = n2.nodeid
 
       join jct_nodes_props jnp on jnp.nodeid = n.nodeid 
         and jnp.nodetypepropid = (select nodetypepropid from nodetype_props where propname = 'Storage Compatibility'
             and nodetypeid = 1014)
       join jct_nodes_props jnp2 on jnp2.nodeid = n2.nodeid
         and jnp2.nodetypepropid in (select nodetypepropid from nodetype_props where propname = 'Storage Compatibility'
             and nodetypeid in (select nodetypeid from nodetypes where objectclassid in (select objectclassid from object_class where objectclass = 'LocationClass')))
 
where (jnp2.clobdata is not null and dbms_lob.instr(jnp2.clobdata, jnp.clobdata) = 0)
      or (jnp.clobdata is null or dbms_lob.instr(jnp.clobdata, '0w') > 0)
      or( jnp2.clobdata is null or dbms_lob.instr(jnp2.clobdata, '0w') > 0)";

                    #endregion

                    AsReport.ReportName.Text = RealReportName;
                    AsReport.Category.Text = "Containers";
                    AsReport.ReportGroup.RelatedNodeId = _getCISProReportGroupNodeId();

                } );


        } // update()

        private string _getUniqueName( string ReportName )
        {
            string RealReportName = ReportName;

            bool NameUsed = true;
            int idx = 1;
            while( NameUsed )
            {
                ICswNbtTree Reports = _getReportTree( RealReportName );
                NameUsed = Reports.getChildNodeCount() > 0;
                if( NameUsed )
                {
                    RealReportName = ReportName + idx;
                    idx++;
                }
            }

            return RealReportName;
        }

        private ICswNbtTree _getReportTree( string ReportName )
        {
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp ReportNameOCP = ReportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.ReportName );

            CswNbtView ReportNodeView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship root = ReportNodeView.AddViewRelationship( ReportOC, false );
            ReportNodeView.AddViewPropertyAndFilter( root, ReportNameOCP,
                Value : ReportName,
                FilterMode : CswEnumNbtFilterMode.Equals );

            return _CswNbtSchemaModTrnsctn.getTreeFromView( ReportNodeView, true );
        }

        private CswPrimaryKey _getCISProReportGroupNodeId()
        {
            CswPrimaryKey ret = null;

            CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
            CswNbtMetaDataObjectClassProp NameOCP = ReportGroupOC.getObjectClassProp( CswNbtObjClassReportGroup.PropertyName.Name );

            CswNbtView ReportGroupsView = _CswNbtSchemaModTrnsctn.makeView();
            CswNbtViewRelationship root = ReportGroupsView.AddViewRelationship( ReportGroupOC, false );
            ReportGroupsView.AddViewPropertyAndFilter( root, NameOCP,
                Value : "CISPro Report Group",
                FilterMode : CswEnumNbtFilterMode.Equals );
            ICswNbtTree ReportGroups = _CswNbtSchemaModTrnsctn.getTreeFromView( ReportGroupsView, true );

            for( int i = 0; i < ReportGroups.getChildNodeCount(); i++ )
            {
                ReportGroups.goToNthChild( 0 );
                ret = ReportGroups.getNodeIdForCurrentPosition();
                ReportGroups.goToRoot();
            }

            return ret;
        }

    }

}//namespace ChemSW.Nbt.Schema