using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{

    public interface ICswNbtNodeWriterImpl
    {
        void clear();
        void makeNewNodeEntry( CswNbtNode Node, bool PostToDatabase );
        void write( CswNbtNode Node, bool ForceSave, bool IsCopy );
        void updateRelationsToThisNode( CswNbtNode Node );
        void delete( CswNbtNode CswNbtNode );

    }//CswNbtNodeWriter

}//namespace ChemSW.Nbt
