using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Base class for all Fieldtype-specific CswNbtNodeProp classes
    /// </summary>
    [DataContract]
    public abstract class CswNbtNodeProp
    {

        /// <summary>
        /// Database interaction layer
        /// </summary>
        private CswNbtNodePropData _CswNbtNodePropData = null;

        /// <summary>
        /// Reference to the CswNbtResources object
        /// </summary>
        protected CswNbtResources _CswNbtResources = null;

        /// <summary>
        /// Node to which this property value is attached
        /// </summary>
        protected CswNbtNode _Node = null;

        /// <summary>
        /// Meta Data for this property
        /// </summary>
        protected CswNbtMetaDataNodeTypeProp _CswNbtMetaDataNodeTypeProp = null;

        //public CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp
        //{
        //    get { return _CswNbtMetaDataNodeTypeProp; }
        //}

        protected ICswNbtFieldTypeRule _FieldTypeRule
        {
            get { return _CswNbtMetaDataNodeTypeProp.getFieldTypeRule(); }
        }

        protected Dictionary<CswNbtSubField, Tuple<Func<dynamic>, Action<dynamic>>> _SubFieldMethods = new Dictionary<CswNbtSubField, Tuple<Func<dynamic>, Action<dynamic>>>();


        //TODO: witout at least one serializable item, this ENTIRE CLASS will try to serialize resulting in a helpfull "error" message. When we WCFify this class we can remove this prop
        [DataMember]
        private string ToDo = string.Empty;

        /// <summary>
        /// WCF Default Contructor
        /// </summary>
        protected CswNbtNodeProp()
        {
        }

        //All WCF Data Contracts MUST have a default constructor

        /// <summary>
        /// Constructor
        /// </summary>
        protected CswNbtNodeProp( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp MetaDataNodeTypeProp, CswNbtNode Node )
        {
            _CswNbtNodePropData = CswNbtNodePropData;
            _Node = Node;
            _CswNbtResources = CswNbtResources;
            _CswNbtMetaDataNodeTypeProp = MetaDataNodeTypeProp;

        }

        //generic

        public delegate void OnPropChangeHandler( CswNbtNodeProp Prop, bool Creating );

        public OnPropChangeHandler OnPropChange;

        /// <summary>
        /// Set an event to be executed when the property's value is changed
        /// </summary>
        public void SetOnPropChange( OnPropChangeHandler ChangeHandler )
        {
            OnPropChange = ChangeHandler;
        }

        /// <summary>
        /// Returns true if the subfield was modified
        /// </summary>
        public bool wasSubFieldModified( CswEnumNbtSubFieldName SubFieldName )
        {
            return _CswNbtNodePropData.wasSubFieldModified( SubFieldName );
        }

        /// <summary>
        /// Returns true if any subfield was modified
        /// </summary>
        public bool wasAnySubFieldModified( bool IncludePendingUpdate = false )
        {
            return _CswNbtNodePropData.wasAnySubFieldModified( IncludePendingUpdate );
        }

        /// <summary>
        /// Sets a subfield to have been modified
        /// </summary>
        public void setSubFieldModified( CswEnumNbtSubFieldName SubFieldName, bool Modified = true )
        {
            _CswNbtNodePropData.setSubFieldModified( SubFieldName, Modified );
        }

        /// <summary>
        /// Clears all subfield modified flags
        /// </summary>
        public void clearSubFieldModifiedFlags()
        {
            _CswNbtNodePropData.clearSubFieldModifiedFlags();
        }



        /// <summary>
        /// Text value for property
        /// </summary>
        //public abstract string Gestalt { get; }
        public string Gestalt
        {
            get { return _CswNbtNodePropData.Gestalt; }
            set { _CswNbtNodePropData.SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, value ); }
        }

        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        public abstract bool Empty { get; }

        /// <summary>
        /// Name of property
        /// </summary>
        public string PropName
        {
            get { return ( _CswNbtMetaDataNodeTypeProp.PropName ); }
        }

        /// <summary>
        /// Primary Key of property
        /// </summary>
        public Int32 JctNodePropId
        {
            get { return ( _CswNbtNodePropData.JctNodePropId ); }
        }

        /// <summary>
        /// Primary Key of property's nodetypeprop
        /// </summary>
        public Int32 NodeTypePropId
        {
            get { return ( _CswNbtMetaDataNodeTypeProp.PropId ); }
        }

        /// <summary>
        /// MetaData class for NodeTypeProp
        /// </summary>
        public CswNbtMetaDataNodeTypeProp NodeTypeProp
        {
            get { return _CswNbtMetaDataNodeTypeProp; }
        }

        /// <summary>
        /// Reference to FieldType Meta Data object for this property
        /// </summary>
        public CswNbtMetaDataFieldType getFieldType()
        {
            return ( _CswNbtMetaDataNodeTypeProp.getFieldType() );
        }

        /// <summary>
        /// Reference to FieldType Meta Data object for this property
        /// </summary>
        public CswEnumNbtFieldType getFieldTypeValue()
        {
            return ( _CswNbtMetaDataNodeTypeProp.getFieldTypeValue() );
        }

        /// <summary>
        /// If the property derives from an Object Class Property, the Object Class Property's Primary Key
        /// </summary>
        public Int32 ObjectClassPropId
        {
            get { return _CswNbtMetaDataNodeTypeProp.ObjectClassPropId; }
        }

        /// <summary>
        /// If the property derives from an Object Class Property, the Object Class Property's name
        /// </summary>
        public string ObjectClassPropName
        {
            get { return _CswNbtMetaDataNodeTypeProp.getObjectClassPropName(); }
        }

        //public bool IsPropRow( DataRow DataRow ) { return ( _CswNbtNodePropData.Row == DataRow ); }
        /// <summary>
        /// True if the property must must be unique
        /// </summary>
        public bool IsUnique()
        {
            return _CswNbtMetaDataNodeTypeProp.IsUnique();
        }

        //set { _CswNbtMetaDataNodeTypeProp.IsUnique = value; } }
        /// <summary>
        /// True if the property must have a value
        /// </summary>
        public bool Required
        {
            get { return _CswNbtMetaDataNodeTypeProp.IsRequired; }
        }

        //set { _CswNbtMetaDataNodeTypeProp.IsRequired = value; } }

        /// <summary>
        /// True if the property must have a value (Temporarily)
        /// </summary>
        public bool TemporarilyRequired
        {
            get { return _CswNbtNodePropData.TemporarilyRequired; }
            set { _CswNbtNodePropData.TemporarilyRequired = value; }
        }

        /// <summary>
        /// The default value of the property
        /// </summary>
        public CswNbtNodePropWrapper DefaultValue
        {
            get { return ( _CswNbtMetaDataNodeTypeProp.DefaultValue ); }
        }

        /// <summary>
        /// Whether a default value of the property is defined
        /// </summary>
        public bool HasDefaultValue()
        {
            return ( _CswNbtMetaDataNodeTypeProp.HasDefaultValue() );
        }

        /// <summary>
        /// The Node's Primary Key
        /// </summary>
        public CswPrimaryKey NodeId
        {
            get { return ( _CswNbtNodePropData.NodeId ); }
        }

        //set { _CswNbtNodePropData.NodeId = value; } }

        /// <summary>
        /// True if the property's value cannot be changed by the end user
        /// <para>NOTE: In the case of CswNbtNodePropButton, use Hidden instead.</para>
        /// </summary>
        public bool ReadOnly
        {
            get { return ( _CswNbtNodePropData.ReadOnly ); }
        }

        /// <summary>
        /// Set whether the property's value can be changed by the end user
        /// <para>NOTE: In the case of CswNbtNodePropButton, use setHidden instead.</para>
        /// </summary>
        /// <param name="value">New value for ReadOnly</param>
        /// <param name="SaveToDb">If true, save this value to the database permanently.  If false, applies only to this request.</param>
        public void setReadOnly( bool value, bool SaveToDb )
        {
            _CswNbtNodePropData.setReadOnly( value, SaveToDb );
        }


        /// <summary>
        ///  Determines whether a property displays.
        /// </summary>
        public bool Hidden
        {
            get { return ( _CswNbtNodePropData.Hidden ); }
        }

        /// <summary>
        /// Set whether the property displays.
        /// </summary>
        /// <param name="value">New value for Hidden</param>
        /// <param name="SaveToDb">If true, save this value to the database permanently.  If false, applies only to this request.</param>
        public void setHidden( bool value, bool SaveToDb )
        {
            _CswNbtNodePropData.setHidden( value, SaveToDb );
        }

        /// <summary>
        /// Property Value: Field1
        /// </summary>
        protected string Field1
        {
            get { return ( _CswNbtNodePropData.Field1 ); }
        }

        // set { _CswNbtNodePropData.Field1 = value; } }
        /// <summary>
        /// Property Value: Field2
        /// </summary>
        protected string Field2
        {
            get { return ( _CswNbtNodePropData.Field2 ); }
        }

        //set { _CswNbtNodePropData.Field2 = value; } }
        /// <summary>
        /// Property Value: Field3
        /// </summary>
        protected string Field3
        {
            get { return ( _CswNbtNodePropData.Field3 ); }
        }

        //set { _CswNbtNodePropData.Field3 = value; } }
        /// <summary>
        /// Property Value: Field4
        /// </summary>
        protected string Field4
        {
            get { return ( _CswNbtNodePropData.Field4 ); }
        }

        //set { _CswNbtNodePropData.Field4 = value; } }
        /// <summary>
        /// Property Value: Field5
        /// </summary>
        protected string Field5
        {
            get { return ( _CswNbtNodePropData.Field5 ); }
        }

        //set { _CswNbtNodePropData.Field5 = value; } }
        /// <summary>
        /// Property Value: Field5
        /// </summary>
        protected string ClobData
        {
            get { return ( _CswNbtNodePropData.ClobData ); }
        }

        //set { _CswNbtNodePropData.ClobData = value; } }
        /// <summary>
        /// If true, the property value needs to be updated by the Scheduler
        /// </summary>
        public bool PendingUpdate
        {
            get { return ( _CswNbtNodePropData.PendingUpdate ); } //set { _CswNbtNodePropData.PendingUpdate = value; } }
            set { _CswNbtNodePropData.SetPropRowValue( CswEnumNbtSubFieldName.PendingUpdate, CswEnumNbtPropColumn.PendingUpdate, value ); }
        }

        /// <summary>
        /// Property value used in name template
        /// </summary>
        public abstract string ValueForNameTemplate { get; }

        /// <summary>
        /// Event which fires before the node prop data row is written to the database
        /// </summary>
        /// <param name="IsCopy">True if the update is part of a Copy operation</param>
        /// <param name="OverrideUniqueValidation"></param>
        public virtual void onBeforeUpdateNodePropRow( CswNbtNode Node, bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            if( false == Node.Properties[this.NodeTypeProp].Empty ) //case 26546 - we allow unique properties to be empty
            {
                //bz # 6686
                if( IsUnique() && wasAnySubFieldModified() && !OverrideUniqueValidation )
                {
                    CswNbtView CswNbtView = new CswNbtView( _CswNbtResources );
                    CswNbtView.ViewName = "Other Nodes, for Property Uniqueness";

                    CswNbtViewRelationship ViewRel = null;
                    if( NodeTypeProp.IsGlobalUnique() ) // BZ 9754
                        ViewRel = CswNbtView.AddViewRelationship( _CswNbtResources.MetaData.getObjectClassByNodeTypeId( NodeTypeProp.NodeTypeId ), false );
                    else
                        ViewRel = CswNbtView.AddViewRelationship( NodeTypeProp.getNodeType(), false );

                    if( NodeId != null )
                        ViewRel.NodeIdsToFilterOut.Add( NodeId );

                    //bz# 5959
                    CswNbtViewProperty UniqueValProperty = CswNbtView.AddViewProperty( ViewRel, NodeTypeProp );

                    // BZ 10099
                    this.NodeTypeProp.getFieldTypeRule().AddUniqueFilterToView( CswNbtView, UniqueValProperty, Node.Properties[this.NodeTypeProp] );

                    ICswNbtTree NodeTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, CswNbtView, true, false, false );

                    if( NodeTree.getChildNodeCount() > 0 )
                    {
                        NodeTree.goToNthChild( 0 );
                        if( !IsCopy || Required )
                        {
                            CswNbtNode CswNbtNode = NodeTree.getNodeForCurrentPosition();
                            string EsotericMessage = "Unique constraint violation: The proposed value '" + this.Gestalt + "' ";
                            EsotericMessage += "of property '" + NodeTypeProp.PropName + "' ";
                            EsotericMessage += "for nodeid (" + NodeId.ToString() + ") ";
                            EsotericMessage += "of nodetype '" + NodeTypeProp.getNodeType().NodeTypeName + "' ";
                            EsotericMessage += "is invalid because the same value is already set for node '" + CswNbtNode.NodeName + "' (" + CswNbtNode.NodeId.ToString() + ").";
                            string ExotericMessage = "The " + NodeTypeProp.PropName + " property value must be unique";
                            throw ( new CswDniException( CswEnumErrorType.Warning, ExotericMessage, EsotericMessage ) );
                        }
                        else
                        {
                            // BZ 9987 - Clear the value
                            this._CswNbtNodePropData.ClearValue();
                            //this.clearModifiedFlag();
                            this.clearSubFieldModifiedFlags();
                        }
                    }

                } //if IsUnique
            } //if empty

            // case 25780 - copy first 512 characters of gestalt to gestaltsearch
            if( _CswNbtNodePropData.wasAnySubFieldModified() )
            {
                string GestaltSearchValue = _CswNbtNodePropData.Gestalt;
                if( GestaltSearchValue.Length > 512 )
                {
                    GestaltSearchValue = GestaltSearchValue.Substring( 0, 512 );
                }
                SetPropRowValue( CswEnumNbtSubFieldName.GestaltSearch, CswEnumNbtPropColumn.GestaltSearch, GestaltSearchValue );

                // We fire this here so that it only fires once per row, not once per subfield.  See case 27241.
                if( null != OnPropChange )
                {
                    OnPropChange( this, Creating );
                }
            }

        }

        protected bool SetPropRowValue( CswNbtSubField SubField, object value, bool IsNonModifying = false )
        {
            return _CswNbtNodePropData.SetPropRowValue( SubField, value, IsNonModifying );
        }

        protected bool SetPropRowValue( CswEnumNbtSubFieldName SubFieldName, CswEnumNbtPropColumn column, object value, bool IsNonModifying = false )
        {
            return _CswNbtNodePropData.SetPropRowValue( SubFieldName, column, value, IsNonModifying );
        }

        protected string GetPropRowValue( CswNbtSubField SubField )
        {
            return _CswNbtNodePropData.GetPropRowValue( SubField );
        }

        protected DateTime GetPropRowValueDate( CswNbtSubField SubField )
        {
            return _CswNbtNodePropData.GetPropRowValueDate( SubField );
        }


        /// <summary>
        /// Event which fires when the property value is retrieved from the database
        /// </summary>
        public virtual void onNodePropRowFilled()
        {
        }

        /// <summary>
        /// Handles when the property value is copied to another node
        /// </summary>
        public virtual void Copy( CswNbtNodePropData Source )
        {
            // Default, just copy the data values

            CswEnumNbtFieldType FieldType = Source.getFieldTypeValue();
            ICswNbtFieldTypeRule FieldTypeRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldType );

            foreach( CswNbtSubField SubField in FieldTypeRule.SubFields )
            {
                if( SubField.Column == CswEnumNbtPropColumn.Field1_FK )
                {
                    //Implementing FieldType specific behavior here. Blame Steve.
                    if( FieldType == CswEnumNbtFieldType.ViewReference )
                    {
                        CswNbtView View = _CswNbtResources.ViewSelect.restoreView( Source.NodeTypeProp.DefaultValue.AsViewReference.ViewId );
                        CswNbtView ViewCopy = new CswNbtView( _CswNbtResources );
                        ViewCopy.saveNew( View.ViewName, View.Visibility, View.VisibilityRoleId, View.VisibilityUserId, View );
                        SetSubFieldValue( CswEnumNbtSubFieldName.ViewID, ViewCopy.ViewId.get() );
                    }
                    else
                    {
                        SetSubFieldValue( SubField.Name, Source.Field1_Fk );
                    }
                } // if( SubField.Column == CswEnumNbtPropColumn.Field1_FK )
                else
                {
                    SetSubFieldValue( SubField.Name, Source.GetPropRowValue( SubField ) );
                }
            } // foreach( CswNbtSubField SubField in NodeTypeProp.getFieldTypeRule().SubFields )

            // Also copy Gestalt, which usually isn't listed as a subfield
            SetSubFieldValue( CswEnumNbtSubFieldName.Gestalt, Source.Gestalt );
            SetSubFieldValue( CswEnumNbtSubFieldName.GestaltSearch, Source.GestaltSearch );
        }

        /// <summary>
        /// Returns the original value of the default subfield for this property
        /// </summary>
        public string GetOriginalPropRowValue()
        {
            string ret = string.Empty;
            ICswNbtFieldTypeRule FieldTypeRule = _CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            if( FieldTypeRule != null )
            {
                ret = GetOriginalPropRowValue( FieldTypeRule.SubFields.Default.Column );
            }
            return ret;
        }

        /// <summary>
        /// Returns the original value of the provided subfield for this property
        /// </summary>
        public string GetOriginalPropRowValue( CswEnumNbtSubFieldName SubfieldName )
        {
            string ret = string.Empty;
            ICswNbtFieldTypeRule FieldTypeRule = _CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            if( FieldTypeRule != null )
            {
                CswEnumNbtPropColumn Column = FieldTypeRule.SubFields[SubfieldName].Column;
                ret = GetOriginalPropRowValue( Column );
            }
            return ret;
        }

        /// <summary>
        /// Set the value for a subfield, triggering the logic associated with that subfield on the fieldtype
        /// reference: http://stackoverflow.com/questions/289980/is-there-a-delegate-available-for-properties-in-c
        /// </summary>
        public void SetSubFieldValue( CswEnumNbtSubFieldName SubFieldName, object value )
        {
            foreach( CswNbtSubField SubFieldKey in _SubFieldMethods.Keys )
            {
                if( SubFieldName == SubFieldKey.Name )
                {
                    // This calls the appropriate set; method in the CswNbtNodeProp* class
                    if( null != _SubFieldMethods[SubFieldKey].Item2 )
                    {
                        _SubFieldMethods[SubFieldKey].Item2( value );
                    }
                }
            }
        } // SetSubFieldValue


        /// <summary>
        /// Get the value for a subfield
        /// </summary>
        public object GetSubFieldValue( CswEnumNbtSubFieldName SubFieldName )
        {
            object ret = null;
            foreach( CswNbtSubField SubFieldKey in _SubFieldMethods.Keys )
            {
                if( SubFieldName == SubFieldKey.Name )
                {
                    // This calls the appropriate set; method in the CswNbtNodeProp* class
                    if( null != _SubFieldMethods[SubFieldKey].Item1 )
                    {
                        ret = _SubFieldMethods[SubFieldKey].Item1();
                    }
                }
            }
            return ret;
        } // GetSubFieldValue

        /// <summary>
        /// Returns the original value of the provided column for this property
        /// </summary>
        public string GetOriginalPropRowValue( CswEnumNbtPropColumn Column )
        {
            return _CswNbtNodePropData.GetOriginalPropRowValue( Column );
        }

        public abstract void SyncGestalt();

        public string OtherPropGestalt( Int32 NodeTypePropId )
        {
            return _CswNbtNodePropData.OtherPropGestalt( NodeTypePropId );
        }

        /// <summary>
        /// Executed before the default values are set for a property
        /// </summary>
        /// <returns>Whether or not the data should be copied from the default value row</returns>
        public virtual bool onBeforeSetDefault()
        {
            return true;
        }

        /// <summary>
        /// Executed after the default values are set for a property
        /// </summary>
        public virtual void onAfterSetDefault() { }

        public delegate void BeforeRenderHandler( CswNbtNodeProp Prop );

        public BeforeRenderHandler onBeforeRender = null;

        /// <summary>
        /// Executed before the property is exported to the UI
        /// </summary>
        public void TriggerOnBeforeRender()
        {
            if( null != onBeforeRender )
            {
                onBeforeRender( this );
            }
        }

        /// <summary>
        /// Set an event to be executed before the property is exported to the UI
        /// </summary>
        public void SetOnBeforeRender( BeforeRenderHandler handler )
        {
            onBeforeRender = handler;
        }


        #region Xml Operations

        abstract public void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );
        public virtual void ToJSON( JObject ParentObject )
        {
            //TriggerOnBeforeRender();  this is now done in CswNbtSdTabsAndProps
        }
        public abstract void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap );

        #endregion Xml Operations


    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
