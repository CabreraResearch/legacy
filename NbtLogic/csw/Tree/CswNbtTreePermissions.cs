using System;
using System.Collections.Generic;
using System.Text;

namespace ChemSW.Nbt
{
    public class CswNbtTreePermissions
    {
        private CswNbtResources _CswNbtResources;
        
        public CswNbtTreePermissions( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public bool  isInsertNodeAllowed( int NodeTypeId ) 
        {
            return( true );
        }//isInsertNodeAllowed()

        public bool  isDeleteNodeAllowed( int NodeTypeId ) 
        {
            return ( true );
        }//isDeleteNodeAllowed()

        public bool  isViewNodeAllowed( int NodeTypeId ) 
        {
            return ( true );
        }//isViewNodeAllowed()

        public bool  isSetNodePropAllowed( int NodeTypeId ) 
        {
            return ( true );
        }//isSetNodePropAllowed()

        public bool  isChangeNodeParentAllowed( int NodeTypeId ) 
        {
            return ( true );
        }//isChangeNodeParentAllowed() 


    }//class CswNbtTreePermissions

}//namespace ChemSW.Nbt
