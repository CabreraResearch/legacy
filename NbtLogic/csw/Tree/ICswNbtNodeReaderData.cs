using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;

namespace ChemSW.Nbt
{

    public interface ICswNbtNodeReaderData
    {
        bool fetchNodeInfo( CswNbtNode CswNbtNode );

    }//CswNbtNodeReader

}//namespace ChemSW.Nbt
