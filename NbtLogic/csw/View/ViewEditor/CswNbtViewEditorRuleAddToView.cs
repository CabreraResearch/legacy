
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ViewEditor
{
    public class CswNbtViewEditorRuleAddToView: CswNbtViewEditorRule
    {
        public CswNbtViewEditorRuleAddToView( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
            : base( CswNbtResources, IncomingRequest )
        {
            RuleName = CswEnumNbtViewEditorRuleName.AddToView;
        }

        public override CswNbtViewEditorData GetStepData()
        {
            CswNbtViewEditorData Return = new CswNbtViewEditorData();

            Return.Step3 = new CswNbtViewEditorStep3();

            string ViewStr = CurrentView.ToString();
            CswNbtView TempView = _CswNbtResources.ViewSelect.restoreView( ViewStr );

            HashSet<string> seenProps = new HashSet<string>();
            Collection<CswNbtViewProperty> relatedProps = new Collection<CswNbtViewProperty>();

            if( CurrentView.ViewMode.Equals( CswEnumNbtViewRenderingMode.Grid ) || CurrentView.ViewMode.Equals( CswEnumNbtViewRenderingMode.Table ) )
            {
                CswNbtViewRoot.forEachRelationship forEachRelationship = relationship =>
                {
                    //For property views, we ignore the top lvl relationship
                    if( ( false == ( relationship.Parent is CswNbtViewRoot ) && CurrentView.Visibility == CswEnumNbtViewVisibility.Property ) ||
                        CurrentView.Visibility != CswEnumNbtViewVisibility.Property )
                    {
                        foreach( CswNbtViewProperty ExistingViewProp in relationship.Properties )
                        {
                            if( false == seenProps.Contains( ExistingViewProp.TextLabel ) )
                            {
                                seenProps.Add( ExistingViewProp.TextLabel );
                                Return.Step3.Properties.Add( ExistingViewProp );
                            }
                        }

                        _populatePropsCollection( relationship, TempView, Return.Step3.Properties, seenProps );

                        //Get all props related to this relationship
                        if( CurrentView.ViewMode == CswEnumNbtViewRenderingMode.Grid )
                        {
                            Collection<CswNbtViewRelationship> rels = getViewChildRelationshipOptions( TempView, relationship.ArbitraryId );
                            foreach( CswNbtViewRelationship relatedRelationship in rels )
                            {
                                if( false == seenProps.Contains( relatedRelationship.TextLabel ) )
                                {
                                    relatedRelationship.Parent = relationship;
                                    Return.Step3.SecondRelationships.Add( relatedRelationship );
                                    _populatePropsCollection( relatedRelationship, TempView, relatedProps, seenProps, true, true, false );
                                    relatedRelationship.Properties = new Collection<CswNbtViewProperty>(); //otherwise this has every prop
                                    seenProps.Add( relatedRelationship.TextLabel );
                                }
                            }
                        }
                    }
                };
                TempView.Root.eachRelationship( forEachRelationship, null );

                HashSet<string> seenRelated = new HashSet<string>();
                foreach( CswNbtViewProperty vp in relatedProps )
                {
                    if( false == seenRelated.Contains( vp.TextLabel ) )
                    {
                        seenRelated.Add( vp.TextLabel );
                        Return.Step3.Properties.Add( vp );
                    }
                }
            }
            else if( CurrentView.ViewMode.Equals( CswEnumNbtViewRenderingMode.Tree ) )
            {
                CswNbtViewRoot.forEachRelationship forEachRelationship = relationship =>
                {
                    foreach( CswNbtViewRelationship related in getViewChildRelationshipOptions( CurrentView, relationship.ArbitraryId ) )
                    {
                        ICswNbtMetaDataProp prop;
                        if( related.PropType.Equals( CswEnumNbtViewPropIdType.ObjectClassPropId ) )
                        {
                            prop = _CswNbtResources.MetaData.getObjectClassProp( related.PropId );
                        }
                        else
                        {
                            prop = _CswNbtResources.MetaData.getNodeTypeProp( related.PropId );
                        }

                        CswNbtViewRelationship tempRel = (CswNbtViewRelationship) TempView.FindViewNodeByArbitraryId( relationship.ArbitraryId );
                        Return.Step3.SecondRelationships.Add( TempView.AddViewRelationship( tempRel, related.PropOwner, prop, true ) );
                    }
                };
                CurrentView.Root.eachRelationship( forEachRelationship, null );
            }

            base.Finalize( Return );
            return Return;
        }

        public override CswNbtViewEditorData HandleAction()
        {
            ICswNbtMetaDataProp prop = null;
            if( Request.Property.Type.Equals( CswEnumNbtViewPropType.NodeTypePropId ) )
            {
                prop = _CswNbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
            }
            else if( Request.Property.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
            {
                prop = _CswNbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
            }

            if( Request.Action == "AddProp" )
            {
                _addProp( prop );
            }
            else
            {
                _removeProp( prop );
            }

            CswNbtViewEditorData Return = new CswNbtViewEditorData();
            base.Finalize( Return );
            return Return;
        }

        #region Private

        private void _addProp( ICswNbtMetaDataProp prop )
        {
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                //we have to iterate a temp view, but use the relationships on the "real" view
                relationship = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( relationship.ArbitraryId );

                //For property views, we ignore the top lvl relationship
                if( ( false == ( relationship.Parent is CswNbtViewRoot ) && CurrentView.Visibility == CswEnumNbtViewVisibility.Property ) ||
                    CurrentView.Visibility != CswEnumNbtViewVisibility.Property )
                {

                    int Id = relationship.SecondId;
                    CswEnumNbtViewRelatedIdType type = relationship.SecondType;
                    if( relationship.PropOwner.Equals( CswEnumNbtViewPropOwnerType.First ) && Int32.MinValue != relationship.FirstId && CurrentView.Visibility == CswEnumNbtViewVisibility.Property )
                    {
                        Id = relationship.FirstId;
                        type = relationship.FirstType;
                    }

                    CswNbtViewProperty existingViewProp = relationship.findPropertyByName( prop.PropName );
                    if( null != existingViewProp )
                    {
                        existingViewProp.ShowInGrid = true;
                    }
                    else
                    {
                        if( null != Request.Relationship )
                        {
                            int ownerTargetId = Request.Relationship.SecondId;

                            Collection<CswNbtViewRelationship> relations = getViewChildRelationshipOptions( CurrentView, relationship.ArbitraryId );
                            foreach( CswNbtViewRelationship related in relations )
                            {
                                ICswNbtMetaDataProp relProp = related.getProp();
                                int relatedTargetId = related.SecondId;

                                if( ownerTargetId == relatedTargetId && Request.Relationship.getRelatedType() == related.getRelatedType() )
                                {
                                    if( related.getRelatedType() == CswEnumNbtViewRelatedIdType.NodeTypeId )
                                    {
                                        CswNbtMetaDataNodeType ownerNT = _CswNbtResources.MetaData.getNodeType( relatedTargetId );
                                        foreach( CswNbtMetaDataNodeTypeProp ntp in ownerNT.getNodeTypeProps() )
                                        {
                                            if( ntp.PropName == prop.PropName )
                                            {
                                                CswNbtViewRelationship parentRel = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( related.ParentArbitraryId );
                                                CswNbtViewRelationship relToAddPropTo = relationship.findChildRelationshipByNodeTypeId( relatedTargetId );
                                                if( null == relToAddPropTo )
                                                {
                                                    relToAddPropTo = CurrentView.AddViewRelationship( parentRel, related.PropOwner, relProp, false );
                                                }
                                                CurrentView.AddViewProperty( relToAddPropTo, ntp );
                                            }
                                        }
                                    }
                                    else if( related.getRelatedType() == CswEnumNbtViewRelatedIdType.ObjectClassId )
                                    {
                                        CswNbtMetaDataObjectClass ownerOC = _CswNbtResources.MetaData.getObjectClass( relatedTargetId );
                                        foreach( CswNbtMetaDataNodeType nt in ownerOC.getNodeTypes() )
                                        {
                                            foreach( CswNbtMetaDataNodeTypeProp ntp in nt.getNodeTypeProps() )
                                            {
                                                if( ntp.PropName == prop.PropName )
                                                {
                                                    CswNbtViewRelationship parentRel = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( related.ParentArbitraryId );
                                                    CswNbtViewRelationship relToAddPropTo = relationship.findChildRelationshipByObjClassId( relatedTargetId );
                                                    if( null == relToAddPropTo )
                                                    {
                                                        relToAddPropTo = CurrentView.AddViewRelationship( parentRel, related.PropOwner, relProp, false );
                                                    }
                                                    CurrentView.AddViewProperty( relToAddPropTo, ntp );
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        CswNbtMetaDataPropertySet ownerPS = _CswNbtResources.MetaData.getPropertySet( relatedTargetId );
                                        foreach( CswNbtMetaDataObjectClass oc in ownerPS.getObjectClasses() )
                                        {
                                            foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                                            {
                                                foreach( CswNbtMetaDataNodeTypeProp ntp in nt.getNodeTypeProps() )
                                                {
                                                    if( ntp.PropName == prop.PropName )
                                                    {
                                                        CswNbtViewRelationship parentRel = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( related.ParentArbitraryId );
                                                        CswNbtViewRelationship relToAddPropTo = relationship.findChildRelationshipByPropSetId( relatedTargetId );
                                                        if( null == relToAddPropTo )
                                                        {
                                                            relToAddPropTo = CurrentView.AddViewRelationship( parentRel, related.PropOwner, relProp, false );
                                                        }
                                                        CurrentView.AddViewProperty( relToAddPropTo, ntp );
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            CswNbtViewRelationship realRelationship = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( relationship.ArbitraryId );

                            if( type.Equals( CswEnumNbtViewRelatedIdType.NodeTypeId ) )
                            {
                                CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( Id );
                                CswNbtMetaDataNodeTypeProp ntp = nt.getNodeTypeProp( prop.PropName );
                                if( null != ntp )
                                {
                                    CurrentView.AddViewProperty( realRelationship, ntp );
                                }
                            }
                            else if( type.Equals( CswEnumNbtViewRelatedIdType.ObjectClassId ) )
                            {
                                CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( Id );
                                CswNbtMetaDataObjectClassProp ocp = oc.getObjectClassProp( prop.PropName );
                                if( null != ocp )
                                {
                                    CurrentView.AddViewProperty( realRelationship, prop );
                                }
                                else
                                {
                                    foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                                    {
                                        CswNbtMetaDataNodeTypeProp ntp = nt.getNodeTypeProp( prop.PropName );
                                        if( null != ntp )
                                        {
                                            CurrentView.AddViewProperty( realRelationship, prop );
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CswNbtMetaDataPropertySet ps = _CswNbtResources.MetaData.getPropertySet( Id );
                                CswNbtMetaDataObjectClassProp ocp = ps.getPropertySetProps().FirstOrDefault( PropSetProp => PropSetProp.PropName == prop.PropName );
                                if( null != ocp )
                                {
                                    CurrentView.AddViewProperty( realRelationship, prop );
                                }
                                else
                                {
                                    foreach( CswNbtMetaDataObjectClass oc in ps.getObjectClasses() )
                                    {
                                        foreach( CswNbtMetaDataNodeType nt in oc.getNodeTypes() )
                                        {
                                            CswNbtMetaDataNodeTypeProp ntp = nt.getNodeTypeProp( prop.PropName );
                                            if( null != ntp )
                                            {
                                                CurrentView.AddViewProperty( realRelationship, prop );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            CswNbtView tempView = _CswNbtResources.ViewSelect.restoreView( CurrentView.ToString() );
            tempView.Root.eachRelationship( eachRelationship, null );
            _addExistingProps( CurrentView );
        }

        private void _removeProp( ICswNbtMetaDataProp prop )
        {
            if( null != prop )
            {
                CurrentView.removeViewProperty( prop );

                Collection<string> doomedRels = new Collection<string>();
                CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
                {
                    if( relationship.Properties.Count == 0 && relationship.ChildRelationships.Count == 0 )
                    {
                        doomedRels.Add( relationship.ArbitraryId );
                    }
                };
                CurrentView.Root.eachRelationship( eachRelationship, null );

                foreach( string doomedRelId in doomedRels )
                {
                    CswNbtViewRelationship doomedRel = (CswNbtViewRelationship) CurrentView.FindViewNodeByArbitraryId( doomedRelId );
                    if( null != doomedRel )
                    {
                        CswNbtViewNode doomedRelsParent = CurrentView.FindViewNodeByArbitraryId( doomedRel.ParentArbitraryId );
                        if( doomedRelsParent is CswNbtViewRelationship )
                        {
                            CswNbtViewRelationship AsRelationship = (CswNbtViewRelationship) doomedRelsParent;
                            AsRelationship.removeChildRelationship( doomedRel );
                        }
                    }
                }
            }
        }

        #endregion

    }
}
