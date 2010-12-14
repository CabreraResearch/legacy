using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt
{
    public class CswNbtPropEnmrtrFiltered : IEnumerator
    {
        CswNbtMetaDataFieldType _FieldType = null;
        private ArrayList _PropList;
        int _Position = -1;


        public IEnumerator GetEnumerator()
        {
            //return new CswNbtPropEnmrtrGeneric( _Props );
            return( this );
        }

        public CswNbtPropEnmrtrFiltered(ArrayList PropList, CswNbtMetaDataFieldType FieldType)
        {
            _FieldType = FieldType;
            _PropList = PropList;
        }

        public bool MoveNext()
        {
            bool ReachedEnd = false;
            bool FoundProp = false;
            while( ! ReachedEnd && ! FoundProp )
            {
                _Position++;

                if( _Position >= ( _PropList.Count - 1 )  )
                {
                    ReachedEnd = true;
                }
                else
                {
                    CswNbtMetaDataFieldType CurrentFieldType = ((CswNbtNodePropWrapper)_PropList[_Position]).FieldType;
                    if( CurrentFieldType == _FieldType )
                    {
                        FoundProp = true;
                    }//
                }//

                
            }//

            return ( ! ReachedEnd );

        }//MoveNext()

        public void Reset()
        {
            _Position = 0;
        }//Reset*(

        public object Current
        {
            get
            {
                try
                {
                    return _PropList[ _Position ];
                }
                catch( IndexOutOfRangeException )
                {
                    throw new InvalidOperationException();
                }

            }//get

        }//Current


    }//CswPropsEnumeratorGeneric
}
