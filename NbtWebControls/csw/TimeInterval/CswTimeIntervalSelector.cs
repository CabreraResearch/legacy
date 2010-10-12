using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls
{
    public class CswTimeIntervalSelector : CompositeControl, IPostBackDataHandler
    {
        //private string WeeklyDayPickerRadioGroupName = "_weeklyday";
        //private string MonthlyDayPickerRadioGroupName = "_monthlyday";
        private bool _UseCheckChanges = true;
        public bool ReadOnly = false;

        public CswTimeIntervalSelector(bool UseCheckChanges)
        {
            DataBinding += new EventHandler( CswTimeIntervalSelector_DataBinding );
            _UseCheckChanges = UseCheckChanges;
        }


        private CswRateInterval _RateInterval = new CswRateInterval();
        public CswRateInterval RateInterval
        {
            get { return _RateInterval; }
            set { _RateInterval = value; }
        }

        private void CswTimeIntervalSelector_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            
            //DatePicker.Required = false;

            if( _RateInterval.RateType == CswRateInterval.RateIntervalType.YearlyByDate )
            {
                RateButtonYearly.Checked = true;
            }
            if( _RateInterval.RateType == CswRateInterval.RateIntervalType.MonthlyByWeekAndDay )
            {
                RateButtonMonthly.Checked = true;
                //MonthlyByWeekAndDay.Checked = true;
                //MonthFrequencySelect.SelectedValue = ( (CswRateIntervalMonthlyWeek) _RateInterval.RateInterval ).MonthlyFrequency.ToString();
                //if( ( (CswRateIntervalMonthlyWeek) _RateInterval.RateInterval ).MonthlyWeek != Int32.MinValue )
                //    MonthWeekSelect.SelectedValue = ( (CswRateIntervalMonthlyWeek) _RateInterval.RateInterval ).MonthlyWeek.ToString();
                //setWeekDayPickerRadioSelectedValue( MonthlyDayPickerTable, MonthlyDayPickerRadioGroupName, ( (CswRateIntervalMonthlyWeek) _RateInterval.RateInterval ).MonthlyDay );
            }
            if( _RateInterval.RateType == CswRateInterval.RateIntervalType.MonthlyByDate )
            {
                RateButtonMonthly.Checked = true;
                //MonthlyByDate.Checked = true;
                //MonthFrequencySelect.SelectedValue = ( (CswRateIntervalMonthlyDate) _RateInterval.RateInterval ).MonthlyFrequency.ToString();
                //if( ( (CswRateIntervalMonthlyDate) _RateInterval.RateInterval ).MonthlyDate != Int32.MinValue )
                //    MonthDateSelect.SelectedValue = ( (CswRateIntervalMonthlyDate) _RateInterval.RateInterval ).MonthlyDate.ToString();
            }
            if( _RateInterval.RateType == CswRateInterval.RateIntervalType.WeeklyByDay )
            {
                RateButtonWeekly.Checked = true;
                //setWeekDayPickerCheckBoxSelectedValues( WeeklyDayPickerTable, WeeklyDayPickerRadioGroupName, ( (CswRateIntervalWeekly) _RateInterval.RateInterval ).WeeklyDays );
            }

            //if( ( (CswRateIntervalYearly) _RateInterval.RateInterval ).YearlyDate != DateTime.MinValue )
            //    DatePicker.SelectedDate = ( (CswRateIntervalYearly) _RateInterval.RateInterval ).YearlyDate;
        }


        //IPostBackDataHandler
        public bool LoadPostData( String postDataKey, NameValueCollection values )
        {
            EnsureChildControls();

            // Rate setting
            if( values[this.UniqueID + "$" + RateButtonWeekly.GroupName] != null )
            {
                if( values[this.UniqueID + "$" + RateButtonWeekly.GroupName] == "WeeklyByDay" )
                {
                    RateButtonWeekly.Checked = true;
                }
                else if( values[this.UniqueID + "$" + RateButtonWeekly.GroupName] == "Monthly" )
                {
                    RateButtonMonthly.Checked = true;
                }
                else if( values[this.UniqueID + "$" + RateButtonWeekly.GroupName] == "YearlyByDate" )
                {
                    RateButtonYearly.Checked = true;
                }
            }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }

        // Controls
        CswHiddenTable WeeklyHiddenTable = new CswHiddenTable();
        CswHiddenTable MonthlyHiddenTable = new CswHiddenTable();
        CswHiddenTable YearlyHiddenTable = new CswHiddenTable();

        public RadioButton RateButtonWeekly = new RadioButton();
        public RadioButton RateButtonMonthly = new RadioButton();
        public RadioButton RateButtonYearly = new RadioButton();

        private CswTimeIntervalSelectorWeekly _Weekly;
        private CswTimeIntervalSelectorMonthly _Monthly;
        private CswTimeIntervalSelectorYearly _Yearly;

        //private Label WeeklyLabel = new Label();
        //private HtmlTable WeeklyDayPickerTable;

        //private RadioButton MonthlyByDate = new RadioButton();
        //private DropDownList MonthDateSelect = new DropDownList();
        //private RadioButton MonthlyByWeekAndDay = new RadioButton();
        //private DropDownList MonthWeekSelect = new DropDownList();
        //private HtmlTable MonthlyDayPickerTable;
        //private DropDownList MonthFrequencySelect = new DropDownList();
        //private Literal MonthFrequencyLiteral = new Literal();
        //private Literal MonthFrequencyLiteral2 = new Literal();

        //private Label YearlyLabel = new Label();
        //private CswDatePicker DatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, true );

        //private CustomValidator _Validator;


        protected override void CreateChildControls()
        {
            Controls.Add( RateButtonWeekly );
            Controls.Add( RateButtonMonthly );
            Controls.Add( RateButtonYearly );

            WeeklyHiddenTable.ID = "WeeklyHiddenTable";
            MonthlyHiddenTable.ID = "MonthlyHiddenTable";
            YearlyHiddenTable.ID = "YearlyHiddenTable";
            WeeklyHiddenTable.ShowButton = false;
            MonthlyHiddenTable.ShowButton = false;
            YearlyHiddenTable.ShowButton = false;
            Controls.Add( WeeklyHiddenTable );
            Controls.Add( MonthlyHiddenTable );
            Controls.Add( YearlyHiddenTable );

            RateButtonWeekly.ID = "WeeklyByDay";
            RateButtonWeekly.GroupName = this.ID + "_rate";
            RateButtonWeekly.Text = "Weekly";

            RateButtonMonthly.ID = "Monthly";
            RateButtonMonthly.GroupName = this.ID + "_rate";
            RateButtonMonthly.Text = "Monthly";

            RateButtonYearly.ID = "YearlyByDate";
            RateButtonYearly.GroupName = this.ID + "_rate";
            RateButtonYearly.Text = "Yearly";

            // Weekly Page
            WeeklyHiddenTable.Table.CssClass = "timeintervaltable";
            _Weekly = new CswTimeIntervalSelectorWeekly( _UseCheckChanges, this );
            WeeklyHiddenTable.Table.addControl( 0, 0, _Weekly );

            // Monthly Page
            MonthlyHiddenTable.Table.CssClass = "timeintervaltable";
            _Monthly = new CswTimeIntervalSelectorMonthly( _UseCheckChanges, this );
            MonthlyHiddenTable.Table.addControl( 0, 0, _Monthly );

            // Yearly Page
            YearlyHiddenTable.Table.CssClass = "timeintervaltable";
            _Yearly = new CswTimeIntervalSelectorYearly( _UseCheckChanges, this );
            YearlyHiddenTable.Table.addControl( 0, 0, _Yearly );

            base.CreateChildControls();

            //_Validator = new CustomValidator();
            //_Validator.ID = "vld";
            //_Validator.ValidateEmptyText = true;
            //_Validator.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            //this.Controls.Add( _Validator );

        } // CreateChildControls()



        protected override void OnPreRender( EventArgs e )
        {
            if( _UseCheckChanges )
            {
                RateButtonWeekly.Attributes.Add( "onclick", "CswTimeInterval_showSettings('weekly', '" + WeeklyHiddenTable.Table.ClientID + "', '" + MonthlyHiddenTable.Table.ClientID + "', '" + YearlyHiddenTable.Table.ClientID + "'); CswFieldTypeWebControl_onchange();" );
                RateButtonMonthly.Attributes.Add( "onclick", "CswTimeInterval_showSettings('monthly', '" + WeeklyHiddenTable.Table.ClientID + "', '" + MonthlyHiddenTable.Table.ClientID + "', '" + YearlyHiddenTable.Table.ClientID + "'); CswFieldTypeWebControl_onchange();" );
                RateButtonYearly.Attributes.Add( "onclick", "CswTimeInterval_showSettings('yearly', '" + WeeklyHiddenTable.Table.ClientID + "', '" + MonthlyHiddenTable.Table.ClientID + "', '" + YearlyHiddenTable.Table.ClientID + "'); CswFieldTypeWebControl_onchange();" );
            }
            else
            {
                RateButtonWeekly.Attributes.Add( "onclick", "CswTimeInterval_showSettings('weekly', '" + WeeklyHiddenTable.Table.ClientID + "', '" + MonthlyHiddenTable.Table.ClientID + "', '" + YearlyHiddenTable.Table.ClientID + "');" );
                RateButtonMonthly.Attributes.Add( "onclick", "CswTimeInterval_showSettings('monthly', '" + WeeklyHiddenTable.Table.ClientID + "', '" + MonthlyHiddenTable.Table.ClientID + "', '" + YearlyHiddenTable.Table.ClientID + "');" );
                RateButtonYearly.Attributes.Add( "onclick", "CswTimeInterval_showSettings('yearly', '" + WeeklyHiddenTable.Table.ClientID + "', '" + MonthlyHiddenTable.Table.ClientID + "', '" + YearlyHiddenTable.Table.ClientID + "');" );
            }

            Page.RegisterRequiresPostBack( this );

            MonthlyHiddenTable.Expanded = false;
            WeeklyHiddenTable.Expanded = false;
            YearlyHiddenTable.Expanded = false;
            if( RateButtonWeekly.Checked )
            {
                //Multi.SelectPageById(WeeklyPage.ID);
                WeeklyHiddenTable.Expanded = true;
            }
            if( RateButtonMonthly.Checked )
            {
                //Multi.SelectPageById(MonthlyPage.ID);
                MonthlyHiddenTable.Expanded = true;
            }
            if( RateButtonYearly.Checked )
            {
                //Multi.SelectPageById(YearlyPage.ID);
                YearlyHiddenTable.Expanded = true;
            }

            // BZ 7998
            if( ReadOnly )
            {
                WeeklyHiddenTable.Visible = false;
                MonthlyHiddenTable.Visible = false;
                YearlyHiddenTable.Visible = false;

                RateButtonWeekly.Visible = false;
                RateButtonMonthly.Visible = false;
                RateButtonYearly.Visible = false;

                //WeeklyLabel.Visible = false;
                //WeeklyDayPickerTable.Visible = false;

                //MonthlyByDate.Visible = false;
                //MonthDateSelect.Visible = false;
                //MonthlyByWeekAndDay.Visible = false;
                //MonthWeekSelect.Visible = false;
                //MonthlyDayPickerTable.Visible = false;
                //MonthFrequencySelect.Visible = false;
                //MonthFrequencyLiteral.Visible = false;
                //MonthFrequencyLiteral2.Visible = false;

                //YearlyLabel.Visible = false;
                //DatePicker.Visible = false;
                //_Validator.Visible = false;
            }

            base.OnPreRender( e );
        }

        public override void RenderControl( HtmlTextWriter writer )
        {
            if( ReadOnly )
            {
                writer.Write( _RateInterval.ToString() );
            }
            else
            {
                base.RenderControl( writer );
            }
        }
    }
}
