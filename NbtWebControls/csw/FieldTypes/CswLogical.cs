using System;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswLogical : CswFieldTypeWebControl
    {
        public CswLogical( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
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
        private DropDownList _ListBox;
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
            _TriStateCheckBox.Checked = Tristate.Null;
        }

        /// <summary>
        /// Returns the current state of the UI control
        /// </summary>
        public Tristate Checked
        {
            get
            {
                EnsureChildControls();
                Tristate ret = Tristate.Null;
                ret = _TriStateCheckBox.Checked;
                return ret;
            }
        }

        /// <summary>
        /// AutoPostBack setting of control
        /// </summary>
        private bool _AutoPostBack = false;
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



