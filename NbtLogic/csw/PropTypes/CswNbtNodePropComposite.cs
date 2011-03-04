using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropComposite : CswNbtNodeProp
    {
        public CswNbtNodePropComposite( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _CachedValueSubField = ( (CswNbtFieldTypeRuleComposite) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CachedValueSubField;
        }

        private CswNbtSubField _CachedValueSubField;

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

        }

        public string CachedValue
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _CachedValueSubField.Column );
            }
        }

        public string TemplateValue
        {
            get { return _CswNbtMetaDataNodeTypeProp.CompositeTemplateValue; }
        }
        public string TemplateText
        {
            get { return _CswNbtMetaDataNodeTypeProp.CompositeTemplateText; }
        }


        public string RecalculateCompositeValue()
        {
            string Value = CswNbtMetaData.TemplateValueToDisplayValue( _CswNbtMetaDataNodeTypeProp.NodeType.NodeTypeProps, TemplateValue, _CswNbtNodePropData );
            _CswNbtNodePropData.SetPropRowValue( _CachedValueSubField.Column, Value );
            _CswNbtNodePropData.Gestalt = Value;
            _CswNbtNodePropData.PendingUpdate = false;
            return Value;
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode CachedValueNode = CswXmlDocument.AppendXmlNode( ParentNode, _CachedValueSubField.ToXmlNodeName(), CachedValue );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
            PendingUpdate = true;
        }

        public override void ToXElement( XElement ParentNode )
        {
            throw new NotImplementedException();
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            throw new NotImplementedException();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
            PendingUpdate = true;
        }

    }//CswNbtNodePropComposite

}//namespace ChemSW.Nbt
