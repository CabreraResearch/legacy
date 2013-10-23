using System;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    interface ICswNbtNodePropCollData
    {
        DataTable PropsTable { get; }
        bool IsTableEmpty {get;}
        void refreshTable();
        void update();

    }//ICswNbtNodePropCollData


}//namespace ChemSW.Nbt
