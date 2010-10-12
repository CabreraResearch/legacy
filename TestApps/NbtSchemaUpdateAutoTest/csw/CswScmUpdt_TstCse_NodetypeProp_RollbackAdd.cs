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

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{

    public class CswScmUpdt_TstCse_NodetypeProp_RollbackAdd : CswScmUpdt_TstCse
    {

        private string _NodeTypeName = "Equipment";
        private string _NodeTypePropName = "FooProperty";

        public CswScmUpdt_TstCse_NodetypeProp_RollbackAdd()
            : base( "Add nodetype prop rollback" )
        {
            _AppVenue = AppVenue.NBT;

        }//ctor


        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswNbtMetaDataNodeType EquipmentNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeName );

            if ( null == EquipmentNodeType )
                throw ( new CswDniException( "Unable to run test: nodetype " + _NodeTypeName + " does not exist" ) );

            _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( EquipmentNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, _NodeTypePropName, string.Empty );

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            if ( null == EquipmentNodeType.getNodeTypeProp( _NodeTypePropName ) )
                throw ( new CswDniException( "Unable to run test: property " + _NodeTypePropName + " was not created" ) );


            _CswNbtSchemaModTrnsctn.rollbackTransaction();

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            if ( null != EquipmentNodeType.getNodeTypeProp( _NodeTypePropName ) )
                throw ( new CswDniException( "Rollback of adding nodetype prop " + _NodeTypePropName + " failed" ) );


        }//runTest()

    }//CswSchemaUpdaterTestCaseAddNodetypePropRollback

}//ChemSW.Nbt.Schema
