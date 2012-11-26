using System.Web.UI;
using System.Web.UI.WebControls;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.NbtWebControls.FieldTypes
{
    public class CswFieldTypeWebControlFactory
    {
        //private Hashtable PropToControl = new Hashtable();

        //private CswNbtResources CswNbtResources;
        //public CswFieldTypeWebControlFactory( CswNbtResources Rsc )
        //{
        //    CswNbtResources = Rsc;
        //}

        /// <summary>
        /// Make a FieldTypeWebControl for a non-Node based property value (like Default Value)
        /// </summary>
        public static CswFieldTypeWebControl makeControl( CswNbtResources CswNbtResources, ControlCollection Controls, string ControlId, CswNbtNodePropWrapper PropWrapper, NodeEditMode EditMode, CswErrorHandler HandleError )
        {
            CswFieldTypeWebControl Control = makeControl( CswNbtResources, Controls, PropWrapper.NodeTypeProp, EditMode, HandleError );
            if( Control != null )
            {
                Control.Prop = PropWrapper;
                Control.Required = PropWrapper.TemporarilyRequired;  // this does not override MetaDataProp.Required
                if( ControlId != string.Empty )
                    ( (WebControl) Control ).ID = ControlId;
                else
                    ( (WebControl) Control ).ID = "prop_0_" + PropWrapper.NodeTypeProp.PropId.ToString();
                _setReadOnly( CswNbtResources, Control, PropWrapper, null, EditMode );
            }
            return Control;
        }

        /// <summary>
        /// Make a FieldTypeWebControl for a Node based property value
        /// </summary>
        public static CswFieldTypeWebControl makeControl( CswNbtResources CswNbtResources, ControlCollection Controls, string ControlId, CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtNode Node, NodeEditMode EditMode, CswErrorHandler HandleError )
        {
            // This screws up add node
            //if( NodeKey.NodeId <= 0 )
            //    throw new CswDniException( "Invalid NodeKey for Property", "CswFieldTypeWebControlFactory.makeControl() got an invalid NodeKey: " + NodeKey.ToString() );

            // This also screws up add node
            //if( Node == null )
            //    throw new CswDniException( "Invalid Node", "CswFieldTypeWebControlFactory.makeControl() got a null Node" );

            CswFieldTypeWebControl Control = makeControl( CswNbtResources, Controls, MetaDataProp, EditMode, HandleError );
            //Control.NodeKey = NodeKey;
            if( Control != null )
            {
                CswNbtNodePropWrapper PropWrapper = null;
                if( Node != null )
                    PropWrapper = Node.Properties[MetaDataProp];
                Control.Prop = PropWrapper;
                Control.Required = PropWrapper.TemporarilyRequired;  // this does not override MetaDataProp.Required
                if( ControlId != string.Empty )
                    ( (WebControl) Control ).ID = ControlId;
                else if( Node != null && Node.NodeId != null )
                    ( (WebControl) Control ).ID = "prop_" + Node.NodeId.PrimaryKey.ToString() + "_" + MetaDataProp.PropId.ToString();
                else
                    ( (WebControl) Control ).ID = "prop_0_" + MetaDataProp.PropId.ToString();
                _setReadOnly( CswNbtResources, Control, PropWrapper, Node, EditMode );
            }
            return Control;
        }

        private static CswFieldTypeWebControl makeControl( CswNbtResources CswNbtResources, ControlCollection Controls, CswNbtMetaDataNodeTypeProp MetaDataProp, NodeEditMode EditMode, CswErrorHandler HandleError )
        {
            CswFieldTypeWebControl Control = null;

            switch( MetaDataProp.getFieldType().FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                    Control = new CswBarcode( CswNbtResources, MetaDataProp, EditMode ); //, (EditMode == NodeEditMode.AddInPopup));
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.CASNo:
                    Control = new CswCASNo( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                    Control = new CswComposite( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    Control = new CswDate( CswNbtResources, MetaDataProp, EditMode );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.External:
                //    //Control = new CswExternal(CswNbtResources, MetaDataProp);
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.File:
                    Control = new CswFile( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                    if( EditMode != NodeEditMode.Add && EditMode != NodeEditMode.Demo )
                    {
                        Control = new CswGrid( CswNbtResources, MetaDataProp, EditMode );
                    }
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Image:
                    Control = new CswImage( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Link:
                    Control = new CswLink( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.List:
                    Control = new CswList( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    Control = new CswLocation( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.LocationContents:
                    Control = new CswLocationContents( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                    Control = new CswLogical( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                    Control = new CswLogicalSet( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                    Control = new CswMemo( CswNbtResources, MetaDataProp, EditMode );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.NodeTypePermissions:
                //    Control = new CswNodeTypePermissions( CswNbtResources, MetaDataProp );
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                    Control = new CswNodeTypeSelect( CswNbtResources, MetaDataProp, EditMode );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                //    //Control = new CswJMol(CswNbtResources, MetaDataProp);
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                    Control = new CswMTBF( CswNbtResources, MetaDataProp, EditMode );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
                //    Control = new CswMultiRelationship( CswNbtResources, MetaDataProp, ( EditMode != NodeEditMode.Edit ) );
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Number:
                    Control = new CswNumber( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Password:
                    Control = new CswPassword( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                    Control = new CswPropertyReference( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                    Control = new CswQuantity( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Question:
                    Control = new CswQuestion( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    Control = new CswRelationship( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                    Control = new CswSequence( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Static:
                    Control = new CswStatic( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Text:
                    Control = new CswText( CswNbtResources, MetaDataProp, EditMode );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.Time:
                //    Control = new CswTime( CswNbtResources, MetaDataProp, EditMode );
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                    Control = new CswTimeInterval( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.UserSelect:
                    Control = new CswUserSelect( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                    Control = new CswViewPickList( CswNbtResources, MetaDataProp, EditMode );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                    Control = new CswViewReference( CswNbtResources, MetaDataProp, EditMode );
                    break;

                //default:
                //    throw new CswDniException("Invalid property field type", "CswFieldTypeWebControlFactory could not find a matching control for field type: " + MetaDataProp.FieldType.FieldType.ToString());
            }

            if( Control != null )
            {
                //( (WebControl) Control ).DataBinding += new EventHandler( ControlDataBinding );
                Control.OnError += new CswErrorHandler( HandleError );
                Controls.Add( (WebControl) Control );
            }

            //if( PropToControl.ContainsKey( MetaDataProp ) )
            //    PropToControl[MetaDataProp] = (CswFieldTypeWebControl) Control;
            //else
            //    PropToControl.Add( MetaDataProp, (CswFieldTypeWebControl) Control );

            return Control;

        }//makeControl()


        private static void _setReadOnly( CswNbtResources CswNbtResources, CswFieldTypeWebControl Control, CswNbtNodePropWrapper PropWrapper, CswNbtNode Node, NodeEditMode EditMode )
        {
            if( false == Control.ReadOnly )
            {
                Control.ReadOnly = ( ( EditMode == NodeEditMode.PrintReport || EditMode == NodeEditMode.AuditHistoryInPopup ) ||
                                     ( PropWrapper != null /*&& PropWrapper.IsReadOnly() */ ) );
            }

            // BZ 8307
            if( EditMode == NodeEditMode.DefaultValue )
                Control.ReadOnly = false;
        }

        //public delegate void ErrorHandler( Exception ex );
        //public event ErrorHandler OnError;

        //protected void HandleError( Exception ex )
        //{
        //    if( OnError != null )
        //        OnError( ex );
        //    else                  // this else case prevents us from not seeing exceptions if the error handling mechanism is not attached
        //        throw ex;
        //}

        //protected void ControlDataBinding( object sender, EventArgs e )
        //{
        //    CswNbtResources.logMessage( "Called Databind on Control: " + sender.GetType().ToString() );
        //}

        //public CswFieldTypeWebControl getControlForProp( CswNbtMetaDataNodeTypeProp MetaDataProp )
        //{
        //    CswFieldTypeWebControl ret = null;
        //    if( PropToControl.ContainsKey( MetaDataProp ) )
        //        ret = (CswFieldTypeWebControl) PropToControl[MetaDataProp];
        //    return ret;
        //}

    } // class CswFieldTypeWebControlFactory
} // namespace ChemSW.NbtWebControls.FieldTypes

