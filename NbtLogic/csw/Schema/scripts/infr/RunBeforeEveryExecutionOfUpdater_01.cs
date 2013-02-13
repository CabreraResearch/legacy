using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01 : CswUpdateSchemaTo
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

        private void _acceptBlame(CswDeveloper BlameMe, Int32 BlameCaseNo)
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

            #region WILLIAM

            // case 26827
            if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("object_class", "searchdeferpropid"))
            {
                _CswNbtSchemaModTrnsctn.addLongColumn("object_class", "searchdeferpropid",
                                                      "Defer to the target of this property in search results", false,
                                                      false);
            }
            if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("nodetypes", "searchdeferpropid"))
            {
                _CswNbtSchemaModTrnsctn.addLongColumn("nodetypes", "searchdeferpropid",
                                                      "Defer to the target of this property in search results", false,
                                                      false);
            }

            _createTierIITable(CswDeveloper.BV, 28247);


            // case 25495
            _acceptBlame(CswDeveloper.SS, 25495);
            string SearchTableName = "search";
            if (false == _CswNbtSchemaModTrnsctn.isTableDefined(SearchTableName))
            {
                _CswNbtSchemaModTrnsctn.addTable(SearchTableName, "searchid");
                _CswNbtSchemaModTrnsctn.addStringColumn(SearchTableName, "category", "category for view selector", false, false, 40);
                _CswNbtSchemaModTrnsctn.addStringColumn(SearchTableName, "name", "name of search", false, false, 80);
                _CswNbtSchemaModTrnsctn.addForeignKeyColumn(SearchTableName, "userid", "owner of search", false, false, "nodes", "nodeid");
                _CswNbtSchemaModTrnsctn.addClobColumn(SearchTableName, "searchdata", "data for building this search", false, false);
            }
            _resetBlame();


            _acceptBlame(CswDeveloper.SS, 28492);
            if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("landingpage", "to_searchid"))
            {
                _CswNbtSchemaModTrnsctn.addLongColumn("landingpage", "to_searchid", "for search items", false, false);
            }
            _resetBlame();

            _acceptBlame(CswDeveloper.CF, 27882);
            if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("node_views", "groupbysiblings"))
            {
                _CswNbtSchemaModTrnsctn.addBooleanColumn("node_views", "groupbysiblings", "Group by Siblings in Tree", true, false);
            }
            _resetBlame();


            _acceptBlame( CswDeveloper.SS, 28508 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "nodes", "iconfilename" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "nodes", "iconfilename", "Overrides the icon from the nodetype", false, false, 50 );
            }
            _resetBlame();

            _acceptBlame( CswDeveloper.SS, 28523 );
            if( false == _CswNbtSchemaModTrnsctn.isColumnDefined( "actions", "iconfilename" ) )
            {
                _CswNbtSchemaModTrnsctn.addStringColumn( "actions", "iconfilename", "Default icon for the action", false, false, 50 );
            }
            _resetBlame();

            _acceptBlame( CswDeveloper.BV, 28746 );
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "nodetype_props", "extended" ) )
            {
                _CswNbtSchemaModTrnsctn.changeColumnDataType( "nodetype_props", "extended", DataDictionaryPortableDataType.String, 100 );
            }
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "object_class_props", "extended" ) )
            {
                _CswNbtSchemaModTrnsctn.changeColumnDataType( "object_class_props", "extended", DataDictionaryPortableDataType.String, 100 );
            }
            _resetBlame();

            _acceptBlame( CswDeveloper.BV, 27642 );
            if( _CswNbtSchemaModTrnsctn.isColumnDefined( "scheduledrules", "statusmessage" ) )
            {
                _CswNbtSchemaModTrnsctn.changeColumnDataType( "scheduledrules", "statusmessage", DataDictionaryPortableDataType.Clob, Int32.MinValue );
            }
            _resetBlame();

            #endregion WILLIAM

            #region YORICK

            //YORICK DDL changes go here.

            #endregion YORICK

        }//Update()

        private void _createTierIITable(CswDeveloper Dev, Int32 CaseNum)
        {
            _acceptBlame(Dev, CaseNum);

            if (false == _CswNbtSchemaModTrnsctn.isTableDefined("tier2"))
            {
                _CswNbtSchemaModTrnsctn.addTable("tier2", "tier2id");
                _CswNbtSchemaModTrnsctn.getNewPrimeKey("tier2");
            }
            if (_CswNbtSchemaModTrnsctn.isTableDefined("tier2"))
            {
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "dateadded"))
                {
                    _CswNbtSchemaModTrnsctn.addDateColumn("tier2", "dateadded", "Date added", false, false);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "locationid"))
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn("tier2", "locationid", "PK of the Location", false, false);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "parentlocationid"))
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn("tier2", "parentlocationid", "PK of the parent Location", false, false);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "materialid"))
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn("tier2", "materialid", "PK of the Material", false, false);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "casno"))
                {
                    _CswNbtSchemaModTrnsctn.addStringColumn("tier2", "casno", "Material's CASNo", false, false, 20);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "quantity"))
                {
                    _CswNbtSchemaModTrnsctn.addDoubleColumn("tier2", "quantity", "Quantity of the Material in the given Location", false, false, 6);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "totalquantity"))
                {
                    _CswNbtSchemaModTrnsctn.addDoubleColumn("tier2", "totalquantity", "Quantity of the Material in the given Location and all child locations", false, false, 6);
                }
                if (false == _CswNbtSchemaModTrnsctn.isColumnDefined("tier2", "unitid"))
                {
                    _CswNbtSchemaModTrnsctn.addLongColumn("tier2", "unitid", "UnitId of the Quantity of the Material", false, false);
                }
            }

            _resetBlame();
        }
    }//class RunBeforeEveryExecutionOfUpdater_01
}//namespace ChemSW.Nbt.Schema


