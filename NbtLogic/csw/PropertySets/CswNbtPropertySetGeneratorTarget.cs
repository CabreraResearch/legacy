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
        /// Property Set ctor
        /// </summary>
        public CswNbtPropertySetGeneratorTarget( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

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

        public virtual void beforePropertySetWriteNode() {}

        public override void beforeWriteNode( bool Creating )
        {
            beforePropertySetWriteNode();

            if( DateTime.MinValue == CreatedDate.DateTimeValue )
            {
                CreatedDate.DateTimeValue = DateTime.Now;
            }
        }//beforeWriteNode()

        public virtual void afterPropertySetWriteNode() {}

        public override void afterWriteNode()
        {
            afterPropertySetWriteNode();
        }//afterWriteNode()

        public virtual void beforePropertySetDeleteNode() {}

        public override void beforeDeleteNode()
        {
            beforePropertySetDeleteNode();
        }//beforeDeleteNode()     

        public virtual void afterPropertySetPopulateProps() {}

        protected override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();
        }//afterPopulateProps()

        public virtual bool onPropertySetButtonClick( NbtButtonData ButtonData ) { return true; }

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
