using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Core;
using ChemSW.CswWebControls;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswQuantity : CswFieldTypeWebControl, INamingContainer
    {
        private Int32 _Precision;
        private double _MinValue;
        private double _MaxValue;

        private TextBox _QuantityTextBox;
        private Label _UnitLabel;
        private DropDownList _UnitList;
        private CswInvalidImage _InvalidImg;

        public CswQuantity( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, NodeEditMode EditMode )
            : base( CswNbtResources, CswNbtMetaDataNodeTypeProp, EditMode )
        {
            DataBinding += new EventHandler( CswQuantity_DataBinding );
        }

        protected override void OnInit( EventArgs e )
        {
            try
            {
                EnsureChildControls();
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
            base.OnInit( e );
        }

        private void CswQuantity_DataBinding( object sender, EventArgs e )
        {
            EnsureChildControls();
            if( Prop != null )
            {
                Quantity = Prop.AsQuantity.Quantity;

                _Precision = Prop.AsQuantity.Precision;
                _MinValue = Prop.AsQuantity.MinValue;
                _MaxValue = Prop.AsQuantity.MaxValue;

                _UnitLabel.Text = Prop.AsQuantity.CachedUnitName + "&nbsp;";

                if( Prop.AsQuantity.TargetType == NbtViewRelatedIdType.NodeTypeId && !ReadOnly )
                    ReadOnly = !( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( Prop.AsQuantity.TargetId ) ) );

                CswNbtMetaDataObjectClass Unit_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.UnitOfMeasureClass );
                CswNbtView View = new CswNbtView( _CswNbtResources );
                View.ViewName = "CswNbtNodePropQuantity()";
                View.AddViewRelationship( Unit_ObjectClass, true );

                if( View != null )
                {
                    _UnitList.Items.Clear();
                    _UnitList.Items.Add( new ListItem( "" ) );
                    ICswNbtTree CswNbtTree = _CswNbtResources.Trees.getTreeFromView( View, false, true, false, false );
                    for( Int32 c = 0; c < CswNbtTree.getChildNodeCount(); c++ )
                    {
                        CswNbtTree.goToNthChild( c );
                        _UnitList.Items.Add( new ListItem( CswNbtTree.getNodeNameForCurrentPosition(), CswNbtTree.getNodeIdForCurrentPosition().ToString() ) );
                        if( Prop.AsQuantity.UnitId == CswNbtTree.getNodeIdForCurrentPosition() )
                            _UnitList.SelectedValue = Prop.AsQuantity.UnitId.ToString();
                        CswNbtTree.goToParentNode();
                    }
                }
            }
        }

        public override void Save()
        {
            if( !ReadOnly )
            {
                Prop.AsQuantity.Quantity = Quantity;
                try
                {
                    if( !ReadOnly )
                    {
                        if( _UnitList.SelectedValue != string.Empty )
                        {
                            Prop.AsQuantity.UnitId = SelectedUnitId;
                            Prop.AsQuantity.CachedUnitName = SelectedUnitName;
                        }
                        else
                        {
                            Prop.AsQuantity.UnitId = null;
                            Prop.AsQuantity.CachedUnitName = String.Empty;
                        }
                    }
                }
                catch( Exception ex )
                {
                    HandleError( ex );
                }
            }
        }

        public override void AfterSave()
        {
            DataBind();
        }

        public override void Clear()
        {
            try
            {
                Quantity = Int32.MinValue;
                _UnitLabel.Text = string.Empty;
                _UnitList.SelectedValue = null;
                _UnitList.Text = string.Empty;
            }
            catch( Exception ex )
            {
                HandleError( ex );
            }
        }

        public double Quantity
        {
            get
            {
                double q = Double.NaN;
                if( CswTools.IsFloat( _QuantityTextBox.Text ) )
                    q = Convert.ToDouble( _QuantityTextBox.Text );
                return q;
            }
            set
            {
                if( Double.IsNaN( value ) )
                    _QuantityTextBox.Text = string.Empty;
                else
                    _QuantityTextBox.Text = value.ToString();
            }
        }

        public CswPrimaryKey SelectedUnitId
        {
            get
            {
                CswPrimaryKey ret = new CswPrimaryKey();
                ret.FromString( _UnitList.SelectedValue );
                return ret;
            }
        }
        public string SelectedUnitName
        {
            get
            {
                return _UnitList.SelectedItem.Text;
            }
        }

        protected override void CreateChildControls()
        {
            CswAutoTable Table = new CswAutoTable();
            this.Controls.Add( Table );

            _QuantityTextBox = new TextBox();
            _QuantityTextBox.ID = "qty";
            _QuantityTextBox.CssClass = TextBoxCssClass;
            _QuantityTextBox.Width = 60;
            Table.addControl( 0, 0, _QuantityTextBox );

            _UnitList = new DropDownList();
            _UnitList.ID = "relval";
            _UnitList.CssClass = DropDownCssClass;
            Table.addControl( 0, 1, _UnitList );

            _UnitLabel = new Label();
            _UnitLabel.ID = "rellabel";
            _UnitLabel.CssClass = StaticTextCssClass;
            Table.addControl( 0, 1, _UnitLabel );

            Table.addControl( 0, 2, new CswLiteralNbsp() );

            _InvalidImg = new CswInvalidImage();
            _InvalidImg.ID = "InvalidImg";
            Table.addControl( 0, 4, _InvalidImg );

            base.CreateChildControls();

            if( Required )
            {
                _RequiredValidator.Visible = true;
                _RequiredValidator.Enabled = true;
                _RequiredValidator.ControlToValidate = _QuantityTextBox.ID;
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            string MinValString = string.Empty;
            if( _MinValue != Int32.MinValue )
                MinValString = _MinValue.ToString();
            string MaxValString = string.Empty;
            if( _MaxValue != Int32.MinValue )
                MaxValString = _MaxValue.ToString();

            _QuantityTextBox.Attributes.Add( "onkeypress", "CswQuantity_onchange('" + _QuantityTextBox.ClientID + "', '" + _UnitList.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );
            _QuantityTextBox.Attributes.Add( "onchange", "CswQuantity_onchange('" + _QuantityTextBox.ClientID + "', '" + _UnitList.ClientID + "', '" + _InvalidImg.ClientID + "', '" + _Precision.ToString() + "', '" + MinValString + "', '" + MaxValString + "');" );

            try
            {
                _UnitList.Attributes.Add( "onchange", "CswFieldTypeWebControl_onchange()" );

                if( ReadOnly )
                {
                    _UnitLabel.Visible = true;
                    _UnitList.Visible = false;
                    _UnitList.Enabled = false;
                }
                else
                {
                    _UnitLabel.Visible = false;
                    _UnitList.Visible = true;
                }

            }
            catch( Exception ex )
            {
                HandleError( ex );
            }

            base.OnPreRender( e );
        }

        protected override void RenderContents( HtmlTextWriter output )
        {
            EnsureChildControls();

            if( ReadOnly )
            {
                output.Write( Quantity );
                output.Write( "&nbsp;" );
                output.Write( SelectedUnitName );
            }
            else
            {
                base.RenderContents( output );
            }
        }
    }
}

