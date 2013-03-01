using System.Data;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropFactory
    {
        //public static CswNbtNodePropWrapper makeNodeProp( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
        //{
        //    CswNbtNodePropData CswNbtNodePropData = new CswNbtNodePropData( CswNbtResources, CswNbtMetaDataNodeTypeProp.PropId );
        //    return makeNodeProp( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
        //}

        //public static CswNbtNodePropWrapper makeNodeProp( CswNbtResources CswNbtResources, DataRow PropRow, DataTable PropsTable, CswPrimaryKey NodeId, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtMetaDataNodeTypeTab Tab, NodeEditMode EditMode = NodeEditMode.Edit )
        //{
        //    CswNbtNodePropData CswNbtNodePropData = new CswNbtNodePropData( CswNbtResources, PropRow, PropsTable, NodeId, CswNbtMetaDataNodeTypeProp.PropId );
        //    CswNbtNode Node = CswNbtResources.Nodes.GetNode( NodeId );
        //    return _makeNodeProp( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node, Tab, EditMode );
        //}

        public static CswNbtNodePropWrapper makeNodeProp( CswNbtResources CswNbtResources, DataRow PropRow, DataTable PropsTable, CswNbtNode Node, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
        {
            CswPrimaryKey NodeId = null;
            if( null != Node )
            {
                NodeId = Node.NodeId;
            }
            CswNbtNodePropData CswNbtNodePropData = new CswNbtNodePropData( CswNbtResources, PropRow, PropsTable, NodeId, CswNbtMetaDataNodeTypeProp.PropId );
            return _makeNodeProp( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
        }

        private static CswNbtNodePropWrapper _makeNodeProp( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
        {
            CswNbtNodePropWrapper ReturnVal = null;

            CswNbtNodeProp InnerProperty = null;
            CswNbtMetaDataFieldType.NbtFieldType FieldType = CswNbtMetaDataNodeTypeProp.getFieldTypeValue();
            switch( FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                    InnerProperty = new CswNbtNodePropBarcode( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Button:
                    InnerProperty = new CswNbtNodePropButton( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.CASNo:
                    InnerProperty = new CswNbtNodePropCASNo( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ChildContents:
                    InnerProperty = new CswNbtNodePropChildContents( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Comments:
                    InnerProperty = new CswNbtNodePropComments( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                    InnerProperty = new CswNbtNodePropComposite( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    InnerProperty = new CswNbtNodePropDateTime( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.File:
                    InnerProperty = new CswNbtNodePropBlob( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                    InnerProperty = new CswNbtNodePropGrid( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Image:
                    InnerProperty = new CswNbtNodePropImage( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ImageList:
                    InnerProperty = new CswNbtNodePropImageList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Link:
                    InnerProperty = new CswNbtNodePropLink( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.List:
                    InnerProperty = new CswNbtNodePropList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    InnerProperty = new CswNbtNodePropLocation( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.LocationContents:
                    InnerProperty = new CswNbtNodePropLocationContents( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                    InnerProperty = new CswNbtNodePropLogical( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                    InnerProperty = new CswNbtNodePropLogicalSet( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                    InnerProperty = new CswNbtNodePropMemo( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                    InnerProperty = new CswNbtNodePropMol( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                    InnerProperty = new CswNbtNodePropMTBF( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                //case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
                //    InnerProperty = new CswNbtNodePropMultiRelationship( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                //    break;
                case CswNbtMetaDataFieldType.NbtFieldType.MultiList:
                    InnerProperty = new CswNbtNodePropMultiList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.NFPA:
                    InnerProperty = new CswNbtNodePropNFPA( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                    InnerProperty = new CswNbtNodePropNodeTypeSelect( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Number:
                    InnerProperty = new CswNbtNodePropNumber( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Password:
                    InnerProperty = new CswNbtNodePropPassword( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                    InnerProperty = new CswNbtNodePropPropertyReference( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                    InnerProperty = new CswNbtNodePropQuantity( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Question:
                    InnerProperty = new CswNbtNodePropQuestion( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    InnerProperty = new CswNbtNodePropRelationship( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Scientific:
                    InnerProperty = new CswNbtNodePropScientific( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                    InnerProperty = new CswNbtNodePropSequence( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Static:
                    InnerProperty = new CswNbtNodePropStatic( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Text:
                    InnerProperty = new CswNbtNodePropText( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                //case CswNbtMetaDataFieldType.NbtFieldType.Time:
                //    InnerProperty = new CswNbtNodePropTime( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                //    break;
                case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                    InnerProperty = new CswNbtNodePropTimeInterval( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.UserSelect:
                    InnerProperty = new CswNbtNodePropUserSelect( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                    InnerProperty = new CswNbtNodePropViewPickList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                    InnerProperty = new CswNbtNodePropViewReference( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                default:
                    throw new CswDniException( ErrorType.Error, "Internal System Error", "There is no CswNbtNodeProp Object for Field Type: " + FieldType.ToString() );
            }

            ReturnVal = new CswNbtNodePropWrapper( CswNbtResources, Node, InnerProperty, CswNbtNodePropData );

            return ( ReturnVal );

        }//makeNodeProp()

    }//CswNbtNodePropFactory

}//namespace ChemSW.Nbt.PropTypes
