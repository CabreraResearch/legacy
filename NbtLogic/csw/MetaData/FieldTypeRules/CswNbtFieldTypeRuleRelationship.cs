using System;
using System.Collections.ObjectModel;
using ChemSW.Exceptions;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleRelationship : ICswNbtFieldTypeRule
    {

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleRelationship( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            NameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, CswEnumNbtSubFieldName.Name );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( NameSubField, true );

            NodeIDSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, CswEnumNbtSubFieldName.NodeID, true );
            NodeIDSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            NodeIDSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            NodeIDSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            NodeIDSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            NodeIDSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.In );
            SubFields.add( NodeIDSubField );
        }//ctor

        public CswNbtSubField NodeIDSubField;
        public CswNbtSubField NameSubField;

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }//get
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn )
        {
            CswEnumNbtSubFieldName OldSubfieldName = CswNbtViewPropertyFilterIn.SubfieldName;
            CswEnumNbtFilterMode OldFilterMode = CswNbtViewPropertyFilterIn.FilterMode;
            string OldValue = CswNbtViewPropertyFilterIn.Value;

            // BZ 8558
            if( OldSubfieldName == NameSubField.Name && OldValue.ToLower() == "me" )
            {
                CswNbtViewProperty Prop = (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent;
                ICswNbtMetaDataProp MetaDataProp = null;
                if( Prop.Type == CswEnumNbtViewPropType.NodeTypePropId )
                    MetaDataProp = Prop.NodeTypeProp;
                else if( Prop.Type == CswEnumNbtViewPropType.ObjectClassPropId )
                    MetaDataProp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( Prop.ObjectClassPropId );

                if( MetaDataProp != null && MetaDataProp.IsUserRelationship() )
                {
                    if( CswNbtViewPropertyFilterIn.Value.ToLower() == "me" && false == ( RunAsUser is CswNbtSystemUser ) )
                    {
                        CswNbtViewPropertyFilterIn.SubfieldName = NodeIDSubField.Name;
                        CswNbtViewPropertyFilterIn.FilterMode = CswEnumNbtFilterMode.Equals;
                        CswNbtViewPropertyFilterIn.Value = RunAsUser.UserId.PrimaryKey.ToString();
                    }
                }
            }
            string ret = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, SubFields, CswNbtViewPropertyFilterIn, false );

            CswNbtViewPropertyFilterIn.SubfieldName = OldSubfieldName;
            CswNbtViewPropertyFilterIn.FilterMode = OldFilterMode;
            CswNbtViewPropertyFilterIn.Value = OldValue;

            return ret;
        }

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries, NodeIDSubField );
        }

        private CswNbtView _setDefaultView( CswNbtMetaDataNodeTypeProp MetaDataProp, CswEnumNbtViewRelatedIdType RelatedIdType, Int32 inFKValue, bool OnlyCreateIfNull )
        {
            //CswNbtMetaDataNodeTypeProp ThisNtProp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( MetaDataProp.PropId );
            CswNbtView RetView = _CswNbtFieldResources.CswNbtResources.ViewSelect.restoreView( MetaDataProp.ViewId );
            if( RelatedIdType != CswEnumNbtViewRelatedIdType.Unknown &&
                ( null == RetView ||
                  RetView.Root.ChildRelationships.Count == 0 ||
                  false == OnlyCreateIfNull ) )
            {

                if( null != RetView )
                {
                    RetView.Root.ChildRelationships.Clear();
                }

                //if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
                //{
                //    CswNbtMetaDataObjectClass TargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( inFKValue );
                //    if( null == TargetOc )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                //    }
                //    RetView = TargetOc.CreateDefaultView();
                //}
                //else if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
                //{
                //    CswNbtMetaDataNodeType TargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( inFKValue );
                //    if( null == TargetNt )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                //    }
                //    RetView = TargetNt.CreateDefaultView();
                //}
                //else if( RelatedIdType == NbtViewRelatedIdType.PropertySetId )
                //{
                //    CswNbtMetaDataPropertySet TargetPs = _CswNbtFieldResources.CswNbtResources.MetaData.getPropertySet( inFKValue );
                //    if( null == TargetPs )
                //    {
                //        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to propertysetid: " + inFKValue + ", but the target is null." );
                //    }
                //    RetView = TargetPs.CreateDefaultView();
                //}
                //else
                //{
                //    throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Invalid RelatedIdType: " + RelatedIdType + "." );
                //}

                ICswNbtMetaDataDefinitionObject targetObj = _CswNbtFieldResources.CswNbtResources.MetaData.getDefinitionObject( RelatedIdType, inFKValue );
                if( null != targetObj )
                {
                    RetView = targetObj.CreateDefaultView();
                }
                else
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Cannot create a relationship without a valid target.", "_setDefaultView() got an invalid RelatedIdType: " + RelatedIdType + " or value: " + inFKValue );
                }

                RetView.ViewId = MetaDataProp.ViewId;
                RetView.Visibility = CswEnumNbtViewVisibility.Property;
                RetView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                RetView.ViewName = MetaDataProp.PropName;
                RetView.save();
            }
            return RetView;
        }

        public void setFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtMetaDataNodeTypeProp.doSetFk doSetFk, string inFKType, Int32 inFKValue, string inValuePropType = "", Int32 inValuePropId = Int32.MinValue )
        {
            string OutFkType = inFKType;
            Int32 OutFkValue = inFKValue;
            string OutValuePropType = inValuePropType;
            Int32 OutValuePropId = inValuePropId;

            //New PropIdTypes
            CswEnumNbtViewRelatedIdType NewFkPropIdType = inFKType;

            //Current PropIdTypes
            CswEnumNbtViewRelatedIdType CurrentFkPropIdType = MetaDataProp.FKType;

            //We have valid values that are different that what is currently set
            if( ( false == string.IsNullOrEmpty( inFKType ) &&
                  Int32.MinValue != inFKValue &&
                  NewFkPropIdType != CswEnumNbtViewRelatedIdType.Unknown
                ) &&
                (
                  NewFkPropIdType != CurrentFkPropIdType ||
                  inFKValue != MetaDataProp.FKValue
                ) //something has changed 
              )
            {
                _setDefaultView( MetaDataProp, NewFkPropIdType, inFKValue, false );
                OutFkType = NewFkPropIdType.ToString();
                OutFkValue = inFKValue;
                OutValuePropType = string.Empty;
                OutValuePropId = Int32.MinValue;
                doSetFk( OutFkType, OutFkValue, OutValuePropType, OutValuePropId );
            }
            else
            {
                //Make sure a default view is set
                _setDefaultView( MetaDataProp, CurrentFkPropIdType, MetaDataProp.FKValue, true );
            }
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = new Collection<CswNbtFieldTypeAttribute>();
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = CswEnumNbtPropertyAttributeName.IsFK,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Isfk
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = CswEnumNbtPropertyAttributeName.Target,
                    AttributeFieldType = CswEnumNbtFieldType.MetaDataList,
                    SubFieldName = CswEnumNbtSubFieldName.Type,
                    Column = CswEnumNbtPropertyAttributeColumn.Fktype
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = CswEnumNbtPropertyAttributeName.Target,
                    AttributeFieldType = CswEnumNbtFieldType.MetaDataList,
                    SubFieldName = CswEnumNbtSubFieldName.Id,
                    Column = CswEnumNbtPropertyAttributeColumn.Fkvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = CswEnumNbtPropertyAttributeName.View,
                    AttributeFieldType = CswEnumNbtFieldType.ViewReference,
                    Column = CswEnumNbtPropertyAttributeColumn.Nodeviewid,
                    SubFieldName = CswEnumNbtSubFieldName.ViewID
                } );
            ret.Add( new CswNbtFieldTypeAttribute()
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = CswEnumNbtPropertyAttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            //Schema Updater will trigger afterCreateNodeTypeProp(), but it won't call setFk
            if( null != NodeTypeProp )
            {
                string FkType = NodeTypeProp.FKType;
                Int32 FkValue = NodeTypeProp.FKValue;

                if( false == string.IsNullOrEmpty( FkType ) &&
                    Int32.MinValue != FkValue )
                {
                    NodeTypeProp.SetFK( FkType, FkValue );
                }

                _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
            }
        }

    }//CswNbtFieldTypeRuleRelationship

}//namespace ChemSW.Nbt.MetaData
