using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsFoxglove : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02F_Case30252(),
                    new CswUpdateMetaData_02F_Case30228(),
                    new CswUpdateMetaData_02F_Case30697(),
                    new CswUpdateMetaData_02F_Case30040()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02F_Case30041_NbtImportQueue(), //Validate the Nbt Import Queue table first
                    new CswUpdateMetaData_02F_Case30281(),
                    new CswUpdateMetaData_02F_Case30251(),
                    new CswUpdateMetaData_02F_Case30251B(),
                    new CswUpdateMetaData_02F_Case30082_UserCache(),
                    new CswUpdateMetaData_02F_Case27883(),
                    new CswUpdateMetaData_02F_Case29992(),
                    new CswUpdateMetaData_02F_Case30529()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateSchema_02F_Case30281(),
                    new CswUpdateSchema_02F_Case28998(),
                    new CswUpdateSchema_02F_Case29973(),
                    new CswUpdateSchema_02F_Case29191(),
                    new CswUpdateSchema_02F_Case29542(),
                    new CswUpdateSchema_02F_Case29438(),
                    new CswUpdateSchema_02F_Case30082_UserCache(),
                    new CswUpdateSchema_02F_Case30197(),
                    new CswUpdateSchema_02F_Case30417(),
                    new CswUpdateSchema_02F_Case27883(),
                    new CswUpdateSchema_02F_Case27495(),
                    new CswUpdateSchema_02F_Case30228(),
                    new CswUpdateSchema_02F_Case30040(),
                    new CswUpdateSchema_02F_Case30041_Vendors(),
                    new CswUpdateSchema_02F_Case29992(),
                    new CswUpdateSchema_02F_Case29402(),
                    new CswUpdateSchema_02F_Case30041_UnitsOfMeasure(),
                    new CswUpdateSchema_02F_Case30041_RolesUsers(),
                    new CswUpdateSchema_02F_Case30252(),
                    new CswUpdateSchema_02F_Case30041_ScheduledRuleImport(),
                    new CswUpdateSchema_02F_Case30043_ControlZones(),
                    new CswUpdateSchema_02F_Case29984(),
                    new CswUpdateSchema_02F_Case30577(),
                    new CswUpdateSchema_02F_Case30043_Locations(),
                    new CswUpdateSchema_02F_Case30043_WorkUnits(),
                    new CswUpdateSchema_02F_Case30043_InventoryGroups(),
                    new CswUpdateSchema_02F_Case30647(),
                    new CswUpdateSchema_02F_Case30661(),
                    new CswUpdateSchema_02F_Case30706(),
                    new CswUpdateSchema_02F_Case30700(),
                    new CswUpdateSchema_02F_Case29562()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsFoxglove
}//namespace ChemSW.Nbt.Schema
