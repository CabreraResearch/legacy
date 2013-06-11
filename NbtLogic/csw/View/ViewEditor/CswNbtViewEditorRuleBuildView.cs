using System;
using System.Linq;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleBuildView: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleBuildView( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.BuildView;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            CswNbtViewId selectedViewId = new CswNbtViewId( Request.ViewId );
            if( null == CurrentView )
            {
                CurrentView = _CswNbtResources.ViewSelect.restoreView( selectedViewId );
                if( null == CurrentView )
                {
                    CswNbtSessionDataId sessionDataId = new CswNbtSessionDataId( Request.ViewId );
                    if( sessionDataId.isSet() )
                    {
                        selectedViewId = _CswNbtResources.ViewSelect.getSessionView( sessionDataId ).ViewId;
                        CurrentView = _CswNbtResources.ViewSelect.restoreView( selectedViewId );
                    }
                }
            }

            if( null != CurrentView )
            {
                CswNbtView TempView = new CswNbtView( _CswNbtResources );
                if( CurrentView.Visibility.Equals( CswEnumNbtViewVisibility.Property ) )
                {
                    _getPropertyViewProps( TempView, Return );
                }
                else
                {
                    _getViewProps( TempView, Return );
                }
            }

            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            _addRelationship();
            CswNbtViewEditorData Return = Request;
            base.Finalize( Return );
            return Return;
        }

        #region Private

        private void _addRelationship()
        {
            if( Request.Relationship.PropOwner == CswEnumNbtViewPropOwnerType.First && Int32.MinValue != Request.Relationship.FirstId )
            {
                CswNbtViewRelationship parentToAddTo = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( Request.Relationship.ParentArbitraryId );
                ICswNbtMetaDataProp prop = null;
                if( Request.Relationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                {
                    prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Relationship.PropId );
                }
                else
                {
                    prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Relationship.PropId );
                }

                if( null != prop )
                {
                    CurrentView.AddViewRelationship( parentToAddTo, Request.Relationship.PropOwner, prop, true );
                }
            }
            else
            {
                if( Request.Relationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( Request.Relationship.SecondId );
                    CswNbtViewRelationship parent = null;
                    if( CurrentView.Visibility == CswEnumNbtViewVisibility.Property )
                    {
                        parent = _addPropViewRelationshiop( CurrentView, Request.Relationship );
                    }
                    else
                    {
                        parent = CurrentView.AddViewRelationship( nt, true );
                    }
                    _addNameTemplateProps( CurrentView, parent, nt );
                }
                else if( Request.Relationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( Request.Relationship.SecondId );
                    CswNbtViewRelationship parent = null;
                    if( CurrentView.Visibility == CswEnumNbtViewVisibility.Property )
                    {
                        parent = _addPropViewRelationshiop( CurrentView, Request.Relationship );
                    }
                    else
                    {
                        parent = CurrentView.AddViewRelationship( oc, true );
                    }
                    foreach( CswNbtMetaDataNodeType NodeType in oc.getNodeTypes() )
                    {
                        _addNameTemplateProps( CurrentView, parent, NodeType );
                    }
                }
                else
                {
                    CswNbtMetaDataPropertySet ps = _CswNbtResources.MetaData.getPropertySet( Request.Relationship.SecondId );
                    CswNbtViewRelationship parent = null;
                    if( CurrentView.Visibility == CswEnumNbtViewVisibility.Property )
                    {
                        parent = _addPropViewRelationshiop( CurrentView, Request.Relationship );
                    }
                    else
                    {
                        parent = CurrentView.AddViewRelationship( ps, true );
                    }
                    foreach( CswNbtMetaDataObjectClass oc in ps.getObjectClasses() )
                    {
                        foreach( CswNbtMetaDataNodeType NodeType in oc.getNodeTypes() )
                        {
                            _addNameTemplateProps( CurrentView, parent, NodeType );
                        }
                    }
                }
            }
            _addExistingProps( CurrentView );
        }

        private void _getPropertyViewProps( CswNbtView TempView, CswNbtViewEditorData Return )
        {
            TempView.Visibility = CswEnumNbtViewVisibility.Property;
            CswNbtViewRelationship propRoot = null; //this is OK if it's null
            TempView = _CswNbtResources.ViewSelect.restoreView( CurrentView.ToString() );
            propRoot = TempView.Root.ChildRelationships[0]; //grab the root level for property views
            propRoot.ChildRelationships.Clear();

            foreach( CswNbtViewRelationship related in getViewChildRelationshipOptions( CurrentView, propRoot.ArbitraryId ) )
            {
                if( related.SecondType.Equals( CswEnumNbtViewRelatedIdType.NodeTypeId ) )
                {
                    CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( related.SecondId );
                    if( null != nt )
                    {
                        _addNameTemplateProps( CurrentView, related, nt );
                    }
                }
                else
                {
                    CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( related.SecondId );
                    if( null != oc )
                    {
                        foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                        {
                            _addNameTemplateProps( CurrentView, related, nt );
                        }
                    }
                }

                Return.Step2.Relationships.Add( related );
            }
        }

        private void _getViewProps( CswNbtView TempView, CswNbtViewEditorData Return )
        {
            foreach( CswNbtMetaDataNodeType NodeType in _CswNbtResources.MetaData.getNodeTypes() )
            {
                CswNbtViewRelationship Relationship = TempView.AddViewRelationship( NodeType, true );
                CswNbtViewNode foundNode = CurrentView.FindViewNodeByArbitraryId( Relationship.ArbitraryId );
                Return.Step2.Relationships.Add( Relationship );
                _addNameTemplateProps( TempView, Relationship, NodeType );
            }

            foreach( CswNbtMetaDataObjectClass ObjClass in _CswNbtResources.MetaData.getObjectClasses().OrderBy( ObjClass => ObjClass.ObjectClass.Value ) )
            {
                CswNbtViewRelationship Relationship = TempView.AddViewRelationship( ObjClass, true );
                CswNbtViewNode foundNode = CurrentView.FindViewNodeByArbitraryId( Relationship.ArbitraryId );
                Return.Step2.Relationships.Add( Relationship );
                foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                {
                    _addNameTemplateProps( TempView, Relationship, NodeType );
                }
            }

            foreach( CswNbtMetaDataPropertySet PropSet in _CswNbtResources.MetaData.getPropertySets().OrderBy( PropSet => PropSet.Name ) )
            {
                CswNbtViewRelationship Relationship = TempView.AddViewRelationship( PropSet, true );
                CswNbtViewNode foundNode = CurrentView.FindViewNodeByArbitraryId( Relationship.ArbitraryId );
                Return.Step2.Relationships.Add( Relationship );
                foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                {
                    foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                    {
                        _addNameTemplateProps( TempView, Relationship, NodeType );
                    }
                }
            }
        }

        private CswNbtViewRelationship _addPropViewRelationshiop( CswNbtView View, CswNbtViewRelationship Relationship )
        {
            ICswNbtMetaDataProp prop = null;
            if( Relationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
            {
                prop = _CswNbtResources.MetaData.getNodeTypeProp( Relationship.PropId );
            }
            else
            {
                prop = _CswNbtResources.MetaData.getObjectClassProp( Relationship.PropId );
            }
            CswNbtViewRelationship parentRel = (CswNbtViewRelationship) View.FindViewNodeByArbitraryId( Relationship.ParentArbitraryId );
            CswNbtViewRelationship parent = View.AddViewRelationship( parentRel, Relationship.PropOwner, prop, true );
            return parent;
        }

        #endregion

    }
}
