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
    public class CswNbt2DDefinition
    {
        private CswNbtResources _CswNbtResources;
        private DataRow _row;

        public CswNbt2DDefinition( CswNbtResources CswNbtResources, string DefinitionName, string SheetName )
        {
            CswTableSelect defSelect = CswNbtResources.makeCswTableSelect( "CswNbt2DDefinition_select", CswNbt2DImportTables.ImportDefinitions.TableName );
            DataTable defTable = defSelect.getTable( "where " + CswNbt2DImportTables.ImportDefinitions.definitionname + " = '" + DefinitionName + "'" +
                                                     "  and " + CswNbt2DImportTables.ImportDefinitions.sheetname + " = '" + SheetName + "'" );
            if( defTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, defTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition: " + DefinitionName + " and sheetname: " + SheetName, "Could not find a matching record in " + CswNbt2DImportTables.ImportDefinitions.TableName );
            }
        }

        public CswNbt2DDefinition( CswNbtResources CswNbtResources, Int32 ImportDefinitionId )
        {
            CswTableSelect defSelect = CswNbtResources.makeCswTableSelect( "CswNbt2DDefinition_select", CswNbt2DImportTables.ImportDefinitions.TableName );
            DataTable defTable = defSelect.getTable( CswNbt2DImportTables.ImportDefinitions.importdefinitionid, ImportDefinitionId );
            if( defTable.Rows.Count > 0 )
            {
                _finishConstructor( CswNbtResources, defTable.Rows[0] );
            }
            else
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid import definition id: " + ImportDefinitionId, "Could not find a matching record in " + CswNbt2DImportTables.ImportDefinitions.TableName );
            }
        }

        public CswNbt2DDefinition( CswNbtResources CswNbtResources, DataRow DefinitionRow )
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
            loadBindings();
        }

        public Int32 ImportDefinitionId
        {
            get { return CswConvert.ToInt32( _row[CswNbt2DImportTables.ImportDefinitions.importdefinitionid] ); }
        }
        public string DefinitionName
        {
            get { return _row[CswNbt2DImportTables.ImportDefinitions.definitionname].ToString(); }
        }
        public string SheetName
        {
            get { return _row[CswNbt2DImportTables.ImportDefinitions.sheetname].ToString(); }
        }

        public CswNbt2DBindingCollection Bindings = new CswNbt2DBindingCollection();
        public SortedList<Int32, CswNbt2DOrder> ImportOrder = new SortedList<int, CswNbt2DOrder>();
        public Collection<CswNbt2DRowRelationship> RowRelationships = new Collection<CswNbt2DRowRelationship>();


        /// <summary>
        /// Loads import definition bindings from the database
        /// </summary>
        private void loadBindings()
        {
            if( Int32.MinValue != ImportDefinitionId )
            {
                // Order
                CswTableSelect OrderSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_order_select", CswNbt2DImportTables.ImportOrder.TableName );
                DataTable OrderDataTable = OrderSelect.getTable( CswNbt2DImportTables.ImportOrder.importdefinitionid, ImportDefinitionId );
                foreach( DataRow OrderRow in OrderDataTable.Rows )
                {
                    this.ImportOrder.Add( CswConvert.ToInt32( OrderRow[CswNbt2DImportTables.ImportOrder.importorder] ), new CswNbt2DOrder( _CswNbtResources, OrderRow ) );
                }

                // Bindings
                CswTableSelect BindingsSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_bindings_select", CswNbt2DImportTables.ImportBindings.TableName );
                DataTable BindingsDataTable = BindingsSelect.getTable( CswNbt2DImportTables.ImportBindings.importdefinitionid, ImportDefinitionId );
                foreach( DataRow BindingRow in BindingsDataTable.Rows )
                {
                    Bindings.Add( new CswNbt2DBinding( _CswNbtResources, BindingRow ) );
                }

                // Row Relationships
                CswTableSelect RelationshipsSelect = _CswNbtResources.makeCswTableSelect( "loadBindings_rel_select", CswNbt2DImportTables.ImportRelationships.TableName );
                DataTable RelationshipsDataTable = RelationshipsSelect.getTable( CswNbt2DImportTables.ImportRelationships.importdefinitionid, ImportDefinitionId );
                foreach( DataRow RelRow in RelationshipsDataTable.Rows )
                {
                    RowRelationships.Add( new CswNbt2DRowRelationship( _CswNbtResources, RelRow ) );
                }
            }
        } // _loadBindings()
    } // class CswNbt2DDefinition
} // namespace
