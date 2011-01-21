using System;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswComposite : CswFieldTypeWebControl
    {
        public CswComposite( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswComposite_DataBinding );
        }

        private void CswComposite_DataBinding( object sender, EventArgs e )
        {
            if( Prop != null )
                Text = Prop.AsComposite.CachedValue;
        }

        public override void Save()
        {
            // No saving!
        }
        public override void AfterSave()
        {
        }
        public override void Clear()
        {
            Text = string.Empty;
        }

        public string Text
        {
            get
            {
                EnsureChildControls();
                return _Label.Text;
            }
            set
            {
                EnsureChildControls();
                _Label.Text = value;
            }
        }

        private Label _Label;
        protected override void CreateChildControls()
        {
            _Label = new Label();
            _Label.ID = "Label";
            _Label.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            this.Controls.Add( _Label );

            base.CreateChildControls();
        }

        protected override void OnPreRender( EventArgs e )
        {
            _Label.Text += "&nbsp;";
            base.OnPreRender( e );
        }

    }
}
