using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public interface ICswNbtNodeWriterImpl
    {
        void clear();
        void makeNewNodeEntry( CswNbtNode Node, bool PostToDatabase );
        void write( CswNbtNode Node, bool ForceSave, bool IsCopy, bool AllowAuditing );
        void updateRelationsToThisNode( CswNbtNode Node );
        void delete( CswNbtNode CswNbtNode );
        void AuditInsert( CswNbtNode Node );
    }//CswNbtNodeWriter

}//namespace ChemSW.Nbt
