using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.PropTypes;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.CswWebControls;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswNodeTypeSelect : CswFieldTypeWebControl, INamingContainer//, IPostBackDataHandler
    {

        public PropertySelectMode SelectMode = PropertySelectMode.Single;

        private bool _AllowEditValue = false;

        public CswNodeTypeSelect( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            this.DataBinding += new EventHandler( CswNodeTypeSelect_DataBinding );
        }

        private void CswNodeTypeSelect_DataBinding( object sender, EventArgs e )
        {
            _AllowEditValue = ( _EditMode != NodeEditMode.Demo && _EditMode != NodeEditMode.PrintReport && !ReadOnly );

            EnsureChildControls();
            if( Prop != null )
            {
                SelectMode = Prop.AsNodeTypeSelect.SelectMode;
                if( _AllowEditValue )
                {
                    DataTable Data = new CswDataTable("nodetypeselectdatatable","");
                    Data.Columns.Add( "NodeType Name", typeof( string ) );
                    Data.Columns.Add( "nodetypeid", typeof( int ) );
                    Data.Columns.Add( "Include", typeof( bool ) );

                    if( SelectMode != PropertySelectMode.Multiple && !Required )
                    {
                        DataRow NTRow = Data.NewRow();
                        NTRow["NodeType Name"] = "[none]";
                        NTRow["nodetypeid"] = CswConvert.ToDbVal( Int32.MinValue );
                        NTRow["Include"] = ( Prop.AsNodeTypeSelect.SelectedNodeTypeIds.Count == 0 );
                        Data.Rows.Add( NTRow );
                    }

                    string searchstr = CswNbtNodePropNodeTypeSelect.delimiter.ToString() + Prop.AsNodeTypeSelect.SelectedNodeTypeIds + CswNbtNodePropNodeTypeSelect.delimiter.ToString();
                    bool first = true;
                    foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.LatestVersionNodeTypes )
                    {
                        DataRow NTRow = Data.NewRow();
                        NTRow["NodeType Name"] = NodeType.NodeTypeName;          // latest name
                        NTRow["nodetypeid"] = NodeType.FirstVersionNodeTypeId;   // first nodetypeid
                        NTRow["Include"] = ( ( searchstr.IndexOf( CswNbtNodePropNodeTypeSelect.delimiter.ToString() + NodeType.FirstVersionNodeTypeId + CswNbtNodePropNodeTypeSelect.delimiter.ToString() ) >= 0 ) ||
                                             ( first && Required && Prop.AsNodeTypeSelect.SelectedNodeTypeIds.Count == 0 ) );
                        Data.Rows.Add( NTRow );
                        first = false;
                    }

                    _CBArray.UseRadios = ( SelectMode == PropertySelectMode.Single );
                    _CBArray.CreateCheckBoxes( Data, "NodeType Name", "nodetypeid" );
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
                _EditButton = new CswImageButton(CswImageButton.ButtonType.Edit);
                _EditButton.ID = "edit";
                Table.addControl( 0, 1, _EditButton );
            }
                
            base.CreateChildControls();
        }


        protected override void OnPreRender(EventArgs e)
        {
            EnsureChildControls();

            if( Prop != null )
            {   
                _ValueLabel.Text = Prop.AsNodeTypeSelect.SelectedNodeTypeNames().ToString();
                if( !_AllowEditValue )
                {
                    if(Prop.NodeId != null)
                        _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
                    if( ReadOnly )
                        _EditButton.Visible = false;

                    _ValueLabel.Text += "&nbsp;";
                }
            }

            base.OnPreRender(e);
        }

    }
}
