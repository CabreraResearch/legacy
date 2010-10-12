using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace ChemSW.Nbt
{

    public class CswNbtNodeTypeProp
    {
        private DataTable _DataTable = null;

        public CswNbtNodeTypeProp(DataTable NodeTypePropTable)
        {
            string Context = "CswNbtNodeTypeProp()::ctor( DataRow ) - ";

            try
            {
                _DataTable = NodeTypePropTable;

            }
            catch (Exception e)
            {
                throw new System.Exception(Context + e.Message);
            }
        }//ctor

        public CswNbtNodeTypeProp()
        {
        }//ctor


        public Object this[string index]
        {
            get { return _DataTable.Rows[0][index]; }
            set { _DataTable.Rows[0][index] = value; }
        }

        public DataTable DataTable
        {
            get { return _DataTable; }
        }

        public DataRow DataRow
        {
            get { return _DataTable.Rows[0]; }
        }

    }//CswNbtNodeTypeProp

}//namespace ChemSW.Nbt
