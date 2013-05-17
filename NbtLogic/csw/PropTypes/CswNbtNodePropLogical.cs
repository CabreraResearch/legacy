using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropLogical : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropLogical( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsLogical;
        }
        public CswNbtNodePropLogical( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _CheckedSubField = ( (CswNbtFieldTypeRuleLogical) _FieldTypeRule ).CheckedSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _CheckedSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Checked, x => Checked = CswConvert.ToTristate( x ) ) );
        }

        private CswNbtSubField _CheckedSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        public CswEnumTristate Checked
        {
            get
            {
                return CswConvert.ToTristate( _CswNbtNodePropData.GetPropRowValue( _CheckedSubField.Column ) );
            }
            set
            {
                object val = CswConvert.ToDbVal( value );
                if( val != DBNull.Value )
                {
                    _CswNbtNodePropData.SetPropRowValue( _CheckedSubField.Column, val );
                }
                else
                {
                    _CswNbtNodePropData.SetPropRowValue( _CheckedSubField.Column, string.Empty );
                }
                _CswNbtNodePropData.Gestalt = toLogicalGestalt( value );
            }
        }

        public override string ValueForNameTemplate
        {
            get
            {
                // For Logicals, return the property name
                string ret = string.Empty;
                if( Checked == CswEnumTristate.True )
                {
                    ret = PropName;
                }
                return ret;
            }
        } // ValueForNameTemplate


        public static string toLogicalGestalt( CswEnumTristate Tristate )
        {
            object val = CswConvert.ToDbVal( Tristate );
            string Ret = string.Empty;
            if( val != DBNull.Value )
            {
                Ret = CswConvert.ToDisplayString( Tristate );
            }
            return Ret;
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_CheckedSubField.ToXmlNodeName( true )] = Checked.ToString().ToLower();
            ParentObject[CswEnumNbtSubFieldName.Required.ToString()] = Required;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Checked = CswConvert.ToTristate( PropRow[_CheckedSubField.ToXmlNodeName()] );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_CheckedSubField.ToXmlNodeName( true )] )
            {
                Checked = CswConvert.ToTristate( JObject[_CheckedSubField.ToXmlNodeName( true )].ToString() );
            }
        }

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, toLogicalGestalt( Checked ) );
        }
    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
