using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using ChemSW.NbtWebControls;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt.WebPages
{
    public partial class Subscriptions : System.Web.UI.Page
    {
        private CswAutoTable _Table;
        private CswCheckBoxArray _NotifCBArray;
        private CswCheckBoxArray _MailReportCBArray;
        private CswDataTable _NotifData;
        private CswDataTable _MailReportData;
        private ICswNbtTree _NotifTree;
        private ICswNbtTree _MailReportTree;
        private Button SaveButton;

        private string NotifPrefix = "notif_";
        private string MailReportPrefix = "mr_";

        protected override void OnInit( EventArgs e )
        {
            try
            {
                this.EnableViewState = false;

                if( ( (CswNbtObjClassUser) Master.CswNbtResources.CurrentNbtUser.UserNode ).EmailProperty.Empty )
                    throw new CswDniException( "Email address required for subscriptions", "Current user has no email address defined" );

                CswCenteredDiv Div = new CswCenteredDiv();
                ph.Controls.Add( Div );

                Div.Controls.Add( new CswLiteralText( "Edit Your Subscriptions to Notifications and Mail Reports:" ) );

                _Table = new CswAutoTable();
                Div.Controls.Add( _Table );

                // Notifications
                _Table.addControl( 0, 0, new CswLiteralNbsp() );
                _Table.addControl( 1, 0, new CswLiteralNbsp() );

                _NotifCBArray = new CswCheckBoxArray( Master.CswNbtResources );
                _NotifCBArray.ID = "NotificationCBArray";
                _NotifCBArray.UseRadios = false;
                _NotifCBArray.Rows = 10;
                _Table.addControl( 2, 0, _NotifCBArray );

                _NotifData = new CswDataTable( "NotificationsDT", "" );
                _NotifData.Columns.Add( "Notification", typeof( string ) );
                _NotifData.Columns.Add( "notificationid", typeof( int ) );
                _NotifData.Columns.Add( "Subscribe", typeof( bool ) );
                _NotifTree = Master.CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.NotificationClass );
                for( Int32 n = 0; n < _NotifTree.getChildNodeCount(); n++ )
                {
                    _NotifTree.goToNthChild( n );

                    CswNbtNode NotifNode = _NotifTree.getNodeForCurrentPosition();
                    string SearchStr = CswNbtNodePropUserSelect.delimiter.ToString() + CswNbtNodeCaster.AsNotification( NotifNode ).SubscribedUsers.SelectedUserIds + CswNbtNodePropUserSelect.delimiter.ToString();
                    bool Checked = SearchStr.Contains( CswNbtNodePropUserSelect.delimiter.ToString() + Master.CswNbtResources.CurrentNbtUser.UserId.PrimaryKey.ToString() + CswNbtNodePropUserSelect.delimiter.ToString() );

                    DataRow NotifRow = _NotifData.NewRow();
                    NotifRow["Notification"] = NotifNode.NodeName;
                    NotifRow["notificationid"] = NotifNode.NodeId.PrimaryKey;
                    NotifRow["Subscribe"] = Checked;
                    _NotifData.Rows.Add( NotifRow );

                    _NotifTree.goToParentNode();
                }
                _NotifCBArray.CreateCheckBoxes( _NotifData, "Notification", "notificationid" );


                // Mail Reports
                _Table.addControl( 0, 1, new CswLiteralNbsp() );
                _Table.addControl( 0, 1, new CswLiteralNbsp() );
                _Table.addControl( 0, 1, new CswLiteralNbsp() );
                _Table.addControl( 0, 1, new CswLiteralNbsp() );
                _Table.addControl( 0, 1, new CswLiteralNbsp() );

                _Table.addControl( 0, 2, new CswLiteralNbsp() );
                _Table.addControl( 1, 2, new CswLiteralNbsp() );

                _MailReportCBArray = new CswCheckBoxArray( Master.CswNbtResources );
                _MailReportCBArray.ID = "MailReportCBArray";
                _MailReportCBArray.UseRadios = false;
                _MailReportCBArray.Rows = 10;
                _Table.addControl( 2, 2, _MailReportCBArray );

                _MailReportData = new CswDataTable( "MailReportsDT", "" );
                _MailReportData.Columns.Add( "Mail Report", typeof( string ) );
                _MailReportData.Columns.Add( "mailreportid", typeof( int ) );
                _MailReportData.Columns.Add( "Subscribe", typeof( bool ) );
                _MailReportTree = Master.CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
                for( Int32 n = 0; n < _MailReportTree.getChildNodeCount(); n++ )
                {
                    _MailReportTree.goToNthChild( n );

                    CswNbtNode MailReportNode = _MailReportTree.getNodeForCurrentPosition();
                    string SearchStr = CswNbtNodePropUserSelect.delimiter.ToString() + CswNbtNodeCaster.AsMailReport( MailReportNode ).Recipients.SelectedUserIds + CswNbtNodePropUserSelect.delimiter.ToString();
                    bool Checked = SearchStr.Contains( CswNbtNodePropUserSelect.delimiter.ToString() + Master.CswNbtResources.CurrentNbtUser.UserId.PrimaryKey.ToString() + CswNbtNodePropUserSelect.delimiter.ToString() );

                    DataRow MailReportRow = _MailReportData.NewRow();
                    MailReportRow["Mail Report"] = MailReportNode.NodeName;
                    MailReportRow["mailreportid"] = MailReportNode.NodeId.PrimaryKey;
                    MailReportRow["Subscribe"] = Checked;
                    _MailReportData.Rows.Add( MailReportRow );

                    _MailReportTree.goToParentNode();
                }
                _MailReportCBArray.CreateCheckBoxes( _MailReportData, "Mail Report", "mailreportid" );

                // Save
                _Table.addControl( 3, 0, new CswLiteralNbsp() );

                SaveButton = new Button();
                SaveButton.ID = "SaveButton";
                SaveButton.CssClass = "Button";
                SaveButton.Text = "Save";
                SaveButton.Click += new EventHandler( SaveButton_Click );
                _Table.addControl( 4, 0, SaveButton );

            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        protected void SaveButton_Click( object sender, EventArgs e )
        {
            try
            {
                if( Page.IsValid && ph.HasControls() )
                {
                    // Save Notifications
                    Collection<Int32> CheckedNotifIds = _NotifCBArray.GetCheckedValues( "Subscribe" ).ToIntCollection();

                    for( Int32 n = 0; n < _NotifTree.getChildNodeCount(); n++ )
                    {
                        _NotifTree.goToNthChild( n );

                        CswNbtNode ThisNode = _NotifTree.getNodeForCurrentPosition();
                        CswNbtObjClassNotification NotifNode = CswNbtNodeCaster.AsNotification( ThisNode );
                        if( CheckedNotifIds.Contains( NotifNode.NodeId.PrimaryKey ) )
                        {
                            // subscribe!  (this will do nothing if we're already subscribed)
                            NotifNode.SubscribedUsers.AddUser( Master.CswNbtResources.CurrentNbtUser.UserId );
                        }
                        else
                        {
                            // unsubscribe!  (this will do nothing if we're already unsubscribed)
                            NotifNode.SubscribedUsers.RemoveUser( Master.CswNbtResources.CurrentNbtUser.UserId );
                        }
                        NotifNode.postChanges( false );
                        _NotifTree.goToParentNode();
                    }

                    // Save Mail Reports
                    Collection<Int32> CheckedMailReportIds = _MailReportCBArray.GetCheckedValues( "Subscribe" ).ToIntCollection();

                    for( Int32 n = 0; n < _MailReportTree.getChildNodeCount(); n++ )
                    {
                        _MailReportTree.goToNthChild( n );

                        CswNbtNode ThisNode = _MailReportTree.getNodeForCurrentPosition();
                        CswNbtObjClassMailReport MailReportNode = CswNbtNodeCaster.AsMailReport( ThisNode );
                        if( CheckedMailReportIds.Contains( MailReportNode.NodeId.PrimaryKey ) )
                        {
                            // subscribe!  (this will do nothing if we're already subscribed)
                            MailReportNode.Recipients.AddUser( Master.CswNbtResources.CurrentNbtUser.UserId );
                        }
                        else
                        {
                            // unsubscribe!  (this will do nothing if we're already unsubscribed)
                            MailReportNode.Recipients.RemoveUser( Master.CswNbtResources.CurrentNbtUser.UserId );
                        }
                        MailReportNode.postChanges( false );
                        _MailReportTree.goToParentNode();
                    }
                }

                // Commit any transactions
                Master.CswNbtResources.finalize();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }//SaveButton_Click()
    }
}
