using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;


namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropFactory
    {
        //public static CswNbtNodePropWrapper makeNodeProp( CswNbtResources CswNbtResources, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
        //{
        //    CswNbtNodePropData CswNbtNodePropData = new CswNbtNodePropData( CswNbtResources, CswNbtMetaDataNodeTypeProp.PropId );
        //    return makeNodeProp( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
        //}

        public static CswNbtNodePropWrapper makeNodeProp( CswNbtResources CswNbtResources, DataRow PropRow, DataTable PropsTable, CswPrimaryKey NodeId, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
        {
            CswNbtNodePropData CswNbtNodePropData = new CswNbtNodePropData( CswNbtResources, PropRow, PropsTable, NodeId, CswNbtMetaDataNodeTypeProp.PropId );
            return makeNodeProp( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
        }

        private static CswNbtNodePropWrapper makeNodeProp( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
        {
            CswNbtNodePropWrapper ReturnVal = null;

            CswNbtNodeProp InnerProperty = null;
            switch( CswNbtMetaDataNodeTypeProp.FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                    InnerProperty = new CswNbtNodePropBarcode( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                    InnerProperty = new CswNbtNodePropComposite( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Date:
                    InnerProperty = new CswNbtNodePropDate( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.File:
                    InnerProperty = new CswNbtNodePropBlob( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                    InnerProperty = new CswNbtNodePropGrid( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Image:
                    InnerProperty = new CswNbtNodePropImage( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Link:
                    InnerProperty = new CswNbtNodePropLink( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.List:
                    InnerProperty = new CswNbtNodePropList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    InnerProperty = new CswNbtNodePropLocation( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.LocationContents:
                    InnerProperty = new CswNbtNodePropLocationContents( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                    InnerProperty = new CswNbtNodePropLogical( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                    InnerProperty = new CswNbtNodePropLogicalSet( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                    InnerProperty = new CswNbtNodePropMemo( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                    InnerProperty = new CswNbtNodePropMTBF( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                //case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
                //    InnerProperty = new CswNbtNodePropMultiRelationship( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                //    break;
                //case CswNbtMetaDataFieldType.NbtFieldType.NodeTypePermissions:
                //    InnerProperty = new CswNbtNodePropNodeTypePermissions(CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp);
                //    break;
                case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                    InnerProperty = new CswNbtNodePropNodeTypeSelect( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Number:
                    InnerProperty = new CswNbtNodePropNumber( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Password:
                    InnerProperty = new CswNbtNodePropPassword( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                    InnerProperty = new CswNbtNodePropPropertyReference( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                    InnerProperty = new CswNbtNodePropQuantity( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Question:
                    InnerProperty = new CswNbtNodePropQuestion(CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp);
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    InnerProperty = new CswNbtNodePropRelationship( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Static:
                    InnerProperty = new CswNbtNodePropStatic( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                    InnerProperty = new CswNbtNodePropSequence( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Text:
                    InnerProperty = new CswNbtNodePropText( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.Time:
                    InnerProperty = new CswNbtNodePropTime( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                    InnerProperty = new CswNbtNodePropTimeInterval( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.UserSelect:
                    InnerProperty = new CswNbtNodePropUserSelect( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                    InnerProperty = new CswNbtNodePropViewPickList( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                    InnerProperty = new CswNbtNodePropViewReference( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp );
                    break;
                default:
                    throw new CswDniException( "Internal System Error", "There is no CswNbtNodeProp Object for Field Type: " + CswNbtMetaDataNodeTypeProp.FieldType.FieldType.ToString() );
            }

            ReturnVal = new CswNbtNodePropWrapper( InnerProperty, CswNbtNodePropData );

            return ( ReturnVal );

        }//makeNodeProp()

    }//CswNbtNodePropFactory

}//namespace ChemSW.Nbt.PropTypes
