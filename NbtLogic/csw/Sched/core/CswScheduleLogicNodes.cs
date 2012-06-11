using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicNodes
    {
        private string _ColName_QueryText = string.Empty;
        private CswNbtResources _CswNbtResources = null;
        public CswScheduleLogicNodes( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor


        private List<CswNbtNode> _getRawNodes( NbtScheduleRuleNames NbtScheduleRuleName )
        {
            List<CswNbtNode> ReturnVal = new List<CswNbtNode>();

            CswStaticSelect CswTableSelect = _CswNbtResources.makeCswStaticSelect( "query for s4: " + NbtScheduleRuleName.ToString(), NbtScheduleRuleName.ToString() );
            DataTable DataTable = CswTableSelect.getTable();
            foreach( DataRow CurrentRow in DataTable.Rows )
            {
                Int32 NodeId = CswConvert.ToInt32( CurrentRow["nodeid"] );
                if( Int32.MinValue != NodeId )
                {
                    CswPrimaryKey NodePk = new CswPrimaryKey( "nodes", NodeId );
                    CswNbtNode CswNbtNode = _CswNbtResources.Nodes.GetNode( NodePk );
                    if( null != CswNbtNode )
                    {
                        ReturnVal.Add( CswNbtNode );
                    }
                }
            }

            return ( ReturnVal );

        }//getRawNodes() 

        public List<CswNbtObjClassInspectionDesign> getInspectonDesigns()
        {
            List<CswNbtObjClassInspectionDesign> ReturnVal = new List<CswNbtObjClassInspectionDesign>();

            List<CswNbtNode> RawNodes = _getRawNodes( NbtScheduleRuleNames.UpdtInspection );
            foreach( CswNbtNode CurrentRawNode in RawNodes )
            {
                ReturnVal.Add( (CswNbtObjClassInspectionDesign) CurrentRawNode );
            }

            return ( ReturnVal );

        }//getInspectons()

        public List<CswNbtObjClassMailReport> getMailReports()
        {
            List<CswNbtObjClassMailReport> ReturnVal = new List<CswNbtObjClassMailReport>();

            List<CswNbtNode> RawNodes = _getRawNodes( NbtScheduleRuleNames.GenEmailRpt );
            foreach( CswNbtNode CurrentRawNode in RawNodes )
            {
                ReturnVal.Add( (CswNbtObjClassMailReport) CurrentRawNode );
            }

            return ( ReturnVal );

        }//getMailReports()

        public List<CswNbtObjClassGenerator> getGenerators()
        {
            List<CswNbtObjClassGenerator> ReturnVal = new List<CswNbtObjClassGenerator>();

            List<CswNbtNode> RawNodes = _getRawNodes( NbtScheduleRuleNames.GenNode );
            foreach( CswNbtNode CurrentRawNode in RawNodes )
            {
                ReturnVal.Add( (CswNbtObjClassGenerator) CurrentRawNode );
            }

            return ( ReturnVal );

        }//getMailReports()

    }//CswScheduleLogicNodes

}//namespace ChemSW.MtSched


