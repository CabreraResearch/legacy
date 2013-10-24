﻿using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDefOrder
    {
        private readonly CswNbtResources _CswNbtResources;
        private readonly DataRow _row;

        public CswNbtImportDefOrder( CswNbtResources CswNbtResources, DataRow OrderRow )
        {
            _CswNbtResources = CswNbtResources;
            _row = OrderRow;
        }

        public Int32 ImportOrderId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.importdeforderid] ); }
        }
        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.importdefid] ); }
        }
        public Int32 Order
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.importorder] ); }
        }
        public string NodeTypeName
        {
            get { return _row[CswNbtImportTables.ImportDefOrder.nodetypename].ToString(); }
        }
        public Int32 Instance
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDefOrder.instance] ); }
        }

        public CswNbtMetaDataNodeType NodeType
        {
            get { return _CswNbtResources.MetaData.getNodeType( NodeTypeName ); }
        }

        public string PkColName
        {
            get
            {
                string ret = CswNbtImportDefBinding.SafeColName( NodeType.NodeTypeName );
                if( Int32.MinValue != Instance )
                {
                    ret += "_" + Instance;
                }
                ret += "_nodeid";
                return ret;
            }
        }

        /// <summary>
        /// Get a DataTable to fill out, for use with addOrderEntries()
        /// </summary>
        /// <returns></returns>
        public static DataTable getDataTableForNewOrderEntries()
        {
            DataTable Table = new DataTable();
            Table.Columns.Add( "sheet" );
            Table.Columns.Add( "nodetype" );
            Table.Columns.Add( "order" );
            Table.Columns.Add( "instance" );
            return Table;
        }

        /// <summary>
        /// Add new Order entries to a definition (for use by CswNbtImporter)
        /// </summary>
        public static void addOrderEntries( CswNbtResources CswNbtResources, DataTable OrderDataTable, Dictionary<string, Int32> DefIdsBySheetName )
        {
            CswTableUpdate importOrderUpdate = CswNbtResources.makeCswTableUpdate( "CswNbtImportDefOrder_addOrderEntries_Update", CswNbtImportTables.ImportDefOrder.TableName );
            DataTable importOrderTable = importOrderUpdate.getEmptyTable();
            foreach( DataRow OrderRow in OrderDataTable.Rows )
            {
                string SheetName = OrderRow["sheetname"].ToString();
                if( false == string.IsNullOrEmpty( SheetName ) )
                {
                    string NTName = OrderRow["nodetypename"].ToString();
                    CswNbtMetaDataNodeType NodeType = CswNbtResources.MetaData.getNodeType( NTName );
                    if( null != NodeType )
                    {
                        DataRow row = importOrderTable.NewRow();
                        row[CswNbtImportTables.ImportDefOrder.importdefid] = DefIdsBySheetName[SheetName];
                        row[CswNbtImportTables.ImportDefOrder.importorder] = CswConvert.ToDbVal( CswConvert.ToInt32( OrderRow["importorder"] ) );
                        row[CswNbtImportTables.ImportDefOrder.nodetypename] = NTName;
                        row[CswNbtImportTables.ImportDefOrder.instance] = CswConvert.ToDbVal( OrderRow["instance"].ToString() );
                        importOrderTable.Rows.Add( row );
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Error, "Error reading definition file", "Invalid NodeType defined in 'Order' sheet: " + NTName );
                    }
                } // if(false == string.IsNullOrEmpty(SheetName) )
            } // foreach( DataRow OrderRow in OrderDataTable.Rows )
            importOrderUpdate.update( importOrderTable );
        } // addOrderEntries()

    }
}
