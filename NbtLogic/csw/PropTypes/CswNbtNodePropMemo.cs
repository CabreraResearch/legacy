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

    public class CswNbtNodePropMemo : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropMemo( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsMemo;
        }

        public CswNbtNodePropMemo( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _TextSubField = ( (CswNbtFieldTypeRuleMemo) _FieldTypeRule ).TextSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _TextSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Text, x => Text = CswConvert.ToString( x ) ) );
        }

        private CswNbtSubField _TextSubField;


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
                return GetPropRowValue( _TextSubField.Column );
            }

        }//Gestalt

        public string Text
        {
            get
            {
                return GetPropRowValue( _TextSubField.Column );
            }
            set
            {
                SetPropRowValue( _TextSubField.Column, value );
            }
        }

        public Int32 Rows
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaRows == Int32.MinValue )
                    return 4;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaRows;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaRows = value;
            //}
        }
        public Int32 Columns
        {
            get
            {
                if( _CswNbtMetaDataNodeTypeProp.TextAreaColumns == Int32.MinValue )
                    return 40;
                else
                    return _CswNbtMetaDataNodeTypeProp.TextAreaColumns;
            }
            //set
            //{
            //    _CswNbtMetaDataNodeTypeProp.TextAreaColumns = value;
            //}
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_TextSubField.ToXmlNodeName( true )] = Text;
            ParentObject["rows"] = Rows;
            ParentObject["columns"] = Columns;
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

        }

    }//CswNbtNodePropMemo

}//namespace ChemSW.Nbt.PropTypes
