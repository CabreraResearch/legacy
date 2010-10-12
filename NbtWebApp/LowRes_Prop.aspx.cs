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
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebPages
{
    public partial class LowRes_Prop : System.Web.UI.Page
    {
        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();

            if( Master.SelectedViewId != Int32.MinValue )
            {
                if( Master.SelectedNodeKey != null )
                {
                    if( Master.SelectedTabId != Int32.MinValue )
                    {
                        if( Master.SelectedPropId != Int32.MinValue )
                        {
                            if( Master.SelectedProp.hasFilter() && !Master.SelectedProp.CheckFilter( Master.SelectedNode ) )
                            {
                                if( Request.QueryString["prev"] == "1" )
                                    _GoPrev();
                                else
                                    _GoNext();
                            }
                            if( Master.SelectedProp.ShowLabel )
                            {
                                Label PropLabel = new Label();
                                PropLabel.Text = Master.SelectedProp.PropNameWithQuestionNo;
                                PropLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
                                _LinksPH.Controls.Add( PropLabel );
                            }
                            _LinksPH.Controls.Add( new CswLiteralBr() );

                            CswFieldTypeWebControl Control = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources,
                                                                                                        _LinksPH.Controls,
                                                                                                        "prop" + Master.SelectedProp.PropId.ToString(),
                                                                                                        Master.SelectedNode.Properties[Master.SelectedProp],
                                                                                                        NodeEditMode.LowRes,
                                                                                                        Master.HandleError );
                            if( !Master.SelectedProp.FieldType.IsSimpleType() )
                                Control.ReadOnly = true;
                            Control.DataBind();

                            _LinksPH.Controls.Add( new CswLiteralBr() );
                            _LinksPH.Controls.Add( new CswLiteralBr() );
                        }
                        else
                        {
                            Master.Redirect( Master.MakeLink( "LowRes_Props.aspx" ) );
                        }
                    }
                    else
                    {
                        Master.Redirect( Master.MakeLink( "LowRes_SelectTab.aspx" ) );
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
        private Button _PrevButton;
        private Button _NextButton;
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

            Literal TabLiteral = new Literal();
            TabLiteral.Text = "Tab: ";
            ph.Controls.Add( TabLiteral );

            Literal TabNameLiteral = new Literal();
            TabNameLiteral.Text = Master.SelectedTab.TabName;
            ph.Controls.Add( TabNameLiteral );

            ph.Controls.Add( new CswLiteralBr() );
            ph.Controls.Add( new CswLiteralBr() );
            
            _PageLabel = new Label();
            _PageLabel.Text = "Properties:";
            _PageLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
            ph.Controls.Add( _PageLabel );

            ph.Controls.Add( new CswLiteralBr() );

            ph.Controls.Add( new CswLiteralBr() );

            _LinksPH = new PlaceHolder();
            _LinksPH.ID = "LinksPH";
            ph.Controls.Add( _LinksPH );

            ph.Controls.Add( new CswLiteralBr() );

            _PrevButton = new Button();
            _PrevButton.ID = "PrevButton";
            _PrevButton.CssClass = "Button";
            _PrevButton.Text = "Previous";
            _PrevButton.Click += new EventHandler( _PrevButton_Click );
            ph.Controls.Add( _PrevButton );

            _NextButton = new Button();
            _NextButton.ID = "NextButton";
            _NextButton.CssClass = "Button";
            _NextButton.Text = "Next";
            _NextButton.Click += new EventHandler( _NextButton_Click );
            ph.Controls.Add( _NextButton );

            ph.Controls.Add( new CswLiteralBr() );

            _BackButton = new Button();
            _BackButton.ID = "BackButton";
            _BackButton.CssClass = "Button";
            _BackButton.Text = "Back";
            _BackButton.Click += new EventHandler( _BackButton_Click );
            ph.Controls.Add( _BackButton );

            base.CreateChildControls();
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( Master.SelectedTab.FirstPropByDisplayOrder() == Master.SelectedProp )
                _PrevButton.Visible = false;
            else
                _PrevButton.Visible = true;

            if( Master.SelectedTab.getNextPropByDisplayOrder( Master.SelectedProp ) == null )
                _NextButton.Text = "Save";
            else
                _NextButton.Text = "Next";
            base.OnPreRender( e );
        }

        void _BackButton_Click( object sender, EventArgs e )
        {
            _GoBack();
        }
        private void _GoBack()
        {
            // If there's only one tab, go back to Select Node instead
            if( Master.SelectedNodeType.NodeTypeTabs.Count == 1 )
                Master.Redirect( Master.MakeLink( "LowRes_SelectNode.aspx" ) );
            else
                Master.Redirect( Master.MakeLink( "LowRes_SelectTab.aspx" ) );
        }

        void _PrevButton_Click( object sender, EventArgs e )
        {
            _GoPrev();
        }

        private void _GoPrev()
        {
            CswNbtMetaDataNodeTypeProp PrevProp = Master.SelectedTab.getPreviousPropByDisplayOrder( Master.SelectedProp );
            Master.Redirect( Master.MakeLink( "LowRes_Prop.aspx?prev=1", Int32.MinValue, string.Empty, Int32.MinValue, PrevProp.PropId ) );
        }
        
        void _NextButton_Click( object sender, EventArgs e )
        {
            CswPropertyTable.SaveFieldTypeWebControls( _LinksPH.Controls );
            Master.SelectedNode.postChanges( false );
            _GoNext();
        }
        
        private void _GoNext()
        {
            CswNbtMetaDataNodeTypeProp NextProp = Master.SelectedTab.getNextPropByDisplayOrder( Master.SelectedProp );
            if( NextProp != null )
                Master.Redirect( Master.MakeLink( "LowRes_Prop.aspx", Int32.MinValue, string.Empty, Int32.MinValue, NextProp.PropId ) );
            else
                _GoBack();
        }
    }
}