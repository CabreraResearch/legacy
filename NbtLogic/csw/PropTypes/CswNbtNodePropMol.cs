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
    public class CswNbtNodePropMol: CswNbtNodeProp
    {
        public static readonly string MolImgFileName = "mol.jpeg";
        public static readonly string MolImgFileContentType = "image/jpeg";

        public static implicit operator CswNbtNodePropMol( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsMol;
        }

        public CswNbtNodePropMol( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _MolSubField = ((CswNbtFieldTypeRuleMol)_FieldTypeRule).MolSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _MolSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Mol, x => Mol = x ) );
        }

        private CswNbtSubField _MolSubField;

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

        public string Mol
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _MolSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _MolSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }


        public static string getLink( Int32 JctNodePropId, CswPrimaryKey NodeId )
        {
            string ret = string.Empty;
            if( JctNodePropId != Int32.MinValue && NodeId != null )
            {
                ret = CswNbtNodePropBlob.getLink( JctNodePropId, NodeId, UseNodeTypeAsPlaceholder : true );
            }
            return ret;
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_MolSubField.ToXmlNodeName( true )] = Mol;
            ParentObject["column"] = _MolSubField.Column.ToString().ToLower();
            ParentObject[CswEnumNbtSubFieldName.Href.ToString().ToLower()] = getLink( JctNodePropId, NodeId );
            ParentObject["placeholder"] = "Images/icons/300/_placeholder.gif";
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Mol = CswTools.XmlRealAttributeName( PropRow[_MolSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_MolSubField.ToXmlNodeName( true )] )
            {
                Mol = JObject[_MolSubField.ToXmlNodeName( true )].ToString();
            }
        }
        
        public override void SyncGestalt()
        {
            _CswNbtNodePropData.SetPropRowValue( CswEnumNbtPropColumn.Gestalt, Mol );
        }
    }//CswNbtNodePropMol

}//namespace ChemSW.Nbt.PropTypes
