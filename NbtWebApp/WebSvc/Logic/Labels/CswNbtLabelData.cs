using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using NbtWebApp.WebSvc.Returns;

namespace NbtWebApp.WebSvc.Logic.Labels
{
    /// <summary>
    /// Basic construct for a Print Label
    /// </summary>
    [DataContract]
    public class PrintLabel
    {
        /// <summary>
        /// The NodeId for which the label will be printed
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "Target for which a label will be printed" )]
        public string TargetId = string.Empty;

        /// <summary>
        /// The Node name for which the label will be printed
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "Target name for which a label will be printed" )]
        public string TargetName = string.Empty;

        /// <summary>
        /// The EPL text of the label to print
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "The EPL Text to define the Print Label" )]
        public string EplText = string.Empty;

        /// <summary>
        /// Optional collection of Picto file names and URLs
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "Optional collection of Picto file names and URLs" )]
        public Collection<Picto> Pictos = new Collection<Picto>();

        public class Picto
        {
            public string FileName = "";
            public string FileURL = "";
        }
    }

    /// <summary>
    /// Represents a print label for selection
    /// </summary>
    [DataContract]
    [Description( "Represents a print label for selection" )]
    public class Label
    {
        /// <summary>
        /// Label name
        /// </summary>
        [DataMember]
        [Description( "Label name" )]
        public string Name = string.Empty;

        /// <summary>
        /// Print Label id
        /// </summary>
        [DataMember]
        [Description( "Label ID" )]
        public string Id = string.Empty;

    }

    /// <summary>
    /// Request to register a label printer
    /// </summary>
    [DataContract]
    [Description( "Represents a label printer registration" )]
    public class LabelPrinter
    {
        /// <summary>
        /// LPC name is unique in nt schem to identify printer
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "LPC Name uniquely identifies an NBT Label Printer to users" )]
        public string LpcName = string.Empty;

        /// <summary>
        /// Additional descriptive info about the label printer
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "Additional descriptive info about the label printer" )]
        public string Description = string.Empty;

    }

    /// <summary>
    /// Request for next label printer job
    /// </summary>
    [DataContract]
    [Description( "Represents a label printer nodekey" )]
    public class CswNbtLabelJobRequest
    {
        /// <summary>
        /// Nodekey of defined NBT printer
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Nodekey of defined NBT Label Printer" )]
        public string PrinterKey = string.Empty;
    }

    /// <summary>
    /// Request for updating a label printer job
    /// </summary>
    [DataContract]
    [Description( "Represents a label print job" )]
    public class CswNbtLabelJobUpdateRequest
    {
        /// <summary>
        /// Nodekey of defined NBT print job
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Nodekey of defined NBT Label Print Job" )]
        public string JobKey = string.Empty;

        /// <summary>
        /// Nodekey of defined NBT print job
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "Whether the job finished successfully" )]
        public bool Succeeded = false;

        /// <summary>
        /// Error message
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "If an error occurred, this is the message to log" )]
        public string ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Response for updating a label printer job    
    /// </summary>
    [DataContract]
    [Description( "Represents a label print job" )]
    public class CswNbtLabelJobUpdateResponse : CswWebSvcReturn
    {
    }

    /// <summary>
    /// Request for next label printer job
    /// </summary>
    [DataContract]
    [Description( "Represents a label printer job" )]
    public class CswNbtLabelJobResponse : CswWebSvcReturn
    {
        /// <summary>
        /// NBT label printer job
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "label print JobNo" )]
        public string JobNo = string.Empty;

        /// <summary>
        /// NBT label printer job
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "label print Job key" )]
        public string JobKey = string.Empty;

        /// <summary>
        /// NBT label printer
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "label print Owner" )]
        public string JobOwner = string.Empty;

        /// <summary>
        /// NBT labels count
        /// </summary>
        [DataMember( IsRequired = true )]
        [Description( "label count for this job" )]
        public string LabelCount = string.Empty;

        /// <summary>
        /// NBT label name
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "label template name " )]
        public string LabelName = string.Empty;

        /// <summary>
        /// NBT label printer data
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "label printer data " )]
        public string LabelData = string.Empty;

        /// <summary>
        /// Remaining job count
        /// </summary>
        [DataMember( IsRequired = false )]
        [Description( "Remaining job count" )]
        public int RemainingJobCount = 0;
         
    }

    /// <summary>
    /// The data contract for a Print Label Request/Response
    /// </summary>
    [DataContract]
    public class NbtPrintLabel
    {
        /// <summary>
        /// Request base for fetching labels and subsequently EPL text
        /// </summary>
        [DataContract]
        public class Request
        {
            /// <summary>
            /// Request for a list of labels
            /// </summary>
            [DataContract]
            [Description( "Request for a list of labels" )]
            public class List
            {
                /// <summary>
                /// NodeTypeId for which to find matching Print Label nodes
                /// </summary>
                [DataMember( IsRequired = false )]
                [Description( "NodeTypeId for which to find matching Print Label nodes" )]
                public Int32 TargetTypeId = Int32.MinValue;

                /// <summary>
                /// NodeId of which to apply selected Print Label.
                /// If multiple NodeIds have been selected for Print Labels, the first shoudl be passed in.
                /// </summary>
                [DataMember( IsRequired = false )]
                [Description( "NodeId of which to apply selected Print Label." )]
                public string TargetId = string.Empty;
            }

            /// <summary>
            /// Request for an array of label EPL text
            /// </summary>
            [DataContract]
            [Description( "Request for an array of label EPL text" )]
            public class Get
            {
                private string _TargetId = string.Empty;

                /// <summary>
                /// NodeId(s) to print
                /// </summary>
                [DataMember( IsRequired = false )]
                [Description( "NodeId(s) to print" )]
                public string TargetId
                {
                    get { return _TargetId; }
                    set
                    {
                        _TargetId = value;
                        TargetIds = new CswCommaDelimitedString();
                        TargetIds.FromString( value );
                    }
                }

                /// <summary>
                /// Target NodeIds
                /// </summary>
                public CswCommaDelimitedString TargetIds = null;

                /// <summary>
                /// PrintLabel to print
                /// </summary>
                [DataMember( IsRequired = false )]
                [Description( "Print Label to print" )]
                public string LabelId = string.Empty;

                /// <summary>
                /// Optional params to apply to EPL text on label
                /// </summary>
                [DataMember( IsRequired = false )]
                [Description( "Optional params to apply to EPL text on label" )]
                public string Params = string.Empty;
            }
        }

        /// <summary>
        /// Label response classes
        /// </summary>
        [DataContract]
        public class Response
        {
            /// <summary>
            /// Returns a collection of EPL data for printing
            /// </summary>
            [DataContract]
            [Description( "Returns a collection of EPL data for printing" )]
            public class Epl
            {
                /// <summary>
                /// Labels
                /// </summary>
                [DataMember]
                [Description( "Collection of labels" )]
                public Collection<PrintLabel> Labels = new Collection<PrintLabel>();
            }

            /// <summary>
            /// Returns a list of available print labels
            /// </summary>
            [DataContract]
            public class List
            {
                /// <summary>
                /// Available print labels for a targettype
                /// </summary>
                [DataMember]
                [Description( "Collection of label names and ids" )]
                public Collection<Label> Labels = new Collection<Label>();

                /// <summary>
                /// Default selected LabelId (based on Target's Label Format)
                /// </summary>
                [DataMember]
                [Description( "Collection of label names and ids" )]
                public string SelectedLabelId = string.Empty;
            }

            /// <summary>
            /// Returns status of register printer request
            /// </summary>
            [DataContract]
            [Description( "Returns status of register printer request" )]
            public class registeredLpc
            {
                /// <summary>
                /// Printer
                /// </summary>
                [DataMember]
                [Description( "The label printer" )]
                public LabelPrinter printer = new LabelPrinter();

                /// <summary>
                /// printer is enabled
                /// </summary>
                [DataMember]
                [Description( "Whether printer is enabled in NBT" )]
                public bool enabled = false;

                /// <summary>
                /// status string of request
                /// </summary>
                [DataMember]
                [Description( "Status string of this request" )]
                public string status = string.Empty;

            }

        }


    }

}