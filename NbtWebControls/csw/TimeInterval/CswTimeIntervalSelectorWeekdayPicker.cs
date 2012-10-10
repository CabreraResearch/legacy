using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ChemSW.NbtWebControls
{
    public class CswTimeIntervalSelectorWeekdayPicker : WebControl
    {
        private bool _UseCheckBoxes;
        private bool _UseCheckChanges;
        private string _RadioGroupName;
        private ArrayList DayButtons = new ArrayList();

        public CswTimeIntervalSelectorWeekdayPicker( string RadioGroupName, bool UseCheckChanges, bool UseCheckBoxes )
        {
            _RadioGroupName = RadioGroupName;
            _UseCheckChanges = UseCheckChanges;
            _UseCheckBoxes = UseCheckBoxes;
            
            EnsureChildControls();
        }

        private HtmlTable _Table;
        protected override void CreateChildControls()
        {
            _Table = new HtmlTable();
            this.Controls.Add( _Table );
            HtmlTableRow Row1 = new HtmlTableRow();
            HtmlTableRow Row2 = new HtmlTableRow();
            _Table.Rows.Add( Row1 );
            _Table.Rows.Add( Row2 );

            foreach( String day in Enum.GetNames( typeof( DayOfWeek ) ) )
            {
                HtmlTableCell LabelCell = new HtmlTableCell();
                LabelCell.Style.Add( HtmlTextWriterStyle.TextAlign, "center" );
                Row1.Cells.Add( LabelCell );
                Label ThisDayLabel = new Label();
                ThisDayLabel.Text = day.Substring( 0, 1 );
                LabelCell.Controls.Add( ThisDayLabel );

                HtmlTableCell ButtonCell = new HtmlTableCell();
                Row2.Cells.Add( ButtonCell );

                if( _UseCheckBoxes )
                {
                    CheckBox ThisDayButton = new CheckBox();
                    ButtonCell.Controls.Add( ThisDayButton );
                    ThisDayButton.ID = _RadioGroupName + "_" + day;
                    DayButtons.Add( ThisDayButton );
                }
                else
                {
                    RadioButton ThisDayButton = new RadioButton();
                    ButtonCell.Controls.Add( ThisDayButton );
                    ThisDayButton.GroupName = _RadioGroupName;
                    ThisDayButton.ID = _RadioGroupName + "_" + day;
                    DayButtons.Add( ThisDayButton );
                }
            }
            
            base.CreateChildControls();
        }

        public void setWeekDayPickerRadioSelectedValue( DayOfWeek SelectedValue )
        {
            Control RadioControl = _Table.FindControl( _RadioGroupName + "_" + SelectedValue.ToString() );
            if( RadioControl != null )
                ( (RadioButton) RadioControl ).Checked = true;
        }

        public void setWeekDayPickerCheckBoxSelectedValues( SortedList SelectedDays )
        {
            foreach( DayOfWeek Day in Enum.GetValues( typeof( DayOfWeek ) ) )
            {
                CheckBox DayCheck = (CheckBox) _Table.FindControl( _RadioGroupName + "_" + Day.ToString() );
                DayCheck.Checked = SelectedDays.ContainsKey( Day );
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( _UseCheckChanges )
            {
                foreach( WebControl DayButton in DayButtons )
                {
                    DayButton.Attributes.Add( "onclick", "CswFieldTypeWebControl_onchange();" );
                }
            }
        }
    }
}
