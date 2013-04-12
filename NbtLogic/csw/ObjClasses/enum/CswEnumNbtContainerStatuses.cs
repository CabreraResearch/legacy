using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public sealed class CswEnumNbtContainerStatuses
    {
        public const string LabUseOnly = "Lab Use Only";
        public const string Central = "Central";
        public const string Approved = "Approved";
        public const string OutOfCompliance = "Out of Compliance";
        public const string Rejected = "Rejected";
        public const string Expired = "Expired";

        public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    LabUseOnly,
                    Central,
                    Approved,
                    OutOfCompliance,
                    Rejected,
                    Expired
                };
    }

}//namespace ChemSW.Nbt.ObjClasses
