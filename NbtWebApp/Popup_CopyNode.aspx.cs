using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.Actions;
using ChemSW.Exceptions;
using ChemSW.CswWebControls;

namespace ChemSW.Nbt.WebPages
{
    public partial class Popup_CopyNode : System.Web.UI.Page
    {
        private CswNbtActCopyNode _CswNbtActCopyNode;

        protected override void OnInit( EventArgs e )
        {
            try
            {
                if( Request.QueryString["nodekey"] != null )
                {
                    // Copy from nodekey
                    _NodeKey = new CswNbtNodeKey( Master.CswNbtResources, Request.QueryString["nodekey"].ToString() );
                    _Node = Master.CswNbtResources.Nodes[_NodeKey.NodeId];
                }

                EnsureChildControls();

                _Message1.Text = "Copying " + _Node.NodeType.NodeTypeName + ": " + _Node.NodeName + "";
                switch( _Node.ObjectClass.ObjectClass )
                {
                    case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass:
                        _Message2.Text = "All Schedules assigned to this Equipment will be copied.  New barcodes will be assigned.";
                        break;
                    case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass:
                        _Message2.Text = "All Equipment assigned to this Assembly (and their Schedules) will be copied.  New barcodes will be assigned.";
                        break;
                    default:
                        _Message2.Text = "";
                        break;
                }

                _CswNbtActCopyNode = new CswNbtActCopyNode( Master.CswNbtResources );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        private CswNbtNodeKey _NodeKey;
        private CswNbtNode _Node;
        private Literal _Message1;
        private Literal _Message2;
        private Button _CopyButton;
        private Button _CancelButton;
        private CswAutoTable _Table;

        protected override void CreateChildControls()
        {
            _Table = new CswAutoTable();
            _Table.ID = "table";
            ph.Controls.Add( _Table );

            _Message1 = new Literal();
            _Table.addControl( 0, 0, _Message1 );

            _Table.addControl( 1, 0, new CswLiteralNbsp() );

            _Message2 = new Literal();
            _Table.addControl( 2, 0, _Message2 );

            _Table.addControl( 3, 0, new CswLiteralNbsp() );

            _CopyButton = new Button();
            _CopyButton.ID = "CopyButton";
            _CopyButton.Text = "Copy";
            _CopyButton.CssClass = "Button";
            _CopyButton.Click += new EventHandler( _CopyButton_Click );
            _Table.addControl( 4, 0, _CopyButton );

            _CancelButton = new Button();
            _CancelButton.ID = "CancelButton";
            _CancelButton.CssClass = "Button";
            _CancelButton.Text = "Cancel";
            _CancelButton.OnClientClick = "Popup_Cancel_Clicked(); return false;";
            _Table.addControl( 4, 0, _CancelButton );

            base.CreateChildControls();
        }

        public void HandleCopyNode( CswNbtNode OldNode, CswNbtNode NewNode )
        {
            try
            {
                Master.HandleCopyNode( OldNode, NewNode );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        }

        void _CopyButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswNbtNode NewNode = null;
                switch( _Node.ObjectClass.ObjectClass )
                {
                    case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass:
                        NewNode = _CswNbtActCopyNode.CopyEquipmentNode( _Node );
                        break;
                    case CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass:
                        NewNode = _CswNbtActCopyNode.CopyEquipmentAssemblyNode( _Node );
                        break;
                    default:
                        NewNode = _CswNbtActCopyNode.CopyNode( _Node );
                        break;
                }

                if( NewNode != null )
                {
                    HandleCopyNode( _Node, NewNode );

                    // Commit any transactions
                    Master.CswNbtResources.finalize();

                    string JS = @"  <script>
                                    Popup_OK_Clicked();
                                </script>";

                    System.Web.UI.ScriptManager.RegisterStartupScript( this, this.GetType(), this.UniqueID + "_JS", JS, false );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // _CopyButton_Click()
    }
}