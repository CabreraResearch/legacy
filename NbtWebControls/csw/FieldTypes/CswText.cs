using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    /// <summary>
    /// Fieldtype: Text
    /// </summary>
    public class CswText : CswFieldTypeWebControl, INamingContainer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CswText( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswText_DataBinding );
        }

        private void CswText_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                Text = Prop.AsText.Text;
                _TextBox.Columns = Prop.AsText.Size;
            }
        }

        /// <summary>
        /// Save the value of this control to the DB
        /// </summary>
        public override void Save()
        {
            if( !ReadOnly )
                Prop.AsText.Text = Text;
        }
        /// <summary>
        /// Event that occurs after save
        /// </summary>
        public override void AfterSave()
        {
            DataBind();
        }
        /// <summary>
        /// Clears the value displayed in the textbox (not in the database)
        /// </summary>
        public override void Clear()
        {
            Text = string.Empty;
        }


        /// <summary>
        /// Value of property
        /// </summary>
        public string Text
        {
            get
            {
                EnsureChildControls();
                return _TextBox.Text;
            }
            set
            {
                EnsureChildControls();
                _TextBox.Text = value;
            }
        }

        /// <summary>
        /// AutoPostBack setting of TextBox
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                EnsureChildControls();
                return _TextBox.AutoPostBack;
            }
            set
            {
                EnsureChildControls();
                _TextBox.AutoPostBack = value;
            }
        }

        private TextBox _TextBox;

        /// <summary>
        /// Create the TextBox control
        /// </summary>
        protected override void CreateChildControls()
        {
            _TextBox = new TextBox();
            _TextBox.ID = "text_" + _CswNbtMetaDataNodeTypeProp.PropId.ToString();
            _TextBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
            this.Controls.Add( _TextBox );

            base.CreateChildControls();

            if( Required )
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _TextBox.ID;
            }
        }

        /// <summary>
        /// PreRender event
        /// </summary>
        protected override void OnPreRender( EventArgs e )
        {
            _TextBox.Attributes.Add( "onkeypress", "CswFieldTypeWebControl_onchange();" );
            _TextBox.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
            base.OnPreRender( e );
        }

        /// <summary>
        /// Render event
        /// </summary>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if( ReadOnly )
            {
                writer.Write( this.Text );
            }
            else
            {
                base.RenderControl( writer );
            }
        }
    }
}
