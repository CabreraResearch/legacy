using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropCollDataNative: ICswNbtNodePropCollData
    {
        private CswNbtResources _CswNbtResources = null;
        private CswTableUpdate _PropsUpdate = null;
        private CswNbtNodePropCollDataRelational _propCollRelational = null;
        public string _DebugId;

        public CswNbtNodePropCollDataNative( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _DebugId = DateTime.Now.ToString();
            _propCollRelational = new CswNbtNodePropCollDataRelational( _CswNbtResources );
        }//ctor

        private CswPrimaryKey _NodeKey = null;
        public CswPrimaryKey NodePk
        {
            set { _NodeKey = value; }
            get { return ( _NodeKey ); }
        }//NodeKey

        private CswPrimaryKey _RelationalId = null;
        public CswPrimaryKey RelationalId
        {
            set { _RelationalId = value; }
            get { return ( _RelationalId ); }
        }//RelationalId

        private Int32 _NodeTypeId = Int32.MinValue;
        public Int32 NodeTypeId
        {
            set { _NodeTypeId = value; }
            get { return ( _NodeTypeId ); }
        }

        private DateTime _Date = DateTime.MinValue;
        public DateTime Date
        {
            get { return _Date; }
            set { _Date = value; }
        }

        private DataTable _PropsTable = null;
        public DataTable PropsTable
        {
            get
            {
                if( null == _PropsTable )
                {
                    _PropsUpdate = _CswNbtResources.makeCswTableUpdate( "Props_update", "jct_nodes_props" );
                    if( _NodeKey == null )
                    {
                        _PropsTable = _PropsUpdate.getEmptyTable();
                    }
                    else
                    {
                        _PropsTable = _PropsUpdate.getTable( "nodeid", _NodeKey.PrimaryKey );
                        if( Date != DateTime.MinValue )
                        {
                            _PropsTable.Columns.Add( "auditchanged" );

                            CswTableSelect PropsAuditSelect = _CswNbtResources.makeCswTableSelect( "Props_update_audit", "jct_nodes_props_audit" );
                            OrderByClause OrderBy = new OrderByClause( "recordcreated", CswEnumOrderByType.Descending );
                            DataTable PropsAuditTable = PropsAuditSelect.getTable( null, "nodeid", _NodeKey.PrimaryKey, "where recordcreated <= " + _CswNbtResources.getDbNativeDate( Date.AddSeconds( 1 ) ), false, new Collection<OrderByClause>() { OrderBy } );

                            // reconcile
                            foreach( DataRow PropRow in _PropsTable.Rows )
                            {
                                Int32 a = 0;
                                bool FoundMatch = false;
                                while( !FoundMatch && a < PropsAuditTable.Rows.Count )
                                {
                                    DataRow AuditRow = PropsAuditTable.Rows[a];
                                    if( CswConvert.ToInt32( AuditRow["jctnodepropid"] ) == CswConvert.ToInt32( PropRow["jctnodepropid"] ) )
                                    {
                                        FoundMatch = true;
                                        if( CswConvert.ToDateTime( AuditRow["recordcreated"] ) == Date )
                                        {
                                            PropRow["auditchanged"] = CswConvert.ToDbVal( true );
                                        }
                                        foreach( DataColumn Column in PropsTable.Columns )
                                        {
                                            if( PropsAuditTable.Columns.Contains( Column.ColumnName ) )
                                            {
                                                PropRow[Column.ColumnName] = AuditRow[Column.ColumnName];
                                            }
                                        }
                                    }
                                    a++;
                                } // while( !FoundMatch && a < PropsAuditTable.Rows.Count )
                            } // foreach( DataRow PropRow in _PropsTable.Rows )
                        } // if( Date != DateTime.MinValue )
                    } // if-else( _NodeKey == null )
                } // if( null == _PropsTable )
                return ( _PropsTable );
            } // get
        }//PropsTable



        public bool IsTableEmpty
        {
            get
            {
                return ( null == _PropsTable );
            }

        }

        public void refreshTable()
        {
            //CswTableCaddy RefillPropsCaddy = _CswNbtResources.makeCswTableCaddy( "jct_nodes_props" );
            //RefillPropsCaddy.FilterColumn = "jctnodepropid";
            //DataTable PropsRefillTable = RefillPropsCaddy[ CswPrimaryKey.PrimaryKey ].Table;
            _PropsTable = null;
        }//refreshTable() 



        public void update()
        {
            _PropsUpdate.update( _PropsTable );

            if( CswTools.IsPrimaryKey( RelationalId ) && "nodes" != RelationalId.TableName.ToLower() )
            {
                _propCollRelational.update( NodeTypeId, RelationalId, _PropsTable );
            }
        }


    }//CswNbtNodePropCollDataNative


}//namespace ChemSW.Nbt
