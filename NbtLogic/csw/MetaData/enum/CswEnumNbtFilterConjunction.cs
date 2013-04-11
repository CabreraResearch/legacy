using System;
using System.Collections.Generic;
using ChemSW.Exceptions;
using ChemSW.Nbt.Security;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{
    /// <summary>
    /// Filter Conjunction
    /// </summary>
    public sealed class CswEnumNbtFilterConjunction : CswEnum<CswEnumNbtFilterConjunction>
    {
        private CswEnumNbtFilterConjunction( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtFilterConjunction> _All { get { return All; } }
        public static implicit operator CswEnumNbtFilterConjunction( string str )
        {
            CswEnumNbtFilterConjunction ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtFilterConjunction Unknown = new CswEnumNbtFilterConjunction( "Unknown" );

        public static readonly CswEnumNbtFilterConjunction And = new CswEnumNbtFilterConjunction( "And" );
        public static readonly CswEnumNbtFilterConjunction Or = new CswEnumNbtFilterConjunction( "Or" );
    }

}//namespace ChemSW.Nbt.MetaData
