using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// List of schema scripts for the labeled milestone
    /// </summary>
    public class CswSchemaScriptsMagnolia : ICswSchemaScripts
    {
        public Collection<CswUpdateSchemaTo> _DDLScripts()
        {
            return new Collection<CswUpdateSchemaTo>
                {
                    // new CswUpdateDDL_02M_CISXXXXX(),
                    new CswUpdateDDL_02M_CIS53175(),
                    new CswUpdateDDL_02M_CIS52735()
                };
        } // _DDLScripts()

        public Collection<CswUpdateSchemaTo> _MetaDataScripts()
        {
            return new Collection<CswUpdateSchemaTo>
                {
                // new CswUpdateMetaData_02M_CISXXXXX(),
                   new CswUpdateMetaData_02M_CIS52307(),
                   new CswUpdateMetaData_02M_CIS53175(),
                   new CswUpdateMetaData_02M_CIS52735(),
                   new CswUpdateMetaData_02M_Case52300(),
                   new CswUpdateMetaData_02M_Case52302(),
                   new CswUpdateMetaData_02M_Case52308(),
                   new CswUpdateMetaData_02M_Case52309(),
                   new CswUpdateMetaData_02M_CIS52312(),
                   new CswUpdateMetaData_02M_CIS53197(),
                   new CswUpdateMetaData_02M_CIS52282(),
                   new CswUpdateMetaData_02M_CIS52301(),
                   new CswUpdateMetaData_02M_CIS53135(),
                   new CswUpdateMetaData_02M_CIS51775(),
                   new CswUpdateMetaData_02M_CIS52286(),
                   new CswUpdateMetaData_02M_CIS52298A(),
                   new CswUpdateMetaData_02M_CIS53248(),
                   new CswUpdateMetaData_02M_CIS53120()
                };
        } // _MetaDataScripts()

        public Collection<CswUpdateSchemaTo> _SchemaScripts()
        {
            return new Collection<CswUpdateSchemaTo>
                {
                // new CswUpdateSchema_02M_CISXXXXX(),
                   new CswUpdateSchema_02M_Case52354(),
                   new CswUpdateSchema_02M_CIS53115(),
                   new CswUpdateSchema_02M_CIS52316(),
                   new CswUpdateSchema_02M_CIS52307(),
                   new CswUpdateSchema_02M_CIS53189(), // must be before 52670
                   new CswUpdateSchema_02M_CIS52670(),
                   new CswUpdateSchema_02M_CIS53123(),
                   new CswUpdateSchema_02M_CIS52772(),
                   new CswUpdateSchema_02M_CIS52751(),
                   new CswUpdateSchema_02M_CIS52735(),
                   new CswUpdateSchema_02M_CIS52799(),
                   new CswUpdateSchema_02M_CIS53135(),
                   new CswUpdateSchema_02M_CIS52301B(),
                   new CswUpdateSchema_02M_CIS52993(),
                   new CswUpdateSchema_02M_CIS51775B(),
                   new CswUpdateSchema_02M_CIS52789(),
                   new CswUpdateSchema_02M_CIS52298B(),
                   new CswUpdateSchema_02M_CIS53248(),
                   new CswUpdateSchema_02M_CIS53357()
                };
        } // _SchemaScripts()

    }//class CswSchemaScriptsMagnolia
}//namespace ChemSW.Nbt.Schema