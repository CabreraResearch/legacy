using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleDefaultImpl
    {
        private CswNbtFieldResources _CswNbtFieldResources = null;
        private CswNbtSubFieldColl _SubFields;

        public CswNbtFieldTypeRuleDefaultImpl( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _SubFields = new CswNbtSubFieldColl();      // this should be filled in by the parent class
        }//ctor

        public CswNbtSubFieldColl SubFields { get { return ( _SubFields ); } }
        public bool SearchAllowed { get { return ( true ); } }

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            //doSetFk( inFKType, inFKValue, inValuePropType, inValuePropId );
        }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber, bool UseNumericHack = false )
        {

            CswNbtSubField CswNbtSubField = null;
            CswNbtSubField = SubFields[CswNbtViewPropertyFilterIn.SubfieldName];
            if( CswNbtSubField == null )
                throw new CswDniException( CswEnumErrorType.Error, "Misconfigured View", "CswNbtFieldTypeRuleDefaultImpl.renderViewPropFilter() could not find SubField '" + CswNbtViewPropertyFilterIn.SubfieldName + "' in field type '" + ( (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent ).FieldType.ToString() + "' for view '" + CswNbtViewPropertyFilterIn.View.ViewName + "'" );

            return ( _CswNbtFieldResources.CswNbtPropFilterSql.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, CswNbtSubField, ParameterCollection, FilterNumber, UseNumericHack ) );

        }//makeWhereClause()


        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            // Default implementation
            return FilterMode.ToString();
        }

        // This is used by CswNbtNodeProp for unique value enforcement
        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false, CswNbtSubField SubField = null )
        {
            if( SubField == null )
            {
                SubField = SubFields.Default;
            }

            string StringValueToCheck = PropertyValueToCheck.GetSubFieldValue( SubField );
            // case 31292 - Kludge fix for NodeID filters
            if( SubField.Name == CswEnumNbtSubFieldName.NodeID && false == string.IsNullOrEmpty( StringValueToCheck ) )
            {
                CswPrimaryKey pkValue = new CswPrimaryKey();
                pkValue.FromString( StringValueToCheck );
                StringValueToCheck = pkValue.PrimaryKey.ToString();
            }
            CswEnumNbtFilterMode FilterMode;
            //case 27670 - in order to reserve the right for compound unique props to be empty, it has to be explicitly stated when creating the ForCompundUnique view
            if( EnforceNullEntries && String.IsNullOrEmpty( StringValueToCheck ) )
            {
                FilterMode = CswEnumNbtFilterMode.Null;
            }
            else
            {
                FilterMode = CswEnumNbtFilterMode.Equals;
            }

            View.AddViewPropertyFilter( UniqueValueViewProperty, SubField.Name, FilterMode, StringValueToCheck.Trim(), false );
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
        }

        /// <summary>
        /// All field types get these attributes
        /// </summary>
        public Collection<CswNbtFieldTypeAttribute> getAttributes( CswEnumNbtFieldType OwnerFieldType )
        {
            Collection<CswNbtFieldTypeAttribute> ret = new Collection<CswNbtFieldTypeAttribute>();
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.AuditLevel,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Auditlevel
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.CompoundUnique,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Iscompoundunique
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.DisplayConditionFilterMode,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Filtermode
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.DisplayConditionProperty,
                AttributeFieldType = CswEnumNbtFieldType.Relationship,
                Column = CswEnumNbtPropertyAttributeColumn.Filterpropid,
                SubFieldName = CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.DisplayConditionSubfield,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Filtersubfield
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.DisplayConditionValue,
                AttributeFieldType = CswEnumNbtFieldType.Text,
                Column = CswEnumNbtPropertyAttributeColumn.Filtervalue
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.FieldType,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Fieldtypeid
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.HelpText,
                AttributeFieldType = CswEnumNbtFieldType.Memo,
                Column = CswEnumNbtPropertyAttributeColumn.Helptext
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.NodeTypeValue,
                AttributeFieldType = CswEnumNbtFieldType.Relationship,
                Column = CswEnumNbtPropertyAttributeColumn.Nodetypeid,
                SubFieldName = CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.ObjectClassPropName,
                AttributeFieldType = CswEnumNbtFieldType.List,
                Column = CswEnumNbtPropertyAttributeColumn.Objectclasspropid
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.PropName,
                AttributeFieldType = CswEnumNbtFieldType.Text,
                Column = CswEnumNbtPropertyAttributeColumn.Propname
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.QuestionNo,
                AttributeFieldType = CswEnumNbtFieldType.Number,
                Column = CswEnumNbtPropertyAttributeColumn.Questionno
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.ReadOnly,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Readonly
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.Required,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Isrequired
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.ServerManaged,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Servermanaged
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.SubQuestionNo,
                AttributeFieldType = CswEnumNbtFieldType.Number,
                Column = CswEnumNbtPropertyAttributeColumn.Subquestionno
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.Unique,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Isunique
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = OwnerFieldType,
                Name = CswEnumNbtPropertyAttributeName.UseNumbering,
                AttributeFieldType = CswEnumNbtFieldType.Logical,
                Column = CswEnumNbtPropertyAttributeColumn.Usenumbering
            } );

            return ret;
        } // getAttributes()

        public static CswNbtView setDefaultView( CswNbtMetaData MetaData, CswNbtObjClassDesignNodeTypeProp DesignNTPNode, CswNbtView View, CswEnumNbtViewRelatedIdType RelatedIdType, Int32 inFKValue, bool OnlyCreateIfNull )
        {
            if( RelatedIdType != CswEnumNbtViewRelatedIdType.Unknown &&
                ( null == View ||
                  View.Root.ChildRelationships.Count == 0 ||
                  false == OnlyCreateIfNull ) )
            {

                if( null != View )
                {
                    View.Root.ChildRelationships.Clear();
                }

                ICswNbtMetaDataDefinitionObject targetObj = MetaData.getDefinitionObject( RelatedIdType, inFKValue );
                if( null != targetObj )
                {
                    CswNbtViewId OldViewId = View.ViewId;
                    View = targetObj.CreateDefaultView();
                    View.ViewId = OldViewId;
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Cannot create a relationship without a valid target.", "setDefaultView() got an invalid RelatedIdType: " + RelatedIdType + " or value: " + inFKValue );
                }

                View.Visibility = CswEnumNbtViewVisibility.Property;
                View.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                View.ViewName = DesignNTPNode.PropName.Text;
                View.save();
            }
            return View;
        }

    }//CswNbtFieldTypeRuleDefaultImpl

}//namespace ChemSW.Nbt.MetaData
