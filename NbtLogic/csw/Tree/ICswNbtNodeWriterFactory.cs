using System;
using System.Collections.Generic;
using System.Text;
using ChemSW.Exceptions;
using ChemSW.Nbt;

namespace ChemSW.Nbt
{
    public interface ICswNbtNodeWriterFactory 
    {

        CswNbtNodeWriter makeNodeWriter( CswNbtResources CswNbtResources );

    }//CswNbtNodeWriterFactory

}//namespace ChemSW.Nbt
