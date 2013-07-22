using System;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.MetaData.FieldTypeRules
{

    public class CswNbtFieldTypeRuleRelationship : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Name;
            public static CswEnumNbtSubFieldName NodeID = CswEnumNbtSubFieldName.NodeID;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;

        public CswNbtFieldTypeRuleRelationship( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            NameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Name );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            NameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( NameSubField, true );

            NodeIDSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, SubFieldName.NodeID, true );
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
            if( OldSubfieldName == SubFieldName.Name && OldValue.ToLower() == "me" )
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
                        CswNbtViewPropertyFilterIn.SubfieldName = SubFieldName.NodeID;
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

        public void onSetFk( CswNbtMetaDataNodeTypeProp MetaDataProp, CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            Collection<CswNbtFieldTypeAttribute> Attributes = getAttributes();
            CswNbtFieldTypeAttribute FkTypeAttr = Attributes.FirstOrDefault( a => a.Column == CswEnumNbtPropertyAttributeColumn.Fktype );
            if( DesignNTPNode.AttributeProperty.ContainsKey( FkTypeAttr.Name ) )
            {
                CswNbtNodePropWrapper FkTypeProp = DesignNTPNode.AttributeProperty[FkTypeAttr.Name];
                if( null != FkTypeProp && FkTypeProp.WasModified )
                {
                    CswNbtNodePropMetaDataList FkProp = FkTypeProp.AsMetaDataList;
                    if( CswEnumNbtViewRelatedIdType.Unknown != FkProp.Type && Int32.MinValue != FkProp.Id )
                    {
                        //We have valid values that are different that what is currently set
                        _setDefaultView( MetaDataProp, FkProp.Type, FkProp.Id, false );
                    }
                    else
                    {
                        //Make sure a default view is set
                        _setDefaultView( MetaDataProp, MetaDataProp.FKType, MetaDataProp.FKValue, true );
                    }
                }
            } // if( DesignNTPNode.AttributeProperty.ContainsKey( FkTypeAttr.Name ) )
        } // onSetFk()

        public sealed class AttributeName : ICswNbtFieldTypeRuleAttributeName
        {
            public const string IsFK = CswEnumNbtPropertyAttributeName.IsFK;
            public const string Target = CswEnumNbtPropertyAttributeName.Target;
            public const string View = CswEnumNbtPropertyAttributeName.View;
            public const string Rows = CswEnumNbtPropertyAttributeName.Rows;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
        }

        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Relationship );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = AttributeName.IsFK,
                    AttributeFieldType = CswEnumNbtFieldType.Logical,
                    Column = CswEnumNbtPropertyAttributeColumn.Isfk
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = AttributeName.Target,
                    AttributeFieldType = CswEnumNbtFieldType.MetaDataList,
                    SubFieldName = CswNbtFieldTypeRuleMetaDataList.SubFieldName.Type,
                    Column = CswEnumNbtPropertyAttributeColumn.Fktype
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = AttributeName.Target,
                    AttributeFieldType = CswEnumNbtFieldType.MetaDataList,
                    SubFieldName = CswNbtFieldTypeRuleMetaDataList.SubFieldName.Id,
                    Column = CswEnumNbtPropertyAttributeColumn.Fkvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = AttributeName.View,
                    AttributeFieldType = CswEnumNbtFieldType.ViewReference,
                    Column = CswEnumNbtPropertyAttributeColumn.Nodeviewid,
                    SubFieldName = CswNbtFieldTypeRuleViewReference.SubFieldName.ViewID
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = AttributeName.Rows,
                    Column = CswEnumNbtPropertyAttributeColumn.Textarearows,
                    AttributeFieldType = CswEnumNbtFieldType.Number
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Relationship,
                    Name = AttributeName.DefaultValue,
                    Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                    AttributeFieldType = CswEnumNbtFieldType.Relationship
                } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            //Schema Updater will trigger afterCreateNodeTypeProp(), but it won't call setFk
            if( null != NodeTypeProp && null != NodeTypeProp.DesignNode )
            {
                string FkType = NodeTypeProp.FKType;
                Int32 FkValue = NodeTypeProp.FKValue;

                if( false == string.IsNullOrEmpty( FkType ) &&
                    Int32.MinValue != FkValue )
                {
                    //NodeTypeProp.SetFK( FkType, FkValue );
                    NodeTypeProp.DesignNode.AttributeProperty[AttributeName.Target].AsMetaDataList.setValue( FkType, FkValue );
                    NodeTypeProp.DesignNode.postChanges( false );
                }
                _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
            }
        }

    }//CswNbtFieldTypeRuleRelationship

}//namespace ChemSW.Nbt.MetaData
