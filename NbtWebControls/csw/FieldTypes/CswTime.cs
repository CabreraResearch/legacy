using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Telerik.Web.UI;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswTime : CswFieldTypeWebControl, INamingContainer
    {
        public CswTime( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswTime_DataBinding );
            EnsureChildControls();
        }

        private void CswTime_DataBinding( object sender, EventArgs e )
        {
            _DateControl.Required = Required;
            if( Prop != null && !Prop.Empty )
                SelectedTime = Convert.ToDateTime( Prop.AsTime.TimeValue );
            else
                SelectedTime = DateTime.MinValue;
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                //if( _DateControl.HiddenClearValue )
                //    Prop.AsTime.TimeValue = DateTime.MinValue;
                //else
                    Prop.AsTime.TimeValue = SelectedTime;
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            _DateControl.Clear();
        }

        public DateTime SelectedTime
        {
            get { return _DateControl.SelectedDate; }
            set { _DateControl.SelectedDate = value; }
        }

        public RadDatePicker DatePicker
        {
            get { return _DateControl.DatePicker; }
        }

        private CswDate.CswDateControl _DateControl;
        protected override void CreateChildControls()
        {
            _DateControl = new CswDate.CswDateControl( _CswNbtResources, CswDatePicker.DateTimeMode.TimeOnly, _EditMode, "time", this );

            base.CreateChildControls();
        }

        protected override void OnPreRender( EventArgs e )
        {
            if( ReadOnly )
                _DateControl.Visible = false;

            base.OnPreRender( e );
        }

        public override void RenderControl( HtmlTextWriter writer )
        {
            if( ReadOnly )
            {
                if( SelectedTime > DateTime.MinValue )
                    writer.Write( SelectedTime.TimeOfDay );
            }
            base.RenderControl( writer );
        }

    }
}
