using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerUseTypes
    {
        public const string Storage = "Storage";
        public const string Closed = "Use Closed";
        public const string Open = "Use Open";
        public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Storage, Closed, Open };
    }

}//namespace ChemSW.Nbt.ObjClasses
