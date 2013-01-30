using System;
using System.Collections.Generic;
using System.Data;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPassword : CswNbtNodeProp
    {
        private CswEncryption _CswEncryption;

        public static implicit operator CswNbtNodePropPassword( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsPassword;
        }

        public CswNbtNodePropPassword( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            _FieldTypeRule = (CswNbtFieldTypeRulePassword) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _EncryptedPasswordSubField = _FieldTypeRule.EncryptedPasswordSubField;
            _ChangedDateSubField = _FieldTypeRule.ChangedDateSubField;

            _CswEncryption = new CswEncryption( CswNbtResources.MD5Seed );
        }
        private CswNbtFieldTypeRulePassword _FieldTypeRule;
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
                CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClassProp UserPassword = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Password );

                if( this.ObjectClassPropId == UserPassword.ObjectClassPropId )
                {
                    //CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( this.NodeId );
                    if( //null != UserNode &&
                        false == (
                                    _CswNbtResources.Permit.isNodeWritable( CswNbtPermit.NodeTypePermission.Edit, NodeTypeProp.getNodeType(), this.NodeId ) ) &&
                                    _CswNbtResources.Permit.isPropWritable( CswNbtPermit.NodeTypePermission.Edit, NodeTypeProp, null )
                                 )
                    {
                        throw new CswDniException( ErrorType.Warning, "User does not have permission to edit this password", "Permit.can() returned false for UserNode '" + this.NodeId + "'." );
                    }
                }

                if( _CswNbtNodePropData.SetPropRowValue( _EncryptedPasswordSubField.Column, value ) )
                {
                    SyncGestalt();
                    ChangedDate = DateTime.Now;
                }
            }
        }

        public DateTime ChangedDate
        {
            get
            {
                //string StringValue = _CswNbtNodePropData.GetPropRowValue( _ChangedDateSubField.Column );
                //DateTime ReturnVal = DateTime.MinValue;
                //if( StringValue != string.Empty )
                //    ReturnVal = Convert.ToDateTime( StringValue );
                //return ( ReturnVal.Date );
                return _CswNbtNodePropData.GetPropRowValueDate( _ChangedDateSubField.Column );
            }

            set
            {
                if( DateTime.MinValue != value )
                    _CswNbtNodePropData.SetPropRowValue( _ChangedDateSubField.Column, value );
                else
                    _CswNbtNodePropData.SetPropRowValue( _ChangedDateSubField.Column, DBNull.Value );
            }
        }

        private string _validatePassword( string NewPassword )
        {
            if( string.IsNullOrEmpty( NewPassword ) )
            {
                throw new CswDniException( ErrorType.Warning, "Passwords cannot be empty.", "The supplied password was null or empty." );
            }
            string Ret = NewPassword.Trim();
            if( string.IsNullOrEmpty( Ret ) )
            {
                throw new CswDniException( ErrorType.Warning, "Passwords cannot be empty.", "The supplied password was null or empty." );
            }
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Length ) )
            {
                Int32 Length = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Length ) );
                if( Ret.Length < Length )
                {
                    throw new CswDniException( ErrorType.Warning, "Passwords must be at least " + Length + " characters long.", "The supplied password was not long enough." );
                }
            }
            if( _CswNbtResources.ConfigVbls.doesConfigVarExist( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Complexity ) )
            {
                Int32 ComplexityLevel = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Complexity ) );
                if( ComplexityLevel > 0 )
                {
                    if( false == CswTools.HasAlpha( Ret ) || false == CswTools.HasNumber( Ret ) )
                    {
                        throw new CswDniException( ErrorType.Warning, "Password complexity requires that passwords contain at least 1 number and 1 letter.", "The supplied password did not contain both a letter and a number." );
                    }

                    if( ComplexityLevel > 1 )
                    {
                        if( false == CswTools.HasSpecialCharacter( Ret ) )
                        {
                            throw new CswDniException( ErrorType.Warning, "Password complexity requires that passwords contain at least 1 special character.", "The supplied password contained only alphanumeric characters." );
                        }
                    }
                }

            }
            return Ret;
        }

        public string Password
        {
            set
            {
                string NewPassword = _validatePassword( value );
                EncryptedPassword = _CswEncryption.getMd5Hash( NewPassword );
                ChangedDate = DateTime.Now;
            }
        }

        public bool IsExpired
        {
            get
            {
                Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );
                return ( ChangedDate == DateTime.MinValue ||
                         ChangedDate.AddDays( PasswordExpiryDays ).Date <= DateTime.Now.Date );
            }
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_EncryptedPasswordSubField.ToXmlNodeName( true )] = EncryptedPassword;
            ParentObject["passwordcomplexity"] = _CswNbtResources.ConfigVbls.getConfigVariableValue( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Complexity );
            ParentObject["passwordlength"] = _CswNbtResources.ConfigVbls.getConfigVariableValue( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Length );
            ParentObject["newpassword"] = string.Empty;
            ParentObject["isexpired"] = IsExpired.ToString().ToLower();
            ParentObject["expire"] = false;
            ParentObject["isadmin"] = _CswNbtResources.CurrentNbtUser.IsAdministrator().ToString().ToLower();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            EncryptedPassword = CswTools.XmlRealAttributeName( PropRow[_EncryptedPasswordSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_EncryptedPasswordSubField.ToXmlNodeName( true )] )
            {
                EncryptedPassword = JObject[_EncryptedPasswordSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject["newpassword"] )
            {
                _saveProp( JObject["newpassword"].ToString() );
            }
            if( null != JObject["expire"] )
            {
                bool inIsExpired = CswConvert.ToBoolean( JObject["expire"].ToString() );
                if( inIsExpired && !IsExpired )
                {
                    ChangedDate = DateTime.MinValue;
                }
            }
        }

        private void _saveProp( string NewPassword )
        {
            if( NewPassword != string.Empty )
            {
                Password = NewPassword;
            }
        }

        public override void SyncGestalt()
        {
            _CswNbtNodePropData.Gestalt = EncryptedPassword;
        }
    }//CswNbtNodePropPassword

}//namespace ChemSW.Nbt.PropTypes
