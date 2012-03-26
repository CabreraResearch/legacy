using System;

namespace ChemSW.Nbt
{
    public abstract class CswNbtTreeLoader
    {
        protected ICswNbtTree _CswNbtTree;

        public CswNbtTreeLoader( ICswNbtTree CswNbtTree ) 
        {
            _CswNbtTree = CswNbtTree;
        }//ctor

        //public abstract void load(Int32 NodeCountLowerBoundExclusive, Int32 NodeCountUpperBoundInclusive);
        //public abstract void load(ref CswNbtNodeKey ParentNodeKey, 
        //                          CswNbtViewRelationship ChildRelationshipToStartWith, 
        //                          Int32 PageSize, 
        //                          bool FetchAllPrior,
        //                          bool SingleLevelOnly, 
        //                          CswNbtNodeKey IncludedKey,
        //                          bool RequireViewPermissions );
        public abstract void load( bool RequireViewPermissions );

    }//CswNbtTreeLoader

}//namespace ChemSW.Nbt
