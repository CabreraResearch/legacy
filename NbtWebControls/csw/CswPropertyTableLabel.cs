using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls
{
    /// <summary>
    /// Represents a label for a CswPropertyTable component
    /// </summary>
    public class CswPropertyTableLabel : CompositeControl, INamingContainer
    {
        /// <summary>
        /// Label text to display
        /// </summary>
        public string LabelText = string.Empty;
        /// <summary>
        /// Optional help text to show in a ToolTip
        /// </summary>
        public string ToolTipText = string.Empty;
        /// <summary>
        /// If true, display a checkbox next to the label
        /// </summary>
        public bool EnableCheckbox = false;
        /// <summary>
        /// OnClientClick event for checkbox
        /// </summary>
        public string CheckboxOnClientClick = string.Empty;
        /// <summary>
        /// If false, do not show a label
        /// </summary>
        public bool ShowLabel = true;
        /// <summary>
        /// State of the Layout Table's EditMode
        /// </summary>
        public bool EditMode = false;

        /// <summary>
        /// Returns true if the checkbox is checked
        /// </summary>
        public bool Checked
        {
            get { return _Check.Checked; }
        }

        protected override void OnInit( EventArgs e )
        {
            EnsureChildControls();
            base.OnInit( e );
        }

        private Label _Label;
        private LinkButton _LinkButton;
        private RadToolTip _ToolTip;
        private CheckBox _Check;

        protected override void CreateChildControls()
        {
            _LinkButton = new LinkButton();
            _LinkButton.ID = "proplabel_linkbutton";
            _LinkButton.CssClass = "propertylabel";
            _LinkButton.OnClientClick = "return false;";
            this.Controls.Add( _LinkButton );

            _Label = new Label();
            _Label.ID = "proplabel_label";
            _Label.CssClass = "propertylabel";
            this.Controls.Add( _Label );

            _ToolTip = new RadToolTip();
            _ToolTip.ID = "proplabel_tooltip";
            _ToolTip.EnableEmbeddedSkins = false;
            _ToolTip.Skin = "ChemSW";
            this.Controls.Add( _ToolTip );

            _Check = new CheckBox();
            _Check.ID = "proplabel_checkbox";
            this.Controls.Add( _Check );

            this.Controls.Add( new CswLiteralBr() );

            base.CreateChildControls();
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( ShowLabel || EditMode )
            {
                _Label.Text = LabelText;
                _LinkButton.Text = LabelText;

                if( CheckboxOnClientClick != string.Empty )
                    _Check.Attributes.Add( "onclick", CheckboxOnClientClick );

                if( ToolTipText != string.Empty )
                {
                    _Label.Visible = false;
                    _LinkButton.Visible = true;
                    _ToolTip.Visible = true;

                    _ToolTip.TargetControlID = _LinkButton.ID;
                    _ToolTip.Title = LabelText;
                    _ToolTip.Text = ToolTipText;
                    _ToolTip.ShowEvent = ToolTipShowEvent.OnClick;
                    _ToolTip.AutoCloseDelay = 10000;
                }
                else
                {
                    _LinkButton.Visible = false;
                    _ToolTip.Visible = false;
                    _Label.Visible = true;
                }

                if( EnableCheckbox )
                    _Check.Visible = true;
                else
                    _Check.Visible = false;
            } // if( ShowLabel )
            else
            {
                this.Visible = false;
            }
            base.OnPreRender( e );
        }

    }
}
