using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswMTBF : CswFieldTypeWebControl, INamingContainer
    {
        public CswMTBF( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswMTBF_DataBinding );
            EnsureChildControls();
        }

        double _hours = 0;
        double _days = 0;
        private void CswMTBF_DataBinding( object sender, EventArgs e )
        {
            _StartDatePicker.Required = Required;
            if( Prop != null && !Prop.Empty )
            {
                _StartDatePicker.SelectedDate = Convert.ToDateTime( Prop.AsMTBF.StartDateTime );
                // We don't save the end date, so that by default it always shows the value to Now.
                //_EndDatePicker.SelectedDate = Convert.ToDateTime( Prop.AsMTBF.EndDateTime );
                if( Prop.AsMTBF.Units != String.Empty )
                    _UnitSelector.SelectedValue = Prop.AsMTBF.Units;

                // BZ 10310 -- this needs to be here in order for the cached value to save
                Prop.AsMTBF.CalculateMTBF( _StartDatePicker.SelectedDate, _EndDatePicker.SelectedDate, ref _hours, ref _days );
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                Prop.AsMTBF.StartDateTime = _StartDatePicker.SelectedDate;
                Prop.AsMTBF.Units = _UnitSelector.SelectedValue;
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            _MTBFValue.Text = string.Empty;
            _StartDatePicker.Clear();
            _EndDatePicker.Clear();
        }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
        }

        private CswDatePicker _StartDatePicker;
        private CswDatePicker _EndDatePicker;
        //private CswTimePicker _CswTimePicker;
        private Label _UnitLabel;
        private Label _MTBFValue;
        private Label _StartDateLabel;
        private Label _EndDateLabel;
        //private Label _TimeLabel;
        private DropDownList _UnitSelector;
        //private CswHiddenTable _HiddenTable;
        private CswAutoTable _EditTable;
        private CswImageButton _EditButton;
        private Button _RefreshButton;

        protected override void CreateChildControls()
        {
            CswAutoTable TopTable = new CswAutoTable();
            this.Controls.Add( TopTable );

            _MTBFValue = new Label();
            _MTBFValue.ID = "mtbfvalue";
            _MTBFValue.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            TopTable.addControl( 0, 0, _MTBFValue );

            _EditButton = new CswImageButton( CswImageButton.ButtonType.Edit );
            _EditButton.ID = "edit";
            TopTable.addControl( 0, 1, _EditButton );

            _EditTable = new CswAutoTable();
            this.Controls.Add( _EditTable );

            _UnitLabel = new Label();
            _UnitLabel.ID = "unitlabel";
            _UnitLabel.Text = "Units";
            _EditTable.addControl( 0, 0, _UnitLabel );

            _UnitSelector = new DropDownList();
            _UnitSelector.ID = "unit";
            _UnitSelector.CssClass = CswFieldTypeWebControl.DropDownCssClass;
            _UnitSelector.Items.Add( new ListItem( "hours", "hours" ) );
            _UnitSelector.Items.Add( new ListItem( "days", "days" ) );
            _EditTable.addControl( 0, 1, _UnitSelector );

            _StartDateLabel = new Label();
            _StartDateLabel.ID = "startdatelabel";
            _StartDateLabel.Text = "Start Date";
            _EditTable.addControl( 1, 0, _StartDateLabel );

            _StartDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, true );
            _StartDatePicker.ID = "startdatepicker";
            _StartDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            _EditTable.addControl( 1, 1, _StartDatePicker );

            _EndDateLabel = new Label();
            _EndDateLabel.ID = "enddatelabel";
            _EndDateLabel.Text = "End Date";
            _EditTable.addControl( 2, 0, _EndDateLabel );

            _EndDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, true );
            _EndDatePicker.ID = "enddatepicker";
            _EndDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            _EditTable.addControl( 2, 1, _EndDatePicker );

            _RefreshButton = new Button();
            _RefreshButton.ID = "refreshbutton";
            _RefreshButton.Text = "Refresh";
            _RefreshButton.Click += new EventHandler( _RefreshButton_Click );
            _EditTable.addControl( 2, 2, _RefreshButton );

            base.CreateChildControls();
        }

        void _RefreshButton_Click( object sender, EventArgs e )
        {
            // BZ 10310 -- this needs to be here in order for the Refresh button to work
            Prop.AsMTBF.CalculateMTBF( _StartDatePicker.SelectedDate, _EndDatePicker.SelectedDate, ref _hours, ref _days );
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( Prop != null && Prop.NodeId != null )
                _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";

            if( _UnitSelector.SelectedValue == "hours" )
            {
                _MTBFValue.Text = _hours.ToString() + " hours ";
            }
            else
            {
                _MTBFValue.Text = _days.ToString() + " days ";
            }

            _MTBFValue.Text += "&nbsp;";
            _UnitSelector.Attributes.Add( "onchange", "CswMTBF_toggleUnit('" + _MTBFValue.ClientID + "', '" + _hours.ToString() + "', '" + _days.ToString() + "', this.value);" );

            _EditTable.Visible = false;
            _EditButton.Visible = true;
            if( _EditMode == NodeEditMode.EditInPopup )
            {
                _EditTable.Visible = true;
                _EditButton.Visible = false;
            }
            if( ReadOnly )
                _EditButton.Visible = false;

            base.OnPreRender( e );
        }

        public override void RenderControl( HtmlTextWriter writer )
        {
            if( ReadOnly )
            {
                _StartDatePicker.Enabled = false;
                _EndDatePicker.Enabled = false;
            }
            base.RenderControl( writer );
        }
    }
}
