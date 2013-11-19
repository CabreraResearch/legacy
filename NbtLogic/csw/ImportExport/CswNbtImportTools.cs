
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.csw.ImportExport
{
    public static class CswNbtImportTools
    {

        /// <summary>
        /// Get all the custom properties in a CAF schema
        /// </summary>
        public static string GetCAFPropertiesSQL( string propValsTblName )
        {
            string sql = @"select distinct p.propertyid,
                             p.propertyname,
                             p.propertytype,
                             p.required, 
                             p.readonly,
                             (select listopts from (select propertyid, listagg(replace(lkpitem, ',', ''), ','), ',') within group (order by lkpitem) as listopts
                                   from properties_list_lkps@caflink
                                        group by propertyid) where propertyid = p.propertyid) listopts
                            from properties@caflink p
                                   join " + propValsTblName + @"@caflink pv on p.propertyid = pv.propertyid
                            order by propertyid";

            return sql;
        }

        /// <summary>
        /// Get a unique NTP name for a given NT
        /// </summary>
        public static string GetUniquePropName( CswNbtMetaDataNodeType NodeType, string PropName )
        {
            bool isUnique = true;
            int idx = 1;
            while( isUnique )
            {
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( PropName );
                if( null != ntp )
                {
                    PropName = PropName + idx;
                    idx++;
                }
                else
                {
                    isUnique = false;
                }
            }
            return PropName;
        }

        /// <summary>
        /// Get the NBT FieldType equivalent for a CAF custom prop code
        /// </summary>
        public static CswEnumNbtFieldType GetFieldTypeFromCAFPropTypeCode( string CafPropTypeCode )
        {
            CswEnumNbtFieldType Ret = null;

            switch( CafPropTypeCode )
            {
                case "T":
                    Ret = CswEnumNbtFieldType.Text;
                    break;
                case "L":
                    Ret = CswEnumNbtFieldType.List;
                    break;
                case "M": //Multi-List
                    Ret = CswEnumNbtFieldType.MultiList;
                    break;
                case "D":
                    Ret = CswEnumNbtFieldType.DateTime;
                    break;
                case "V": //Multi-Value
                    Ret = CswEnumNbtFieldType.MultiList;
                    break;
                case "E":
                    Ret = CswEnumNbtFieldType.Memo;
                    break;
                case "O":
                    Ret = CswEnumNbtFieldType.Logical;
                    break;
                case "N":
                    Ret = CswEnumNbtFieldType.Number;
                    break;
                case "Q":
                    //Query - we don't import this prop
                    break;
            }

            return Ret;
        }

    }
}
