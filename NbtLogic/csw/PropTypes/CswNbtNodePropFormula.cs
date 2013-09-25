using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.StructureSearch;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{


    public class CswNbtNodePropFormula : CswNbtNodeProp
    {

        public static implicit operator CswNbtNodePropFormula( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsFormula;
        }

        public CswNbtNodePropFormula( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _RawFormulaTextSubfield = ( (CswNbtFieldTypeRuleFormula) _FieldTypeRule ).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _RawFormulaTextSubfield, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = CswConvert.ToString( x ) ) );
        }

        private CswNbtSubField _RawFormulaTextSubfield;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }

        public string Text
        {
            get
            {
                return GetPropRowValue( _RawFormulaTextSubfield );
            }
            set
            {
                SetPropRowValue( _RawFormulaTextSubfield, value );
                Gestalt = value;
            }
        }
        public Int32 Size
        {
            get
            {
                Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute1 );
                if( Ret <= 0 )
                {
                    Ret = 25;
                }
                return Ret;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.Length = value;
            //}
        }

        public Int32 MaxLength
        {
            get
            {
                Int32 Ret = CswConvert.ToInt32( _CswNbtMetaDataNodeTypeProp.Attribute2 );
                if( Ret <= 0 )
                {
                    Ret = 255;
                }
                return Ret;
            }
        }

        public string RegEx
        {
            get
            {
                return ( _CswNbtMetaDataNodeTypeProp.Attribute3 );
            }
        }

        public string RegExMsg
        {
            get
            {
                return ( _CswNbtMetaDataNodeTypeProp.Attribute4 );
            }
        }


        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        //private string _ElemName_Value = "Value";

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            ParentObject[_RawFormulaTextSubfield.ToXmlNodeName( true )] = Text;
            ParentObject["formattedText"] = _parseChemicalFormula( Text );
            ParentObject["size"] = Size;
            ParentObject["maxlength"] = MaxLength;
            ParentObject["regex"] = RegEx;
            ParentObject["regexmsg"] = RegExMsg;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Text = CswTools.XmlRealAttributeName( PropRow[_RawFormulaTextSubfield.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_RawFormulaTextSubfield.ToXmlNodeName( true )] )
            {
                Text = JObject[_RawFormulaTextSubfield.ToXmlNodeName( true )].ToString();
            }
        }

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Text );
        }

        /// <summary>
        /// Build a version of the chemical formula which includes subscripting and converted chemical names
        /// </summary>
        /// <param name="Formula">The raw ASCII formula to parse</param>
        /// <returns>a unicode string representing the prettified formula</returns>
        private string _parseChemicalFormula( string Formula )
        {
            string FormattedFormula = "";
            Regex FormulaExpression = new Regex( "(\\d*)([a-zA-Z\\d]+)([ .]*)(.*)" );

            //borrow a dictionary of elements from structure search
            PeriodicTable PT = new PeriodicTable();


            while( false == string.IsNullOrEmpty( Formula ) )
            {
                GroupCollection FormulaGroups = FormulaExpression.Match( Formula ).Groups;

                //append prefix numbers without changing them
                FormattedFormula = FormattedFormula + FormulaGroups[1];

                //iterate through the characters of the current compound from left to right
                char[] ChemicalCompound = FormulaGroups[2].ToString().ToCharArray();
                for( int i = 0; i < ChemicalCompound.Length; i++ )
                {
                    char Letter = ChemicalCompound[i];

                    //convert numbers to subscript versions of themselves
                    if( '0' <= Letter && Letter <= '9' )
                        ChemicalCompound[i] = (char) ( Letter + 0x2050 );

                 //determine the proper case for non-element letters
                    else if( false == PT.ElementExists( Letter.ToString() ) && 'A' <= Letter && Letter <= 'Z' )
                    {
                        char LastLetter = i > 0 ? ChemicalCompound[i - 1] : '\0';
                        char NextLetter = i < ChemicalCompound.Length - 1 ? ChemicalCompound[i + 1] : '\0';

                        //if the last letter plus this one lowercased make an element and the next is not already lowered
                        if( NextLetter != Char.ToLower( NextLetter ) && PT.ElementExists( "" + LastLetter + Char.ToLower( Letter ) ) )
                            ChemicalCompound[i] = Char.ToLower( Letter );
                        //if this letter plus the next one lowercased make an element
                        else if( PT.ElementExists( "" + Letter + Char.ToLower( NextLetter ) ) )
                        {
                            ChemicalCompound[i + 1] = Char.ToLower( NextLetter );
                            i = i + 1;
                        }

                    }//else if( false == PT.ElementExists( Letter.ToString() ) && 'A' <= Letter && Letter <= 'Z' )
                }//for( int i = 0; i < ChemicalCompound.Length; i++ )

                //append the prettified compound
                FormattedFormula = FormattedFormula + new string( ChemicalCompound );

                //append trailing characters, with periods converted to big middle dots
                FormattedFormula = FormattedFormula + FormulaGroups[3].ToString().Trim().Replace( '.', (char) 0x25cf );

                //move on to the next compoud in the formula
                Formula = FormulaGroups[4].ToString();

            }//while( false == string.IsNullOrEmpty( Formula ) )



            return FormattedFormula;

        }//_parseChemicalFormula

    }//CswNbtNodePropText

}//namespace ChemSW.Nbt.PropTypes
