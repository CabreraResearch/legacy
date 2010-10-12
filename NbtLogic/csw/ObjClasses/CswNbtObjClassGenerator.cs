using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Actions;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using ChemSW.Nbt.PropertySets;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGenerator : CswNbtObjClass, ICswNbtPropertySetScheduler
    {
        public static string DueDateIntervalPropertyName { get { return "Due Date Interval"; } }
        public static string RunTimePropertyName { get { return "Run Time"; } }
        public static string FinalDueDatePropertyName { get { return "Final Due Date"; } }
        public static string NextDueDatePropertyName { get { return "Next Due Date"; } }
        public static string WarningDaysPropertyName { get { return "Warning Days"; } }
        public static string GraceDaysPropertyName { get { return "Grace Days"; } }
        public static string EnabledPropertyName { get { return "Enabled"; } }
        public static string RunStatusPropertyName { get { return "Run Status"; } }
        public static string TargetTypePropertyName { get { return "Target Type"; } }
        public static string OwnerPropertyName { get { return "Owner"; } }
        public static string DescriptionPropertyName { get { return "Description"; } }
        public static string SummaryPropertyName { get { return "Summary"; } }
        public static string ParentTypePropertyName { get { return "Parent Type"; } }
        public static string ParentViewPropertyName { get { return "Parent View"; } }

        //ICswNbtPropertySetScheduler
        public string SchedulerFinalDueDatePropertyName { get { return FinalDueDatePropertyName; } }
        public string SchedulerNextDueDatePropertyName { get { return NextDueDatePropertyName; } }
        public string SchedulerRunStatusPropertyName { get { return RunStatusPropertyName; } }
        public string SchedulerWarningDaysPropertyName { get { return WarningDaysPropertyName; } }
        public string SchedulerDueDateIntervalPropertyName { get { return DueDateIntervalPropertyName; } }
        public string SchedulerRunTimePropertyName { get { return RunTimePropertyName; } }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtPropertySetSchedulerImpl _CswNbtPropertySetSchedulerImpl;

        public CswNbtObjClassGenerator( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this, Node );
        }//ctor()

        public CswNbtObjClassGenerator( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate();

            // BZ 7845
            if ( TargetType.Empty )
                Enabled.Checked = Tristate.False;
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
            //_CswNbtPropertySetSchedulerImpl.setLastFutureDate();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate();

            // BZ 7845
            if ( TargetType.Empty )
                Enabled.Checked = Tristate.False;
        } //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtPropertySetSchedulerImpl.setLastFutureDate();
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            // BZ 6754 - Delete all future nodes
            CswNbtMetaDataObjectClass GeneratorObjectClass = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( Convert.ToInt32( TargetType.SelectedNodeTypeIds ) );
            CswNbtMetaDataObjectClass TargetObjectClass = TargetNodeType.ObjectClass;

            CswNbtObjClass TargetObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, TargetObjectClass );
            if ( !( TargetObjClass is ICswNbtPropertySetGeneratorTarget ) )
                throw new CswDniException( "CswNbtObjClassGenerator.beforeDeleteNode() got an invalid object class: " + TargetObjectClass.ObjectClass.ToString() );
            ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget)TargetObjectClass;

            CswNbtMetaDataNodeTypeProp GeneratorProp = TargetNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetGeneratorPropertyName );
            CswNbtMetaDataNodeTypeProp IsFutureProp = TargetNodeType.getNodeTypePropByObjectClassPropName( GeneratorTarget.GeneratorTargetIsFuturePropertyName );

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtObjClassSchedule.beforeDeleteNode()";
            CswNbtViewRelationship GeneratorRelationship = View.AddViewRelationship( GeneratorObjectClass, false );
            GeneratorRelationship.NodeIdsToFilterIn.Add( _CswNbtNode.NodeId );
            CswNbtViewRelationship TargetRelationship = View.AddViewRelationship( GeneratorRelationship, CswNbtViewRelationship.PropOwnerType.Second, GeneratorProp, false );
            CswNbtViewProperty IsFutureProperty = View.AddViewProperty( TargetRelationship, IsFutureProp );
            CswNbtViewPropertyFilter IsFutureYesFilter = View.AddViewPropertyFilter( IsFutureProperty, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "True", false );

            ICswNbtTree TargetTree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );

            TargetTree.goToRoot();
            if ( TargetTree.getChildNodeCount() > 0 )  // should always be the case
            {
                TargetTree.goToNthChild( 0 );
                if ( TargetTree.getChildNodeCount() > 0 )   // might not always be the case
                {
                    for ( int i = 0; i < TargetTree.getChildNodeCount(); i++ )
                    {
                        TargetTree.goToNthChild( i );

                        CswNbtNode TargetNode = TargetTree.getNodeForCurrentPosition();
                        TargetNode.delete();

                        TargetTree.goToParentNode();
                    }
                }
            }

            _CswNbtObjClassDefault.beforeDeleteNode();

        } //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropDate FinalDueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[FinalDueDatePropertyName].AsDate );
            }
        }

        public CswNbtNodePropDate NextDueDate
        {
            get
            {
                return ( _CswNbtNode.Properties[NextDueDatePropertyName].AsDate );
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

        public CswNbtNodePropStatic RunStatus
        {
            get
            {
                return ( _CswNbtNode.Properties[RunStatusPropertyName].AsStatic );
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

        /// <summary>
        /// Days after due date to continue to allow edits
        /// </summary>
        public CswNbtNodePropNumber GraceDays
        {
            get
            {
                return ( _CswNbtNode.Properties[GraceDaysPropertyName].AsNumber );
            }
        }
        
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

        public CswNbtNodePropTime RunTime
        {
            get
            {
                return ( _CswNbtNode.Properties[RunTimePropertyName].AsTime );
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
        /// Node type of parent. In FE parent is node type of Fire Extinguisher or Mount Point. In IMCS, parent type is not used.
        /// </summary>
        public CswNbtNodePropNodeTypeSelect ParentType
        {
            get
            {
                return ( _CswNbtNode.Properties[ParentTypePropertyName].AsNodeTypeSelect );
            }
        }

        /// <summary>
        /// View from owner to parent. In FE this is Location Group > Location > Mount Point > Inspection. Parent view not utilized elsewhere, yet.
        /// </summary>
        public CswNbtNodePropViewReference ParentView
        {
            get
            {
                return ( _CswNbtNode.Properties[ParentViewPropertyName].AsViewReference );
            }
        }


        #endregion



    }//CswNbtObjClassGenerator

}//namespace ChemSW.Nbt.ObjClasses
