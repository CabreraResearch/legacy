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
using System.Web.UI.HtmlControls;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswStatic : CswFieldTypeWebControl
    {
        public CswStatic( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswStatic_DataBinding);
        }

        private void CswStatic_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _ScrollingLabel.Text = Prop.AsStatic.StaticText;
                _ScrollingLabel.WidthInChars = Prop.AsStatic.Columns;
                _ScrollingLabel.HeightInChars = Prop.AsStatic.Rows;
            }
        }

        public override void Save()
        {
            // No saving!
            //_PropAsStatic.Value = Text;
        }
        public override void AfterSave()
        {
        }
        public override void Clear()
        {
            _ScrollingLabel.Text = string.Empty;
        }

        private CswScrollingLabel _ScrollingLabel;
        protected override void CreateChildControls()
        {
            _ScrollingLabel = new CswScrollingLabel();
            _ScrollingLabel.ID = "staticlbl";
            this.Controls.Add(_ScrollingLabel);

            base.CreateChildControls();
        }
    }
}
