using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
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

    /// <summary>
    /// Print Job Return Object
    /// </summary>
    [DataContract]
    public class CswNbtPrintJobReturn : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtPrintJobReturn()
        {
            Data = new NbtPrintLabel.Response.printJob();
        }

        /// <summary> data </summary>
        [DataMember]
        public NbtPrintLabel.Response.printJob Data;

    }

    /// <summary>
    /// Label EPL Return Object
    /// </summary>
    [DataContract]
    public class CswNbtLabelPrinterReg : CswWebSvcReturn
    {
        /// <summary> ctor </summary>
        public CswNbtLabelPrinterReg()
        {
            PrinterKey = string.Empty;
        }

        /// <summary> data </summary>
        [DataMember]
        public string PrinterKey;

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
                    PrintLabelView.AddViewPropertyFilter( PrintLabelNodeTypesProperty,
                                                          NodeTypesProperty.getFieldTypeRule().SubFields.Default.Name,
                                                          CswNbtPropFilterSql.PropertyFilterMode.Contains,
                                                          Request.TargetTypeId.ToString() );

                    ICswNbtTree PrintLabelsTree = NbtResources.Trees.getTreeFromView( NbtResources.CurrentNbtUser, PrintLabelView, true, false, false );
                    Int32 PrintLabelCount = PrintLabelsTree.getChildNodeCount();
                    if( PrintLabelCount > 0 )
                    {
                        CswPrimaryKey LabelFormatId = _getLabelFormatForTargetId( NbtResources, TargetNodeType, Request.TargetId );
                        PrintLabelsTree.goToRoot();
                        for( int P = 0; P < PrintLabelCount; P += 1 )
                        {
                            PrintLabelsTree.goToNthChild( P );
                            CswPrimaryKey PrintLabelId = PrintLabelsTree.getNodeIdForCurrentPosition();
                            Return.Data.Labels.Add( new Label
                            {
                                Name = PrintLabelsTree.getNodeNameForCurrentPosition(),
                                Id = PrintLabelId.ToString()
                            } );
                            if( PrintLabelId == LabelFormatId )
                            {
                                Return.Data.SelectedLabelId = PrintLabelId.ToString();
                            }
                            PrintLabelsTree.goToParentNode();
                        }
                    }
                }
            }
        } // getLabels()

        private static CswPrimaryKey _getLabelFormatForTargetId( CswNbtResources NbtResources, CswNbtMetaDataNodeType TargetNodeType, String TargetId )
        {
            CswPrimaryKey LabelFormatId = null;
            if( null != TargetId )
            {
                CswNbtNode TargetNode = NbtResources.Nodes.GetNode( CswConvert.ToPrimaryKey( TargetId ) );
                if( null != TargetNode )
                {
                    CswNbtMetaDataObjectClass PrintLabelClass = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
                    foreach( CswNbtMetaDataNodeTypeProp RelationshipProp in TargetNodeType.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Relationship ) )
                    {
                        bool PropMatchesPrintLabel = false;
                        if( RelationshipProp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                            RelationshipProp.FKValue == PrintLabelClass.ObjectClassId )
                        {
                            PropMatchesPrintLabel = true;
                        }
                        else if( RelationshipProp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                        {
                            foreach( Int32 PrintLabelNTId in PrintLabelClass.getNodeTypeIds() )
                            {
                                if( RelationshipProp.FKValue == PrintLabelNTId )
                                {
                                    PropMatchesPrintLabel = true;
                                }
                            }
                        }
                        if( PropMatchesPrintLabel )
                        {
                            LabelFormatId = TargetNode.Properties[RelationshipProp].AsRelationship.RelatedNodeId;
                            break;
                        }
                    }
                }
            }
            return LabelFormatId;
        }

        public static void newPrintJob( ICswResources CswResources, CswNbtPrintJobReturn Return, NbtPrintLabel.Request.printJob Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass PrintJobOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrintJobClass );
            if( null == PrintJobOC )
            {
                throw new CswDniException( ErrorType.Error, "Could not create new Print Job", "newPrintJob() could not find a Print Job Object Class" );
            }

            CswNbtMetaDataNodeType PrintJobNT = PrintJobOC.FirstNodeType;
            if( null == PrintJobNT )
            {
                throw new CswDniException( ErrorType.Error, "Could not create new Print Job", "newPrintJob() could not find a Print Job NodeType" );
            }

            CswPrimaryKey LabelPk = new CswPrimaryKey();
            LabelPk.FromString( Request.LabelId );
            if( false == CswTools.IsPrimaryKey( LabelPk ) )
            {
                throw new CswDniException( ErrorType.Error, "Invalid print label key", "newPrintJob() Print Label Key is not a valid CswPrimaryKey: " + Request.PrinterId );
            }

            CswNbtObjClassPrintLabel PrintLabel = NbtResources.Nodes[LabelPk];
            if( null == PrintLabel )
            {
                throw new CswDniException( ErrorType.Error, "Invalid print label", "newPrintJob() Print Label Key did not match a node." );
            }

            CswPrimaryKey PrinterPk = new CswPrimaryKey();
            PrinterPk.FromString( Request.PrinterId );
            if( false == CswTools.IsPrimaryKey( PrinterPk ) )
            {
                throw new CswDniException( ErrorType.Error, "Invalid printer key", "newPrintJob() Printer Key is not a valid CswPrimaryKey: " + Request.PrinterId );
            }

            CswNbtObjClassPrinter Printer = NbtResources.Nodes[PrinterPk];
            if( null == Printer )
            {
                throw new CswDniException( ErrorType.Error, "Invalid printer", "newPrintJob() Printer Key did not match a node." );
            }

            string JobData = string.Empty;
            Int32 JobCount = 0;

            CswCommaDelimitedString RealTargetIds = new CswCommaDelimitedString();
            RealTargetIds.FromString( Request.TargetIds );
            foreach( string TargetId in RealTargetIds )
            {
                CswNbtNode TargetNode = NbtResources.Nodes[TargetId];
                if( null != TargetNode )
                {
                    string EPLText = PrintLabel.EplText.Text;
                    string Params = PrintLabel.Params.Text;
                    JobData += GenerateEPLScript( NbtResources, EPLText, Params, TargetNode );
                    JobCount += 1;
                }
            } // foreach( string TargetId in RealTargetIds )

            CswNbtObjClassPrintJob NewJob = NbtResources.Nodes.makeNodeFromNodeTypeId( PrintJobNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode, false );
            NewJob.Label.RelatedNodeId = PrintLabel.NodeId;
            NewJob.LabelCount.Value = JobCount;
            NewJob.LabelData.Text = JobData;
            NewJob.Printer.RelatedNodeId = Printer.NodeId;
            NewJob.RequestedBy.RelatedNodeId = NbtResources.CurrentNbtUser.UserId;
            NewJob.CreatedDate.DateTimeValue = DateTime.Now;
            NewJob.postChanges( false );

            Return.Data.JobId = NewJob.NodeId.ToString();
            Return.Data.JobNo = NewJob.JobNo.Sequence;
            Return.Data.JobLink = CswNbtNode.getNodeLink( NewJob.NodeId, NewJob.NodeName );

        } // newPrintJob()

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
                        //string EplText = GenerateEPLScript( NbtResources, EPLText, Params, TargetNode ) + "\n";
                        Return.Data.Labels.Add( new PrintLabel
                            {
                                TargetId = TargetNode.NodeId.ToString(),
                                TargetName = TargetNode.NodeName,
                                EplText = GenerateEPLScript( NbtResources, EPLText, Params, TargetNode )
                            } );
                    }
                }
            }
            if( Return.Data.Labels.Count == 0 )
            {
                throw new CswDniException( ErrorType.Error, "Failed to get valid EPL text from the provided parameters.", "getEplText received invalid PropIdAttr and PrintLabelNodeIdStr parameters." );
            }
        } // getEPLText()


        // case 28716
        // Special case: Spool GHS data from this container's Material's GHS in the user's Jurisdiction and language
        private static string _getGhsValueForLabel( CswNbtResources NbtResources, CswNbtObjClassContainer Node, bool ShowCodes, bool ShowPhrases )
        {
            string ret = string.Empty;

            CswNbtMetaDataNodeType ContainerNT = Node.NodeType;
            if( null != ContainerNT && ContainerNT.getObjectClass().ObjectClass == NbtObjectClass.ContainerClass )
            {
                CswNbtMetaDataNodeTypeProp ContainerMaterialNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );

                CswNbtMetaDataObjectClass GhsOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.GHSClass );
                if( null != GhsOC )
                {
                    CswNbtMetaDataObjectClassProp GhsMaterialOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                    CswNbtMetaDataObjectClassProp GhsJurisdictionOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );

                    CswNbtView GHSView = new CswNbtView( NbtResources );
                    GHSView.ViewName = "GHS for Container";

                    CswNbtViewRelationship ContainerRel = GHSView.AddViewRelationship( Node.NodeType, false );
                    CswNbtViewRelationship MaterialRel = GHSView.AddViewRelationship( ContainerRel, NbtViewPropOwnerType.First, ContainerMaterialNTP, false );
                    CswNbtViewRelationship GHSRel = GHSView.AddViewRelationship( MaterialRel, NbtViewPropOwnerType.Second, GhsMaterialOCP, false );

                    ContainerRel.NodeIdsToFilterIn.Add( Node.NodeId );
                    CswPrimaryKey JurisdictionId = NbtResources.CurrentNbtUser.JurisdictionId;
                    if( CswTools.IsPrimaryKey( JurisdictionId ) )
                    {
                        GHSView.AddViewPropertyAndFilter( GHSRel, GhsJurisdictionOCP,
                                                          Value: NbtResources.CurrentNbtUser.JurisdictionId.PrimaryKey.ToString(),
                                                          SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                                                          FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                    }

                    ICswNbtTree GHSTree = NbtResources.Trees.getTreeFromView( GHSView, false, false, false );
                    if( GHSTree.getChildNodeCount() > 0 )
                    {
                        GHSTree.goToNthChild( 0 ); // Container
                        if( GHSTree.getChildNodeCount() > 0 )
                        {
                            GHSTree.goToNthChild( 0 ); // Material
                            if( GHSTree.getChildNodeCount() > 0 )
                            {
                                GHSTree.goToNthChild( 0 ); // GHS
                                CswNbtObjClassGHS GHSNode = GHSTree.getNodeForCurrentPosition();

                                // Run the Label Codes View
                                ICswNbtTree LabelCodesTree = NbtResources.Trees.getTreeFromView( GHSNode.LabelCodesGrid.View, false, false, false );
                                SortedList<string, string> Phrases = new SortedList<string, string>();
                                for( Int32 p = 0; p < LabelCodesTree.getChildNodeCount(); p++ )
                                {
                                    LabelCodesTree.goToNthChild( p );

                                    Collection<CswNbtTreeNodeProp> Props = LabelCodesTree.getChildNodePropsOfNode();

                                    string Code = string.Empty;
                                    string Phrase = string.Empty;
                                    foreach( CswNbtTreeNodeProp Prop in Props )
                                    {
                                        CswNbtMetaDataNodeTypeProp Ntp = NbtResources.MetaData.getNodeTypeProp( Prop.NodeTypePropId );
                                        if( null != Ntp && Ntp.getObjectClassPropName() == CswNbtObjClassGHSPhrase.PropertyName.Code )
                                        {
                                            Code = Prop.Gestalt;
                                        }
                                        else
                                        {
                                            Phrase = Prop.Gestalt;
                                        }
                                    }
                                    Phrases.Add( Code, Phrase );

                                    LabelCodesTree.goToParentNode();
                                } // for( Int32 p = 0; p < LabelCodesTree.getChildNodeCount(); p++ )

                                foreach( string Code in Phrases.Keys )
                                {
                                    if( ShowCodes )
                                    {
                                        if( false == ShowPhrases && ret != string.Empty )
                                        {
                                            ret += ",";
                                        }
                                        ret += Code;
                                    }
                                    if( ShowPhrases )
                                    {
                                        if( ShowCodes )
                                        {
                                            ret += ": ";
                                        }
                                        ret += Phrases[Code] + "\n";
                                    }
                                } // foreach( string Code in Phrases.Keys )

                            } // if( GHSTree.getChildNodeCount() > 0 ) ghs
                        } // if( GHSTree.getChildNodeCount() > 0 )     material
                    } // if( GHSTree.getChildNodeCount() > 0 )         container
                } // if(null != GhsOC)
            } // if( null != ContainerNT && ContainerNT.getObjectClass().ObjectClass == NbtObjectClass.ContainerClass )
            
            return ret;
        } // _getGhsValueForLabel()


        private static string GenerateEPLScript( CswNbtResources NbtResources, string EPLText, string Params, CswNbtNode Node )
        {
            string EPLScript = string.Empty;
            if( false == string.IsNullOrEmpty( EPLText ) && null != Node )
            {
                EPLScript = EPLText;
                
                // Extract sizes from Params array
                Dictionary<string,Int32> ParamSizes = new Dictionary<string, Int32>();
                if( false == string.IsNullOrEmpty( Params ) )
                {
                    string[] ParamsArray = Params.Split( '\n' );
                    foreach( string ParamNVP in ParamsArray )
                    {
                        string[] ParamSplit = ParamNVP.Split( '=' );
                        if( ParamSplit.Length > 1 && CswTools.IsInteger( ParamSplit[1] ) )
                        {
                            ParamSizes[ParamSplit[0]] = CswConvert.ToInt32( ParamSplit[1] );
                        } // if( ParamSplit.Length > 1 && CswTools.IsInteger( ParamSplit[1] ) )
                    } // foreach( string ParamNVP in ParamsArray )
                } // if( false == string.IsNullOrEmpty( Params ) )


                // Find template names in the EPLText
                Dictionary<string, string> TemplateValues = new Dictionary<string, string>();
                MatchCollection TemplateMatches = Regex.Matches( EPLText, @"{.+}" );
                foreach( Match TemplateMatch in TemplateMatches )
                {
                    string TemplateName = TemplateMatch.Value.Substring( 1, TemplateMatch.Value.Length - 2 );

                    if( false == TemplateValues.ContainsKey( TemplateName ) )
                    {
                        // Fetch template value
                        string TemplateValue = string.Empty;
                        if( TemplateName.StartsWith( "NBTGHS" ) )   // Ignore NBTGHS_2, will fill in below
                        {
                            if( TemplateName.Equals( "NBTGHS" ) || TemplateName.Equals( "NBTGHSA" ) )
                            {
                                // A - phrases only
                                TemplateValue = _getGhsValueForLabel( NbtResources, Node, ShowCodes: false , ShowPhrases: true );
                            }
                            if( TemplateName.Equals( "NBTGHSB" ) )
                            {
                                // B - codes only
                                TemplateValue = _getGhsValueForLabel( NbtResources, Node, ShowCodes: true, ShowPhrases: false );
                            }
                            if( TemplateName.Equals( "NBTGHSC" ) )
                            {
                                // C - phrases and codes
                                TemplateValue = _getGhsValueForLabel( NbtResources, Node, ShowCodes: true, ShowPhrases: true );
                            }
                        }
                        else
                        {
                            CswNbtMetaDataNodeType MetaDataNodeType = NbtResources.MetaData.getNodeType( Node.NodeTypeId );
                            if( null != MetaDataNodeType )
                            {
                                CswNbtMetaDataNodeTypeProp MetaDataProp = MetaDataNodeType.getNodeTypeProp( TemplateName );
                                if( null != MetaDataProp )
                                {
                                    TemplateValue = Node.Properties[MetaDataProp].Gestalt;
                                }
                            }
                        }

                        // Handle splitting template value over lines
                        IEnumerable<string> ValueChunks = null;
                        if( ParamSizes.ContainsKey( TemplateName ) )
                        {
                            ValueChunks = CswTools.Chunk( TemplateValue, ParamSizes[TemplateName] );
                        }
                        else if( TemplateValue.Contains( "\n" ) )
                        {
                            ValueChunks = TemplateValue.Replace( "\r", "" ).Split( '\n' );
                        }

                        if( null != ValueChunks )
                        {
                            Int32 CurrentIteration = 1;
                            string CurrentTemplateName = TemplateName;
                            foreach( string Chunk in ValueChunks )
                            {
                                TemplateValues[CurrentTemplateName] = Chunk;
                                CurrentIteration++;
                                CurrentTemplateName = TemplateName + "_" + CurrentIteration;
                            }
                        } // if( null != ValueChunks )
                        else
                        {
                            TemplateValues[TemplateName] = TemplateValue;
                        }
                    } // if( false == TemplateValues.ContainsKey( TemplateName ) )
                } // foreach( Match TemplateMatch in TemplateMatches )


                // Apply template values to EPLScript
                foreach( string TemplateName in TemplateValues.Keys )
                {
                    EPLScript = EPLScript.Replace( "{" + TemplateName + "}", TemplateValues[TemplateName] );
                }

            } // false == string.IsNullOrEmpty( EPLText ) && null != Node )

            if( string.IsNullOrEmpty( EPLScript ) )
            {
                throw new CswDniException( ErrorType.Error, "Could not generate an EPL script from the provided parameters.", "EPL Text='" + EPLText + "', Params='" + Params + "'" );
            }
            return EPLScript + "\n";
        } // GenerateEPLScript()

        public static void registerLpc( ICswResources CswResources, CswNbtLabelPrinterReg Return, LabelPrinter Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Status.Success = false;

            CswNbtMetaDataObjectClass PrinterOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrinterClass );
            if( null != PrinterOC )
            {
                CswNbtMetaDataNodeType PrinterNT = PrinterOC.FirstNodeType;
                if( null != PrinterNT )
                {
                    CswNbtMetaDataObjectClassProp PrinterNameOCP = PrinterOC.getObjectClassProp( CswNbtObjClassPrinter.PropertyName.Name );

                    CswNbtView ExistingPrintersView = new CswNbtView( NbtResources );
                    ExistingPrintersView.ViewName = "Existing Printers";
                    CswNbtViewRelationship PrinterRel = ExistingPrintersView.AddViewRelationship( PrinterOC, false );
                    ExistingPrintersView.AddViewPropertyAndFilter( PrinterRel, PrinterNameOCP,
                                                                   Value: Request.LpcName,
                                                                   FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                    ICswNbtTree ExistingPrintersTree = NbtResources.Trees.getTreeFromView( ExistingPrintersView, false, true, true );
                    if( ExistingPrintersTree.getChildNodeCount() == 0 )
                    {
                        CswNbtObjClassPrinter NewPrinter = NbtResources.Nodes.makeNodeFromNodeTypeId( PrinterNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                        NewPrinter.Name.Text = Request.LpcName;
                        NewPrinter.Description.Text = Request.Description;
                        NewPrinter.postChanges( false );

                        Return.Status.Success = true;
                        Return.PrinterKey = NewPrinter.NodeId.ToString();
                    } // if( ExistingPrintersTree.getChildNodeCount() == 0 )
                    else
                    {
                        Return.addException( new CswDniException( ErrorType.Error, "That printer is already registered.", "registerLpc() found a printer with the same name: " + Request.LpcName ) );
                    }
                } // if( null != PrinterNT )
                else
                {
                    Return.addException( new CswDniException( ErrorType.Error, "Printer could not be created.", "registerLpc() could not access a Printer NodeType" ) );
                }
            } // if( null != PrinterOC )
            else
            {
                Return.addException( new CswDniException( ErrorType.Error, "Printer could not be created.", "registerLpc() could not access a Printer Object Class" ) );
            }
        } // registerLpc()

        public static void nextLabelJob( ICswResources CswResources, CswNbtLabelJobResponse Return, CswNbtLabelJobRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Status.Success = false;

            CswPrimaryKey PrinterNodeId = new CswPrimaryKey();
            PrinterNodeId.FromString( Request.PrinterKey );
            if( CswTools.IsPrimaryKey( PrinterNodeId ) )
            {
                CswNbtObjClassPrinter Printer = NbtResources.Nodes[PrinterNodeId];
                if( null != Printer )
                {
                    CswNbtMetaDataObjectClass PrinterOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrinterClass );
                    CswNbtMetaDataObjectClass PrintJobOC = NbtResources.MetaData.getObjectClass( NbtObjectClass.PrintJobClass );
                    if( null != PrinterOC && null != PrintJobOC )
                    {
                        CswNbtMetaDataObjectClassProp JobPrinterOCP = PrintJobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.Printer );
                        CswNbtMetaDataObjectClassProp JobCreatedDateOCP = PrintJobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.CreatedDate );
                        CswNbtMetaDataObjectClassProp JobStateOCP = PrintJobOC.getObjectClassProp( CswNbtObjClassPrintJob.PropertyName.JobState );

                        CswNbtView JobQueueView = new CswNbtView( NbtResources );
                        JobQueueView.ViewName = "Printer Job Queue";
                        // Print jobs...
                        CswNbtViewRelationship JobRel = JobQueueView.AddViewRelationship( PrintJobOC, false );
                        // ... assigned to this printer ...
                        JobQueueView.AddViewPropertyAndFilter( JobRel, JobPrinterOCP,
                                                               SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                                                               Value: PrinterNodeId.PrimaryKey.ToString(),
                                                               FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                        //with state==pending
                        JobQueueView.AddViewPropertyAndFilter( JobRel, JobStateOCP,
                                                               SubFieldName: CswNbtSubField.SubFieldName.Value,
                                                               Value: CswNbtObjClassPrintJob.StateOption.Pending,
                                                               FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                        // ... order by Created Date
                        CswNbtViewProperty CreatedDateVP = JobQueueView.AddViewProperty( JobRel, JobCreatedDateOCP );
                        JobQueueView.setSortProperty( CreatedDateVP, NbtViewPropertySortMethod.Ascending );

                        ICswNbtTree QueueTree = NbtResources.Trees.getTreeFromView( JobQueueView, false, true, true );

                        if( QueueTree.getChildNodeCount() >= 1 )
                        {
                            QueueTree.goToNthChild( 0 );
                            CswNbtObjClassPrintJob Job = QueueTree.getNodeForCurrentPosition();

                            Job.JobState.Value = CswNbtObjClassPrintJob.StateOption.Processing;
                            Job.ProcessedDate.DateTimeValue = DateTime.Now;
                            Job.postChanges( false );

                            Printer.LastJobRequest.DateTimeValue = DateTime.Now;
                            Printer.postChanges( false );

                            Return.Status.Success = true;
                            Return.JobKey = Job.NodeId.ToString();
                            Return.JobNo = Job.JobNo.Sequence;
                            Return.JobOwner = Job.RequestedBy.CachedNodeName;
                            Return.LabelCount = CswConvert.ToInt32( Job.LabelCount.Value );
                            Return.LabelData = Job.LabelData.Text;
                            Return.LabelName = Job.Label.CachedNodeName;
                            QueueTree.goToParentNode();
                            Return.RemainingJobCount = QueueTree.getChildNodeCount() - 1;
                        }
                        else
                        {
                            //success may have zero labels (and no labeldata)
                            Return.Status.Success = true;
                            Return.LabelCount = 0;
                            Return.RemainingJobCount = 0;
                        }
                    } // if( null != PrinterOC && null != PrintJobOC )
                    else
                    {
                        Return.addException( new CswDniException( ErrorType.Error, "Job fetch failed.", "nextLabelJob() could not access a Printer or Print Job Object Class" ) );
                    }
                } // if( null != Printer )
                else
                {
                    Return.addException( new CswDniException( ErrorType.Error, "Invalid Printer.", "nextLabelJob() printer key (" + Request.PrinterKey + ") did not match a node" ) );
                }
            } // if( CswTools.IsPrimaryKey( PrinterNodeId ) )
            else
            {
                Return.addException( new CswDniException( ErrorType.Error, "Invalid Printer.", "nextLabelJob() got an invalid printer key:" + Request.PrinterKey ) );
            }
        } // nextLabelJob()

        public static void updateLabelJob( ICswResources CswResources, CswNbtLabelJobUpdateResponse Return, CswNbtLabelJobUpdateRequest Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Status.Success = false;

            CswPrimaryKey JobNodeId = new CswPrimaryKey();
            JobNodeId.FromString( Request.JobKey );
            if( CswTools.IsPrimaryKey( JobNodeId ) )
            {
                CswNbtObjClassPrintJob Job = NbtResources.Nodes[JobNodeId];
                if( null != Job )
                {
                    if( Request.Succeeded )
                    {
                        Job.JobState.Value = CswNbtObjClassPrintJob.StateOption.Closed;
                    }
                    else
                    {
                        Job.JobState.Value = CswNbtObjClassPrintJob.StateOption.Error;
                        Job.ErrorInfo.Text = Request.ErrorMessage;
                    }
                    Job.EndedDate.DateTimeValue = DateTime.Now;
                    Job.postChanges( false );

                    Return.Status.Success = true;
                }
                else
                {
                    Return.addException( new CswDniException( ErrorType.Error, "Invalid Job.", "updateLabelJob() job key (" + Request.JobKey + ") did not match a node" ) );
                }
            } // if( CswTools.IsPrimaryKey( PrinterNodeId ) )
            else
            {
                Return.addException( new CswDniException( ErrorType.Error, "Invalid Job.", "updateLabelJob() got an invalid job key:" + Request.JobKey ) );
            }
        } // updateLabelJob()

    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
