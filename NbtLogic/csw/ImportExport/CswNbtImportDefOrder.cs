using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.csw.Schema.Import;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Schema;

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
        /// Add new Order entries to a definition (for use by CswNbtImporter)
        /// </summary>
        public static void addOrderEntries( CswNbtResources CswNbtResources, DataTable OrderDataTable )
        {
            CswTableUpdate importOrderUpdate = CswNbtResources.makeCswTableUpdate( "CswNbtImportDefOrder_addOrderEntries_Update", CswNbtImportTables.ImportDefOrder.TableName );
            foreach( DataRow OrderRow in OrderDataTable.Select() )
            {
                //set blank instances to min value
                if( OrderRow["instance"] == DBNull.Value || String.IsNullOrEmpty( OrderRow["instance"].ToString() ) )
                {
                    OrderRow["instance"] = Int32.MinValue;
                }

                string NTName = OrderRow["nodetypename"].ToString();
                CswNbtMetaDataNodeType NodeType = CswNbtResources.MetaData.getNodeType( NTName );

                if( null == NodeType )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Error reading definition", "Invalid NodeType defined in 'Order' sheet: " + NTName );
                } // if(false == string.IsNullOrEmpty(SheetName) )
            } // foreach( DataRow OrderRow in OrderDataTable.Rows )

            //this is a hack, and the fact that we can even do this makes me sad
            importOrderUpdate._DoledOutTables.Add( OrderDataTable );
            importOrderUpdate.update( OrderDataTable );

        } // addOrderEntries()

        /// <summary>
        /// // first get importdefid from import_def
        // then get each row from import_order that has above importdefid
        // iterate rows, change order column value
        /// </summary>
        /// <param name="CswNbtResources"></param>
        public static void updateOrderEntries( CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn )
        {
            Dictionary<string, Int32> CafImportOrder = CswNbtCAFImportOrder.CAFOrder;

            CswTableSelect TblSelect1 = CswNbtSchemaModTrnsctn.makeCswTableSelect( "get_caf_importdefid", CswNbtImportTables.ImportDef.TableName );
            DataTable ImportDefDt = TblSelect1.getTable( "where " + CswNbtImportTables.ImportDef.definitionname + " = 'CAF'" );
            Int32 CAFImportDefId = Int32.MinValue;
            if( ImportDefDt.Rows.Count > 0 )
            {
                CAFImportDefId = CswConvert.ToInt32( ImportDefDt.Rows[0][CswNbtImportTables.ImportDef.importdefid] );
            }
            CswTableUpdate TblUpdate1 = CswNbtSchemaModTrnsctn.makeCswTableUpdate( "update_import_order_caf", CswNbtImportTables.ImportDefOrder.TableName );
            DataTable ImportOrderDt = TblUpdate1.getTable( "where " + CswNbtImportTables.ImportDefOrder.importdefid + " = " + CAFImportDefId );
            foreach( DataRow CurrentRow in ImportOrderDt.Rows )
            {
                string currentNt = CswConvert.ToString( CurrentRow[CswNbtImportTables.ImportDefOrder.nodetypename] );
                Int32 newOrder = CafImportOrder[currentNt];
                CurrentRow[CswNbtImportTables.ImportDefOrder.importorder] = newOrder;
            }
            TblUpdate1.update( ImportOrderDt );
        }

    }
}
