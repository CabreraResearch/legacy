using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswViewReference : CswFieldTypeWebControl, INamingContainer
    {
        //public PropertySelectMode SelectMode = PropertySelectMode.Single;

        private bool _AllowEditValue = false;

        public CswViewReference( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswEnumNbtNodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler( CswViewReference_DataBinding );
        }

        private void CswViewReference_DataBinding( object sender, EventArgs e )
        {
            _AllowEditValue = !( _EditMode == CswEnumNbtNodeEditMode.Add ||
                                 _EditMode == CswEnumNbtNodeEditMode.EditInPopup ||
                                 _EditMode == CswEnumNbtNodeEditMode.Demo ||
                                 _EditMode == CswEnumNbtNodeEditMode.PrintReport ||
                                 ReadOnly );

            EnsureChildControls();
            //if( Prop != null && Prop.NodeId != null )
            //{
            //    //SelectMode = Prop.AsViewReference.SelectMode;
            //    CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
            //    if( Prop.AsViewReference.ViewId == Int32.MinValue )
            //    {
            //        // make a new view
            //        CswNbtView NewView = new CswNbtView( _CswNbtResources );
            //        NewView.makeNew( MetaDataProp.PropName, NbtViewVisibility.Property, null, null, null );
            //        NewView.save();
            //    }
            //}
        } // CswViewReference_DataBinding()

        public override void Save()
        {
            // Not allowed
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            // Nothing to do
        }

        private Label _ValueLabel;
        private CswImageButton _EditButton;
        private CswImageButton _ViewButton;

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _ValueLabel = new Label();
            _ValueLabel.ID = "value";
            _ValueLabel.CssClass = CswFieldTypeWebControl.StaticTextCssClass;
            Table.addControl( 0, 0, _ValueLabel );

            _ViewButton = new CswImageButton( CswImageButton.ButtonType.View );
            _ViewButton.ID = "view";
            Table.addControl( 0, 1, _ViewButton );

            _EditButton = new CswImageButton( CswImageButton.ButtonType.Edit );
            _EditButton.ID = "edit";
            Table.addControl( 0, 2, _EditButton );

            base.CreateChildControls();
        } // CreateChildControls()


        protected override void OnPreRender( EventArgs e )
        {
            EnsureChildControls();

            _ValueLabel.Text = Prop.AsViewReference.CachedViewName;
            //_ValueLabel.Text += "&nbsp;";

            if( Prop != null && _AllowEditValue && Prop.NodeId != null )
                _EditButton.OnClientClick = "window.location='EditView.aspx?step=2&viewid=" + Prop.AsViewReference.ViewId + "&return=Main.aspx';";
            else
                _EditButton.Visible = false;

            if( Prop != null && Prop.NodeId != null )
                _ViewButton.OnClientClick = "window.location='Main.aspx?viewid=" + Prop.AsViewReference.ViewId + "'";
            else
                _ViewButton.Visible = false;

            base.OnPreRender( e );
        } // OnPreRender()

    } // class CswViewReference 
} // namespace ChemSW.NbtWebControls.FieldTypes
