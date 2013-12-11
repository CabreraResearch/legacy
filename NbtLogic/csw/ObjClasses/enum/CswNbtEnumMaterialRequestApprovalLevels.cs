
using ChemSW.Core;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswEnumNbtMaterialRequestApprovalLevel
    {
        public static CswCommaDelimitedString All
        {
            get
            {
                return new CswCommaDelimitedString
                    {
                        Lvl1,
                        Lvl2,
                        Lvl3,
                        Lvl4,
                        Lvl5,
                        Lvl6
                    };
            }
        }

        public const string Lvl1 = "Level 1";
        public const string Lvl2 = "Level 2";
        public const string Lvl3 = "Level 3";
        public const string Lvl4 = "Level 4";
        public const string Lvl5 = "Level 5";
        public const string Lvl6 = "Level 6";
    }
}
