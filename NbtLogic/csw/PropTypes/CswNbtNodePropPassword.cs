using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPassword : CswNbtNodeProp
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
                string UserName = string.Empty;
                CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClassProp UserPassword = UserOC.getObjectClassProp( CswNbtObjClassUser.PasswordPropertyName );

                if( this.ObjectClassPropId == UserPassword.ObjectClassPropId )
                {
                    CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( this.NodeId );
					if( null != UserNode && 
						!_CswNbtResources.Permit.can( CswNbtPermit.NodeTypePermission.Edit, NodeTypeProp.NodeType, UserNode, NodeTypeProp ) )
                    {
						throw new CswDniException( "User does not have permission to edit this password", "Permit.can() returned false for UserNode '" + UserNode.NodeName + "'." );
					}
                }
                
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
            XmlNode EncryptedPWNode = CswXmlDocument.AppendXmlNode( ParentNode, _EncryptedPasswordSubField.ToXmlNodeName(), EncryptedPassword.ToString() );

            // We don't provide the raw password, but we do provide a node in which someone can set a new password for saving
            XmlNode RawPWNode = CswXmlDocument.AppendXmlNode( ParentNode, "newpassword", "" );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            EncryptedPassword = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _EncryptedPasswordSubField.ToXmlNodeName() );
            string NewPw = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, "newpassword" );
            if( NewPw != string.Empty )
                Password = NewPw;
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
            EncryptedPassword = CswTools.XmlRealAttributeName( PropRow[_EncryptedPasswordSubField.ToXmlNodeName()].ToString() );
        }

    }//CswNbtNodePropPassword

}//namespace ChemSW.Nbt.PropTypes
