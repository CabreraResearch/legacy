using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.LandingPage
{
    /// <summary>
    /// Represents a LandingPage ItemType
    /// </summary>
    public sealed class CswNbtLandingPageItemType : CswEnum<CswNbtLandingPageItemType>
    {
        private CswNbtLandingPageItemType( string Name ) : base( Name ) { }
        public static IEnumerable<CswNbtLandingPageItemType> _All { get { return All; } }
        public static implicit operator CswNbtLandingPageItemType( string str )
        {
            CswNbtLandingPageItemType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswNbtLandingPageItemType Unknown = new CswNbtLandingPageItemType( "Unknown" );

        /// <summary>
        /// Static Text
        /// </summary>
        public static readonly CswNbtLandingPageItemType Text = new CswNbtLandingPageItemType( "Text" );
        /// <summary>
        /// Opens an Add Node dialog for the specified NodeType
        /// </summary>
        public static readonly CswNbtLandingPageItemType Add = new CswNbtLandingPageItemType( "Add" );
        /// <summary>
        /// Provides a Link to an existing View, Action, or Report
        /// </summary>
        public static readonly CswNbtLandingPageItemType Link = new CswNbtLandingPageItemType( "Link" );
        /// <summary>
        /// Provides a link to a specified Tab for a given Node (or NodeType)
        /// </summary>
        public static readonly CswNbtLandingPageItemType Tab = new CswNbtLandingPageItemType( "Tab" );
        /// <summary>
        /// Executes ObjectClass Button logic within the context of a given Node
        /// </summary>
        public static readonly CswNbtLandingPageItemType Button = new CswNbtLandingPageItemType( "Button" );

    } // class CswNbtModule
}
