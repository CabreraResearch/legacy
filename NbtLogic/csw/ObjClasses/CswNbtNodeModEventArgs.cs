using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtNodeModEventArgs: EventArgs
    {

        public CswNbtNodeModEventArgs( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        private CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources
        {

            get
            {
                return ( _CswNbtResources );
            }//
        }//


    }//CswNbtNodeModEventArgs

}//namespace ChemSW.Nbt.ObjClasses
