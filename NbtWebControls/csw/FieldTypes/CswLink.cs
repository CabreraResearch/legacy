using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswLink : CswFieldTypeWebControl, INamingContainer
    {
        public CswLink( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswLink_DataBinding );
        }

        private void CswLink_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _HyperLink.Text = Prop.AsLink.Text;
                _HyperLink.NavigateUrl = Prop.AsLink.Href;
                _TextBox.Text = Prop.AsLink.Text;
                _HrefBox.Text = Prop.AsLink.Href;
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                EnsureChildControls();
                Prop.AsLink.Href = _HrefBox.Text;
                if( _TextBox.Text != String.Empty )
                    Prop.AsLink.Text = _TextBox.Text;
                else
                    Prop.AsLink.Text = _HrefBox.Text;
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            _TextBox.Text = string.Empty;
            _HrefBox.Text = string.Empty;
            _HyperLink.Text = string.Empty;
        }

        private HyperLink _HyperLink = null;
        private TextBox _TextBox = null;
        private TextBox _HrefBox = null;
        private CswAutoTable _SubTable = null;
        private CswHiddenTable _HiddenTable;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _HyperLink = new HyperLink();
            _HyperLink.ID = "link";
            _HyperLink.Target = "_blank";
            Table.addControl( 0, 0, _HyperLink );

            _HiddenTable = new CswHiddenTable();
            _HiddenTable.ID = "table";
            Table.addControl( 0, 0, _HiddenTable );

            _SubTable = _HiddenTable.Table;
            _SubTable.ID = "subtable";
            _SubTable.getCell( 0, 0 ).HorizontalAlign = HorizontalAlign.Right;
            _SubTable.getCell( 1, 0 ).HorizontalAlign = HorizontalAlign.Right;

            Label TextLabel = new Label();
            TextLabel.ID = "textlabel";
            TextLabel.Text = "Text: ";
            _SubTable.addControl( 0, 0, TextLabel );

            _TextBox = new TextBox();
            _TextBox.CssClass = "textinput";
            _TextBox.ID = "text";
            _TextBox.Columns = 40;
            _SubTable.addControl( 0, 1, _TextBox );

            Label HrefLabel = new Label();
            HrefLabel.ID = "hreflabel";
            HrefLabel.Text = "URL: ";
            _SubTable.addControl( 1, 0, HrefLabel );

            _HrefBox = new TextBox();
            _HrefBox.CssClass = "textinput";
            _HrefBox.ID = "href";
            _HrefBox.Columns = 40;
            _SubTable.addControl( 1, 1, _HrefBox );

            base.CreateChildControls();

            if( Required )
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _HrefBox.ID;
                _SubTable.addControl( 1, 1, _RequiredValidator );   // BZ 7959
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            _TextBox.Attributes.Add( "onkeypress", "CswFieldTypeWebControl_onchange()" );
            _TextBox.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange()" );
            _HrefBox.Attributes.Add( "onkeypress", "CswFieldTypeWebControl_onchange()" );
            _HrefBox.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange()" );

            if( _EditMode == NodeEditMode.AddInPopup || ( Required && Prop != null && Prop.AsLink.Href == string.Empty ) )   // BZ 7959
                _HiddenTable.Expanded = true;

            if( ReadOnly || _EditMode == NodeEditMode.PrintReport )
            {
                _TextBox.Visible = false;
                _HrefBox.Visible = false;
                _HiddenTable.Visible = false;
            }


            base.OnPreRender( e );
        }
    }
}
