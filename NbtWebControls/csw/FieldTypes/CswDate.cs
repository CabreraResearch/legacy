using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    [ToolboxData("<{0}:CswDate runat=server></{0}:CswDate>")]
    public class CswDate : CswFieldTypeWebControl, INamingContainer
    {
        public CswDate( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswDate_DataBinding );
            EnsureChildControls();
        }

        private void CswDate_DataBinding( object sender, EventArgs e )
        {
            _DateControl.Required = Required;

            if( Prop != null && !Prop.Empty )
                SelectedDate = Convert.ToDateTime( Prop.AsDate.DateValue );
            else
                SelectedDate = DateTime.MinValue;
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                //if( _DateControl.HiddenClearValue )
                //    Prop.AsDate.DateValue = DateTime.MinValue;
                //else
                    Prop.AsDate.DateValue = SelectedDate;
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

        public DateTime SelectedDate
        {
            get { return _DateControl.SelectedDate; }
            set { _DateControl.SelectedDate = value; }
        }

        public RadDatePicker DatePicker
        {
            get { return _DateControl.DatePicker; }
        }

        private CswDateControl _DateControl;
        protected override void CreateChildControls()
        {
            _DateControl = new CswDate.CswDateControl( _CswNbtResources, CswDatePicker.DateTimeMode.DateOnly, _EditMode, "date", this ); 
            base.CreateChildControls();
        }

        protected override void OnPreRender(EventArgs e)
        {
            if( ReadOnly )
            {
                _DateControl.Visible = false;
            }

            base.OnPreRender(e);
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (ReadOnly)
            {
                if (SelectedDate > DateTime.MinValue)
                    writer.Write(SelectedDate.Date.ToShortDateString());
            }
            base.RenderControl(writer);
        }



        public class CswDateControl 
        {
            private CswNbtResources _CswNbtResources;
            private CswDatePicker _CswDatePicker;
            //private TextBox _DateBox;
            private NodeEditMode _EditMode;

            public CswDateControl( CswNbtResources CswNbtResources, CswDatePicker.DateTimeMode DateTimeMode, NodeEditMode EditMode, string IDPrefix, Control ParentControl )
            {
                _CswNbtResources = CswNbtResources;
                _EditMode = EditMode;

                _CswDatePicker = new CswDatePicker( DateTimeMode, true );
                _CswDatePicker.ID = IDPrefix + "picker";
                _CswDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                ParentControl.Controls.Add( _CswDatePicker );
            }

            public bool Required
            {
                set
                {
                     _CswDatePicker.Required = value;
                }
            }

            //public bool HiddenClearValue
            //{
            //    get
            //    {
            //        if( _EditMode != NodeEditMode.LowRes )
            //            return ( _CswDatePicker.HiddenClear.Value == "1" );
            //        else
            //            return false;
            //    }
            //}

            public void Clear()
            {
                _CswDatePicker.Clear();
            }

            public DateTime SelectedDate
            {
                get
                {
                    DateTime ret = DateTime.MinValue;
                    ret = _CswDatePicker.SelectedDate;
                    return ret;
                }
                set
                {
                    _CswDatePicker.SelectedDate = value;
                }
            }

            public RadDatePicker DatePicker
            {
                get
                {
                    RadDatePicker ret = null;
                    ret = _CswDatePicker.DatePicker;
                    return ret;
                }
            }

            public bool Visible
            {
                set
                {
                    _CswDatePicker.Visible = value;
                }
            }
        }

    }
}
