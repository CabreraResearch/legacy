using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{

    public class CswNbtNodePropComposite : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropComposite( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsComposite;
        }

        public CswNbtNodePropComposite( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRuleComposite) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _CachedValueSubField = _FieldTypeRule.CachedValueSubField;
        }
        private CswNbtFieldTypeRuleComposite _FieldTypeRule;
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
        public string TemplateText()
        {
            return _CswNbtMetaDataNodeTypeProp.getCompositeTemplateText();
        }


        public string RecalculateCompositeValue()
        {
            string Value = CswNbtMetaData.TemplateValueToDisplayValue( _CswNbtResources.MetaData.getNodeTypeProps( _CswNbtMetaDataNodeTypeProp.NodeTypeId ), TemplateValue, _CswNbtNodePropData );
            _CswNbtNodePropData.SetPropRowValue( _CachedValueSubField.Column, Value );
            _CswNbtNodePropData.Gestalt = Value;
            _CswNbtNodePropData.PendingUpdate = false;
            return Value;
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_CachedValueSubField.ToXmlNodeName( true )] = CachedValue;
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // Nothing to restore
            PendingUpdate = true;
        }
        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            PendingUpdate = true;
        }

        public override void SyncGestalt()
        {
            string gestaltVal = CswNbtMetaData.TemplateValueToDisplayValue( _CswNbtResources.MetaData.getNodeTypeProps( _CswNbtMetaDataNodeTypeProp.NodeTypeId ), TemplateValue, _CswNbtNodePropData );
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, gestaltVal );
        }
    }//CswNbtNodePropComposite

}//namespace ChemSW.Nbt
