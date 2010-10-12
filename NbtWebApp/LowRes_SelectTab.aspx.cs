using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls;
using ChemSW.NbtWebControls.FieldTypes;

namespace ChemSW.Nbt.WebPages
{
    public partial class LowRes_SelectTab : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();

            if( Master.SelectedViewId != Int32.MinValue )
            {
                if( Master.SelectedNodeKey != null )
                {
                    if( Master.SelectedNodeType.NodeTypeTabs.Count == 1 )
                    {
                        // Don't make them pick if there's only one
                        Master.Redirect( Master.MakeLink( "LowRes_Props.aspx", Int32.MinValue, string.Empty, Master.SelectedNodeType.getFirstNodeTypeTab().TabId ) );
                    }
                    else
                    {
                        foreach( CswNbtMetaDataNodeTypeTab Tab in Master.SelectedNodeType.NodeTypeTabs )
                        {
                            HyperLink TabLink = new HyperLink();
                            TabLink.NavigateUrl = Master.MakeLink( "LowRes_Props.aspx", Int32.MinValue, string.Empty, Tab.TabId );
                            TabLink.Text = Tab.TabName;

                            if( Master.SelectedNodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass )
                            {
                                Int32 PropertyCount = 0;
                                Int32 AnsweredPropertyCount = 0;
                                foreach( CswNbtMetaDataNodeTypeProp Prop in Tab.NodeTypeProps )
                                {
                                    if( !Master.SelectedNode.Properties[Prop].Empty )
                                        AnsweredPropertyCount++;
                                    PropertyCount++;
                                }
                                TabLink.Text += " (" + AnsweredPropertyCount.ToString() + "/" + PropertyCount.ToString() + ")";
                            }

                            if( Tab.TabId == Master.SelectedTabId )
                                TabLink.Style.Add( HtmlTextWriterStyle.BackgroundColor, "#ffff00" );
                            _LinksPH.Controls.Add( TabLink );
                            _LinksPH.Controls.Add( new CswLiteralBr() );
                        }
                    }
                }
                else
                {
                    Master.Redirect( Master.MakeLink( "LowRes_SelectNode.aspx" ) );
                }
            }
            else
            {
                Master.Redirect( Master.MakeLink( "LowRes_SelectView.aspx" ) );
            }


            base.OnInit( e );
        }

        private Label _PageLabel;
        private Button _BackButton;
        private PlaceHolder _LinksPH;

        protected override void CreateChildControls()
        {
            Literal NodeLiteral = new Literal();
            NodeLiteral.Text = "Node: ";
            ph.Controls.Add( NodeLiteral );

            Literal NodeNameLiteral = new Literal();
            NodeNameLiteral.Text = Master.SelectedNode.NodeName;
            ph.Controls.Add( NodeNameLiteral );

            ph.Controls.Add( new CswLiteralBr() );
            ph.Controls.Add( new CswLiteralBr() );

            _PageLabel = new Label();
            _PageLabel.Text = "Select a Tab:";
            _PageLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            ph.Controls.Add( _PageLabel );

            ph.Controls.Add( new CswLiteralBr() );

            _LinksPH = new PlaceHolder();
            _LinksPH.ID = "LinksPH";
            ph.Controls.Add( _LinksPH );

            ph.Controls.Add( new CswLiteralBr() );

            _BackButton = new Button();
            _BackButton.ID = "BackButton";
            _BackButton.CssClass = "Button";
            _BackButton.Text = "Back";
            _BackButton.Click += new EventHandler( _BackButton_Click );
            ph.Controls.Add( _BackButton );

            base.CreateChildControls();
        }

        void _BackButton_Click( object sender, EventArgs e )
        {
            Master.Redirect( Master.MakeLink( "LowRes_SelectNode.aspx" ) );
        }

    }
}
