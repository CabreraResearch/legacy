using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.PropTypes
{
    public sealed class CswEnumNbtNFPADisplayMode : CswEnum<CswEnumNbtNFPADisplayMode>
    {
        private CswEnumNbtNFPADisplayMode( string mode ) : base( mode ) { }
        public static IEnumerable<CswEnumNbtNFPADisplayMode> all { get { return All; } }
        public static explicit operator CswEnumNbtNFPADisplayMode( string str )
        {
            CswEnumNbtNFPADisplayMode ret = Parse( str );
            return ret ?? Diamond; //return the selected value, or Diamond if none
        }
        public static readonly CswEnumNbtNFPADisplayMode Linear = new CswEnumNbtNFPADisplayMode( "Linear" );
        public static readonly CswEnumNbtNFPADisplayMode Diamond = new CswEnumNbtNFPADisplayMode( "Diamond" );
    }


}//namespace ChemSW.Nbt.PropTypes
