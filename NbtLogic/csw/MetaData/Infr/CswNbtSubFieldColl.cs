using System;
using System.Collections;
using ChemSW.Core;

namespace ChemSW.Nbt.MetaData
{


    public class CswNbtSubFieldColl
    {
        public CswNbtSubFieldColl()
        {
        }//ctor

        private ArrayList _SubFields = new ArrayList();

        public CswNbtSubField Default = null;

        //public void add( CswEnumNbtPropColumn PropColumn, CswEnumNbtSubFieldName Name )
        //{
        //    add(PropColumn, Name, false);
        //}
        //public void add( CswEnumNbtPropColumn PropColumn, CswEnumNbtSubFieldName Name, bool IsDefault )
        //{
        //    if( null != this[PropColumn] )
        //        throw ( new CswDniException( "Collection already contains a column definition for " + PropColumn.ToString() ) );

        //    if( null != this[Name] )
        //        throw ( new CswDniException( "Collection already contains a name" + Name ) );

        //    CswNbtSubField SubField = new CswNbtSubField(PropColumn, Name);
        //    add( SubField, IsDefault );
        //}
        public void add( CswNbtSubField SubField, bool IsDefault )
        {
            _SubFields.Add( SubField );

            if( IsDefault || Default == null )
                Default = SubField;
        }
        public void add( CswNbtSubField SubField )
        {
            add( SubField, false );
        }

        public void remove( CswNbtSubField SubField )
        {
            _SubFields.Remove( SubField );
        }

        public IEnumerator GetEnumerator()
        {
            return ( new CswEnmrtrGeneric( _SubFields ) );
        }//GetEnumerator()

        public Int32 Count
        {
            get
            {
                return ( _SubFields.Count );
            }
        }

        public CswNbtSubField this[CswEnumNbtSubFieldName Name]
        {
            get
            {
                CswNbtSubField ReturnVal = null;

                //if( string.Empty != Name )//bz # 6629: Default behavior when no subfield name is given
                //{

                foreach( CswNbtSubField CurrentSubField in this )
                {
                    if( CurrentSubField.Name == Name )
                    {
                        ReturnVal = CurrentSubField;
                        break;
                    }
                }
                //}
                //else
                //{
                //    ReturnVal = this[ CswEnumNbtPropColumn.Field1 ];
                //}

                return ( ReturnVal );

            }
        }//index by sub field name

        public bool contains( CswEnumNbtPropColumn Column )
        {
            return ( null != this[Column] );
        }

        public CswNbtSubField this[CswEnumNbtPropColumn Column]
        {
            get
            {

                CswNbtSubField ReturnVal = null;

                foreach( CswNbtSubField CurrentSubField in this )
                {
                    if( CurrentSubField.Column == Column )
                    {
                        ReturnVal = CurrentSubField;
                        break;
                    }
                }

                return ( ReturnVal );

            }

        }//index by sub field name

    }//CswNbtSubFieldColl


}//namespace ChemSW.Nbt.MetaData
