using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.SchemaUpdaterAutoTest
{
    //9102
    //when you retrieve the data table using a list of select columns, 
    //the update performed with that Table results in the deletion of the record.
    public class CswScmUpdt_TstCse_DataTable_PreserveBlobColumns : CswScmUpdt_TstCse
    {
        public CswScmUpdt_TstCse_DataTable_PreserveBlobColumns()
            : base( "Update Preserve Blob Column" )
        {
            _AppVenue = AppVenue.Generic;
        }//ctor

        //bz #: 10115
        public override void runTest()
        {

            Int32 MaterialsPk = Int32.MinValue;
            try
            {


                _CswNbtSchemaModTrnsctn.beginTransaction();

                //SETUP: BEGIN *****************************************
                //Step 1: Make the nodetype ****************************
                Int32 TestByteArraySize = 256;
                string BlobNodeTypeName = "TestForBlobNodeType";
                string BlobPropName = "The Blob";
                CswNbtMetaData CswNbtMetaData = _CswNbtSchemaModTrnsctn.MetaData;
                CswNbtMetaDataObjectClass GenericObjectClass = CswNbtMetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GenericClass );
                CswNbtMetaDataNodeType BlobNodeTypeNodeType = CswNbtMetaData.makeNewNodeType( GenericObjectClass.ObjectClassId, BlobNodeTypeName, string.Empty );
                CswNbtMetaDataNodeTypeProp BlobNodeTypeNodeTypeProp = CswNbtMetaData.makeNewProp( BlobNodeTypeNodeType, CswNbtMetaDataFieldType.NbtFieldType.Image, BlobPropName, string.Empty );


                _CswNbtSchemaModTrnsctn.MetaData.refreshAll();

                if ( null == _CswNbtSchemaModTrnsctn.MetaData.getNodeType( BlobNodeTypeName ) )
                    throw ( new CswDniException( "Nodetype " + BlobNodeTypeName + " was not created; test cannot proceed" ) );

                //Step 1: Make a node ****************************
                CswNbtNode BlobNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( BlobNodeTypeNodeType.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                Int32 BlobJctNodePropId = BlobNode.Properties[BlobNodeTypeNodeTypeProp].JctNodePropId;
                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "CswScmUpdt_TstCse_DataTable_PreserveBlobColumns_1", "jct_nodes_props" );
                JctUpdate.AllowBlobColumns = true;
                DataTable JctTable = JctUpdate.getTable( "jctnodepropid", BlobJctNodePropId );
                JctTable.Rows[0]["blobdata"] = new Byte[TestByteArraySize];
                JctTable.Rows[0]["field1"] = "Dummy File Name";
                JctTable.Rows[0]["field2"] = "Dummy File Type";
                JctUpdate.update( JctTable );


                //SETUP: END *****************************************



                //*** TEST PART 1: That we don't nuke the blob column when select columns are unspecified
                JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01G02_JctNodesProps_update", "jct_nodes_props" );

                JctTable = JctUpdate.getTable();
                foreach ( DataRow JctRow in JctTable.Rows )
                {
                    JctRow["nodeidtablename"] = "nodes";
                }
                JctUpdate.update( JctTable );


                CswTableSelect CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswScmUpdt_TstCse_DataTable_PreserveBlobColumns_2", "jct_nodes_props" );
                DataTable DataTable = CswTableSelect.getTable( " where blobdata is not null  " );
                if ( 0 == DataTable.Rows.Count )
                {
                    throw( new CswDniException( "A generic update of jct_nodes_props nuked the blob columns" ) ); 
                }


                //*** TEST PART 2: That we can retrieve and update the blob column when we use the AllowBlob flag
                //***              Note that in the setup sections we implciitly tested that we can retrieve and 
                //***              update the blob column when set the AllowBlobFlag. 
                JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01G02_JctNodesProps_update", "jct_nodes_props" );

                CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
                SelectColumns.Add( "field1" );
                SelectColumns.Add( "field2" );
                SelectColumns.Add( "blobdata" );
                JctTable = JctUpdate.getTable( SelectColumns );

                if ( false == JctTable.Columns.Contains( "blobdata" ) )
                {
                    throw ( new CswDniException( "Blob column was not retrieved when it was specified as a select column" ) );
                }


                 Byte[] TestByteArray = new Byte[TestByteArraySize]; // update an arbitrary row

                for ( int idx = 0; idx < TestByteArraySize; idx++ )
                {
                    TestByteArray[idx] = 1;
                }

                JctTable.Rows[0]["blobdata"] = TestByteArray;
                Int32 Arbitraryjctnodepropid = Convert.ToInt32 ( JctTable.Rows[0]["jctnodepropid"] ) ;
                JctUpdate.update( JctTable );


                CswTableSelect = _CswNbtSchemaModTrnsctn.makeCswTableSelect( "CswScmUpdt_TstCse_DataTable_PreserveBlobColumns_2", "jct_nodes_props" );
                DataTable = CswTableSelect.getTable( " where jctnodepropid=  " + Arbitraryjctnodepropid.ToString() + "and blobdata is not null ");

                if ( 0 == DataTable.Rows.Count  )
                {
                    throw ( new CswDniException( "An update of a blob column with specified select columns failed" ) );
                }


            }

            finally
            {
                _CswNbtSchemaModTrnsctn.rollbackTransaction();
            }

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
