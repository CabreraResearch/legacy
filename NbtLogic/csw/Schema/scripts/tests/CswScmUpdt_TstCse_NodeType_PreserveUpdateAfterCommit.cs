using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_TstCse_NodeType_PreserveUpdateAfterCommit : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_NodeType_PreserveUpdateAfterCommit( )
            : base( "Table update to nodetype" )
        {
            _AppVenue = AppVenue.NBT;

        }//ctor

        public override void runTest()
        {

            //Update nodetype through meta data
            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswNbtMetaDataNodeType UserNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "User" );
            UserNodeType.Category = "badcategory";

            _CswNbtSchemaModTrnsctn.commitTransaction();

            //Update meta data through a table
            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswTableUpdate NodeTypesTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_NodeType_PreserveUpdateAfterCommit_update", "nodetypes" );
            DataTable NodeTypesTable = NodeTypesTableUpdate.getTable();
            foreach ( DataRow NodeTypeRow in NodeTypesTable.Rows )
            {
                if ( NodeTypeRow[ "nodetypename" ].ToString() == "User" )
                    NodeTypeRow[ "category" ] = "goodcategory";
            }
            NodeTypesTableUpdate.update( NodeTypesTable );

            _CswNbtSchemaModTrnsctn.commitTransaction();

            CswTableSelect NodeTypesSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswScmUpdt_TstCse_NodeType_PreserveUpdateAfterCommit_select", "nodetypes" );
            if( "goodcategory" != NodeTypesSelect.getTable("where nodetypename = 'User'").Rows[0]["category"].ToString() )
                throw ( new CswScmUpdt_Exception ( "Update to nodetypes table did not superceed update to meta data node type in previous transaction" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseRollbackViewUpdates

}//ChemSW.Nbt.Schema
