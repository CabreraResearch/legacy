using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassProblem : CswNbtObjClass
    {
        public static string PartsPropertyName { get { return "Parts"; } }
        public static string OwnerPropertyName { get { return "Owner"; } }
        public static string DateOpenedPropertyName { get { return "Date Opened"; } }
        public static string DateClosedPropertyName { get { return "Date Closed"; } }
        public static string ClosedPropertyName { get { return "Closed"; } }
        public static string ReportedByPropertyName { get { return "Reported By"; } }
        public static string FailurePropertyName { get { return "Failure"; } }
        public static string PartsXValueName { get { return "Service"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassProblem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassProblem( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            // BZ 10051 - Set the Date Opened to today
			DateOpened.DateTimeValue = DateTime.Today;
            ReportedBy.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
            ReportedBy.CachedNodeName = _CswNbtResources.CurrentNbtUser.Username;

            _checkClosed();

            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _checkClosed();

            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        private void _checkClosed()
        {
            // BZ 10051 - If we're closing the Problem, set the Date Closed to today
			if( Closed.Checked == Tristate.True && DateClosed.DateTimeValue == DateTime.MinValue )
				DateClosed.DateTimeValue = DateTime.Today;

            // BZ 10051 - If we're reopening the Problem, clear the Date Closed
			if( Closed.Checked == Tristate.False && DateClosed.DateTimeValue != DateTime.MinValue )
				DateClosed.DateTimeValue = DateTime.MinValue;
        }

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            if( Owner.RelatedNodeId != null )
            {
                CswNbtNode EquipmentOrAssemblyNode = _CswNbtResources.Nodes[Owner.RelatedNodeId];
                if( EquipmentOrAssemblyNode != null )
                {
                    if( EquipmentOrAssemblyNode.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass )
                    {
                        CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                        CswNbtObjClassEquipment EquipmentNodeAsEquipment = CswNbtNodeCaster.AsEquipment( EquipmentOrAssemblyNode );
                        //CswNbtObjClassEquipment Equipment = CswNbtObjClassFactory.Make( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass ) as CswNbtObjClassEquipment;
                        foreach( string YValue in EquipmentNodeAsEquipment.Parts.YValues )
                        {
                            if( EquipmentNodeAsEquipment.Parts.CheckValue( CswNbtObjClassEquipment.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                        this.Parts.YValues = NewYValues;
                    }
                    else if( EquipmentOrAssemblyNode.ObjectClass.ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass )
                    {
                        CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                        CswNbtObjClassEquipmentAssembly AssemblyNodeAsAssembly = CswNbtNodeCaster.AsEquipmentAssembly( EquipmentOrAssemblyNode );
                        foreach( string YValue in AssemblyNodeAsAssembly.Parts.YValues )
                        {
                            if( AssemblyNodeAsAssembly.Parts.CheckValue( CswNbtObjClassEquipmentAssembly.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                        this.Parts.YValues = NewYValues;
                    }
                } // if( EquipmentOrAssemblyNode != null )
            } // if( Owner.RelatedNodeId != null )

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties



        //public CswNbtNodePropRelationship Equipment
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[EquipmentPropertyName].AsRelationship );
        //    }
        //}
        public CswNbtNodePropRelationship Owner
        {
            get
            {
                return ( _CswNbtNode.Properties[OwnerPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropRelationship ReportedBy
        {
            get
            {
                return ( _CswNbtNode.Properties[ReportedByPropertyName].AsRelationship );
            }
        }
        public CswNbtNodePropLogicalSet Parts
        {
            get
            {
                return ( _CswNbtNode.Properties[PartsPropertyName].AsLogicalSet );
            }
        }
        public CswNbtNodePropLogical Closed
        {
            get
            {
                return ( _CswNbtNode.Properties[ClosedPropertyName].AsLogical );
            }
        }

		public CswNbtNodePropDateTime DateOpened
        {
            get
            {
                return ( _CswNbtNode.Properties[DateOpenedPropertyName].AsDateTime );
            }
        }
		public CswNbtNodePropDateTime DateClosed
        {
            get
            {
                return ( _CswNbtNode.Properties[DateClosedPropertyName].AsDateTime );
            }
        }
        public CswNbtNodePropLogical Failure
        {
            get
            {
                return ( _CswNbtNode.Properties[FailurePropertyName].AsLogical );
            }
        }

        #endregion



    }//CswNbtObjClassProblem

}//namespace ChemSW.Nbt.ObjClasses
