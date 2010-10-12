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
    public class CswTimeIntervalSelectorMonthly : WebControl, IPostBackDataHandler, INamingContainer
    {
        private bool _UseCheckChanges = true;
        public bool ReadOnly = false; 
        private string MonthlyDayPickerRadioGroupName = "_monthlyday";
        private CswTimeIntervalSelector _ParentSelector; 

        public CswTimeIntervalSelectorMonthly( bool UseCheckChanges, CswTimeIntervalSelector ParentSelector )
        {
            DataBinding += new EventHandler( CswTimeIntervalSelectorMonthly_DataBinding );
            _UseCheckChanges = UseCheckChanges;
            _ParentSelector = ParentSelector;
        }


        private RadioButton MonthlyByDate = new RadioButton();
        private DropDownList MonthDateSelect = new DropDownList();
        private RadioButton MonthlyByWeekAndDay = new RadioButton();
        private DropDownList MonthWeekSelect = new DropDownList();
        private CswTimeIntervalSelectorWeekdayPicker MonthlyDayPickerTable;
        private DropDownList MonthFrequencySelect = new DropDownList();
        private DropDownList StartMonthSelect = new DropDownList();
        private DropDownList StartYearSelect = new DropDownList();
        private Literal MonthFrequencyLiteral = new Literal();
        private Literal MonthFrequencyLiteral2 = new Literal();
        private Literal StartDateLiteral = new Literal();
        private Literal StartDateLiteral2 = new Literal();

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            MonthFrequencyLiteral.Text = "Every ";
            MonthFrequencyLiteral2.Text = " Month(s)";

            MonthFrequencySelect.ID = "MonthFrequencySelect";
            MonthFrequencySelect.CssClass = CswFieldTypeWebControl.DropDownCssClass;
            for( int i = 1; i <= 12; i++ )
                MonthFrequencySelect.Items.Add( new ListItem( i.ToString() ) );
            Table.addControl( 0, 0, MonthFrequencyLiteral );
            Table.addControl( 0, 0, MonthFrequencySelect );
            Table.addControl( 0, 0, MonthFrequencyLiteral2 );

            MonthlyByDate.ID = "MonthlyByDate";
            MonthlyByDate.GroupName = this.ID + "_monthlyrate";
            MonthlyByDate.Text = "On Day of Month: ";
            Table.addControl( 1, 0, MonthlyByDate );

            for( Int32 i = 1; i <= 31; i++ )
            {
                ListItem ThisDate = new ListItem();
                ThisDate.Text = i.ToString();
                ThisDate.Value = i.ToString();
                MonthDateSelect.Items.Add( ThisDate );
            }
            MonthDateSelect.ID = "MonthDateSelect";
            MonthDateSelect.CssClass = CswFieldTypeWebControl.DropDownCssClass;
            Table.addControl( 1, 0, MonthDateSelect );

            MonthlyByWeekAndDay.ID = "MonthlyByWeekAndDay";
            MonthlyByWeekAndDay.GroupName = this.ID + "_monthlyrate";
            MonthlyByWeekAndDay.Text = "Every ";
            Table.addControl( 2, 0, MonthlyByWeekAndDay );

            MonthWeekSelect.CssClass = CswFieldTypeWebControl.DropDownCssClass;
            for( Int32 i = 1; i <= 4; i++ )
            {
                ListItem ThisWeek = new ListItem();
                switch( i )
                {
                    case 1: ThisWeek.Text = "First:"; break;
                    case 2: ThisWeek.Text = "Second:"; break;
                    case 3: ThisWeek.Text = "Third:"; break;
                    case 4: ThisWeek.Text = "Fourth:"; break;
                }
                ThisWeek.Value = i.ToString();
                MonthWeekSelect.Items.Add( ThisWeek );
            }
            Table.addControl( 2, 0, MonthWeekSelect );

            MonthlyDayPickerTable = new CswTimeIntervalSelectorWeekdayPicker(MonthlyDayPickerRadioGroupName, _UseCheckChanges, false );
            Table.addControl( 3, 0, MonthlyDayPickerTable );

            StartDateLiteral.Text = "Starting On: ";
            Table.addControl( 4, 0, StartDateLiteral );

            StartYearSelect.ID = "StartYearSelect";
            StartMonthSelect.ID = "StartMonthSelect";

            for( int m = 1; m <= 12; m++ )
                StartMonthSelect.Items.Add( new ListItem( m.ToString() ) );
            StartYearSelect.SelectedValue = DateTime.Now.Month.ToString(); 
            Table.addControl( 4, 0, StartMonthSelect );

            StartDateLiteral2.Text = "/";
            Table.addControl( 4, 0, StartDateLiteral2 );

            for( int m = ( DateTime.Now.Year - 20 ); m <= ( DateTime.Now.Year + 10 ); m++ )
                StartYearSelect.Items.Add( new ListItem( m.ToString() ) );
            StartYearSelect.SelectedValue = DateTime.Now.Year.ToString();
            Table.addControl( 4, 0, StartYearSelect);

            base.CreateChildControls();
        }
        
        public CswRateInterval RateInterval
        {
            get { return _ParentSelector.RateInterval; }
            set { _ParentSelector.RateInterval = value; }
        }

        void CswTimeIntervalSelectorMonthly_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( RateInterval.RateType == CswRateInterval.RateIntervalType.MonthlyByWeekAndDay )
            {
                MonthlyByWeekAndDay.Checked = true;
                MonthFrequencySelect.SelectedValue = ( (CswRateIntervalMonthlyWeek) RateInterval.RateInterval ).MonthlyFrequency.ToString();
                if( ( (CswRateIntervalMonthlyWeek) RateInterval.RateInterval ).MonthlyWeek != Int32.MinValue )
                    MonthWeekSelect.SelectedValue = ( (CswRateIntervalMonthlyWeek) RateInterval.RateInterval ).MonthlyWeek.ToString();
                MonthlyDayPickerTable.setWeekDayPickerRadioSelectedValue( ( (CswRateIntervalMonthlyWeek) RateInterval.RateInterval ).MonthlyDay );
                StartMonthSelect.SelectedValue = ( (CswRateIntervalMonthlyWeek) RateInterval.RateInterval ).StartingMonth.ToString();
                StartYearSelect.SelectedValue = ( (CswRateIntervalMonthlyWeek) RateInterval.RateInterval ).StartingYear.ToString();
            }
            if( RateInterval.RateType == CswRateInterval.RateIntervalType.MonthlyByDate )
            {
                MonthlyByDate.Checked = true;
                MonthFrequencySelect.SelectedValue = ( (CswRateIntervalMonthlyDate) RateInterval.RateInterval ).MonthlyFrequency.ToString();
                if( ( (CswRateIntervalMonthlyDate) RateInterval.RateInterval ).MonthlyDate != Int32.MinValue )
                    MonthDateSelect.SelectedValue = ( (CswRateIntervalMonthlyDate) RateInterval.RateInterval ).MonthlyDate.ToString();
                StartMonthSelect.SelectedValue = ( (CswRateIntervalMonthlyDate) RateInterval.RateInterval ).StartingMonth.ToString();
                StartYearSelect.SelectedValue = ( (CswRateIntervalMonthlyDate) RateInterval.RateInterval ).StartingYear.ToString();
            }
        }

        public bool LoadPostData( String postDataKey, NameValueCollection values )
        {
            EnsureChildControls();

            if( values[this.UniqueID + "$" + MonthlyByDate.GroupName] != null )
            {
                if( values[this.UniqueID + "$" + MonthlyByDate.GroupName] == "MonthlyByDate" )
                    MonthlyByDate.Checked = true;
                else if( values[this.UniqueID + "$" + MonthlyByDate.GroupName] == "MonthlyByWeekAndDay" )
                    MonthlyByWeekAndDay.Checked = true;
            }
            if( values[MonthFrequencySelect.UniqueID] != null )
                MonthFrequencySelect.SelectedValue = values[MonthFrequencySelect.UniqueID];
            if( values[MonthWeekSelect.UniqueID] != null )
                MonthWeekSelect.SelectedValue = values[MonthWeekSelect.UniqueID];
            if( values[MonthDateSelect.UniqueID] != null )
                MonthDateSelect.SelectedValue = values[MonthDateSelect.UniqueID];
            if( values[StartMonthSelect.UniqueID] != null )
                StartMonthSelect.SelectedValue = values[StartMonthSelect.UniqueID];
            if( values[StartYearSelect.UniqueID] != null )
                StartYearSelect.SelectedValue = values[StartYearSelect.UniqueID];

            if( _ParentSelector.RateButtonMonthly.Checked )
            {
                if( MonthlyByDate.Checked )
                    RateInterval.setMonthlyByDate( Convert.ToInt32( MonthFrequencySelect.SelectedValue ),
                                                   Convert.ToInt32( MonthDateSelect.SelectedValue ),
                                                   Convert.ToInt32( StartMonthSelect.SelectedValue ),
                                                   Convert.ToInt32( StartYearSelect.SelectedValue ) );
                else
                    RateInterval.setMonthlyByWeekAndDay( Convert.ToInt32( MonthFrequencySelect.SelectedValue ),
                                                         Convert.ToInt32( MonthWeekSelect.SelectedValue ),
                                                         (DayOfWeek) Enum.Parse( typeof( DayOfWeek ), values[this.UniqueID + "$" + "_monthlyday"].Substring( ( "_monthlyday_" ).Length ), true ),
                                                         Convert.ToInt32( StartMonthSelect.SelectedValue ),
                                                         Convert.ToInt32( StartYearSelect.SelectedValue ) );
            }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( _UseCheckChanges )
            {
                MonthlyByDate.Attributes.Add( "onclick", "CswFieldTypeWebControl_onchange();" );
                MonthDateSelect.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
                MonthlyByWeekAndDay.Attributes.Add( "onclick", "CswFieldTypeWebControl_onchange();" );
                MonthWeekSelect.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
                MonthFrequencySelect.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );
            }

            Page.RegisterRequiresPostBack( this );

            if( ReadOnly )
            {
                MonthlyByDate.Visible = false;
                MonthDateSelect.Visible = false;
                MonthlyByWeekAndDay.Visible = false;
                MonthWeekSelect.Visible = false;
                MonthlyDayPickerTable.Visible = false;
                MonthFrequencySelect.Visible = false;
                MonthFrequencyLiteral.Visible = false;
                MonthFrequencyLiteral2.Visible = false;
            }

            base.OnPreRender( e );
        }

    }
}
