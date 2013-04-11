using System;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswLogical : CswFieldTypeWebControl
    {
        public CswLogical( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswEnumNbtNodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler(CswLogical_DataBinding);
            EnsureChildControls();
        }

        private void CswLogical_DataBinding(object sender, EventArgs e)
        {
            if( Prop != null )
            {

                _TriStateCheckBox.Required = Required;
                _TriStateCheckBox.ReadOnly = ReadOnly;
                _TriStateCheckBox.Checked = Prop.AsLogical.Checked;

            }
        }

        private CswTriStateCheckBox _TriStateCheckBox;
        //private DropDownList _ListBox;
        protected override void CreateChildControls()
        {

            _TriStateCheckBox = new CswTriStateCheckBox( Required );
            _TriStateCheckBox.ID = "TriStateCheckBox";
            _TriStateCheckBox.OnError += new CswErrorHandler( HandleError );
            _TriStateCheckBox.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
            this.Controls.Add( _TriStateCheckBox );


            base.CreateChildControls();
        }


        public override void Save()
        {
            if( !ReadOnly )
                Prop.AsLogical.Checked = Checked;
        }

        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            _TriStateCheckBox.Checked = CswEnumTristate.Null;
        }

        /// <summary>
        /// Returns the current state of the UI control
        /// </summary>
        public CswEnumTristate Checked
        {
            get
            {
                EnsureChildControls();
                CswEnumTristate ret = CswEnumTristate.Null;
                ret = _TriStateCheckBox.Checked;
                return ret;
            }
        }

        /// <summary>
        /// AutoPostBack setting of control
        /// </summary>
        //private bool _AutoPostBack = false;
        public bool AutoPostBack
        {
            get
            {
                EnsureChildControls();
                return _TriStateCheckBox.AutoPostBack;

            }
            set
            {
                EnsureChildControls();
                _TriStateCheckBox.AutoPostBack = value;
            }
        }
        

        protected override void OnPreRender(EventArgs e)
        {
            _TriStateCheckBox.OnClientClick = "CswFieldTypeWebControl_onchange();";
            base.OnPreRender(e);
        }

    }
}



