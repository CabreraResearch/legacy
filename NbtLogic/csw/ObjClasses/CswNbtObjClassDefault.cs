using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassDefault : CswNbtObjClass
    {
        public CswNbtObjClassDefault( CswNbtResources CswNbtResources, CswNbtNode CswNbtNode )
            : base( CswNbtResources, CswNbtNode )
        {
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtResources.UnknownEnum ); }
        }
    }//CswNbtObjClassDefault

}//namespace ChemSW.Nbt.ObjClasses
