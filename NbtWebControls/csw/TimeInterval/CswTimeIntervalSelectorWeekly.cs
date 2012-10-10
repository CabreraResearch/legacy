using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls.FieldTypes;

namespace ChemSW.NbtWebControls
{
    public class CswTimeIntervalSelectorWeekly : WebControl, IPostBackDataHandler, INamingContainer
    {
        private bool _UseCheckChanges = true;
        public bool ReadOnly = false;
        private string WeeklyDayPickerRadioGroupName = "_weeklyday";
        private CswTimeIntervalSelector _ParentSelector;

        public CswTimeIntervalSelectorWeekly( bool UseCheckChanges, CswTimeIntervalSelector ParentSelector )
        {
            DataBinding += new EventHandler( CswTimeIntervalSelectorWeekly_DataBinding );
            _UseCheckChanges = UseCheckChanges;
            _ParentSelector = ParentSelector;
        }

        private Literal StartDateLiteral;
        private CswDatePicker StartDatePicker;
        private Literal WeeklyLiteral;
        private CswTimeIntervalSelectorWeekdayPicker WeeklyDayPickerTable;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            Table.OddCellRightAlign = true;
            this.Controls.Add( Table );

            WeeklyLiteral = new Literal();
            WeeklyLiteral.Text = "Every:";
            Table.addControl( 0, 0, WeeklyLiteral );

            WeeklyDayPickerTable = new CswTimeIntervalSelectorWeekdayPicker( WeeklyDayPickerRadioGroupName, _UseCheckChanges, true );
            Table.addControl( 0, 1, WeeklyDayPickerTable );

            StartDateLiteral = new Literal();
            StartDateLiteral.Text = "Starting On:";
            Table.addControl( 1, 0, StartDateLiteral );

            StartDatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, _UseCheckChanges );
            StartDatePicker.ID = "WeeklyStartDate";
            StartDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup; 
            StartDatePicker.SelectedDate = DateTime.Now.Date;
            Table.addControl( 1, 1, StartDatePicker );

            base.CreateChildControls();
        }

        public CswRateInterval RateInterval
        {
            get { return _ParentSelector.RateInterval; }
            set { _ParentSelector.RateInterval = value; }
        }

        void CswTimeIntervalSelectorWeekly_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( RateInterval.RateType == CswRateInterval.RateIntervalType.WeeklyByDay )
            {
                WeeklyDayPickerTable.setWeekDayPickerCheckBoxSelectedValues( RateInterval.Data.WeeklyDays );
                StartDatePicker.SelectedDate = RateInterval.Data.StartingDate;
            }
        }

        public bool LoadPostData( String postDataKey, NameValueCollection values )
        {
            EnsureChildControls();
            SortedList SelectedWeeklyDays = new SortedList();
            foreach( DayOfWeek Day in Enum.GetValues( typeof( DayOfWeek ) ) )
            {
                string ThisKey = this.UniqueID + "$" + WeeklyDayPickerRadioGroupName + "_" + Day.ToString();
                if( values[ThisKey] != null && values[ThisKey] == "on" )
                    SelectedWeeklyDays.Add( Day, Day );
            }
            if( values[StartDatePicker.DatePicker.UniqueID] != null && values[StartDatePicker.DatePicker.UniqueID].ToString() != string.Empty )
            {
                StartDatePicker.SelectedDate = Convert.ToDateTime( values[StartDatePicker.DatePicker.UniqueID] ).Date;
            }

            if( _ParentSelector.RateButtonWeekly.Checked )
                RateInterval.setWeeklyByDay( SelectedWeeklyDays, StartDatePicker.SelectedDate );

            return false;
        }

        public void RaisePostDataChangedEvent()
        {
        }



        protected override void OnPreRender( EventArgs e )
        {
            Page.RegisterRequiresPostBack( this );

            if( ReadOnly )
            {
                WeeklyLiteral.Visible = false;
                WeeklyDayPickerTable.Visible = false;
                StartDateLiteral.Visible = false;
                StartDatePicker.Visible = false;
            }
            base.OnPreRender( e );
        }

    }
}
