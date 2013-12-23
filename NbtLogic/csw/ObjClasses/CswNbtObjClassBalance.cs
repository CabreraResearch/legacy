﻿using ChemSW.Nbt.MetaData;
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
            public const string Operational = "Operational";
            public const string BalanceConfiguration = "Balance Configuration";
        }

        public CswNbtNodePropText Name  { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropQuantity Quantity{ get { return _CswNbtNode.Properties[PropertyName.Quantity]; } }
        public CswNbtNodePropDateTime LastActive { get { return _CswNbtNode.Properties[PropertyName.LastActive]; } }
        public CswNbtNodePropText Manufacturer { get { return _CswNbtNode.Properties[PropertyName.Manufacturer]; } }
        public CswNbtNodePropText Device { get { return _CswNbtNode.Properties[PropertyName.Device]; } }
        public CswNbtNodePropLogical Operational { get { return _CswNbtNode.Properties[PropertyName.Operational]; } }
        public CswNbtNodePropRelationship BalanceConfiguration { get { return _CswNbtNode.Properties[PropertyName.BalanceConfiguration];  } }

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

        public override void beforePromoteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforePromoteNode( IsCopy, OverrideUniqueValidation );
        }//beforeCreateNode()

        public override void afterPromoteNode()
        {
            _CswNbtObjClassDefault.afterPromoteNode();
        }//afterCreateNode()


        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation, bool Creating )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation, Creating );
        }//beforeWriteNode()


        public override void afterWriteNode( bool Creating )
        {
            _CswNbtObjClassDefault.afterWriteNode( Creating );
        }//afterWriteNode()


        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false, bool ValidateRequiredRelationships = true )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes, ValidateRequiredRelationships );

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