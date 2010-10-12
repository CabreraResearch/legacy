using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Xml;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleQuestion : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleQuestion( CswNbtFieldResources CswNbtFieldResources, ICswNbtMetaDataProp MetaDataProp )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources, MetaDataProp );

            //List
            AnswerSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Value );
            AnswerSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                         CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( AnswerSubField, true );

            //List
            CorrectiveActionSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field2, CswNbtSubField.SubFieldName.Value );
            CorrectiveActionSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                         CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( CorrectiveActionSubField );

            //Logical
            IsCompliantSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field3, CswNbtSubField.SubFieldName.Checked );
            IsCompliantSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                         CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                         CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( IsCompliantSubField );

            //Memo
            CommentsSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.ClobData, CswNbtSubField.SubFieldName.Value );
            CommentsSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                       CswNbtPropFilterSql.PropertyFilterMode.Begins |
                                       CswNbtPropFilterSql.PropertyFilterMode.Ends |
                                       CswNbtPropFilterSql.PropertyFilterMode.Contains |
                                       CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                       CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( CommentsSubField );

            //Date
            DateAnsweredSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field1_Date, CswNbtSubField.SubFieldName.Value );
            DateAnsweredSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                            CswNbtPropFilterSql.PropertyFilterMode.Null;
            SubFields.add( DateAnsweredSubField );
            
            //Date
            DateCorrectedSubField = new CswNbtSubField( _CswNbtFieldResources, MetaDataProp, CswNbtSubField.PropColumn.Field2_Date, CswNbtSubField.SubFieldName.Value );
            DateCorrectedSubField.FilterModes = CswNbtPropFilterSql.PropertyFilterMode.Equals |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.GreaterThanOrEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThan |
                                            CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotEquals |
                                            CswNbtPropFilterSql.PropertyFilterMode.NotNull |
                                            CswNbtPropFilterSql.PropertyFilterMode.Null;
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

            if ( !CswNbtSubField.SupportedFilterModes.Contains( CswNbtViewPropertyFilterIn.FilterMode ) )
                throw ( new CswDniException( "Filter mode " + CswNbtViewPropertyFilterIn.FilterMode.ToString() + " is not supported for sub field: " + CswNbtSubField.Name + "; view name is: " + CswNbtViewPropertyFilterIn.View.ViewName ) );

            // Are we using a Date filter?
            if ( CswNbtSubField.Name == DateAnsweredSubField.Name || CswNbtSubField.Name == DateCorrectedSubField.Name )
            {
                ReturnVal = CswNbtFieldTypeRuleDateImpl.renderViewPropFilter( RunAsUser, _CswNbtFieldResources, CswNbtViewPropertyFilterIn, CswNbtSubField.Column );
            }
            else if ( CswNbtSubField.Name == IsCompliantSubField.Name )
            {
                string ValueColumn = "jnp." + CswNbtSubField.Column.ToString();
                if ( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.Equals )
                {
                    if ( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                    {
                        ReturnVal = ValueColumn + " = '1' ";
                    }
                    else if ( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
                    {
                        ReturnVal = ValueColumn + " = '0' ";
                    }
                    else
                    {
                        ReturnVal = ValueColumn + " is null";
                    }
                }
                else if ( CswNbtViewPropertyFilterIn.FilterMode == CswNbtPropFilterSql.PropertyFilterMode.NotEquals )
                {
                    if ( CswNbtViewPropertyFilterIn.Value == "1" || CswNbtViewPropertyFilterIn.Value.ToLower() == "true" )
                    {
                        ReturnVal = "(" + ValueColumn + " = '0' or " + ValueColumn + " is null) ";
                    }
                    else if ( CswNbtViewPropertyFilterIn.Value == "0" || CswNbtViewPropertyFilterIn.Value.ToLower() == "false" )
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
                    throw new CswDniException( "Invalid filter", "An invalid FilterMode was encountered in CswNbtFieldTypeRuleQuestion.renderViewPropFilter()" );
                }
            }
            else
            {
                ReturnVal = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn );
            }

            return ( ReturnVal );

        }//renderViewPropFilter()


        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            string ReturnVal = string.Empty;

            // Are we using a Date filter?
            if ( SubField.Name == DateAnsweredSubField.Name || SubField.Name == DateCorrectedSubField.Name )
            {
                ReturnVal = CswNbtFieldTypeRuleDateImpl.FilterModeToString( FilterMode );
            }
            else
            {
                ReturnVal = _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
            }

            return ( ReturnVal );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropData PropertyValueToCheck )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck );
        }

        public void afterCreateNodeTypeProp(  CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            //_CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
            NodeTypeProp.ListOptions = "Yes,No,N/A";
        }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
