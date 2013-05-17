using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{


    public class CswNbtNodePropCASNo : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropCASNo( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsCASNo;
        }

        public CswNbtNodePropCASNo( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _TextSubField = ((CswNbtFieldTypeRuleCASNo) _FieldTypeRule).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = x ) );
        }

        private CswNbtSubField _TextSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }
        }//Gestalt

        public string Text
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _TextSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _TextSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        /// <summary>
        /// The size of the text field
        /// </summary>
        public Int32 Size
        {
            get
            {
                if( false == String.IsNullOrEmpty( _CswNbtMetaDataNodeTypeProp.Attribute1 ) )
                    return CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                else
                    return 25;
            }
        }

        /// <summary>
        /// If the CAS number regex and checksum are valid, returns empty string.
        /// Otherwise, returns an error message.
        /// </summary>
        public bool Validate( out string ErrorMessage )
        {
            return Validate( Text, out ErrorMessage );
        }

        /// <summary>
        /// If the CAS number regex and checksum are valid, returns empty string.
        /// Otherwise, returns an error message.
        /// </summary>
        public static bool Validate( string CasNo, out string ErrorMessage )
        {
            bool ret = true;
            ErrorMessage = string.Empty;

            // Format
            Regex CasNoRegEx = new Regex( @"^\d{1,7}-\d{2}-\d$" );
            if( false == CasNoRegEx.IsMatch( CasNo ) )
            {
                ret = false;
                ErrorMessage = "Input is not a valid CAS No";
            }
            else
            {
                // Checksum
                Int32 checksum = CswConvert.ToInt32( CasNo[CasNo.Length - 1] );
                Int32 sum = 0;
                Int32 inc = 1;
                for( Int32 i = CasNo.Length - 2; i >= 0; i-- )
                {
                    if( CasNo[i] != '-' )
                    {
                        sum += CswConvert.ToInt32( CasNo[i] ) * inc;
                        inc++;
                    }
                }
                if( sum % 10 != checksum )
                {
                    ret = false;
                    ErrorMessage = "Checksum is invalid";
                }
            }
            return ret;
        } // Validate()

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject["size"] = Size.ToString();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_TextSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_TextSubField.ToXmlNodeName( true )] )
            {
                Text = JObject[_TextSubField.ToXmlNodeName( true )].ToString();
            }
        }

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Text );
        }
    }//CswNbtNodePropText

}//namespace ChemSW.Nbt.PropTypes
