
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;
using Newtonsoft.Json.Linq;

namespace NbtWebApp.Actions.Receiving
{
    [DataContract]
    public class CswNbtReceivingDefiniton
    {
        public int CountNumberContainersToMake()
        {
            int Ret = 0;
            foreach( CswNbtAmountsGridQuantity Quant in Quantities )
            {
                Ret += Quant.NumContainers;
            }
            return Ret;
        }
        
        [DataMember]
        public string ActionData;

        private CswPrimaryKey _containerNodeId = null;
        public CswPrimaryKey ContainerNodeId
        {
            get
            {
                if( null == _containerNodeId )
                {
                    _containerNodeId = CswConvert.ToPrimaryKey( _containerNodeIdStr );
                    if( false == CswTools.IsPrimaryKey( _containerNodeId ) )
                    {
                        _containerNodeId = null;
                    }
                }
                return _containerNodeId;
            }
            set
            {
                _containerNodeId = value;
                _containerNodeIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _materialNodeId = null;
        public CswPrimaryKey MaterialNodeId
        {
            get
            {
                if( null == _materialNodeId )
                {
                    _materialNodeId = CswConvert.ToPrimaryKey( _materialIdStr );
                    if( false == CswTools.IsPrimaryKey( _materialNodeId ) )
                    {
                        _materialNodeId = null;
                    }
                }
                return _materialNodeId;
            }
            set
            {
                _materialNodeId = value;
                _materialIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _sdsNodeId = null;
        public CswPrimaryKey SDSNodeId
        {
            get
            {
                if( null == _sdsNodeId )
                {
                    _sdsNodeId = CswConvert.ToPrimaryKey( _sdsDocIdStr );
                    if( false == CswTools.IsPrimaryKey( _sdsNodeId ) )
                    {
                        _sdsNodeId = null;
                    }
                }
                return _sdsNodeId;
            }
            set
            {
                _sdsNodeId = value;
                _sdsDocIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _cofaDocNodeId = null;
        public CswPrimaryKey CofADocNodeId
        {
            get
            {
                if( null == _cofaDocNodeId )
                {
                    _cofaDocNodeId = CswConvert.ToPrimaryKey( _cofaDocIdStr );
                    if( false == CswTools.IsPrimaryKey( _cofaDocNodeId ) )
                    {
                        _cofaDocNodeId = null;
                    }
                }
                return _cofaDocNodeId;
            }
            set
            {
                _cofaDocNodeId = value;
                _cofaDocIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _receiptLotNodeId = null;
        public CswPrimaryKey ReceiptLotNodeId
        {
            get
            {
                if( null == _receiptLotNodeId )
                {
                    _receiptLotNodeId = CswConvert.ToPrimaryKey( _receiptLotIdStr );
                    if( false == CswTools.IsPrimaryKey( _receiptLotNodeId ) )
                    {
                        _receiptLotNodeId = null;
                    }
                }
                return _receiptLotNodeId;
            }
            set
            {
                _receiptLotNodeId = value;
                _receiptLotIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _requestItemNodeId = null;
        public CswPrimaryKey RequestItemtNodeId
        {
            get
            {
                if( null == _requestItemNodeId )
                {
                    _requestItemNodeId = CswConvert.ToPrimaryKey( _requestItemIdStr );
                    if( false == CswTools.IsPrimaryKey( _requestItemNodeId ) )
                    {
                        _requestItemNodeId = null;
                    }
                }
                return _requestItemNodeId;
            }
            set
            {
                _requestItemNodeId = value;
                _requestItemIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _printLabelId = null;
        public CswPrimaryKey PrintLabelId
        {
            get
            {
                if( null == _printLabelId )
                {
                    _printLabelId = CswConvert.ToPrimaryKey( _printLabelIdStr );
                    if( false == CswTools.IsPrimaryKey( _printLabelId ) )
                    {
                        _printLabelId = null;
                    }
                }
                return _printLabelId;
            }
            set
            {
                _printLabelId = value;
                _printLabelIdStr = value.ToString();
            }
        }

        private CswPrimaryKey _printerId = null;
        public CswPrimaryKey PrinterNodeId
        {
            get
            {
                if( null == _printerId )
                {
                    _printerId = CswConvert.ToPrimaryKey( _printerIdStr );
                    if( false == CswTools.IsPrimaryKey( _printerId ) )
                    {
                        _printerId = null;
                    }
                }
                return _printerId;
            }
            set
            {
                _printerId = value;
                _printerIdStr = value.ToString();
            }
        }

        private int _containerNodeTypeId = Int32.MinValue;
        [DataMember( Name = "containernodetypeid" )]
        public int ContainerNodeTypeId
        {
            get { return _containerNodeTypeId; }
            set { _containerNodeTypeId = value; }
        }

        private JObject _containerPropsJSON = null;
        public JObject ContainerProps
        {
            get
            {
                if( null == _containerPropsJSON )
                {
                    _containerPropsJSON = CswConvert.ToJObject( _containerPropsJSONStr );
                }
                return _containerPropsJSON;
            }
            set
            {
                _containerPropsJSON = value;
                _containerPropsJSONStr = value.ToString();
            }
        }

        private JObject _sdsPropsJSON = null;
        public JObject SDSProps
        {
            get
            {
                if( null == _sdsPropsJSON )
                {
                    _sdsPropsJSON = CswConvert.ToJObject( _sdsPropsJSONStr );
                }
                return _sdsPropsJSON;
            }
            set
            {
                _sdsPropsJSON = value;
                _sdsPropsJSONStr = value.ToString();
            }
        }

        private JObject _cofaPropsJSON = null;
        public JObject CofAPropsJSON
        {
            get
            {
                if( null == _cofaPropsJSON )
                {
                    _cofaPropsJSON = CswConvert.ToJObject( _cofaJSONStr );
                }
                return _cofaPropsJSON;
            }
            set
            {
                _cofaPropsJSON = value;
                _cofaJSONStr = value.ToString();
            }
        }

        private JObject _receiptLotPropsJSON = null;
        public JObject ReceiptLotProps
        {
            get
            {
                if( null == _receiptLotPropsJSON )
                {
                    _receiptLotPropsJSON = CswConvert.ToJObject( _receiptLotJSONStr );
                }
                return _receiptLotPropsJSON;
            }
            set
            {
                _receiptLotPropsJSON = value;
                _receiptLotJSONStr = value.ToString();
            }
        }

        [DataMember( Name = "quantities" )]
        public Collection<CswNbtAmountsGridQuantity> Quantities
        {
            get;
            set;
        }

        //THE FOLLOWING ARE FOR WCF
        private string _containerNodeIdStr = String.Empty;
        [DataMember( Name = "containernodeid" )]
        private string containerNodeIdStr
        {
            get { return _containerNodeIdStr; }
            set { _containerNodeIdStr = value; }
        }


        private string _materialIdStr = string.Empty;
        [DataMember( Name = "materialid" )]
        private string materialIdStr
        {
            get { return _materialIdStr; }
            set { _materialIdStr = value; }
        }

        private string _sdsDocIdStr = string.Empty;
        [DataMember( Name = "sdsDocId" )]
        private string sdsDocIdStr
        {
            get { return _sdsDocIdStr; }
            set { _sdsDocIdStr = value; }
        }

        private string _cofaDocIdStr = string.Empty;
        [DataMember( Name = "cofaDocId" )]
        private string cofaDocIdStr
        {
            get { return _cofaDocIdStr; }
            set { _cofaDocIdStr = value; }
        }

        private string _receiptLotIdStr = string.Empty;
        [DataMember( Name = "receiptLotId" )]
        private string receiptLotIdStr
        {
            get { return _receiptLotIdStr; }
            set { _receiptLotIdStr = value; }
        }

        private string _requestItemIdStr = string.Empty;
        [DataMember( Name = "requestitemid" )]
        private string requestItemIdStr
        {
            get { return _requestItemIdStr; }
            set { _requestItemIdStr = value; }
        }

        private string _printLabelIdStr = string.Empty;
        [DataMember( Name = "LabelId" )]
        private string printlabelIdStr
        {
            get { return _printLabelIdStr; }
            set { _printLabelIdStr = value; }
        }

        private string _printerIdStr = string.Empty;
        [DataMember( Name = "PrinterId" )]
        private string printerIdStr
        {
            get { return _printerIdStr; }
            set { _printerIdStr = value; }
        }

        private string _containerPropsJSONStr = string.Empty;
        [DataMember( Name = "props" )]
        private string containerPropsJSONStr
        {
            get { return _containerPropsJSONStr; }
            set { _containerPropsJSONStr = value; }
        }

        private string _sdsPropsJSONStr = string.Empty;
        [DataMember( Name = "sdsDocProperties" )]
        private string sdsPropsJSONStr
        {
            get { return _sdsPropsJSONStr; }
            set { _sdsPropsJSONStr = value; }
        }

        private string _cofaJSONStr = string.Empty;
        [DataMember( Name = "cofaDocProperties" )]
        private string cofaJSONStr
        {
            get { return _cofaJSONStr; }
            set { _cofaJSONStr = value; }
        }

        private string _receiptLotJSONStr = string.Empty;
        [DataMember( Name = "receiptLotProperties" )]
        private string receiptLotJSONStr
        {
            get { return _receiptLotJSONStr; }
            set { _receiptLotJSONStr = value; }
        }

    }
}