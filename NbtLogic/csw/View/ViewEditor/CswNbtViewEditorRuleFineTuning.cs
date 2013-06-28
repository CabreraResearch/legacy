using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleFineTuning: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleFineTuning( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.FineTuning;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();
            Return.Step4.ViewJson = CswConvert.ToString( CurrentView.ToJson() );
            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            if( Request.Action == "Click" )
            {
                CswNbtViewNode foundNode = Request.CurrentView.FindViewNodeByArbitraryId( Request.ArbitraryId );
                if( null != foundNode )
                {
                    CswNbtView TempView = _CswNbtResources.ViewSelect.restoreView( CurrentView.ToString() );
                    if( foundNode is CswNbtViewPropertyFilter )
                    {
                        Return.Step6.FilterNode = (CswNbtViewPropertyFilter) foundNode;
                    }
                    else if( foundNode is CswNbtViewRelationship )
                    {
                        CswNbtViewRelationship asRelationship = (CswNbtViewRelationship) foundNode;

                        if( asRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                        {
                            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( asRelationship.SecondId );
                            Return.Step6.Properties = _getProps( NodeType, TempView, new HashSet<string>(), asRelationship, false );
                        }
                        else if( asRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                        {
                            CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( asRelationship.SecondId );
                            Return.Step6.Properties = _getProps( ObjClass, TempView, new HashSet<string>(), asRelationship, false );
                        }
                        else
                        {
                            CswNbtMetaDataPropertySet PropSet = _CswNbtResources.MetaData.getPropertySet( asRelationship.SecondId );
                            HashSet<string> seenProps = new HashSet<string>();
                            foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                            {
                                foreach( CswNbtViewProperty prop in _getProps( ObjClass, TempView, seenProps, asRelationship ).OrderBy( prop => prop.TextLabel ) )
                                {
                                    Return.Step6.Properties.Add( prop );
                                }
                            }
                        }

                        Return.Step6.Relationships = getViewChildRelationshipOptions( CurrentView, asRelationship.ArbitraryId );

                        Return.Step6.RelationshipNode = asRelationship;
                    }
                    else if( foundNode is CswNbtViewRoot && CurrentView.Visibility != CswEnumNbtViewVisibility.Property ) //can't add to view root on Prop view
                    {
                        TempView.Root.ChildRelationships.Clear();
                        foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes().OrderBy( NT => NT.NodeTypeName ) )
                        {
                            Return.Step6.Relationships.Add( TempView.AddViewRelationship( NodeType, false ) );
                        }
                        foreach( CswNbtMetaDataObjectClass ObjClass in _CswNbtResources.MetaData.getObjectClasses().OrderBy( OC => OC.ObjectClass.Value ) )
                        {
                            Return.Step6.Relationships.Add( TempView.AddViewRelationship( ObjClass, false ) );
                        }
                        foreach( CswNbtMetaDataPropertySet PropSet in _CswNbtResources.MetaData.getPropertySets().OrderBy( PS => PS.Name ) )
                        {
                            Return.Step6.Relationships.Add( TempView.AddViewRelationship( PropSet, false ) );
                        }
                        Return.Step6.RootNode = (CswNbtViewRoot) foundNode;
                    }
                    else if( foundNode is CswNbtViewProperty )
                    {
                        Return.Step6.PropertyNode = (CswNbtViewProperty) foundNode;
                        Request.Relationship = (CswNbtViewRelationship) foundNode.Parent; //getFilterProps needs Request.Relationship to be populated
                        _getFilterProps( Return );
                    }
                }
            }
            else if( Request.Action == "AddProp" )
            {
                ICswNbtMetaDataProp prop = null;
                if( Request.Property.Type == CswEnumNbtViewPropType.NodeTypePropId )
                {
                    prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
                }
                else
                {
                    prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
                }
                CswNbtViewRelationship relToAddTo = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( Request.Relationship.ArbitraryId );
                CurrentView.AddViewProperty( relToAddTo, prop, CurrentView.getOrderedViewProps( false ).Count + 1 );
            }
            else if( Request.Action == "AddRelationship" )
            {
                CswNbtViewRelationship relToAddTo = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( Request.ArbitraryId );
                ICswNbtMetaDataProp prop = null;
                if( Request.Relationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                {
                    prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Relationship.PropId );
                }
                else
                {
                    prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Relationship.PropId );
                }
                CurrentView.AddViewRelationship( relToAddTo, Request.Relationship.PropOwner, prop, true );
            }
            else if( Request.Action == "AddFilter" )
            {
                CswNbtViewProperty propNode = (CswNbtViewProperty) CurrentView.FindViewNodeByArbitraryId( Request.PropArbId );
                CurrentView.AddViewPropertyFilter( propNode,
                                                   Conjunction : (CswEnumNbtFilterConjunction) Request.FilterConjunction,
                                                   SubFieldName : (CswEnumNbtSubFieldName) Request.FilterSubfield,
                                                   FilterMode : (CswEnumNbtFilterMode) Request.FilterMode,
                                                   Value : Request.FilterValue
                    );
            }
            else if( Request.Action == "RemoveNode" )
            {
                CswNbtViewNode nodeToRemove = CurrentView.FindViewNodeByArbitraryId( Request.ArbitraryId );
                nodeToRemove.Parent.RemoveChild( nodeToRemove );
            }
            else if( Request.Action == "UpdateView" )
            {
                string grp = string.Empty;
                if( null != Request.Property )
                {
                    CswNbtViewRelationship selectedPropsParent = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( Request.Property.ParentArbitraryId );
                    Request.Property.Parent = selectedPropsParent;
                    CswNbtViewProperty rel = (CswNbtViewProperty) CurrentView.FindViewNodeByArbitraryId( Request.Property.ArbitraryId );
                    if( null == rel )
                    {
                        CswNbtViewRelationship parent = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( Request.Property.ParentArbitraryId );
                        ICswNbtMetaDataProp prop = null;
                        if( Request.Property.Type == CswEnumNbtViewPropType.NodeTypePropId )
                        {
                            prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
                        }
                        else
                        {
                            prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
                        }
                        rel = CurrentView.AddViewProperty( parent, prop );
                    }
                    grp = rel.TextLabel;
                }

                CurrentView.GridGroupByCol = grp;
            }

            base.Finalize( Return );
            return Return;
        }


    }
}
