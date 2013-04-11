using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerRequestMenu
    {
        public const string Move = "Request Move";
        public const string Dispose = "Request Dispose";
        public const string Dispense = "Request Dispense";

        public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Move,
                    Dispose,
                    Dispense
                };

    }
}//namespace ChemSW.Nbt.ObjClasses
