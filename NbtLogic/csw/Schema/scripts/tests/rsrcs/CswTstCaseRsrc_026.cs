using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
using ChemSW.DB;
using ChemSW.Core;
using ChemSW.Nbt.Schema;
using ChemSW.Audit;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Test Case: 001, part 01
    /// </summary>
    public class CswTstCaseRsrc_026
    {

        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        private CswTestCaseRsrc _CswTestCaseRsrc = null;
        private CswAuditMetaData _CswAuditMetaData = new CswAuditMetaData();
        public CswTstCaseRsrc_026( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {

            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTestCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
        }//ctor



        public string Purpose = "Audit of NBT node and prop val changes";


        public string InsertValOfBuiltInProp = "insert_val_foo";
        public string InsertValOfAddedProp = "insert_val_bar";
        public string UpdateValOfBuiltInProp = "update_val_foo";
        public string UpdateValOfAddedProp = "update_val_bar";
        public Int32 JctNodePropIdOfBuiltInProp = Int32.MaxValue;
        public Int32 JctNodePropIdOfAddedProp = Int32.MaxValue;


        public CswNbtMetaDataNodeType TestNodeType = null;
        public CswNbtNode TestNode = null;
        public CswNbtMetaDataNodeTypeProp BuiltInProp = null;
        public CswNbtMetaDataNodeTypeProp AddedProp = null;
        public void makeArbitraryNode()
        {
            if( null == TestNode )
            {

                if( null == TestNodeType )
                {
                    TestNodeType = _CswTestCaseRsrc.makeTestNodeType( TestNodeTypeNamesFake.TestNodeType01 );
                    BuiltInProp = TestNodeType.getNodeTypeProp( "Name" );
                    AddedProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( TestNodeType, CswNbtMetaDataFieldType.NbtFieldType.Text, "AddedProp", 0 );
                }

                CswNbtMetaDataNodeType LatestVersionNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( TestNodeType.NodeTypeId ).LatestVersionNodeType;
                TestNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( LatestVersionNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.DoNothing );
            }//if we haven't already made a node

        }//makeArbitraryNode()


        public DataTable getJctAuditRecords()
        {
            DataTable ReturnVal = null;

            CswCommaDelimitedString CswCommaDelimitedString = new CswCommaDelimitedString();
            CswCommaDelimitedString.Add( JctNodePropIdOfBuiltInProp.ToString() );
            CswCommaDelimitedString.Add( JctNodePropIdOfAddedProp.ToString() );

            CswArbitrarySelect CswArbitrarySelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( Purpose, "select * from jct_nodes_props_audit where jctnodepropid in (" + CswCommaDelimitedString.ToString() + ")" );


            ReturnVal = CswArbitrarySelect.getTable();

            return ( ReturnVal );
        }



        private string _OriginalAuditSetting_Audit = string.Empty;
        public void setAuditingOn()
        {
            _OriginalAuditSetting_Audit = _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName );

            if( "1" != _OriginalAuditSetting_Audit )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( _CswAuditMetaData.AuditConfgVarName, "1" );
            }

        }//setAuditingOn()

        public void restoreAuditSetting()
        {
            if( _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName ) != _OriginalAuditSetting_Audit )
            {
                _CswNbtSchemaModTrnsctn.setConfigVariableValue( _CswAuditMetaData.AuditConfgVarName, _OriginalAuditSetting_Audit );
            }
        }//setAuditingOn()

        public void assertAuditSettingIsRestored()
        {
            string CurrentAuditSetting = _CswNbtSchemaModTrnsctn.getConfigVariableValue( _CswAuditMetaData.AuditConfgVarName );

            if( CurrentAuditSetting != _OriginalAuditSetting_Audit )
            {
                throw ( new CswDniException( "Current audit configuration setting (" + CurrentAuditSetting + ") does not match the original setting (" + _OriginalAuditSetting_Audit + ")" ) );
            }

        }//assertAuditSettingIsRestored()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
