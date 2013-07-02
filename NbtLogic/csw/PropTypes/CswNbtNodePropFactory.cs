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
            CswNbtNodePropData CswNbtNodePropData = new CswNbtNodePropData( CswNbtResources, PropRow, PropsTable, NodeId, CswNbtMetaDataNodeTypeProp );
            return _makeNodeProp( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
        }

        private static CswNbtNodePropWrapper _makeNodeProp( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
        {
            CswNbtNodePropWrapper ReturnVal = null;

            CswNbtNodeProp InnerProperty = null;
            CswEnumNbtFieldType FieldType = CswNbtMetaDataNodeTypeProp.getFieldTypeValue();
            switch( FieldType )
            {
                case CswEnumNbtFieldType.Barcode:
                    InnerProperty = new CswNbtNodePropBarcode( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Button:
                    InnerProperty = new CswNbtNodePropButton( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.CASNo:
                    InnerProperty = new CswNbtNodePropCASNo( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.ChildContents:
                    InnerProperty = new CswNbtNodePropChildContents( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Comments:
                    InnerProperty = new CswNbtNodePropComments( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Composite:
                    InnerProperty = new CswNbtNodePropComposite( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.DateTime:
                    InnerProperty = new CswNbtNodePropDateTime( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.File:
                    InnerProperty = new CswNbtNodePropBlob( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Grid:
                    InnerProperty = new CswNbtNodePropGrid( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Image:
                    InnerProperty = new CswNbtNodePropImage( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.ImageList:
                    InnerProperty = new CswNbtNodePropImageList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Link:
                    InnerProperty = new CswNbtNodePropLink( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.List:
                    InnerProperty = new CswNbtNodePropList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Location:
                    InnerProperty = new CswNbtNodePropLocation( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.LocationContents:
                    InnerProperty = new CswNbtNodePropLocationContents( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Logical:
                    InnerProperty = new CswNbtNodePropLogical( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.LogicalSet:
                    InnerProperty = new CswNbtNodePropLogicalSet( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.MetaDataList:
                    InnerProperty = new CswNbtNodePropMetaDataList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Memo:
                    InnerProperty = new CswNbtNodePropMemo( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.MOL:
                    InnerProperty = new CswNbtNodePropMol( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.MTBF:
                    InnerProperty = new CswNbtNodePropMTBF( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                //case CswEnumNbtFieldType.MultiRelationship:
                //    InnerProperty = new CswNbtNodePropMultiRelationship( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                //    break;
                case CswEnumNbtFieldType.MultiList:
                    InnerProperty = new CswNbtNodePropMultiList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.NFPA:
                    InnerProperty = new CswNbtNodePropNFPA( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.NodeTypeSelect:
                    InnerProperty = new CswNbtNodePropNodeTypeSelect( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Number:
                    InnerProperty = new CswNbtNodePropNumber( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Password:
                    InnerProperty = new CswNbtNodePropPassword( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.PropertyReference:
                    InnerProperty = new CswNbtNodePropPropertyReference( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Quantity:
                    InnerProperty = new CswNbtNodePropQuantity( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Question:
                    InnerProperty = new CswNbtNodePropQuestion( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Relationship:
                    InnerProperty = new CswNbtNodePropRelationship( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Scientific:
                    InnerProperty = new CswNbtNodePropScientific( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Sequence:
                    InnerProperty = new CswNbtNodePropSequence( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Static:
                    InnerProperty = new CswNbtNodePropStatic( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.Text:
                    InnerProperty = new CswNbtNodePropText( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                //case CswEnumNbtFieldType.Time:
                //    InnerProperty = new CswNbtNodePropTime( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                //    break;
                case CswEnumNbtFieldType.TimeInterval:
                    InnerProperty = new CswNbtNodePropTimeInterval( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.UserSelect:
                    InnerProperty = new CswNbtNodePropUserSelect( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.ViewPickList:
                    InnerProperty = new CswNbtNodePropViewPickList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                case CswEnumNbtFieldType.ViewReference:
                    InnerProperty = new CswNbtNodePropViewReference( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node );
                    break;
                default:
                    throw new CswDniException( CswEnumErrorType.Error, "Internal System Error", "There is no CswNbtNodeProp Object for Field Type: " + FieldType.ToString() );
            }

            ReturnVal = new CswNbtNodePropWrapper( CswNbtResources, Node, InnerProperty, CswNbtNodePropData );

            return ( ReturnVal );

        }//makeNodeProp()

    }//CswNbtNodePropFactory

}//namespace ChemSW.Nbt.PropTypes
