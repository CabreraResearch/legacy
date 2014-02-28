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
    public class CswUpdateSchema_02L_Case31909: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 31909; }
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
)
select c.nodeid, c.casno, c.tradename, decode(c.istierii, null, 'N', c.istierii) istierii from chemicals c
join counts ct on ct.casno = c.casno
where counts > 1";
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtObjClassReportGroup CISProReportGroup = _getReportGroup();
            _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( ReportOC.FirstNodeType.NodeTypeId, delegate( CswNbtNode NewNode )
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