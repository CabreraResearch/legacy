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

    public class CswNbtFieldTypeRuleQuantity : ICswNbtFieldTypeRule
    {
        public sealed class SubFieldName : ICswNbtFieldTypeRuleSubFieldName
        {
            public static CswEnumNbtSubFieldName Value = CswEnumNbtSubFieldName.Value;
            public static CswEnumNbtSubFieldName NodeID = CswEnumNbtSubFieldName.NodeID;
            public static CswEnumNbtSubFieldName Name = CswEnumNbtSubFieldName.Name;
        }

        private CswNbtFieldTypeRuleDefaultImpl _CswNbtFieldTypeRuleDefault = null;
        private CswNbtFieldResources _CswNbtFieldResources = null;
        public CswNbtSubField QuantitySubField;
        public CswNbtSubField UnitNameSubField;
        public CswNbtSubField UnitIdSubField;
        public CswNbtSubField Val_kg_SubField;
        public CswNbtSubField Val_Liters_SubField;

        public CswNbtFieldTypeRuleQuantity( CswNbtFieldResources CswNbtFieldResources )
        {
            _CswNbtFieldResources = CswNbtFieldResources;
            _CswNbtFieldTypeRuleDefault = new CswNbtFieldTypeRuleDefaultImpl( _CswNbtFieldResources );

            QuantitySubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_Numeric, SubFieldName.Value, true );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            QuantitySubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( QuantitySubField, true );

            UnitIdSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1_FK, SubFieldName.NodeID, true );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            UnitIdSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.In );
            SubFields.add( UnitIdSubField );

            UnitNameSubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field1, SubFieldName.Name );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Begins );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Ends );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Contains );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotContains );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            UnitNameSubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( UnitNameSubField );

            Val_kg_SubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field2_Numeric, CswEnumNbtSubFieldName.Val_kg, true );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            Val_kg_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( Val_kg_SubField );

            Val_Liters_SubField = new CswNbtSubField( _CswNbtFieldResources, CswEnumNbtPropColumn.Field3_Numeric, CswEnumNbtSubFieldName.Val_Liters, true );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Equals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotEquals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThanOrEquals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.GreaterThan );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThan );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.LessThanOrEquals );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.NotNull );
            Val_Liters_SubField.SupportedFilterModes.Add( CswEnumNbtFilterMode.Null );
            SubFields.add( Val_Liters_SubField );

        }//ctor

        public CswNbtSubFieldColl SubFields
        {
            get
            {
                return ( _CswNbtFieldTypeRuleDefault.SubFields );
            }
        }

        public bool SearchAllowed { get { return ( _CswNbtFieldTypeRuleDefault.SearchAllowed ); } }
        public string renderViewPropFilter( ICswNbtUser RunAsUser, CswNbtViewPropertyFilter CswNbtViewPropertyFilterIn, Dictionary<string, string> ParameterCollection, int FilterNumber )
        {
            // BZ 7941
            bool UseNumericHack = CswNbtViewPropertyFilterIn.SubfieldName == SubFieldName.Value;
            return ( _CswNbtFieldTypeRuleDefault.renderViewPropFilter( RunAsUser, CswNbtViewPropertyFilterIn, ParameterCollection, FilterNumber, UseNumericHack ) );
        }//makeWhereClause()

        public string FilterModeToString( CswNbtSubField SubField, CswEnumNbtFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries );
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries, UnitIdSubField );
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
                    CswNbtViewId ViewId = DesignNTPNode.AttributeProperty[AttributeName.UnitView].AsViewReference.ViewId;
                    CswNbtView View = _CswNbtFieldResources.CswNbtResources.ViewSelect.restoreView( ViewId );
                    if( CswEnumNbtViewRelatedIdType.Unknown != FkProp.Type && Int32.MinValue != FkProp.Id )
                    {
                        //We have valid values that are different that what is currently set
                        CswNbtFieldTypeRuleDefaultImpl.setDefaultView( _CswNbtFieldResources.CswNbtResources.MetaData, DesignNTPNode, View, FkProp.Type, FkProp.Id, false );
                    }
                    else
                    {
                        //Make sure a default view is set
                        CswEnumNbtViewRelatedIdType TargetType = DesignNTPNode.AttributeProperty[AttributeName.UnitTarget].AsMetaDataList.Type;
                        Int32 TargetId = DesignNTPNode.AttributeProperty[AttributeName.UnitTarget].AsMetaDataList.Id;
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
            public const string Precision = CswEnumNbtPropertyAttributeName.Precision;
            public const string MinimumValue = CswEnumNbtPropertyAttributeName.MinimumValue;
            public const string MaximumValue = CswEnumNbtPropertyAttributeName.MaximumValue;
            public const string UnitTarget = CswEnumNbtPropertyAttributeName.UnitTarget;
            public const string UnitView = CswEnumNbtPropertyAttributeName.UnitView;
            public const string DefaultValue = CswEnumNbtPropertyAttributeName.DefaultValue;
            public const string QuantityOptional = CswEnumNbtPropertyAttributeName.QuantityOptional;
            public const string ExcludeRangeLimits = CswEnumNbtPropertyAttributeName.ExcludeRangeLimits;
        }
        public Collection<CswNbtFieldTypeAttribute> getAttributes()
        {
            Collection<CswNbtFieldTypeAttribute> ret = _CswNbtFieldTypeRuleDefault.getAttributes( CswEnumNbtFieldType.Quantity );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = AttributeName.Precision,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numberprecision
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = AttributeName.MinimumValue,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numberminvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = AttributeName.MaximumValue,
                    AttributeFieldType = CswEnumNbtFieldType.Number,
                    Column = CswEnumNbtPropertyAttributeColumn.Numbermaxvalue
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = AttributeName.UnitTarget,
                AttributeFieldType = CswEnumNbtFieldType.MetaDataList,
                Column = CswEnumNbtPropertyAttributeColumn.Fktype,
                SubFieldName = CswNbtFieldTypeRuleMetaDataList.SubFieldName.Type
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = AttributeName.UnitTarget,
                AttributeFieldType = CswEnumNbtFieldType.MetaDataList,
                Column = CswEnumNbtPropertyAttributeColumn.Fkvalue,
                SubFieldName = CswNbtFieldTypeRuleMetaDataList.SubFieldName.Id
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
                {
                    OwnerFieldType = CswEnumNbtFieldType.Quantity,
                    Name = AttributeName.UnitView,
                    AttributeFieldType = CswEnumNbtFieldType.ViewReference,
                    Column = CswEnumNbtPropertyAttributeColumn.Nodeviewid,
                    SubFieldName = CswNbtFieldTypeRuleViewReference.SubFieldName.ViewID
                } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = AttributeName.DefaultValue,
                Column = CswEnumNbtPropertyAttributeColumn.Defaultvalueid,
                AttributeFieldType = CswEnumNbtFieldType.Quantity
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = AttributeName.QuantityOptional,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute1,
                AttributeFieldType = CswEnumNbtFieldType.Logical
            } );
            ret.Add( new CswNbtFieldTypeAttribute( _CswNbtFieldResources.CswNbtResources )
            {
                OwnerFieldType = CswEnumNbtFieldType.Quantity,
                Name = AttributeName.ExcludeRangeLimits,
                Column = CswEnumNbtPropertyAttributeColumn.Attribute2,
                AttributeFieldType = CswEnumNbtFieldType.Logical
            } );
            return ret;
        }

        public void afterCreateNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            if( null != NodeTypeProp && null != NodeTypeProp.DesignNode )
            {
                string FkType = NodeTypeProp.FKType;
                Int32 FkValue = NodeTypeProp.FKValue;

                if( false == string.IsNullOrEmpty( FkType ) &&
                    Int32.MinValue != FkValue )
                {
                    //NodeTypeProp.SetFK( FkType, FkValue );
                    NodeTypeProp.DesignNode.AttributeProperty[AttributeName.UnitTarget].AsMetaDataList.setValue( FkType, FkValue );
                    NodeTypeProp.DesignNode.postChanges( false );
                }

                _CswNbtFieldTypeRuleDefault.afterCreateNodeTypeProp( NodeTypeProp );
            }
        }

        public string getHelpText()
        {
            return string.Empty;
        }

        public void onBeforeWriteDesignNode( CswNbtObjClassDesignNodeTypeProp DesignNTPNode ) { }

    }//ICswNbtFieldTypeRule

}//namespace ChemSW.Nbt.MetaData
