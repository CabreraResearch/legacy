using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerStorageTemperatures
    {
        public const string RoomTemperature = "4 = Room Temperature";
        public const string GreaterThanRoomTemp = "5 = Greater than room temp.";
        public const string LessThanRoomTemp = "6 = Less than room temp.";
        public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { RoomTemperature, GreaterThanRoomTemp, LessThanRoomTemp };
    }

}//namespace ChemSW.Nbt.ObjClasses
