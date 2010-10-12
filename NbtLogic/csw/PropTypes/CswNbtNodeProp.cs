using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Base class for all Fieldtype-specific CswNbtNodeProp classes
    /// </summary>
    abstract public class CswNbtNodeProp
    {


        /// <summary>
        /// Database interaction layer
        /// </summary>
        protected CswNbtNodePropData _CswNbtNodePropData = null;
        /// <summary>
        /// Reference to the CswNbtResources object
        /// </summary>
        protected CswNbtResources _CswNbtResources = null;
        ///// <summary>
        ///// Node to which this property value is attached
        ///// </summary>
        //protected CswNbtNode _CswNbtNode = null;

        /// <summary>
        /// Meta Data for this property
        /// </summary>
        protected CswNbtMetaDataNodeTypeProp _CswNbtMetaDataNodeTypeProp = null;
        //public CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp
        //{
        //    get { return _CswNbtMetaDataNodeTypeProp; }
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        protected CswNbtNodeProp( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp MetaDataNodeTypeProp )
        {
            _CswNbtNodePropData = CswNbtNodePropData;
            //_CswNbtNode = CswNbtNodePropData.Node;
            _CswNbtResources = CswNbtResources;
            _CswNbtMetaDataNodeTypeProp = MetaDataNodeTypeProp;
        }//generic


        /// <summary>
        /// Sets the property to non-modified.  Changes made between the last save and this call are lost.
        /// </summary>
        public void clearModifiedFlag()
        {
            _CswNbtNodePropData.clearModifiedFlag();
        }//clearModifiedFlag()


        /// <summary>
        /// True if the value was modified or not.  Cannot be set.
        /// </summary>
        /// <remark>
        /// You don't want to put a setter here. We want CswNbtNodePropData
        /// to be entirely in charge of the meaning of "WasModified";
        /// Use clearModifyFlag() if you want to do a reset
        /// </remark>
        public bool WasModified
        {
            get
            {
                return ( _CswNbtNodePropData.WasModified );
            }//

        }//WasModified

        /// <summary>
        /// True prevents the ModifiedFlag from changing
        /// </summary>
        public bool SuspendModifyTracking
        {
            set
            {
                _CswNbtNodePropData.SuspendModifyTracking = value;
            }
            get
            {
                return ( _CswNbtNodePropData.SuspendModifyTracking );
            }//
        }//SuspendModifyTracking

        /// <summary>
        /// Text value for property
        /// </summary>
        public abstract string Gestalt { get; }
        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        public abstract bool Empty { get; }

        /// <summary>
        /// Name of property
        /// </summary>
        public string PropName { get { return ( _CswNbtMetaDataNodeTypeProp.PropName ); } }
        /// <summary>
        /// Primary Key of property
        /// </summary>
        public Int32 JctNodePropId { get { return ( _CswNbtNodePropData.JctNodePropId ); } }
        /// <summary>
        /// Primary Key of property's nodetypeprop
        /// </summary>
        public Int32 NodeTypePropId { get { return ( _CswNbtMetaDataNodeTypeProp.PropId ); } }
        /// <summary>
        /// MetaData class for NodeTypeProp
        /// </summary>
        public CswNbtMetaDataNodeTypeProp NodeTypeProp { get { return _CswNbtMetaDataNodeTypeProp; } }
        /// <summary>
        /// Reference to FieldType Meta Data object for this property
        /// </summary>
        public CswNbtMetaDataFieldType FieldType { get { return ( _CswNbtMetaDataNodeTypeProp.FieldType ); } }
        /// <summary>
        /// If the property derives from an Object Class Property, the Object Class Property's Primary Key
        /// </summary>
        public Int32 ObjectClassPropId
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.ObjectClassProp != null )
                    return ( _CswNbtMetaDataNodeTypeProp.ObjectClassProp.PropId );
                else
                    return Int32.MinValue;
            }
        }
        /// <summary>
        /// If the property derives from an Object Class Property, the Object Class Property's name
        /// </summary>
        public string ObjectClassPropName
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.ObjectClassProp != null )
                    return ( _CswNbtMetaDataNodeTypeProp.ObjectClassProp.PropName );
                else
                    return string.Empty;
            }
        }
        //public bool IsPropRow( DataRow DataRow ) { return ( _CswNbtNodePropData.Row == DataRow ); }
        /// <summary>
        /// True if the property must must be unique
        /// </summary>
        public bool IsUnique { get { return _CswNbtMetaDataNodeTypeProp.IsUnique; } } //set { _CswNbtMetaDataNodeTypeProp.IsUnique = value; } }
        /// <summary>
        /// True if the property must have a value
        /// </summary>
        public bool Required { get { return _CswNbtMetaDataNodeTypeProp.IsRequired; } } //set { _CswNbtMetaDataNodeTypeProp.IsRequired = value; } }
        /// <summary>
        /// The default value of the property
        /// </summary>
        public CswNbtNodePropWrapper DefaultValue { get { return ( _CswNbtMetaDataNodeTypeProp.DefaultValue ); } }
        /// <summary>
        /// Whether a default value of the property is defined
        /// </summary>
        public bool HasDefaultValue() { return ( _CswNbtMetaDataNodeTypeProp.HasDefaultValue() ); }

        /// <summary>
        /// The Node's Primary Key
        /// </summary>
        public CswPrimaryKey NodeId { get { return ( _CswNbtNodePropData.NodeId ); } set { _CswNbtNodePropData.NodeId = value; } }
        /// <summary>
        /// True if the property's value cannot be changed by the end user
        /// </summary>
        public bool ReadOnly { get { return ( _CswNbtNodePropData.ReadOnly ); } set { _CswNbtNodePropData.ReadOnly = value; } }
        /// <summary>
        ///  Determines whether a property displays.
        /// </summary>
        public bool Hidden { get { return ( _CswNbtNodePropData.Hidden ); } set { _CswNbtNodePropData.Hidden = value; } }
        /// <summary>
        /// Property Value: Field1
        /// </summary>
        protected string Field1 { get { return ( _CswNbtNodePropData.Field1 ); } set { _CswNbtNodePropData.Field1 = value; } }
        /// <summary>
        /// Property Value: Field2
        /// </summary>
        protected string Field2 { get { return ( _CswNbtNodePropData.Field2 ); } set { _CswNbtNodePropData.Field2 = value; } }
        /// <summary>
        /// Property Value: Field3
        /// </summary>
        protected string Field3 { get { return ( _CswNbtNodePropData.Field3 ); } set { _CswNbtNodePropData.Field3 = value; } }
        /// <summary>
        /// Property Value: Field4
        /// </summary>
        protected string Field4 { get { return ( _CswNbtNodePropData.Field4 ); } set { _CswNbtNodePropData.Field4 = value; } }
        /// <summary>
        /// Property Value: Field5
        /// </summary>
        protected string Field5 { get { return ( _CswNbtNodePropData.Field5 ); } set { _CswNbtNodePropData.Field5 = value; } }
        /// <summary>
        /// If true, the property value needs to be updated by the Scheduler
        /// </summary>
        public bool PendingUpdate { get { return ( _CswNbtNodePropData.PendingUpdate ); } set { _CswNbtNodePropData.PendingUpdate = value; } }

        /// <summary>
        /// Event which fires before the node prop data row is written to the database
        /// </summary>
        /// <param name="IsCopy">True if the update is part of a Copy operation</param>
        virtual public void onBeforeUpdateNodePropRow( bool IsCopy )
        {
            //bz # 6686
            if( IsUnique && WasModified )
            {
                CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                CswNbtView.ViewName = "Other Nodes, for Property Uniqueness";

                CswNbtViewRelationship ViewRel = null;
                if( NodeTypeProp.IsGlobalUnique )  // BZ 9754
                    ViewRel = CswNbtView.AddViewRelationship( NodeTypeProp.NodeType.ObjectClass, false );
                else
                    ViewRel = CswNbtView.AddViewRelationship( NodeTypeProp.NodeType, false );

                if( NodeId != null )
                    ViewRel.NodeIdsToFilterOut.Add( NodeId );

                //bz# 5959
                CswNbtViewProperty UniqueValProperty = CswNbtView.AddViewProperty( ViewRel, NodeTypeProp );

                // BZ 10099
                this.NodeTypeProp.FieldTypeRule.AddUniqueFilterToView( CswNbtView, UniqueValProperty, _CswNbtNodePropData );

                ICswNbtTree NodeTree = _CswNbtResources.Trees.getTreeFromView( CswNbtView, true, true, false, false );

                if( NodeTree.getChildNodeCount() > 0 )
                {
                    NodeTree.goToNthChild( 0 );
                    if( !IsCopy || Required )
                    {
                        CswNbtNode CswNbtNode = NodeTree.getNodeForCurrentPosition();
                        string EsotericMessage = "Unique constraint violation: The proposed value '" + this.Gestalt + "' ";
                        EsotericMessage += "of property '" + NodeTypeProp.PropName + "' ";
                        EsotericMessage += "for nodeid (" + this.NodeId.ToString() + ") ";
                        EsotericMessage += "of nodetype '" + NodeTypeProp.NodeType.NodeTypeName + "' ";
                        EsotericMessage += "is invalid because the same value is already set for node '" + CswNbtNode.NodeName + "' (" + CswNbtNode.NodeId.ToString() + ").";
                        string ExotericMessage = "The " + NodeTypeProp.PropName + " property value must be unique";
                        throw ( new CswDniException( ExotericMessage, EsotericMessage ) );
                    }
                    else
                    {
                        // BZ 9987 - Clear the value
                        this._CswNbtNodePropData.ClearValue();
                        this.clearModifiedFlag();
                    }
                }


            }//if IsUnique
        }
        /// <summary>
        /// Event which fires when the property value is retrieved from the database
        /// </summary>
        virtual public void onNodePropRowFilled() { }

        /// <summary>
        /// Handles when the property value is copied to another node
        /// </summary>
        virtual public void Copy( CswNbtNodePropData Source )
        {
            // Default, just copy the data values
            _CswNbtNodePropData.copy( Source );
        }



        #region Xml Operations

        public enum NbtDataItemType { Unknown, Reference, Value };
        public enum NbtDataItemSource { Unknown, Doc, Proc };
        public enum IdRefContext { Unknown, Source, Destination };
        public enum IdRefState { Unknown, SourceResolved, SourceOpen, DestinationOpen, DestinationResolved };

        abstract public void ToXml( XmlNode ParentNode );
        abstract public void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );
        abstract public void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );

        #endregion Xml Operations


    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
