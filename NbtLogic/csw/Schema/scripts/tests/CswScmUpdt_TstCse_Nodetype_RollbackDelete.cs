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

    public class CswScmUpdt_TstCse_Nodetype_RollbackDelete : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_Nodetype_RollbackDelete( )
            : base( "Delete nodetype rollback" )
        {
            _AppVenue = AppVenue.NBT;
        }//ctor

        private string _NodeTypeToTest = "Equipment";

        public override void runTest()
        {

            _CswNbtSchemaModTrnsctn.beginTransaction();

            CswNbtMetaDataNodeType EquipmentNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeToTest );

            if ( null != EquipmentNodeType )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteNodeType( EquipmentNodeType );
            }
            else
            {
                throw ( new CswDniException( "There is no " + _NodeTypeToTest + " nodetype with which to conduct this test" ) );
            }//


            EquipmentNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeToTest );
            if ( null != EquipmentNodeType )
                throw ( new CswDniException( "Nodetype " + _NodeTypeToTest + " was not deleted" ) );

            //DateTime Start = DateTime.Now;
            _CswNbtSchemaModTrnsctn.rollbackTransaction();
            //TimeSpan Span = DateTime.Now - Start;

            _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

            EquipmentNodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( _NodeTypeToTest );
            if ( null == EquipmentNodeType )
                throw ( new CswDniException( "Nodetype " + _NodeTypeToTest + " was not restored after rollback" ) );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDeleteNodetypeRollback

}//ChemSW.Nbt.Schema
