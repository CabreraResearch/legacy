using System;
using System.Collections.ObjectModel;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public class CswNbtNodePropCollDataNative : ICswNbtNodePropCollData
    {
        private CswNbtResources _CswNbtResources = null;
        private CswTableUpdate _PropsUpdate = null;
        private CswNbtNode _Node;
        public string _DebugId;

        public CswNbtNodePropCollDataNative( CswNbtResources CswNbtResources, CswNbtNode Node )
        {
            _CswNbtResources = CswNbtResources;
            _Node = Node;
            _DebugId = DateTime.Now.ToString();
        }//ctor

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
                    if( _Node.NodeId == null )
                    {
                        _PropsTable = _PropsUpdate.getEmptyTable();
                    }
                    else
                    {
                        if( false == CswTools.IsDate( Date ) )
                        {
                            _PropsTable = _PropsUpdate.getTable( "nodeid", _Node.NodeId.PrimaryKey );
                        }
                        else
                        {
                            // see case 30702 - we're only using audit data here, not live data
                            //string Sql = "select t.*, '' as auditchanged " +
                            //             "  from " + CswNbtAuditTableAbbreviation.getAuditTableSql( _CswNbtResources, "jct_nodes_props", Date, _NodeKey.PrimaryKey ) + " t ";

                            string Sql = @"select a.*, '' as auditchanged
                                             from jct_nodes_props_audit a
                                            where a.auditeventtype <> 'PhysicalDelete'
                                              and a.nodeid = " + _Node.NodeId.PrimaryKey + @"
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
                    } // if-else( _Node.NodeId == null )
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
            _PropsTable = null;
        }//refreshTable() 

        public void update()
        {
            _PropsUpdate.update( _PropsTable, ( false == _Node.IsTemp ) );
        }

    }//CswNbtNodePropCollDataNative


}//namespace ChemSW.Nbt
