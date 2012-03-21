using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 25374, part B
    /// </summary>
    public class CswUpdateSchemaCase25374B : CswUpdateSchemaTo
    {

        public override void update()
        {
            CswNbtNode SqlRptNode = null;

            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ReportClass );
            foreach( CswNbtNode ReportNode in ReportOC.getNodes( false, true ) )
            {
                if( ReportNode.NodeName == "SQL Report View Dictionary" )
                {
                    SqlRptNode = ReportNode;
                }
            }

            if( SqlRptNode != null )
            {
                CswNbtObjClassReport SqlRptNodeAsReport = CswNbtNodeCaster.AsReport( SqlRptNode );
                if( SqlRptNodeAsReport.RPTFile.Empty )
                {
                    // Add Sql Report Dictionary report .rpt file, stored in Resources
                    SetPropBlobValue( ChemSW.Nbt.Properties.Resources.SqlRptViewDict_rpt,
                                     "SqlRptViewDict.rpt",
                                     "application/octet-stream",
                                     SqlRptNode,
                                     _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( SqlRptNodeAsReport.RPTFile.NodeTypePropId ),
                                     "blobdata" );
                }
            }

        }//Update()


        // stolen from CswNbtWebServiceTabsAndProps()

        private bool SetPropBlobValue( byte[] Data, string FileName, string ContentType, CswNbtNode Node, CswNbtMetaDataNodeTypeProp MetaDataProp, string Column )
        {
            bool ret = false;
            if( Int32.MinValue != Node.NodeId.PrimaryKey )
            {
                CswNbtNodePropWrapper PropWrapper = Node.Properties[MetaDataProp];

                // Do the update directly
                CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "25374B_save_update", "jct_nodes_props" );
                JctUpdate.AllowBlobColumns = true;
                if( PropWrapper.JctNodePropId > 0 )
                {
                    DataTable JctTable = JctUpdate.getTable( "jctnodepropid", PropWrapper.JctNodePropId );
                    if( JctTable.Columns[Column].DataType == typeof( string ) )
                    {
                        JctTable.Rows[0][Column] = CswTools.ByteArrayToString( Data );
                    }
                    else
                    {
                        JctTable.Rows[0][Column] = Data;
                    }
                    JctTable.Rows[0]["field1"] = FileName;
                    JctTable.Rows[0]["field2"] = ContentType;
                    JctUpdate.update( JctTable );
                }
                else
                {
                    DataTable JctTable = JctUpdate.getEmptyTable();
                    DataRow JRow = JctTable.NewRow();
                    JRow["nodetypepropid"] = CswConvert.ToDbVal( MetaDataProp.PropId );
                    JRow["nodeid"] = CswConvert.ToDbVal( Node.NodeId.PrimaryKey );
                    JRow["nodeidtablename"] = Node.NodeId.TableName;
                    JRow[Column] = Data;
                    JRow["field1"] = FileName;
                    JRow["field2"] = ContentType;
                    JctTable.Rows.Add( JRow );
                    JctUpdate.update( JctTable );
                }
                ret = true;
            } // if( Int32.MinValue != NbtNodeKey.NodeId.PrimaryKey )
            return ret;
        } // SetPropBlobValue()


    }//class CswUpdateSchemaCase25374B

}//namespace ChemSW.Nbt.Schema