using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using Newtonsoft.Json.Linq;

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
            if( _CswNbtMetaDataNodeTypeProp.FieldType.FieldType != CswNbtMetaDataFieldType.NbtFieldType.Question )
            {
                throw ( new CswDniException( ErrorType.Error, "A data consistency problem occurred",
                                            "CswNbtNodePropQuestion() was created on a property with fieldtype: " + _CswNbtMetaDataNodeTypeProp.FieldType.FieldType ) );
            }

            CswNbtFieldTypeRuleQuestion FieldTypeRuleQuestion = (CswNbtFieldTypeRuleQuestion) _CswNbtMetaDataNodeTypeProp.FieldTypeRule;

            _AnswerSubField = FieldTypeRuleQuestion.AnswerSubField;
            _CommentsSubField = FieldTypeRuleQuestion.CommentsSubField;
            _CorrectiveActionSubField = FieldTypeRuleQuestion.CorrectiveActionSubField;
            _DateAnsweredSubField = FieldTypeRuleQuestion.DateAnsweredSubField;
            _DateCorrectedSubField = FieldTypeRuleQuestion.DateCorrectedSubField;
            _IsCompliantSubField = FieldTypeRuleQuestion.IsCompliantSubField;

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
                if( value != _CswNbtNodePropData.GetPropRowValue( _AnswerSubField.Column ) )
                {
                    string AnswerVal = value;

                    DateTime UpdateDateAnswered = DateAnswered;
                    if( !string.IsNullOrEmpty( AnswerVal ) )
                    {
                        if( UpdateDateAnswered == DateTime.MinValue )  // case 21056
                        {
                            UpdateDateAnswered = DateTime.Today;
                        }
                    }
                    else
                    {
                        UpdateDateAnswered = DateTime.MinValue;
                    }
                    DateAnswered = UpdateDateAnswered;
                    //IsCompliant = _IsCompliant;
                    _CswNbtNodePropData.SetPropRowValue( _AnswerSubField.Column, AnswerVal );

                    _synchGestalt( AnswerVal );
                }
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
                if( value != _CswNbtNodePropData.GetPropRowValue( _CorrectiveActionSubField.Column ) )
                {
                    string val = value;
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

        private void _synchGestalt( String GestaltValue )
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
            CswXmlDocument.AppendXmlNode( ParentNode, _AnswerSubField.ToXmlNodeName(), Answer );
            CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.AllowedAnswers.ToString().ToLower(), AllowedAnswersString );
            CswXmlDocument.AppendXmlNode( ParentNode, CswNbtSubField.SubFieldName.CompliantAnswers.ToString().ToLower(), CompliantAnswersString );
            CswXmlDocument.AppendXmlNode( ParentNode, _CommentsSubField.ToXmlNodeName(), Comments );
            CswXmlDocument.AppendXmlNode( ParentNode, _CorrectiveActionSubField.ToXmlNodeName(), CorrectiveAction );
            CswXmlDocument.AppendXmlNode( ParentNode, _IsCompliantSubField.ToXmlNodeName(), IsCompliant.ToString() );

            CswXmlDocument.AppendXmlNode( ParentNode, _DateAnsweredSubField.ToXmlNodeName(), ( DateAnswered != DateTime.MinValue ) ?
                DateAnswered.ToShortDateString() : string.Empty );

            CswXmlDocument.AppendXmlNode( ParentNode, _DateCorrectedSubField.ToXmlNodeName(), ( DateCorrected != DateTime.MinValue ) ?
                DateCorrected.ToShortDateString() : string.Empty );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _AnswerSubField.ToXmlNodeName( true ), Answer ),
                            new XElement( CswNbtSubField.SubFieldName.AllowedAnswers.ToString().ToLower(), AllowedAnswersString ),
                            new XElement( CswNbtSubField.SubFieldName.CompliantAnswers.ToString().ToLower(), CompliantAnswersString ),
                            new XElement( _CommentsSubField.ToXmlNodeName( true ), Comments ),
                            new XElement( _CorrectiveActionSubField.ToXmlNodeName( true ), CorrectiveAction ),
                            new XElement( _IsCompliantSubField.ToXmlNodeName( true ), IsCompliant.ToString() ),
                            new XElement( _DateAnsweredSubField.ToXmlNodeName( true ), ( DateAnswered != DateTime.MinValue ) ?
                                    DateAnswered.ToShortDateString() : string.Empty ),
                            new XElement( _DateCorrectedSubField.ToXmlNodeName( true ), ( DateCorrected != DateTime.MinValue ) ?
                                    DateCorrected.ToShortDateString() : string.Empty ) );
        }

		public override void ToJSON( JObject ParentObject )
		{
			ParentObject[_AnswerSubField.ToXmlNodeName( true )] = Answer;
			ParentObject[CswNbtSubField.SubFieldName.AllowedAnswers.ToString().ToLower()] = AllowedAnswersString;
			ParentObject[CswNbtSubField.SubFieldName.CompliantAnswers.ToString().ToLower()] = CompliantAnswersString;
			ParentObject[_CommentsSubField.ToXmlNodeName( true )] = Comments;
			ParentObject[_CorrectiveActionSubField.ToXmlNodeName( true )] = CorrectiveAction;
			ParentObject[_IsCompliantSubField.ToXmlNodeName( true )] = IsCompliant.ToString();
			//ParentObject[_DateAnsweredSubField.ToXmlNodeName( true )] = ( DateAnswered != DateTime.MinValue ) ?
			//        DateAnswered.ToShortDateString() : string.Empty;
			//ParentObject[_DateCorrectedSubField.ToXmlNodeName( true )] = ( DateCorrected != DateTime.MinValue ) ?
			//        DateCorrected.ToShortDateString() : string.Empty;

			CswDateTime CswDateAnswered = new CswDateTime( _CswNbtResources, DateAnswered );
			ParentObject.Add( new JProperty( _DateAnsweredSubField.ToXmlNodeName( true ),
											 CswDateAnswered.ToClientAsJObject() ) );
			CswDateTime CswDateCorrected = new CswDateTime( _CswNbtResources, DateCorrected );
			ParentObject.Add( new JProperty( _DateCorrectedSubField.ToXmlNodeName( true ),
											 CswDateCorrected.ToClientAsJObject() ) );
		}

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            // case 21010 - set dates from data first, and only if they are currently blank
            if( DateAnswered == DateTime.MinValue )
                DateAnswered = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _DateAnsweredSubField.ToXmlNodeName() );
            if( DateCorrected == DateTime.MinValue )
                DateCorrected = CswXmlDocument.ChildXmlNodeValueAsDate( XmlNode, _DateCorrectedSubField.ToXmlNodeName() );

            Answer = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _AnswerSubField.ToXmlNodeName() );
            Comments = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _CommentsSubField.ToXmlNodeName() );
            CorrectiveAction = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _CorrectiveActionSubField.ToXmlNodeName() );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( DateAnswered == DateTime.MinValue )
            {
                if( null != XmlNode.Element( _DateAnsweredSubField.ToXmlNodeName( true ) ) )
                {
                    DateAnswered = CswConvert.ToDateTime( XmlNode.Element( _DateAnsweredSubField.ToXmlNodeName( true ) ).Value );
                }
            }
            if( DateCorrected == DateTime.MinValue )
            {
                if( null != XmlNode.Element( _DateCorrectedSubField.ToXmlNodeName( true ) ) )
                {
                    DateCorrected = CswConvert.ToDateTime( XmlNode.Element( _DateCorrectedSubField.ToXmlNodeName( true ) ).Value );
                }
            }
            if( null != XmlNode.Element( _AnswerSubField.ToXmlNodeName( true ) ) )
            {
                Answer = XmlNode.Element( _AnswerSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _CommentsSubField.ToXmlNodeName( true ) ) )
            {
                Comments = XmlNode.Element( _CommentsSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( _CorrectiveActionSubField.ToXmlNodeName( true ) ) )
            {
                CorrectiveAction = XmlNode.Element( _CorrectiveActionSubField.ToXmlNodeName( true ) ).Value;
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            Answer = CswTools.XmlRealAttributeName( PropRow[_AnswerSubField.ToXmlNodeName()].ToString() );
            Comments = CswTools.XmlRealAttributeName( PropRow[_CommentsSubField.ToXmlNodeName()].ToString() );
            CorrectiveAction = CswTools.XmlRealAttributeName( PropRow[_CorrectiveActionSubField.ToXmlNodeName()].ToString() );
            String DateAnsweredString = CswTools.XmlRealAttributeName( PropRow[_DateAnsweredSubField.ToXmlNodeName()].ToString() );
            if( !String.IsNullOrEmpty( DateAnsweredString ) )
                DateAnswered = Convert.ToDateTime( DateAnsweredString );
            String DateCorrectedString = CswTools.XmlRealAttributeName( PropRow[_DateCorrectedSubField.ToXmlNodeName()].ToString() );
            if( !String.IsNullOrEmpty( DateCorrectedString ) )
                DateCorrected = Convert.ToDateTime( DateCorrectedString );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
			if( DateAnswered == DateTime.MinValue )
			{
				//    if( null != JObject.Property( _DateAnsweredSubField.ToXmlNodeName( true ) ) )
				//    {
				//        DateAnswered = CswConvert.ToDateTime( JObject.Property( _DateAnsweredSubField.ToXmlNodeName( true ) ).Value );
				//    }
				CswDateTime CswDateAnswered = new CswDateTime( _CswNbtResources );
				CswDateAnswered.FromClientJObject( (JObject) JObject[_DateAnsweredSubField.ToXmlNodeName( true )] );
				DateAnswered = CswDateAnswered.ToDateTime();
			}
			if( DateCorrected == DateTime.MinValue )
			{
				//    if( null != JObject.Property( _DateCorrectedSubField.ToXmlNodeName( true ) ) )
				//    {
				//        DateCorrected = CswConvert.ToDateTime( JObject.Property( _DateCorrectedSubField.ToXmlNodeName( true ) ).Value );
				//    }
				CswDateTime CswDateCorrected = new CswDateTime( _CswNbtResources );
				CswDateCorrected.FromClientJObject( (JObject) JObject[_DateCorrectedSubField.ToXmlNodeName( true )] );
				DateCorrected = CswDateCorrected.ToDateTime();
			}

			if( null != JObject.Property( _AnswerSubField.ToXmlNodeName( true ) ) )
            {
                Answer = (string) JObject.Property( _AnswerSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != JObject.Property( _CommentsSubField.ToXmlNodeName( true ) ) )
            {
                Comments = (string) JObject.Property( _CommentsSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != JObject.Property( _CorrectiveActionSubField.ToXmlNodeName( true ) ) )
            {
                CorrectiveAction = (string) JObject.Property( _CorrectiveActionSubField.ToXmlNodeName( true ) ).Value;
            }
        }
    }//CswNbtNodePropQuestion

}//namespace 
