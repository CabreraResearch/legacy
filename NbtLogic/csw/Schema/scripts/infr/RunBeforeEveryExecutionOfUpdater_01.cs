
using System;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: DDL";

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo = 0;

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

        public override void update()
        {
            // This script is for changes to schema structure,
            // or other changes that must take place before any other schema script.

            // NOTE: This script will be run many times, so make sure your changes are safe!

            #region SEBASTIAN

            //Add 5 generic nodetype prop attribute columns
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute1" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute1", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute2" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute2", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute3" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute3", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute4" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute4", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "attribute5" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodetype_props", "attribute5", "Generic nodetype prop attribute col", false, false, 100 );
            }
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetypes", "haslabel" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetypes", "haslabel", "Indicated whether the NodeType maps to a print label", false, false );
            }

            #endregion SEBASTIAN

            #region TITANIA

            _acceptBlame( CswDeveloper.CF, 27965 );

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_tabset", "servermanaged" ) )
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn( "nodetype_tabset", "servermanaged", "Indicates that the tab is Server Managed", logicaldelete: false, required: false );
            }

            _resetBlame();


            #region Case 27862 - add "hidden" col to nodes table

            _acceptBlame( CswDeveloper.MB, 27862 );

            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "hidden" ) )
            {
                //Add a "hidden" column to nodes
                _CswNbtSchemaModTrnsctn.addBooleanColumn(
                    tablename: "nodes",
                    columnname: "hidden",
                    description: "whether the node is hidden or not",
                    logicaldelete: false,
                    required: true );
            }

            _resetBlame();

            #endregion

            _changeWelcomeTableToLandingPageTable();

            #endregion TITANIA

        }//Update()

        private void _changeWelcomeTableToLandingPageTable()
        {

            _acceptBlame( CswDeveloper.BV, 27881 );

            if( false == _CswNbtSchemaModTrnsctn.isTableDefined( "landingpage" ) && _CswNbtSchemaModTrnsctn.isTableDefined( "welcome" ) )
            {
                _CswNbtSchemaModTrnsctn.copyTable( "welcome", "landingpage", false );
                _CswNbtSchemaModTrnsctn.dropTable( "welcome" );
            }

            if( _CswNbtSchemaModTrnsctn.isTableDefined( "landingpage" ) )
            {
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "landingpage", "to_tabid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "landingpage", "to_tabid", "TabId to use for a given node - if the given Node's NodeType does not contain this TabId, component will not be created.", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "landingpage", "to_objectclasspropid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "landingpage", "to_objectclasspropid", "PropId for an ObjectClass button - if the given Node's does not have this button, component will not be created.", false, false );
                }
                if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "landingpage", "for_actionid" ) )
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn( "landingpage", "for_actionid", "Indicates the action to which this landing page is associated", false, false );
                }
                _renameLandingPageColumn( "welcomeid", "landingpageid" );
                _renameLandingPageColumn( "roleid", "for_roleid" );
                _renameLandingPageColumn( "nodeviewid", "to_nodeviewid" );
                _renameLandingPageColumn( "nodetypeid", "to_nodetypeid" );
                _renameLandingPageColumn( "actionid", "to_actionid" );
                _renameLandingPageColumn( "reportid", "to_reportid" );
            }

            _resetBlame();
        }

        private void _renameLandingPageColumn( string OldName, string NewName )
        {
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "landingpage", OldName ) && false == _CswNbtSchemaModTrnsctn.isColumnDefined( "landingpage", NewName ) )
            {
                _CswNbtSchemaModTrnsctn.renameColumn( "landingpage", OldName, NewName );
            }
        }

    }//class RunBeforeEveryExecutionOfUpdater_01

}//namespace ChemSW.Nbt.Schema


