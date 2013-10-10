using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// CAF scripts
    /// </summary>
    public class CswSchemaScriptsCAF : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                };
                    
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02F_Case30041_NbtImportQueue(), //Validate the Nbt Import Queue table first

                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateSchema_02F_Case30041_Vendors(),
                    new CswUpdateSchema_02F_Case30041_RegLists(),
                    new CswUpdateSchema_02F_Case30041_UnitsOfMeasure(),
                    new CswUpdateSchema_02F_Case30041_RolesUsers(),
                    new CswUpdateSchema_02F_Case30043_ControlZones(),
                    new CswUpdateSchema_02F_Case30043_Locations(),
                    new CswUpdateSchema_02F_Case30043_WorkUnits(),
                    new CswUpdateSchema_02F_Case30043_InventoryGroups(),
                    new CswUpdateSchema_02G_Case30743_Materials(),
                    new CswUpdateSchema_02G_Case30744_PackDetail(),
                    new CswUpdateSchema_02G_Case30047_SDS(),
                    new CswUpdateSchema_02G_Case30047_Docs(),
                    new CswUpdateSchema_02H_Case30046_Containers(),
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsCAF
}//namespace ChemSW.Nbt.Schema