using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswUserSelect : CswFieldTypeWebControl, INamingContainer//, IPostBackDataHandler
    {
        private bool _AllowEditValue = false;

        public CswUserSelect( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswUserSelect_DataBinding );
        }


        private void CswUserSelect_DataBinding( object sender, EventArgs e )
        {
            try
            {
                _AllowEditValue = ( _EditMode != NodeEditMode.Demo && _EditMode != NodeEditMode.PrintReport && !ReadOnly );

                EnsureChildControls();
                if( Prop != null )
                {
                    if( _AllowEditValue )
                    {
                        DataTable Data = Prop.AsUserSelect.getUserOptions();

                        _CBArray.UseRadios = false;
                        _CBArray.CreateCheckBoxes( Data, "label", "userIdString" );
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }


        public override void Save()
        {
            if( _AllowEditValue && !ReadOnly )
            {
                Prop.AsUserSelect.SelectedUserIds = _CBArray.GetCheckedValues( "Include" );
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
            try
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
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.CreateChildControls();
        }


        protected override void OnPreRender( EventArgs e )
        {
            try
            {
                EnsureChildControls();
                if( Prop != null )
                {
                    _ValueLabel.Text = Prop.AsUserSelect.SelectedUserNames().ToString();
                    if( !_AllowEditValue )
                    {
                        if( Prop.NodeId != null )
                            _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                        if( ReadOnly )
                            _EditButton.Visible = false;

                        _ValueLabel.Text += "&nbsp;";
                    }
                }
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnPreRender( e );
        }

    }
}
