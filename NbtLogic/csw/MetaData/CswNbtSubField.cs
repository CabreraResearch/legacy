using System.Collections.ObjectModel;
using System.Linq;

namespace ChemSW.Nbt.MetaData
{
    public class CswNbtSubField
    {
        public CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Value;
        public string Table = string.Empty;
        public CswEnumNbtPropColumn Column = CswEnumNbtPropColumn.Unknown;
        //public string RelationalTable = string.Empty;
        //public string RelationalColumn = string.Empty;
        private CswNbtFieldResources _CswNbtFieldResources;
        public bool isReportable;

        public CswNbtSubField( CswNbtFieldResources CswNbtFieldResources, CswEnumNbtPropColumn DefaultColumn, CswEnumNbtSubFieldName SubFieldName, bool Reportable = false )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            Name = SubFieldName;
            Table = "jct_nodes_props";  // default
            Column = DefaultColumn;
            isReportable = Reportable;
        }

        public Collection<CswEnumNbtFilterMode> _FilterModes = new Collection<CswEnumNbtFilterMode>();
        public CswEnumNbtFilterMode DefaultFilterMode
        {
            get
            {
                return SupportedFilterModes.First(); //CswEnumNbtFilterMode.Begins;      
            }
        }

        public Collection<CswEnumNbtFilterMode> SupportedFilterModes
        {
            get
            {
                return _FilterModes;
            }//get

        }//FilterModes

        public string ToXmlNodeName( bool ToLower = false )
        {
            // case 20371 - In the NBT property importer, need to distinguish between NodeID (the subfield) and nodeid (the pk column)
            //return this.Name.ToString().ToLower();
            string ret = this.Name.ToString();
            if( ToLower )
            {
                ret = ret.ToLower();
            }
            return ret;
        }

    }//CswNbtSubField

}//namespace ChemSW.Nbt.MetaData
