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
                _DateControl.Visible = false;

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
            private TextBox _DateBox;
            private NodeEditMode _EditMode;

            public CswDateControl( CswNbtResources CswNbtResources, CswDatePicker.DateTimeMode DateTimeMode, NodeEditMode EditMode, string IDPrefix, Control ParentControl )
            {
                _CswNbtResources = CswNbtResources;
                _EditMode = EditMode;

                _CswDatePicker = new CswDatePicker( DateTimeMode, true );
                _CswDatePicker.ID = IDPrefix + "picker";
                _CswDatePicker.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                if( _EditMode != NodeEditMode.LowRes )
                {
                    ParentControl.Controls.Add( _CswDatePicker );
                }
                else
                {
                    _DateBox = new TextBox();
                    _DateBox.ID = IDPrefix + "box";
                    _DateBox.Width = Unit.Parse( "80px" );
                    _DateBox.CssClass = CswFieldTypeWebControl.TextBoxCssClass;
                    ParentControl.Controls.Add( _DateBox );
                    
                    Literal DateMask = new Literal();
                    DateMask.Text = "&nbsp;(" + _CswDatePicker.DateFormat + ")";
                    ParentControl.Controls.Add( DateMask );
                }
            }

            public bool Required
            {
                set
                {
                    if( _EditMode != NodeEditMode.LowRes ) 
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
                if( _EditMode != NodeEditMode.LowRes )
                    _CswDatePicker.Clear();
                else
                    _DateBox.Text = string.Empty;
            }

            public DateTime SelectedDate
            {
                get
                {
                    DateTime ret = DateTime.MinValue;
                    if( _EditMode != NodeEditMode.LowRes )
                        ret = _CswDatePicker.SelectedDate;
                    else if( _DateBox.Text != string.Empty )
                    {
                        try
                        {
                            ret = Convert.ToDateTime( _DateBox.Text );
                        }
                        catch( System.FormatException ex )
                        {
                            //swallow it and continue
                            _CswNbtResources.logError( ex );
                        }
                    } 
                    return ret;
                }
                set
                {
                    if( _EditMode != NodeEditMode.LowRes )
                        _CswDatePicker.SelectedDate = value;
                    else if( value != DateTime.MinValue )
                        _DateBox.Text = value.ToShortDateString();
                    else
                        _DateBox.Text = string.Empty;
                }
            }

            public RadDatePicker DatePicker
            {
                get
                {
                    RadDatePicker ret = null;
                    if( _EditMode != NodeEditMode.LowRes )
                        ret = _CswDatePicker.DatePicker;
                    return ret;
                }
            }

            public bool Visible
            {
                set
                {
                    if( _EditMode != NodeEditMode.LowRes )
                        _CswDatePicker.Visible = value;
                    else
                        _DateBox.Visible = value;
                }
            }
        }

    }
}
