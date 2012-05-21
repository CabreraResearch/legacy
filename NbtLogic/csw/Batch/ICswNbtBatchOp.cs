using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChemSW.Core;

namespace ChemSW.Nbt.Batch
{
    public interface ICswNbtBatchOp
    {

        // This needs to be present, but the syntax will be different for each operation
        //CswNbtBatchRow makeBatchOp();

        void runBatchOp( CswNbtBatchRow BatchRow );

    } // interface ICswNbtBatchOp
} // namespace ChemSW.Nbt.Batch
