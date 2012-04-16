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

        public abstract void load( bool RequireViewPermissions );

    }//CswNbtTreeLoader

}//namespace ChemSW.Nbt
