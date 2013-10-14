using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case30733: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30733; }
        }

        public override string ScriptName
        {
            get { return "02F_Case30733"; }
        }

        public override void update()
        {
            //get all Structure NTP ids
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp StructureOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.Structure );
            CswCommaDelimitedString StructreNTPIds = new CswCommaDelimitedString();
            foreach( CswNbtMetaDataNodeTypeProp StructureNTP in StructureOCP.getNodeTypeProps() )
            {
                StructreNTPIds.Add( StructureNTP.PropId.ToString() );
            }

            //get all node ids from jct_nodes_props that have more than one record for the Structure prop
            Collection<CswPrimaryKey> NodeIds = new Collection<CswPrimaryKey>();
            string sql = "select nodeid, count(nodeid) as cnt from jct_nodes_props where nodetypepropid in (" + StructreNTPIds.ToString() + ") group by nodeid";
            CswArbitrarySelect ArbSelect = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "GetDuplicateStructureRecords", sql );
            DataTable jnpTbl = ArbSelect.getTable();
            foreach( DataRow row in jnpTbl.Rows )
            {
                if( CswConvert.ToInt32( row["cnt"] ) > 1 )
                {
                    NodeIds.Add( new CswPrimaryKey( "nodes", CswConvert.ToInt32( row["nodeid"] ) ) );
                }
            }

            //Fix the Nodes
            foreach( CswPrimaryKey NodeId in NodeIds )
            {
                CswCommaDelimitedString JctNodePropRowsToDelete = new CswCommaDelimitedString();

                CswNbtObjClassChemical ChemicalNode = _CswNbtSchemaModTrnsctn.Nodes[NodeId];
                if( String.IsNullOrEmpty( ChemicalNode.Structure.Mol ) ) //if there is no Mol, a user never uploaded a new Mol, find the JctNodeProp row that has the clob data and set it correctly
                {
                    DataTable JnpTbl = _getJNPTblUpdate( NodeId, StructreNTPIds );
                    foreach( DataRow row in JnpTbl.Rows )
                    {
                        if( String.IsNullOrEmpty( row["clobdata"].ToString() ) )
                        {
                            JctNodePropRowsToDelete.Add( CswConvert.ToInt32( row["jctnodepropid"] ).ToString() );
                        }
                    }
                    CswTableUpdate blobDataTblUpdate = _getBlobTblUpdate( JctNodePropRowsToDelete );
                    DataTable blobDataTbl = blobDataTblUpdate.getTable();
                    if( blobDataTbl.Rows.Count > 0 )
                    {
                        blobDataTbl.Rows[0]["jctnodepropid"] = ChemicalNode.Structure.JctNodePropId;
                    }
                    blobDataTblUpdate.update( blobDataTbl );

                }
                else //the user uploaded a new mol to the Chemical, throw out the old data and use the new data
                {
                    DataTable JnpTbl = _getJNPTblUpdate( NodeId, StructreNTPIds );
                    foreach( DataRow row in JnpTbl.Rows )
                    {
                        if( CswConvert.ToInt32( row["jctnodepropid"] ) != ChemicalNode.Structure.JctNodePropId )
                        {
                            JctNodePropRowsToDelete.Add( CswConvert.ToInt32( row["jctnodepropid"] ).ToString() );
                        }
                    }

                    if( JctNodePropRowsToDelete.Count > 0 )
                    {
                        CswTableUpdate BlobDataTblUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DeleteOldBlobDataRows", "blob_data" );
                        DataTable blobDataTbl = BlobDataTblUpdate.getTable( "where jctnodepropid in (" + JctNodePropRowsToDelete.ToString() + ")" );
                        foreach( DataRow row in blobDataTbl.Rows )
                        {
                            row.Delete();
                        }
                        BlobDataTblUpdate.update( blobDataTbl );
                    }
                }


                CswTableUpdate JnpTblUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "DeleteBadJnpDataRows", "jct_nodes_props" );
                DataTable jnpDataTbl = JnpTblUpdate.getTable( "where jctnodepropid in (" + JctNodePropRowsToDelete.ToString() + ")" );
                foreach( DataRow row in jnpDataTbl.Rows )
                {
                    row.Delete();
                }
                JnpTblUpdate.update( jnpDataTbl );
            }


        } // update()

        private DataTable _getJNPTblUpdate( CswPrimaryKey NodeId, CswCommaDelimitedString StructureNTPIds )
        {
            string sql = "select * from jct_nodes_props where nodetypepropid in (" + StructureNTPIds.ToString() + ") and nodeid = " + NodeId.PrimaryKey;
            CswArbitrarySelect arbSel = _CswNbtSchemaModTrnsctn.makeCswArbitrarySelect( "getJNPRows", sql );
            return arbSel.getTable();
        }

        private CswTableUpdate _getBlobTblUpdate( CswCommaDelimitedString jnpIds )
        {
            string sql = " where jctnodepropid in (" + jnpIds.ToString() + ")";
            return _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "getBlobDataTblUpdate", sql );
        }

    }

}//namespace ChemSW.Nbt.Schema