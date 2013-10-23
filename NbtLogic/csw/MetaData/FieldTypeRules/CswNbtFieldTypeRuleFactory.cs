using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleFactory
    {
        public static ICswNbtFieldTypeRule makeRule( CswNbtFieldResources CswNbtFieldResources, CswEnumNbtFieldType FieldType )
        {
            ICswNbtFieldTypeRule ReturnVal = null;
            //CswNbtFieldResources CswNbtFieldResources = new CswNbtFieldResources( CswNbtResources );

            switch( FieldType )
            {
                case CswEnumNbtFieldType.Barcode:
                    ReturnVal = new CswNbtFieldTypeRuleBarCode( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Button:
                    ReturnVal = new CswNbtFieldTypeRuleButton( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.CASNo:
                    ReturnVal = new CswNbtFieldTypeRuleCASNo( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.ChildContents:
                    ReturnVal = new CswNbtFieldTypeRuleChildContents( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Comments:
                    ReturnVal = new CswNbtFieldTypeRuleComments( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Composite:
                    ReturnVal = new CswNbtFieldTypeRuleComposite( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.DateTime:
                    ReturnVal = new CswNbtFieldTypeRuleDateTime( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.External:
                    ReturnVal = new CswNbtFieldTypeRuleExternal( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.File:
                    ReturnVal = new CswNbtFieldTypeRuleBlob( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Formula:
                    ReturnVal = new CswNbtFieldTypeRuleFormula( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Grid:
                    ReturnVal = new CswNbtFieldTypeRuleGrid( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Image:
                    ReturnVal = new CswNbtFieldTypeRuleImage( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.ImageList:
                    ReturnVal = new CswNbtFieldTypeRuleImageList( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Link:
                    ReturnVal = new CswNbtFieldTypeRuleLink( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.List:
                    ReturnVal = new CswNbtFieldTypeRuleList( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Location:
                    ReturnVal = new CswNbtFieldTypeRuleLocation( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Logical:
                    ReturnVal = new CswNbtFieldTypeRuleLogical( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.LogicalSet:
                    ReturnVal = new CswNbtFieldTypeRuleLogicalSet( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Memo:
                    ReturnVal = new CswNbtFieldTypeRuleMemo( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.MOL:
                    ReturnVal = new CswNbtFieldTypeRuleMol( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.MTBF:
                    ReturnVal = new CswNbtFieldTypeRuleMTBF( CswNbtFieldResources );
                    break;

                //case CswEnumNbtFieldType.MultiRelationship:
                //    ReturnVal = new CswNbtFieldTypeRuleMultiRelationship( CswNbtFieldResources );
                //    break;

                case CswEnumNbtFieldType.MultiList:
                    ReturnVal = new CswNbtFieldTypeRuleMultiList( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.NFPA:
                    ReturnVal = new CswNbtFieldTypeRuleNFPA( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.NodeTypeSelect:
                    ReturnVal = new CswNbtFieldTypeRuleNodeTypeSelect( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Number:
                    ReturnVal = new CswNbtFieldTypeRuleNumber( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Password:
                    ReturnVal = new CswNbtFieldTypeRulePassword( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.PropertyReference:
                    ReturnVal = new CswNbtFieldTypeRulePropertyReference( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Quantity:
                    ReturnVal = new CswNbtFieldTypeRuleQuantity( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Question:
                    ReturnVal = new CswNbtFieldTypeRuleQuestion( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Relationship:
                    ReturnVal = new CswNbtFieldTypeRuleRelationship( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.ReportLink:
                    ReturnVal = new CswNbtFieldTypeRuleReportLink( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Scientific:
                    ReturnVal = new CswNbtFieldTypeRuleScientific( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Sequence:
                    ReturnVal = new CswNbtFieldTypeRuleSequence( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Static:
                    ReturnVal = new CswNbtFieldTypeRuleStatic( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.Text:
                    ReturnVal = new CswNbtFieldTypeRuleText( CswNbtFieldResources );
                    break;

                //case CswEnumNbtFieldType.Time:
                //    ReturnVal = new CswNbtFieldTypeRuleTime( CswNbtFieldResources );
                //    break;

                case CswEnumNbtFieldType.TimeInterval:
                    ReturnVal = new CswNbtFieldTypeRuleTimeInterval( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.UserSelect:
                    ReturnVal = new CswNbtFieldTypeRuleUserSelect( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.ViewPickList:
                    ReturnVal = new CswNbtFieldTypeRuleViewPickList( CswNbtFieldResources );
                    break;

                case CswEnumNbtFieldType.ViewReference:
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
