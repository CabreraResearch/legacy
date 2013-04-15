
namespace ChemSW.Nbt
{
    public interface ICswNbtTreeFactory
    {
        CswNbtResources CswNbtResources { set; get; }


		ICswNbtTree makeTree( CswEnumNbtTreeMode TreeMode, CswNbtView View, bool IsFullyPopulated ); //, CswNbtTreeKey CswNbtTreeKey );
        //CswNbtNodes Nodes { get; set; }
        CswNbtNodeCollection CswNbtNodeCollection { get; set; }

    }//ICswNbtTreeFactory

}//namespace ChemSW.Nbt

