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
    public partial class LowRes_Props : System.Web.UI.Page
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
                        // BZ 8786
                        Int32 PropCount = Master.SelectedTab.NodeTypePropsByDisplayOrder.Count;
                        if( PropCount > 0 && ( PropCount >= 10 || Master.SelectedTab.HasConditionalProps ) )
                        {
                            Master.Redirect( Master.MakeLink( "LowRes_Prop.aspx", Int32.MinValue, string.Empty, Int32.MinValue, Master.SelectedTab.FirstPropByDisplayOrder().PropId ) );
                        }
                        else
                        {
                            foreach( CswNbtMetaDataNodeTypeProp Prop in Master.SelectedTab.NodeTypePropsByDisplayOrder )
                            {
                                if( Prop.ShowLabel )
                                {
                                    Label PropLabel = new Label();
                                    PropLabel.Text = Prop.PropNameWithQuestionNo;
                                    PropLabel.Style.Add( HtmlTextWriterStyle.FontWeight, "bold" );
                                    _LinksPH.Controls.Add( PropLabel );
                                }
                                _LinksPH.Controls.Add( new CswLiteralBr() );

                                CswFieldTypeWebControl Control = CswFieldTypeWebControlFactory.makeControl( Master.CswNbtResources,
                                                                                                            _LinksPH.Controls,
                                                                                                            "prop" + Prop.PropId.ToString(),
                                                                                                            Master.SelectedNode.Properties[Prop],
                                                                                                            NodeEditMode.LowRes,
                                                                                                            Master.HandleError );
                                if( !Prop.FieldType.IsSimpleType() )
                                    Control.ReadOnly = true;
                                Control.DataBind();

                                _LinksPH.Controls.Add( new CswLiteralBr() );
                                _LinksPH.Controls.Add( new CswLiteralBr() );
                            }
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
        private Button _SaveButton;
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

            _SaveButton = new Button();
            _SaveButton.ID = "SaveButton";
            _SaveButton.CssClass = "Button";
            _SaveButton.Text = "Save";
            _SaveButton.Click += new EventHandler( _SaveButton_Click );
            ph.Controls.Add( _SaveButton );

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
            _GoBack();
        }

        void _SaveButton_Click( object sender, EventArgs e )
        {
            CswPropertyTable.SaveFieldTypeWebControls( _LinksPH.Controls );
            Master.SelectedNode.postChanges( false );
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
    }
}