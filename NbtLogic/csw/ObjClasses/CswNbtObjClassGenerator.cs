using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGenerator : CswNbtObjClass, ICswNbtPropertySetScheduler
    {
        public static string InspectionGeneratorNodeTypeName { get { return "Inspection Schedule"; } }

        public static string DueDateIntervalPropertyName { get { return "Due Date Interval"; } }
        public static string RunTimePropertyName { get { return "Run Time"; } }
        public static string FinalDueDatePropertyName { get { return "Final Due Date"; } }
        public static string NextDueDatePropertyName { get { return "Next Due Date"; } }
        public static string WarningDaysPropertyName { get { return "Warning Days"; } }
        //public static string GraceDaysPropertyName { get { return "Grace Days"; } }
        public static string EnabledPropertyName { get { return "Enabled"; } }
        public static string RunStatusPropertyName { get { return "Run Status"; } }
        public static string TargetTypePropertyName { get { return "Target Type"; } }
        public static string OwnerPropertyName { get { return "Owner"; } }
        public static string DescriptionPropertyName { get { return "Description"; } }
        public static string SummaryPropertyName { get { return "Summary"; } }
        public static string ParentTypePropertyName { get { return "Parent Type"; } }
        public static string ParentViewPropertyName { get { return "Parent View"; } }
        public static string RunNowPropertyName { get { return "Run Now"; } }

        //ICswNbtPropertySetScheduler
        public string SchedulerFinalDueDatePropertyName { get { return FinalDueDatePropertyName; } }
        public string SchedulerNextDueDatePropertyName { get { return NextDueDatePropertyName; } }
        public string SchedulerRunStatusPropertyName { get { return RunStatusPropertyName; } }
        public string SchedulerWarningDaysPropertyName { get { return WarningDaysPropertyName; } }
        public string SchedulerDueDateIntervalPropertyName { get { return DueDateIntervalPropertyName; } }
        public string SchedulerRunTimePropertyName { get { return RunTimePropertyName; } }
        public string SchedulerRunNowPropertyName { get { return RunNowPropertyName; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtPropertySetSchedulerImpl _CswNbtPropertySetSchedulerImpl;

        public CswNbtObjClassGenerator( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGenerator
        /// </summary>
        public static implicit operator CswNbtObjClassGenerator( CswNbtNode Node )
        {
            CswNbtObjClassGenerator ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass ) )
            {
                ret = (CswNbtObjClassGenerator) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );

            //// case 24309
            //CswNbtMetaDataNodeType ThisGeneratorNT = _CswNbtResources.MetaData.getNodeType( this.NodeTypeId );
            //CswNbtMetaDataNodeTypeProp OwnerNTP = ThisGeneratorNT.getNodeTypePropByObjectClassProp( OwnerPropertyName );
            //if( ParentType.SelectedNodeTypeIds.Count > 0 )
            //{
            //    CswNbtMetaDataNodeType ParentNT = _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( ParentType.SelectedNodeTypeIds[0] ) );
            //    // Only need a view if parent is defined and parent is different than owner
            //    if( ParentNT != null && ( ( OwnerNTP.FKType == RelatedIdType.NodeTypeId.ToString() && OwnerNTP.FKValue != ParentNT.NodeTypeId ) ||
            //                              ( OwnerNTP.FKType == RelatedIdType.ObjectClassId.ToString() && OwnerNTP.FKValue != ParentNT.ObjectClass.ObjectClassId ) ) )
            //    {
            //        // Default view:
            //        //    This Generator nodetype
            //        //      Owner nodetype (by Generator's Owner)
            //        //        Parent nodetype (by whatever property connects Owner and Parent)

            //        CswNbtView PView = _CswNbtResources.ViewSelect.restoreView( ParentView.ViewId );
            //        if( PView.Root == null || PView.Root.ChildRelationships.Count == 0 )
            //        {
            //            // Discover what relates the Owner and the Parent
            //            CswNbtMetaDataNodeTypeProp ParentRelationNTP = null;
            //            foreach( CswNbtMetaDataNodeTypeProp RelationshipNTP in ParentNT.NodeTypeProps )
            //            {
            //                if( RelationshipNTP.FieldType.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Relationship &&
            //                    RelationshipNTP.FKType == OwnerNTP.FKType &&
            //                    RelationshipNTP.FKValue == OwnerNTP.FKValue )
            //                {
            //                    ParentRelationNTP = RelationshipNTP;
            //                    break;
            //                }
            //            }

            //            if( ParentRelationNTP != null )
            //            {
            //                CswNbtViewRelationship GeneratorViewRel = PView.AddViewRelationship( ThisGeneratorNT, true );
            //                CswNbtViewRelationship OwnerViewRel = PView.AddViewRelationship( GeneratorViewRel, PropOwnerType.First, OwnerNTP, true );
            //                CswNbtViewRelationship ParentViewRel = PView.AddViewRelationship( OwnerViewRel, PropOwnerType.Second, ParentRelationNTP, true );
            //                PView.save();
            //            }

            //        } // if( PView.Root == null || PView.Root.ChildRelationships.Count == 0 )
            //    } // if( OwnerNTP.NodeType != ParentNT )
            //} // if( ParentType.SelectedNodeTypeIds.Count > 0 )

            //Case 24572
            bool DeleteFutureNodes = ( TargetType.WasModified || ParentType.WasModified );
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate( DeleteFutureNodes );


            _trySetNodeTypeSelectDefaultValues();

            // BZ 7845
            if( TargetType.Empty )
            {
                Enabled.Checked = Tristate.False;
            }
        } //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtPropertySetSchedulerImpl.setLastFutureDate();
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        private void _trySetNodeTypeSelectDefaultValues()
        {
            bool RequiresParentView = Owner.RelatedNodeId != null &&
                          ( Node.getNodeType().getFirstVersionNodeType().NodeTypeName == InspectionGeneratorNodeTypeName ||
                            ( ParentView.ViewId != null &&
                              ParentView.ViewId.isSet() ) );

            if( RequiresParentView )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                Collection<CswNbtMetaDataNodeType> MatchingInspectionTargetNts = new Collection<CswNbtMetaDataNodeType>();

                bool SetDefaultParentType = ( ( false == ParentType.WasModified ||
                                                ParentType.SelectedNodeTypeIds.Count == 0 ) &&
                                                null != OwnerNode &&
                                                OwnerNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass &&
                                                ParentType.SelectMode != PropertySelectMode.Blank );
                if( SetDefaultParentType )
                {
                    CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                    foreach( CswNbtMetaDataNodeType InspectionTargetNt in InspectionTargetOc.getNodeTypes() )
                    {
                        if( InspectionTargetNt.IsLatestVersion() )
                        {
                            CswNbtMetaDataNodeTypeProp TargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.InspectionTargetGroupPropertyName );
                            if( TargetGroupNtp.IsFK &&
                                NbtViewRelatedIdType.NodeTypeId.ToString() == TargetGroupNtp.FKType &&
                                Int32.MinValue != TargetGroupNtp.FKValue )
                            {
                                CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( TargetGroupNtp.FKValue ).getNodeTypeLatestVersion();
                                if( null != InspectionTargetGroupNt &&
                                    false == MatchingInspectionTargetNts.Contains( InspectionTargetNt ) &&
                                    InspectionTargetGroupNt == OwnerNode.getNodeTypeLatestVersion() )
                                {
                                    MatchingInspectionTargetNts.Add( InspectionTargetNt );
                                    ParentType.SelectedNodeTypeIds.Add( InspectionTargetNt.NodeTypeId.ToString(), false, true );
                                    if( ParentType.SelectMode == PropertySelectMode.Single )
                                    {
                                        break;
                                    }
                                }
                            } // is valid FK
                        } // if( InspectionTargetNt.IsLatestVersion )
                    } // foreach( CswNbtMetaDataNodeType InspectionTargetNt in InspectionTargetOc.NodeTypes )
                } // if( SetDefaultTargetType )

                bool SetDefaultTargetType = ( ( false == TargetType.WasModified ||
                                            TargetType.SelectedNodeTypeIds.Count == 0 ) &&
                                          TargetType.SelectMode != PropertySelectMode.Blank &&
                                          ( MatchingInspectionTargetNts.Count > 0 ||
                                            TargetType.SelectedNodeTypeIds.Count > 0 ) );
                if( SetDefaultTargetType )
                {
                    if( MatchingInspectionTargetNts.Count == 0 )
                    {
                        foreach( Int32 InspectionTargetNodeTypeId in TargetType.SelectedNodeTypeIds.ToIntCollection() )
                        {
                            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetNodeTypeId );
                            if( null != InspectionTargetNt )
                            {
                                CswNbtMetaDataNodeType LatestInspectionTargetNt = InspectionTargetNt.getNodeTypeLatestVersion();
                                if( LatestInspectionTargetNt.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass &&
                                    false == MatchingInspectionTargetNts.Contains( LatestInspectionTargetNt ) )
                                {
                                    MatchingInspectionTargetNts.Add( LatestInspectionTargetNt );
                                }
                            }
                        }
                    }
                    if( MatchingInspectionTargetNts.Count > 0 )
                    {
                        CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                        foreach( CswNbtMetaDataNodeType InspectionDesignNt in InspectionDesignOc.getNodeTypes() )
                        {
                            if( InspectionDesignNt.IsLatestVersion() )
                            {
                                CswNbtMetaDataNodeTypeProp DesignTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
                                foreach( CswNbtMetaDataNodeType MatchingInspectionTargetNt in MatchingInspectionTargetNts )
                                {
                                    if( DesignTargetNtp.IsFK &&
                                        NbtViewRelatedIdType.NodeTypeId.ToString() == DesignTargetNtp.FKType &&
                                        Int32.MinValue != DesignTargetNtp.FKValue )
                                    {
                                        if( MatchingInspectionTargetNt.NodeTypeId == DesignTargetNtp.FKValue )
                                        {
                                            TargetType.SelectedNodeTypeIds.Add( InspectionDesignNt.NodeTypeId.ToString(), false, true );
                                            if( TargetType.SelectMode == PropertySelectMode.Single )
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void _deleteFutureNodes()
        {
            // BZ 6754 - Delete all future nodes
            CswNbtMetaDataObjectClass GeneratorObjectClass = ObjectClass;

            if( TargetType.SelectedNodeTypeIds.Count == 1 )
            {
                Int32 SelectedTargetNtId = CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] );
                if( Int32.MinValue != SelectedTargetNtId )
                {
                    CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( SelectedTargetNtId );
                    if( null != TargetNodeType )
                    {
                        CswNbtMetaDataObjectClass TargetObjectClass = TargetNodeType.getObjectClass();

                        CswNbtObjClass TargetObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, TargetObjectClass );
                        if( !( TargetObjClass is ICswNbtPropertySetGeneratorTarget ) )
                        {
                            throw new CswDniException( "CswNbtObjClassGenerator.beforeDeleteNode() got an invalid object class: " + TargetObjectClass.ObjectClass.ToString() );
                        }
                        ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget) TargetObjClass;

                        CswNbtMetaDataNodeTypeProp GeneratorProp = TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetGeneratorPropertyName );
                        CswNbtMetaDataNodeTypeProp IsFutureProp = TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetIsFuturePropertyName );

                        CswNbtView View = new CswNbtView( _CswNbtResources );
                        View.ViewName = "CswNbtObjClassSchedule.beforeDeleteNode()";
                        CswNbtViewRelationship GeneratorRelationship = View.AddViewRelationship( GeneratorObjectClass, false );
                        GeneratorRelationship.NodeIdsToFilterIn.Add( _CswNbtNode.NodeId );
                        CswNbtViewRelationship TargetRelationship = View.AddViewRelationship( GeneratorRelationship, NbtViewPropOwnerType.Second, GeneratorProp, false );
                        CswNbtViewProperty IsFutureProperty = View.AddViewProperty( TargetRelationship, IsFutureProp );
                        View.AddViewPropertyFilter( IsFutureProperty, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "True", false );

                        ICswNbtTree TargetTree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );

                        TargetTree.goToRoot();
                        if( TargetTree.getChildNodeCount() > 0 ) // should always be the case
                        {
                            TargetTree.goToNthChild( 0 );
                            if( TargetTree.getChildNodeCount() > 0 ) // might not always be the case
                            {
                                for( int i = 0; i < TargetTree.getChildNodeCount(); i += 1 )
                                {
                                    TargetTree.goToNthChild( i );

                                    CswNbtNode TargetNode = TargetTree.getNodeForCurrentPosition();
                                    TargetNode.delete();

                                    TargetTree.goToParentNode();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _deleteFutureNodes();
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        } //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            TargetType.SetOnPropChange( OnTargetTypePropChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {




            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                if( RunNowPropertyName == OCP.PropName )
                {
                    NextDueDate.DateTimeValue = DateTime.Now;
                    //case 25702 - empty comment?
                    //RunStatus.StaticText = string.Empty;
                    //RunStatus_new.a
                    Node.postChanges( false );
                    ButtonData.Action = NbtButtonAction.refresh;
                }
            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropDateTime FinalDueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[FinalDueDatePropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropDateTime NextDueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[NextDueDatePropertyName].AsDateTime );
            }
        }

        //public CswNbtNodePropDate InitialDueDate
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[InitialDueDatePropertyName].AsDate );
        //    }
        //}

        //public CswNbtNodePropList ActionName
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[ActionNamePropertyName].AsList );
        //    }
        //}

        /// <summary>
        /// Node type of target, where target is the node generated by schedule
        /// </summary>
        public CswNbtNodePropNodeTypeSelect TargetType
        {
            get
            {
                return ( _CswNbtNode.Properties[TargetTypePropertyName].AsNodeTypeSelect );
            }
        }
        private void OnTargetTypePropChange( CswNbtNodeProp NodeProp )
        {
            if( TargetType.Empty )
            {
                Enabled.Checked = Tristate.False;
            }
        }
        public CswNbtNodePropMemo Description
        {
            get
            {
                return ( _CswNbtNode.Properties[DescriptionPropertyName].AsMemo );
            }
        }

        /// <summary>
        /// In IMCS, owner == Equipment or Assembly node, in FE owner == Location Group node
        /// </summary>
        public CswNbtNodePropRelationship Owner
        {
            get
            {
                return ( _CswNbtNode.Properties[OwnerPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropComments RunStatus
        {
            get
            {
                return ( _CswNbtNode.Properties[RunStatusPropertyName].AsComments );
            }
        }

        /// <summary>
        /// Days before due date to anticipate upcoming event
        /// </summary>
        public CswNbtNodePropNumber WarningDays
        {
            get
            {
                return ( _CswNbtNode.Properties[WarningDaysPropertyName].AsNumber );
            }
        }

        ///// <summary>
        ///// Days after due date to continue to allow edits
        ///// </summary>
        //public CswNbtNodePropNumber GraceDays
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[GraceDaysPropertyName].AsNumber );
        //    }
        //}

        public CswNbtNodePropText Summary
        {
            get
            {
                return ( _CswNbtNode.Properties[SummaryPropertyName].AsText );
            }
        }
        public CswNbtNodePropTimeInterval DueDateInterval
        {
            get
            {
                return ( _CswNbtNode.Properties[DueDateIntervalPropertyName].AsTimeInterval );
            }
        }

        //public CswNbtNodePropRelationship MailReport
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[MailReportPropertyName].AsRelationship );
        //    }
        //}

        public CswNbtNodePropDateTime RunTime
        {
            get
            {
                return ( _CswNbtNode.Properties[RunTimePropertyName].AsDateTime );
            }
        }

        public CswNbtNodePropLogical Enabled
        {
            get
            {
                return ( _CswNbtNode.Properties[EnabledPropertyName].AsLogical );
            }
        }

        /// <summary>
        /// Node type of parent. In FE parent is node type of Fire Extinguisher or Inspection Target. In IMCS, parent type is not used.
        /// </summary>
        public CswNbtNodePropNodeTypeSelect ParentType
        {
            get
            {
                return ( _CswNbtNode.Properties[ParentTypePropertyName].AsNodeTypeSelect );
            }
        }

        /// <summary>
        /// View from owner to parent. In FE this is Location Group > Location > Inspection Target > Inspection. Parent view not utilized elsewhere, yet.
        /// </summary>
        public CswNbtNodePropViewReference ParentView
        {
            get
            {
                return ( _CswNbtNode.Properties[ParentViewPropertyName].AsViewReference );
            }
        }

        /// <summary>
        /// Run Now button clears the Last Run Date thereby forcing scheduler to process the Generator node on its next iteration 
        /// </summary>
        public CswNbtNodePropButton RunNow
        {
            get
            {
                return ( _CswNbtNode.Properties[RunNowPropertyName].AsButton );
            }
        }

        #endregion

    }//CswNbtObjClassGenerator

}//namespace ChemSW.Nbt.ObjClasses
