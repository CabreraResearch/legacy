using System;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NbtWebApp.WebSvc.Logic.Labels;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    /// <summary>
    /// Label List Return Object
    /// </summary>
    [DataContract]
    public class CswNbtLabelList : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtLabelList()
        {
            Data = new NbtPrintLabel.Response.List();
        }

        /// <summary> data </summary>
        [DataMember]
        public NbtPrintLabel.Response.List Data;
    }

    /// <summary>
    /// Label EPL Return Object
    /// </summary>
    [DataContract]
    public class CswNbtLabelEpl : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtLabelEpl()
        {
            Data = new NbtPrintLabel.Response.Epl();
        }

        /// <summary> data </summary>
        [DataMember]
        public NbtPrintLabel.Response.Epl Data;
    }


    public class CswNbtWebServicePrintLabels
    {
        public static void getLabels( ICswResources CswResources, CswNbtLabelList Return, NbtPrintLabel.Request.List Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( Int32.MinValue != Request.TargetTypeId )
            {

                CswNbtMetaDataNodeType TargetNodeType = NbtResources.MetaData.getNodeType( Request.TargetTypeId );
                if( null != TargetNodeType )
                {
                    CswNbtMetaDataObjectClass PrintLabelObjectClass = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
                    CswNbtMetaDataObjectClassProp NodeTypesProperty = PrintLabelObjectClass.getObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );

                    CswNbtView PrintLabelView = new CswNbtView( NbtResources );
                    PrintLabelView.ViewName = "getPrintLabelsForNodeType(" + Request.TargetTypeId.ToString() + ")";
                    CswNbtViewRelationship PrintLabelRelationship = PrintLabelView.AddViewRelationship( PrintLabelObjectClass, true );
                    CswNbtViewProperty PrintLabelNodeTypesProperty = PrintLabelView.AddViewProperty( PrintLabelRelationship, NodeTypesProperty );
                    PrintLabelView.AddViewPropertyFilter( PrintLabelNodeTypesProperty, NodeTypesProperty.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.Contains, Request.TargetTypeId.ToString() );

                    ICswNbtTree PrintLabelsTree = NbtResources.Trees.getTreeFromView( NbtResources.CurrentNbtUser, PrintLabelView, true, false, false );
                    Int32 PrintLabelCount = PrintLabelsTree.getChildNodeCount();
                    if( PrintLabelCount > 0 )
                    {
                        String LabelFormatId = _getLabelFormatForTargetId( NbtResources, TargetNodeType, Request.TargetId );
                        PrintLabelsTree.goToRoot();
                        for( int P = 0; P < PrintLabelCount; P += 1 )
                        {
                            PrintLabelsTree.goToNthChild( P );
                            String PrintLabelId = PrintLabelsTree.getNodeIdForCurrentPosition().ToString();
                            Return.Data.Labels.Add( new Label
                            {
                                Name = PrintLabelsTree.getNodeNameForCurrentPosition(),
                                Id = PrintLabelId
                            } );
                            if( PrintLabelId == LabelFormatId )
                            {
                                Return.Data.SelectedLabelId = PrintLabelId;
                            }
                            PrintLabelsTree.goToParentNode();
                        }
                    }
                }
            }
        } // getLabels()

        private static String _getLabelFormatForTargetId( CswNbtResources NbtResources, CswNbtMetaDataNodeType TargetNodeType, String TargetId )
        {
            String LabelFormatId = String.Empty;
            if( null != TargetId )
            {
                CswNbtNode TargetNode = NbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( TargetId ) );
                if( null != TargetNode )
                {
                    CswNbtMetaDataObjectClass PrintLabelClass = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
                    foreach( CswNbtMetaDataNodeTypeProp RelationshipProp in TargetNodeType.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Relationship ) )
                    {
                        if( RelationshipProp.FKValue == PrintLabelClass.ObjectClassId )
                        {
                            LabelFormatId = TargetNode.Properties[RelationshipProp].AsRelationship.RelatedNodeId.ToString();
                        }
                    }
                }
            }
            return LabelFormatId;
        }

        public static void getEPLText( ICswResources CswResources, CswNbtLabelEpl Return, NbtPrintLabel.Request.Get Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtObjClassPrintLabel NodeAsPrintLabel = NbtResources.Nodes[Request.LabelId];
            if( null != NodeAsPrintLabel )
            {
                string EPLText = NodeAsPrintLabel.EplText.Text;
                string Params = NodeAsPrintLabel.Params.Text;

                foreach( string TargetId in Request.TargetIds )
                {
                    CswNbtNode TargetNode = NbtResources.Nodes[TargetId];
                    if( null != TargetNode )
                    {
                        // BZ 6118 - this prevents " from being turned into &quot;
                        // BUT SEE BZ 7881!
                        string EplText = GenerateEPLScript( NbtResources, EPLText, Params, TargetNode ) + "\n";
                        Return.Data.Labels.Add( new PrintLabel
                        {
                            TargetId = TargetNode.NodeId.ToString(),
                            TargetName = TargetNode.NodeName,
                            EplText = EplText
                        } );
                    }
                }
            }
            if( Return.Data.Labels.Count == 0 )
            {
                throw new CswDniException( ErrorType.Error, "Failed to get valid EPL text from the provided parameters.", "getEplText received invalid PropIdAttr and PrintLabelNodeIdStr parameters." );
            }
        } // getEPLText()

        private static string GenerateEPLScript( CswNbtResources NbtResources, string EPLText, string Params, CswNbtNode Node )
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
                            CswNbtMetaDataNodeType MetaDataNodeType = NbtResources.MetaData.getNodeType( Node.NodeTypeId );
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
