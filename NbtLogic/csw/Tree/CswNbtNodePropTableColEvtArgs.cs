using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;

namespace ChemSW.Nbt
{
    public class CswNbtNodePropTableColEvtArgs: EventArgs
    {

        public CswNbtNodePropTableColEvtArgs( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        private CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources
        {
            get
            {
                return ( _CswNbtResources );
            }
        }//



    }//CswNbtNodePropTableColEvtArgs

}//namespace ChemSW.Nbt
