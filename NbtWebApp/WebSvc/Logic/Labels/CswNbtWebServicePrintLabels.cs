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
                    PrintLabel Label = GenerateEPLScript( NbtResources, EPLText, Params, TargetNode );
                    JobData += Label.EplText;
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
                        PrintLabel Label = GenerateEPLScript( NbtResources, EPLText, Params, TargetNode );
                        Return.Data.Labels.Add( Label );
                    }
                }
            }
            if( Return.Data.Labels.Count == 0 )
            {
                throw new CswDniException( ErrorType.Error, "Failed to get valid EPL text from the provided parameters.", "getEplText received invalid PropIdAttr and PrintLabelNodeIdStr parameters." );
            }
        } // getEPLText()

        private static PrintLabel GenerateEPLScript( CswNbtResources NbtResources, string EPLText, string Params, CswNbtNode Node )
        {
            PrintLabel Ret = new PrintLabel();
            string EPLScript = string.Empty;
            if( false == string.IsNullOrEmpty( EPLText ) )
            {
                EPLScript = EPLText;
                while( EPLScript.Contains( "{" ) )
                {
                    Int32 ParamStartIndex = EPLScript.IndexOf( "{" );
                    Int32 ParamEndIndex = EPLScript.IndexOf( "}" );

                    string PropertyParamString = EPLScript.Substring( ParamStartIndex, ParamEndIndex - ParamStartIndex + 1 );
                    string PropertyParamName = PropertyParamString.Substring( 1, PropertyParamString.Length - 2 );
                    // Find the property
                    if( null != Node )
                    {
                        Ret.TargetId = Node.NodeId.ToString();
                        Ret.TargetName = Node.NodeName;

                        CswNbtMetaDataNodeType MetaDataNodeType = NbtResources.MetaData.getNodeType( Node.NodeTypeId );
                        if( null != MetaDataNodeType )
                        {
                            CswNbtMetaDataNodeTypeProp MetaDataProp = MetaDataNodeType.getNodeTypeProp( PropertyParamName );
                            if( null != MetaDataProp )
                            {
                                string PropertyValue = Node.Properties[MetaDataProp].Gestalt;
                                if( MetaDataProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Image )
                                {
                                    PropertyValue = Node.Properties[MetaDataProp].AsImage.FileName;
                                    Ret.Pictos.Add( new PrintLabel.Picto
                                        {
                                            FileName = PropertyValue,
                                            FileURL = Node.Properties[MetaDataProp].AsImage.ImageUrl
                                        } );
                                }

                                bool FoundMatch = false;
                                if( false == string.IsNullOrEmpty( Params ) )
                                {
                                    string[] ParamsArray = Params.Split( '\n' );
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
            //}
            if( string.IsNullOrEmpty( EPLScript ) )
            {
                throw new CswDniException( ErrorType.Error, "Could not generate an EPL script from the provided parameters.", "EPL Text='" + EPLText + "', Params='" + Params + "'" );
            }
            Ret.EplText = EPLScript + "\n";
            return Ret;
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

                        CswNbtView JobQueueView = new CswNbtView( NbtResources );
                        JobQueueView.ViewName = "Printer Job Queue";
                        // Print jobs...
                        CswNbtViewRelationship JobRel = JobQueueView.AddViewRelationship( PrintJobOC, false );
                        // ... assigned to this printer ...
                        JobQueueView.AddViewPropertyAndFilter( JobRel, JobPrinterOCP,
                                                               SubFieldName: CswNbtSubField.SubFieldName.NodeID,
                                                               Value: PrinterNodeId.PrimaryKey.ToString(),
                                                               FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                        // ... order by Created Date
                        CswNbtViewProperty CreatedDateVP = JobQueueView.AddViewProperty( JobRel, JobCreatedDateOCP );
                        JobQueueView.setSortProperty( CreatedDateVP, NbtViewPropertySortMethod.Ascending );

                        ICswNbtTree QueueTree = NbtResources.Trees.getTreeFromView( JobQueueView, false, true, true );

                        if( QueueTree.getChildNodeCount() >= 1 )
                        {
                            QueueTree.goToNthChild( 1 );
                            CswNbtObjClassPrintJob Job = QueueTree.getNodeForCurrentPosition();

                            Job.JobState.Value = CswNbtObjClassPrintJob.StateOption.Processing;
                            Job.ProcessedDate.DateTimeValue = DateTime.Now;

                            Printer.LastJobRequest.DateTimeValue = DateTime.Now;

                            Return.Status.Success = true;
                            Return.JobKey = Job.NodeId.ToString();
                            Return.JobNo = Job.JobNo.Sequence;
                            Return.JobOwner = Job.RequestedBy.CachedNodeName;
                            Return.LabelCount = CswConvert.ToInt32( Job.LabelCount.Value );
                            Return.LabelData = Job.LabelData.Text;
                            Return.LabelName = Job.Label.CachedNodeName;
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
