using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

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

                /// <summary>
                /// The print mechanism (ActiveX or jZebra)
                /// </summary>
                [DataMember( IsRequired = false )]
                [Description( "Client-side mechanism to print the label (ActiveX or jZebra)" )]
                public string ControlType = CswNbtObjClassPrintLabel.ControlTypes.jZebra;

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
            }


        }

    }

}