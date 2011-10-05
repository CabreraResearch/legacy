using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ChemSW.NbtWebControls;
using ChemSW.Core;
using ChemSW.NbtWebControls.FieldTypes;
using ChemSW.CswWebControls;

public partial class TimeIntervalTest : System.Web.UI.Page
{
    private CswAutoTable Table;
    private Literal TimeIntervalLiteral;
    private CswTimeIntervalSelector TimeIntervalSelector;
    //private Literal StartingDateLiteral;
    //private TextBox StartingDateBox;
    private Button SubmitButton;

    protected override void OnInit( EventArgs e )
    {
        Table = new CswAutoTable();
        Table.OddCellRightAlign = true;
        ph.Controls.Add( Table );

        TimeIntervalLiteral = new Literal();
        TimeIntervalLiteral.Text = "Time Interval: ";
        Table.addControl(0,0,TimeIntervalLiteral);

        TimeIntervalSelector = new CswTimeIntervalSelector(null, false);
        TimeIntervalSelector.ID = "TimeIntervalSelector";
        Table.addControl(0,1,TimeIntervalSelector);

        //StartingDateLiteral = new Literal();
        //StartingDateLiteral.Text = "Starting Date: ";
        //Table.addControl( 1, 0, StartingDateLiteral );

        //StartingDateBox = new TextBox();
        //StartingDateBox.ID = "StartingDate";
        //StartingDateBox.CssClass = "textinput";
        //Table.addControl( 1, 1, StartingDateBox );

        SubmitButton = new Button();
        SubmitButton.ID = "SubmitButton";
        SubmitButton.Text = "Submit";
        SubmitButton.Click += new EventHandler( SubmitButton_Click );
        Table.addControl( 2, 1, SubmitButton );

        base.OnInit( e );
    }

    void SubmitButton_Click( object sender, EventArgs e )
    {
        CswAutoTable ResultsTable = new CswAutoTable();
        ph.Controls.Add( ResultsTable );

        CswRateInterval RateInterval = TimeIntervalSelector.RateInterval;
        DateTime CurrentDate = RateInterval.getFirst();
        for( int r = 0; r < 500; r++ )
        {
            Literal Current = new Literal();
            Current.Text = CurrentDate.Date.ToString();
            ResultsTable.addControl( r, 0, Current );
            CurrentDate = RateInterval.getNext( CurrentDate );
        }
    }

}
