using System;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswStatic : CswFieldTypeWebControl
    {
        public CswStatic( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswEnumNbtNodeEditMode EditMode )
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
