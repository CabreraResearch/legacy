using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.ViewEditor
{
    public abstract class CswNbtViewEditorRule
    {
        protected CswNbtResources _CswNbtResources;
        public CswEnumNbtViewEditorRuleName RuleName = CswEnumNbtViewEditorRuleName.Unknown;

        protected CswNbtView CurrentView;
        protected CswNbtViewEditorData Request;

        protected CswNbtViewEditorRule( CswNbtResources CswNbtResources, CswNbtViewEditorData IncomingRequest )
        {
            _CswNbtResources = CswNbtResources;
            Request = IncomingRequest;

            //Whenever we get a view from a Wcf service, all View Nodes are missing the View and Parent properties
            //  because those props can't be serialized without an infinite loop - this restores them
            if( null != Request.CurrentView )
            {
                Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
                Request.CurrentView.SetResources( _CswNbtResources );
                _addViewNodeViews( Request.CurrentView );
                CurrentView = Request.CurrentView;
            }
        }

        public abstract CswNbtViewEditorData GetStepData();
        public abstract CswNbtViewEditorData HandleAction();

        public virtual void Finalize( CswNbtViewEditorData Return )
        {
            Return.CurrentView = CurrentView;
        }

        #region Helpers

        #region init
        protected void _addViewNodeViews( CswNbtView View )
        {
            CswNbtViewRoot.forEachProperty eachProperty = property =>
            {
                property.SetViewRootView( View );
                foreach( CswNbtViewPropertyFilter filter in property.Filters )
                {
                    filter.Parent = property;
                    filter.SetViewRootView( View );
                }
            };
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                if( null == relationship.Parent )
                {
                    relationship.Parent = View.Root;
                }
                relationship.SetViewRootView( View );
                foreach( CswNbtViewRelationship childRel in relationship.ChildRelationships )
                {
                    if( null == childRel.Parent )
                    {
                        childRel.Parent = relationship;
                    }
                }
                foreach( CswNbtViewProperty viewProp in relationship.Properties )
                {
                    if( null == viewProp.Parent )
                    {
                        viewProp.Parent = relationship;
                    }
                }
            };
            View.Root.eachRelationship( eachRelationship, eachProperty );
        }
        #endregion

        protected static void _addNameTemplateProps( CswNbtView View, CswNbtViewRelationship Relationship, CswNbtMetaDataNodeType NodeType )
        {
            foreach( string TemplateId in NodeType.NameTemplatePropIds )
            {
                Int32 TemplateIdInt = CswConvert.ToInt32( TemplateId );
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( TemplateIdInt );
                if( null != ntp && null == Relationship.findPropertyByName( ntp.PropName ) )
                {
                    View.AddViewProperty( Relationship, ntp );
                }
            }
        }

        protected void _addExistingProps( CswNbtView View )
        {
            HashSet<string> propNames = new HashSet<string>();

            //get all prop names
            CswNbtViewRoot.forEachProperty eachProperty = property =>
            {
                CswNbtViewRelationship Relationship = (CswNbtViewRelationship) property.Parent;
                Int32 id = Relationship.SecondId;
                CswEnumNbtViewRelatedIdType type = Relationship.SecondType;
                if( Relationship.PropOwner.Equals( CswEnumNbtViewPropOwnerType.First ) && Int32.MinValue != Relationship.FirstId && View.Visibility == CswEnumNbtViewVisibility.Property )
                {
                    id = Relationship.FirstId;
                    type = Relationship.FirstType;
                }

                ICswNbtMetaDataProp prop = null;
                if( type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( id );
                    prop = nt.getNodeTypeProp( property.Name );
                }
                else if( type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( id );
                    prop = oc.getObjectClassProp( property.Name );
                }
                else
                {
                    CswNbtMetaDataPropertySet ps = _CswNbtResources.MetaData.getPropertySet( id );
                    prop = ps.getPropertySetProps().FirstOrDefault( ocp => ocp.PropName == property.Name );
                }
                if( null != prop )
                {
                    propNames.Add( prop.PropName );
                }
            };
            View.Root.eachRelationship( null, eachProperty );

            //if a relationship meta data obj has the propname, add it to the relationship
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                //For property views, we ignore the top lvl relationship
                if( ( false == ( relationship.Parent is CswNbtViewRoot ) && View.Visibility == CswEnumNbtViewVisibility.Property ) ||
                    View.Visibility != CswEnumNbtViewVisibility.Property )
                {
                    foreach( string propName in propNames )
                    {
                        if( null == relationship.findPropertyByName( propName ) )
                        {
                            int id = relationship.SecondId;
                            CswEnumNbtViewRelatedIdType type = relationship.SecondType;

                            ICswNbtMetaDataProp prop;
                            if( type == CswEnumNbtViewRelatedIdType.NodeTypeId )
                            {
                                CswNbtMetaDataNodeType nt = _CswNbtResources.MetaData.getNodeType( id );
                                prop = nt.getNodeTypeProp( propName );
                            }
                            else if( type == CswEnumNbtViewRelatedIdType.ObjectClassId )
                            {
                                CswNbtMetaDataObjectClass oc = _CswNbtResources.MetaData.getObjectClass( id );
                                prop = oc.getObjectClassProp( propName );
                            }
                            else
                            {
                                CswNbtMetaDataPropertySet ps = _CswNbtResources.MetaData.getPropertySet( id );
                                prop = ps.getPropertySetProps().FirstOrDefault( ocp => ocp.PropName == propName );
                            }

                            if( null != prop )
                            {
                                View.AddViewProperty( relationship, prop );
                            }
                        }
                    }
                }
            };
            View.Root.eachRelationship( eachRelationship, null );
        }

        protected void _populatePropsCollection( CswNbtViewRelationship relationship, CswNbtView TempView, Collection<CswNbtViewProperty> Props,
            HashSet<string> seenProps, bool UseMetaName = false, bool overrideFirst = false, bool DoCheck = true )
        {
            CswEnumNbtViewRelatedIdType type;
            Int32 Id;
            if( relationship.PropOwner == CswEnumNbtViewPropOwnerType.First && Int32.MinValue != relationship.FirstId && false == overrideFirst )
            {
                type = relationship.FirstType;
                Id = relationship.FirstId;
            }
            else
            {
                type = relationship.SecondType;
                Id = relationship.SecondId;
            }

            if( type.Equals( CswEnumNbtViewRelatedIdType.NodeTypeId ) )
            {
                CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Id );
                if( null != NodeType )
                {
                    Collection<CswNbtViewProperty> props = _getProps( NodeType, TempView, seenProps, relationship, DoCheck );

                    foreach( CswNbtViewProperty vp in props )
                    {
                        if( UseMetaName )
                        {
                            vp.TextLabel = NodeType.NodeTypeName + "'s " + vp.MetaDataProp.PropName;
                        }
                        if( false == DoCheck && false == seenProps.Contains( vp.TextLabel ) || DoCheck )
                        {
                            seenProps.Add( vp.TextLabel );
                            Props.Add( vp );
                        }
                    }
                }
            }
            else if( type.Equals( CswEnumNbtViewRelatedIdType.ObjectClassId ) )
            {
                CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( Id );
                if( null != ObjClass )
                {
                    Collection<CswNbtViewProperty> props = _getProps( ObjClass, TempView, seenProps, relationship, DoCheck );

                    foreach( CswNbtViewProperty vp in props )
                    {
                        if( UseMetaName )
                        {
                            vp.TextLabel = ObjClass.ObjectClass.Value + "'s " + vp.MetaDataProp.PropName;
                        }
                        Props.Add( vp );
                    }
                }
            }
            else if( type.Equals( CswEnumNbtViewRelatedIdType.PropertySetId ) )
            {
                CswNbtMetaDataPropertySet PropSet = _CswNbtResources.MetaData.getPropertySet( Id );
                if( null != PropSet )
                {
                    foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                    {
                        Collection<CswNbtViewProperty> props = _getProps( ObjClass, TempView, seenProps, relationship, DoCheck );

                        foreach( CswNbtViewProperty vp in props )
                        {
                            if( UseMetaName )
                            {
                                vp.TextLabel = ObjClass.ObjectClass.Value + "'s " + vp.MetaDataProp.PropName;
                            }
                            Props.Add( vp );
                        }
                    }
                }
            }
        }

        protected Collection<CswNbtViewProperty> _getProps( CswNbtMetaDataObjectClass ObjClass, CswNbtView TempView, HashSet<string> seenProps, CswNbtViewRelationship Relationship, bool DoCheck = true )
        {
            Collection<CswNbtViewProperty> Props = new Collection<CswNbtViewProperty>();
            if( null != ObjClass )
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                {
                    foreach( CswNbtViewProperty prop in _getProps( NodeType, TempView, seenProps, Relationship, DoCheck ) )
                    {
                        Props.Add( prop );
                    }
                }
            }
            return Props;
        }

        protected Collection<CswNbtViewProperty> _getProps( CswNbtMetaDataNodeType NodeType, CswNbtView TempView, HashSet<string> seenProps, CswNbtViewRelationship Relationship, bool DoCheck = true )
        {
            Collection<CswNbtViewProperty> Props = new Collection<CswNbtViewProperty>();
            foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
            {
                if( ntp.getFieldTypeRule().SearchAllowed ||
                    ntp.getFieldTypeValue() == CswEnumNbtFieldType.Button && false == ntp.IsSaveProp )
                {
                    CswNbtViewProperty viewProp = TempView.AddViewProperty( Relationship, ntp );
                    if( false == DoCheck )
                    {
                        Props.Add( viewProp );
                    }
                    else if( false == seenProps.Contains( viewProp.TextLabel ) )
                    {
                        seenProps.Add( viewProp.TextLabel );
                        Props.Add( viewProp );
                    }
                }
            }
            return Props;
        }

        protected void _getFilterProps( CswNbtViewEditorData Return )
        {
            string viewStr = CurrentView.ToString();
            CswNbtView TempView = new CswNbtView( _CswNbtResources );
            TempView.LoadXml( viewStr );
            HashSet<string> seenProps = new HashSet<string>();

            CswNbtViewRelationship Relationship = (CswNbtViewRelationship) TempView.FindViewNodeByArbitraryId( Request.Relationship.ArbitraryId );
            if( null != Relationship )
            {
                foreach( CswNbtViewProperty viewProp in Relationship.Properties )
                {
                    seenProps.Add( viewProp.TextLabel );
                    Return.Step4.Properties.Add( viewProp );
                }

                if( Relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.PropertySetId ) )
                {
                    CswNbtMetaDataPropertySet PropSet = _CswNbtResources.MetaData.getPropertySet( Relationship.SecondId );
                    if( null != PropSet )
                    {
                        foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                        {
                            Collection<CswNbtViewProperty> props = _getProps( ObjClass, TempView, seenProps, Relationship );
                            foreach( CswNbtViewProperty vp in props )
                            {
                                Return.Step4.Properties.Add( vp );
                            }
                        }
                    }
                }
                else if( Relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.ObjectClassId ) )
                {
                    CswNbtMetaDataObjectClass ObjClass = _CswNbtResources.MetaData.getObjectClass( Relationship.SecondId );
                    if( null != ObjClass )
                    {
                        Collection<CswNbtViewProperty> props = _getProps( ObjClass, TempView, seenProps, Relationship );
                        foreach( CswNbtViewProperty vp in props )
                        {
                            Return.Step4.Properties.Add( vp );
                        }
                    }
                }
                else if( Relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.NodeTypeId ) )
                {
                    CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Relationship.SecondId );
                    if( null != NodeType )
                    {
                        Collection<CswNbtViewProperty> props = _getProps( NodeType, TempView, seenProps, Relationship );
                        foreach( CswNbtViewProperty vp in props )
                        {
                            Return.Step4.Properties.Add( vp );
                        }
                    }
                }
            }

            Return.Step4.ViewJson = TempView.ToJson().ToString();
        }

        #region Get Related

        protected Collection<CswNbtViewRelationship> getViewChildRelationshipOptions( CswNbtView View, string ArbitraryId )
        {
            Collection<CswNbtViewRelationship> ret = new Collection<CswNbtViewRelationship>();

            if( View.ViewId != null )
            {
                CswNbtViewNode SelectedViewNode = View.FindViewNodeByArbitraryId( ArbitraryId );
                if( View.ViewMode != CswEnumNbtViewRenderingMode.Unknown || View.Root.ChildRelationships.Count == 0 )
                {
                    if( SelectedViewNode is CswNbtViewRelationship )
                    {
                        CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;
                        Int32 CurrentLevel = 0;
                        CswNbtViewNode Parent = CurrentRelationship;
                        while( !( Parent is CswNbtViewRoot ) )
                        {
                            CurrentLevel++;
                            Parent = Parent.Parent;
                        }

                        // Child options are all relations to this nodetype
                        Int32 CurrentId = CurrentRelationship.SecondId;

                        Collection<CswNbtViewRelationship> Relationships = null;
                        if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                        {
                            Relationships = getPropertySetRelated( CurrentId, View, CurrentLevel );
                        }
                        else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                        {
                            Relationships = getObjectClassRelated( CurrentId, View, CurrentLevel );
                        }
                        else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                        {
                            Relationships = getNodeTypeRelated( CurrentId, View, CurrentLevel );
                        }

                        foreach( CswNbtViewRelationship R in from CswNbtViewRelationship _R in Relationships orderby _R.SecondName select _R )
                        {
                            R.Parent = CurrentRelationship;

                            string Label = String.Empty;
                            if( R.PropOwner == CswEnumNbtViewPropOwnerType.First )
                            {
                                Label = R.SecondName + " (by " + R.PropName + ")";
                            }
                            else if( R.PropOwner == CswEnumNbtViewPropOwnerType.Second )
                            {
                                Label = R.SecondName + " (by " + R.SecondName + "'s " + R.PropName + ")";
                            }
                            R.TextLabel = Label;

                            bool contains = ret.Any( existingRel => existingRel.TextLabel == R.TextLabel );  //no dupes
                            if( false == contains )
                            {
                                ret.Add( R );
                            }
                        } // foreach( CswNbtViewRelationship R in Relationships )

                    }
                }
            }
            return ret;
        } // getViewChildOptions()

        private Collection<CswNbtViewRelationship> getNodeTypeRelated( Int32 FirstVersionId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataNodeType FirstVersionNodeType = _CswNbtResources.MetaData.getNodeType( FirstVersionId );
            CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.getObjectClass();
            CswNbtMetaDataPropertySet PropertySet = ObjectClass.getPropertySet();

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForNodeTypeId_select", "getRelationsForNodeTypeId" );
            RelationshipPropsSelect.S4Parameters.Add( "getnodetypeid", new CswStaticParam( "getnodetypeid", FirstVersionNodeType.NodeTypeId ) );
            //RelationshipPropsQueryCaddy.S4Parameters.Add("getroleid", _CswNbtResources.CurrentUser.RoleId);
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                    PropRow["fkvalue"].ToString() != String.Empty )
                {
                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );

                    if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                          PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                          PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
                    {
                        if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, FirstVersionNodeType ) )
                        {
                            // Special case -- relationship to my own type
                            // We need to create two relationships from this

                            CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            _InsertRelationship( Relationships, R1 );

                            if( !Restrict )
                            {
                                CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                _InsertRelationship( Relationships, R2 );
                            }
                        }
                    }
                    else if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                               PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                             ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                               PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                    {
                        // Special case -- relationship to my own class
                        // We need to create two relationships from this

                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( FirstVersionNodeType );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( FirstVersionNodeType );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                            PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            //if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else
                            //    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            if( R.SecondType != CswEnumNbtViewRelatedIdType.NodeTypeId ||
                                _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                            {
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else if( ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                                   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() && PropertySet != null &&
                                   PropRow["fkvalue"].ToString() == PropertySet.PropertySetId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me or my object class
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }

                                if( R.SecondType != CswEnumNbtViewRelatedIdType.NodeTypeId ||
                                    _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                                {
                                    _InsertRelationship( Relationships, R );
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getNodeTypeRelated() found a relationship which did not match the original nodetypeid" );
                        }
                        if( R != null )
                            R.overrideFirst( FirstVersionNodeType );

                    }
                }
            }

            return Relationships;
        }

        private Collection<CswNbtViewRelationship> getObjectClassRelated( Int32 ObjectClassId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            CswNbtMetaDataPropertySet PropertySet = ObjectClass.getPropertySet();

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
            RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", new CswStaticParam( "getobjectclassid", ObjectClassId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() &&
                          PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                          PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
                    {
                        CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

                        // Special case -- relationship to my own class
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( ObjectClass );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( ObjectClass );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() &&
                            PropRow["typeid"].ToString() == ObjectClassId.ToString() )
                        {
                            // my relation to something else
                            CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            //if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.PropertySetId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getPropertySet( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            R.overrideFirst( ObjectClass );
                            _InsertRelationship( Relationships, R );
                        }
                        else if( ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() && PropertySet != null &&
                                   PropRow["fkvalue"].ToString() == PropertySet.PropertySetId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                ICswNbtMetaDataProp ThisProp = null;
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                }
                                else
                                {
                                    ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                }
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( ObjectClass );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getObjectClassRelated() found a relationship which did not match the original objectclassid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private Collection<CswNbtViewRelationship> getPropertySetRelated( Int32 PropertySetId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( PropertySetId );

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForPropertySetId_select", "getRelationsForPropertySetId" );
            RelationshipPropsSelect.S4Parameters.Add( "getpropertysetid", new CswStaticParam( "getpropertysetid", PropertySetId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    ICswNbtMetaDataProp ThisProp = null;
                    if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                    {
                        ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                    }
                    else if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() )
                    {
                        ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                    }

                    if( PropRow["propertysetid"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                        PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                        PropRow["fkvalue"].ToString() == PropertySetId.ToString() )
                    {
                        // Special case -- relationship to my own set
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( PropertySet );
                        R1.overrideSecond( PropertySet );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( PropertySet );
                            R2.overrideSecond( PropertySet );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["propertysetid"].ToString() == PropertySetId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            R.overrideFirst( PropertySet );
                            _InsertRelationship( Relationships, R );
                        }
                        else if( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                                 PropRow["fkvalue"].ToString() == PropertySetId.ToString() )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( PropertySet );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getPropertySetRelated() found a relationship which did not match the original propertysetid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private void _InsertRelationship( Collection<CswNbtViewRelationship> Relationships, CswNbtViewRelationship AddMe )
        {
            Int32 InsertAt = Relationships.Count;
            for( Int32 i = 0; i < Relationships.Count; i++ )
            {
                if( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) > 0 ||
                    ( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) == 0 &&
                      Relationships[i].PropName.CompareTo( AddMe.PropName ) >= 0 ) )
                {
                    InsertAt = i;
                    break;
                }
            }
            Relationships.Insert( InsertAt, AddMe );
        } // _InsertRelationship

        private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }

        private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in ThisVersionNodeType.getNodeTypeProps() orderby _ThisProp.PropName select _ThisProp )
                {
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.getPriorVersionNodeType();
            }
            return PropsByName.Values;
        }

        private ICollection _getPropertySetPropsCollection( Int32 PropertySetId )
        {
            // Need to generate all properties on all nodetypes of all object classes of this propertyset
            SortedList AllProps = new SortedList();
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClassesByPropertySetId( PropertySetId ) )
            {
                foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
                {
                    ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                    {
                        string ThisKey = NodeTypeProp.PropName.ToLower();
                        if( !AllProps.ContainsKey( ThisKey ) )
                        {
                            AllProps.Add( ThisKey, NodeTypeProp );
                        }
                    }
                }
            }
            return AllProps.Values;
        }

        #endregion

        #endregion
    }


    #region Data Contracts

    public class CswNbtViewEditorData
    {
        [DataMember]
        public string ViewId = string.Empty;

        [DataMember]
        public string StepName = string.Empty;

        [DataMember]
        public string Action = string.Empty;

        [DataMember]
        public string Preview = string.Empty;

        [DataMember]
        public CswNbtView CurrentView = null;

        [DataMember]
        public CswNbtViewRelationship Relationship;

        public CswPrimaryKey NodeId = null;
        [DataMember]
        public string CurrentNodeId
        {
            get
            {
                string ret = "";
                if( null != NodeId )
                {
                    ret = NodeId.ToString();
                }
                return ret;
            }
            set
            {
                NodeId = CswConvert.ToPrimaryKey( value );
            }
        }

        [DataMember]
        public CswNbtViewProperty Property;

        [DataMember]
        public CswNbtViewEditorStep2 Step2 = new CswNbtViewEditorStep2();

        [DataMember]
        public CswNbtViewEditorStep3 Step3 = new CswNbtViewEditorStep3();

        [DataMember]
        public CswNbtViewEditorStep4 Step4 = new CswNbtViewEditorStep4();

        [DataMember]
        public CswNbtViewEditorStep6 Step6 = new CswNbtViewEditorStep6();


        //Filters
        [DataMember]
        public CswNbtViewPropertyFilter FilterToRemove;

        [DataMember]
        public string ArbitraryId = string.Empty;

        [DataMember]
        public string FilterConjunction = string.Empty;
        [DataMember]
        public string FilterMode = string.Empty;
        [DataMember]
        public string FilterValue = string.Empty;
        [DataMember]
        public string FilterSubfield = string.Empty;
        [DataMember]
        public string PropArbId = string.Empty;

        //Attributes
        [DataMember]
        public string NewViewName = string.Empty;
        [DataMember]
        public string NewViewCategory = string.Empty;
        [DataMember]
        public string NewViewVisibility = string.Empty;
        [DataMember]
        public string NewVisibilityRoleId = string.Empty;
        [DataMember]
        public string NewVisbilityUserId = string.Empty;
        [DataMember]
        public int NewViewWidth = Int32.MinValue;
    }

    [DataContract]
    public class CswNbtViewEditorStep2
    {
        [DataMember]
        public Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();
    }

    [DataContract]
    public class CswNbtViewEditorStep3
    {
        [DataMember]
        public Collection<CswNbtViewProperty> Properties = new Collection<CswNbtViewProperty>();

        [DataMember]
        public Collection<CswNbtViewRelationship> SecondRelationships = new Collection<CswNbtViewRelationship>();
    }

    [DataContract]
    public class CswNbtViewEditorStep4
    {
        [DataMember]
        public string ViewJson = string.Empty;

        [DataMember]
        public Collection<CswNbtViewPropertyFilter> Filters = new Collection<CswNbtViewPropertyFilter>();

        [DataMember]
        public Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

        [DataMember]
        public Collection<CswNbtViewProperty> Properties = new Collection<CswNbtViewProperty>();
    }

    [DataContract]
    public class CswNbtViewEditorStep6
    {
        [DataMember]
        public CswNbtViewPropertyFilter FilterNode;
        [DataMember]
        public CswNbtViewRelationship RelationshipNode;

        [DataMember]
        public CswNbtViewProperty PropertyNode;

        [DataMember]
        public Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

        [DataMember]
        public Collection<CswNbtViewProperty> Properties = new Collection<CswNbtViewProperty>();
    }

    public class CswNbtViewEditorAttributeData
    {
        [DataMember]
        public string NewViewName = string.Empty;
        [DataMember]
        public string NewViewCategory = string.Empty;
        [DataMember]
        public string NewViewVisibility = string.Empty;
        [DataMember]
        public string NewVisibilityRoleId = string.Empty;
        [DataMember]
        public string NewVisbilityUserId = string.Empty;
        [DataMember]
        public int NewViewWidth = Int32.MinValue;
        [DataMember]
        public CswNbtView CurrentView;
    }

    #endregion
}
