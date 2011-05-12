using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
using ChemSW.DB;
using ChemSW.Nbt.Schema;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{

    public class CswTestCase_014_02 : ICswUpdateSchemaTo
    {


        private CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn;

        public string Description { get { return ( _CswTstCaseRsrc.makeTestCaseDescription( this.GetType().Name, _CswTstCaseRsrc_014.Purpose, "Do select with select columns to reproduce errant behavior" ) ); } }

        private CswTestCaseRsrc _CswTstCaseRsrc = null;
        private CswTstCaseRsrc_014 _CswTstCaseRsrc_014 = null;

        private CswSchemaVersion _CswSchemaVersion = null;
        public CswSchemaVersion SchemaVersion { get { return ( _CswSchemaVersion ); } }
        public CswTestCase_014_02( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn, CswSchemaVersion CswSchemaVersion, object CswTstCaseRsrc )
        {
            _CswSchemaVersion = CswSchemaVersion;
            _CswNbtSchemaModTrnsctn = CswNbtSchemaModTrnsctn;
            _CswTstCaseRsrc = new CswTestCaseRsrc( _CswNbtSchemaModTrnsctn );
            _CswTstCaseRsrc_014 = (CswTstCaseRsrc_014) CswTstCaseRsrc;
        }//ctor

        public void update()
        {


            CswTableSelect CswTableSelectNodeTypes = _CswNbtSchemaModTrnsctn.makeCswTableSelect( Description, "nodetypes" );
            DataTable NodetypesTable = CswTableSelectNodeTypes.getTable( " where lower(tablename)='materials'" );
            Int32 NodeTypeId = Convert.ToInt32( NodetypesTable.Rows[0]["nodetypeid"] );

            CswNbtMetaDataNodeType NodeType = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( NodeTypeId );
            CswTableUpdate CswTableUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( Description, NodeType.TableName );
            string PkColumnName = _CswNbtSchemaModTrnsctn.getPrimeKeyColName( NodeType.TableName );

            //bz # 9102: This is the way of getting the record that causes the updated record disappear
            CswCommaDelimitedString SelectColumns = new CswCommaDelimitedString();
            foreach( CswNbtMetaDataNodeTypeProp CurrentNodeTypeProp in NodeType.NodeTypeProps )
            {
                foreach( CswNbtSubField CurrentSubField in CurrentNodeTypeProp.FieldTypeRule.SubFields )
                {
                    if( CurrentSubField.RelationalColumn != string.Empty )
                        SelectColumns.Add( CurrentSubField.RelationalColumn );
                }
            }//iterate node type props to set up select columns
            DataTable DataTable = CswTableUpdate.getTable( SelectColumns, PkColumnName, _CswTstCaseRsrc_014.InsertedMaterialsRecordPk, string.Empty, false );

            DataTable.Rows[0]["materialname"] = "nu _CswTstCaseRsrc_014.Purpose";

            CswTableUpdate.update( DataTable );

        }//runTest()

    }//CswSchemaUpdaterTestCaseDropColumnRollback

}//ChemSW.Nbt.Schema
