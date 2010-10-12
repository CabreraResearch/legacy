using System;
using System.Data;
using System.Collections;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

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
