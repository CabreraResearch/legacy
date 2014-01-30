using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
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
                    CswNbtMetaDataObjectClass PrintLabelObjectClass = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
                    CswNbtMetaDataObjectClassProp NodeTypesProperty = PrintLabelObjectClass.getObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );

                    CswNbtView PrintLabelView = new CswNbtView( NbtResources );
                    PrintLabelView.ViewName = "getPrintLabelsForNodeType(" + Request.TargetTypeId.ToString() + ")";
                    CswNbtViewRelationship PrintLabelRelationship = PrintLabelView.AddViewRelationship( PrintLabelObjectClass, true );
                    CswNbtViewProperty PrintLabelNodeTypesProperty = PrintLabelView.AddViewProperty( PrintLabelRelationship, NodeTypesProperty );
                    PrintLabelView.AddViewPropertyFilter( PrintLabelNodeTypesProperty,
                                                          NodeTypesProperty.getFieldTypeRule().SubFields.Default.Name,
                                                          CswEnumNbtFilterMode.Contains,
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
                    CswNbtMetaDataObjectClass PrintLabelClass = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintLabelClass );
                    foreach( CswNbtMetaDataNodeTypeProp RelationshipProp in TargetNodeType.getNodeTypeProps( CswEnumNbtFieldType.Relationship ) )
                    {
                        //bool PropMatchesPrintLabel = false;
                        //if( RelationshipProp.FKType == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                        //    RelationshipProp.FKValue == PrintLabelClass.ObjectClassId )
                        //{
                        //    PropMatchesPrintLabel = true;
                        //}
                        //else if( RelationshipProp.FKType == NbtViewRelatedIdType.NodeTypeId.ToString() )
                        //{
                        //    foreach( Int32 PrintLabelNTId in PrintLabelClass.getNodeTypeIds() )
                        //    {
                        //        if( RelationshipProp.FKValue == PrintLabelNTId )
                        //        {
                        //            PropMatchesPrintLabel = true;
                        //        }
                        //    }
                        //}
                        //if( PropMatchesPrintLabel )
                        if( RelationshipProp.FkMatchesDeprecated( PrintLabelClass ) )
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

            CswNbtMetaDataObjectClass PrintJobOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintJobClass );
            if( null == PrintJobOC )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Could not create new Print Job", "newPrintJob() could not find a Print Job Object Class" );
            }

            CswNbtMetaDataNodeType PrintJobNT = PrintJobOC.FirstNodeType;
            if( null == PrintJobNT )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Could not create new Print Job", "newPrintJob() could not find a Print Job NodeType" );
            }

            CswPrimaryKey LabelPk = new CswPrimaryKey();
            LabelPk.FromString( Request.LabelId );
            if( false == CswTools.IsPrimaryKey( LabelPk ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid print label key", "newPrintJob() Print Label Key is not a valid CswPrimaryKey: " + Request.PrinterId );
            }

            CswNbtObjClassPrintLabel PrintLabel = NbtResources.Nodes[LabelPk];
            if( null == PrintLabel )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid print label", "newPrintJob() Print Label Key did not match a node." );
            }

            CswPrimaryKey PrinterPk = new CswPrimaryKey();
            PrinterPk.FromString( Request.PrinterId );
            if( false == CswTools.IsPrimaryKey( PrinterPk ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid printer key", "newPrintJob() Printer Key is not a valid CswPrimaryKey: " + Request.PrinterId );
            }

            CswNbtObjClassPrinter Printer = NbtResources.Nodes[PrinterPk];
            if( null == Printer )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid printer", "newPrintJob() Printer Key did not match a node." );
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
                    Dictionary<string, string> PropVals = TargetNode.getPropertiesAndValues();
                    JobData += GenerateEPLScript( NbtResources, PrintLabel, TargetNode, PropVals );
                    JobCount += 1;
                }
            } // foreach( string TargetId in RealTargetIds )

            CswNbtObjClassPrintJob Job = NbtResources.Nodes.makeNodeFromNodeTypeId( PrintJobNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassPrintJob NewJob = NewNode;
                    NewJob.Label.RelatedNodeId = PrintLabel.NodeId;
                    NewJob.LabelCount.Value = JobCount;
                    NewJob.LabelData.Text = JobData;
                    NewJob.Printer.RelatedNodeId = Printer.NodeId;
                    NewJob.RequestedBy.RelatedNodeId = NbtResources.CurrentNbtUser.UserId;
                    NewJob.CreatedDate.DateTimeValue = DateTime.Now;
                    //NewJob.postChanges( false );
                } );
            Return.Data.JobId = Job.NodeId.ToString();
            Return.Data.JobNo = Job.JobNo.Sequence;
            Return.Data.JobLink = CswNbtNode.getNodeLink( Job.NodeId, Job.NodeName );

        } // newPrintJob()

        public static void newPrintJob( ICswResources CswResources, CswPrimaryKey PrinterId, CswPrimaryKey LabelId, CswPrimaryKey InitialContainerId, Collection<Dictionary<string, string>> ContainerPropVals )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtMetaDataObjectClass PrintJobOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintJobClass );
            if( null == PrintJobOC )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Could not create new Print Job", "newPrintJob() could not find a Print Job Object Class" );
            }

            CswNbtMetaDataNodeType PrintJobNT = PrintJobOC.FirstNodeType;
            if( null == PrintJobNT )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Could not create new Print Job", "newPrintJob() could not find a Print Job NodeType" );
            }

            if( false == CswTools.IsPrimaryKey( LabelId ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid print label key", "newPrintJob() Print Label Key is not a valid CswPrimaryKey: " + LabelId );
            }

            CswNbtObjClassPrintLabel PrintLabel = NbtResources.Nodes[LabelId];
            if( null == PrintLabel )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid print label", "newPrintJob() Print Label Key did not match a node." );
            }

            if( false == CswTools.IsPrimaryKey( PrinterId ) )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid printer key", "newPrintJob() Printer Key is not a valid CswPrimaryKey: " + PrinterId );
            }

            CswNbtObjClassPrinter Printer = NbtResources.Nodes[PrinterId];
            if( null == Printer )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid printer", "newPrintJob() Printer Key did not match a node." );
            }

            string JobData = string.Empty;
            Int32 JobCount = 0;


            /* BUG HERE
             * This method is for generating a print job when a user clicks finish on the Receiving wizard. We don't have the node ids for each container, so this print job will use the 
             * SAME node id for each label. This means we COULD get wrong data for labels when using an SQL query.
             * 
             * EXAMPLE: User writes in a query to get the Containers "Container Type" which lives on the Size. If two quantities use different sizes with different Container Types, then
             * the query will only fetch the container type of the FIRST container because we only have the container id of the container that was used to initialize the wizard
             */
            CswNbtNode TargetNode = NbtResources.Nodes[InitialContainerId];
            foreach( Dictionary<string, string> PropVals in ContainerPropVals )
            {
                JobData += GenerateEPLScript( NbtResources, PrintLabel, TargetNode, PropVals );
                JobCount += 1;
            }

            NbtResources.Nodes.makeNodeFromNodeTypeId( PrintJobNT.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassPrintJob NewJob = NewNode;
                NewJob.Label.RelatedNodeId = PrintLabel.NodeId;
                NewJob.LabelCount.Value = JobCount;
                NewJob.LabelData.Text = JobData;
                NewJob.Printer.RelatedNodeId = Printer.NodeId;
                NewJob.RequestedBy.RelatedNodeId = NbtResources.CurrentNbtUser.UserId;
                NewJob.CreatedDate.DateTimeValue = DateTime.Now;
            } );

        } // newPrintJob()

        public static void getEPLText( ICswResources CswResources, CswNbtLabelEpl Return, NbtPrintLabel.Request.Get Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            CswNbtObjClassPrintLabel PrintLabel = NbtResources.Nodes[Request.LabelId];
            if( null != PrintLabel )
            {
                foreach( string TargetId in Request.TargetIds )
                {
                    CswNbtNode TargetNode = NbtResources.Nodes[TargetId];
                    if( null != TargetNode )
                    {
                        Dictionary<string, string> PropVals = TargetNode.getPropertiesAndValues();
                        Return.Data.Labels.Add( new PrintLabel
                            {
                                TargetId = TargetNode.NodeId.ToString(),
                                TargetName = TargetNode.NodeName,
                                EplText = GenerateEPLScript( NbtResources, PrintLabel, TargetNode, PropVals )
                            } );
                    }
                }
            }
            if( Return.Data.Labels.Count == 0 )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Failed to get valid EPL text from the provided parameters.", "getEplText received invalid PropIdAttr and PrintLabelNodeIdStr parameters." );
            }
        } // getEPLText()


        /// <summary>
        /// Convert four-byte little-endian hex to integer
        /// </summary>
        private static Int32 _fourBytesToInt32( byte[] src, Int32 start )
        {
            Int32 headerLen = CswConvert.ToInt32( src[start] );
            headerLen += CswConvert.ToInt32( src[start + 1] ) * 256;
            headerLen += CswConvert.ToInt32( src[start + 2] ) * 256 * 256;
            headerLen += CswConvert.ToInt32( src[start + 3] ) * 256 * 256 * 256;
            return headerLen;
        }


        private static Dictionary<string, Int32> _extractParamSizes( string Params )
        {
            Dictionary<string, Int32> ParamSizes = new Dictionary<string, Int32>();
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
            return ParamSizes;
        }

        private static DataRow _executeSqlQuery( CswNbtResources NbtResources, string SqlScript, CswNbtNode Node )
        {
            DataRow SqlResultRow = null;
            if( false == string.IsNullOrEmpty( SqlScript ) )
            {
                String FormattedSqlScript = SqlScript.Replace( "{nodeid}", Node.NodeId.PrimaryKey.ToString() );
                CswArbitrarySelect SqlSelect = NbtResources.makeCswArbitrarySelect( "GenerateEPLScript_Sql", FormattedSqlScript );
                DataTable SqlResultTable = SqlSelect.getTable();
                if( SqlResultTable.Rows.Count > 0 )
                {
                    SqlResultRow = SqlResultTable.Rows[0];
                }
            }
            return SqlResultRow;
        }

        private static Dictionary<string, string> _findTemplateMatchesInEPLText( CswNbtResources NbtResources, string EPLText, Dictionary<string, string> PropVals, DataRow SqlResultRow, CswNbtNode TargetNode )
        {
            Dictionary<string, string> TemplateValues = new Dictionary<string, string>();
            MatchCollection TemplateMatches = Regex.Matches( EPLText, @"{.+}" );
            foreach( Match TemplateMatch in TemplateMatches )
            {
                string TemplateName = TemplateMatch.Value.Substring( 1, TemplateMatch.Value.Length - 2 );

                if( false == TemplateValues.ContainsKey( TemplateName ) )
                {
                    // Fetch template value
                    string TemplateValue = string.Empty;
                    if( _IsHazardTemplateName( TemplateName ) )
                    {
                        TemplateValue = _handleHazardTemplate( NbtResources, TargetNode, TemplateName );
                    }
                    else if( null != SqlResultRow && SqlResultRow.Table.Columns.Contains( TemplateName ) )
                    {
                        // Extract template values from SQL script results (case 31308)
                        TemplateValue = CswConvert.ToString( SqlResultRow[TemplateName] );
                    }
                    else
                    {
                        PropVals.TryGetValue( TemplateName, out TemplateValue );
                        //CswNbtMetaDataNodeType MetaDataNodeType = NbtResources.MetaData.getNodeType( Node.NodeTypeId );
                        //if( null != MetaDataNodeType )
                        //{
                        //    CswNbtMetaDataNodeTypeProp MetaDataProp = MetaDataNodeType.getNodeTypeProp( TemplateName );
                        //    if( null != MetaDataProp )
                        //    {
                        //        TemplateValue = Node.Properties[MetaDataProp].Gestalt;
                        //    }
                        //}
                    }

                    TemplateValues.Add( TemplateName, TemplateValue );
                }
            }
            return TemplateValues;
        }

        private static Dictionary<string, string> _chunkData( Dictionary<string, Int32> ParamSizes, Dictionary<string, string> TemplateVals )
        {
            Dictionary<string, string> ChunkedTemplateValues = new Dictionary<string, string>();
            foreach( KeyValuePair<string, string> Pair in TemplateVals )
            {
                string TemplateName = Pair.Key;
                string TemplateValue = Pair.Value;
                if( false == ChunkedTemplateValues.ContainsKey( TemplateName ) )
                {
                    if( false == string.IsNullOrEmpty( TemplateValue ) )
                    {
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
                            string CurrentTemplateName = TemplateName.Split( ':' )[0];
                            foreach( string Chunk in ValueChunks )
                            {
                                if( CurrentIteration == 1 )
                                {
                                    ChunkedTemplateValues[TemplateName] = Chunk;
                                }
                                else
                                {
                                    ChunkedTemplateValues[CurrentTemplateName + "_" + CurrentIteration] = Chunk;
                                }
                                CurrentIteration++;
                            }
                        } // if( null != ValueChunks )
                        else
                        {
                            ChunkedTemplateValues[TemplateName] = TemplateValue;
                        }
                    } // if( false == string.IsNullOrEmpty( TemplateValue ) )
                    else
                    {
                        ChunkedTemplateValues[TemplateName] = "";
                    }
                } // if( false == ChunkedTemplateValues.ContainsKey( TemplateName ) )
            } // foreach( KeyValuePair<string, string> Pair in TemplateVals )
            return ChunkedTemplateValues;
        }

        /// <summary>
        /// 
        /// </summary>
        private static string GenerateEPLScript( CswNbtResources NbtResources, CswNbtObjClassPrintLabel PrintLabel, CswNbtNode TargetNode, Dictionary<string, string> PropVals )
        {
            string EPLScript = string.Empty;

            if( null != PrintLabel )
            {
                string EPLText = PrintLabel.EplText.Text;
                string Params = PrintLabel.Params.Text;

                if( false == string.IsNullOrEmpty( EPLText ) )
                {
                    EPLScript = EPLText;

                    // Extract sizes from Params array
                    Dictionary<string, Int32> ParamSizes = _extractParamSizes( Params );

                    // Run SQL script (case 31308)
                    DataRow SqlResultRow = _executeSqlQuery( NbtResources, PrintLabel.SqlScript.Text, TargetNode );

                    // Find template names in the EPLText
                    Dictionary<string, string> TemplateValues = _findTemplateMatchesInEPLText( NbtResources, EPLText, PropVals, SqlResultRow, TargetNode );

                    // Handle splitting template value over lines
                    Dictionary<string, string> ChunkedTemplateValues = _chunkData( ParamSizes, TemplateValues );

                    // Apply template values to EPLScript
                    foreach( string TemplateName in ChunkedTemplateValues.Keys )
                    {
                        if( ChunkedTemplateValues.ContainsKey( TemplateName ) )
                        {
                            EPLScript = EPLScript.Replace( "{" + TemplateName + "}", ChunkedTemplateValues[TemplateName] );
                        }
                        else
                        {
                            EPLScript = EPLScript.Replace( "{" + TemplateName + "}", string.Empty );
                        }
                    }

                } // false == string.IsNullOrEmpty( EPLText ) && null != Node )

                if( string.IsNullOrEmpty( EPLScript ) )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Could not generate an EPL script from the provided parameters.", "EPL Text='" + EPLText + "', Params='" + Params + "'" );
                }
            } // if( null != PrintLabel )
            return EPLScript + "\n";
        } // GenerateEPLScript()

        public static void registerLpc( ICswResources CswResources, CswNbtLabelPrinterReg Return, LabelPrinter Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Return.Status.Success = false;

            CswNbtMetaDataObjectClass PrinterOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrinterClass );
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
                                                                   FilterMode: CswEnumNbtFilterMode.Equals );
                    ICswNbtTree ExistingPrintersTree = NbtResources.Trees.getTreeFromView( ExistingPrintersView, false, true, true );
                    if( ExistingPrintersTree.getChildNodeCount() == 0 )
                    {
                        CswNbtObjClassPrinter NewPrinter = NbtResources.Nodes.makeNodeFromNodeTypeId( PrinterNT.NodeTypeId, delegate( CswNbtNode NewNode )
                            {
                                ( (CswNbtObjClassPrinter) NewNode ).Name.Text = Request.LpcName;
                                ( (CswNbtObjClassPrinter) NewNode ).Description.Text = Request.Description;
                                //NewPrinter.postChanges( false );
                            } );
                        Return.Status.Success = true;
                        Return.PrinterKey = NewPrinter.NodeId.ToString();
                    } // if( ExistingPrintersTree.getChildNodeCount() == 0 )
                    else
                    {
                        Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "That printer is already registered.", "registerLpc() found a printer with the same name: " + Request.LpcName ) );
                    }
                } // if( null != PrinterNT )
                else
                {
                    Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Printer could not be created.", "registerLpc() could not access a Printer NodeType" ) );
                }
            } // if( null != PrinterOC )
            else
            {
                Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Printer could not be created.", "registerLpc() could not access a Printer Object Class" ) );
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
                    CswNbtMetaDataObjectClass PrinterOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrinterClass );
                    CswNbtMetaDataObjectClass PrintJobOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.PrintJobClass );
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
                                                               SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                               Value: PrinterNodeId.PrimaryKey.ToString(),
                                                               FilterMode: CswEnumNbtFilterMode.Equals );
                        //with state==pending
                        JobQueueView.AddViewPropertyAndFilter( JobRel, JobStateOCP,
                                                               SubFieldName: CswNbtFieldTypeRuleList.SubFieldName.Value,
                                                               Value: CswNbtObjClassPrintJob.StateOption.Pending,
                                                               FilterMode: CswEnumNbtFilterMode.Equals );
                        // ... order by Created Date
                        CswNbtViewProperty CreatedDateVP = JobQueueView.AddViewProperty( JobRel, JobCreatedDateOCP );
                        JobQueueView.setSortProperty( CreatedDateVP, CswEnumNbtViewPropertySortMethod.Ascending );

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

                        Return.PrinterName = Printer.Name.Text;

                    } // if( null != PrinterOC && null != PrintJobOC )
                    else
                    {
                        Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Job fetch failed.", "nextLabelJob() could not access a Printer or Print Job Object Class" ) );
                    }
                } // if( null != Printer )
                else
                {
                    Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Invalid Printer.", "nextLabelJob() printer key (" + Request.PrinterKey + ") did not match a node" ) );
                }
            } // if( CswTools.IsPrimaryKey( PrinterNodeId ) )
            else
            {
                Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Invalid Printer.", "nextLabelJob() got an invalid printer key:" + Request.PrinterKey ) );
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
                    Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Invalid Job.", "updateLabelJob() job key (" + Request.JobKey + ") did not match a node" ) );
                }
            } // if( CswTools.IsPrimaryKey( PrinterNodeId ) )
            else
            {
                Return.addException( CswResources, new CswDniException( CswEnumErrorType.Error, "Invalid Job.", "updateLabelJob() got an invalid job key:" + Request.JobKey ) );
            }
        } // updateLabelJob()


        #region Hazard Label

        private const string _GHSTemplatePrefix = "NBTGHS";
        private const string _DSDTemplatePrefix = "NBTDSD";

        private static bool _IsHazardTemplateName( string TemplateName )
        {
            return ( TemplateName.StartsWith( _GHSTemplatePrefix ) || TemplateName.StartsWith( _DSDTemplatePrefix ) );
        }

        private static string _handleHazardTemplate( CswNbtResources NbtResources, CswNbtNode TargetNode, string TemplateName )
        {
            string TemplateValue = string.Empty;

            if( TargetNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ContainerClass )
            {
                CswNbtObjClassContainer ContainerNode = TargetNode;
                if( null != ContainerNode.Material.RelatedNodeId )
                {
                    CswNbtPropertySetMaterial MaterialNode = NbtResources.Nodes[ContainerNode.Material.RelatedNodeId];
                    if( MaterialNode.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                    {
                        CswNbtObjClassChemical ChemicalNode = MaterialNode.Node;

                        // Figure out configuration from GHS/DSD
                        CswNbtView LabelCodeView = null;
                        CswDelimitedString ImageUrls = new CswCommaDelimitedString();
                        string Prefix = string.Empty;
                        if( TemplateName.StartsWith( _DSDTemplatePrefix ) )
                        {
                            // DSD - from Chemical DSD tab
                            Prefix = _DSDTemplatePrefix;
                            LabelCodeView = ChemicalNode.setupDsdPhraseView();
                            LabelCodeView = LabelCodeView.PrepGridView( ChemicalNode.NodeId );
                            ImageUrls = ChemicalNode.Pictograms.Value;
                        }
                        else if( TemplateName.StartsWith( _GHSTemplatePrefix ) )
                        {
                            // GHS - from user's matching GHS node
                            Prefix = _GHSTemplatePrefix;
                            CswNbtObjClassGHS GHSNode = _GetGhsNodeForContainer( NbtResources, TargetNode );
                            if( null != GHSNode )
                            {
                                LabelCodeView = GHSNode.setupPhraseView( GHSNode.LabelCodesGrid.View, GHSNode.LabelCodes.Value );
                                LabelCodeView = LabelCodeView.PrepGridView( GHSNode.NodeId );
                                ImageUrls = GHSNode.Pictograms.Value;
                            }
                        }

                        // Apply the template value
                        if( TemplateName.StartsWith( Prefix + "PICTOS:" ) || TemplateName.Equals( Prefix + "PICTOS" ) )  // Ignore "NBTGHSPICTOS_2"
                        {
                            if( ImageUrls.Count > 0 )
                            {
                                // pictos
                                Int32 Scale = 256;
                                bool NoBorder = false;
                                if( TemplateName.StartsWith( Prefix + "PICTOS:" ) )
                                {
                                    // decode parameters
                                    CswCommaDelimitedString hazParams = new CswCommaDelimitedString();
                                    hazParams.FromString( TemplateName.Substring( TemplateName.IndexOf( ':' ) + 1 ).ToLower().Trim() );
                                    foreach( string hazParam in hazParams )
                                    {
                                        if( hazParam.StartsWith( "scale" ) )
                                        {
                                            Int32 NewScale = CswConvert.ToInt32( hazParam.Substring( "scale".Length ) );
                                            if( NewScale > 0 && NewScale % 16 == 0 ) // case 30937
                                            {
                                                Scale = NewScale;
                                            }
                                        }
                                        if( hazParam == "noborder" )
                                        {
                                            NoBorder = true;
                                        }
                                    } // foreach( string hazParam in hazParams )
                                } // if( TemplateName.StartsWith( Prefix + "PICTOS:" ) )

                                TemplateValue = _getHazardPictosForLabel( NbtResources, ImageUrls, Scale, NoBorder );

                            } // if( ImageUrls.Count > 0 )
                        } // if( TemplateName.StartsWith( Prefix + "PICTOS:" ) || TemplateName.Equals( Prefix + "PICTOS" ) )
                        else if( null != LabelCodeView )
                        {
                            // NBTGHS  - phrases only
                            // NBTGHSA - phrases only
                            // NBTGHSB - codes only
                            // NBTGHSC - phrases and codes
                            TemplateValue = _getHazardCodeValueForLabel( NbtResources,
                                                                         LabelCodeView,
                                                                         ShowCodes: TemplateName.Equals( Prefix + "B" ) ||
                                                                                    TemplateName.Equals( Prefix + "C" ),
                                                                         ShowPhrases: TemplateName.Equals( Prefix ) ||
                                                                                      TemplateName.Equals( Prefix + "A" ) ||
                                                                                      TemplateName.Equals( Prefix + "C" ) );
                        } // else if( null != LabelCodeView )
                    } // if( MaterialNode.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                } // if( null != ContainerNode.Material.RelatedNodeId )
            } // if( ContainerNode.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ContainerClass)
            return TemplateValue;
        } // _handleHazardTemplate()


        private static CswNbtNode _GetGhsNodeForContainer( CswNbtResources NbtResources, CswNbtObjClassContainer ContainerNode )
        {
            CswNbtNode ret = null;

            CswNbtMetaDataNodeType ContainerNT = ContainerNode.NodeType;
            if( null != ContainerNT && ContainerNT.getObjectClass().ObjectClass == CswEnumNbtObjectClass.ContainerClass )
            {
                CswNbtMetaDataNodeTypeProp ContainerMaterialNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.Material );

                CswNbtMetaDataObjectClass GhsOC = NbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
                if( null != GhsOC )
                {
                    CswNbtMetaDataObjectClassProp GhsMaterialOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
                    CswNbtMetaDataObjectClassProp GhsJurisdictionOCP = GhsOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Jurisdiction );

                    CswNbtView GHSView = new CswNbtView( NbtResources );
                    GHSView.ViewName = "GHS for Container";

                    CswNbtViewRelationship ContainerRel = GHSView.AddViewRelationship( ContainerNode.NodeType, false );
                    CswNbtViewRelationship MaterialRel = GHSView.AddViewRelationship( ContainerRel, CswEnumNbtViewPropOwnerType.First, ContainerMaterialNTP, false );
                    CswNbtViewRelationship GHSRel = GHSView.AddViewRelationship( MaterialRel, CswEnumNbtViewPropOwnerType.Second, GhsMaterialOCP, false );

                    ContainerRel.NodeIdsToFilterIn.Add( ContainerNode.NodeId );
                    CswPrimaryKey JurisdictionId = NbtResources.CurrentNbtUser.JurisdictionId;
                    if( CswTools.IsPrimaryKey( JurisdictionId ) )
                    {
                        GHSView.AddViewPropertyAndFilter( GHSRel, GhsJurisdictionOCP,
                                                          Value: NbtResources.CurrentNbtUser.JurisdictionId.PrimaryKey.ToString(),
                                                          SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                          FilterMode: CswEnumNbtFilterMode.Equals );
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

                                ret = GHSTree.getNodeForCurrentPosition();

                            } // if( GHSTree.getChildNodeCount() > 0 ) ghs
                        } // if( GHSTree.getChildNodeCount() > 0 )     material
                    } // if( GHSTree.getChildNodeCount() > 0 )         container
                } // if(null != GhsOC)
            } // if( null != ContainerNT && ContainerNT.getObjectClass().ObjectClass == NbtObjectClass.ContainerClass )
            return ret;
        } // _getGhsForContainer()


        private static string _getHazardCodeValueForLabel( CswNbtResources NbtResources, CswNbtView LabelCodesView, bool ShowCodes, bool ShowPhrases )
        {
            string ret = string.Empty;

            if( null != LabelCodesView )
            {
                // Run the Label Codes View
                ICswNbtTree LabelCodesTree = NbtResources.Trees.getTreeFromView( LabelCodesView, false, false, false );
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
                        if( null != Ntp && Ntp.getObjectClassPropName() == CswNbtPropertySetPhrase.PropertyName.Code )
                        {
                            Code = Prop.Gestalt;
                        }
                        else
                        {
                            Phrase = Prop.Gestalt;
                        }
                    }
                    if( false == Phrases.ContainsKey( Code ) )
                    {
                        Phrases.Add( Code, Phrase );
                    }

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
            } // if( null != LabelCodesView )

            return ret;
        } // _getHazardCodeValueForLabel()

        private static string _getHazardPictosForLabel( CswNbtResources NbtResources, CswDelimitedString ImageUrls, Int32 Scale, bool NoBorder )
        {
            string ret = string.Empty;
            foreach( string ImageUrl in ImageUrls )
            {
                string RealImageUrl = ImageUrl.Replace( ".jpg", "" )
                                              .Replace( ".gif", "" );
                RealImageUrl = RealImageUrl.Replace( "/512/", "/" + Scale.ToString() + "/" );
                if( NoBorder )
                {
                    RealImageUrl += "_nobrd";
                }
                RealImageUrl += ".bmp";

                // use the EPL command GWx_orig,y_orig,width_bytes,height_bits,[byte array data]
                // image must be BMP
                byte[] rawimage = File.ReadAllBytes( CswFilePath.getConfigurationFilePath( CswEnumSetupMode.NbtWeb ) + "/../" + RealImageUrl );
                Int32 headerLen = _fourBytesToInt32( rawimage, 10 ); // BMP format has a variable length header block
                Int32 heightPixels = _fourBytesToInt32( rawimage, 18 );
                Int32 widthBytes = _fourBytesToInt32( rawimage, 22 ) / 8;
                //widthBytes = 256;

                // strip out header content
                Int32 newlen = rawimage.Length - headerLen;
                byte[] image = new byte[newlen];
                System.Buffer.BlockCopy( rawimage, headerLen, image, 0, newlen );

                // Convert the byte[] to a hex string with markup
                string imageHex = Convert.ToBase64String( image );

                //build the epl data and append the width (bytes) and height (pixels). template has the leading "GWn,n,"  before width
                ret += widthBytes + "," + heightPixels + ",<HEX>" + imageHex + "</HEX>\n";

            } // foreach( string ImageUrl in ImageUrls )
            return ret;
        } // _getHazardPictosForLabel()

        #endregion Hazard Label


    } // class CswNbtWebServiceTabsAndProps

} // namespace ChemSW.Nbt.WebServices
