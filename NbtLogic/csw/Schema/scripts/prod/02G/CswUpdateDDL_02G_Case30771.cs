using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateDDL_02G_Case30771 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30771; }
        }

        public override string ScriptName
        {
            get { return "02G_Case30771"; }
        }

        public override string Title
        {
            get { return "Reorganize CAF Import Columns"; }
        }

        public override void update()
        {
            
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "TABLENAME" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORTDEF", "TABLENAME", "the source table for this sheet", false, false, 30 );
                //the correct value for this column for all Foxglove rows was the sheet name
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set tablename = sheetname where definitionname='CAF'" );
                
            }



            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "VIEWNAME" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF", "VIEWNAME", "the source view for this sheet", false, false, 30 );

                //the rows of IMPORT_DEF that could be added prior to this script must be filled in. Steve chose hardcoded update statements for the task
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'locations_view' where sheetname='locations' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'workunits_view' where sheetname='work_units' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'chemicals_view' where sheetname='packages' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'packdetail_view' where sheetname='packdetail' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'volume_view' where sheetname='volume_view' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'weight_view' where sheetname='weight_view' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set viewname = 'each_view' where sheetname='each_view' and definitionname='CAF'" );
            }



            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "IMPORT_DEF", "PKCOUMNNAME" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "IMPORT_DEF", "PKCOLUMNNAME", "the column of PKs stored in import queue", false, false, 30 );

                //the rows of IMPORT_DEF that could be added prior to this script must be filled in. Steve chose hardcoded update statements for the task
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'vendorid' where sheetname='vendors' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'locationid' where sheetname='locations' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'workunitid' where sheetname='work_units' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'inventorygroupid' where sheetname='inventory_groups' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'controlzoneid' where sheetname='cispro_controlzones' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'packageid' where sheetname='packages' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'packdetailid' where sheetname='packdetail' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'unitofmeasureid' where sheetname='volume_view' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'unitofmeasureid' where sheetname='weight_view' and definitionname='CAF'" );
                   _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update import_def set pkcolumnname = 'unitofmeasureid' where sheetname='each_view' and definitionname='CAF'" );

            }




        } // update()

    }

}//namespace ChemSW.Nbt.Schema