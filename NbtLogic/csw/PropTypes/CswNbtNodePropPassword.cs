using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Encryption;
using System.Xml;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPassword: CswNbtNodeProp
    {
        private CswEncryption _CswEncryption;

        public CswNbtNodePropPassword( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _EncryptedPasswordSubField = ( (CswNbtFieldTypeRulePassword) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).EncryptedPasswordSubField;
            _ChangedDateSubField = ( (CswNbtFieldTypeRulePassword) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).ChangedDateSubField;

            _CswEncryption = new CswEncryption( CswNbtResources.MD5Seed );
        }

        private CswNbtSubField _EncryptedPasswordSubField;
        private CswNbtSubField _ChangedDateSubField;

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
            }//

        }//Gestalt

        public string EncryptedPassword
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _EncryptedPasswordSubField.Column );
            }
            set
            {
                _CswNbtNodePropData.SetPropRowValue( _EncryptedPasswordSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
                ChangedDate = DateTime.Now;
            }
        }

        public DateTime ChangedDate
        {
            get
            {
                string StringValue = _CswNbtNodePropData.GetPropRowValue( _ChangedDateSubField.Column );
                DateTime ReturnVal = DateTime.MinValue;
                if( StringValue != string.Empty )
                    ReturnVal = Convert.ToDateTime( StringValue );
                return ( ReturnVal.Date );
            }

            set
            {
                if( DateTime.MinValue != value )
                    _CswNbtNodePropData.SetPropRowValue( _ChangedDateSubField.Column, value.Date.ToShortDateString() );
                else
                    _CswNbtNodePropData.SetPropRowValue( _ChangedDateSubField.Column, DBNull.Value );
            }
        }

        public string Password
        {
            set
            {
                EncryptedPassword = _CswEncryption.getMd5Hash( value );
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode PWNode = CswXmlDocument.AppendXmlNode( ParentNode, _EncryptedPasswordSubField.ToXmlNodeName(), EncryptedPassword.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            EncryptedPassword = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _EncryptedPasswordSubField.ToXmlNodeName() );
        }
        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            EncryptedPassword = CswTools.XmlRealAttributeName( PropRow[_EncryptedPasswordSubField.ToXmlNodeName()].ToString() );
        }

    }//CswNbtNodePropPassword

}//namespace ChemSW.Nbt.PropTypes
