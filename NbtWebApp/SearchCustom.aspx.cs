using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.CswWebControls;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.CswWebControls;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.WebPages
{
    public partial class SearchCustom : System.Web.UI.Page
    {
        #region Page Lifecycle

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnInit( e );
        }

        private Literal _NodeTypeLiteral;
        private DropDownList _NodeTypeDropDown;
        private CswCheckBoxArray _PropCheckBoxArray;
        private Button _SearchButton;
        protected override void CreateChildControls()
        {
            try
            {

                CswCenteredDiv CenteredDiv = new CswCenteredDiv();
                centerph.Controls.Add( CenteredDiv );

                Label TitleLabel = new Label();
                TitleLabel.Text = "Build a Custom Search";
                TitleLabel.CssClass = "SearchLabel";
                CenteredDiv.Controls.Add( TitleLabel );

                CenteredDiv.Controls.Add( new CswLiteralBr() );
                CenteredDiv.Controls.Add( new CswLiteralBr() );

                CswAutoTable Table = new CswAutoTable();
                CenteredDiv.Controls.Add( Table );

                _NodeTypeLiteral = new Literal();
                _NodeTypeLiteral.Text = "What do you want to search?";
                Table.addControl( 0, 0, _NodeTypeLiteral );

                _NodeTypeDropDown = new DropDownList();
                _NodeTypeDropDown.ID = "NodeTypeDropDown";
                _NodeTypeDropDown.AutoPostBack = true;
                _NodeTypeDropDown.SelectedIndexChanged += new EventHandler( _NodeTypeDropDown_SelectedIndexChanged );
                _initNodeTypeOptions();
                Table.addControl( 1, 0, _NodeTypeDropDown );

                Table.addControl( 2, 0, new CswLiteralNbsp() );

                _PropCheckBoxArray = new CswCheckBoxArray( Master.CswNbtResources );
                _PropCheckBoxArray.ID = "PropCheckBoxArray";
                _PropCheckBoxArray.CheckboxesOnLeft = true;
                _PropCheckBoxArray.Visible = false;
                Table.addControl( 3, 0, _PropCheckBoxArray );

                _SearchButton = new Button();
                _SearchButton.ID = "SearchButton";
                _SearchButton.Text = "Search";
                _SearchButton.Visible = false;
                _SearchButton.Click += new EventHandler( _SearchButton_Click );
                Table.addControl( 4, 0, _SearchButton );
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.CreateChildControls();
        } // CreateChildControls()

        protected override void OnLoad( EventArgs e )
        {
            try
            {
                if( _SelectedValue != Int32.MinValue )
                {
                    initPropDataTable();
                    _PropCheckBoxArray.Visible = true;
                    _SearchButton.Visible = true;
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }

            base.OnLoad( e );
        }

        protected override void OnPreRender( EventArgs e )
        {
            try
            {
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
            base.OnPreRender( e );
        }

        #endregion Page Lifecycle

        #region Events


        void _NodeTypeDropDown_SelectedIndexChanged( object sender, EventArgs e )
        {
            // just need to trigger postback
        }

        void _SearchButton_Click( object sender, EventArgs e )
        {
            try
            {
                CswNbtView View = new CswNbtView( Master.CswNbtResources );
                View.ViewName = "Custom Search";
                View.ViewMode = NbtViewRenderingMode.List;

                CswNbtViewRelationship TopRel = null;
                if( _IsNodeTypeSelected() )
                    TopRel = View.AddViewRelationship( Master.CswNbtResources.MetaData.getNodeType( _SelectedValue ), true );
                else
                    TopRel = View.AddViewRelationship( Master.CswNbtResources.MetaData.getObjectClass( _SelectedValue ), true );

                string CheckedProps = _PropCheckBoxArray.GetCheckedValues( "Search" );
                string[] SplitCheckedProps = CheckedProps.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                if( SplitCheckedProps.Length > 0 )
                {
                    foreach( string PropId in SplitCheckedProps )
                    {
                        CswNbtMetaDataNodeTypeProp ThisProp = Master.CswNbtResources.MetaData.getNodeTypeProp( Convert.ToInt32( PropId ) );
                        CswNbtViewProperty ViewProp = View.AddViewProperty( TopRel, ThisProp );
                        View.AddViewPropertyFilter( ViewProp, ThisProp.FieldTypeRule.SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Begins, "", false );
                    }

                    Master.setViewXml( View.ToString() );
                    Master.Redirect( "Search.aspx" );
                }
            }
            catch( Exception ex )
            {
                Master.HandleError( ex );
            }
        } // _SearchButton_Click()

        #endregion Events

        #region NodeType DropDown

        private string _ObjectClassPrefix = "oc_";
        private string _NodeTypePrefix = "nt_";

        private bool _IsObjectClassSelected()
        {
            return _NodeTypeDropDown.SelectedValue.StartsWith( _ObjectClassPrefix );
        }
        private bool _IsNodeTypeSelected()
        {
            return _NodeTypeDropDown.SelectedValue.StartsWith( _NodeTypePrefix );
        }

        private Int32 _SelectedValue
        {
            get
            {
                Int32 ret = Int32.MinValue;
                if( _NodeTypeDropDown.SelectedValue != string.Empty )
                {
                    if( _IsNodeTypeSelected() )
                        ret = Convert.ToInt32( _NodeTypeDropDown.SelectedValue.Substring( _NodeTypePrefix.Length ) );
                    else
                        ret = Convert.ToInt32( _NodeTypeDropDown.SelectedValue.Substring( _ObjectClassPrefix.Length ) );
                }
                return ret;
            }
        }

        private void _initNodeTypeOptions()
        {
            _NodeTypeDropDown.Items.Clear();

            _NodeTypeDropDown.Items.Add( new ListItem( "" ) );

            foreach( CswNbtMetaDataNodeType LatestNodeType in Master.CswNbtResources.MetaData.LatestVersionNodeTypes )
            {
                if( Master.CswNbtResources.CurrentNbtUser.CheckPermission( NodeTypePermission.View, LatestNodeType.NodeTypeId, null, null ) )
                {
                    ListItem Item = new ListItem( LatestNodeType.NodeTypeName, _NodeTypePrefix + LatestNodeType.NodeTypeId.ToString() );
                    _NodeTypeDropDown.Items.Add( Item );
                }
            }

            foreach( CswNbtMetaDataObjectClass ObjectClass in Master.CswNbtResources.MetaData.ObjectClasses )
            {
                ListItem Item = new ListItem( "Any " + ObjectClass.ObjectClass, _ObjectClassPrefix + ObjectClass.ObjectClassId.ToString() );
                _NodeTypeDropDown.Items.Add( Item );
            }
        } // _initNodeTypeOptions()

        #endregion NodeType DropDown

        #region Prop CheckBoxArray
        // Similar, but not the same as in EditView.aspx.cs

        private CswDataTable _PropDataTable;
        private void initPropDataTable()
        {
            _PropDataTable = new CswDataTable( "propdatatable", "PropDataTable" );
            _PropDataTable.Columns.Add( "Property Name" );
            _PropDataTable.Columns.Add( "Search", typeof( bool ) );
            _PropDataTable.Columns.Add( "Value" );

            ICollection PropsCollection = null;
            if( _IsObjectClassSelected() )
                PropsCollection = _getObjectClassPropsCollection( _SelectedValue );
            else if( _IsNodeTypeSelected() )
                PropsCollection = _getNodeTypePropsCollection( _SelectedValue );
            else
                throw new CswDniException( "A Data Misconfiguration has occurred", "SearchCustom.aspx.cs::initPropDataTable() has a selected value which is neither a NodeType nor an ObjectClass" );

            foreach( CswNbtMetaDataNodeTypeProp ThisProp in PropsCollection )
            {
                // BZs 7085, 6651, 6644, 7092
                if( ThisProp.FieldTypeRule.SearchAllowed )
                {
                    DataRow PropRow = _PropDataTable.NewRow();
                    PropRow["Value"] = ThisProp.PropId;
                    PropRow["Property Name"] = ThisProp.PropName;
                    PropRow["Search"] = false; //( CurrentRelationship.Properties.Contains( ViewProp ) );
                    _PropDataTable.Rows.Add( PropRow );
                } // if( ThisProp.FieldTypeRule.SearchAllowed )

            } // foreach (DataRow Row in Props.Rows)
            _PropCheckBoxArray.CreateCheckBoxes( _PropDataTable, "Property Name", "Value" );
        }

        private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = Master.CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = Master.CswNbtResources.MetaData.getLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in ThisVersionNodeType.NodeTypeProps )
                {
                    //string ThisKey = ThisProp.PropName.ToLower(); //+ "_" + ThisProp.FirstPropVersionId.ToString();
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.PriorVersionNodeType;
            }
            return PropsByName.Values;
        }

        private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = Master.CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in NodeTypeProps )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower(); //+ "_" + NodeTypeProp.FirstPropVersionId.ToString();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }

        #endregion Prop CheckBoxArray
    }
}

