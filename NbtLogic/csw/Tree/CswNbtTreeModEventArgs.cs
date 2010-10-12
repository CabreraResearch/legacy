using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt
{
    public class CswNbtTreeModEventArgs: EventArgs
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtTreeModEventArgs( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }//ctor

        public Hashtable NodeKeys = new Hashtable();

        public CswNbtNode _InsertedNode = null;
        public CswNbtNode InsertedNode
        {
            set
            {
                _InsertedNode = value;
            }//set

            get
            {
                return( _InsertedNode );
            }//get

        }//Inserted Node

        //public CswDbResources CswDbResources
        //{
        //    get
        //    {
        //        return ( _CswNbtResources.CswDataObjects.CswDbResources );
        //    }
        //}



    }//CswNbtTreeModEventArgs

}//namespace ChemSW.Nbt
