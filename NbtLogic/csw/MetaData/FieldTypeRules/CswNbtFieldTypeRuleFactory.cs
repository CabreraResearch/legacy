using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleFactory
    {
        public static ICswNbtFieldTypeRule makeRule(CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp)
        {
            ICswNbtFieldTypeRule ReturnVal = null;
            //CswNbtFieldResources CswNbtFieldResources = new CswNbtFieldResources( CswNbtResources );
            
            switch( MetaDataProp.FieldType.FieldType )
            {
                case CswNbtMetaDataFieldType.NbtFieldType.Barcode:
                    ReturnVal = new CswNbtFieldTypeRuleBarCode( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Composite:
                    ReturnVal = new CswNbtFieldTypeRuleComposite( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.DateTime:
                    ReturnVal = new CswNbtFieldTypeRuleDateTime( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.External:
                    ReturnVal = new CswNbtFieldTypeRuleExternal( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.File:
                    ReturnVal = new CswNbtFieldTypeRuleBlob( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Grid:
                    ReturnVal = new CswNbtFieldTypeRuleGrid( CswNbtFieldResources, MetaDataProp );
                    break;

				case CswNbtMetaDataFieldType.NbtFieldType.Image:
					ReturnVal = new CswNbtFieldTypeRuleImage( CswNbtFieldResources, MetaDataProp );
					break;

				case CswNbtMetaDataFieldType.NbtFieldType.ImageList:
					ReturnVal = new CswNbtFieldTypeRuleImageList( CswNbtFieldResources, MetaDataProp );
					break;

				case CswNbtMetaDataFieldType.NbtFieldType.Link:
                    ReturnVal = new CswNbtFieldTypeRuleLink( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.List:
                    ReturnVal = new CswNbtFieldTypeRuleList( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Location:
                    ReturnVal = new CswNbtFieldTypeRuleLocation( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.LocationContents:
                    ReturnVal = new CswNbtFieldTypeRuleLocationContents( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Logical:
                    ReturnVal = new CswNbtFieldTypeRuleLogical( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.LogicalSet:
                    ReturnVal = new CswNbtFieldTypeRuleLogicalSet( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Memo:
                    ReturnVal = new CswNbtFieldTypeRuleMemo( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.MOL:
                    ReturnVal = new CswNbtFieldTypeRuleMol( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.MTBF:
                    ReturnVal = new CswNbtFieldTypeRuleMTBF( CswNbtFieldResources, MetaDataProp );
                    break;

				//case CswNbtMetaDataFieldType.NbtFieldType.MultiRelationship:
				//    ReturnVal = new CswNbtFieldTypeRuleMultiRelationship( CswNbtFieldResources );
				//    break;

				case CswNbtMetaDataFieldType.NbtFieldType.MultiList:
					ReturnVal = new CswNbtFieldTypeRuleMultiList( CswNbtFieldResources, MetaDataProp );
					break;

                //case CswNbtMetaDataFieldType.NbtFieldType.NodeTypePermissions:
                //    ReturnVal = new CswNbtFieldTypeRuleNodeTypePermissions( CswNbtFieldResources );
                //    break;

				case CswNbtMetaDataFieldType.NbtFieldType.NFPA:
					ReturnVal = new CswNbtFieldTypeRuleNFPA( CswNbtFieldResources, MetaDataProp );
					break;

				case CswNbtMetaDataFieldType.NbtFieldType.NodeTypeSelect:
					ReturnVal = new CswNbtFieldTypeRuleNodeTypeSelect( CswNbtFieldResources, MetaDataProp );
					break;

				case CswNbtMetaDataFieldType.NbtFieldType.Number:
                    ReturnVal = new CswNbtFieldTypeRuleNumber( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Password:
                    ReturnVal = new CswNbtFieldTypeRulePassword( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.PropertyReference:
                    ReturnVal = new CswNbtFieldTypeRulePropertyReference( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Quantity:
                    ReturnVal = new CswNbtFieldTypeRuleQuantity( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Question:
                    ReturnVal = new CswNbtFieldTypeRuleQuestion(CswNbtFieldResources, MetaDataProp);
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Relationship:
                    ReturnVal = new CswNbtFieldTypeRuleRelationship( CswNbtFieldResources, MetaDataProp );
                    break;

				case CswNbtMetaDataFieldType.NbtFieldType.Scientific:
					ReturnVal = new CswNbtFieldTypeRuleScientific( CswNbtFieldResources, MetaDataProp );
					break;

				case CswNbtMetaDataFieldType.NbtFieldType.Sequence:
					ReturnVal = new CswNbtFieldTypeRuleSequence( CswNbtFieldResources, MetaDataProp );
					break;

				case CswNbtMetaDataFieldType.NbtFieldType.Static:
                    ReturnVal = new CswNbtFieldTypeRuleStatic( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.Text:
                    ReturnVal = new CswNbtFieldTypeRuleText( CswNbtFieldResources, MetaDataProp );
                    break;

				//case CswNbtMetaDataFieldType.NbtFieldType.Time:
				//    ReturnVal = new CswNbtFieldTypeRuleTime( CswNbtFieldResources, MetaDataProp );
				//    break;

                case CswNbtMetaDataFieldType.NbtFieldType.TimeInterval:
                    ReturnVal = new CswNbtFieldTypeRuleTimeInterval( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.UserSelect:
                    ReturnVal = new CswNbtFieldTypeRuleUserSelect( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ViewPickList:
                    ReturnVal = new CswNbtFieldTypeRuleViewPickList( CswNbtFieldResources, MetaDataProp );
                    break;

                case CswNbtMetaDataFieldType.NbtFieldType.ViewReference:
                    ReturnVal = new CswNbtFieldTypeRuleViewReference( CswNbtFieldResources, MetaDataProp );
                    break;

                default:
                    throw( new CswDniException( "There is no field rule class for field type " + MetaDataProp.FieldType.FieldType.ToString() ) );
                    //break;
            }//switch


            return ( ReturnVal );

        }//makeRule() 

    }//CswNbtFieldTypeRuleFactory

}//namespace ChemSW.Nbt.MetaData
