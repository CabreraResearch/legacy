using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;
//using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.TreeEvents
{
    public class CswNbtTreeEventInsertNodeGeneric
    {

        public static void handleBeforeInsertNode(object CswNbtTree, CswNbtTreeModEventArgs CswNbtTreeModEventArgs)
        {
            CswNbtNode Node = CswNbtTreeModEventArgs.InsertedNode;

            /*
            // Iterate properties on the node
            foreach (CswNbtNodeProp Property in Node.Properties)
            {
                if (Property is CswNbtNodePropBarcode)
                {
                    CswNbtNodePropBarcode Barcode = (CswNbtNodePropBarcode)Property;
                    Barcode.setSequenceValue();
                }
                else if (Property is CswNbtNodePropSequence)
                {
                    CswNbtNodePropSequence Sequence = (CswNbtNodePropSequence)Property;
                    Sequence.setSequenceValue();
                }
            }
             * */

        }//handleBeforeInsertNode()

        public static void handleAfterInsertNode(object CswNbtTree, CswNbtTreeModEventArgs CswNbtTreeModEventArgs)
        {
        }//handleAfterInsertNode()


    }//CswNbtTreeEventInsertNodeGeneric

}//namespace ChemSW.Nbt.TreeEvents
