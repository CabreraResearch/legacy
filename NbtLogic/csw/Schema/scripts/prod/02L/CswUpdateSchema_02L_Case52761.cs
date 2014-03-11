using ChemSW.MtSched.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Sched;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case52761 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 52761; }
        }

        public override string AppendToScriptName()
        {
            return "";
        }

        public override string Title
        {
            get { return "Create Duplicate Materials Report"; }
        }

        public override void update()
        {
            string ReportSQL = @"with chemicals as (
select * from(
  select ch.nodeid, ch.casno, ch.tradename, ch.istierii from chemical ch
  union all
  select co.nodeid, co.casno, co.tradename, co.istierii from constituent co) 
  where casno is not null
),
counts as (
  select count(nodeid) counts, casno from chemicals group by casno
),
chemical_dupes as (
  select c.nodeid as materialid, c.casno, c.tradename, decode(c.istierii, null, 'N', c.istierii) istierii from chemicals c
  join counts ct on ct.casno = c.casno
  where counts > 1
),
tier_ii_casnos as (
  select distinct casno, istierii from chemical_dupes where istierii = 'Y'
)
select * from chemical_dupes where casno in (select casno from tier_ii_casnos)
order by casno, tradename";
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtObjClassReportGroup CISProReportGroup = _getReportGroup();
            CswNbtMetaDataNodeType GenericReportNT = ReportOC.FirstNodeType;
            foreach( CswNbtMetaDataNodeType ReportNT in ReportOC.getNodeTypes() )
            {
                if( ReportNT.NodeTypeName == "Report" )
                {
                    GenericReportNT = ReportNT;
                    break;
                }
            }
            foreach( CswNbtObjClassReport DupChemsReport in ReportOC.getNodes( false, false, false ) )
            {
                if( DupChemsReport.ReportName.Text == "Duplicate Chemicals" )
                {
                    DupChemsReport.Node.delete( false, true );
                }
            }
            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( GenericReportNT.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassReport ReportNode = NewNode;
                if( null != CISProReportGroup )
                {
                    ReportNode.ReportGroup.RelatedNodeId = CISProReportGroup.NodeId;
                }
                ReportNode.SQL.Text = ReportSQL;
                ReportNode.ReportName.Text = "Duplicate Chemicals";
            } );
        } // update()

        private CswNbtObjClassReportGroup _getReportGroup()
        {
            CswNbtObjClassReportGroup RG = null;
            CswNbtMetaDataObjectClass ReportGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass );
            foreach( CswNbtObjClassReportGroup ReportGroup in ReportGroupOC.getNodes( false, false ) )
            {
                if( ReportGroup.Name.Text == "CISPro Report Group" )
                {
                    RG = ReportGroup;
                }
            }
            return RG;
        }
    }
}//namespace ChemSW.Nbt.Schema