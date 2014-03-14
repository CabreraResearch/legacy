using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsLarch : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02L_CaseXXXXX()
                    new CswUpdateDDL_02L_Case31907(),
                    new CswUpdateDDL_02L_Case52544(),
                    new CswUpdateDDL_02L_Case52562()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateMetaData_02L_Case29446A(),
                    new CswUpdateMetaData_02L_Case31750(),
                    new CswUpdateMetaData_02L_Case31893A(),
                    new CswUpdateMetaData_02L_Case31893B(),
                    new CswUpdateMetaData_02L_Case52281(),
                    new CswUpdateMetaData_02L_Case52284(),
                    new CswUpdateMetaData_02L_Case52285(),
                    new CswUpdateMetaData_02L_Case51743A(),
                    new CswUpdateMetaData_02L_Case52017(),
                    new CswUpdateMetaData_02L_Case52017B(),
                    new CswUpdateMetaData_02L_Case52280(),
                    new CswUpdateMetaData_02L_Case52280B(),
                    new CswUpdateMetaData_02L_Case52280C()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    new CswUpdateSchema_02L_Case29446B(),
                    new CswUpdateSchema_02L_Case31907(),
                    new CswUpdateSchema_02L_Case32003(),
                    new CswUpdateSchema_02L_Case52761(),
                    new CswUpdateSchema_02L_Case31611(),
                    new CswUpdateSchema_02L_Case31893_Biologicals(),
                    new CswUpdateSchema_02L_Case31893_Supplies(),
                    new CswUpdateSchema_02L_Case31893_Constituents(),
                    new CswUpdateSchema_02L_Case31893_MaterialComps(),
                    new CswUpdateSchema_02L_Case31893(),
                    new CswUpdateSchema_02L_Case52266(),
                    new CswUpdateSchema_02L_Case52446(),
                    new CswUpdateSchema_02L_Case51743B(),
                    new CswUpdateSchema_02L_Case53015(), //fix for 52285, must run before it
                    new CswUpdateSchema_02L_Case52285(),
                    new CswUpdateSchema_02L_Case52280(),
                    new CswUpdateSchema_02L_Case52281(),
                    new CswUpdateSchema_02L_Case52281(),
                    new CswUpdateSchema_02L_Case52786(),
                    new CswUpdateSchema_02L_Case52562()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsLarch
}//namespace ChemSW.Nbt.Schema