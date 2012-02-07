using System;
using System.Data;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    interface ICswNbtNodePropCollData
    {

        DataTable PropsTable { get; }
        CswPrimaryKey NodePk {set; get;}
        Int32 NodeTypeId {get; set;}
        bool IsTableEmpty {get;}
        void refreshTable();
        void update();

    }//ICswNbtNodePropCollData


}//namespace ChemSW.Nbt
