
using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Schema;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.csw.ImportExport
{
    public static class CswNbtImportTools
    {
        public static void CreateAllCAFProps( CswNbtResources NbtResources )
        {
            CreateCafProps( NbtResources, CswEnumNbtObjectClass.ChemicalClass, "properties_values", "propertiesvaluesid" );
            CreateCafProps( NbtResources, CswEnumNbtObjectClass.ContainerClass, "properties_values_cont", "contpropsvaluesid" );
            CreateCafProps( NbtResources, CswEnumNbtObjectClass.ContainerClass, "properties_values_lot", "lotpropsvaluesid" );
        }

        public static void CreateCafProps( CswNbtResources NbtResources, CswEnumNbtObjectClass ObjClass, string PropsValsTblName, string PropsValsPKName )
        {
            CswNbtSchemaUpdateImportMgr ImpMgr = new CswNbtSchemaUpdateImportMgr( new CswNbtSchemaModTrnsctn( NbtResources ), "CAF" );

            CswNbtMetaDataObjectClass MetaDataObjClass = NbtResources.MetaData.getObjectClass( ObjClass );
            string sql = GetCAFPropertiesSQL( PropsValsTblName );
            CswArbitrarySelect cafChemPropAS = NbtResources.makeCswArbitrarySelect( "cafProps_" + ObjClass.Value, sql );
            DataTable cafChemPropsDT = cafChemPropAS.getTable();

            foreach( DataRow row in cafChemPropsDT.Rows )
            {
                foreach( CswNbtMetaDataNodeType NodeType in MetaDataObjClass.getNodeTypes() )
                {
                    string PropName = row["propertyname"].ToString();
                    int PropId = CswConvert.ToInt32( row["propertyid"] );
                    PropName = GetUniquePropName( NodeType, PropName ); //keep appending numbers until we have a unique prop name

                    CswEnumNbtFieldType propFT = GetFieldTypeFromCAFPropTypeCode( row["propertytype"].ToString() );

                    CswNbtMetaDataNodeTypeProp newProp = NbtResources.MetaData.makeNewProp( NodeType, propFT, PropName, Int32.MinValue );
                    newProp.IsRequired = CswConvert.ToBoolean( row["required"] );
                    newProp.ReadOnly = CswConvert.ToBoolean( row["readonly"] );
                    newProp.ListOptions = row["listopts"].ToString();
                    newProp.removeFromAllLayouts();

                    string cafColPropName = "prop" + row["propertyid"];
                    string cafSourceCol = "propvaltext";
                    if( CswEnumNbtFieldType.DateTime == propFT )
                    {
                        cafSourceCol = "propvaldate";
                    }
                    else if( CswEnumNbtFieldType.Number == propFT )
                    {
                        cafSourceCol = "propvalnumber";
                    }

                    ImpMgr.importBinding( cafSourceCol, PropName, "", "CAF", NodeType.NodeTypeName,
                        ClobTableName : PropsValsTblName,
                        LobDataPkColOverride : cafColPropName,
                        LobDataPkColName : PropsValsPKName,
                        LegacyPropId : PropId );
                }
            }

            ImpMgr.finalize();
        }

        /// <summary>
        /// Get all the custom properties in a CAF schema
        /// </summary>
        private static string GetCAFPropertiesSQL( string propValsTblName )
        {
            string sql = @"select distinct p.propertyid,
                             p.propertyname,
                             p.propertytype,
                             p.required, 
                             p.readonly,
                             (select listopts from (select propertyid, listagg(replace(lkpitem, ',', ''), ',') within group (order by lkpitem) as listopts
                                   from properties_list_lkps@caflink
                                        group by propertyid) where propertyid = p.propertyid) listopts
                            from properties@caflink p
                                   join " + propValsTblName + @"@caflink pv on p.propertyid = pv.propertyid
                             where p.propertyid not in (select legacypropid from import_def_bindings idb
                                         join properties@caflink p on p.propertyid = idb.legacypropid)
                            order by propertyid";

            return sql;
        }

        /// <summary>
        /// Get a unique NTP name for a given NT
        /// </summary>
        private static string GetUniquePropName( CswNbtMetaDataNodeType NodeType, string PropName )
        {
            bool isUnique = true;
            int idx = 1;
            string OrigPropName = PropName;
            while( isUnique )
            {
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( PropName );
                if( null != ntp )
                {
                    PropName = OrigPropName + idx;
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
        private static CswEnumNbtFieldType GetFieldTypeFromCAFPropTypeCode( string CafPropTypeCode )
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
