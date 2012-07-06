using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReport : CswNbtObjClass
    {
        public const string RPTFilePropertyName = "RPT File";
        public const string ReportNamePropertyName = "Report Name";
        public const string CategoryPropertyName = "Category";
        public const string SqlPropertyName = "SQL";
        public const string FormattedSqlPropertyName = "FormattedSQL";
        public const string btnRunPropertyName = "Run";
        public const string ReportUserNamePropertyName = "ReportUserName";

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassReport( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReport
        /// </summary>
        public static implicit operator CswNbtObjClassReport( CswNbtNode Node )
        {
            CswNbtObjClassReport ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass ) )
            {
                ret = (CswNbtObjClassReport) Node.ObjClass;
            }
            return ret;
        }

        #region Object class specific Events

        public delegate void AfterModifyReportEventHandler();
        public static string AfterModifyReportEventName = "AfterModifyReport";

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out JObject ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = new JObject();
            ButtonAction = NbtButtonAction.Unknown;
            CswNbtMetaDataObjectClassProp OCP = NodeTypeProp.getObjectClassProp();
            if( null != NodeTypeProp && null != OCP )
            {
                if( btnRunPropertyName == OCP.PropName )
                {
                    ButtonAction = NbtButtonAction.popup;
                    ActionData["url"] = ReportUrl;
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


            _CswNbtNode.Properties[ReportUserNamePropertyName].Hidden = true;
            _CswNbtNode.Properties[FormattedSqlPropertyName].Hidden = true;

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

                CswTemplateTextFormatter CswTemplateTextFormatter = new Core.CswTemplateTextFormatter();
                CswTemplateTextFormatter.addReplacementValue( "username", ReportUserName.Text );
                string Message = string.Empty;
                CswTemplateTextFormatter.setTemplateText( _CswNbtNode.Properties[SqlPropertyName].AsMemo.Text, ref Message );
                FormattedSQL.Text = CswTemplateTextFormatter.getFormattedText();

                return ( _CswNbtNode.Properties[FormattedSqlPropertyName] ); //sic.
            }
        }


        public CswNbtNodePropText ReportUserName
        {
            //set
            //{
            //    _CswNbtNode.Properties[ReportUserNamePropertyName].AsText.Text = value.Text; ;
            //}
            get
            {
                return ( _CswNbtNode.Properties[ReportUserNamePropertyName] );
            }
        }

        public CswNbtNodePropMemo FormattedSQL
        {
            get
            {
                return ( _CswNbtNode.Properties[FormattedSqlPropertyName] );
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



        //public CswNbtNodePropViewReference View
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[ViewPropertyName].AsViewReference );
        //    }
        //}

        #endregion

    }//CswNbtObjClassReport

}//namespace ChemSW.Nbt.ObjClasses
