using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemSW.Nbt
{
    public class CswSequenceName
    {
        public string DisplayName;
        public string DBName
        {
            get
            {
                return DisplayName.Replace( " ", string.Empty );
            }
        }

        public CswSequenceName( string inDisplayName )
        {
            DisplayName = inDisplayName;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
