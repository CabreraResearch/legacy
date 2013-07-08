using System;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleQuestion : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleQuestion( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            //List
            AnswerSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Answer, true );
            AnswerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            AnswerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            AnswerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            AnswerSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( AnswerSubField, true );

            //List
            CorrectiveActionSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2, CswEnumNbtSubFieldName.CorrectiveAction, true );
            CorrectiveActionSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CorrectiveActionSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            CorrectiveActionSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            CorrectiveActionSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( CorrectiveActionSubField );

            //Logical
            IsCompliantSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3, CswEnumNbtSubFieldName.IsCompliant, true );
            IsCompliantSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            IsCompliantSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            IsCompliantSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            IsCompliantSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( IsCompliantSubField );

            //Memo
            CommentsSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.ClobData, CswEnumNbtSubFieldName.Comments, true );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            CommentsSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( CommentsSubField );

            //Date
            DateAnsweredSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Date, CswEnumNbtSubFieldName.DateAnswered, true );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            DateAnsweredSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( DateAnsweredSubField );

            //Date
            DateCorrectedSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Date, CswEnumNbtSubFieldName.DateCorrected, true );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            DateCorrectedSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( DateCorrectedSubField );
        }//ctor

        /// <summary>
        /// Answer: List, field1, value
        /// </summary>
        public CswNbtSubField AnswerSubField;

        /// <summary>
        /// Corrective Action: List, field2, value
        /// </summary>
        public CswNbtSubField CorrectiveActionSubField;

        /// <summary>
        /// Is Compliant: Logical, field3, checked
        /// </summary>
        public CswNbtSubField IsCompliantSubField;

        /// <summary>
        /// Comments: Memo, clobdata, value
        /// </summary>
        public CswNbtSubField CommentsSubField;

        /// <summary>
        /// Date Answered: Date, field1_date, value
        /// </summary>
        public CswNbtSubField DateAnsweredSubField;

        /// <summary>
        /// Date Corrected: Date, field2_date, value
        /// </summary>
        public CswNbtSubField DateCorrectedSubField;

        /// <summary>
        /// CswNbtFieldTypeRuleQuestion Subfields
        /// </summary>
        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }
        }

        /// <summary>
        /// Is Search Allowed? True == yes.
        /// </summary>
        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            string ReturnVal = string.Empty;

            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];

            if( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            // Are we using a Date filter?
            if( CswNbtSubField.Name == DateAnsweredSubField.Name || CswNbtSubField.Name == DateCorrectedSubField.Name )
            {
                ReturnVal = CswNbtFieldTypeRuleDateImpl.renderViewPropFilter( RunAsUser, _CswNbtFieldResources, CswNbtViewPropertyFilterIn, CswNbtSubField.Column );
            }
            else if( CswNbtSubField.Name == IsCompliantSubField.Name )
            {
                string ValueColumn = "jnp." + CswNbtSubField.Column.ToString();
                if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.Equals )
                {
                    if( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                    {
                        ReturnVal = ValueColumn + " = '1' ";
                    }
                    else if( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                    {
                        ReturnVal = ValueColumn + " = '0' ";
                    }
                    else
                    {
                        ReturnVal = ValueColumn + " is null";
                    }
                }
                else if( CswNbtViewPropertyFilterIn.FilterMode == CswEnumNbtFilterMode.NotEquals )
                {
                    if( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                    {
                        ReturnVal = "(" + ValueColumn + " = '0' or " + ValueColumn + " is null) ";
                    }
                    else if( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                    {
                        ReturnVal = "(" + ValueColumn + " = '1' or " + ValueColumn + " is null) ";
                    }
                    else
                    {
                        ReturnVal = "(" + ValueColumn + " = '1' or " + ValueColumn + " = '0') ";
                    }
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Invalid filter", "An invalid FilterMode was encountered in CswNbtFieldTypeRuleQuestion.renderViewPropFilter()" );
                }
            }
            else
            {
                ReturnVal = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn );
            }

            return ( ReturnVal );

        }//renderViewPropFilter()


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            string ReturnVal = string.Empty;

            // Are we using a Date filter?
            if( SubField.Name == DateAnsweredSubField.Name || SubField.Name == DateCorrectedSubField.Name )
            {
                ReturnVal = CswNbtFieldTypeRuleDateImpl.FilterModeToString( FilterMode );
            }
            else
            {
                ReturnVal = _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
            }

            return ( ReturnVal );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            _CswNbtFieldTypeRuleDefault.setFk( MetaDataProp, doSetFk, inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string PossibleAnswers = CswEnumNbtPropertyAttributeName.PossibleAnswers;
            public const string CompliantAnswers = CswEnumNbtPropertyAttributeName.CompliantAnswers;
            public const string PreferredAnswer = CswEnumNbtPropertyAttributeName.PreferredAnswer;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Question );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Question,
                    Name = AttributeName.PossibleAnswers,
                    AttributeFieldType = CswEnumNbtFieldType.Text,
                    Column = CswEnumNbtPropertyAttributeColumn.Listoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Question,
                    Name = AttributeName.CompliantAnswers,
                    AttributeFieldType = CswEnumNbtFieldType.MultiList,
                    Column = CswEnumNbtPropertyAttributeColumn.Valueoptions
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Question,
                    Name = AttributeName.PreferredAnswer,
                    AttributeFieldType = CswEnumNbtFieldType.List,
                    Column = CswEnumNbtPropertyAttributeColumn.Extended
                } );
            //ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            //{
            //    OwnerFieldType = CswEnumNbtFieldType.Question,
            //    Name = CswEnumNbtPropertyAttributeName.DefaultValue,
            //    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
            //    AttributeFieldType = CswEnumNbtFieldType.Question
            //} );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            NodeTypeProp.ListOptions = "Yes,No,N/A";
            NodeTypeProp.ValueOptions = "Yes";        // case 20297

            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
