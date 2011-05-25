using System.Data;
using System.Collections;
using System;

namespace ChemSW.Nbt
{
    public enum TreeMode { NLevelDs, DomProxy }


    public interface ICswNbtTreeFactory
    {
        CswNbtResources CswNbtResources { set; get; }


        ICswNbtTree makeTree( TreeMode TreeMode, CswNbtView View ); //, CswNbtTreeKey CswNbtTreeKey );
        //CswNbtNodes Nodes { get; set; }
        CswNbtNodeCollection CswNbtNodeCollection { get; set; }

    }//ICswNbtTreeFactory

}//namespace ChemSW.Nbt

