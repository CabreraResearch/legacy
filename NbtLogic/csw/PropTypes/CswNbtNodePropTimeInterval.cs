using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Core;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropTimeInterval : CswNbtNodeProp
    {
        public CswNbtNodePropTimeInterval(CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp)
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            //_RateInterval = new CswRateInterval(CswNbtNodePropData.Gestalt);   //this should be backwards compatible...
            if( CswNbtNodePropData.ClobData.ToString() != string.Empty )
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.LoadXml( CswNbtNodePropData.ClobData.ToString() );
                _RateInterval = new CswRateInterval( XmlDoc.FirstChild );
            }
            else
            {
                _RateInterval = new CswRateInterval();
            }
            _IntervalSubField = ( (CswNbtFieldTypeRuleTimeInterval) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).IntervalSubField;
            _StartDateSubField = ( (CswNbtFieldTypeRuleTimeInterval) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).StartDateSubField;
        }

        private CswNbtSubField _IntervalSubField;
        private CswNbtSubField _StartDateSubField;

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

        private CswRateInterval _RateInterval;
        public CswRateInterval RateInterval
        {
            get
            {
                return _RateInterval;
            }
            set
            {
                _RateInterval = value;
                _CswNbtNodePropData.SetPropRowValue( _IntervalSubField.Column, value.ToString() );
                _CswNbtNodePropData.SetPropRowValue( _StartDateSubField.Column, value.getFirst() );
                _CswNbtNodePropData.Gestalt = value.ToString();
                _CswNbtNodePropData.ClobData = value.ToXmlString();
            }
        }


        public DateTime getNextOccuranceAfter( DateTime DateTimeToAdd )
        {
            return _RateInterval.getNext( DateTimeToAdd );
        }//getNextOccuranceFrom()


        public DateTime getStartDate()
        {
            return _RateInterval.getFirst();
        }//getStartDate()



        private string _ElemName_Rateinterval = "Rateinterval";

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode RateIntervalNode = CswXmlDocument.AppendXmlNode( ParentNode, _IntervalSubField.ToXmlNodeName() );
            CswXmlDocument.SetInnerTextAsCData( RateIntervalNode, RateInterval.ToXmlString() );
            //RateInterval.ToXml( RateIntervalNode );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //RateInterval = new CswRateInterval( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _IntervalSubField.ToXmlNodeName() ) );
            string IntervalXmlAsString = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _IntervalSubField.ToXmlNodeName() );
            XmlDocument Doc = new XmlDocument();
            XmlNode IntervalNode = CswXmlDocument.SetDocumentElement( Doc, _IntervalSubField.ToXmlNodeName() );
            IntervalNode.InnerXml = IntervalXmlAsString;
            
            CswRateInterval NewRateInterval = new CswRateInterval();
            NewRateInterval.ReadXml( IntervalNode );
            // Setting RateInterval triggers the change to the property value -- don't skip this step
            RateInterval = NewRateInterval;
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //RateInterval = new CswRateInterval( PropRow[_IntervalSubField.ToXmlNodeName()].ToString() );
            if( PropRow.Table.Columns.Contains( _IntervalSubField.ToXmlNodeName() ) )
            {
                string IntervalXmlAsString = CswTools.XmlRealAttributeName( PropRow[_IntervalSubField.ToXmlNodeName()].ToString() );
                XmlDocument Doc = new XmlDocument();
                XmlNode IntervalNode = CswXmlDocument.SetDocumentElement( Doc, _IntervalSubField.ToXmlNodeName() );
                IntervalNode.InnerXml = IntervalXmlAsString.Trim();
                
                CswRateInterval NewRateInterval = new CswRateInterval();
                NewRateInterval.ReadXml( IntervalNode );
                // Setting RateInterval triggers the change to the property value -- don't skip this step
                RateInterval = NewRateInterval;
            }
        }

    }//CswNbtNodeProp

}//namespace ChemSW.Nbt.PropTypes
