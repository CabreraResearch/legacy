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

        public CswNbtObjClassBalance( CswNbtResources Resources, CswNbtNode Node ) : base( Resources, Node ) {}

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

        //Extend CswNbtObjClass events here

        #endregion

    }//class CswNbtObjClassBalance

}//namespace ChemSW.Nbt.csw.ObjClasses