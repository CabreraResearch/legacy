﻿using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.RscAdo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReport : CswNbtObjClass, ICswNbtPermissionTarget
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string RPTFile = "RPT File";
            public const string ReportName = "Report Name";
            public const string Category = "Category";
            public const string Sql = "SQL";
            public const string BtnRun = "Run";
            public const string Instructions = "Instructions";
            public const string ReportGroup = "Report Group";
            public const string WebService = "Web Service";
        }

        public CswNbtObjClassReport( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReport
        /// </summary>
        public static implicit operator CswNbtObjClassReport( CswNbtNode Node )
        {
            CswNbtObjClassReport ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ReportClass ) )
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
            if( null != ButtonData.NodeTypeProp )
            {
                if( PropertyName.BtnRun == ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    ButtonData.Action = CswEnumNbtButtonAction.popup;
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

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( CswSqlAnalysis.doesSqlContainDmlOrDdl( SQL.Text ) )
            {
                throw ( new CswDniException( "Invalid sql: " + SQL.Text ) );
            }
        }//beforeWriteNode()

        protected override void afterWriteNodeLogic()
        {
            // BZ 10048
            Collection<object> AfterModifyReportEvents = _CswNbtResources.CswEventLinker.Trigger( AfterModifyReportEventName );
            foreach( object Handler in AfterModifyReportEvents )
            {
                if( Handler is AfterModifyReportEventHandler )
                    ( (AfterModifyReportEventHandler) Handler )();
            }
        }//afterWriteNode()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropMemo SQL { get { return ( _CswNbtNode.Properties[PropertyName.Sql] ); } }
        public CswNbtNodePropButton Run { get { return ( _CswNbtNode.Properties[PropertyName.BtnRun] ); } }
        public CswNbtNodePropBlob RPTFile { get { return ( _CswNbtNode.Properties[PropertyName.RPTFile] ); } }
        public CswNbtNodePropText ReportName { get { return ( _CswNbtNode.Properties[PropertyName.ReportName] ); } }
        public CswNbtNodePropText Category { get { return ( _CswNbtNode.Properties[PropertyName.Category] ); } }
        public CswNbtNodePropMemo Instructions { get { return ( _CswNbtNode.Properties[PropertyName.Instructions] ); } }
        public CswNbtNodePropRelationship ReportGroup { get { return ( _CswNbtNode.Properties[PropertyName.ReportGroup] ); } }
        public CswNbtNodePropMemo WebService { get { return ( _CswNbtNode.Properties[PropertyName.WebService] ); } }

        #endregion

        #region custom logic

        public sealed class ControlledParams
        {
            public const string UserId = "userid";
            public const string NodeId = "nodeid";
            public const string RoleId = "roleid";
            public static CswCommaDelimitedString List = new CswCommaDelimitedString { UserId, NodeId, RoleId };
        }

        /// <summary>
        /// Extract report parameters for a report
        /// </summary>
        /// <param name="UserNode">Use this user for matching parameter values</param>
        /// <param name="SourceNode">Use this node for matching parameter values</param>
        /// <returns></returns>
        public Dictionary<string, string> ExtractReportParams( CswNbtObjClassUser UserNode = null, CswNbtNode SourceNode = null )
        {
            Dictionary<string, string> reportParams = new Dictionary<string, string>();
            MatchCollection matchedParams = null;
            if( false == string.IsNullOrEmpty( WebService.Text ) )
            {
                matchedParams = Regex.Matches( WebService.Text, @"\{([\s\w0-9])+\}" );
            }
            else if( false == string.IsNullOrEmpty( SQL.Text ) )
            {
                matchedParams = Regex.Matches( SQL.Text, @"\{([\s\w0-9])+\}" );
            }
            foreach( Match match in matchedParams )
            {
                string paramName = match.Value.Replace( '{', ' ' ).Replace( '}', ' ' ).Trim(); //remove the '{' and '}' and whitespace

                string replacementVal = "";
                if( null != UserNode )
                {
                    if( paramName == ControlledParams.UserId ||
                        paramName == ControlledParams.NodeId )
                    {
                        replacementVal = UserNode.Node.NodeId.PrimaryKey.ToString();
                    }
                    else if( paramName == ControlledParams.RoleId )
                    {
                        replacementVal = UserNode.RoleId.PrimaryKey.ToString();
                    }
                    else
                    {
                        CswNbtMetaDataNodeTypeProp userNTP = UserNode.NodeType.getNodeTypeProp( paramName );
                        if( null != userNTP )
                        {
                            replacementVal = UserNode.Node.Properties[userNTP].Gestalt;
                        }
                    }
                } // if( null != UserNode )

                if( null != SourceNode )
                {
                    CswNbtMetaDataNodeTypeProp sourceNTP = SourceNode.getNodeType().getNodeTypeProp( paramName );
                    if( null != sourceNTP )
                    {
                        replacementVal = SourceNode.Properties[sourceNTP].Gestalt;
                    }
                }

                if( false == string.IsNullOrEmpty( SQL.Text ) )
                {
                    if( CswSqlAnalysis.doesSqlContainDmlOrDdl( replacementVal ) )
                    {
                        throw ( new CswDniException( "Parameter contains sql: " + paramName ) );
                    }
                }

                if( false == reportParams.ContainsKey( paramName ) )
                {
                    reportParams.Add( paramName, replacementVal );
                }
            }
            return reportParams;
        }

        //Case 30293: This is not intended to provide the most rubust SQL Param massage possible;
        //rather, it handles only a few edge cases. This should be refactored as more robust SQL mechanics are implemented.
        private static string _getParamVal( string Param )
        {
            string Ret = "";
            Int32 IntVal = CswConvert.ToInt32( Param );
            if( Int32.MinValue != IntVal )
            {
                Ret = IntVal.ToString();
            }
            else
            {
                Ret = CswTools.SafeSqlParam( Param );
            }

            return Ret;
        }

        public static string ReplaceReportParams( string SQL, Dictionary<string, string> reportParams )
        {
            foreach( var paramPair in reportParams )
            {
                SQL = SQL.Replace( "{" + paramPair.Key + "}", _getParamVal( paramPair.Value ) );
            }
            return SQL;
        }

        public CswPrimaryKey getPermissionGroupId()
        {
            return ReportGroup.RelatedNodeId;
        }

        #endregion

    }//CswNbtObjClassReport

}//namespace ChemSW.Nbt.ObjClasses
