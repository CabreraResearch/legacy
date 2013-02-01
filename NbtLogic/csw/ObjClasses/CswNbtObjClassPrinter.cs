using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;


namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Printer Object Class
    /// </summary>
    public class CswNbtObjClassPrinter : CswNbtObjClass
    {
        /// <summary>
        /// Property names on the Printer class
        /// </summary>
        public sealed class PropertyName
        {
            public const string Name = "Name";
            public const string Description = "Description";
            public const string Enabled = "Enabled";
            public const string LastJobRequest = "Last Job Request";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassPrinter( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.PrinterClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassPrinter
        /// </summary>
        public static implicit operator CswNbtObjClassPrinter( CswNbtNode Node )
        {
            CswNbtObjClassPrinter ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.PrinterClass ) )
            {
                ret = (CswNbtObjClassPrinter) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
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
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            //NodeTypes.SetOnPropChange( OnNodeTypesPropChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropMemo Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropLogical Enabled { get { return _CswNbtNode.Properties[PropertyName.Enabled]; } }
        public CswNbtNodePropDateTime LastJobRequest { get { return _CswNbtNode.Properties[PropertyName.LastJobRequest]; } }

        #endregion

    }//CswNbtObjClassPrinter

}//namespace ChemSW.Nbt.ObjClasses
