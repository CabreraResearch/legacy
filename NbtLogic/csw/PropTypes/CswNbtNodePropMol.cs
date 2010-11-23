using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Nbt.MetaData;
using System.Xml;
using ChemSW.Core;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropMol : CswNbtNodeProp
    {
        public CswNbtNodePropMol (CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp)
            : base(CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp)
        {
            _MolSubField = ( (CswNbtFieldTypeRuleMol) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).MolSubField;
        }
    
        private CswNbtSubField _MolSubField;

        override public bool Empty
        {
            get
            {
                return (0 == Gestalt.Length);
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
                return _CswNbtNodePropData.GetPropRowValue(_MolSubField.Column);
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _MolSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode MolNode = CswXmlDocument.AppendXmlNode( ParentNode, _MolSubField.ToXmlNodeName() );
            CswXmlDocument.SetInnerTextAsCData( MolNode, Mol );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Mol = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _MolSubField.ToXmlNodeName() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Mol = CswTools.XmlRealAttributeName( PropRow[_MolSubField.ToXmlNodeName()].ToString() );
        }


    }//CswNbtNodePropMol

}//namespace ChemSW.Nbt.PropTypes
