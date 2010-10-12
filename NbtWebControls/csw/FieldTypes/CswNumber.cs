using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.NbtWebControls;
using ChemSW.CswWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswNumber : CswFieldTypeWebControl, INamingContainer
    {
        public CswNumber( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswNumber_DataBinding);
            EnsureChildControls();
        }

        private void CswNumber_DataBinding( object sender, EventArgs e )
        {
            if( Prop != null )
            {
                _Precision = Prop.AsNumber.Precision;
                _MinValue = Prop.AsNumber.MinValue;
                _MaxValue = Prop.AsNumber.MaxValue;

                if( Double.IsNaN( Prop.AsNumber.Value ) )
                    Text = "";
                else
                    Text = Prop.AsNumber.Value.ToString();
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                //if (_Validator.IsValid)
                //{
                if( Text != String.Empty )
                    Prop.AsNumber.Value = Convert.ToDouble( Text );
                else
                    Prop.AsNumber.Value = Double.NaN;

                //// This only *looks* redundant.  We might change the values on the way in.
                // Changed again in favor of AfterSave(), see BZ 6122
                //if (Double.IsNaN(Prop.AsNumber.Value))
                //    Text = "";
                //else
                //    Text = Prop.AsNumber.Value.ToString();
                //}
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            Text = string.Empty;
        }

        public string Text
        {
            get
            {
                return _TextBox.Text;
            }
            set
            {
                _TextBox.Text = value;
            }
        }

        private Int32 _Precision;
        private double _MinValue;
        private double _MaxValue;

        private TextBox _TextBox;
        private CswInvalidImage _InvalidImg;

        protected override void CreateChildControls()
        {
            _TextBox = new TextBox();
            _TextBox.ID = "_tb";
            _TextBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
            this.Controls.Add(_TextBox);

            _InvalidImg = new CswInvalidImage();
            _InvalidImg.ID = "InvalidImg";
            this.Controls.Add( _InvalidImg );

            base.CreateChildControls();

            if( Required )
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _TextBox.ID;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            string MinValString = string.Empty;
            if (_MinValue != Int32.MinValue)
                MinValString = _MinValue.ToString();
            string MaxValString = string.Empty;
            if (_MaxValue != Int32.MinValue)
                MaxValString = _MaxValue.ToString();

            if( _EditMode != NodeEditMode.LowRes && !ReadOnly )
            {
                _TextBox.Attributes.Add( "onkeypress", "CswNumber_onchange('" + _TextBox.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );
                _TextBox.Attributes.Add( "onchange", "CswNumber_onchange('" + _TextBox.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );
            }
            base.OnPreRender(e);
        }

        
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (ReadOnly)
            {
                writer.Write(this.Text);
            }
            else
            {
                base.RenderControl(writer);
            }
        }

    }
}
