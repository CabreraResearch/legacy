using System;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01: CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        #region Blame Logic

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        #endregion Blame Logic

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            #region ASPEN

            _createNodeCountColumns( CswDeveloper.MB, 28355 );
            _createLoginDataTable( CswDeveloper.BV, 27906 );
            _addViewIsSystemColumn( CswDeveloper.BV, 28890 );
            _fixKioskModeName( CswDeveloper.MB, 29274 );

            #endregion ASPEN

            #region BUCKEYE

            _propSetTable(CswDeveloper.SS, 28160 );

            #endregion BUCKEYE

        }//Update()

        #region ASPEN

        private void _createNodeCountColumns( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string NodeCountColName = "nodecount";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", NodeCountColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "nodetypes", NodeCountColName, "The number of nodes", false, false );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class", NodeCountColName ) )
            {
                _CswNbtSchemaModTrnsctn.addLongColumn( "object_class", NodeCountColName, "The number of nodes", false, false );
            }

            _resetBlame();
        }

        private void _createLoginDataTable( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            const string LoginDataTableName = "login_data";

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( LoginDataTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( LoginDataTableName, "loginid" );
                _CswNbtSchemaModTrnsctn.getNewPrimeKey( LoginDataTableName );
            }
            if( _CswNbtSchemaModTrnsctn.isTableDefined( LoginDataTableName ) )
            {
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "username" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "username", "User's Username", false, false, 50 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "ipaddress" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "ipaddress", "User's IP Address", false, false, 30 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "logindate" ) )
                {
                    _CswNbtSchemaModTrnsctn.addDateColumn( LoginDataTableName, "logindate", "Date of Login Attempt", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "loginstatus" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "loginstatus", "Status of Login Attempt", false, false, 50 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "failurereason" ) )
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn( LoginDataTableName, "failurereason", "Reason for Login Failure", false, false, 100 );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( LoginDataTableName, "failedlogincount" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( LoginDataTableName, "failedlogincount", "Number of times user login has failed this session", false, false );
                }
            }

            _resetBlame();
        }

        private void _addViewIsSystemColumn( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string IsSystemColumnName = "issystem";
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "node_views", IsSystemColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "node_views", IsSystemColumnName, "When set to true, only ChemSWAdmin can edit this view", false, false );
            }

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "node_views_audit", IsSystemColumnName ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "node_views_audit", IsSystemColumnName, "When set to true, only ChemSWAdmin can edit this view", false, false );
            }

            _resetBlame();
        }

        private void _fixKioskModeName( CswDeveloper Dev, Int32 CaseNo )
        {
            if( null == _CswNbtSchemaModTrnsctn.Actions[CswNbtActionName.Kiosk_Mode] )
            {
                _CswNbtSchemaModTrnsctn.execArbitraryPlatformNeutralSql( "update actions set actionname = 'Kiosk Mode' where actionname = 'KioskMode'" );
            }
        }

        #endregion ASPEN

        #region BUCKEYE Methods

        private void _propSetTable( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );

            string PropSetTableName = "property_set";
            string PropSetPkName = "propertysetid";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( PropSetTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( PropSetTableName, PropSetPkName );
                _CswNbtSchemaModTrnsctn.addStringColumn( PropSetTableName, "name", "Name of property set", false, false, 50 );
                _CswNbtSchemaModTrnsctn.addStringColumn( PropSetTableName, "iconfilename", "Icon for property set", false, false, 50 );
            }

            string JctPsOcTableName = "jct_propertyset_objectclass";
            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( JctPsOcTableName ) )
            {
                _CswNbtSchemaModTrnsctn.addTable( JctPsOcTableName, "jctpropsetobjclassid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( JctPsOcTableName, "objectclassid", "Object class foreign key", false, true, "object_class", "objectclassid" );
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn( JctPsOcTableName, PropSetPkName, "Property Set foreign key", false, true, PropSetTableName, PropSetPkName );
            }

            _resetBlame();
        }

        #endregion BUCKEYE Methods

    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


