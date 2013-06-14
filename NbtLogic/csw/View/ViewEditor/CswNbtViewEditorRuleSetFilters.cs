

using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleSetFilters: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleSetFilters( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.SetFilters;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            _getFilters( Return, CurrentView );

            HashSet<string> seenRels = new HashSet<string>();
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                if( false == seenRels.Contains( relationship.TextLabel ) )
                {
                    seenRels.Add( relationship.TextLabel );
                    Return.Step4.Relationships.Add( relationship );
                }
            };
            CurrentView.Root.eachRelationship( eachRelationship, null );

            Return.Step4.ViewJson = CswConvert.ToString( CurrentView.ToJson() );

            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            if( Request.Action == "GetFilterProps" )
            {
                _getFilterProps( Return );
            }
            else if( Request.Action == "AddFilter" )
            {
                _addFilter( Return );
            }
            else
            {
                _removeFilter( Return );
            }

            base.Finalize( Return );
            return Return;
        }

        #region private

        private void _addFilter( CswNbtViewEditorData Return )
        {
            CswNbtViewProperty ViewProp = (CswNbtViewProperty) CurrentView.FindViewNodeByArbitraryId( Request.PropArbId );
            if( null != ViewProp )
            {
                CswNbtViewRelationship selectedParent = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( ViewProp.ParentArbitraryId );
                CswNbtViewRoot.forEachProperty eachProperty = property =>
                {
                    CswNbtViewRelationship currentParent = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( property.ParentArbitraryId );
                    if( property.Name == ViewProp.Name & currentParent.SecondId == selectedParent.SecondId )
                    {
                        CurrentView.AddViewPropertyFilter( property,
                                                                       Conjunction : (CswEnumNbtFilterConjunction) Request.FilterConjunction,
                                                                       SubFieldName : (CswEnumNbtSubFieldName) Request.FilterSubfield,
                                                                       FilterMode : (CswEnumNbtFilterMode) Request.FilterMode,
                                                                       Value : Request.FilterValue );
                    }
                };
                CurrentView.Root.eachRelationship( null, eachProperty );
            }
            else
            {
                ICswNbtMetaDataProp Prop = null;
                if( Request.Property.Type.Equals( CswEnumNbtViewPropType.NodeTypePropId ) )
                {
                    Prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
                }
                else if( Request.Property.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
                {
                    Prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
                }

                Dictionary<CswNbtViewRelationship, ICswNbtMetaDataProp> relsToAddTo = new Dictionary<CswNbtViewRelationship, ICswNbtMetaDataProp>();
                CswNbtViewRoot.forEachRelationship eachRelationshipToAddPropTo = relationship =>
                {
                    if( relationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( relationship.SecondId );
                        foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
                        {
                            if( ntp.PropName == Prop.PropName )
                            {
                                relsToAddTo.Add( relationship, ntp );
                            }
                        }
                    }
                    else if( relationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                    {
                        CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( relationship.SecondId );
                        foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                        {
                            foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
                            {
                                if( ntp.PropName == Prop.PropName )
                                {
                                    relsToAddTo.Add( relationship, ntp );
                                }
                            }
                        }
                    }
                    else
                    {
                        CswNbtMetaDataPropertySet PropSet = _CswNbtResources.MetaData.getPropertySet( relationship.SecondId );
                        foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                        {
                            foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                            {
                                foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
                                {
                                    if( ntp.PropName == Prop.PropName )
                                    {
                                        relsToAddTo.Add( relationship, ntp );
                                    }
                                }
                            }
                        }
                    }
                };
                CurrentView.Root.eachRelationship( eachRelationshipToAddPropTo, null );
                foreach( var parentAndProp in relsToAddTo )
                {
                    CurrentView.AddViewPropertyAndFilter( parentAndProp.Key, parentAndProp.Value,
                                                                      Value : Request.FilterValue,
                                                                      Conjunction : Request.FilterConjunction,
                                                                      SubFieldName : (CswEnumNbtSubFieldName) Request.FilterSubfield,
                                                                      FilterMode : (CswEnumNbtFilterMode) Request.FilterMode,
                                                                      ShowInGrid : false // the user is filtering on a prop not in the grid, don't show it in the grid
                        );
                }

            }

            _getFilters( Return, CurrentView );
            HashSet<string> seenRels = new HashSet<string>();
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                if( false == seenRels.Contains( relationship.TextLabel ) )
                {
                    seenRels.Add( relationship.TextLabel );
                    Return.Step4.Relationships.Add( relationship );
                }
            };
            CurrentView.Root.eachRelationship( eachRelationship, null );

            Return.Step4.ViewJson = CswConvert.ToString( CurrentView.ToJson() );
        }

        private void _removeFilter( CswNbtViewEditorData Return )
        {
            Dictionary<string, CswNbtViewPropertyFilter> filtersToRemove = new Dictionary<string, CswNbtViewPropertyFilter>();
            Collection<ICswNbtMetaDataProp> propsToRemove = new Collection<ICswNbtMetaDataProp>();

            CswNbtViewRoot.forEachProperty eachProperty = prop =>
            {
                foreach( CswNbtViewPropertyFilter filter in prop.Filters )
                {
                    if( filter.TextLabel == Request.FilterToRemove.TextLabel )
                    {
                        if( prop.ShowInGrid ) //if ShowInGrid == true, just remove the filter
                        {
                            filtersToRemove.Add( prop.UniqueId, filter );
                        }
                        else //otherwise, remove the property as well
                        {
                            ICswNbtMetaDataProp propToRemove;
                            if( prop.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
                            {
                                propToRemove = _CswNbtResources.MetaData.getObjectClassProp( prop.ObjectClassPropId );
                            }
                            else
                            {
                                propToRemove = _CswNbtResources.MetaData.getNodeTypeProp( prop.NodeTypePropId );
                            }

                            if( null != propToRemove )
                            {
                                propsToRemove.Add( propToRemove );
                            }
                        }
                    }
                }
            };
            CurrentView.Root.eachRelationship( null, eachProperty );

            foreach( var propAndFilter in filtersToRemove )
            {
                CswNbtViewProperty prop = (CswNbtViewProperty) CurrentView.FindViewNodeByUniqueId( propAndFilter.Key );
                prop.removeFilter( propAndFilter.Value );
            }
            foreach( ICswNbtMetaDataProp propToRemove in propsToRemove )
            {
                CurrentView.removeViewProperty( propToRemove );
            }

            _getFilters( Return, CurrentView );

            HashSet<string> seenRels = new HashSet<string>();
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                if( false == seenRels.Contains( relationship.TextLabel ) )
                {
                    seenRels.Add( relationship.TextLabel );
                    Return.Step4.Relationships.Add( relationship );
                }
            };
            CurrentView.Root.eachRelationship( eachRelationship, null );

            Return.Step4.ViewJson = CswConvert.ToString( CurrentView.ToJson() );
        }

        private void _getFilters( CswNbtViewEditorData Return, CswNbtView View )
        {
            HashSet<string> seenFilters = new HashSet<string>();
            CswNbtViewRoot.forEachProperty eachProp = property =>
            {
                foreach( CswNbtViewPropertyFilter filter in property.Filters )
                {
                    if( false == seenFilters.Contains( filter.TextLabel ) )
                    {
                        Return.Step4.Filters.Add( filter );
                        seenFilters.Add( filter.TextLabel );
                    }
                }
            };
            View.Root.eachRelationship( null, eachProp );
        }

        #endregion
    }
}
