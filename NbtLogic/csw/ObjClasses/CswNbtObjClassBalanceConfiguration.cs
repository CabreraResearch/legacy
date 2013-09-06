using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Stores information about a set of configuration settings for Balances
    /// </summary>
    public class CswNbtObjClassBalanceConfiguration : CswNbtObjClass
    {

        
        /// <summary>
        /// Property names for Balance Configurations
        /// </summary>
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Name = "Name";
            public const string RequestFormat = "Request Format";
            public const string ResponseFormat = "Response Format";
            public const string BaudRate = "Baud Rate";
            public const string ParityBit = "Parity Bit";
            public const string DataBits = "Data Bits";
            public const string StopBits = "Stop Bits";
            public const string Handshake = "Handshake";
        }

        public CswNbtNodePropText Name  { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropText RequestFormat { get { return _CswNbtNode.Properties[PropertyName.RequestFormat]; } }
        public CswNbtNodePropText ResponseFormat { get { return _CswNbtNode.Properties[PropertyName.ResponseFormat]; } }
        public CswNbtNodePropNumber BaudRate { get { return _CswNbtNode.Properties[PropertyName.BaudRate];  } }
        public CswNbtNodePropText ParityBit { get { return _CswNbtNode.Properties[PropertyName.ParityBit]; } }
        public CswNbtNodePropNumber DataBits { get { return _CswNbtNode.Properties[PropertyName.DataBits]; } }
        public CswNbtNodePropText StopBits { get { return _CswNbtNode.Properties[PropertyName.StopBits]; } }
        public CswNbtNodePropText Handshake { get { return _CswNbtNode.Properties[PropertyName.Handshake]; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;





        public CswNbtObjClassBalanceConfiguration( CswNbtResources Resources, CswNbtNode Node )
            : base( Resources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }


        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass ); }
        }



        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassBalance
        /// </summary>
        public static implicit operator CswNbtObjClassBalanceConfiguration( CswNbtNode Node )
        {
            CswNbtObjClassBalanceConfiguration ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.BalanceConfigurationClass ) )
            {
                ret = (CswNbtObjClassBalanceConfiguration) Node.ObjClass;
            }
            return ret;
        }



        #region Inherited Events


        public override void beforeCreateNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeCreateNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()


        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
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

    }
}
