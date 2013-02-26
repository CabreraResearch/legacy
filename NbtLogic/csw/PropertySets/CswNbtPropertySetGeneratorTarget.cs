using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Defines nodetypes that can be the target of a Generator.
    /// </summary>
    public abstract class CswNbtPropertySetGeneratorTarget : CswNbtObjClass
    {
        public class PropertyName
        {
            public const string Generator = "Generator";
            public const string DueDate = "Due Date";
            public const string CreatedDate = "Date Created";
            public const string IsFuture = "IsFuture";
        }

        // Configurable Object Class properties
        public abstract string ParentPropertyName { get; }

        public static Collection<NbtObjectClass> Members()
        {
            Collection<NbtObjectClass> Ret = new Collection<NbtObjectClass>();
            Ret.Add( NbtObjectClass.InspectionDesignClass );
            Ret.Add( NbtObjectClass.TaskClass );
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
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.GenericClass ); }
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

        public abstract void beforePropertySetWriteNode( bool IsCopy, bool OverrideUniqueValidation );

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            beforePropertySetWriteNode( IsCopy, OverrideUniqueValidation );

            if( DateTime.MinValue == CreatedDate.DateTimeValue )
            {
                CreatedDate.DateTimeValue = DateTime.Now;
            }

            CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public abstract void afterPropertySetWriteNode();

        public override void afterWriteNode()
        {
            afterPropertySetWriteNode();

            CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public abstract void beforePropertySetDeleteNode( bool DeleteAllRequiredRelatedNodes );

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            beforePropertySetDeleteNode( DeleteAllRequiredRelatedNodes );

            CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public abstract void afterPropertySetPopulateProps();

        public override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();

            CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public abstract bool onPropertySetButtonClick( NbtButtonData ButtonData );

        public override bool onButtonClick( NbtButtonData ButtonData )
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
