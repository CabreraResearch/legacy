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

        private CswCommaDelimitedString _Barcodes = null;
        private CswCommaDelimitedString Barcodes
        {
            get
            {
                if( null == _Barcodes )
                {
                    _Barcodes = new CswCommaDelimitedString();
                    _Barcodes.FromString( _BarcodesStr );
                }
                return _Barcodes;
            }
            set
            {
                _Barcodes = value;
                _BarcodesStr = _Barcodes.ToString();
            }
        }
        public CswCommaDelimitedString getBarcodes()
        {
            return Barcodes;
        }
        public void AddBarcode( string Barcode )
        {
            Barcodes.Add( Barcode, false );
            _BarcodesStr = Barcodes.ToString();
        }
        public int getNumBarcodes()
        {
            return Barcodes.Count;
        }

        private string _BarcodesStr = string.Empty;
        [DataMember( Name = "barcodes" )]
        private string BarcodesStr
        {
            get { return _BarcodesStr; }
            set { _BarcodesStr = value; }
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