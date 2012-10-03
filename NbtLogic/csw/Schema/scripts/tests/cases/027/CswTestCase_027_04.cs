using System;
using System.Data;
using System.Threading;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_027_04 : CswUpdateSchemaTo
    {
        public override string Description { set { ; } get { return ( CswTestCaseRsrc.makeTestCaseDescription( this.GetType().Name, CswTstCaseRsrc_027.Purpose, "CswNbtActUpdatePropertyValue: " + TotalIterations.ToString() + " iterations"  ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_027 _CswTstCaseRsrc_027 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        //public override CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_027_04( CswSchemaVersion CswSchemaVersion, object CswTstCaseRsc )
        {
            _CswSchemaVersion = CswSchemaVersion;
			_CswTstCaseRsrc_027 = (CswTstCaseRsrc_027) CswTstCaseRsc;

        }//ctor

        public Int32 TotalIterations = 100; 
        public override void update()
        {
			_CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
			_CswTstCaseRsrc_027.CswNbtSchemaModTrnsctn = _CswNbtSchemaModTrnsctn;
			
			// Find which nodes are out of date
            CswStaticSelect OutOfDateNodesQuerySelect = _CswNbtSchemaModTrnsctn.makeCswStaticSelect( "OutOfDateNodes_select", "ValuesToUpdate" );
            DataTable OutOfDateNodes = OutOfDateNodesQuerySelect.getTable( false, false, 0, 25 );

            if( OutOfDateNodes.Rows.Count > 0 )
            {
                // Update one of them at random (which will keep us from encountering errors which gum up the queue)
                for( Int32 idx = 0; idx < TotalIterations; idx++ )
                {

                    Int32 TestEveryNth = 10;
                    bool DoMemoryTest = ( 0 == ( idx % TestEveryNth ) );

                    if( DoMemoryTest )
                    {
                        _CswTstCaseRsrc_027.memoryTestBegin();
                    }

                    Random rand = new Random();
                    Int32 index = rand.Next( 0, OutOfDateNodes.Rows.Count );
                    CswPrimaryKey nodeid = new CswPrimaryKey( "nodes", CswConvert.ToInt32( OutOfDateNodes.Rows[index]["nodeid"].ToString() ) );
                    //Int32 propid = CswConvert.ToInt32(OutOfDateNodes.Rows[index]["nodetypepropid"].ToString());
                    //Int32 jctnodepropid = CswConvert.ToInt32(OutOfDateNodes.Rows[index]["jctnodepropid"].ToString());
                    CswNbtNode Node = _CswNbtSchemaModTrnsctn.Nodes[nodeid];
                    if( Node == null )
                        throw new CswDniException( "Node not found (" + nodeid.ToString() + ")" );
                    // Don't update nodes of disabled nodetypes
                    if( Node.getNodeType() != null )
                    {
                        CswNbtActUpdatePropertyValue CswNbtActUpdatePropertyValue = _CswNbtSchemaModTrnsctn.getCswNbtActUpdatePropertyValue();
                        CswNbtActUpdatePropertyValue.UpdateNode( Node, false );
                        Node.postChanges( false );
                    }

                    if( DoMemoryTest )
                    {
                        _CswTstCaseRsrc_027.memoryTestEnd();
                        _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total Process Memory after " + idx.ToString() + "iterations: " + _CswTstCaseRsrc_027.TotalProcessMemory );
                        _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": Total GC Memory after: " + idx.ToString() + "iterations: " + _CswTstCaseRsrc_027.TotalGCMemorySansCollection );
                        _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": ProcessMemory Delta after: " + idx.ToString() + "iterations:  " + _CswTstCaseRsrc_027.ProcessMemoryDelta );
                        _CswNbtSchemaModTrnsctn.CswLogger.reportAppState( Description + ": GCMemory Delta: after " + _CswTstCaseRsrc_027.GCMemoryDelta );
                    }

                    Thread.Sleep( 100 ); 

                }//iterate 

            }//if there were out of date nodes


        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.PG; }
        }

        public override int CaseNo
        {
            get { return 0; }
        }

        //runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
