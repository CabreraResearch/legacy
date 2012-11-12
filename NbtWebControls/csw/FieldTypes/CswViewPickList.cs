using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswViewPickList : CswFieldTypeWebControl, INamingContainer
    {
        public PropertySelectMode SelectMode = PropertySelectMode.Single;

        private bool _AllowEditValue = false;

        public CswViewPickList( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler( CswViewPickList_DataBinding );
        }

        private void CswViewPickList_DataBinding( object sender, EventArgs e )
        {
            _AllowEditValue = !( _EditMode == NodeEditMode.Edit ||
                                 _EditMode == NodeEditMode.Demo ||
                                 _EditMode == NodeEditMode.PrintReport ||
                                 ReadOnly );

            EnsureChildControls();
            if( Prop != null )
            {
                SelectMode = Prop.AsViewPickList.SelectMode;
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );

                // Kind of a kludge fix for BZ 6941
                // BUT SEE BZ 8288
                //DataTable Views = Prop.AsViewPickList.Views;
                //DataTable Views = null;
                //CswQueryCaddy ViewsCaddy = _CswNbtResources.makeCswQueryCaddy( "getVisibleViewInfo" );
                //ViewsCaddy.S4Parameters.Add( "orderbyclause", "lower(v.viewname)" );
                ////if( MetaDataProp.NodeType.ObjectClass.ObjectClass == CswNbtMetaDataObjectClassName.NbtObjectClass.UserClass )
                ////{
                ////    CswNbtObjClassUser UserNode = (CswNbtObjClassUser) _CswNbtResources.Nodes[this.NodeKey];
                ////    if( UserNode != null && UserNode.RoleId > 0 )
                ////    {
                ////        // use this user's visible views, not the current logged in user
                ////        ViewsCaddy.S4Parameters.Add( "getroleid", UserNode.RoleId.ToString() );
                ////        ViewsCaddy.S4Parameters.Add( "getuserid", UserNode.UserId.ToString() );
                ////        Views = ViewsCaddy.Table;
                ////    }
                ////    else
                ////    {
                ////        // Creating a new user, don't pick a default view (BZ 7055)
                ////        Views = new DataTable();
                ////    }
                ////}
                ////else
                ////{
                //    ViewsCaddy.S4Parameters.Add( "getroleid", _CswNbtResources.CurrentUser.RoleId.ToString() );
                //    ViewsCaddy.S4Parameters.Add( "getuserid", _CswNbtResources.CurrentUser.UserId.ToString() );
                //    Views = ViewsCaddy.Table;
                ////}

                //_ViewsForCBA = new CswDataTable( "viewpicklistdatatable", "" );
                //_ViewsForCBA.Columns.Add( "nodeviewid", typeof( Int32 ) );
                //_ViewsForCBA.Columns.Add( "View Name", typeof( string ) );
                //_ViewsForCBA.Columns.Add( "Include", typeof( bool ) );

                //if( SelectMode != PropertySelectMode.Multiple && !Required )
                //{
                //    DataRow NoneRow = _ViewsForCBA.NewRow();
                //    NoneRow["View Name"] = "[none]";
                //    NoneRow["nodeviewid"] = CswConvert.ToDbVal( Int32.MinValue );
                //    NoneRow["Include"] = ( Prop.AsViewPickList.SelectedViewIds.Count == 0 );
                //    _ViewsForCBA.Rows.Add( NoneRow );
                //}

                //string searchstr = CswNbtNodePropViewPickList.delimiter.ToString() + Prop.AsViewPickList.SelectedViewIds + CswNbtNodePropViewPickList.delimiter.ToString();
                //bool first = true;
                //foreach( DataRow ViewRow in Views.Rows )
                //{
                //    DataRow NewViewRow = _ViewsForCBA.NewRow();
                //    NewViewRow["View Name"] = ViewRow["viewname"].ToString();
                //    NewViewRow["nodeviewid"] = ViewRow["nodeviewid"].ToString();
                //    NewViewRow["Include"] = ( searchstr.IndexOf( CswNbtNodePropViewPickList.delimiter.ToString() + ViewRow["nodeviewid"].ToString() + CswNbtNodePropViewPickList.delimiter.ToString() ) >= 0 );
                //    NewViewRow["Include"] = ( ( searchstr.IndexOf( CswNbtNodePropNodeTypeSelect.delimiter.ToString() + ViewRow["nodeviewid"].ToString() + CswNbtNodePropNodeTypeSelect.delimiter.ToString() ) >= 0 ) ||
                //                              ( first && Required && Prop.AsViewPickList.SelectedViewIds.Count == 0 ) );
                //    first = false;
                //    _ViewsForCBA.Rows.Add( NewViewRow );
                //}

                _ViewsForCBA = Prop.AsViewPickList.ViewsForCBA();

                if( _AllowEditValue )
                {
                    _CBArray.UseRadios = ( SelectMode == PropertySelectMode.Single );
                    _CBArray.CreateCheckBoxes( _ViewsForCBA, "View Name", "nodeviewid" );
                }
            }
        }

        private DataTable _ViewsForCBA;

        public override void Save()
        {
            if( _AllowEditValue && !ReadOnly )
            {
                Prop.AsViewPickList.SelectedViewIds = _CBArray.GetCheckedValues( "Include" );
                if( SelectMode == PropertySelectMode.Single )
                {
                    foreach( DataRow ViewRow in _ViewsForCBA.Rows )
                    {
                        if( ViewRow["nodeviewid"].ToString() == Prop.AsViewPickList.SelectedViewIds[0] )
                            Prop.AsViewPickList.CachedViewNames[0] = ViewRow["View Name"].ToString();
                    }
                }
            }
        }
        public override void AfterSave()
        {
            DataBind();
        }
        public override void Clear()
        {
            _ValueLabel.Text = string.Empty;
            if( _CBArray != null )
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

            _ValueLabel.Text = SelectedViewNames().ToString();

            if( Prop != null && !_AllowEditValue )
            {
                if( Prop.NodeId != null )
                    _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                if( ReadOnly )
                    _EditButton.Visible = false;

                _ValueLabel.Text += "&nbsp;";
            }

            base.OnPreRender( e );
        }

        private CswCommaDelimitedString SelectedViewNames()
        {
            CswCommaDelimitedString ViewNames = new CswCommaDelimitedString();
            foreach( string ViewId in Prop.AsViewPickList.SelectedViewIds )
            {
                if( ViewId != string.Empty )
                {
                    foreach( DataRow ViewRow in _ViewsForCBA.Rows )
                    {
                        if( ViewRow["nodeviewid"].ToString() == ViewId )
                        {
                            ViewNames.Add( ViewRow["View Name"].ToString() );
                        }
                    }
                }
            }

            // Sort alphabetically
            ViewNames.Sort();

            return ViewNames;
        } // SelectedViewNames()
    }
}
