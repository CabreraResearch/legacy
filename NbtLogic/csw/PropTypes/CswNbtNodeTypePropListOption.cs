
using System.Runtime.Serialization;

namespace ChemSW.Nbt.PropTypes
{
    [DataContract]
    public class CswNbtNodeTypePropListOption
    {
        public CswNbtNodeTypePropListOption( string Text, string Value )
        {
            _Text = Text;
            _Value = Value;
        }//ctor

        public bool Empty
        {
            get { return ( string.Empty == Text || string.Empty == Value ); }
        }//Empty

        private string _Text = "";
        private string _Value = "";

        [DataMember]
        public string Text
        {
            set { _Text = value; }
            get { return ( _Text ); }
        }//Text

        [DataMember]
        public string Value
        {
            set { _Value = value; }
            get { return ( _Value ); }
        }//Value

    }//CswNbtNodeTypePropListOption

}//namespace ChemSW.Nbt.PropTypes
