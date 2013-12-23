using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Defines nodetypes that can be the target of a Generator.
    /// </summary>
    public abstract class CswNbtPropertySetGeneratorTarget : CswNbtObjClass
    {
        public new class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Generator = "Generator";
            public const string DueDate = "Due Date";
            public const string CreatedDate = "Date Created";
            public const string IsFuture = "IsFuture";
        }

        // Configurable Object Class properties
        public abstract string ParentPropertyName { get; }

        public static Collection<CswEnumNbtObjectClass> Members()
        {
            Collection<CswEnumNbtObjectClass> Ret = new Collection<CswEnumNbtObjectClass>();
            Ret.Add( CswEnumNbtObjectClass.InspectionDesignClass );
            Ret.Add( CswEnumNbtObjectClass.TaskClass );
            return Ret;
        }

        /// <summary>
        /// Default Object Class for consumption by derived classes
        /// </summary>
        public CswNbtObjClassDefault CswNbtObjClassDefault = null;

        /// <summary>
        /// Property Set ctor
        /// </summary>
        public CswNbtPropertySetGeneratorTarget( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetRequestItem
        /// </summary>
        public static implicit operator CswNbtPropertySetGeneratorTarget( CswNbtNode Node )
        {
            CswNbtPropertySetGeneratorTarget ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetGeneratorTarget) Node.ObjClass;
            }
            return ret;
        }


        #region Inherited Events

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
        }

        public override void afterPromoteNode()
        {
        }

        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );

            if( DateTime.MinValue == CreatedDate.DateTimeValue )
            {
                CreatedDate.DateTimeValue = DateTime.Now;
            }

            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()

        public abstract void afterPropertySetWriteNode();

        public override void afterWriteNode( bool Creating )
        {
            afterPropertySetWriteNode();

            CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()

        public abstract void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes );

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );

            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public abstract void afterPropertySetPopulateProps();

        protected override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();

            CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public abstract bool onPropertySetButtonClick( NbtButtonData ButtonData );

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            bool Ret = false;
            if( null != ButtonData.NodeTypeProp )
            {
                Ret = onPropertySetButtonClick( ButtonData );
            }
            return Ret;
        }
        #endregion

        #region Property Set specific properties

        public CswNbtNodePropDateTime DueDate { get { return _CswNbtNode.Properties[PropertyName.DueDate]; } }
        public CswNbtNodePropDateTime CreatedDate { get { return _CswNbtNode.Properties[PropertyName.CreatedDate]; } }
        public CswNbtNodePropLogical IsFuture { get { return _CswNbtNode.Properties[PropertyName.IsFuture]; } }
        public CswNbtNodePropRelationship Generator { get { return _CswNbtNode.Properties[PropertyName.Generator]; } }
        
        public abstract CswNbtNodePropRelationship Parent { get; }

        #endregion


    }//CswNbtPropertySetRequestItem

}//namespace ChemSW.Nbt.ObjClasses
