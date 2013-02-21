using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Core;

namespace ChemSW.Nbt.ImportExport
{
    public class CswImporterLocationTree
    {
        private readonly Collection<LocationEntry> _RootLevel;
        private readonly Collection<LocationEntry> _All;

        public CswImporterLocationTree()
        {
            _RootLevel = new Collection<LocationEntry>();
            _All = new Collection<LocationEntry>();
        }

        /// <summary>
        /// Conditionally adds any missing part of the path submitted
        /// </summary>
        public void AddPath( CswCommaDelimitedString Path )
        {
            if( null != Path && Path.Count > 0 )
            {
                // Local destructable copy
                CswCommaDelimitedString myPath = new CswCommaDelimitedString();
                myPath.FromString( Path.ToString() );

                _AddPathRecursive( myPath, _RootLevel, 1, null );
            }
        }

        private void _AddPathRecursive( CswCommaDelimitedString Path, Collection<LocationEntry> Level, Int32 LevelNo, LocationEntry Parent )
        {
            if( Path.Count > 0 )
            {
                string ThisName = Path.Pop().Trim();
                LocationEntry Entry = Level.FirstOrDefault( e => e.Name.ToLower() == ThisName.ToLower() );
                if( null == Entry )
                {
                    Entry = new LocationEntry()
                        {
                            Name = ThisName,
                            Level = LevelNo,
                            Parent = Parent
                        };
                    Level.Add( Entry );
                    _All.Add( Entry );
                }

                _AddPathRecursive( Path, Entry.Children, Entry.Level + 1 , Entry);
            }
        }

        /// <summary>
        /// Returns LocationEntry, or null if path not found
        /// </summary>
        public LocationEntry FindPath( CswCommaDelimitedString Path )
        {
            LocationEntry ret = null;
            if( null != Path && Path.Count > 0 )
            {
                // Local destructable copy
                CswCommaDelimitedString myPath = new CswCommaDelimitedString();
                myPath.FromString( Path.ToString() );

                ret = _FindPathRecursive( myPath, _RootLevel );
            }
            return ret;
        }

        private LocationEntry _FindPathRecursive( CswCommaDelimitedString Path, Collection<LocationEntry> Level )
        {
            LocationEntry ret = null;
            if( Path.Count > 0 )
            {
                string ThisName = Path.Pop();
                LocationEntry Entry = Level.FirstOrDefault( e => e.Name.ToLower() == ThisName.ToLower() );
                if( null != Entry )
                {
                    if( Path.Count > 0 )
                    {
                        ret = _FindPathRecursive( Path, Entry.Children );
                    }
                    else
                    {
                        ret = Entry;
                    }
                }
            }
            return ret;
        }

        public IOrderedEnumerable<LocationEntry> BreadthFirst()
        {
            return _All.OrderBy( e => e.Level );
        }

        public class LocationEntry
        {
            public string Name = string.Empty;
            public LocationEntry Parent = null;
            public Int32 Level = Int32.MinValue;
            public Int32 ImportNodeId = Int32.MinValue;
            public Collection<LocationEntry> Children = new Collection<LocationEntry>();

            public override string ToString()
            {
                return Name;
            }
        }

    } // class CswImporterLocationTree

} // namespace ChemSW.Nbt.ImportExport