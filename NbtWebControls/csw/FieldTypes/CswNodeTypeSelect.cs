using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswNodeTypeSelect : CswFieldTypeWebControl, INamingContainer//, IPostBackDataHandler
    {

        public PropertySelectMode SelectMode = PropertySelectMode.Single;

        private bool _AllowEditValue = false;

        public CswNodeTypeSelect( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswEnumNbtNodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswNodeTypeSelect_DataBinding );
        }

        private void CswNodeTypeSelect_DataBinding( object sender, EventArgs e )
        {
            _AllowEditValue = ( _EditMode != CswEnumNbtNodeEditMode.Demo && _EditMode != CswEnumNbtNodeEditMode.PrintReport && !ReadOnly );

            EnsureChildControls();
            if( Prop != null )
            {
                SelectMode = Prop.AsNodeTypeSelect.SelectMode;
                if( _AllowEditValue )
                {
                    /* Options is going to be wrong if this prop is required. 
                     * There is no good way to distinguish between default value (the only context this class will be used until it retires)
                     * and an actual instance of the property. This is OK, because we're replicating the behavior on prop instance:
                     * if required, the first nodetype is selected. */
                    DataTable Data = Prop.AsNodeTypeSelect.Options;

                    _CBArray.UseRadios = ( SelectMode == PropertySelectMode.Single );
                    _CBArray.CreateCheckBoxes( Data, CswNbtNodePropNodeTypeSelect.NameColumn, CswNbtNodePropNodeTypeSelect.KeyColumn );
                }
            }
        }

        public override void Save()
        {
            if( _AllowEditValue && !ReadOnly )
            {
                Prop.AsNodeTypeSelect.SelectedNodeTypeIds = _CBArray.GetCheckedValues( "Include" );
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            _ValueLabel.Text = string.Empty;
            _CBArray.Clear();
        }

        private Label _ValueLabel;
        private CswImageButton _EditButton;
        private CswCheckBoxArray _CBArray;
        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _ValueLabel = new Label();
            _ValueLabel.ID = "value";
            _ValueLabel.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            Table.addControl( 0, 0, _ValueLabel );

            if( _AllowEditValue )
            {
                _CBArray = new CswCheckBoxArray( _CswNbtResources );
                _CBArray.ID = "cbarray";
                Table.addControl( 1, 0, _CBArray );
            }
            else
            {
                _EditButton = new CswImageButton( CswImageButton.ButtonType.Edit );
                _EditButton.ID = "edit";
                Table.addControl( 0, 1, _EditButton );
            }

            base.CreateChildControls();
        }


        protected override void OnPreRender( EventArgs e )
        {
            EnsureChildControls();

            if( Prop != null )
            {
                _ValueLabel.Text = Prop.AsNodeTypeSelect.SelectedNodeTypeNames().ToString();
                if( !_AllowEditValue )
                {
                    if( Prop.NodeId != null )
                        _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                    if( ReadOnly )
                        _EditButton.Visible = false;

                    _ValueLabel.Text += "&nbsp;";
                }
            }

            base.OnPreRender( e );
        }

    }
}
