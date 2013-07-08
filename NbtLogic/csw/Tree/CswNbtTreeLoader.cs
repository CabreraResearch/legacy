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

        public abstract void load( bool RequireViewPermissions, Int32 ResultsLimit = Int32.MinValue );

    }//CswNbtTreeLoader

}//namespace ChemSW.Nbt
