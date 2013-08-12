using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDef
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbtImportDef( CswNbtResources CswNbtResources, string DefinitionName, string SheetName )
        {
            // If incoming sheet name has a $, trim it off    
            SheetName = SheetName.TrimEnd( new char[] { '$' } );

            CswTableSelect defSelect = CswNbtResources.makeCswTableSelect( "CswNbt2DDefinition_select", CswNbtImportTables.ImportDef.TableName );
            DataTable defTable = defSelect.getTable( "where lower(" + CswNbtImportTables.ImportDef.definitionname + ") = '" + DefinitionName.ToLower() + "'" +
                                                     "  and lower(" + CswNbtImportTables.ImportDef.sheetname + ") = '" + SheetName.ToLower() + "'" );
            if( defTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, defTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition: " + DefinitionName + " and sheetname: " + SheetName, "Could not find a matching record in " + CswNbtImportTables.ImportDef.TableName );
            }
        }

        public CswNbtImportDef( CswNbtResources CswNbtResources, Int32 ImportDefinitionId )
        {
            CswTableSelect defSelect = CswNbtResources.makeCswTableSelect( "CswNbt2DDefinition_select", CswNbtImportTables.ImportDef.TableName );
            DataTable defTable = defSelect.getTable( CswNbtImportTables.ImportDef.importdefid, ImportDefinitionId );
            if( defTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, defTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition id: " + ImportDefinitionId, "Could not find a matching record in " + CswNbtImportTables.ImportDef.TableName );
            }
        }

        public CswNbtImportDef( CswNbtResources CswNbtResources, DataRow DefinitionRow )
        {
            _finishConstructor( CswNbtResources, DefinitionRow );
        }

        private void _finishConstructor( CswNbtResources CswNbtResources, DataRow DefinitionRow )
        {
            _CswNbtResources = CswNbtResources;
            if( null != DefinitionRow )
            {
                _row = DefinitionRow;
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition", "CswNbt2DDefinition was passed a null DefinitionRow" );
            }
            _loadBindings();
        }

        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDef.importdefid] ); }
        }
        public string DefinitionName
        {
            get { return _row[CswNbtImportTables.ImportDef.definitionname].ToString(); }
        }
        public string SheetName
        {
            get { return _row[CswNbtImportTables.ImportDef.sheetname].ToString(); }
        }
        public Int32 SheetOrder
        {
            get { return CswConvert.ToInt32( _row[CswNbtImportTables.ImportDef.sheetorder] ); }
        }

        public CswNbtImportDefBindingCollection Bindings = new CswNbtImportDefBindingCollection();
        public SortedList<Int32, CswNbtImportDefOrder> ImportOrder = new SortedList<int, CswNbtImportDefOrder>();
        public Collection<CswNbtImportDefRelationship> RowRelationships = new Collection<CswNbtImportDefRelationship>();


        /// <summary>
        /// Loads import definition bindings from the database
        /// </summary>
        private void _loadBindings()
        {
            if( Int32.MinValue != ImportDefinitionId )
            {
                // Order
                CswTableSelect OrderSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_order_select", CswNbtImportTables.ImportDefOrder.TableName );
                DataTable OrderDataTable = OrderSelect.getTable( CswNbtImportTables.ImportDefOrder.importdefid, ImportDefinitionId );
                foreach( DataRow OrderRow in OrderDataTable.Rows )
                {
                    this.ImportOrder.Add( CswConvert.ToInt32( OrderRow[CswNbtImportTables.ImportDefOrder.importorder] ), new CswNbtImportDefOrder( _CswNbtResources, OrderRow ) );
                }

                // Bindings
                CswTableSelect BindingsSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_bindings_select", CswNbtImportTables.ImportDefBindings.TableName );
                DataTable BindingsDataTable = BindingsSelect.getTable( CswNbtImportTables.ImportDefBindings.importdefid, ImportDefinitionId );
                foreach( DataRow BindingRow in BindingsDataTable.Rows )
                {
                    Bindings.Add( new CswNbtImportDefBinding( _CswNbtResources, BindingRow ) );
                }

                // Row Relationships
                CswTableSelect RelationshipsSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_rel_select", CswNbtImportTables.ImportDefRelationships.TableName );
                DataTable RelationshipsDataTable = RelationshipsSelect.getTable( CswNbtImportTables.ImportDefRelationships.importdefid, ImportDefinitionId );
                foreach( DataRow RelRow in RelationshipsDataTable.Rows )
                {
                    RowRelationships.Add( new CswNbtImportDefRelationship( _CswNbtResources, RelRow ) );
                }
            }
        } // _loadBindings()
    } // class CswNbt2DDefinition
} // namespace
