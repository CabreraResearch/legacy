using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReport : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string RPTFile = "RPT File";
            public const string ReportName = "Report Name";
            public const string Category = "Category";
            public const string Sql = "SQL";
            public const string BtnRun = "Run";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassReport( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.ReportClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReport
        /// </summary>
        public static implicit operator CswNbtObjClassReport( CswNbtNode Node )
        {
            CswNbtObjClassReport ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.ReportClass ) )
            {
                ret = (CswNbtObjClassReport) Node.ObjClass;
            }
            return ret;
        }

        #region Object class specific Events

        public delegate void AfterModifyReportEventHandler();
        public static string AfterModifyReportEventName = "AfterModifyReport";

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {



            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                if( PropertyName.BtnRun == OCP.PropName )
                {
                    ButtonData.Action = NbtButtonAction.popup;
                    ButtonData.Data["url"] = ReportUrl;
                }
            }
            return true;
        }

        public string ReportUrl
        {
            get
            {
                return "Report.html?reportid=" + NodeId.ToString();
            }
        }

        #endregion Object class specific Events


        #region Inherited Events

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

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

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
                return ( _CswNbtNode.Properties[PropertyName.Sql] ); //sic.
            }
        }

        public CswNbtNodePropButton Run { get { return ( _CswNbtNode.Properties[PropertyName.BtnRun] ); } }
        public CswNbtNodePropBlob RPTFile { get { return ( _CswNbtNode.Properties[PropertyName.RPTFile] ); } }
        public CswNbtNodePropText ReportName { get { return ( _CswNbtNode.Properties[PropertyName.ReportName] ); } }
        public CswNbtNodePropText Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }

        //public CswNbtNodePropViewReference View
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[ViewPropertyName].AsViewReference );
        //    }
        //}

        #endregion

        #region custom logic

        public Dictionary<string, string> ExtractReportParams( CswNbtObjClassUser UserNode = null )
        {
            Dictionary<string, string> reportParams = new Dictionary<string, string>();

            MatchCollection matchedParams = Regex.Matches( SQL.Text, @"\{(\w|[0-9])*\}" );
            foreach( Match match in matchedParams )
            {
                string paramName = match.Value.Replace( '{', ' ' ).Replace( '}', ' ' ).Trim(); //remove the '{' and '}' and whitespace
                string replacementVal = "";
                if( null != UserNode )
                {
                    CswNbtMetaDataNodeTypeProp userNTP = UserNode.NodeType.getNodeTypeProp( paramName );
                    if( null != userNTP )
                    {
                        replacementVal = UserNode.Node.Properties[userNTP].Gestalt;
                    }
                    else if( paramName == "nodeid" || paramName == "userid" )
                    {
                        replacementVal = UserNode.Node.NodeId.PrimaryKey.ToString();
                    }
                }
                if( false == reportParams.ContainsKey( paramName ) )
                {
                    reportParams.Add( paramName, replacementVal );
                }
            }
            return reportParams;
        }

        public static string ReplaceReportParams( string SQL, Dictionary<string, string> reportParams )
        {
            foreach( var paramPair in reportParams )
            {
                SQL = SQL.Replace( "{" + paramPair.Key + "}", CswTools.SafeSqlParam( paramPair.Value ) );
            }
            return SQL;
        }

        #endregion

    }//CswNbtObjClassReport

}//namespace ChemSW.Nbt.ObjClasses
