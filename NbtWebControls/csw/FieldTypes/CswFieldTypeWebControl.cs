using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.NbtWebControls;
using ChemSW.Exceptions;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public abstract class CswFieldTypeWebControl : CompositeControl
    {
        protected CswNbtResources _CswNbtResources;
        protected CswNbtMetaDataNodeTypeProp _CswNbtMetaDataNodeTypeProp;
        protected NodeEditMode _EditMode;

        public CswFieldTypeWebControl( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
        {
            _CswNbtResources = CswNbtResources;
            _CswNbtMetaDataNodeTypeProp = CswNbtMetaDataNodeTypeProp;
            ReadOnly = _CswNbtMetaDataNodeTypeProp.ReadOnly;
            _EditMode = EditMode;
        }

        private CswNbtNodePropWrapper _Prop = null;
        public CswNbtNodePropWrapper Prop
        {
            get { return _Prop; }
            set { _Prop = value; }
        }
        public Int32 PropId
        {
            get { return _CswNbtMetaDataNodeTypeProp.PropId; }
        }
        private bool _Required;
        public bool Required
        {
            get { return _CswNbtMetaDataNodeTypeProp.IsRequired || _Required; }
            set { _Required = value; }
        }
        private bool _ReadOnly = false;
        public bool ReadOnly
        {
            get { return _ReadOnly; }
            set { _ReadOnly = value; }
        }
        //private CswNbtNodeKey _NodeKey = null;
        //public CswNbtNodeKey NodeKey
        //{
        //    get { return _NodeKey; }
        //    set { _NodeKey = value; }
        //}

        protected RequiredFieldValidator _RequiredValidator;
        protected override void CreateChildControls()
        {
            _RequiredValidator = new RequiredFieldValidator();
            _RequiredValidator.ID = "RequiredValidator_" + PropId;
            _RequiredValidator.Display = ValidatorDisplay.Dynamic;
            _RequiredValidator.EnableClientScript = true;
            _RequiredValidator.Text = "&nbsp;<img src=\"Images/vld/bad.gif\" alt=\"Value is required\" />";
            _RequiredValidator.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            
            // these should be reset when ControlToValidate is assigned
            _RequiredValidator.Visible = false;
            _RequiredValidator.Enabled = false;
            
            this.Controls.Add( _RequiredValidator );

            base.CreateChildControls();
        }

        public static string DataTextField = "gestalt";
        public static string DataValueField = "field1";
        public static string DataValue2Field = "field2";
        public static string DataValue3Field = "field3";
        public static string DataValueBlobField = "gestalt";
        public static string ListOptionTextField = "optionvalue";
        public static string ListOptionValueField = "optionvalue";

        public static string TextBoxCssClass = "textinput";
        public static string TextBoxCssClassInvalid = "textinput_invalid";
        public static string DropDownCssClass = "selectinput";
        public static string DropDownCssClassInvalid = "selectinput_invalid";
        public static string DatePickerCssClass = "DatePicker";
        public static string DatePickerCssClassInvalid = "DatePickerInvalid";
        public static string StaticTextCssClass = "staticvalue";
    
        public static string FieldTypeValidationGroup = "FieldTypeVG";


        public abstract void Save();
        public abstract void AfterSave();
        public abstract void Clear();


        public event CswErrorHandler OnError;
        protected void HandleError( Exception ex )
        {
            if( OnError != null )
                OnError( ex );
            else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
                throw ex;
        }
    }
}
