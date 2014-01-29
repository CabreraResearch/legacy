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
            public const string AuditLevel = CswEnumNbtPropertyAttributeName.AuditLevel;
            public const string CompoundUnique = CswEnumNbtPropertyAttributeName.CompoundUnique; //"Compound Unique";
            public const string DisplayConditionFilterMode = CswEnumNbtPropertyAttributeName.DisplayConditionFilterMode; //"Display Condition Filter Mode";
            public const string DisplayConditionProperty = CswEnumNbtPropertyAttributeName.DisplayConditionProperty; //"Display Condition Property";
            public const string DisplayConditionSubfield = CswEnumNbtPropertyAttributeName.DisplayConditionSubfield; //"Display Condition Subfield";
            public const string DisplayConditionValue = CswEnumNbtPropertyAttributeName.DisplayConditionValue; // "Display Condition Value";
            public const string FieldType = CswEnumNbtPropertyAttributeName.FieldType; // "Field Type";
            public const string HelpText = CswEnumNbtPropertyAttributeName.HelpText; // "Help Text";
            public const string NodeTypeValue = CswEnumNbtPropertyAttributeName.NodeTypeValue; // "NodeType";
            public const string ObjectClassPropName = CswEnumNbtPropertyAttributeName.ObjectClassPropName; //"Original Name";
            public const string PropName = CswEnumNbtPropertyAttributeName.PropName; //"Prop Name";
            public const string ReadOnly = CswEnumNbtPropertyAttributeName.ReadOnly; //"Read Only";
            public const string Required = CswEnumNbtPropertyAttributeName.Required; //"Required";
            public const string ServerManaged = CswEnumNbtPropertyAttributeName.ServerManaged; // "Server Managed";
            public const string Unique = CswEnumNbtPropertyAttributeName.Unique; // "Unique";
            public const string UseNumbering = CswEnumNbtPropertyAttributeName.UseNumbering; // "Use Numbering";
            public const string QuestionNo = CswEnumNbtPropertyAttributeName.QuestionNo;
            public const string SubQuestionNo = CswEnumNbtPropertyAttributeName.SubQuestionNo;
        }

        public static string PropRefValue_OCP_Suffix = " (by Object Class)";

        //private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignNodeTypeProp( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            //_CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
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

        public static string getNodeTypeName( CswEnumNbtFieldType FieldType )
        {
            return "Design " + FieldType.ToString() + " NodeTypeProp";
        }


        #region Inherited Events

        protected override void beforePromoteNodeLogic( bool OverrideUniqueValidation = false )
        {
            // Make sure propname is unique for this nodetype
            if( false == CswTools.IsPrimaryKey( NodeTypeValue.RelatedNodeId ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning,
                                           "Property must be attached to a nodetype",
                                           "Attempted to save a new property without a nodetype" );
            }
            if( false == OverrideUniqueValidation &&
                null != RelationalNodeType && null != RelationalNodeType.getNodeTypeProp( PropName.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning,
                                           "Property Name must be unique per nodetype",
                                           "Attempted to save a propname which is equal to a propname of another property in this nodetype" );
            }
        } // beforePromoteNode()

        protected override void afterPromoteNodeLogic()
        {
            // ------------------------------------------------------------
            // This logic from makeNewNodeType and makeNewProp in CswNbtMetaData.cs
            // ------------------------------------------------------------
            if( CswTools.IsPrimaryKey( RelationalId ) )
            {
                Int32 PropId = RelationalId.PrimaryKey;

                //CswTableUpdate PropsUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeTypeProp_afterCreateNode_PropsUpdate", "nodetype_props" );
                //DataTable PropsTable = PropsUpdate.getTable( "nodetypepropid", PropId );
                //if( PropsTable.Rows.Count > 0 )
                //{
                DataRow InsertedRow = RelationalNodeTypeProp._DataRow;
                if( null != InsertedRow )
                {
                    //DataRow InsertedRow = PropsTable.Rows[0];
                    InsertedRow["firstpropversionid"] = PropId;
                    if( null != RelationalNodeType )
                    {
                        InsertedRow["nodetypeid"] = RelationalNodeType.NodeTypeId;
                    }
                    if( DerivesFromObjectClassProp )
                    {
                        InsertedRow["objectclasspropid"] = CswConvert.ToInt32( ObjectClassPropName.Value );
                    }

                    //NOTE: this does not keep the oraviewcolname up to date with updates after original post
                    if( UseNumbering.Checked != CswEnumTristate.True )
                    {
                        string OraViewColName = PropName.Text;
                        if( null == ObjectClass ) //Case 31160 - all NTPs with no ObjClass get a special prefix
                        {
                            OraViewColName = "cu_" + OraViewColName;
                        }
                        InsertedRow["oraviewcolname"] = CswFormat.MakeOracleCompliantIdentifier( OraViewColName );

                    }

                    // Copy values from ObjectClassProp
                    _syncFromObjectClassProp( InsertedRow );
                    postChanges( false, SkipEvents: true );

                    ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
                    InsertedRow["isquicksearch"] = CswConvert.ToDbVal( RelationalRule.SearchAllowed );

                    //PropsUpdate.update( PropsTable );

                    //_CswNbtResources.MetaData._CswNbtMetaDataResources.RecalculateQuestionNumbers( RelationalNodeTypeProp.getNodeType() );    // this could cause versioning
                    RelationalNodeType.DesignNode.RecalculateQuestionNumbers();

                    //if( OnMakeNewNodeTypeProp != null )
                    //{
                    //    OnMakeNewNodeTypeProp( NewProp );
                    //}

                    //will need to refresh auto-views
                    //_RefreshViewForNodetypeId.Add( NtpModel.NodeTypeId );

                } // if( PropsTable.Rows.Count > 0 )
            } // if( CswTools.IsPrimaryKey( RelationalId ) )

            if( null != RelationalNodeTypeProp )
            {
                // Trigger field type rule
                ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
                RelationalRule.afterCreateNodeTypeProp( RelationalNodeTypeProp );

                // Add default layout entry for new property
                if( CswEnumTristate.True == Required.Checked )
                {
                    _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, RelationalNodeTypeProp.NodeTypeId, RelationalNodeTypeProp, false );
                }
                if( null != RelationalNodeType )
                {
                    _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, RelationalNodeTypeProp.NodeTypeId, RelationalNodeTypeProp, false, RelationalNodeType.getFirstNodeTypeTab().TabId );
                }
            } // if( null != RelationalNodeTypeProp )

            _UpdateEquipmentAssemblyMatchingProperties( CswEnumNbtPropAction.Add );

        } // afterPromoteNode()

        protected override void beforeWriteNodeLogic( bool IsCreating ) // bool IsCopy, bool OverrideUniqueValidation )
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

            ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
            RelationalRule.onBeforeWriteDesignNode( this );
        }//beforeWriteNode()

        protected override void afterWriteNodeLogic()
        {
            ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
            RelationalRule.onSetFk( this );
        }//afterWriteNode()


        // Some ObjectClass specific behavior:  
        // Perhaps this should live in the ObjClass...
        // If adding a property to a nodetype of class equipment or equipmentassembly, 
        // or editing a property on a nodetype of class equipment or equipmentassembly,
        // if there is a matching property of the same propname and fieldtype on the related nodetype or objectclass, 
        // set all equipment nodes pendingupdate = 1 (see BZ 5964)

        private void _UpdateEquipmentAssemblyMatchingProperties( CswEnumNbtPropAction Action )
        {
            if( null != RelationalNodeTypeProp )
            {
                CswNbtObjClassDesignNodeType NodeTypeNode = _CswNbtResources.Nodes[NodeTypeValue.RelatedNodeId];
                if( null != NodeTypeNode && null != NodeTypeNode.ObjectClassPropertyValue )
                {
                    CswEnumNbtObjectClass EditedPropObjectClass = NodeTypeNode.ObjectClassPropertyValue.ObjectClass;
                    if( EditedPropObjectClass == CswEnumNbtObjectClass.EquipmentClass )
                    {
                        if( Action != CswEnumNbtPropAction.Delete )
                        {
                            CswNbtMetaDataNodeType EquipmentNodeType = RelationalNodeTypeProp.getNodeType();
                            if( null != EquipmentNodeType )
                            {
                                CswNbtMetaDataNodeTypeProp RelationshipProp = EquipmentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipment.PropertyName.Assembly );
                                if( RelationshipProp != null )
                                {
                                    // We have to update all these nodes always, not just when there's a prop name 
                                    // that matches, in case we renamed a prop and it no longer matches.

                                    // We do this directly, not using a view, for performance
                                    CswTableUpdate NodesTableUpdate = _CswNbtResources.makeCswTableUpdate( "nodes_pendingupdate_update", "nodes" );
                                    DataTable NodesTable = NodesTableUpdate.getTable( "nodetypeid", EquipmentNodeType.NodeTypeId );
                                    foreach( DataRow NodesRow in NodesTable.Rows )
                                    {
                                        NodesRow["pendingupdate"] = "1";
                                    }
                                    NodesTableUpdate.update( NodesTable );
                                }
                            } // if( null != EquipmentNodeType )
                        } // if( Action != CswEnumNbtPropAction.Delete )
                    }
                    else if( EditedPropObjectClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                    {
                        CswNbtMetaDataNodeType AssemblyNodeType = RelationalNodeTypeProp.getNodeType();
                        if( null != AssemblyNodeType )
                        {
                            CswNbtMetaDataObjectClass EquipmentOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
                            foreach( CswNbtMetaDataNodeType EquipmentNodeType in EquipmentOC.getNodeTypes() )
                            {
                                CswNbtMetaDataNodeTypeProp RelationshipProp = EquipmentNodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipment.PropertyName.Assembly );
                                if( RelationshipProp != null )
                                {
                                    if( RelationshipProp.FkMatchesDeprecated( AssemblyNodeType ) )
                                    {
                                        // There is a matching property on the assembly.  Mark all nodes of this nodetype as pendingupdate
                                        // We have to update all these nodes always, not just when there's a prop name 
                                        // that matches, in case we renamed a prop and it no longer matches.
                                        CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "UpdateEquipmentAssemblyMatchingProperties_nodespendingupdate_update", "nodes" );
                                        DataTable NodesTable = NodesUpdate.getTable( "nodetypeid", EquipmentNodeType.NodeTypeId );
                                        foreach( DataRow NodesRow in NodesTable.Rows )
                                        {
                                            NodesRow["pendingupdate"] = "1";
                                        }
                                        NodesUpdate.update( NodesTable );
                                    }
                                } // if( RelationshipProp != null )
                            } // foreach( CswNbtMetaDataNodeType EquipmentNodeType in EquipmentOC.NodeTypes )
                        } // if( null != AssemblyNodeType )
                    } // else if( EditedPropObjectClass == CswEnumNbtObjectClass.EquipmentAssemblyClass )
                } // if( null != NodeTypeNode && null != NodeTypeNode.ObjectClassPropertyValue )
            } // if( null != RelationalNodeTypeProp )
        } // UpdateEquipmentAssemblyMatchingProperties()


        /// <summary>
        /// True if the delete is a result of deleting the nodetype
        /// </summary>
        public bool InternalDelete = false;

        protected override void beforeDeleteNodeLogic() //bool DeleteAllRequiredRelatedNodes = false )
        {
            if( false == IsTemp )
            {
                if( false == InternalDelete && false == RelationalNodeTypeProp.IsDeletable() )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "Cannot delete property", "Property is not allowed to be deleted: Propname = " + PropName.Text + " ; PropId = " + RelationalNodeTypeProp.PropId + "; NodeId = " + this.NodeId.ToString() );
                }

                // Delete jct_nodes_props records
                {
                    CswTableUpdate JctNodesPropsUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtObjClassDesignNodeTypeProp_beforeDeleteNode_jctUpdate", "jct_nodes_props" );
                    DataTable JctNodesPropsTable = JctNodesPropsUpdate.getTable( "nodetypepropid", RelationalNodeTypeProp.PropId );
                    foreach( DataRow CurrentJctNodesPropsRow in JctNodesPropsTable.Rows )
                    {
                        CurrentJctNodesPropsRow.Delete();
                    }
                    JctNodesPropsUpdate.update( JctNodesPropsTable );
                }

                // Delete nodetype_layout records
                _CswNbtResources.MetaData.NodeTypeLayout.removePropFromAllLayouts( RelationalNodeTypeProp );

                //// Delete Views
                //// This has to come after because nodetype_props has an fk to node_views.
                //CswTableUpdate ViewsUpdate = _CswNbtMetaDataResources.CswNbtResources.makeCswTableUpdate( "DeleteNodeTypeProp_nodeview_update", "node_views" );
                //CswCommaDelimitedString SelectCols = new CswCommaDelimitedString();
                //SelectCols.Add( "nodeviewid" );
                //SelectCols.Add( "viewxml" );
                //DataTable ViewsTable = ViewsUpdate.getTable( SelectCols );
                //foreach( DataRow CurrentRow in ViewsTable.Rows )
                //{
                //    if( CurrentRow.RowState != DataRowState.Deleted )
                //    {
                //        CswNbtView CurrentView = new CswNbtView( _CswNbtMetaDataResources.CswNbtResources );
                //        CurrentView.LoadXml( CurrentRow["viewxml"].ToString() );
                //        CurrentView.ViewId = new CswNbtViewId( CswConvert.ToInt32( CurrentRow["nodeviewid"] ) );

                //        if( CurrentView.ContainsNodeTypeProp( NodeTypeProp ) || CurrentView.ViewId == NodeTypeProp.ViewId )
                //            CurrentView.Delete();
                //    }
                //}
                //ViewsUpdate.update( ViewsTable );

                // BZ 8745
                // Update nodename template
                string NodeTypeTemp = RelationalNodeType.NameTemplateValue;
                NodeTypeTemp = NodeTypeTemp.Replace( " " + CswNbtMetaData.MakeTemplateEntry( RelationalNodeTypeProp.PropName ), "" );
                NodeTypeTemp = NodeTypeTemp.Replace( CswNbtMetaData.MakeTemplateEntry( RelationalNodeTypeProp.PropName ), "" );
                RelationalNodeType.DesignNode.NameTemplateText.Text = NodeTypeTemp;
                RelationalNodeType.DesignNode.postChanges( false );

                if( false == InternalDelete )
                {
                //    _CswNbtResources.MetaData.RecalculateQuestionNumbers( RelationalNodeType );
                    RelationalNodeType.DesignNode.RecalculateQuestionNumbers();
                }

            } // if( false == IsTemp )
        }//beforeDeleteNode()

        protected override void afterDeleteNodeLogic()
        {
            _UpdateEquipmentAssemblyMatchingProperties( CswEnumNbtPropAction.Delete );
        }//afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            PropName.SetOnPropChange( _PropName_OnChange );

            // Prevent renaming "Design" properties
            if( null != RelationalNodeTypeProp &&
                RelationalNodeTypeProp.getNodeType().getObjectClass().ObjectClass == CswEnumNbtObjectClass.DesignNodeTypePropClass )
            {
                PropName.setReadOnly( true, true );
            }

            // Add warning helptext to 'Required' and 'Unique'
            if( Required.Checked == CswEnumTristate.True && ( Unique.Checked == CswEnumTristate.True || CompoundUnique.Checked == CswEnumTristate.True ) )
            {
                Required[CswEnumNbtPropertyAttributeName.HelpText] = "Warning: A Property that is both Unique and Required will prevent Multi-Add and Copy";
                Unique[CswEnumNbtPropertyAttributeName.HelpText] = "Warning: A Property that is both Unique and Required will prevent Multi-Add and Copy";
                CompoundUnique[CswEnumNbtPropertyAttributeName.HelpText] = "Warning: A Property that is both Unique and Required will prevent Multi-Add and Copy";
            }

            // Options for Field Type property
            SortedList<string, CswNbtNodeTypePropListOption> FieldTypeOptions = new SortedList<string, CswNbtNodeTypePropListOption>();
            Dictionary<Int32, CswEnumNbtFieldType> FieldTypeIds = _CswNbtResources.MetaData.getFieldTypeIds();
            foreach( Int32 FieldTypeId in FieldTypeIds.Keys )
            {
                CswEnumNbtFieldType thisFieldtypeName = FieldTypeIds[FieldTypeId];
                FieldTypeOptions.Add( thisFieldtypeName.ToString(), new CswNbtNodeTypePropListOption( thisFieldtypeName.ToString(), FieldTypeId.ToString() ) );
            }
            Collection<CswNbtNodeTypePropListOption> SortedOptions = new Collection<CswNbtNodeTypePropListOption>( FieldTypeOptions.Values.ToList() );
            FieldType.Options.Override( SortedOptions );


            ObjectClassPropName.InitOptions = delegate()
                {
                    // Options for ObjectClassPropName
                    //CswNbtNodeTypePropListOptions Options = new CswNbtNodeTypePropListOptions( _CswNbtResources, ObjectClassPropName.NodeTypeProp );
                    CswNbtNodeTypePropListOptions Options = new CswNbtNodeTypePropListOptions( _CswNbtResources, "", Int32.MinValue, ObjectClassPropName.Required );

                    Int32 selectedOcpId = CswConvert.ToInt32( ObjectClassPropName.Value );
                    if( Int32.MinValue != selectedOcpId )
                    {
                        CswNbtMetaDataObjectClassProp selectedOCP = _CswNbtResources.MetaData.getObjectClassProp( selectedOcpId );
                        Options.Override( new Collection<CswNbtNodeTypePropListOption>()
                            {
                                new CswNbtNodeTypePropListOption( selectedOCP.PropName, selectedOCP.PropId.ToString() )
                            } );
                    }
                    return Options;
                };

            if( FieldTypeValue == CswEnumNbtFieldType.Question )
            {
                // Options for Compliant Answer
                CswNbtMetaDataNodeTypeProp CompliantAnswersNTP = this.NodeType.getNodeTypeProp( CswNbtFieldTypeRuleQuestion.AttributeName.CompliantAnswers.ToString() );
                if( null != CompliantAnswersNTP )
                {
                    CswNbtNodePropWrapper CompliantAnswersProp = Node.Properties[CompliantAnswersNTP];
                    if( null != CompliantAnswersProp )
                    {
                        CompliantAnswersProp.AsMultiList.InitOptions = _initCompliantAnswerOptions;
                    }
                }
                // Options for Preferred Answer
                CswNbtMetaDataNodeTypeProp PreferredAnswerNTP = this.NodeType.getNodeTypeProp( CswNbtFieldTypeRuleQuestion.AttributeName.PreferredAnswer.ToString() );
                if( null != PreferredAnswerNTP )
                {
                    CswNbtNodePropWrapper PreferredAnswerProp = Node.Properties[PreferredAnswerNTP];
                    if( null != PreferredAnswerProp )
                    {
                        PreferredAnswerProp.AsList.InitOptions = _initPreferredAnswerOptions;
                    }
                }
            } // if( FieldTypeValue == CswEnumNbtFieldType.Question )

            // Display conditions
            DisplayConditionProperty.onBeforeRender = delegate( CswNbtNodeProp prop )
                {
                    _setDisplayConditionOptions();
                };
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

            // Relationship - Target
            if( FieldTypeValue == CswEnumNbtFieldType.Relationship )
            {
                CswNbtNodePropWrapper TargetProp = this.AttributeProperty[CswNbtFieldTypeRuleRelationship.AttributeName.Target];
                //this.NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.Target.ToString() );

                // If nodes exist, relationship target is readonly
                // See cases 7176, 7600, 8025
                if( RelationalNodeTypeProp != null && RelationalNodeTypeProp.InUse )
                {
                    TargetProp.setReadOnly( true, true );
                }

                // case 31526 - If derived from Object Class, restrict Target options
                if( DerivesFromObjectClassProp )
                {
                    if( ObjectClassPropValue.FKType == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() )
                    {
                        TargetProp.AsMetaDataList.ConstrainToObjectClass = CswNbtNodePropMetaDataList.ObjectClassPrefix + ObjectClassPropValue.FKValue;
                    }
                    else if( ObjectClassPropValue.FKType == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() )
                    {
                        TargetProp.AsMetaDataList.ConstrainToObjectClass = CswNbtNodePropMetaDataList.PropertySetPrefix + ObjectClassPropValue.FKValue;
                    }
                }
            } // if( FieldTypeValue == CswEnumNbtFieldType.Relationship )

            // PropertyReference - filter to my relationships
            if( FieldTypeValue == CswEnumNbtFieldType.PropertyReference )
            {
                CswNbtNodePropText FKTypeWrapper = AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.FKType].AsText;
                CswNbtNodePropList RelationshipWrapper = AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.Relationship].AsList;
                CswNbtNodePropText RelatedPropTypeWrapper = AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedPropType].AsText;
                CswNbtNodePropList RelatedPropWrapper = AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedProperty].AsList;

                // Choices for Relationship derive from other Relationships on this NodeType
                RelationshipWrapper.InitOptions = delegate
                    {
                        CswNbtNodeTypePropListOptions ret = new CswNbtNodeTypePropListOptions( _CswNbtResources, string.Empty, Int32.MinValue, true );
                        if( null != RelationalNodeType )
                        {
                            // We have to embed whether this is an object class or nodetype in the Text of the list value, 
                            // or else the Value won't sync with nodetype_props correctly
                            foreach( CswNbtMetaDataNodeTypeProp RelationshipNTP in RelationalNodeType.getNodeTypeProps( CswEnumNbtFieldType.Relationship ) )
                            {
                                ret.Options.Add( new CswNbtNodeTypePropListOption( RelationshipNTP.PropName, RelationshipNTP.PropId.ToString() ) );
                            }
                            foreach( CswNbtMetaDataObjectClassProp RelationshipOCP in RelationalNodeType.getObjectClass().getObjectClassProps( CswEnumNbtFieldType.Relationship ) )
                            {
                                ret.Options.Add( new CswNbtNodeTypePropListOption( RelationshipOCP.PropName + PropRefValue_OCP_Suffix, RelationshipOCP.PropId.ToString() ) );
                            }
                        }
                        return ret;
                    };

                // Choices for Related Property derive from all properties on the selected Relationship
                RelatedPropWrapper.InitOptions = delegate
                    {
                        CswNbtNodeTypePropListOptions ret = new CswNbtNodeTypePropListOptions( _CswNbtResources, string.Empty, Int32.MinValue, false );
                        if( false == FKTypeWrapper.Empty && false == RelationshipWrapper.Empty )
                        {
                            if( FKTypeWrapper.Text == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() )
                            {
                                CswNbtMetaDataNodeTypeProp SelectedNTP = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( RelationshipWrapper.Value ) );
                                CswNbtNodePropMetaDataList SelectedNTPTarget = SelectedNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleRelationship.AttributeName.Target].AsMetaDataList;
                                if( SelectedNTPTarget.Type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                                {
                                    CswNbtMetaDataNodeType SelectedTargetNT = (CswNbtMetaDataNodeType) SelectedNTPTarget.MetaDataValue;
                                    foreach( CswNbtMetaDataNodeTypeProp OtherNTP in SelectedTargetNT.getNodeTypeProps() )
                                    {
                                        ret.Options.Add( new CswNbtNodeTypePropListOption( OtherNTP.PropName, OtherNTP.PropId.ToString() ) );
                                    }
                                }
                                if( SelectedNTPTarget.Type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                                {
                                    CswNbtMetaDataObjectClass SelectedTargetOC = (CswNbtMetaDataObjectClass) SelectedNTPTarget.MetaDataValue;
                                    foreach( CswNbtMetaDataObjectClassProp OtherOCP in SelectedTargetOC.getObjectClassProps() )
                                    {
                                        ret.Options.Add( new CswNbtNodeTypePropListOption( OtherOCP.PropName + PropRefValue_OCP_Suffix, OtherOCP.PropId.ToString() ) );
                                    }
                                }
                            }
                            else if( FKTypeWrapper.Text == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                            {
                                CswNbtMetaDataObjectClassProp SelectedOCP = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( RelationshipWrapper.Value ) );
                                CswNbtMetaDataObjectClass SelectedTarget = _CswNbtResources.MetaData.getObjectClass( SelectedOCP.FKValue );
                                foreach( CswNbtMetaDataObjectClassProp OtherOCP in SelectedTarget.getObjectClassProps() )
                                {
                                    ret.Options.Add( new CswNbtNodeTypePropListOption( OtherOCP.PropName + PropRefValue_OCP_Suffix, OtherOCP.PropId.ToString() ) );
                                }
                            }
                        } // if( false == FKTypeWrapper.Empty && false == RelationshipWrapper.Empty )
                        return ret;
                    };

            } // if( FieldTypeValue == CswEnumNbtFieldType.PropertyReference )

            if( FieldTypeValue == CswEnumNbtFieldType.ChildContents )
            {
                CswNbtNodePropText FKTypeWrapper = AttributeProperty[CswNbtFieldTypeRuleChildContents.AttributeName.FKType].AsText;
                CswNbtNodePropList ChildRelationshipWrapper = AttributeProperty[CswNbtFieldTypeRuleChildContents.AttributeName.ChildRelationship].AsList;

                // ChildContents ChildRelationship - filter to relationships that point to my relational nodetype
                ChildRelationshipWrapper.InitOptions = delegate
                    {
                        CswNbtNodeTypePropListOptions ret = new CswNbtNodeTypePropListOptions( _CswNbtResources, string.Empty, Int32.MinValue, true );
                        if( null != RelationalNodeType )
                        {
                            // We have to embed whether this is an object class or nodetype in the Text of the list value, 
                            // or else the Value won't sync with nodetype_props correctly
                            foreach( CswNbtMetaDataNodeTypeProp RelationshipNTP in _CswNbtResources.MetaData.getNodeTypeProps( CswEnumNbtFieldType.Relationship ) )
                            {
                                if( RelationshipNTP.FkMatchesNew( RelationalNodeType ) )
                                {
                                    ret.Options.Add( new CswNbtNodeTypePropListOption( RelationshipNTP.getNodeType().NodeTypeName + ": " + RelationshipNTP.PropName, RelationshipNTP.PropId.ToString() ) );
                                }
                            }
                            foreach( CswNbtMetaDataObjectClassProp RelationshipOCP in _CswNbtResources.MetaData.getObjectClassProps( CswEnumNbtFieldType.Relationship ) )
                            {
                                if( RelationshipOCP.FkMatches( RelationalNodeType ) )
                                {
                                    ret.Options.Add( new CswNbtNodeTypePropListOption( RelationshipOCP.getObjectClass().ObjectClass + ": " + RelationshipOCP.PropName + PropRefValue_OCP_Suffix, RelationshipOCP.PropId.ToString() ) );
                                }
                            }
                        }
                        return ret;
                    };
            } // if( FieldTypeValue == CswEnumNbtFieldType.ChildContents )

            if( FieldTypeValue == CswEnumNbtFieldType.Composite )
            {
                // Options for 'Add To Template' are all properties in this nodetype
                CswNbtView addTemplateView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship PropRel1 = addTemplateView.AddViewRelationship( this.ObjectClass, false );
                addTemplateView.AddViewPropertyAndFilter( PropRel1,
                                                          NodeTypeValue.NodeTypeProp,
                                                          CswEnumNbtFilterConjunction.And,
                                                          FilterMode: CswEnumNbtFilterMode.Equals,
                                                          SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                          Value: this.NodeTypeValue.RelatedNodeId.PrimaryKey.ToString() );
                CswNbtNodePropRelationship AddToTemplateProp = AttributeProperty[CswNbtFieldTypeRuleComposite.AttributeName.AddToTemplate].AsRelationship;
                AddToTemplateProp.OverrideView( addTemplateView );
            }

            // Default value
            CswNbtMetaDataNodeTypeProp DefaultValueNTP = NodeType.getNodeTypeProp( CswEnumNbtPropertyAttributeName.DefaultValue );
            if( null != DefaultValueNTP )
            {
                CswNbtNodePropWrapper defaultValueWrapper = this.Node.Properties[DefaultValueNTP];
                foreach( CswNbtFieldTypeAttribute attribute in defaultValueWrapper.getFieldType().getFieldTypeRule().getAttributes() )
                {
                    if( attribute.Name != CswEnumNbtPropertyAttributeName.DefaultValue ) // god save us
                    {
                        // Override the attribute on the default value with what is defined on this property
                        // Example: list options
                        CswNbtMetaDataNodeTypeProp attrNTP = NodeType.getNodeTypeProp( attribute.Name );
                        if( this.Node.Properties.Contains( attrNTP ) &&
                            attribute.Name != CswEnumNbtPropertyAttributeName.Required ) // never make default values required
                        {
                            defaultValueWrapper[attribute.Name] = this.Node.Properties[attrNTP].Gestalt;
                        }
                    }
                }
            }
            //_CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }


        public override CswNbtNode CopyNode( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            string NewPropName = "Copy Of " + PropName.Text;
            Int32 CopyInt = 1;
            while( null != _CswNbtResources.MetaData.getNodeType( NewPropName ) )
            {
                CopyInt++;
                NewPropName = "Copy " + CopyInt.ToString() + " Of " + PropName.Text;
            }

            return base.CopyNodeImpl( IsNodeTemp, delegate( CswNbtNode NewNode )
                {
                    ( (CswNbtObjClassDesignNodeTypeProp) NewNode ).PropName.Text = NewPropName;
                    if( null != OnCopy )
                    {
                        OnCopy( NewNode );
                    }
                } );
        } // CopyNode()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList AuditLevel { get { return ( _CswNbtNode.Properties[PropertyName.AuditLevel] ); } }
        public CswNbtNodePropLogical CompoundUnique { get { return ( _CswNbtNode.Properties[PropertyName.CompoundUnique] ); } }
        public CswNbtNodePropList DisplayConditionFilterMode { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionFilterMode] ); } }

        public CswNbtNodePropRelationship DisplayConditionProperty { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionProperty] ); } }
        public void _DisplayConditionProperty_Change( CswNbtNodeProp Prop, bool Creating )
        {
            _setDisplayConditionOptions();
            RelationalNodeType.DesignNode.RecalculateQuestionNumbers();
        }

        public CswNbtNodePropList DisplayConditionSubfield { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionSubfield] ); } }
        public CswNbtNodePropText DisplayConditionValue { get { return ( _CswNbtNode.Properties[PropertyName.DisplayConditionValue] ); } }
        public CswNbtNodePropList FieldType { get { return ( _CswNbtNode.Properties[PropertyName.FieldType] ); } }
        public CswNbtNodePropMemo HelpText { get { return ( _CswNbtNode.Properties[PropertyName.HelpText] ); } }
        public CswNbtNodePropRelationship NodeTypeValue { get { return ( _CswNbtNode.Properties[PropertyName.NodeTypeValue] ); } }
        public CswNbtNodePropList ObjectClassPropName { get { return ( _CswNbtNode.Properties[PropertyName.ObjectClassPropName] ); } }

        public CswNbtNodePropText PropName { get { return ( _CswNbtNode.Properties[PropertyName.PropName] ); } }
        public void _PropName_OnChange( CswNbtNodeProp Prop, bool Creating )
        {
            _UpdateEquipmentAssemblyMatchingProperties( CswEnumNbtPropAction.Edit );
        }

        public CswNbtNodePropNumber QuestionNo { get { return ( _CswNbtNode.Properties[PropertyName.QuestionNo] ); } }
        public CswNbtNodePropLogical ReadOnly { get { return ( _CswNbtNode.Properties[PropertyName.ReadOnly] ); } }
        public CswNbtNodePropLogical Required { get { return ( _CswNbtNode.Properties[PropertyName.Required] ); } }
        public CswNbtNodePropLogical ServerManaged { get { return ( _CswNbtNode.Properties[PropertyName.ServerManaged] ); } }
        public CswNbtNodePropNumber SubQuestionNo { get { return ( _CswNbtNode.Properties[PropertyName.SubQuestionNo] ); } }
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
        /// Options for DisplayCondition* properties
        /// </summary>
        private void _setDisplayConditionOptions()
        {
            // Options for DisplayConditionProperty - all properties on the same nodetype
            if( CswTools.IsPrimaryKey( this.NodeTypeValue.RelatedNodeId ) )
            {
                CswNbtView DispCondView = new CswNbtView( _CswNbtResources );
                CswNbtViewRelationship PropRel1 = DispCondView.AddViewRelationship( this.ObjectClass, false );
                DispCondView.AddViewPropertyAndFilter( PropRel1,
                                                       NodeTypeValue.NodeTypeProp,
                                                       CswEnumNbtFilterConjunction.And,
                                                       FilterMode: CswEnumNbtFilterMode.Equals,
                                                       SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                                                       Value: this.NodeTypeValue.RelatedNodeId.PrimaryKey.ToString() );
                DisplayConditionProperty.OverrideView( DispCondView );


                Collection<CswNbtNodeTypePropListOption> FilterOptions = new Collection<CswNbtNodeTypePropListOption>();
                Collection<CswNbtNodeTypePropListOption> SubfieldOptions = new Collection<CswNbtNodeTypePropListOption>();

                if( CswTools.IsPrimaryKey( DisplayConditionProperty.RelatedNodeId ) )
                {
                    CswNbtObjClassDesignNodeTypeProp dispCondPropDesignNode = _CswNbtResources.Nodes[DisplayConditionProperty.RelatedNodeId];
                    if( null != dispCondPropDesignNode )
                    {
                        CswPrimaryKey dispCondPropId = dispCondPropDesignNode.RelationalId;
                        CswNbtMetaDataNodeTypeProp dispCondProp = _CswNbtResources.MetaData.getNodeTypeProp( dispCondPropId.PrimaryKey );
                        ICswNbtFieldTypeRule dispCondRule = dispCondProp.getFieldTypeRule();

                        // Options for DisplayConditionFilterMode
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
                    }

                } // if( CswTools.IsPrimaryKey( DisplayConditionProperty.RelatedNodeId ) )

                DisplayConditionFilterMode.Options.Override( FilterOptions );
                DisplayConditionSubfield.Options.Override( SubfieldOptions );
            }
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

        public CswNbtNodeTypePropListOptions _initPreferredAnswerOptions()
        {
            CswNbtNodeTypePropListOptions ret = new CswNbtNodeTypePropListOptions( _CswNbtResources, string.Empty, Int32.MinValue, false );
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
                        ret.Options.Add( new CswNbtNodeTypePropListOption( Answer, Answer ) );
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

        public Dictionary<CswEnumNbtPropertyAttributeName, CswNbtNodePropWrapper> _AttributeProperty = new Dictionary<CswEnumNbtPropertyAttributeName, CswNbtNodePropWrapper>();
        public Dictionary<CswEnumNbtPropertyAttributeName, CswNbtNodePropWrapper> AttributeProperty
        {
            get
            {
                if( null == _AttributeProperty || _AttributeProperty.Count == 0 )
                {
                    // Populate collection of AttributeProperties
                    _AttributeProperty = new Dictionary<CswEnumNbtPropertyAttributeName, CswNbtNodePropWrapper>();
                    if( CswNbtResources.UnknownEnum != FieldTypeValue )
                    {
                        ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
                        foreach( CswNbtFieldTypeAttribute a in RelationalRule.getAttributes() )
                        {
                            CswNbtMetaDataNodeTypeProp AttributeProp = this.NodeType.getNodeTypeProp( a.Name );
                            if( null != AttributeProp )
                            {
                                if( false == _AttributeProperty.ContainsKey( a.Name ) )
                                {
                                    _AttributeProperty.Add( a.Name, this.Node.Properties[AttributeProp] );
                                }
                            }
                        }
                    }
                }
                return _AttributeProperty;
            }
        }


        public string getAttributeValueByName( CswEnumNbtPropertyAttributeName attributeName )
        {
            ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
            CswNbtFieldTypeAttribute Attr = RelationalRule.getAttributes().FirstOrDefault( a => a.Name == attributeName );
            return CswConvert.ToString( AttributeProperty[Attr.Name].GetSubFieldValue( Attr.SubFieldName ) );
        }

        public string getAttributeValueByColumn( CswEnumNbtPropertyAttributeColumn attributeColumn )
        {
            ICswNbtFieldTypeRule RelationalRule = _CswNbtResources.MetaData.getFieldTypeRule( FieldTypeValue );
            CswNbtFieldTypeAttribute Attr = RelationalRule.getAttributes().FirstOrDefault( a => a.Column == attributeColumn );
            return CswConvert.ToString( AttributeProperty[Attr.Name].GetSubFieldValue( Attr.SubFieldName ) );
        }


        /// <summary>
        /// Synchronize attributes from object class prop
        /// </summary>
        public void syncFromObjectClassProp()
        {
            Int32 PropId = RelationalId.PrimaryKey;
            CswTableUpdate PropsUpdate = _CswNbtResources.makeCswTableUpdate( "DesignNodeTypeProp_afterCreateNode_PropsUpdate", "nodetype_props" );
            DataTable PropsTable = PropsUpdate.getTable( "nodetypepropid", PropId );
            if( PropsTable.Rows.Count > 0 )
            {
                _syncFromObjectClassProp( PropsTable.Rows[0] );
                PropsUpdate.update( PropsTable );
            }
        }


        private void _syncFromObjectClassProp( DataRow PropRow )
        {
            // Copy values from ObjectClassProp
            if( DerivesFromObjectClassProp )
            {
                CswNbtMetaDataObjectClassProp OCProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( ObjectClassPropName.Value ) );
                if( null != OCProp )
                {
                    // Copy all attributes from the Object Class Prop
                    _CswNbtResources.MetaData.CopyNodeTypePropFromObjectClassProp( OCProp, PropRow );

                    // Override propname if provided
                    if( false == string.IsNullOrEmpty( PropName.Text ) )
                    {
                        PropRow["propname"] = PropName.Text;
                    }

                    // Sync properties with what was just copied
                    CswTableSelect mapSelect = _CswNbtResources.makeCswTableSelect( "CswNbtObjClassDesignNodeTypeProp_afterCreateNode_jctSelect", "jct_dd_ntp" );
                    DataTable mapTable = mapSelect.getTable( "where nodetypepropid in (select nodetypepropid from nodetype_props where nodetypeid = " + NodeType.NodeTypeId.ToString() + ")" );

                    // Special case: Because interpretation of fkvalue requires fktype, we have to do that one first
                    foreach( DataRow mapRow in mapTable.Rows )
                    {
                        _CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( mapRow["datadictionaryid"] ) );
                        if( _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Fktype )
                        {
                            _applyValToProperty( mapRow, PropRow );
                        }
                    } // foreach( DataRow mapRow in mapTable.Rows )

                    // Now do the rest
                    foreach( DataRow mapRow in mapTable.Rows )
                    {
                        _CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( mapRow["datadictionaryid"] ) );
                        if( _CswNbtResources.DataDictionary.ColumnName != CswEnumNbtPropertyAttributeColumn.Fktype )
                        {
                            _applyValToProperty( mapRow, PropRow );
                        }
                    } // foreach( DataRow mapRow in mapTable.Rows )


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
                                //Node.Properties[ntp].SetPropRowValue( ntp.getFieldTypeRule().SubFields[attr.SubFieldName ?? ntp.getFieldTypeRule().SubFields.Default.Name].Column, value );
                                Node.Properties[ntp].SetSubFieldValue( attr.SubFieldName ?? ntp.getFieldTypeRule().SubFields.Default.Name, value );
                            }
                        } // foreach( CswNbtFieldTypeAttribute attr in Attributes )
                    } // if( null != ObjectClassPropValue && ObjectClassPropValue.FKValue != Int32.MinValue )


                    // Layout
                    if( null != RelationalNodeType )
                    {
                        CswNbtMetaDataNodeTypeTab FirstTab = RelationalNodeType.getFirstNodeTypeTab();
                        if( OCProp.PropName.Equals( CswNbtObjClass.PropertyName.Save ) ) //case 29181 - Save prop on Add/Edit layouts at the bottom of tab
                        {
                            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, Int32.MaxValue, 1 );
                            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, Int32.MaxValue, 1 );
                        }
                        else
                        {
                            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, Int32.MinValue, 1 );
                            if( OCProp.getFieldType().IsLayoutCompatible( CswEnumNbtLayoutType.Add ) &&
                                ( ( OCProp.IsRequired && false == OCProp.HasDefaultValue() ) ||
                                  ( OCProp.SetValueOnAdd ||
                                    ( Int32.MinValue != OCProp.DisplayColAdd &&
                                      Int32.MinValue != OCProp.DisplayRowAdd ) ) ) )
                            {
                                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, RelationalNodeType.NodeTypeId, RelationalNodeTypeProp, true, FirstTab.TabId, OCProp.DisplayRowAdd, OCProp.DisplayColAdd );
                            }
                        }
                    } // if( null != RelationalNodeType )

                    // Handle default values from ObjectClassProp
                    _CswNbtResources.MetaData.CopyNodeTypePropDefaultValueFromObjectClassProp( OCProp, RelationalNodeTypeProp );
                } // if( null != OCProp )
            } // if( DerivesFromObjectClassProp )
        } // syncFromObjectClass()


        private void _applyValToProperty( DataRow mapRow, DataRow PropRow )
        {
            _CswNbtResources.DataDictionary.setCurrentColumn( CswConvert.ToInt32( mapRow["datadictionaryid"] ) );
            if( false == string.IsNullOrEmpty( PropRow[_CswNbtResources.DataDictionary.ColumnName].ToString() ) )
            {
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( CswConvert.ToInt32( mapRow["nodetypepropid"] ) );
                if( _CswNbtResources.DataDictionary.ColumnName != "nodetypeid" )
                {
                    CswEnumNbtSubFieldName SubFieldName = (CswEnumNbtSubFieldName) mapRow["subfieldname"].ToString();
                    if( SubFieldName.ToString() == CswNbtResources.UnknownEnum &&
                        null != ntp.getFieldTypeRule().SubFields.Default )
                    {
                        SubFieldName = ntp.getFieldTypeRule().SubFields.Default.Name;
                    }
                    if( SubFieldName.ToString() != CswNbtResources.UnknownEnum )
                    {
                        // Special case: sequenceid needs to be decoded from sequences_# to nodes_# (case 31748)
                        if( _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Sequenceid )
                        {
                            CswPrimaryKey SequenceId = new CswPrimaryKey( "sequences", CswConvert.ToInt32( PropRow[_CswNbtResources.DataDictionary.ColumnName] ) );
                            if( CswTools.IsPrimaryKey( SequenceId ) )
                            {
                                CswNbtObjClassDesignSequence SequenceNode = _CswNbtResources.Nodes.getNodeByRelationalId( SequenceId );
                                if( null != SequenceNode )
                                {
                                    Node.Properties[ntp].SetSubFieldValue( SubFieldName, SequenceNode.NodeId );
                                }
                            }
                        }
                        else
                        {
                            Node.Properties[ntp].SetSubFieldValue( SubFieldName, PropRow[_CswNbtResources.DataDictionary.ColumnName] );

                            // Object class property attributes shouldn't be modifiable on the NodeType
                            if( _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Fktype ||
                                _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Fkvalue ||
                                _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Listoptions ||
                                _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Valueoptions ||
                                _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Multi ||
                                ( CswConvert.ToBoolean( PropRow[_CswNbtResources.DataDictionary.ColumnName] ) &&
                                  ( _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Isrequired ||
                                    _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Isunique ||
                                    _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Iscompoundunique ||
                                    _CswNbtResources.DataDictionary.ColumnName == CswEnumNbtPropertyAttributeColumn.Readonly ) ) )
                            {
                                Node.Properties[ntp].setReadOnly( true, true );
                            }
                        }
                    }
                }
            }
        } // _applyValToProperty()


    }//CswNbtObjClassDesignNodeTypeProp

}//namespace ChemSW.Nbt.ObjClasses
