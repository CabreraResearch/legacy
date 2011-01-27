using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Config;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropQuestion : CswNbtNodeProp
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropQuestion( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            if ( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Question )
            {
                throw ( new CswDniException( "A data consistency problem occurred",
                                            "CswNbtNodePropQuestion() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            _AnswerSubField = ( (CswNbtFieldTypeRuleQuestion)CswNbtMetaDataNodeTypeProp.FieldTypeRule ).AnswerSubField;
            _CommentsSubField = ( (CswNbtFieldTypeRuleQuestion)CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CommentsSubField;
            _CorrectiveActionSubField = ( (CswNbtFieldTypeRuleQuestion)CswNbtMetaDataNodeTypeProp.FieldTypeRule ).CorrectiveActionSubField;
            _DateAnsweredSubField = ( (CswNbtFieldTypeRuleQuestion)CswNbtMetaDataNodeTypeProp.FieldTypeRule ).DateAnsweredSubField;
            _DateCorrectedSubField = ( (CswNbtFieldTypeRuleQuestion)CswNbtMetaDataNodeTypeProp.FieldTypeRule ).DateCorrectedSubField;
            _IsCompliantSubField = ( (CswNbtFieldTypeRuleQuestion)CswNbtMetaDataNodeTypeProp.FieldTypeRule ).IsCompliantSubField;

        }//ctor

        private CswNbtSubField _AnswerSubField;
        private CswNbtSubField _CommentsSubField;
        private CswNbtSubField _CorrectiveActionSubField;
        private CswNbtSubField _DateAnsweredSubField;
        private CswNbtSubField _DateCorrectedSubField;
        private CswNbtSubField _IsCompliantSubField;

        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        override public bool Empty
        {
            get { return ( 0 == AllowedAnswers.Count ); }
        }

        /// <summary>
        /// Text value of property
        /// </summary>
        override public string Gestalt
        {
            get { return _CswNbtNodePropData.Gestalt; }
        }

        /// <summary>
        /// Answer value as string
        /// </summary>
        public string Answer
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _AnswerSubField.Column ); }
            set
            {
                string AnswerVal = value;
                
                DateTime UpdateDateAnswered = DateTime.MinValue;
                if ( !string.IsNullOrEmpty( AnswerVal ) )
                {
                    UpdateDateAnswered = DateTime.Today;
                }

                DateAnswered = UpdateDateAnswered;
                //IsCompliant = _IsCompliant;
                _CswNbtNodePropData.SetPropRowValue( _AnswerSubField.Column, AnswerVal );

                _synchGestalt( AnswerVal );
            }
        }

        /// <summary>
        /// Corrective action taken to change corrected status, as string
        /// </summary>
        public String CorrectiveAction
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _CorrectiveActionSubField.Column ); }
            set 
            {
                String val = value;
                DateTime UpdateDateCorrected = DateTime.Today;

                if( string.IsNullOrEmpty( val ) )
                {
                    UpdateDateCorrected = DateTime.MinValue;
                }
                
                DateCorrected = UpdateDateCorrected;
                //IsCompliant = _IsCompliant;
                _CswNbtNodePropData.SetPropRowValue( _CorrectiveActionSubField.Column, val );
            }
        }

        /// <summary>
        /// True if Answer is compliant or Corrective Action is not empty
        /// </summary>
        public bool IsCompliant
        {
            get { return ( CompliantAnswers.Contains( Answer ) || string.Empty != CorrectiveAction ); }
        }

        /// <summary>
        /// Date value set when question is answered
        /// </summary>
        public DateTime DateAnswered
        {
            get { return CswConvert.ToDateTime( _CswNbtNodePropData.GetPropRowValue( _DateAnsweredSubField.Column ) ); }
            private set
            {
                _CswNbtNodePropData.SetPropRowValue( _DateAnsweredSubField.Column, value );
            }
        }

        /// <summary>
        /// Date value set when question is marked corrected
        /// </summary>
        public DateTime DateCorrected
        {
            get { return CswConvert.ToDateTime( _CswNbtNodePropData.GetPropRowValue( _DateCorrectedSubField.Column ) ); }
            private set
            {
                _CswNbtNodePropData.SetPropRowValue( _DateCorrectedSubField.Column, value );
            }
        }

        /// <summary>
        /// Text (clob) value with comments on question
        /// </summary>
        public string Comments
        {
            get { return _CswNbtNodePropData.GetPropRowValue( _CommentsSubField.Column ); }
            set { _CswNbtNodePropData.SetPropRowValue( _CommentsSubField.Column, value ); }
        }

        /// <summary>
        /// Bool representing the state of compliance by comparing the answer against the list of property's Compliant answer attribute 
        /// </summary>
        //public bool IsCompliant
        //{
        //    get { return CswConvert.ToBoolean( _CswNbtNodePropData.GetPropRowValue( _IsCompliantSubField.Column ) ); }
        //    private set
        //    {
        //        _CswNbtNodePropData.SetPropRowValue( _IsCompliantSubField.Column, value );
        //    }
        //}

        private void _synchGestalt(String GestaltValue)
        {
            _CswNbtNodePropData.SetPropRowValue( CswNbtSubField.PropColumn.Gestalt, GestaltValue );
        }

        //Begin NTP attributes
        /// <summary>
        /// Property name
        /// </summary>
        public string Question
        {
            get { return _CswNbtMetaDataNodeTypeProp.PropName; }
        }

        private CswCommaDelimitedString _CompliantAnswers = null;

        /// <summary>
        /// StringCollection of compliant answers
        /// </summary>
        public CswCommaDelimitedString CompliantAnswers
        {
            get
            {
                if( _CompliantAnswers == null )
                {
                    _CompliantAnswers = new CswCommaDelimitedString();
                    _CompliantAnswers.FromString( _CswNbtMetaDataNodeTypeProp.ValueOptions );
                }
                return _CompliantAnswers;
            }
        }
        /// <summary>
        /// String value for compliant answers
        /// </summary>
        public string CompliantAnswersString
        {
            get
            {
                return _CswNbtMetaDataNodeTypeProp.ValueOptions;
            }
        }

        private CswCommaDelimitedString _AllowedAnswers = null;

        /// <summary>
        /// List of answers used to populate Answer picklist
        /// </summary>
        public CswCommaDelimitedString AllowedAnswers
        {
            get
            {
                if( null == _AllowedAnswers || 
                    _AllowedAnswers.ToString() != _CswNbtMetaDataNodeTypeProp.ListOptions ) // Case 20629
                {
                    _AllowedAnswers = new CswCommaDelimitedString();
                    _AllowedAnswers.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions );

                    if( _AllowedAnswers.Count == 0 )
                    {
                        _AllowedAnswers.Add( "" );
                        _AllowedAnswers.Add( "Yes" );
                        _AllowedAnswers.Add( "No" );
                        _AllowedAnswers.Add( "N/A" );
                    }
                    else
                    {
                        _AllowedAnswers.Insert( 0, "" );
                    }
                } // if( _AllowedAnswers == null )
                return _AllowedAnswers;
            } // get
        } // AllowedAnswers

        /// <summary>
        /// String value for allowed answers
        /// </summary>
        public string AllowedAnswersString
        {
            get
            {
                string AnswerString = _CswNbtMetaDataNodeTypeProp.ListOptions;
                if( string.IsNullOrEmpty( AnswerString ) )
                    AnswerString = "Yes,No,N/A";
                return AnswerString;
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            XmlNode AnswerNode = CswXmlDocument.AppendXmlNode( ParentNode, _AnswerSubField.ToXmlNodeName(), Answer.ToString() );
            XmlNode AllowedAnswersNode = CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.AllowedAnswers.ToString().ToLower(), AllowedAnswersString );
            XmlNode CompliantAnswersNode = CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.CompliantAnswers.ToString().ToLower(), CompliantAnswersString );
            XmlNode CommentsNode = CswXmlDocument.AppendXmlNode( ParentNode, _CommentsSubField.ToXmlNodeName(), Comments.ToString() );
            XmlNode CorrectiveActionNode = CswXmlDocument.AppendXmlNode( ParentNode, _CorrectiveActionSubField.ToXmlNodeName(), CorrectiveAction.ToString() );
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Answer = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _AnswerSubField.ToXmlNodeName() );
            Comments = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _CommentsSubField.ToXmlNodeName() );
            CorrectiveAction = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _CorrectiveActionSubField.ToXmlNodeName() );
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Answer = CswTools.XmlRealAttributeName( PropRow[_AnswerSubField.ToXmlNodeName()].ToString() );
            Comments = CswTools.XmlRealAttributeName( PropRow[_CommentsSubField.ToXmlNodeName()].ToString() );
            CorrectiveAction = CswTools.XmlRealAttributeName( PropRow[_CorrectiveActionSubField.ToXmlNodeName()].ToString() );
        }

    }//CswNbtNodePropQuestion

}//namespace 
