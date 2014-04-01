using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Sched;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52735: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52735; }
        }

        public override string Title
        {
            get { return "Move mol clob data to mol_data table; Rename Fingerprints Sched Rule to 'MolData'"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            //Rename sched rule
            CswTableUpdate schedRulesTU = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "RenameFingerprintRule", "scheduledrules" );
            DataTable schedRules = schedRulesTU.getTable( "where rulename = 'MolFingerprints'" );
            if( schedRules.Rows.Count > 0 )
            {
                schedRules.Rows[0]["rulename"] = CswEnumNbtScheduleRuleNames.MolData.ToString();
            }
            schedRulesTU.update( schedRules );

            //Get all Mol NTP ids
            CswCommaDelimitedString molNTPs = new CswCommaDelimitedString();
            foreach( CswNbtMetaDataNodeTypeProp molNTP in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswEnumNbtFieldType.MOL ) )
            {
                molNTPs.Add( molNTP.PropId.ToString() );
            }

            CswTableUpdate molDataUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "moldataUpdate_52735", "mol_data" );
            DataTable molDataTbl = molDataUpdate.getEmptyTable();

            CswTableUpdate molDataPropsTblUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "molselect_52735", "jct_nodes_props" );
            DataTable propsWithMols = molDataPropsTblUpdate.getTable( "where nodetypepropid in (" + molNTPs.ToString() + ") and clobdata is not null" );
            foreach( DataRow molPropRow in propsWithMols.Rows )
            {
                string molString = molPropRow["clobdata"].ToString();
                int jctnodepropid = CswConvert.ToInt32( molPropRow["jctnodepropid"] );

                DataRow newMolDataTblRow = molDataTbl.NewRow();
                newMolDataTblRow["jctnodepropid"] = jctnodepropid;
                newMolDataTblRow["originalmol"] = Encoding.UTF8.GetBytes( molString );
                newMolDataTblRow["contenttype"] = ".mol";
                newMolDataTblRow["nodeid"] = molPropRow["nodeid"];
                molDataTbl.Rows.Add( newMolDataTblRow );

                molPropRow["clobdata"] = string.Empty;
                molPropRow["field1"] = "1";
            }

            molDataUpdate.update( molDataTbl );
            molDataPropsTblUpdate.update( propsWithMols );
        }
    }
}