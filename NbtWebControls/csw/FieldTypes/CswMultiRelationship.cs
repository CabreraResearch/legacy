//using System;
//using System.Collections.Specialized;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Text;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Data;
//using ChemSW.Nbt;
//using ChemSW.Exceptions;
//using ChemSW.NbtWebControls;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Core;
//using Telerik.Web.UI;
//using ChemSW.Nbt.MetaData;
//using ChemSW.Nbt.ObjClasses;

//namespace ChemSW.NbtWebControls.FieldTypes
//{
//    public class CswMultiRelationship : CswFieldTypeWebControl, INamingContainer
//    {
//        private bool AllowEditValue = false;

//        //private CswNbtNodeKey _SelectedNodeKey;

//        public CswMultiRelationship( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode, bool paramAllowEditValue )
//            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
//        {
//            AllowEditValue = paramAllowEditValue;
//            this.DataBinding += new EventHandler( CswMultiRelationship_DataBinding );
//        }

//        protected override void OnInit( EventArgs e )
//        {
//            EnsureChildControls();
//            base.OnInit( e );
//        }

//        private DataTable _Data;
//        private void CswMultiRelationship_DataBinding( object sender, EventArgs e )
//        {
//            EnsureChildControls();

//            _ValueLabel.Text = Prop.AsMultiRelationship.CommaSeparatedNodeNames;

//            if (AllowEditValue)
//            {
//                CswNbtView View = Prop.AsMultiRelationship.View;
//                if( View != null )
//                {
//                    // Filter out my node
//                    ( (CswNbtViewRelationship) View.Root.ChildRelationships[0] ).NodeIdsToFilterOut.Add( NodeKey.NodeId );
//                    ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );

//                    // Make a DataTable for CswCheckBoxArray
//                    _Data = new DataTable();
//                    _Data.Columns.Add( "Node Name", typeof( string ) );
//                    _Data.Columns.Add( "Node Id", typeof( Int32 ) );
//                    _Data.Columns.Add( "Related", typeof( bool ) );

//                    Tree.goToRoot();
//                    _addRowsToDataTableRecursive( Tree );

//                    _CheckBoxArray.CheckboxesOnLeft = true;
//                    _CheckBoxArray.CreateCheckBoxes( _Data, "Node Name", "Node Id" );
//                }
//            }
//        }

//        private void _addRowsToDataTableRecursive( ICswNbtTree Tree )
//        {
//            for( Int32 i = 0; i < Tree.getChildNodeCount(); i++ )
//            {
//                Tree.goToNthChild( i );
//                CswNbtNode CurrentNode = Tree.getNodeForCurrentPosition();
//                DataRow NewRow = _Data.NewRow();
//                NewRow["Node Name"] = CurrentNode.NodeName;
//                NewRow["Node Id"] = CurrentNode.NodeId;
//                NewRow["Related"] = Prop.AsMultiRelationship.IsRelated( CurrentNode.NodeId );
//                _Data.Rows.Add( NewRow );
//                Tree.goToParentNode();
//            }
//        }

        
//        public override void Save()
//        {
//            if( _hiddenClear.Value == "1" )
//            {
//                _Prop.ClearValue();
//            }
//            else
//            {
//                if( AllowEditValue )
//                {
//                    foreach( DataRow Row in _Data.Rows )
//                    {
//                        if( _CheckBoxArray.GetValue( Row["NodeId"].ToString(), "Related" ) )
//                            Prop.AsMultiRelationship.AddRelatedNode( CswConvert.ToInt32( Row["Node Id"].ToString() ), Row["Node Name"].ToString() );
//                        else
//                            Prop.AsMultiRelationship.RemoveRelatedNode( CswConvert.ToInt32( Row["Node Id"].ToString() ) );
//                    }
//                    Prop.AsMultiRelationship.Save();
//                }
//            }
//        }
//        public override void AfterSave()
//        {
//            DataBind();
//        }

//        public override void Clear()
//        {
//            foreach( DataRow Row in _Data.Rows )
//            {
//                _CheckBoxArray.GetCheckBox( Row["NodeId"].ToString(), "Related" ).Checked = false;
//            }
//        }

//        private Label _ValueLabel;
//        private CswImageButton _EditButton;
//        private CswImageButton _ClearButton;
//        private HiddenField _hiddenClear;
//        private CswCheckBoxArray _CheckBoxArray;

//        protected override void CreateChildControls()
//        {
//            _ValueLabel = new Label();
//            _ValueLabel.ID = "relval";
//            this.Controls.Add( _ValueLabel );

//            _EditButton = new CswImageButton( CswImageButton.ButtonType.Edit );
//            _EditButton.ID = "reledit";
//            this.Controls.Add( _EditButton );

//            _ClearButton = new CswImageButton( CswImageButton.ButtonType.Clear );
//            _ClearButton.ID = "relclearbutton";
//            this.Controls.Add( _ClearButton );

//            _hiddenClear = new HiddenField();
//            _hiddenClear.ID = "hiddenclear";
//            this.Controls.Add( _hiddenClear );

//            _CheckBoxArray = new CswCheckBoxArray( _CswNbtResources );
//            _CheckBoxArray.ID = "MRCBA";
//            this.Controls.Add( _CheckBoxArray );

//            base.CreateChildControls();

//            if( !( AllowEditValue && !ReadOnly ) )
//                _RequiredValidator.ControlToValidate = _ValueLabel.ID;
//        }

//        protected override void OnPreRender( EventArgs e )
//        {
//            _EditButton.OnClientClick = "EditPropertyInPopup('" + Prop.NodeId.ToString() + "', '" + PropId.ToString() + "');";
//            _ClearButton.OnClientClick = "return CswRelationship_clear('" + _ValueLabel.ClientID + "', '" + _hiddenClear.ClientID + "');";

//            _ValueLabel.Visible = true;
//            _EditButton.Visible = true;
//            _CheckBoxArray.Visible = false;
//            _ClearButton.Visible = true;

//            if( AllowEditValue && !ReadOnly )
//            {
//                _CheckBoxArray.Visible = true;
//                _EditButton.Visible = false;
//                _ClearButton.Visible = false;
//                _ValueLabel.Visible = false;
//            }
//            if( ReadOnly )
//            {
//                _EditButton.Visible = false;
//                _ClearButton.Visible = false;
//            }
//            if( Required )
//            {
//                _ClearButton.Visible = false;
//            }

//            base.OnPreRender( e );
//        }
//    }
//}
