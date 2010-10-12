using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;

namespace ChemSW.Nbt
{
    /// <summary>
    /// The Nodes Collection, to which all Trees subscribe
    /// </summary>
    public class CswNbtNodeFactory
    {
        private CswNbtResources _CswNbtResources;
        //private ICswNbtObjClassFactory _ICswNbtObjClassFactory;
        private CswNbtNodeReader _CswNbtNodeReader;
        private CswNbtNodeWriter _CswNbtNodeWriter;

        public CswNbtNodeWriter CswNbtNodeWriter { get { return _CswNbtNodeWriter; } }


        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodeFactory( CswNbtResources CswNbtResources ) //, ICswNbtObjClassFactory ICswNbtObjClassFactory )
        {
            _CswNbtResources = CswNbtResources;
            //_ICswNbtObjClassFactory = ICswNbtObjClassFactory;
            _CswNbtNodeReader = new CswNbtNodeReader( _CswNbtResources );
            _CswNbtNodeWriter = new CswNbtNodeWriter( _CswNbtResources );
        }

        /// <remark>
        /// We need a NodeTypeId because the NodeId is missing from the HashKey if this is a new node we're about to add
        /// BZ 9930 - Pass in the nodeid
        /// </remark>
        public CswNbtNode make( NodeSpecies NodeSpecies, CswPrimaryKey NodeId, Int32 NodeTypeId, Int32 UniqueID )
        {
            CswNbtNode ReturnVal = new CswNbtNode( _CswNbtResources, NodeTypeId, NodeSpecies, NodeId, UniqueID ); //, _ICswNbtObjClassFactory );

            ReturnVal.OnRequestWriteNode += new CswNbtNode.OnRequestWriteNodeHandler( _CswNbtNodeWriter.write );
            if( OnWriteNode != null )
                ReturnVal.OnRequestWriteNode += new CswNbtNode.OnRequestWriteNodeHandler( OnWriteNode );
            ReturnVal.OnRequestDeleteNode += new CswNbtNode.OnRequestDeleteNodeHandler( _CswNbtNodeWriter.delete );
            if( OnDeleteNode != null )
                ReturnVal.OnRequestDeleteNode += new CswNbtNode.OnRequestDeleteNodeHandler( OnDeleteNode );
            ReturnVal.OnRequestFill += new CswNbtNode.OnRequestFillHandler( _CswNbtNodeReader.completeNodeData );
            ReturnVal.OnRequestFillFromNodeTypeId += new CswNbtNode.OnRequestFillFromNodeTypeIdHandler( _CswNbtNodeReader.fillFromNodeTypeIdWithProps );

            return( ReturnVal );

        }//make()

        public event CswNbtNode.OnRequestWriteNodeHandler OnWriteNode = null;
        public event CswNbtNode.OnRequestDeleteNodeHandler OnDeleteNode = null;

    } // CswNbtNodeFactory()

} // namespace ChemSW.Nbt


