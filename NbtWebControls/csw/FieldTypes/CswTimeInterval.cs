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

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswTimeInterval : CswFieldTypeWebControl
    {
        private string WeeklyDayPickerRadioGroupName = "_weeklyday";
        private string MonthlyDayPickerRadioGroupName = "_monthlyday";

        public CswTimeInterval( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
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
                if( _EditMode != NodeEditMode.LowRes )
                {
                    _CswTimeIntervalSelector.RateInterval = Prop.AsTimeInterval.RateInterval;
                    _CswTimeIntervalSelector.ReadOnly = ReadOnly;
                }
                else
                {
                    _Label.Text = Prop.AsTimeInterval.RateInterval.ToString();
                }
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                if( _EditMode != NodeEditMode.LowRes )
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
        
        private Label _Label;
        protected override void CreateChildControls()
        {
            if( _EditMode != NodeEditMode.LowRes )
            {
                _CswTimeIntervalSelector = new CswTimeIntervalSelector( true );
                _CswTimeIntervalSelector.ID = "CswTimeIntervalSelector";
                this.Controls.Add( _CswTimeIntervalSelector );
            }
            else
            {
                _Label = new Label();
                _Label.ID = "timeintervallabel";
                this.Controls.Add( _Label );
            }
            base.CreateChildControls();
        } // CreateChildControls()
    }
}
