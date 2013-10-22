using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropCollDataNative : ICswNbtNodePropCollData
    {
        private CswNbtResources _CswNbtResources = null;
        private CswTableUpdate _PropsUpdate = null;
        public string _DebugId;

        public CswNbtNodePropCollDataNative( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            _DebugId = DateTime.Now.ToString();
        }//ctor

        private CswPrimaryKey _NodeKey = null;
        public CswPrimaryKey NodePk
        {
            set { _NodeKey = value; }
            get { return ( _NodeKey ); }
        }//NodeKey

        private Int32 _NodeTypeId = Int32.MinValue;
        public Int32 NodeTypeId
        {
            set { _NodeTypeId = value; }
            get { return ( _NodeTypeId ); }
        }

        private CswDateTime _Date = null;
        public CswDateTime Date
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
                        if( false == CswTools.IsDate( Date ) )
                        {
                            _PropsTable = _PropsUpdate.getTable( "nodeid", _NodeKey.PrimaryKey );
                        }
                        else
                        {
                            // see case 30702 - we're only using audit data here, not live data
                            //string Sql = "select t.*, '' as auditchanged " +
                            //             "  from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtResources, "jct_nodes_props", Date, _NodeKey.PrimaryKey ) + " t ";

                            string Sql = @"select a.*, '' as auditchanged
                                             from jct_nodes_props_audit a
                                            where a.auditeventtype <> 'PhysicalDelete'
                                              and a.nodeid = " + _NodeKey.PrimaryKey + @"
                                              and a.jctnodespropsauditid = (select max(jctnodespropsauditid)
                                                                              from jct_nodes_props_audit a2
                                                                             where a2.recordcreated <= " + _CswNbtResources.getDbNativeDate( Date.ToDateTime().AddSeconds( 1 ) ) + @"
                                                                               and a2.jctnodepropid = a.jctnodepropid)";

                            CswArbitrarySelect PropsSelect = _CswNbtResources.makeCswArbitrarySelect( "propcolldata_audit_select", Sql );
                            _PropsTable = PropsSelect.getTable();
                            foreach( DataRow AuditRow in _PropsTable.Rows )
                            {
                                if( CswDateTime.EqualsNoMs( CswConvert.ToDateTime( AuditRow["recordcreated"] ), Date.ToDateTime() ) )
                                {
                                    AuditRow["auditchanged"] = CswConvert.ToDbVal( true );
                                }
                            }
                        }
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
        }


    }//CswNbtNodePropCollDataNative


}//namespace ChemSW.Nbt
