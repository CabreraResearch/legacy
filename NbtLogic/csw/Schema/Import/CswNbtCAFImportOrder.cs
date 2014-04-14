using System;
using System.Collections.Generic;

namespace ChemSW.Nbt.csw.Schema.Import
{
    public sealed class CswNbtCAFImportOrder
    {
        /// <summary>
        /// The ordering of CAF imported nodetypes. Used to make ordering imports convenient for CAFimportOrder.
        /// </summary>
        public static readonly Dictionary<string, Int32> CAFOrder = new Dictionary<string, int>
            {
                //{Nodetype, Order}
                {"Control Zone", 1},
                {"Work Unit", 2},
                {"Inventory Group", 3},
                {"Site", 4},
                {"Building", 5},
                {"Room", 6},
                {"Cabinet", 7},
                {"Shelf", 8},
                {"Box", 9},
                {"Vendor", 10},
                {"Role", 11},
                {"User", 12},
                {"Regulatory List", 13},
                {"Regulatory List CAS", 14},
                {"Unit_Weight", 15},
                {"Unit_Volume", 16},
                {"Unit_Each", 17},
                {"DSD Phrase", 18}, //DSD Phrases
                {"Constituent", 19},
                {"Chemical", 20},
                {"Supply", 21},
                {"Biological", 22},
                {"Material Component", 23},
                {"Size", 24},
                {"SDS Document", 25},
                {"Material Document", 26},
                {"Receipt Lot", 27},
                {"C of A Document", 28},
                {"Container Group", 29},
                {"Container", 30},
                {"Inventory Level", 31},
                {"Jurisdiction", 32},
                {"GHS Phrase", 33},
                {"GHS", 34},
                {"Material Synonym", 35},
                {"Equipment Type", 36},
                {"Equipment", 37}
            };
    }
}
