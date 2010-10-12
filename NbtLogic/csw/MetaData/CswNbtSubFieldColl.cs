using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;

namespace ChemSW.Nbt.MetaData
{


    public class CswNbtSubFieldColl
    {
        public CswNbtSubFieldColl()
        {
        }//ctor

        private ArrayList _SubFields = new ArrayList();

        public CswNbtSubField Default = null;

        //public void add( CswNbtSubField.PropColumn PropColumn, CswNbtSubField.SubFieldName Name )
        //{
        //    add(PropColumn, Name, false);
        //}
        //public void add( CswNbtSubField.PropColumn PropColumn, CswNbtSubField.SubFieldName Name, bool IsDefault )
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

        public CswNbtSubField this[CswNbtSubField.SubFieldName Name]
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
                //    ReturnVal = this[ CswNbtSubField.PropColumn.Field1 ];
                //}

                return ( ReturnVal );

            }
        }//index by sub field name

        public bool contains( CswNbtSubField.PropColumn Column )
        {
            return ( null != this[Column] );
        }

        public CswNbtSubField this[CswNbtSubField.PropColumn Column]
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
