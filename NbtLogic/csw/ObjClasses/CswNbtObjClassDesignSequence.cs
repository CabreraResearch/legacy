using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDesignSequence : CswNbtObjClass
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Pre = "Pre";
            public const string Post = "Post";
            public const string Pad = "Pad";
            public const string NextValue = "Next Value";
        }


        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassDesignSequence( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }

        //ctor()

        /// <summary>
        /// This is the object class that OWNS this property (DesignNodeType)
        /// If you want the object class property value, look for ObjectClassProperty
        /// </summary>
        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignSequenceClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassDesignNodeType
        /// </summary>
        public static implicit operator CswNbtObjClassDesignSequence( CswNbtNode Node )
        {
            CswNbtObjClassDesignSequence ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.DesignSequenceClass ) )
            {
                ret = (CswNbtObjClassDesignSequence) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        } //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        } //afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        } //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        } // afterDeleteNode()        

        protected override void afterPopulateProps()
        {
            CswNbtSequenceValue NextSequenceValue = new CswNbtSequenceValue( _CswNbtResources, RelationalId.PrimaryKey );
            NextValue.Text = NextSequenceValue.Current;

            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        } //afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp )
            {
                /*Do Something*/
            }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropText Pre { get { return ( _CswNbtNode.Properties[PropertyName.Pre] ); } }
        public CswNbtNodePropText Post { get { return ( _CswNbtNode.Properties[PropertyName.Post] ); } }
        public CswNbtNodePropNumber Pad { get { return ( _CswNbtNode.Properties[PropertyName.Pad] ); } }
        public CswNbtNodePropText NextValue { get { return ( _CswNbtNode.Properties[PropertyName.NextValue] ); } }

        #endregion

    }//CswNbtObjClassDesignSequence

}//namespace ChemSW.Nbt.ObjClasses
