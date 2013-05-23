using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignNodeTypeProp : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string AuditLevel = "Audit Level";
            public const string CompoundUnique = "Compound Unique";
            public const string DisplayConditionFilter = "Display Condition Filter";
            public const string DisplayConditionProperty = "Display Condition Property";
            public const string DisplayConditionSubfield = "Display Condition Subfield";
            public const string DisplayConditionValue = "Display Condition Value";
            public const string FieldType = "Field Type";
            public const string HelpText = "Help Text";
            public const string NodeTypeValue = "NodeType";
            public const string ObjectClassPropName = "Original Name";
            public const string PropName = "Prop Name";
            public const string ReadOnly = "Read Only";
            public const string Required = "Required";
            public const string Unique = "Unique";
            public const string UseNumbering = "Use Numbering";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeTypeProp( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeTypeProp
        /// </summary>
        public static implicit operator CswNbtObjClassDesignNodeTypeProp( CswNbtNode Node )
        {
            CswNbtObjClassDesignNodeTypeProp ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignNodeTypePropClass ) )
            {
                ret = (CswNbtObjClassDesignNodeTypeProp) Node.ObjClass;
            }
            return ret;
        }

        /// <summary>
        /// The NodeTypeProp that this node represents
        /// </summary>
        public CswNbtMetaDataNodeTypeProp RelationalNodeTypeProp
        {
            get
            {
                CswNbtMetaDataNodeTypeProp ret = null;
                if( CswTools.IsPrimaryKey( RelationalId ) )
                {
                    ret = _CswNbtResources.MetaData.getNodeTypeProp( RelationalId.PrimaryKey );
                }
                return ret;
            }
        }

        /// <summary>
        /// The NodeType that the relational property is attached to
        /// </summary>
        public CswNbtMetaDataNodeType RelationalNodeType
        {
            get
            {
                CswNbtMetaDataNodeType ret = null;
                if( CswTools.IsPrimaryKey( NodeTypeValue.RelatedNodeId ) )
                {
                    CswNbtObjClassDesignNodeType NodeTypeNode = _CswNbtResources.Nodes[NodeTypeValue.RelatedNodeId];
                    if( null != NodeTypeNode )
                    {
                        ret = NodeTypeNode.RelationalNodeType;
                    }
                }
                return ret;
            }
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            // Note: RelationalNodeTypeProp is null here!

            // Make sure propname is unique for this nodetype
            if( null == RelationalNodeType )
            {
                throw new CswDniException( CswEnumErrorType.Warning,
                                           "Property must be attached to a nodetype",
                                           "Attempted to save a new property without a nodetype" );
            }
            if( null != RelationalNodeType.getNodeTypeProp( PropName.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning,
                                           "Property Name must be unique per nodetype",
                                           "Attempted to save a propname which is equal to a propname of another property in this nodetype" );
            }

            // ------------------------------------------------------------
            // This logic from makeNewNodeType and makeNewProp in CswNbtMetaData.cs
            // ------------------------------------------------------------
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                Int32 PropId = RelationalId.PrimaryKey;

                CswTableUpdate PropsUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeTypeProp_afterCreateNode_PropsUpdate", "nodetype_props" );
                DataTable PropsTable = PropsUpdate.getTable( "nodetypepropid", PropId );
                if( PropsTable.Rows.Count > 0 )
                {
                    CswNbtMetaDataNodeTypeTab FirstTab = RelationalNodeType.getFirstNodeTypeTab();

                    DataRow InsertedRow = PropsTable.Rows[0];
                    InsertedRow["firstpropversionid"] = PropId;

                    // Copy values from ObjectClassProp
                    CswNbtMetaDataObjectClassProp OCProp = null;
                    if( DerivesFromObjectClassProp )
                    {
                        OCProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( ObjectClassPropName.Value ) );
                    }

                    // Set objectclasspropid and nodetypepropid
                    // This would sync by itself in CswNbtNodeWriterRelationalDb, but we need it sooner than that
                    InsertedRow["nodetypeid"] = RelationalNodeType.NodeTypeId;
                    if( null != OCProp )
                    {
                        InsertedRow["objectclasspropid"] = OCProp.PropId;

                        // Copy all attributes from the Object Class Prop
                        _CswNbtResources.MetaData.CopyNodeTypePropFromObjectClassProp( OCProp, InsertedRow );

                        // Sync properties with what was just copied
                        CswTableSelect mapSelect = _CswNbtResources.makeCswTableSelect( "CswNbtObjClassDesignNodeTypeProp_afterCreateNode_jctSelect", "jct_dd_ntp" );
                        DataTable mapTable = mapSelect.getTable( "where nodetypepropid in (select nodetypepropid from nodetype_props where nodetypeid = " + NodeType.NodeTypeId.ToString() + ")" );
                        foreach( DataRow mapRow in mapTable.Rows )
                        {
                            _CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( mapRow["datadictionaryid"] ) );
                            CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( CswConvert.ToInt32( mapRow["nodetypepropid"] ) );
                            if( _CswNbtResources.DataDictionary.ColumnName != "objectclasspropid" &&
                                _CswNbtResources.DataDictionary.ColumnName != "nodetypeid" )
                            {
                                CswEnumNbtSubFieldName SubFieldName = (CswEnumNbtSubFieldName) mapRow["subfieldname"].ToString();
                                if( SubFieldName.ToString() == CswNbtResources.UnknownEnum &&
                                    null != ntp.getFieldTypeRule().SubFields.Default )
                                {
                                    SubFieldName = ntp.getFieldTypeRule().SubFields.Default.Name;
                                }
                                if( SubFieldName.ToString() != CswNbtResources.UnknownEnum )
                                {
                                    Node.Properties[ntp].SetPropRowValue( ntp.getFieldTypeRule().SubFields[SubFieldName].Column, InsertedRow[_CswNbtResources.DataDictionary.ColumnName] );
                                }
                            }
                        }

                        // Handle setFk() from ObjectClassProp
                        if( null != ObjectClassPropValue && ObjectClassPropValue.FKValue != Int32.MinValue )
                        {
                            ICswNbtFieldTypeRule rule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
                            Collection<CswNbtFieldTypeAttribute> Attributes = rule.getAttributes();
                            foreach( CswNbtFieldTypeAttribute attr in Attributes )
                            {
                                object value = null;
                                switch( attr.Column )
                                {
                                    case CswEnumNbtPropertyAttributeColumn.Fktype:
                                        value = ObjectClassPropValue.FKType;
                                        break;
                                    case CswEnumNbtPropertyAttributeColumn.Fkvalue:
                                        value = ObjectClassPropValue.FKValue;
                                        break;
                                    case CswEnumNbtPropertyAttributeColumn.Valuepropid:
                                        value = ObjectClassPropValue.ValuePropId;
                                        break;
                                    case CswEnumNbtPropertyAttributeColumn.Valueproptype:
                                        value = ObjectClassPropValue.ValuePropType;
                                        break;
                                }
                                if( null != value )
                                {
                                    CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( attr.Name );
                                    Node.Properties[ntp].SetPropRowValue( ntp.getFieldTypeRule().SubFields[attr.SubFieldName ?? ntp.getFieldTypeRule().SubFields.Default.Name].Column, value );
                                }
                            } // foreach( CswNbtFieldTypeAttribute attr in Attributes )
                        } // if( null != ObjectClassPropValue && ObjectClassPropValue.FKValue != Int32.MinValue )
                    } // if( null != OCProp )

                    ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
                    InsertedRow["isquicksearch"] = CswConvert.ToDbVal( RelationalRule.SearchAllowed );

                    PropsUpdate.update( PropsTable );

                    // Layout
                    if( OCProp.PropName.Equals( CswNbtObjClass.PropertyName.Save ) ) //case 29181 - Save prop on Add/Edit layouts at the bottom of tab
                    {
                        _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, Int32.MaxValue, 1 );
                        _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, Int32.MaxValue, 1 );
                    }
                    else
                    {
                        _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, Int32.MinValue, 1 );
                        if( OCProp.getFieldType().IsLayoutCompatible( CswEnumNbtLayoutType.Add ) &&
                            ( ( OCProp.IsRequired &&
                                false == OCProp.HasDefaultValue() ) ||
                              ( OCProp.SetValueOnAdd ||
                                ( Int32.MinValue != OCProp.DisplayColAdd &&
                                  Int32.MinValue != OCProp.DisplayRowAdd ) ) ) )
                        {
                            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, OCProp.DisplayRowAdd, OCProp.DisplayColAdd );
                        }
                    }

                    RelationalRule.afterCreateNodeTypeProp( RelationalNodeTypeProp );

                    //_CswNbtResources.MetaData._CswNbtMetaDataResources.RecalculateQuestionNumbers( RelationalNodeTypeProp.getNodeType() );    // this could cause versioning

                    // Handle default values from ObjectClassProp
                    if( null != OCProp )
                    {
                        _CswNbtResources.MetaData.CopyNodeTypePropDefaultValueFromObjectClassProp( OCProp, RelationalNodeTypeProp );
                    }

                    //if( OnMakeNewNodeTypeProp != null )
                    //{
                    //    OnMakeNewNodeTypeProp( NewProp );
                    //}

                    //will need to refresh auto-views
                    //_RefreshViewForNodetypeId.Add( NtpModel.NodeTypeId );

                } // if( PropsTable.Rows.Count > 0 )
            } // if( CswTools.IsPrimaryKey( RelationalId ) )

        } // beforeCreateNode()

        public override void afterCreateNode()
        {
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            // If display condition is set, required must be false
            if( false == DisplayConditionProperty.Empty )
            {
                Required.Checked = CswEnumTristate.False;
                Required.setReadOnly( true, true );
            }
            else
            {
                Required.setReadOnly( false, true );
            }

            //// Call setFk on new Relationship target
            //if( FieldTypeValue == CswEnumNbtFieldType.Relationship )
            //{
            //    CswNbtMetaDataNodeTypeProp TargetNTP = this.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Target.ToString() );
            //    if( null != TargetNTP )
            //    {
            //        CswNbtNodePropWrapper TargetProp = Node.Properties[TargetNTP];
            //        if( null != TargetProp )
            //        {
            //            RelationalNodeTypeProp.SetFK( TargetProp.AsMetaDataList.Type.ToString(), TargetProp.AsMetaDataList.Id );
            //        }
            //    }
            //}

            if( FieldTypeValue == CswEnumNbtFieldType.Question )
            {
                CswNbtMetaDataNodeTypeProp PossibleAnswersNTP = this.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.PossibleAnswers.ToString() );
                CswNbtMetaDataNodeTypeProp CompliantAnswersNTP = this.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.CompliantAnswers.ToString() );
                if( null != PossibleAnswersNTP && null != CompliantAnswersNTP )
                {
                    CswNbtNodePropWrapper PossibleAnswersProp = Node.Properties[PossibleAnswersNTP];
                    CswNbtNodePropWrapper CompliantAnswersProp = Node.Properties[CompliantAnswersNTP];
                    if( null != PossibleAnswersProp && null != CompliantAnswersProp )
                    {
                        // Guarantee a Compliant Answer for Question
                        if( CompliantAnswersProp.AsMultiList.Empty &&
                            CompliantAnswersProp.AsMultiList.Options.Count > 0 )
                        {
                            //throw new CswDniException( CswEnumErrorType.Warning, "Compliant Answer is a required field", "Compliant Answer is a required field" );
                            CompliantAnswersProp.AsMultiList.AddValue( CompliantAnswersProp.AsMultiList.Options.First().Value );
                        }
                    }
                }
            } // if( FieldTypeValue == CswEnumNbtFieldType.Question )

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtResources.MetaData.DeleteNodeTypeProp( _CswNbtResources.MetaData.getNodeTypeProp( this.RelationalId.PrimaryKey ) );
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            // Options for Field Type property
            SortedList<string, CswNbtNodeTypePropListOption> FieldTypeOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            Dictionary<Int32, CswEnumNbtFieldType> FieldTypeIds = _CswNbtResources.MetaData.getFieldTypeIds();
            foreach( Int32 FieldTypeId in FieldTypeIds.Keys )
            {
                CswEnumNbtFieldType thisFieldtypeName = FieldTypeIds[FieldTypeId];
                FieldTypeOptions.Add( thisFieldtypeName.ToString(), new CswNbtNodeTypePropListOption( thisFieldtypeName.ToString(), FieldTypeId.ToString() ) );
            }
            FieldType.Options.Override( FieldTypeOptions.Values );

            // Options for ObjectClassPropName
            Int32 objectClassPropId = CswConvert.ToInt32( ObjectClassPropName.Value );
            if( Int32.MinValue != objectClassPropId )
            {
                CswNbtMetaDataObjectClassProp objectClassProp = _CswNbtResources.MetaData.getObjectClassProp( objectClassPropId );
                ObjectClassPropName.Options.Override( new Collection<CswNbtNodeTypePropListOption>()
                    {
                        new CswNbtNodeTypePropListOption( objectClassProp.PropName, objectClassProp.PropId.ToString() )
                    } );
            }

            // Options for Compliant Answer
            if( FieldTypeValue == CswEnumNbtFieldType.Question )
            {
                CswNbtMetaDataNodeTypeProp CompliantAnswersNTP = this.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.CompliantAnswers.ToString() );
                if( null != CompliantAnswersNTP )
                {
                    CswNbtNodePropWrapper CompliantAnswersProp = Node.Properties[CompliantAnswersNTP];
                    if( null != CompliantAnswersProp )
                    {
                        CompliantAnswersProp.AsMultiList.InitOptions = _initCompliantAnswerOptions;
                    }
                }
            }

            // Display conditions
            _setDisplayConditionOptions();
            DisplayConditionProperty.SetOnPropChange( _DisplayConditionProperty_Change );

            // Servermanaged and ReadOnly
            if( _CswNbtResources.EditMode != CswEnumNbtNodeEditMode.Add )
            {
                NodeTypeValue.ServerManaged = true;
            }
            if( NodeType.IsLocked )
            {
                this.Node.setReadOnly( true, true );
            }

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropList AuditLevel { get { return ( _CswNbtNode.Properties[PropertyName.AuditLevel] ); } }
        public CswNbtNodePropLogical CompoundUnique { get { return ( _CswNbtNode.Properties[PropertyName.CompoundUnique] ); } }
        public CswNbtNodePropList DisplayConditionFilter { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionFilter] ); } }
        public CswNbtNodePropRelationship DisplayConditionProperty { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionProperty] ); } }
        public void _DisplayConditionProperty_Change( CswNbtNodeProp Prop )
        {
            _setDisplayConditionOptions();
        }
        public CswNbtNodePropList DisplayConditionSubfield { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionSubfield] ); } }
        public CswNbtNodePropText DisplayConditionValue { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionValue] ); } }
        public CswNbtNodePropList FieldType { get { return ( _CswNbtNode.Properties[PropertyName.FieldType] ); } }
        public CswNbtNodePropMemo HelpText { get { return ( _CswNbtNode.Properties[PropertyName.HelpText] ); } }
        public CswNbtNodePropRelationship NodeTypeValue { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeValue] ); } }
        public CswNbtNodePropList ObjectClassPropName { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClassPropName] ); } }
        public CswNbtNodePropText PropName { get { return ( _CswNbtNode.Properties[PropertyName.PropName] ); } }
        public CswNbtNodePropLogical ReadOnly { get { return ( _CswNbtNode.Properties[PropertyName.ReadOnly] ); } }
        public CswNbtNodePropLogical Required { get { return ( _CswNbtNode.Properties[PropertyName.Required] ); } }
        public CswNbtNodePropLogical Unique { get { return ( _CswNbtNode.Properties[PropertyName.Unique] ); } }
        public CswNbtNodePropLogical UseNumbering { get { return ( _CswNbtNode.Properties[PropertyName.UseNumbering] ); } }

        public CswEnumNbtFieldType FieldTypeValue
        {
            get
            {
                CswEnumNbtFieldType ret = CswNbtResources.UnknownEnum;
                Int32 FieldTypeId = CswConvert.ToInt32( FieldType.Value );
                if( Int32.MinValue != FieldTypeId )
                {
                    ret = _CswNbtResources.MetaData.getFieldTypeValue( FieldTypeId );
                }
                return ret;
            }
        }

        #endregion

        /// <summary>
        /// Options for DisplayConditionFilter
        /// </summary>
        private void _setDisplayConditionOptions()
        {
            Collection<CswNbtNodeTypePropListOption> FilterOptions = new Collection<CswNbtNodeTypePropListOption>();
            Collection<CswNbtNodeTypePropListOption> SubfieldOptions = new Collection<CswNbtNodeTypePropListOption>();

            // Options for DisplayConditionFilter
            if( CswTools.IsPrimaryKey( DisplayConditionProperty.RelatedNodeId ) )
            {
                CswNbtObjClassDesignNodeTypeProp dispCondPropDesignNode = _CswNbtResources.Nodes[DisplayConditionProperty.RelatedNodeId];
                CswPrimaryKey dispCondPropId = dispCondPropDesignNode.RelationalId;
                CswNbtMetaDataNodeTypeProp dispCondProp = _CswNbtResources.MetaData.getNodeTypeProp( dispCondPropId.PrimaryKey );
                ICswNbtFieldTypeRule dispCondRule = dispCondProp.getFieldTypeRule();

                FilterOptions.Add( new CswNbtNodeTypePropListOption( CswEnumNbtFilterMode.Equals.ToString() ) );
                FilterOptions.Add( new CswNbtNodeTypePropListOption( CswEnumNbtFilterMode.NotEquals.ToString() ) );
                if( dispCondProp.getFieldTypeValue() != CswEnumNbtFieldType.Logical )
                {
                    FilterOptions.Add( new CswNbtNodeTypePropListOption( CswEnumNbtFilterMode.Null.ToString() ) );
                    FilterOptions.Add( new CswNbtNodeTypePropListOption( CswEnumNbtFilterMode.NotNull.ToString() ) );
                }

                // Options for DisplayConditionSubfield
                foreach( CswNbtSubField subField in dispCondRule.SubFields )
                {
                    SubfieldOptions.Add( new CswNbtNodeTypePropListOption( subField.Name.ToString() ) );
                }

            } // if( CswTools.IsPrimaryKey( DisplayConditionProperty.RelatedNodeId ) )

            DisplayConditionFilter.Options.Override( FilterOptions );
            DisplayConditionSubfield.Options.Override( FilterOptions );
        } // _setDisplayConditionOptions()

        public Dictionary<string, string> _initCompliantAnswerOptions()
        {
            Dictionary<string, string> ret = new Dictionary<string, string>();
            CswNbtMetaDataNodeTypeProp PossibleAnswersNTP = this.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.PossibleAnswers.ToString() );
            if( null != PossibleAnswersNTP )
            {
                CswNbtNodePropWrapper PossibleAnswersProp = Node.Properties[PossibleAnswersNTP];
                if( null != PossibleAnswersProp )
                {
                    CswCommaDelimitedString PossibleAnswers = new CswCommaDelimitedString();
                    PossibleAnswers.FromString( PossibleAnswersProp.AsText.Text );
                    foreach( string Answer in PossibleAnswers )
                    {
                        ret.Add( Answer, Answer );
                    }
                }
            }
            return ret;
        } // _initCompliantAnswerOptions()

        public bool DerivesFromObjectClassProp
        {
            get { return false == string.IsNullOrEmpty( ObjectClassPropName.Value ); }
        }

        public CswNbtMetaDataObjectClassProp ObjectClassPropValue
        {
            get
            {
                CswNbtMetaDataObjectClassProp ret = null;
                Int32 OcpId = CswConvert.ToInt32( ObjectClassPropName.Value );
                if( Int32.MinValue != OcpId )
                {
                    ret = _CswNbtResources.MetaData.getObjectClassProp( OcpId );
                }
                return ret;
            }
        }

    }//CswNbtObjClassDesignNodeTypeProp

}//namespace ChemSW.Nbt.ObjClasses
