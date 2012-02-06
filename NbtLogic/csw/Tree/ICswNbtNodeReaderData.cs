using System;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt
{

    public interface ICswNbtNodeReaderData
    {
        bool fetchNodeInfo( CswNbtNode CswNbtNode, DateTime Date );

    }//CswNbtNodeReader

}//namespace ChemSW.Nbt
