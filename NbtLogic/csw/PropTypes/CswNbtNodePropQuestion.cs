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
    public class CswNbtNodePropQuestion : CswNbtNodeProp
    {
        public static implicit operator CswNbtNodePropQuestion( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsQuestion;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropQuestion( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp, CswNbtNode Node )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp, Node )
        {
            //if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswEnumNbtFieldType.Question )
            //{
            //    throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
            //                                "CswNbtNodePropQuestion() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            //}

            CswNbtFieldTypeRuleQuestion FieldTypeRule = (CswNbtFieldTypeRuleQuestion) _FieldTypeRule;

            _AnswerSubField = FieldTypeRule.AnswerSubField;
            _CommentsSubField = FieldTypeRule.CommentsSubField;
            _CorrectiveActionSubField = FieldTypeRule.CorrectiveActionSubField;
            _DateAnsweredSubField = FieldTypeRule.DateAnsweredSubField;
            _DateCorrectedSubField = FieldTypeRule.DateCorrectedSubField;
            _IsCompliantSubField = FieldTypeRule.IsCompliantSubField;

            // Associate subfields with methods on this object, for SetSubFieldValue()
            _SubFieldMethods.Add( _AnswerSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Answer, x => Answer = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _CommentsSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => Comments, x => Comments = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _CorrectiveActionSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => CorrectiveAction, x => CorrectiveAction = CswConvert.ToString( x ) ) );
            _SubFieldMethods.Add( _DateAnsweredSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => DateAnswered, x => DateAnswered = CswConvert.ToDateTime( x ) ) );
            _SubFieldMethods.Add( _DateCorrectedSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => DateCorrected, x => DateCorrected = CswConvert.ToDateTime( x ) ) );
            _SubFieldMethods.Add( _IsCompliantSubField, new Tuple<Func<dynamic>, Action<dynamic>>( () => IsCompliant, x => IsCompliant = CswConvert.ToBoolean( x ) ) );
        }//ctor

        private CswNbtSubField _AnswerSubField;
        private CswNbtSubField _CommentsSubField;
        private CswNbtSubField _CorrectiveActionSubField;
        private CswNbtSubField _DateAnsweredSubField;
        private CswNbtSubField _DateCorrectedSubField;
        private CswNbtSubField _IsCompliantSubField;
        private bool _IsActionRequired = false;//case 25035

        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        override public bool Empty
        {
            get { return ( 0 == AllowedAnswers.Count ); }
        }

        private bool _IsValidNode { get { return ( null != NodeId && Int32.MinValue != NodeId.PrimaryKey ); } }

        /// <summary>
        /// Answer value as string
        /// </summary>
        public string Answer
        {
            get { return GetPropRowValue( _AnswerSubField ); }
            set
            {
                if( value != GetPropRowValue( _AnswerSubField ) )
                {
                    string AnswerVal = value.Trim();

                    DateTime UpdateDateAnswered = DateAnswered;
                    if( false == string.IsNullOrEmpty( AnswerVal ) )
                    {
                        if( _IsValidNode &&
                            UpdateDateAnswered == DateTime.MinValue )  // case 21056
                        {
                            UpdateDateAnswered = DateTime.Now;
                        }
                    }
                    else
                    {
                        UpdateDateAnswered = DateTime.MinValue;
                    }
                    DateAnswered = UpdateDateAnswered;
                    SetPropRowValue( _AnswerSubField, AnswerVal );
                    IsCompliant = TestIsCompliant();

                    SyncGestalt();
                }
            }
        }

        /// <summary>
        /// Corrective action taken to change corrected status, as string
        /// </summary>
        public String CorrectiveAction
        {
            get { return GetPropRowValue( _CorrectiveActionSubField ); }
            set
            {
                if( value != GetPropRowValue( _CorrectiveActionSubField ) )
                {
                    DateTime UpdateDateCorrected = DateTime.MinValue;

                    string val = value.Trim();
                    if( _IsValidNode && false == string.IsNullOrEmpty( val ) )
                    {
                        UpdateDateCorrected = DateTime.Now;
                    }

                    DateCorrected = UpdateDateCorrected;
                    SetPropRowValue( _CorrectiveActionSubField, val );
                    IsCompliant = TestIsCompliant();
                }
            }
        }

        private bool TestIsCompliant()
        {
            return ( CompliantAnswers.Contains( Answer, true ) || string.Empty != CorrectiveAction );
        }

        /// <summary>
        /// True if the answer to the question is compliant, not if the entire question is compliant
        /// </summary>
        /// <returns></returns>
        public bool IsAnswerCompliant()
        {
            return CompliantAnswers.Contains( Answer, true );
        }

        /// <summary>
        /// True if Answer is compliant or Corrective Action is not empty
        /// </summary>
        public bool IsCompliant
        {
            get
            {
                return CswConvert.ToBoolean( GetPropRowValue( _IsCompliantSubField ) );
            }
            set
            {
                SetPropRowValue( _IsCompliantSubField, value );
            }
        }

        /// <summary>
        /// True only when the parent Node has a Status of ActionRequired
        /// </summary>
        public bool IsActionRequired//case 25035
        {
            get
            {
                return _IsActionRequired;
            }
            set
            {
                _IsActionRequired = value;
            }
        }

        /// <summary>
        /// Whether the answer is editable (case 31231)
        /// </summary>
        public bool IsAnswerEditable
        {
            get
            {
                return false == _IsActionRequired ||
                       _CswNbtResources.CurrentNbtUser.IsAdministrator() ||
                       "1" != _CswNbtResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.lock_inspection_answer.ToString() );
            }
        }

        /// <summary>
        /// Date value set when question is answered
        /// </summary>
        public DateTime DateAnswered
        {
            get { return GetPropRowValueDate( _DateAnsweredSubField ); }
            set
            {
                SetPropRowValue( _DateAnsweredSubField, value );
            }
        }

        /// <summary>
        /// Date value set when question is marked corrected
        /// </summary>
        public DateTime DateCorrected
        {
            get { return GetPropRowValueDate( _DateCorrectedSubField ); }
            set
            {
                SetPropRowValue( _DateCorrectedSubField, value );
            }
        }

        /// <summary>
        /// Text (clob) value with comments on question
        /// </summary>
        public string Comments
        {
            get { return GetPropRowValue( _CommentsSubField ); }
            set { SetPropRowValue( _CommentsSubField, value ); }
        }

        /// <summary>
        /// Bool representing the state of compliance by comparing the answer against the list of property's Compliant answer attribute 
        /// </summary>
        //public bool IsCompliant
        //{
        //    get { return CswConvert.ToBoolean( GetPropRowValue( _IsCompliantSubField.Column ) ); }
        //    private set
        //    {
        //        SetPropRowValue( _IsCompliantSubField.Column, value );
        //    }
        //}

        public override void SyncGestalt()
        {
            SetPropRowValue( CswEnumNbtSubFieldName.Gestalt, CswEnumNbtPropColumn.Gestalt, Answer );
        }

        //Begin NTP attributes
        /// <summary>
        /// Property name
        /// </summary>
        public string Question
        {
            get { return _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.PropName]; }
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
                    //_CompliantAnswers.FromString( _CswNbtMetaDataNodeTypeProp.ValueOptions );
                    _CompliantAnswers.FromString( _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.CompliantAnswers] );
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
                //return _CswNbtMetaDataNodeTypeProp.ValueOptions;
                return _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.CompliantAnswers];
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
                    //_AllowedAnswers.ToString() != _CswNbtMetaDataNodeTypeProp.ListOptions ) // Case 20629
                    _AllowedAnswers.ToString() != _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.PossibleAnswers] ) // Case 20629
                {
                    _AllowedAnswers = new CswCommaDelimitedString();
                    //_AllowedAnswers.FromString( _CswNbtMetaDataNodeTypeProp.ListOptions );
                    _AllowedAnswers.FromString( _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.PossibleAnswers] );

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
                //string AnswerString = _CswNbtMetaDataNodeTypeProp.ListOptions;
                string AnswerString = _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.PossibleAnswers];
                if( string.IsNullOrEmpty( AnswerString ) )
                { AnswerString = "Yes,No,N/A"; }
                return AnswerString;
            }
        }

        public string PreferredAnswer
        {
            get
            {
                //string Ret = "";
                //if( AllowedAnswers.Contains( _CswNbtMetaDataNodeTypeProp.Extended, CaseSensitive: false ) )
                //{
                //    Ret = _CswNbtMetaDataNodeTypeProp.Extended;
                //}
                //return Ret;
                string ret = _CswNbtNodePropData[CswNbtFieldTypeRuleQuestion.AttributeName.PreferredAnswer];
                if( false == AllowedAnswers.Contains( ret, CaseSensitive: false ) )
                {
                    ret = string.Empty;
                }
                return ret;
            }
            //set
            //{
            //    if( AllowedAnswers.Contains( value, CaseSensitive: false ) )
            //    {
            //        _CswNbtMetaDataNodeTypeProp.Extended = value;
            //    }
            //}
        }

        public override string ValueForNameTemplate
        {
            get { return Gestalt; }
        }

        public override void ToJSON( JObject ParentObject )
        {
            base.ToJSON( ParentObject );  // FIRST

            ParentObject[_AnswerSubField.ToXmlNodeName( true )] = Answer;
            ParentObject["allowedanswers"] = AllowedAnswersString;
            ParentObject["compliantanswers"] = CompliantAnswersString;
            ParentObject[_CommentsSubField.ToXmlNodeName( true )] = Comments;
            ParentObject[_CorrectiveActionSubField.ToXmlNodeName( true )] = CorrectiveAction;
            ParentObject[_IsCompliantSubField.ToXmlNodeName( true )] = IsCompliant;

            ParentObject["isactionrequired"] = IsActionRequired;
            ParentObject["isanswereditable"] = IsAnswerEditable;

            CswDateTime CswDateAnswered = new CswDateTime( _CswNbtResources, DateAnswered );
            ParentObject[_DateAnsweredSubField.ToXmlNodeName( true )] = CswDateAnswered.ToClientAsDateTimeJObject();
            CswDateTime CswDateCorrected = new CswDateTime( _CswNbtResources, DateCorrected );
            ParentObject[_DateCorrectedSubField.ToXmlNodeName( true )] = CswDateCorrected.ToClientAsDateTimeJObject();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( IsAnswerEditable )
            {
                Answer = CswTools.XmlRealAttributeName( PropRow[_AnswerSubField.ToXmlNodeName()].ToString() );
            }
            Comments = CswTools.XmlRealAttributeName( PropRow[_CommentsSubField.ToXmlNodeName()].ToString() );
            CorrectiveAction = CswTools.XmlRealAttributeName( PropRow[_CorrectiveActionSubField.ToXmlNodeName()].ToString() );
            String DateAnsweredString = CswTools.XmlRealAttributeName( PropRow[_DateAnsweredSubField.ToXmlNodeName()].ToString() );
            if( !String.IsNullOrEmpty( DateAnsweredString ) )
            {
                DateAnswered = Convert.ToDateTime( DateAnsweredString );
            }
            String DateCorrectedString = CswTools.XmlRealAttributeName( PropRow[_DateCorrectedSubField.ToXmlNodeName()].ToString() );
            if( !String.IsNullOrEmpty( DateCorrectedString ) )
            {
                DateCorrected = Convert.ToDateTime( DateCorrectedString );
            }
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( DateAnswered == DateTime.MinValue )
            {
                CswDateTime CswDateAnswered = new CswDateTime( _CswNbtResources );
                CswDateAnswered.FromClientDateTimeJObject( (JObject) JObject[_DateAnsweredSubField.ToXmlNodeName( true )] );
                DateAnswered = CswDateAnswered.ToDateTime();
            }
            if( DateCorrected == DateTime.MinValue )
            {
                CswDateTime CswDateCorrected = new CswDateTime( _CswNbtResources );
                CswDateCorrected.FromClientDateTimeJObject( (JObject) JObject[_DateCorrectedSubField.ToXmlNodeName( true )] );
                DateCorrected = CswDateCorrected.ToDateTime();
            }

            if( null != JObject[_AnswerSubField.ToXmlNodeName( true )] && IsAnswerEditable )
            {
                Answer = JObject[_AnswerSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_CommentsSubField.ToXmlNodeName( true )] )
            {
                Comments = JObject[_CommentsSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject[_CorrectiveActionSubField.ToXmlNodeName( true )] )
            {
                CorrectiveAction = JObject[_CorrectiveActionSubField.ToXmlNodeName( true )].ToString();
            }
        }
    }//CswNbtNodePropQuestion

}//namespace 
