using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswQuantity : CswFieldTypeWebControl, INamingContainer
    {
        public CswQuantity( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler(CswQuantity_DataBinding);
        }

        private void CswQuantity_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _UnitList.Items.Clear();
                ListItem ListItemNone = new ListItem( "", "" );
                _UnitList.Items.Add( ListItemNone );
                foreach( CswNbtNode UnitNode in Prop.AsQuantity.UnitNodes )
                {
                    string Unit = UnitNode.Properties[CswNbtObjClassUnitOfMeasure.NamePropertyName].AsText.Text;
                    ListItem UnitItem = new ListItem( Unit, Unit );
                    _UnitList.Items.Add( UnitItem );
                }

                Quantity = Prop.AsQuantity.Quantity;
                Units = Prop.AsQuantity.Units;
                _Precision = Prop.AsQuantity.Precision;
                _MinValue = Prop.AsQuantity.MinValue;
                _MaxValue = Prop.AsQuantity.MaxValue;
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                Prop.AsQuantity.Quantity = Quantity;
                Prop.AsQuantity.Units = Units;

                //if (_Validator.IsValid)
                //{
                // This only *looks* redundant.  We might change the values on the way in.
                // Changed for BZ 6122
                //Quantity = Prop.AsQuantity.Quantity;
                //Units = Prop.AsQuantity.Units;
                //}
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            Quantity = Int32.MinValue;
            _UnitList.SelectedValue = string.Empty;
        }

        public double Quantity
        {
            get
            {
                double q = Double.NaN;
                if (CswTools.IsFloat(_QuantityTextBox.Text))
                    q = Convert.ToDouble(_QuantityTextBox.Text);
                return q;
            }

            set
            {
                if (Double.IsNaN(value))
                    _QuantityTextBox.Text = string.Empty;
                else
                    _QuantityTextBox.Text = value.ToString();
            }
        }
        
        public string Units
        {
            get { return _UnitList.SelectedValue; }
            set
            {
                if (null == _UnitList.Items.FindByValue(value))
                {
                    // Add it!  This guarantees we see the data that was saved, even if the options change
                    ListItem SelectedItem = new ListItem(value, value);
                    _UnitList.Items.Add(SelectedItem);
                }
                _UnitList.SelectedValue = value;
            }
        }

        private Int32 _Precision;
        private double _MinValue;
        private double _MaxValue;

        private TextBox _QuantityTextBox = new TextBox();
        private DropDownList _UnitList = new DropDownList();
        private CswInvalidImage _InvalidImg;

        protected override void CreateChildControls()
        {
            _QuantityTextBox.ID = "qty";
            _QuantityTextBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
            _QuantityTextBox.Width = 60;
            //if (!Double.IsNaN(Quantity))
            //    _QuantityTextBox.Text = Quantity.ToString();
            //else
            //    _QuantityTextBox.Text = "";
            this.Controls.Add(_QuantityTextBox);
            
            _UnitList.ID = "u";
            _UnitList.CssClass = CswFieldTypeWebControl.DropDownCssClass;
            this.Controls.Add( _UnitList );

            _InvalidImg = new CswInvalidImage();
            _InvalidImg.ID = "InvalidImg";
            this.Controls.Add( _InvalidImg );

            base.CreateChildControls();

            if( Required )
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _QuantityTextBox.ID;
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            string MinValString = string.Empty;
            if( _MinValue != Int32.MinValue )
                MinValString = _MinValue.ToString();
            string MaxValString = string.Empty;
            if( _MaxValue != Int32.MinValue )
                MaxValString = _MaxValue.ToString();

            _QuantityTextBox.Attributes.Add( "onkeypress", "CswQuantity_onchange('" + _QuantityTextBox.ClientID + "', '" + _UnitList.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );
            _QuantityTextBox.Attributes.Add( "onchange", "CswQuantity_onchange('" + _QuantityTextBox.ClientID + "', '" + _UnitList.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );
            _UnitList.Attributes.Add( "onchange", "CswQuantity_onchange('" + _QuantityTextBox.ClientID + "', '" + _UnitList.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );

            base.OnPreRender( e );
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            EnsureChildControls();
            
            if (ReadOnly)
            {
                output.Write(Quantity);
                output.Write("&nbsp;");
                output.Write(Units);
            }
            else
            {
                base.RenderContents(output);
            }
        }
    }
}

