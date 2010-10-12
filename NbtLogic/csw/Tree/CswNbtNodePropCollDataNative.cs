using System;
using System.Data;
using System.Collections;
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
            set
            {
                _NodeKey = value;
            }
            get
            {
                return ( _NodeKey );
            }
        }//NodeKey


        private Int32 _NodeTypeId = Int32.MinValue;
        public Int32 NodeTypeId
        {
            set
            {
                _NodeTypeId = value;
            }

            get
            {
                return ( _NodeTypeId );
            }
        }


        private DataTable _PropsTable = null;
        public DataTable PropsTable
        {
            get
            {
                if ( null == _PropsTable )
                {
                    _PropsUpdate = _CswNbtResources.makeCswTableUpdate( "Props_update", "jct_nodes_props" );
                    if ( _NodeKey != null )
                        _PropsTable = _PropsUpdate.getTable( "nodeid", _NodeKey.PrimaryKey );
                    else
                        _PropsTable = _PropsUpdate.getEmptyTable();
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
