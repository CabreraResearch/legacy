using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Audit;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_01S_Case27709 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswAuditMetaData CswAuditMetaData = new CswAuditMetaData();


            foreach( CswNbtMetaDataNodeType CurrentNodeType in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes() )
            {
                if( AuditLevel.NoAudit != CurrentNodeType.AuditLevel )
                {


                    string CurrentAuditLevel = CurrentNodeType.AuditLevel.ToString();

                    string Where = " where nodetypeid= " + CurrentNodeType.NodeTypeId.ToString() + " and " + CswAuditMetaData.AuditLevelColName + " <> '" + CurrentAuditLevel + "' ";
                    CswTableUpdate CurrentNodesUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update_nodes_case_27709", "nodes" );
                    DataTable NodesTable = CurrentNodesUpdate.getTable( Where );
                    foreach( DataRow CurrentRow in NodesTable.Rows )
                    {
                        CurrentRow[CswAuditMetaData.AuditLevelColName] = CurrentAuditLevel;
                    }

                    CurrentNodesUpdate.update( NodesTable );
                }
            }

        }//Update()

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 27709; }
        }

    }//class CswUpdateSchema_01S_Case27709

}//namespace ChemSW.Nbt.Schema