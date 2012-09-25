using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.ServiceDrivers;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServicePrintLabels
    {
        private CswNbtResources _CswNbtResources;
        public CswNbtWebServicePrintLabels( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }

        public JObject getLabels( string PropIdAttr )
        {
            JObject ret = new JObject();
            JArray Labels = new JArray();
            ret.Add( new JProperty( "labels", Labels ) );
            if( false == string.IsNullOrEmpty( PropIdAttr ) )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                CswNbtMetaDataNodeTypeProp MetaDataProp = _CswNbtResources.MetaData.getNodeTypeProp( PropId.NodeTypePropId );
                if( null != MetaDataProp )
                {
                    Int32 NodeTypeId = MetaDataProp.NodeTypeId;

                    string PrintLabelNodeTypesPropertyName = "NodeTypes";
                    CswNbtMetaDataObjectClass PrintLabelObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.PrintLabelClass );
                    CswNbtMetaDataObjectClassProp NodeTypesProperty = PrintLabelObjectClass.getObjectClassProp( PrintLabelNodeTypesPropertyName );

                    CswNbtView PrintLabelView = new CswNbtView( _CswNbtResources );
                    PrintLabelView.ViewName = "getPrintLabelsForNodeType(" + NodeTypeId.ToString() + ")";
                    CswNbtViewRelationship PrintLabelRelationship = PrintLabelView.AddViewRelationship( PrintLabelObjectClass, true );
                    CswNbtViewProperty PrintLabelNodeTypesProperty = PrintLabelView.AddViewProperty( PrintLabelRelationship, NodeTypesProperty );
                    CswNbtViewPropertyFilter PrintLabelNodeTypesPropertyFilter = PrintLabelView.AddViewPropertyFilter( PrintLabelNodeTypesProperty, CswNbtSubField.SubFieldName.Unknown, CswNbtPropFilterSql.PropertyFilterMode.Contains, NodeTypeId.ToString(), false );

                    ICswNbtTree PrintLabelsTree = _CswNbtResources.Trees.getTreeFromView( PrintLabelView, true, true, false, false );

                    PrintLabelsTree.goToRoot();
                    for( int i = 0; i < PrintLabelsTree.getChildNodeCount(); i++ )
                    {
                        PrintLabelsTree.goToNthChild( i );
                        Labels.Add( new JObject(
                                       new JProperty( "name", PrintLabelsTree.getNodeNameForCurrentPosition() ),
                                       new JProperty( "nodeid", PrintLabelsTree.getNodeIdForCurrentPosition().ToString() )
                                       ) );
                        PrintLabelsTree.goToParentNode();
                    }
                }
            }
            return ret;
        } // getLabels()


        public JObject getEPLText( string PropIdAttr, string PrintLabelNodeIdStr )
        {
            JObject Ret = new JObject();

            CswNbtObjClassPrintLabel NodeAsPrintLabel = _CswNbtResources.Nodes[PrintLabelNodeIdStr];
            if( null != NodeAsPrintLabel )
            {
                CswPropIdAttr PropId = new CswPropIdAttr( PropIdAttr );
                CswNbtNode TargetNode = _CswNbtResources.Nodes[PropId.NodeId];
                if( null != TargetNode )
                {
                    string EPLText = NodeAsPrintLabel.EplText.Text;
                    string Params = NodeAsPrintLabel.Params.Text;
                    string ControlType = NodeAsPrintLabel.ControlType.Value;
                    if( string.IsNullOrEmpty( ControlType ) )
                    {
                        ControlType = CswNbtObjClassPrintLabel.ControlTypes.jZebra;
                    }

                    // BZ 6118 - this prevents " from being turned into &quot;
                    // BUT SEE BZ 7881!
                    Ret["epl"] = GenerateEPLScript( EPLText, Params, TargetNode ) + "\n";
                    Ret["controltype"] = ControlType;
                }
            }
            if( false == Ret.HasValues )
            {
                throw new CswDniException( ErrorType.Error, "Failed to get valid EPL text from the provided parameters.", "getEplText received invalid PropIdAttr and PrintLabelNodeIdStr parameters." );
            }
            return Ret;
        } // getEPLText()

        private string GenerateEPLScript( string EPLText, string Params, CswNbtNode Node )
        {
            string EPLScript = string.Empty;
            if( false == string.IsNullOrEmpty( EPLText ) )
            {
                EPLScript = EPLText;
                if( false == string.IsNullOrEmpty( Params ) )
                {
                    string[] ParamsArray = Params.Split( '\n' );

                    while( EPLScript.Contains( "{" ) )
                    {
                        Int32 ParamStartIndex = EPLScript.IndexOf( "{" );
                        Int32 ParamEndIndex = EPLScript.IndexOf( "}" );

                        string PropertyParamString = EPLScript.Substring( ParamStartIndex, ParamEndIndex - ParamStartIndex + 1 );
                        string PropertyParamName = PropertyParamString.Substring( 1, PropertyParamString.Length - 2 );
                        // Find the property
                        if( null != Node )
                        {
                            CswNbtMetaDataNodeType MetaDataNodeType = _CswNbtResources.MetaData.getNodeType( Node.NodeTypeId );
                            if( null != MetaDataNodeType )
                            {
                                CswNbtMetaDataNodeTypeProp MetaDataProp = MetaDataNodeType.getNodeTypeProp( PropertyParamName );
                                if( null != MetaDataProp )
                                {
                                    string PropertyValue = Node.Properties[MetaDataProp].Gestalt;

                                    bool FoundMatch = false;
                                    foreach( string ParamNVP in ParamsArray )
                                    {
                                        string[] ParamSplit = ParamNVP.Split( '=' );
                                        if( ParamSplit.Length > 1 &&
                                            ParamSplit[0] == PropertyParamName &&
                                            CswTools.IsInteger( ParamSplit[1] ) )
                                        {
                                            FoundMatch = true;
                                            Int32 MaxLength = CswConvert.ToInt32( ParamSplit[1] );
                                            Int32 CurrentIteration = 1;
                                            while( ParamStartIndex >= 0 )
                                            {
                                                if( PropertyValue.Length > MaxLength )
                                                {
                                                    EPLScript = EPLScript.Substring( 0, ParamStartIndex ) + PropertyValue.Substring( 0, MaxLength ) + EPLScript.Substring( ParamEndIndex + 1 );
                                                    PropertyValue = PropertyValue.Substring( MaxLength + 1 );
                                                }
                                                else
                                                {
                                                    EPLScript = EPLScript.Substring( 0, ParamStartIndex ) + PropertyValue + EPLScript.Substring( ParamEndIndex + 1 );
                                                    PropertyValue = "";
                                                }
                                                CurrentIteration += 1;
                                                ParamStartIndex = EPLScript.IndexOf( "{" + PropertyParamName + "_" + CurrentIteration + "}" );
                                                ParamEndIndex = ParamStartIndex + ( "{" + PropertyParamName + "_" + CurrentIteration + "}" ).Length - 1;
                                            }
                                        }
                                    }
                                    if( false == FoundMatch )
                                    {
                                        EPLScript = EPLScript.Substring( 0, ParamStartIndex ) + PropertyValue + EPLScript.Substring( ParamEndIndex + 1 );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if( string.IsNullOrEmpty( EPLScript ) )
            {
                throw new CswDniException( ErrorType.Error, "Could not generate an EPL script from the provided parameters.", "EPL Text='" + EPLText + "', Params='" + Params + "'" );
            }
            return EPLScript;
        } // GenerateEPLScript()



    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
