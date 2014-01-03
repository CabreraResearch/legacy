using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace NbtWebApp.Actions.Receiving
{
    [DataContract]
    public class CswNbtAmountsGridQuantity
    {

        [DataMember( Name = "containerNo" )]
        public int NumContainers { get; set; }

        [DataMember( Name = "quantity" )]
        public double Quantity { get; set; }

        [DataMember( Name = "sizename" )]
        public string SizeName { get; set; }

        private CswPrimaryKey _sizeNodeId = null;

        public CswPrimaryKey SizeNodeId
        {
            get
            {
                if( null == _sizeNodeId )
                {
                    _sizeNodeId = CswConvert.ToPrimaryKey( _sizeIdStr );
                    if( false == CswTools.IsPrimaryKey( _sizeNodeId ) )
                    {
                        _sizeNodeId = null;
                    }
                }
                return _sizeNodeId;
            }
            set
            {
                _sizeNodeId = value;
                _sizeIdStr = _sizeNodeId.ToString();
            }
        }

        private CswPrimaryKey _unitNodeId = null;

        public CswPrimaryKey UnitNodeId
        {
            get
            {
                if( null == _unitNodeId )
                {
                    _unitNodeId = CswConvert.ToPrimaryKey( _unitIdStr );
                    if( false == CswTools.IsPrimaryKey( _unitNodeId ) )
                    {
                        _unitNodeId = null;
                    }
                }
                return _unitNodeId;
            }
            set
            {
                _unitNodeId = value;
                _unitIdStr = _unitNodeId.ToString();
            }
        }

        //Note - I am unsure about Barcodes, it may be a collection or a comma delimited string.
        private Collection<string> _Barcodes = new Collection<string>();
        [DataMember( Name = "barcodes" )]
        public Collection<string> Barcodes
        {
            get { return _Barcodes; }
            set { _Barcodes = value; }
        }

        private Collection<string> _ContainerIds = new Collection<string>();
        [DataMember( Name = "containerids" )]
        public Collection<string> ContainerIds
        {
            get { return _ContainerIds; }
            set { _ContainerIds = value; }
        }

        //FOR WCF ONLY
        private string _sizeIdStr = string.Empty;

        [DataMember( Name = "sizeid" )]
        private string sizeIdStr
        {
            get { return _sizeIdStr; }
            set { _sizeIdStr = value; }
        }

        private string _unitIdStr = string.Empty;

        [DataMember( Name = "unitid" )]
        private string unitIdStr
        {
            get { return _unitIdStr; }
            set { _unitIdStr = value; }
        }

    }
}