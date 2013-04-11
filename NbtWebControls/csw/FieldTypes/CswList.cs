using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswList : CswFieldTypeWebControl, INamingContainer
    {
        public CswList( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswEnumNbtNodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler( CswList_DataBinding );
        }

        private void CswList_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                _DropDownList.DataSource = Prop.AsList.Options.Options;
                _DropDownList.DataTextField = CswNbtNodePropList.OptionTextField; // "Text";
                _DropDownList.DataValueField = CswNbtNodePropList.OptionValueField; // "Value";
                _DropDownList.DataBound += new EventHandler( _DropDownList_DataBound );
            }
        }

        void _DropDownList_DataBound( object sender, EventArgs e )
        {
            if( Prop != null )
            {
                if( null != _DropDownList.Items.FindByValue( Prop.AsList.Value ) )
                {
                    SelectedValue = Prop.AsList.Value;
                }
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                // DataSource.updatePropertyValue(TreeId, ViewId, ViewXml, NodeKey, PropId, SelectedValue);
                Prop.AsList.Value = SelectedValue;
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            SelectedValue = string.Empty;
        }

        public string SelectedValue
        {
            get
            {
                string Selected = _DropDownList.SelectedValue;
                if( null == _DropDownList.Items.FindByText( Selected ) )
                {
                    Selected = string.Empty;
                }

                EnsureChildControls();
                return Selected;
            }
            set
            {
                EnsureChildControls();
                if( value == String.Empty && _DropDownList.Items.Count > 0 )
                    _DropDownList.SelectedIndex = 0;
                else
                    _DropDownList.SelectedValue = value;
            }
        }

        /// <summary>
        /// AutoPostBack setting of DropDownList
        /// </summary>
        public bool AutoPostBack
        {
            get
            {
                EnsureChildControls();
                return _DropDownList.AutoPostBack;
            }
            set
            {
                EnsureChildControls();
                _DropDownList.AutoPostBack = value;
            }
        }

        private DropDownList _DropDownList;

        protected override void CreateChildControls()
        {
            _DropDownList = new DropDownList();
            _DropDownList.ID = "list";
            _DropDownList.CssClass = CswFieldTypeWebControl.DropDownCssClass;
            this.Controls.Add( _DropDownList );

            base.CreateChildControls();

            if( Required )
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _DropDownList.ID;
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            //Case 22769. This now only affects Design mode, so get rid of it
            //if( Prop != null && Prop.AsList != null )
            //{
            //    if( null == _DropDownList.Items.FindByValue( Prop.AsList.Value ) )
            //    {
            //        // Add it!  This guarantees we see the data that was saved, even if the options change
            //        ListItem SelectedItem = new ListItem( Prop.AsList.Value, Prop.AsList.Value );
            //        _DropDownList.Items.Add( SelectedItem );
            //        SelectedValue = Prop.AsList.Value;
            //    }
            //}

            _DropDownList.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange();" );

            base.OnPreRender( e );
        }

        public override void RenderControl( HtmlTextWriter writer )
        {
            if( ReadOnly )
            {
                writer.Write( this.SelectedValue );
            }
            else
            {
                base.RenderControl( writer );
            }
        }
    }
}
