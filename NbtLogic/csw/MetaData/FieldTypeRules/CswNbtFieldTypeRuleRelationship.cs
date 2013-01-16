using System;
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

            NameSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1, CswNbtSubField.SubFieldName.Name );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Begins );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Ends );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Contains );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotContains );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            NameSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
            SubFields.add( NameSubField, true );

            NodeIDSubField = new CswNbtSubField( _CswNbtFieldResources, CswNbtSubField.PropColumn.Field1_FK, CswNbtSubField.SubFieldName.NodeID, true );
            NodeIDSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Equals );
            NodeIDSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
            NodeIDSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.NotNull );
            NodeIDSubField.SupportedFilterModes.Add( CswNbtPropFilterSql.PropertyFilterMode.Null );
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
            CswNbtSubField.SubFieldName OldSubfieldName = CswNbtViewPropertyFilterIn.SubfieldName;
            CswNbtPropFilterSql.PropertyFilterMode OldFilterMode = CswNbtViewPropertyFilterIn.FilterMode;
            string OldValue = CswNbtViewPropertyFilterIn.Value;

            // BZ 8558
            if( OldSubfieldName == NameSubField.Name && OldValue.ToLower() == "me" )
            {
                CswNbtViewProperty Prop = (CswNbtViewProperty) CswNbtViewPropertyFilterIn.Parent;
                ICswNbtMetaDataProp MetaDataProp = null;
                if( Prop.Type == NbtViewPropType.NodeTypePropId )
                    MetaDataProp = Prop.NodeTypeProp;
                else if( Prop.Type == NbtViewPropType.ObjectClassPropId )
                    MetaDataProp = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClassProp( Prop.ObjectClassPropId );

                if( MetaDataProp != null && MetaDataProp.IsUserRelationship() )
                {
                    if( CswNbtViewPropertyFilterIn.Value.ToLower() == "me" )
                    {
                        CswNbtViewPropertyFilterIn.SubfieldName = NodeIDSubField.Name;
                        CswNbtViewPropertyFilterIn.FilterMode = CswNbtPropFilterSql.PropertyFilterMode.Equals;
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

        public string FilterModeToString( CswNbtSubField SubField, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return _CswNbtFieldTypeRuleDefault.FilterModeToString( SubField, FilterMode );
        }

        public void AddUniqueFilterToView( CswNbtView View, CswNbtViewProperty UniqueValueViewProperty, CswNbtNodePropWrapper PropertyValueToCheck, bool EnforceNullEntries = false )
        {
            _CswNbtFieldTypeRuleDefault.AddUniqueFilterToView( View, UniqueValueViewProperty, PropertyValueToCheck, EnforceNullEntries, NodeIDSubField );
        }

        private CswNbtView _setDefaultView( CswNbtMetaDataNodeTypeProp MetaDataProp, NbtViewRelatedIdType RelatedIdType, Int32 inFKValue, bool OnlyCreateIfNull )
        {
            //CswNbtMetaDataNodeTypeProp ThisNtProp = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeTypeProp( MetaDataProp.PropId );
            CswNbtView RetView = _CswNbtFieldResources.CswNbtResources.ViewSelect.restoreView( MetaDataProp.ViewId );
            if( RelatedIdType != NbtViewRelatedIdType.Unknown &&
                ( null == RetView ||
                  RetView.Root.ChildRelationships.Count == 0 ||
                  false == OnlyCreateIfNull ) )
            {

                if( null != RetView )
                {
                    RetView.Root.ChildRelationships.Clear();
                }

                if( RelatedIdType == NbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass TargetOc = _CswNbtFieldResources.CswNbtResources.MetaData.getObjectClass( inFKValue );
                    if( null == TargetOc )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                    }
                    RetView = TargetOc.CreateDefaultView();
                }
                else if( RelatedIdType == NbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType TargetNt = _CswNbtFieldResources.CswNbtResources.MetaData.getNodeType( inFKValue );
                    if( null == TargetNt )
                    {
                        throw new CswDniException( ErrorType.Error, "Cannot create a relationship without a valid target.", "Attempted to create a relationship to objectclassid: " + inFKValue + ", but the target is null." );
                    }
                    RetView = TargetNt.CreateDefaultView();
                }

                RetView.ViewId = MetaDataProp.ViewId;
                RetView.Visibility = NbtViewVisibility.Property;
                RetView.ViewMode = NbtViewRenderingMode.List;
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
            NbtViewRelatedIdType NewFkPropIdType = (NbtViewRelatedIdType) inFKType;
            //Enum.TryParse( inFKType, true, out NewFkPropIdType );

            //Current PropIdTypes
            NbtViewRelatedIdType CurrentFkPropIdType = (NbtViewRelatedIdType) MetaDataProp.FKType;
            //Enum.TryParse( MetaDataProp.FKType, true, out CurrentFkPropIdType );

            //We have valid values that are different that what is currently set
            if( ( false == string.IsNullOrEmpty( inFKType ) &&
                  Int32.MinValue != inFKValue &&
                  NewFkPropIdType != NbtViewRelatedIdType.Unknown
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
