using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.NbtWebControls;
using ChemSW.Nbt.PropTypes;
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
                if( _EditMode != NodeEditMode.LowRes )
                {
                    _TriStateCheckBox.Required = Required;
                    _TriStateCheckBox.ReadOnly = ReadOnly;
                    _TriStateCheckBox.Checked = Prop.AsLogical.Checked;
                }
                else
                {
                    _ListBox.SelectedValue = Prop.AsLogical.Checked.ToString();
                }
            }
        }

        private CswTriStateCheckBox _TriStateCheckBox;
        private DropDownList _ListBox;
        protected override void CreateChildControls()
        {
            if( _EditMode != NodeEditMode.LowRes )
            {
                _TriStateCheckBox = new CswTriStateCheckBox( Required );
                _TriStateCheckBox.ID = "TriStateCheckBox";
                _TriStateCheckBox.OnError += new CswErrorHandler( HandleError );
                _TriStateCheckBox.ValidationGroup = CswFieldTypeWebControl.FieldTypeValidationGroup;
                this.Controls.Add( _TriStateCheckBox );
            }
            else
            {
                _ListBox = new DropDownList();
                _ListBox.ID = "ListBox";
                _ListBox.AutoPostBack = false;
                _ListBox.CssClass = CswFieldTypeWebControl.DropDownCssClass;
                _ListBox.Items.Add( new ListItem( "", Tristate.Null.ToString() ) );
                _ListBox.Items.Add( new ListItem( "Yes", Tristate.True.ToString() ) );
                _ListBox.Items.Add( new ListItem( "No", Tristate.False.ToString() ) );
                this.Controls.Add( _ListBox );
            }

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
            if( _EditMode != NodeEditMode.LowRes )
                _TriStateCheckBox.Checked = Tristate.Null;
            else
                _ListBox.SelectedValue = Tristate.Null.ToString();
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
                if( _EditMode != NodeEditMode.LowRes )
                    ret = _TriStateCheckBox.Checked;
                else
                    ret = (Tristate) Enum.Parse( typeof( Tristate ), _ListBox.SelectedValue );
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
                if( _EditMode != NodeEditMode.LowRes )
                    return _TriStateCheckBox.AutoPostBack;
                //else
                //    return _ListBox.AutoPostBack;
                else
                    return false;
            }
            set
            {
                EnsureChildControls();
                if( _EditMode != NodeEditMode.LowRes )
                    _TriStateCheckBox.AutoPostBack = value;
                //else
                //    _ListBox.AutoPostBack = value;
            }
        }
        

        protected override void OnPreRender(EventArgs e)
        {
            if( _EditMode != NodeEditMode.LowRes )
                _TriStateCheckBox.OnClientClick = "CswFieldTypeWebControl_onchange();";
            base.OnPreRender(e);
        }

    }
}



