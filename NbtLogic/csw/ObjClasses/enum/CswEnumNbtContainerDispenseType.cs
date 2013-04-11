using System.Collections.Generic;
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{

    public sealed class CswEnumNbtContainerDispenseType : CswEnum<CswEnumNbtContainerDispenseType>
    {
        private CswEnumNbtContainerDispenseType( string Name ) : base( Name ) { }
        public static IEnumerable<CswEnumNbtContainerDispenseType> _All { get { return All; } }
        public static implicit operator CswEnumNbtContainerDispenseType( string str )
        {
            CswEnumNbtContainerDispenseType ret = Parse( str );
            return ret ?? Unknown;
        }
        public static readonly CswEnumNbtContainerDispenseType Unknown = new CswEnumNbtContainerDispenseType( "Unknown" );
        /// <summary>
        /// Add new (child) containers with material specified in existing source container (no parent container)
        /// </summary>
        public static readonly CswEnumNbtContainerDispenseType Receive = new CswEnumNbtContainerDispenseType( "Receive" );

        /// <summary>
        /// Transfer material from a source (parent) container to zero or more destination (child) containers
        /// </summary>
        public static readonly CswEnumNbtContainerDispenseType Dispense = new CswEnumNbtContainerDispenseType( "Dispense" );

        /// <summary>
        /// Transfer material from a source (parent) container to an undocumented location (no child containers)
        /// </summary>
        public static readonly CswEnumNbtContainerDispenseType Waste = new CswEnumNbtContainerDispenseType( "Waste" );

        /// <summary>
        /// Empty material from a source (parent) container and mark as disposed (no child containers)
        /// </summary>
        public static readonly CswEnumNbtContainerDispenseType Dispose = new CswEnumNbtContainerDispenseType( "Dispose" );

        /// <summary>
        /// Add material to an existing source container (no parent container, no child containers)
        /// </summary>
        public static readonly CswEnumNbtContainerDispenseType Add = new CswEnumNbtContainerDispenseType( "Add" );

    }
}//namespace ChemSW.Nbt.ObjClasses

