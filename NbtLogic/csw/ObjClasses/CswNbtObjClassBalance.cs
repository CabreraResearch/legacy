using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Balance Object Class
    /// </summary>
    public class CswNbtObjClassBalance : CswNbtObjClass
    {
        /// <summary>
        /// Property names for Balances
        /// </summary>
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string Quantity = "Quantity";
            public const string LastActive = "Last Active";
            public const string Manufacturer = "Manufacturer";
            public const string Device = "Device Description";
            public const string RequestConfiguration = "Request Configuration";
            public const string ResponseConfiguration = "Response Configuration";
            public const string Operational = "Operational";
        }

        public CswNbtNodePropText Name  { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropQuantity Quantity{ get { return _CswNbtNode.Properties[PropertyName.Quantity]; } }
        public CswNbtNodePropDateTime LastActive { get { return _CswNbtNode.Properties[PropertyName.LastActive]; } }
        public CswNbtNodePropText Manufacturer { get { return _CswNbtNode.Properties[PropertyName.Manufacturer]; } }
        public CswNbtNodePropText Device { get { return _CswNbtNode.Properties[PropertyName.Device]; } }
        public CswNbtNodePropText RequestConfiguration { get { return _CswNbtNode.Properties[PropertyName.RequestConfiguration]; } }
        public CswNbtNodePropText ResponseConfiguration { get { return _CswNbtNode.Properties[PropertyName.ResponseConfiguration]; } }
        public CswNbtNodePropLogical Operational { get { return _CswNbtNode.Properties[PropertyName.Operational]; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;





        public CswNbtObjClassBalance( CswNbtResources Resources, CswNbtNode Node )
            : base( Resources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }


        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass ); }
        }



        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassBalance
        /// </summary>
        public static implicit operator CswNbtObjClassBalance( CswNbtNode Node )
        {
            CswNbtObjClassBalance ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.BalanceClass ) )
            {
                ret = (CswNbtObjClassBalance) Node.ObjClass;
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


        protected override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.triggerAfterPopulateProps();
        }//afterPopulateProps()


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


    }//class CswNbtObjClassBalance

}//namespace ChemSW.Nbt.csw.ObjClasses