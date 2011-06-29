using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

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
					if( _NodeKey == null )
					{
						_PropsTable = _PropsUpdate.getEmptyTable();
					}
					else if( Date == DateTime.MinValue )
					{
						_PropsUpdate = _CswNbtResources.makeCswTableUpdate( "Props_update", "jct_nodes_props" );
						_PropsTable = _PropsUpdate.getTable( "nodeid", _NodeKey.PrimaryKey );
					}
					else
					{
						_PropsUpdate = _CswNbtResources.makeCswTableUpdate( "Props_update_audit", "jct_nodes_props_audit" );
						OrderByClause OrderBy = new OrderByClause( "recordcreated", OrderByType.Descending );
						_PropsTable = _PropsUpdate.getTable( null, "nodeid", _NodeKey.PrimaryKey, "where recordcreated <= '" + _CswNbtResources.getDbNativeDate( Date ) + "'", false, new Collection<OrderByClause>() { OrderBy } );
					}
				}
                return ( _PropsTable );
            }
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
