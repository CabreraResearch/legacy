using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02L_Case31883: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31883; }
        }

        public override string Title
        {
            get { return "Remove Fingerprinter sched rule, drop mol keys table, delete existing Mol image blobs"; }
        }

        public override void update()
        {
            //Remove fingerprint sched rule
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "delete from scheduledrules where rulename = 'MolFingerprints'" );

            //Drop mol keys table
            if( _CswNbtSchemaModTrnsctn.isTableDefined( "mol_keys" ) )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "drop table mol_keys" );
            }

            //Delete mol images from blob_data
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"delete from (select bd.* from blob_data bd
                                                                            	join jct_nodes_props jnp on jnp.jctnodepropid = bd.jctnodepropid
                                                                    			join nodetype_props ntp on jnp.nodetypepropid = ntp.nodetypepropid
                                                                    			join field_types ft on ft.fieldtypeid = ntp.fieldtypeid
                                                                    	where ft.fieldtype = 'MOL')");

        } // update()

    }

}//namespace ChemSW.Nbt.Schema