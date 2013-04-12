using System;
using System.Collections;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt
{
    public class CswNbtPropEnmrtrFiltered : IEnumerator
    {
        CswEnumNbtFieldType _FieldType;
        private ArrayList _PropList;
        int _Position = -1;


        public IEnumerator GetEnumerator()
        {
            //return new CswNbtPropEnmrtrGeneric( _Props );
            return ( this );
        }

        public CswNbtPropEnmrtrFiltered( ArrayList PropList, CswEnumNbtFieldType FieldType )
        {
            _FieldType = FieldType;
            _PropList = PropList;
        }

        public bool MoveNext()
        {
            bool ReachedEnd = false;
            bool FoundProp = false;
            while( !ReachedEnd && !FoundProp )
            {
                _Position++;

                if( _Position > ( _PropList.Count - 1 ) )
                {
                    ReachedEnd = true;
                }
                else
                {
                    CswNbtMetaDataFieldType CurrentFieldType = ( (CswNbtNodePropWrapper) _PropList[_Position] ).getFieldType();
                    if( CurrentFieldType.FieldType == _FieldType )
                    {
                        FoundProp = true;
                    }
                }
            }

            return ( !ReachedEnd );

        }//MoveNext()

        public void Reset()
        {
            _Position = -1;
        }//Reset*(

        public object Current
        {
            get
            {
                try
                {
                    return _PropList[_Position];
                }
                catch( IndexOutOfRangeException )
                {
                    throw new InvalidOperationException();
                }

            }//get

        }//Current


    }//CswPropsEnumeratorGeneric
}
