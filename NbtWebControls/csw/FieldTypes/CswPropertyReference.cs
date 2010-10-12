using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswPropertyReference : CswFieldTypeWebControl
    {
        public CswPropertyReference( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswPropertyReference_DataBinding);
        }

        private void CswPropertyReference_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
                _Label.Text = Prop.AsPropertyReference.CachedValue;
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

        public override void Save()
        {
            // No saving!
        }
        public override void AfterSave()
        {
        }
        public override void Clear()
        {
            _Label.Text = string.Empty;
        }

        protected override void OnPreRender( EventArgs e )
        {
            _Label.Text += "&nbsp;";

            base.OnPreRender( e );
        }
    }
}
