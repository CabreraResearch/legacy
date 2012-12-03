using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleFactory
    {
        public static ICswNbtFieldTypeRule makeRule( CswNbtFieldResources CswNbtFieldResources, CswNbtMetaDataFieldType.NbtFieldType FieldType )
        {
            ICswNbtFieldTypeRule ReturnVal = null;
            //CswNbtFieldResources CswNbtFieldResources = new CswNbtFieldResources( CswNbtResources );

            switch( FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                    ReturnVal = new CswNbtFieldTypeRuleBarCode( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Button:
                    ReturnVal = new CswNbtFieldTypeRuleButton( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.CASNo:
                    ReturnVal = new CswNbtFieldTypeRuleCASNo( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Comments:
                    ReturnVal = new CswNbtFieldTypeRuleComments( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                    ReturnVal = new CswNbtFieldTypeRuleComposite( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    ReturnVal = new CswNbtFieldTypeRuleDateTime( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.External:
                    ReturnVal = new CswNbtFieldTypeRuleExternal( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.File:
                    ReturnVal = new CswNbtFieldTypeRuleBlob( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                    ReturnVal = new CswNbtFieldTypeRuleGrid( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Image:
                    ReturnVal = new CswNbtFieldTypeRuleImage( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ImageList:
                    ReturnVal = new CswNbtFieldTypeRuleImageList( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Link:
                    ReturnVal = new CswNbtFieldTypeRuleLink( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.List:
                    ReturnVal = new CswNbtFieldTypeRuleList( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    ReturnVal = new CswNbtFieldTypeRuleLocation( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.LocationContents:
                    ReturnVal = new CswNbtFieldTypeRuleLocationContents( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                    ReturnVal = new CswNbtFieldTypeRuleLogical( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                    ReturnVal = new CswNbtFieldTypeRuleLogicalSet( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                    ReturnVal = new CswNbtFieldTypeRuleMemo( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                    ReturnVal = new CswNbtFieldTypeRuleMol( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                    ReturnVal = new CswNbtFieldTypeRuleMTBF( CswNbtFieldResources );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
                //    ReturnVal = new CswNbtFieldTypeRuleMultiRelationship( CswNbtFieldResources );
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.MultiList:
                    ReturnVal = new CswNbtFieldTypeRuleMultiList( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.NFPA:
                    ReturnVal = new CswNbtFieldTypeRuleNFPA( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
                    ReturnVal = new CswNbtFieldTypeRuleNodeTypeSelect( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Number:
                    ReturnVal = new CswNbtFieldTypeRuleNumber( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Password:
                    ReturnVal = new CswNbtFieldTypeRulePassword( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                    ReturnVal = new CswNbtFieldTypeRulePropertyReference( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                    ReturnVal = new CswNbtFieldTypeRuleQuantity( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Question:
                    ReturnVal = new CswNbtFieldTypeRuleQuestion( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    ReturnVal = new CswNbtFieldTypeRuleRelationship( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Scientific:
                    ReturnVal = new CswNbtFieldTypeRuleScientific( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
                    ReturnVal = new CswNbtFieldTypeRuleSequence( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Static:
                    ReturnVal = new CswNbtFieldTypeRuleStatic( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Text:
                    ReturnVal = new CswNbtFieldTypeRuleText( CswNbtFieldResources );
                    break;

                //case CswNbtMetaDataFieldType.NbtFieldType.Time:
                //    ReturnVal = new CswNbtFieldTypeRuleTime( CswNbtFieldResources );
                //    break;

                case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                    ReturnVal = new CswNbtFieldTypeRuleTimeInterval( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.UserSelect:
                    ReturnVal = new CswNbtFieldTypeRuleUserSelect( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                    ReturnVal = new CswNbtFieldTypeRuleViewPickList( CswNbtFieldResources );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                    ReturnVal = new CswNbtFieldTypeRuleViewReference( CswNbtFieldResources );
                    break;

                default:
                    throw ( new CswDniException( "There is no field rule class for field type " + FieldType.ToString() ) );
                //break;
            }//switch


            return ( ReturnVal );

        }//makeRule() 

    }//CswNbtFieldTypeRuleFactory

}//namespace ChemSW.Nbt.MetaData
