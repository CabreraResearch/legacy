
namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodeTypePropListOption
    {


        public CswNbtNodeTypePropListOption( string Text, string Value )
        {
            _Text = Text;
            _Value = Value;
        }//ctor

        public bool Empty
        {
            get
            {
                return ( string.Empty == Text || string.Empty == Value );
            }//get
        }//Empty

        private string _Text = "";
        public string Text
        {
            set
            {
                _Text = value;
            }//set

            get
            {
                return ( _Text );
            }//get
        }//Text

        private string _Value = "";
        public string Value
        {
            set
            {
                _Value = value;
            }//set

            get
            {
                return ( _Value );
            }//get
        }//Value

    }//CswNbtNodeTypePropListOption

}//namespace ChemSW.Nbt.PropTypes
