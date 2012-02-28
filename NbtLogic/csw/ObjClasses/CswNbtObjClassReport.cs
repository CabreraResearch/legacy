using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.Logic;
using ChemSW.Exceptions;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReport : CswNbtObjClass
    {
        public static string RPTFilePropertyName { get { return "RPT File"; } }
        public static string ReportNamePropertyName { get { return "Report Name"; } }
        public static string CategoryPropertyName { get { return "Category"; } }
        public static string ViewPropertyName { get { return "View"; } }
        public static string SqlPropertyName { get { return "SQL"; } }
        public static string btnRunPropertyName { get { return "Run"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassReport( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassReport( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass ); }
        }

        #region Object class specific Events

        public delegate void AfterModifyReportEventHandler();
        public static string AfterModifyReportEventName = "AfterModifyReport";

        public override void onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, JObject ActionObj )
        {
            
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                if( btnRunPropertyName == OCP.PropName )
                {
                    ActionObj["action"] = "popup";
                    ActionObj["url"] = "report.html?reportid=" + Node.NodeId.ToString();
                    /*//if we are not RPT, then just run the SQL
                    if( Node.Properties[RPTFilePropertyName].AsBlob.Empty )
                    {
                        if(string.Empty != Node.Properties[SqlPropertyName].AsMemo.Text){
                                CswArbitrarySelect cswRptSql = _CswNbtResources.makeCswArbitrarySelect( "report_sql", Node.Properties[SqlPropertyName].AsMemo.Text );
                                DataTable rptDataTbl = cswRptSql.getTable();
                                ActionObj["action"] = "reportgrid";
                                CswGridData cg = new CswGridData( _CswNbtResources );
                                ActionObj["data"] = cg.DataTableToJSON( rptDataTbl );
                        }
                        else throw ( new CswDniException( "Report has no SQL to run!") );
                    }
                    else
                    {
                        //we are CRPE, run as such...
                        throw ( new CswDniException( "CRPE report not implemented yet." ) );
                    }*/
                }
            }
        }

        #endregion Object class specific Events


        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            // BZ 10048
            Collection<object> AfterModifyReportEvents = _CswNbtResources.CswEventLinker.Trigger( AfterModifyReportEventName );
            foreach( object Handler in AfterModifyReportEvents )
            {
                if( Handler is AfterModifyReportEventHandler )
                    ( (AfterModifyReportEventHandler) Handler )();
            }

            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo SQL
        {
            get
            {
                return ( _CswNbtNode.Properties[SqlPropertyName].AsMemo );
            }
        }

        public CswNbtNodePropButton Run
        {
            get
            {
                return ( _CswNbtNode.Properties[btnRunPropertyName].AsButton );
            }
        }

        public CswNbtNodePropBlob RPTFile
        {
            get
            {
                return ( _CswNbtNode.Properties[RPTFilePropertyName].AsBlob );
            }
        }

        public CswNbtNodePropText ReportName
        {
            get
            {
                return ( _CswNbtNode.Properties[ReportNamePropertyName].AsText );
            }
        }

        public CswNbtNodePropText Category
        {
            get
            {
                return ( _CswNbtNode.Properties[CategoryPropertyName].AsText );
            }
        }

        public CswNbtNodePropViewReference View
        {
            get
            {
                return ( _CswNbtNode.Properties[ViewPropertyName].AsViewReference );
            }
        }

        #endregion

    }//CswNbtObjClassReport

}//namespace ChemSW.Nbt.ObjClasses
