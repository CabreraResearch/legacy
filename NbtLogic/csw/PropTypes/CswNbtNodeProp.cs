using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

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
        
        public void SetOnPropChange( CswNbtNodePropData.OnPropChangeHandler ChangeHandler )
        {
            _CswNbtNodePropData.OnPropChange = ChangeHandler;
        }
        
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
        public CswNbtMetaDataFieldType getFieldType() { return ( _CswNbtMetaDataNodeTypeProp.getFieldType() ); }
        /// <summary>
        /// If the property derives from an Object Class Property, the Object Class Property's Primary Key
        /// </summary>
        public Int32 ObjectClassPropId
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.ObjectClassPropId;
            }
        }
        /// <summary>
        /// If the property derives from an Object Class Property, the Object Class Property's name
        /// </summary>
        public string ObjectClassPropName
        {
            get
            {
                CswNbtMetaDataObjectClassProp OCP = _CswNbtMetaDataNodeTypeProp.getObjectClassProp();
                if( OCP != null )
                    return OCP.PropName;
                else
                    return string.Empty;
            }
        }
        //public bool IsPropRow( DataRow DataRow ) { return ( _CswNbtNodePropData.Row == DataRow ); }
        /// <summary>
        /// True if the property must must be unique
        /// </summary>
        public bool IsUnique() { return _CswNbtMetaDataNodeTypeProp.IsUnique(); } //set { _CswNbtMetaDataNodeTypeProp.IsUnique = value; } }
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
        /// This value is never saved to the database, so it's useful for object-class-specific logic
        /// </summary>
        public bool ReadOnlyTemporary { get { return ( _CswNbtNodePropData.ReadOnlyTemporary ); } set { _CswNbtNodePropData.ReadOnlyTemporary = value; } }
        /// <summary>
        /// True if the property's value cannot be changed by the end user
        /// This value is saved to the database if the node is saved.
        /// </summary>
        public bool ReadOnlyPermanent { get { return ( _CswNbtNodePropData.ReadOnlyPermanent ); } set { _CswNbtNodePropData.ReadOnlyPermanent = value; } }
        /// <summary>
        ///  Determines whether a property displays.
        /// This value is never saved to the database, so it's useful for object-class-specific logic
        /// </summary>
        public bool HiddenTemporary { get { return ( _CswNbtNodePropData.HiddenTemporary ); } set { _CswNbtNodePropData.HiddenTemporary = value; } }
        /// <summary>
        ///  Determines whether a property displays.
        /// This value is saved to the database if the node is saved.
        /// </summary>
        public bool HiddenPermanent { get { return ( _CswNbtNodePropData.HiddenPermanent ); } set { _CswNbtNodePropData.HiddenPermanent = value; } }
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
        virtual public void onBeforeUpdateNodePropRow( bool IsCopy, bool OverrideUniqueValidation )
        {

            /*case 26545
            Check if the field type is text, then check if it's a null string
            If it is not a null string, we can continue on to check if it is unique
            if it IS a null string, we don't do the uniqueness check, as uniqueness does not apply to empty strings
            */
            bool fieldIsNotNull = true;
            CswNbtMetaDataFieldType textFT = _CswNbtResources.MetaData.getFieldType( CswNbtMetaDataFieldType.NbtFieldType.Text );
            if( NodeTypeProp.getFieldType().FieldTypeId == textFT.FieldTypeId )
            {
                if( _CswNbtResources.Nodes[this.NodeId].Properties[this.NodeTypeProp].Field1.Equals( "" ) )
                {
                    fieldIsNotNull = false;
                }
            }

            if( fieldIsNotNull )
            {
                //bz # 6686
                if( IsUnique() && WasModified && !OverrideUniqueValidation )
                {
                    CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                    CswNbtView.ViewName = "Other Nodes, for Property Uniqueness";

                    CswNbtViewRelationship ViewRel = null;
                    if( NodeTypeProp.IsGlobalUnique() )  // BZ 9754
                        ViewRel = CswNbtView.AddViewRelationship( _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeProp.NodeTypeId ), false );
                    else
                        ViewRel = CswNbtView.AddViewRelationship( NodeTypeProp.getNodeType(), false );

                    if( NodeId != null )
                        ViewRel.NodeIdsToFilterOut.Add( NodeId );

                    //bz# 5959
                    CswNbtViewProperty UniqueValProperty = CswNbtView.AddViewProperty( ViewRel, NodeTypeProp );

                    // BZ 10099
                    this.NodeTypeProp.getFieldTypeRule().AddUniqueFilterToView( CswNbtView, UniqueValProperty, _CswNbtResources.Nodes[this.NodeId].Properties[this.NodeTypeProp] );

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
                            EsotericMessage += "of nodetype '" + NodeTypeProp.getNodeType().NodeTypeName + "' ";
                            EsotericMessage += "is invalid because the same value is already set for node '" + CswNbtNode.NodeName + "' (" + CswNbtNode.NodeId.ToString() + ").";
                            string ExotericMessage = "The " + NodeTypeProp.PropName + " property value must be unique";
                            throw ( new CswDniException( ErrorType.Warning, ExotericMessage, EsotericMessage ) );
                        }
                        else
                        {
                            // BZ 9987 - Clear the value
                            this._CswNbtNodePropData.ClearValue();
                            this.clearModifiedFlag();
                        }
                    }

                }//if IsUnique
            } //false == equals()

            // case 25780 - copy first 512 characters of gestalt to gestaltsearch
            if( _CswNbtNodePropData.WasModified )
            {
                string GestaltSearchValue = _CswNbtNodePropData.Gestalt;
                if( GestaltSearchValue.Length > 512 )
                {
                    GestaltSearchValue = GestaltSearchValue.Substring( 0, 512 );
                }
                _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.GestaltSearch, GestaltSearchValue );
            }

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
        abstract public void ToXElement( XElement ParentNode );
        abstract public void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );
        abstract public void ReadXElement( XElement XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );
        abstract public void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );
        public abstract void ToJSON( JObject ParentObject );
        public abstract void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );

        #endregion Xml Operations


    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
