using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Keeps the schema up-to-date
    /// </summary>
    public class CswSchemaScriptsKatsura : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateDDL_02K_CaseXXXXX()
                    new CswUpdateDDL_02K_Case29311_AddRelationalColumn(),
                    new CswUpdateDDL_02K_Case31616(),
                    new CswUpdateDDL_02K_Case31517(),
                    new CswUpdateDDL_02K_Case31783()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateMetaData_02K_CaseXXXXX()
                    new CswUpdateMetaData_02K_Case31616B(),
                    new CswUpdateMetaData_02K_Case29311(),
                    new CswUpdateMetaData_02K_Case10480(),
                    new CswUpdateMetaData_02K_Case31542A(),
                    new CswUpdateMetaData_02K_Case31542B(),
                    new CswUpdateMetaData_02K_Case31672(),
                    new CswUpdateMetaData_02K_Case31545(),
                    new CswUpdateMetaData_02K_Case31517B(),
                    new CswUpdateMetaData_02K_Case31585(),
                    new CswUpdateMetaData_02K_Case31505(),
                    new CswUpdateMetaData_02K_Case31798(),
                    new CswUpdateMetaData_02K_Case31749()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>()
                {
                    // new CswUpdateSchema_02K_CaseXXXXX()
                    new CswUpdateSchema_02K_Case29311_Design(),
                    new CswUpdateSchema_02K_Case29311_Sequences(),
                    new CswUpdateSchema_02K_Case29311_Fixes(),
                    new CswUpdateSchema_02K_Case29311_DefaultValue(),
                    new CswUpdateSchema_02K_Case29311_MoreFixes(),
                    new CswUpdateSchema_02K_Case29314(),
                    new CswUpdateSchema_02K_Case31396(),
                    new CswUpdateSchema_02K_Case6780(),
                    new CswUpdateSchema_02K_Case31545(),
                    new CswUpdateSchema_02K_Case31584(),
                    new CswUpdateSchema_02K_Case31509(),
                    new CswUpdateSchema_02K_Case31530(),
                    new CswUpdateSchema_02K_Case31501(),
                    new CswUpdateSchema_02K_Case31753(),
                    new CswUpdateSchema_02K_Case31416(),
                    new CswUpdateSchema_02K_Case31406(),
                    new CswUpdateSchema_02K_Case31329(),
                    new CswUpdateSchema_02K_Case31375(),
                    new CswUpdateSchema_02K_Case31801(),
                    new CswUpdateSchema_02K_Case31192(),
                    new CswUpdateSchema_02K_Case31829(),
                    new CswUpdateSchema_02K_Case31329B(),
                    new CswUpdateSchema_02K_Case31783(),
                    new CswUpdateSchema_02K_Case31783B(),
                    new CswUpdateSchema_02K_Case31783C()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsJuniper
}//namespace ChemSW.Nbt.Schema