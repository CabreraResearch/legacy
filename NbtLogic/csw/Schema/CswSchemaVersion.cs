using System;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Represents a Schema Version tag for NBT
    /// </summary>
    public class CswSchemaVersion : IComparable<CswSchemaVersion>, IEquatable<CswSchemaVersion>
    {
        /// <summary>
        /// The release cycle iteration (how many times we've gone from A to Z).  This rarely changes.
        /// </summary>
        public Int32 CycleIteration;
        /// <summary>
        /// Single character release identifier (A-Z).  This changes for every release.
        /// </summary>
        public char ReleaseIdentifier;
        /// <summary>
        /// Iteration within this release.  This changes for every script.
        /// </summary>
        public Int32 ReleaseIteration;

        /// <summary>
        /// Constructor to build a Schema Version tag from parts
        /// </summary>
        public CswSchemaVersion( Int32 inCycleIteration, char inReleaseIdentifier, Int32 inReleaseIteration )
        {
            _init( inCycleIteration, inReleaseIdentifier, inReleaseIteration );
        }

        /// <summary>
        /// Constructor for single integer version numbers (for backwards compatibility)
        /// </summary>
        public CswSchemaVersion( Int32 SingleIntegerVersion )
        {
            _init( 0, 'X', SingleIntegerVersion );
        }
        /// <summary>
        /// Constructor for string version of Schema Version tag.  Detects single integers for backwards compatibility.
        /// </summary>
        public CswSchemaVersion( string SchemaVersionAsString )
        {
            if( SchemaVersionAsString.Length < 6 && CswTools.IsInteger( SchemaVersionAsString ) )
            {
                // Example: 74   (backwards compatibility)
                _init( 0, 'X', CswConvert.ToInt32( SchemaVersionAsString ) );
            }
            else
            {
                // Example: 01F-02

                Int32 CycleIteration = CswConvert.ToInt32( SchemaVersionAsString.Substring( 0, 2 ) );
                char ReleaseIdentifier = SchemaVersionAsString.Substring( 2, 1 )[0];

                //case 27448: variable length iteration segment (retro-handle 2-character segments and forward-handle 3-character segments)
                Int32 ReleaseIteration = CswConvert.ToInt32( SchemaVersionAsString.Substring( 4, SchemaVersionAsString.Length - 4 ) );

                _init( CycleIteration, ReleaseIdentifier, ReleaseIteration );
            }
        }

        private void _init( Int32 inCycleIteration, char inReleaseIdentifier, Int32 inReleaseIteration )
        {
            CycleIteration = inCycleIteration;
            ReleaseIdentifier = inReleaseIdentifier;
            ReleaseIteration = inReleaseIteration;

        }

        /// <summary>
        /// String version of Schema Version tag, e.g. 01F-02
        /// </summary>
        public override string ToString()
        {
            string ret = "";
            ret += CswTools.PadInt( CycleIteration, 2 );
            ret += ReleaseIdentifier.ToString();
            ret += "-";
            ret += CswTools.PadInt( ReleaseIteration, 3 );//27448: going forward, three character iteration segments
            return ret;
        }

        #region IComparable<CswSchemaVersion> Members

        public static bool operator <( CswSchemaVersion ver1, CswSchemaVersion ver2 )
        {
            return ver1.CompareTo( ver2 ) < 0;
        }
        public static bool operator >( CswSchemaVersion ver1, CswSchemaVersion ver2 )
        {
            return ver1.CompareTo( ver2 ) > 0;
        }

        public Int32 CompareTo( CswSchemaVersion other )
        {
            Int32 ret = 0;
            if( this.CycleIteration != other.CycleIteration )
                ret = this.CycleIteration.CompareTo( other.CycleIteration );
            else if( this.ReleaseIdentifier != other.ReleaseIdentifier )
                ret = this.ReleaseIdentifier.CompareTo( other.ReleaseIdentifier );
            else
                ret = this.ReleaseIteration.CompareTo( other.ReleaseIteration );
            return ret;
        }

        #endregion

        #region IEquatable<CswSchemaVersion> Members

        public static bool operator ==( CswSchemaVersion version1, CswSchemaVersion version2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( version1, version2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) version1 == null ) || ( (object) version2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( version1.CycleIteration == version2.CycleIteration &&
                version1.ReleaseIdentifier == version2.ReleaseIdentifier &&
                version1.ReleaseIteration == version2.ReleaseIteration )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=( CswSchemaVersion version1, CswSchemaVersion version2 )
        {
            return !( version1 == version2 );
        }

        public override bool Equals( object obj )
        {
            if( !( obj is CswSchemaVersion ) )
                return false;
            return this == (CswSchemaVersion) obj;
        }

        public bool Equals( CswSchemaVersion obj )
        {
            return this == (CswSchemaVersion) obj;
        }

        public override int GetHashCode()
        {                                                            // For 01A-16:
            return this.CycleIteration * 100000 +                    // 100000
                   Convert.ToInt16( this.ReleaseIdentifier ) * 100 +   // 106500
                   this.ReleaseIteration;                            // 106516
        }

        #endregion

    }
}
