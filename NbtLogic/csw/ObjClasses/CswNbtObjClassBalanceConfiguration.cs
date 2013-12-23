﻿using ChemSW.Nbt.MetaData;
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

        public CswNbtObjClassBalanceConfiguration( CswNbtResources Resources, CswNbtNode Node ) : base( Resources, Node ) {}

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

        //Extend CswNbtObjClass events here

        #endregion

    }
}
