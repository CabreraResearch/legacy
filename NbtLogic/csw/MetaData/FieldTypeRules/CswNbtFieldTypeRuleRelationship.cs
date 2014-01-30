using System;
using System.Collections.Generic;
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

        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
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
            string ret = _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber );

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

        public void onSetFk( CswNbtObjClassDesignNodeTypeProp DesignNTPNode )
        {
            Collection<CswNbtFieldTypeAttribute> Attributes = getAttributes();
            CswNbtFieldTypeAttribute FkTypeAttr = Attributes.FirstOrDefault( a => a.Column == CswEnumNbtPropertyAttributeColumn.Fktype );
            if( DesignNTPNode.AttributeProperty.ContainsKey( FkTypeAttr.Name ) )
            {
                CswNbtNodePropWrapper FkTypeProp = DesignNTPNode.AttributeProperty[FkTypeAttr.Name];
                if( null != FkTypeProp && FkTypeProp.wasAnySubFieldModified( false ) )
                {
                    CswNbtNodePropMetaDataList FkProp = FkTypeProp.AsMetaDataList;
                    CswNbtViewId ViewId = DesignNTPNode.AttributeProperty[AttributeName.View].AsViewReference.ViewId;
                    CswNbtView View = _CswNbtFieldResources.CswNbtResources.ViewSelect.restoreView( ViewId );
                    if( CswEnumNbtViewRelatedIdType.Unknown != FkProp.Type && Int32.MinValue != FkProp.Id )
                    {
                        //We have valid values that are different that what is currently set
                        CswNbtFieldTypeRuleDefaultImpl.setDefaultView( _CswNbtFieldResources.CswNbtResources.MetaData, DesignNTPNode, View, FkProp.Type, FkProp.Id, false );
                    }
                    else
                    {
                        //Make sure a default view is set
                        CswEnumNbtViewRelatedIdType TargetType = DesignNTPNode.AttributeProperty[AttributeName.Target].AsMetaDataList.Type;
                        Int32 TargetId = DesignNTPNode.AttributeProperty[AttributeName.Target].AsMetaDataList.Id;
                        if( CswEnumNbtViewRelatedIdType.Unknown != TargetType && Int32.MinValue != TargetId )
                        {
                            CswNbtFieldTypeRuleDefaultImpl.setDefaultView( _CswNbtFieldResources.CswNbtResources.MetaData, DesignNTPNode, View, TargetType, TargetId, true );
                        }
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
            //if( null != NodeTypeProp && null != NodeTypeProp.DesignNode )
            //{
            //    string FkType = NodeTypeProp.FKType;
            //    Int32 FkValue = NodeTypeProp.FKValue;

            //    if( false == string.IsNullOrEmpty( FkType ) &&
            //        Int32.MinValue != FkValue )
            //    {
            //        //NodeTypeProp.SetFK( FkType, FkValue );
            //        NodeTypeProp.DesignNode.AttributeProperty[AttributeName.Target].AsMetaDataList.setValue( FkType, FkValue );
            //        NodeTypeProp.DesignNode.postChanges( false );
            //    }
            _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
            //}
        }

        public string getHelpText()
        {
            return "Enter \"me\" to use the current user as the search term";
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//CswNbtFieldTypeRuleRelationship

}//namespace ChemSW.Nbt.MetaData
