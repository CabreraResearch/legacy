using System.Data;
using ChemSW.MtSched.Core;

namespace ChemSW.Nbt.Sched
{
    public class CswScheduleLogicParamNbt : ICswScheduleLogicPersistedDetailParam
    {
        private DataRow _DataRow = null;
        private string  _ColName_ParameterName = string.Empty;
        private string _ColName_ParameterValue = string.Empty;
        public CswScheduleLogicParamNbt( DataRow DataRow, string ColName_ParameterName , string ColName_ParameterValue  )
        {
            _DataRow = DataRow;
            _ColName_ParameterName = ColName_ParameterName;
            _ColName_ParameterValue = ColName_ParameterValue;
        }//ctor

        public string Name
        {
            get
            {
                return ( _DataRow[_ColName_ParameterName].ToString() ); 
            }

            set
            {
                _DataRow[_ColName_ParameterName] = value; 
            }
        }


        public string Value
        {
            get
            {
                return ( _DataRow[_ColName_ParameterValue].ToString() );
            }

            set
            {
                _DataRow[_ColName_ParameterValue] = value;
            }
        }


    }//CswScheduleLogicParam

}//namespace ChemSW.MtSched


