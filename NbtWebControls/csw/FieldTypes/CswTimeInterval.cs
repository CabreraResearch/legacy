using System;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswTimeInterval : CswFieldTypeWebControl
    {
		//private string WeeklyDayPickerRadioGroupName = "_weeklyday";
		//private string MonthlyDayPickerRadioGroupName = "_monthlyday";

        public CswTimeInterval( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswEnumNbtNodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler(CswTimeInterval_DataBinding);
        }


        private CswTimeIntervalSelector _CswTimeIntervalSelector;
        public CswRateInterval RateInterval { get { return _CswTimeIntervalSelector.RateInterval; } }

        private void CswTimeInterval_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _CswTimeIntervalSelector.RateInterval = Prop.AsTimeInterval.RateInterval;
                _CswTimeIntervalSelector.ReadOnly = ReadOnly;
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                Prop.AsTimeInterval.RateInterval = RateInterval;
            }
        }

        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            throw new NotImplementedException();
        }
        
        //private Label _Label;
        protected override void CreateChildControls()
        {
			_CswTimeIntervalSelector = new CswTimeIntervalSelector( _CswNbtResources, true );
            _CswTimeIntervalSelector.ID = "CswTimeIntervalSelector";
            this.Controls.Add( _CswTimeIntervalSelector );
            base.CreateChildControls();
        }
        // CreateChildControls()
    }
}
