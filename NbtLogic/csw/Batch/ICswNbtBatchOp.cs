using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Batch
{
    public interface ICswNbtBatchOp
    {

        // This needs to be present, but the syntax will be different for each operation
        //CswNbtObjClassBatchOp makeBatchOp();

        void runBatchOp( CswNbtObjClassBatchOp BatchNode );

    } // interface ICswNbtBatchOp
} // namespace ChemSW.Nbt.Batch
