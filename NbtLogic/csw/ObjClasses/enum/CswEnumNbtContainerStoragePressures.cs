using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerStoragePressures
    {
        public const string Atmospheric = "1 = Atmospheric";
        public const string Pressurized = "2 = Pressurized";
        public const string Subatmospheric = "3 = Subatmospheric";
        public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Atmospheric, Pressurized, Subatmospheric };
    }

}//namespace ChemSW.Nbt.ObjClasses
