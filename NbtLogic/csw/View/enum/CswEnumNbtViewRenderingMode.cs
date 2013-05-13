using System.Collections.Generic;
using System.Runtime.Serialization;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// View Rendering Mode
    /// </summary>
    [DataContract]
    public sealed class CswEnumNbtViewRenderingMode : CswEnum<CswEnumNbtViewRenderingMode>
    {
        private CswEnumNbtViewRenderingMode( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtViewRenderingMode> _All { get { return All; } }
        public static implicit operator CswEnumNbtViewRenderingMode( string str )
        {
            CswEnumNbtViewRenderingMode ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtViewRenderingMode Unknown = new CswEnumNbtViewRenderingMode( "Unknown" );

        public static readonly CswEnumNbtViewRenderingMode Tree = new CswEnumNbtViewRenderingMode( "Tree" );
        public static readonly CswEnumNbtViewRenderingMode Grid = new CswEnumNbtViewRenderingMode( "Grid" );
        public static readonly CswEnumNbtViewRenderingMode List = new CswEnumNbtViewRenderingMode( "List" );
        public static readonly CswEnumNbtViewRenderingMode Table = new CswEnumNbtViewRenderingMode( "Table" );
        public static readonly CswEnumNbtViewRenderingMode Any = new CswEnumNbtViewRenderingMode( "Any" );
    }

} // namespace ChemSW.Nbt
