using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public interface ICswNbtNodePropNodeReference
    {
        /// <summary>
        /// Empty the subfield data on this Prop
        /// </summary>
        void clearRelationship();

        /// <summary>
        /// Primary key of related node
        /// </summary>
        CswPrimaryKey ReferencedNodeId { get; set; }

        /// <summary>
        /// Cached node name of related node
        /// </summary>
        string CachedNodeName { get; set; }

        /// <summary>
        /// RelatedIdType of the TargetId
        /// </summary>
        CswEnumNbtViewRelatedIdType TargetType { get; }

        /// <summary>
        /// Relationship's Target NodeTypeId
        /// </summary>
        Int32 TargetId { get; }

        /// <summary>
        /// Method to refresh the cached node name
        /// </summary>
        void RefreshNodeName();

    }//ICswNbtNodePropNodeReference

}//namespace ChemSW.Nbt.PropTypes

