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
        /// Text
        /// </summary>
        public static readonly CswNbtLandingPageItemType Text = new CswNbtLandingPageItemType( "Text" );
        /// <summary>
        /// Add
        /// </summary>
        public static readonly CswNbtLandingPageItemType Add = new CswNbtLandingPageItemType( "Add" );
        /// <summary>
        /// Link
        /// </summary>
        public static readonly CswNbtLandingPageItemType Link = new CswNbtLandingPageItemType( "Link" );
        /// <summary>
        /// Tab
        /// </summary>
        public static readonly CswNbtLandingPageItemType Tab = new CswNbtLandingPageItemType( "Tab" );

    } // class CswNbtModule
}
