using System;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public interface ICswNbtNodeReaderData
    {
        bool fetchNodeInfo( CswNbtNode CswNbtNode, CswDateTime Date );

    }//CswNbtNodeReader

}//namespace ChemSW.Nbt
