using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public interface ICswNbtNodeWriterImpl
    {
        void clear();
        void makeNewNodeEntry( CswNbtNode Node );
        void write( CswNbtNode Node, bool ForceSave, bool IsCopy );
        void updateRelationsToThisNode( CswNbtNode Node );
        void delete( CswNbtNode CswNbtNode );

    }//CswNbtNodeWriter

}//namespace ChemSW.Nbt
