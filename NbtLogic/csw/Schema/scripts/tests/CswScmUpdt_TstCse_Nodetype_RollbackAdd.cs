using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_TstCse_Nodetype_RollbackAdd : CswScmUpdt_TstCse
    {

        private string _NodeTypeName = "ArbitraryNodeType";

        public CswScmUpdt_TstCse_Nodetype_RollbackAdd( )
            : base( "Add nodetype rollback" )
        {
            _AppVenue = AppVenue.NBT;
        }//ctor


        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswNbtMetaDataNodeType TestNodeType = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( "GenericClass", _NodeTypeName, string.Empty );
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( TestNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Name", string.Empty );
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( TestNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "Brand", string.Empty );
            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( TestNodeType, CswNbtMetaDataFieldType.NbtFieldType.Location, "Location", string.Empty );

            CswNbtNodeTypePermissions CswNbtNodeTypePermissions = _CswNbtSchemaModTrnsctn.getNodeTypePermissions( "Administrator", _NodeTypeName );
            CswNbtNodeTypePermissions.Create = true;
            CswNbtNodeTypePermissions.Delete = true;
            CswNbtNodeTypePermissions.Edit = true;
            CswNbtNodeTypePermissions.View = true;
            CswNbtNodeTypePermissions.Save();

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            if ( null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeName ) )
                throw ( new CswDniException( "Nodetype " + _NodeTypeName + " was not created; test cannot proceed" ) );


            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            if ( null != _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeName ) )
                throw ( new CswDniException( "Rollback of adding Nodetype " + _NodeTypeName + " failed" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseAddNodetypeRollback

}//ChemSW.Nbt.Schema
