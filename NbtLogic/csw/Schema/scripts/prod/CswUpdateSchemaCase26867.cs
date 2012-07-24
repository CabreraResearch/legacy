
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchemaCase26867 : CswUpdateSchemaTo
    {
        public override void update()
        {

            //retroactively update the audit user name to the new standard name
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update audit_transactions set transactionusername = '" + ChemSW.Nbt.Security.SystemUserNames.SysUsr_SchemaUpdt.ToString() + "' where transactionusername='_SchemaUpdaterUser'" );

            //Set auditing defaults as per TDU :-( 
            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update nodetype_props
                                                                       set auditlevel = 'NoAudit'
                                                                     where nodetypeid in
                                                                           (select nodetypeid
                                                                              from nodetypes
                                                                             where nodetypename in
                                                                                   ('Aliquot', 'Container Dispense Transaction',
                                                                                    'Assembly Document', 'Container Document',
                                                                                    'Equipment Document', 'Material Document', 'Equipment Type',
                                                                                    'Feedback', 'Assembly Schedule', 'Equipment Schedule',
                                                                                    'Inspection Schedule', 'Department', 'GHS Phrase',
                                                                                    'Lab Safety (demo)', 'Lab Safety Group (demo)',
                                                                                    'Inventory Group', 'Mail Report', 'Notification', 'Parameter',
                                                                                    'Print Label', 'IMCS Report', 'Report', 'SI Report', 'Test',
                                                                                    'Unit (Time)', 'Work Unit', 'Equipment'))" );


            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update nodetypes
                                                                       set auditlevel = 'NoAudit'
                                                                     where nodetypename in
                                                                           ('Aliquot', 'Container Dispense Transaction', 'Assembly Document',
                                                                            'Container Document', 'Equipment Document', 'Material Document',
                                                                            'Equipment Type', 'Feedback', 'Assembly Schedule',
                                                                            'Equipment Schedule', 'Inspection Schedule', 'Department',
                                                                            'GHS Phrase', 'Lab Safety (demo)', 'Lab Safety Group (demo)',
                                                                            'Inventory Group', 'Mail Report', 'Notification', 'Parameter',
                                                                            'Print Label', 'IMCS Report', 'Report', 'SI Report', 'Test',
                                                                            'Unit (Time)', 'Work Unit', 'Equipment')" );


            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update nodetype_props
                                                                       set auditlevel = 'PlainAudit'
                                                                     where nodetypeid in
                                                                           (select nodetypeid
                                                                              from nodetypes
                                                                             where nodetypename not in
                                                                                   ('Aliquot', 'Container Dispense Transaction',
                                                                                    'Assembly Document', 'Container Document',
                                                                                    'Equipment Document', 'Material Document', 'Equipment Type',
                                                                                    'Feedback', 'Assembly Schedule', 'Equipment Schedule',
                                                                                    'Inspection Schedule', 'Department', 'GHS Phrase',
                                                                                    'Lab Safety (demo)', 'Lab Safety Group (demo)',
                                                                                    'Inventory Group', 'Mail Report', 'Notification', 'Parameter',
                                                                                    'Print Label', 'IMCS Report', 'Report', 'SI Report', 'Test',
                                                                                    'Unit (Time)', 'Work Unit', 'Equipment'))" );



            _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( @"update nodetypes
                                                                       set auditlevel = 'PlainAudit'
                                                                     where nodetypename not in
                                                                           ('Aliquot', 'Container Dispense Transaction', 'Assembly Document',
                                                                            'Container Document', 'Equipment Document', 'Material Document',
                                                                            'Equipment Type', 'Feedback', 'Assembly Schedule',
                                                                            'Equipment Schedule', 'Inspection Schedule', 'Department',
                                                                            'GHS Phrase', 'Lab Safety (demo)', 'Lab Safety Group (demo)',
                                                                            'Inventory Group', 'Mail Report', 'Notification', 'Parameter',
                                                                            'Print Label', 'IMCS Report', 'Report', 'SI Report', 'Test',
                                                                            'Unit (Time)', 'Work Unit', 'Equipment')" );


        }//Update()

    }//class CswUpdateSchemaCase26867

}//namespace ChemSW.Nbt.Schema