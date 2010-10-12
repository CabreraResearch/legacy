using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{


    public class CswNbtFieldResources
    {
        public CswNbtResources CswNbtResources = null;
        public CswNbtPropFilterSql CswNbtPropFilterSql = null;
        //public CswNbtSubFieldColl SubFields = null;


        public CswNbtFieldResources( CswNbtResources CswNbtResourcesIn )
        {
            CswNbtResources = CswNbtResourcesIn;
            CswNbtPropFilterSql = new CswNbtPropFilterSql();
            //SubFields = new CswNbtSubFieldColl();
        }

    }//CswNbtFieldResources


}//namespace ChemSW.Nbt.MetaData
