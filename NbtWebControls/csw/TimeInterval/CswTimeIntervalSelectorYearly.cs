using System;
using System.Collections.Specialized;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.NbtWebControls.FieldTypes;

namespace ChemSW.NbtWebControls
{
    public class CswTimeIntervalSelectorYearly : WebControl, IPostBackDataHandler, INamingContainer
    {
        private bool _UseCheckChanges = true;
        public bool ReadOnly = false;
        private CswTimeIntervalSelector _ParentSelector;

        public CswTimeIntervalSelectorYearly( bool UseCheckChanges, CswTimeIntervalSelector ParentSelector )
        {
            DataBinding += new EventHandler( CswTimeIntervalSelectorYearly_DataBinding );
            _UseCheckChanges = UseCheckChanges;
            _ParentSelector = ParentSelector;
        }


        private Label YearlyLabel = new Label();
        private CswDatePicker DatePicker = new CswDatePicker( CswDatePicker.DateTimeMode.DateOnly, true );

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );
            
            YearlyLabel.Text = "Every Year Starting On:";
            Table.addControl( 0, 0, YearlyLabel );

            DatePicker.ID = "yearlydate";
            DatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup; 
            Table.addControl( 1, 0, DatePicker );

            base.CreateChildControls();
        }


        public CswRateInterval RateInterval
        {
            get { return _ParentSelector.RateInterval; }
            set { _ParentSelector.RateInterval = value; }
        }
        void CswTimeIntervalSelectorYearly_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            DatePicker.Required = false;
            if( RateInterval.RateType == CswRateInterval.RateIntervalType.YearlyByDate )
            {
                if( RateInterval.Data.YearlyDate != DateTime.MinValue )
                    DatePicker.SelectedDate = RateInterval.Data.YearlyDate;
            }
        }
        public bool LoadPostData( String postDataKey, NameValueCollection values )
        {
            EnsureChildControls();
            if( _ParentSelector.RateButtonYearly.Checked &&
                values[DatePicker.DatePicker.UniqueID] != null &&
                values[DatePicker.DatePicker.UniqueID].ToString() != string.Empty )
            {
                RateInterval.setYearlyByDate( Convert.ToDateTime( values[DatePicker.DatePicker.UniqueID] ).Date );
            }
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
                YearlyLabel.Visible = false;
                DatePicker.Visible = false;
            }
            base.OnPreRender( e );
        }
    }
}
