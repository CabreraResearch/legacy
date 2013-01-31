using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;
using ChemSW.Grid.ExtJs;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceView
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public JArray getAllViewNames()
        {
            JArray Ret = new JArray();

            bool IsAdmin = _CswNbtResources.CurrentNbtUser.IsAdministrator();
            Dictionary<CswNbtViewId, CswNbtView> AllViews = null;

            if( IsAdmin )
            {
                _CswNbtResources.ViewSelect.getAllEnabledViews( out AllViews );
            }
            else
            {
                _CswNbtResources.ViewSelect.getUserViews( out AllViews );
            }
            if( null != AllViews )
            {
                foreach( KeyValuePair<CswNbtViewId, CswNbtView> ViewPair in AllViews )
                {
                    if( null != ViewPair.Key && ViewPair.Key.isSet() && null != ViewPair.Value )
                    {
                        Ret.Add( new JObject
                            {
                                new JProperty("id", ViewPair.Key.ToString()), 
                                new JProperty("name", ViewPair.Value.ViewName)
                            } );
                    }
                }
            }

            return Ret;
        }

        public JObject getViewGrid( bool All )
        {
            JObject ReturnVal = new JObject();
            CswNbtGrid gd = new CswNbtGrid( _CswNbtResources );
            bool IsAdmin = _CswNbtResources.CurrentNbtUser.IsAdministrator();

            DataTable ViewsTable = null;
            if( IsAdmin )
            {
                if( All )
                {
                    ViewsTable = _CswNbtResources.ViewSelect.getAllEnabledViews();
                }
                else
                {
                    //ViewsTable = (via out)
                    _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser,
                                                                         true, false, false, NbtViewRenderingMode.Any,
                                                                         out ViewsTable );

                }
            }
            else
            {
                ViewsTable = _CswNbtResources.ViewSelect.getUserViews();
            }

            if( ViewsTable != null )
            {
                ViewsTable.Columns.Add( "viewid" );      // string CswNbtViewId
                foreach( DataRow Row in ViewsTable.Rows )
                {
                    Row["viewid"] = new CswNbtViewId( CswConvert.ToInt32( Row["nodeviewid"] ) ).ToString();
                }

                if( ViewsTable.Columns.Contains( "viewxml" ) )
                    ViewsTable.Columns.Remove( "viewxml" );
                if( ViewsTable.Columns.Contains( "roleid" ) )
                    ViewsTable.Columns.Remove( "roleid" );
                if( ViewsTable.Columns.Contains( "userid" ) )
                    ViewsTable.Columns.Remove( "userid" );
                if( ViewsTable.Columns.Contains( "mssqlorder" ) )
                    ViewsTable.Columns.Remove( "mssqlorder" );

                if( !IsAdmin )
                {
                    if( ViewsTable.Columns.Contains( "visibility" ) )
                        ViewsTable.Columns.Remove( "visibility" );
                    if( ViewsTable.Columns.Contains( "username" ) )
                        ViewsTable.Columns.Remove( "username" );
                    if( ViewsTable.Columns.Contains( "rolename" ) )
                        ViewsTable.Columns.Remove( "rolename" );
                }

                CswExtJsGrid grid = gd.DataTableToGrid( ViewsTable );
                grid.getColumn( "nodeviewid" ).hidden = true;
                grid.getColumn( "viewid" ).hidden = true;

                ReturnVal = grid.ToJson();
            } // if(ViewsTable != null)

            return ReturnVal;
        } // getViewGrid()

        public JObject getViewChildOptions( string ViewJson, string ArbitraryId, Int32 StepNo )
        {
            JObject ret = new JObject();

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadJson( ViewJson );

            if( View.ViewId != null )
            {
                CswNbtViewNode SelectedViewNode = View.FindViewNodeByArbitraryId( ArbitraryId );
                if( View.ViewMode != NbtViewRenderingMode.Unknown || View.Root.ChildRelationships.Count == 0 )
                {
                    if( SelectedViewNode is CswNbtViewRelationship )
                    {
                        if( StepNo == 3 && View.ViewMode != NbtViewRenderingMode.List )
                        {
                            // Potential child relationships

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
                            if( CurrentRelationship.SecondType == NbtViewRelatedIdType.ObjectClassId )
                            {
                                Relationships = getObjectClassRelatedNodeTypesAndObjectClasses( CurrentId, View, CurrentLevel );
                            }
                            else if( CurrentRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
                            {
                                Relationships = getNodeTypeRelatedNodeTypesAndObjectClasses( CurrentId, View, CurrentLevel );
                            }

                            foreach( CswNbtViewRelationship R in from CswNbtViewRelationship _R in Relationships orderby _R.SecondName select _R )
                            {
                                if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                                {
                                    R.Parent = CurrentRelationship;
                                    string Label = String.Empty;

                                    if( R.PropOwner == NbtViewPropOwnerType.First )
                                    {
                                        Label = R.SecondName + " (by " + R.PropName + ")";
                                    }
                                    else if( R.PropOwner == NbtViewPropOwnerType.Second )
                                    {
                                        Label = R.SecondName + " (by " + R.SecondName + "'s " + R.PropName + ")";
                                    }

                                    JProperty RProp = R.ToJson( Label, true );
                                    ret.Add( RProp );

                                } //  if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                            } // foreach( CswNbtViewRelationship R in Relationships )
                        } // if( StepNo == 3)
                        else if( StepNo == 4 )
                        {
                            // Potential child properties

                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;

                            ICollection PropsCollection = null;
                            if( CurrentRelationship.SecondType == NbtViewRelatedIdType.ObjectClassId )
                            {
                                PropsCollection = _getObjectClassPropsCollection( CurrentRelationship.SecondId );
                            }
                            else if( CurrentRelationship.SecondType == NbtViewRelatedIdType.NodeTypeId )
                            {
                                PropsCollection = _getNodeTypePropsCollection( CurrentRelationship.SecondId );
                            }
                            else
                            {
                                throw new CswDniException( ErrorType.Error, "A Data Misconfiguration has occurred", "CswViewEditor.initPropDataTable() has a selected node which is neither a NodeTypeNode nor an ObjectClassNode" );
                            }

                            foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in PropsCollection orderby _ThisProp.PropNameWithQuestionNo select _ThisProp )
                            {
                                // BZs 7085, 6651, 6644, 7092
                                if( ThisProp.getFieldTypeRule().SearchAllowed ||
                                    ThisProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Button )
                                {
                                    CswNbtViewProperty ViewProp = View.AddViewProperty( null, (CswNbtMetaDataNodeTypeProp) ThisProp );
                                    if( !CurrentRelationship.Properties.Contains( ViewProp ) )
                                    {
                                        ViewProp.Parent = CurrentRelationship;

                                        string PropName = ViewProp.MetaDataProp.PropNameWithQuestionNo;
                                        if( false == ThisProp.getNodeType().IsLatestVersion() )
                                            PropName += "&nbsp;(v" + ThisProp.getNodeType().VersionNo + ")";

                                        JProperty PropJProp = ViewProp.ToJson( PropName, true );
                                        ret.Add( PropJProp );

                                    } // if( !CurrentRelationship.Properties.Contains( ViewProp ) )
                                } // if( ThisProp.FieldTypeRule.SearchAllowed )
                            } // foreach (DataRow Row in Props.Rows)
                        } // if-else(StepNo == 4)
                    } // if( SelectedViewNode is CswNbtViewRelationship )
                    else if( SelectedViewNode is CswNbtViewRoot )
                    {
                        // Set NextOptions to be all viewable nodetypes and objectclasses
                        foreach( CswNbtMetaDataNodeType LatestNodeType in from CswNbtMetaDataNodeType _LatestNodeType in _CswNbtResources.MetaData.getNodeTypesLatestVersion() orderby _LatestNodeType.NodeTypeName select _LatestNodeType )
                        {
                            if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, LatestNodeType ) )
                            {
                                // This is purposefully not the typical way of creating CswNbtViewRelationships.
                                CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, LatestNodeType.getFirstVersionNodeType(), false );
                                R.Parent = SelectedViewNode;

                                bool IsChildAlready = false;
                                foreach( CswNbtViewRelationship ChildRel in from CswNbtViewRelationship _ChildRel in ( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships orderby _ChildRel.SecondName select _ChildRel )
                                {
                                    if( ChildRel.SecondType == R.SecondType && ChildRel.SecondId == R.SecondId )
                                    {
                                        IsChildAlready = true;
                                    }
                                }

                                if( !IsChildAlready )
                                {
                                    JProperty RProp = R.ToJson( LatestNodeType.NodeTypeName, true );
                                    ret.Add( RProp );
                                }
                            }
                        }

                        foreach( CswNbtMetaDataObjectClass ObjectClass in
                            from CswNbtMetaDataObjectClass _ObjectClass
                                in _CswNbtResources.MetaData.getObjectClasses()
                            orderby _ObjectClass.ObjectClass
                            where _ObjectClass.ObjectClass != CswNbtResources.UnknownEnum
                            select _ObjectClass )
                        {
                            // This is purposefully not the typical way of creating CswNbtViewRelationships.

                            CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, ObjectClass, false );
                            R.Parent = SelectedViewNode;

                            if( !( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships.Contains( R ) )
                            {
                                JProperty RProp = R.ToJson( "Any " + ObjectClass.ObjectClass, true );
                                ret.Add( RProp );
                            }
                        }
                    } // else if( SelectedViewNode is CswNbtViewRoot )
                    else if( SelectedViewNode is CswNbtViewProperty )
                    {
                        ret.Add( new JProperty( "filters", "" ) );
                    }
                    else if( SelectedViewNode is CswNbtViewPropertyFilter )
                    {
                    }

                } // if( _View.ViewMode != NbtViewRenderingMode.List || _View.Root.ChildRelationships.Count == 0 )
            } // if( _View != null )

            return ret;
        } // getViewChildOptions()

        public JObject getRuntimeViewFilters( CswNbtView View )
        {
            JObject ret = new JObject();
            if( View != null )
            {
                // We need the property arbitrary id, so we're doing this by property, not by filter.  
                // However, we're filtering to only those properties that have filters that have ShowAtRuntime == true
                foreach( CswNbtViewProperty Property in from CswNbtViewProperty _Property
                                                          in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewProperty )
                                                        where null != _Property.MetaDataProp
                                                        orderby _Property.MetaDataProp.PropNameWithQuestionNo
                                                        select _Property )
                {
                    JProperty PropertyJson = Property.ToJson( ShowAtRuntimeOnly : true );
                    if( ( (JObject) PropertyJson.Value["filters"] ).Count > 0 )
                    {
                        // case 26166 - collapse redundant filters
                        bool foundMatch = false;
                        foreach( JProperty OtherPropertyJson in ret.Properties() )
                        {
                            if( PropertyJson.Value["name"].ToString() == OtherPropertyJson.Value["name"].ToString() &&
                                PropertyJson.Value["fieldtype"].ToString() == OtherPropertyJson.Value["fieldtype"].ToString() )
                            {
                                foundMatch = true;
                            }
                        }
                        if( false == foundMatch )
                        {
                            ret.Add( PropertyJson );
                        }
                    }
                }
            }
            return ret;
        } // getRuntimeViewFilters()

        public JObject updateRuntimeViewFilters( CswNbtView View, JObject NewFiltersJson )
        {
            foreach( JProperty NewFilterProp in NewFiltersJson.Properties() )
            {
                string FilterArbitraryId = NewFilterProp.Name;
                JObject NewFilter = (JObject) NewFilterProp.Value;
                if( NewFilter.Children().Count() > 0 )
                {
                    // case 26166 - apply to all matching properties
                    CswNbtViewPropertyFilter ViewPropFilter = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FilterArbitraryId );
                    string OrigValue = ViewPropFilter.Value;
                    
                    CswNbtViewProperty ViewParentProp = (CswNbtViewProperty) ViewPropFilter.Parent;
                    foreach( CswNbtViewPropertyFilter OtherPropFilter in View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewPropertyFilter ) )
                    {
                        CswNbtViewProperty OtherParentProp = ( (CswNbtViewProperty) OtherPropFilter.Parent );
                        if( OtherParentProp.Name == ViewParentProp.Name &&
                            OtherParentProp.FieldType == ViewParentProp.FieldType &&
                            OtherPropFilter.Value == OrigValue )
                        {
                            OtherPropFilter.Conjunction = (CswNbtPropFilterSql.PropertyFilterConjunction) NewFilter["conjunction"].ToString();
                            OtherPropFilter.FilterMode = (CswNbtPropFilterSql.PropertyFilterMode) NewFilter["filter"].ToString();
                            OtherPropFilter.SubfieldName = (CswNbtSubField.SubFieldName) NewFilter["subfieldname"].ToString();
                            OtherPropFilter.Value = NewFilter["filtervalue"].ToString();
                        }
                    }
                }
            }

            View.SaveToCache( true, true );

            JObject ret = new JObject();
            ret["newviewid"] = View.SessionViewId.ToString();
            return ret;
        }

        #region Helper Functions

        private Collection<CswNbtViewRelationship> getNodeTypeRelatedNodeTypesAndObjectClasses( Int32 FirstVersionId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid || View.ViewMode == NbtViewRenderingMode.Table ) &&
                            ( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataNodeType FirstVersionNodeType = _CswNbtResources.MetaData.getNodeType( FirstVersionId );
            CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.getObjectClass();

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

                    if( ( PropRow["proptype"].ToString() == NbtViewPropIdType.NodeTypePropId.ToString() &&
                          PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() &&
                          PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
                    {
                        if( _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, FirstVersionNodeType ) )
                        {
                            // Special case -- relationship to my own type
                            // We need to create two relationships from this

                            CswNbtViewRelationship R1 = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                            _InsertRelationship( Relationships, R1 );

                            if( !Restrict )
                            {
                                CswNbtViewRelationship R2 = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                _InsertRelationship( Relationships, R2 );
                            }
                        }
                    }
                    else if( ( PropRow["proptype"].ToString() == NbtViewPropIdType.NodeTypePropId.ToString() &&
                               PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                             ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                               PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                    {
                        // Special case -- relationship to my own class
                        // We need to create two relationships from this

                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( FirstVersionNodeType );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( FirstVersionNodeType );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == NbtViewPropIdType.NodeTypePropId.ToString() &&
                            PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                            if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                                R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            else
                                R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );

                            if( R.SecondType != NbtViewRelatedIdType.NodeTypeId ||
                                _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                            {
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else if( ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() &&
                                   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me or my object class
                                R = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() )
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                else
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );

                                if( R.SecondType != NbtViewRelatedIdType.NodeTypeId ||
                                    _CswNbtResources.Permit.canNodeType( CswNbtPermit.NodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                                {
                                    _InsertRelationship( Relationships, R );
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Error, "An unexpected data condition has occurred", "CswDataSourceNodeType.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original nodetypeid" );
                        }
                        if( R != null )
                            R.overrideFirst( FirstVersionNodeType );

                    }
                }
            }

            return Relationships;
        }

        private Collection<CswNbtViewRelationship> getObjectClassRelatedNodeTypesAndObjectClasses( Int32 ObjectClassId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == NbtViewRenderingMode.Grid || View.ViewMode == NbtViewRenderingMode.Table ) &&
                            ( View.Visibility != NbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
            RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", new CswStaticParam( "getobjectclassid", ObjectClassId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    if( ( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() &&
                          PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() &&
                          PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
                    {
                        CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

                        // Special case -- relationship to my own class
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( ObjectClass );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( ObjectClass );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() &&
                            PropRow["typeid"].ToString() == ObjectClassId.ToString() )
                        {
                            // my relation to something else
                            CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                            R = View.AddViewRelationship( null, NbtViewPropOwnerType.First, ThisProp, false );
                            if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                                R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            else
                                R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideFirst( ObjectClass );
                            _InsertRelationship( Relationships, R );
                        }
                        else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() && PropRow["fkvalue"].ToString() == ObjectClassId.ToString() )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                if( PropRow["proptype"].ToString() == NbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                    R = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                    R = View.AddViewRelationship( null, NbtViewPropOwnerType.Second, ThisProp, false );
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( ObjectClass );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( ErrorType.Error, "An unexpected data condition has occurred", "CswDataSourceObjectClass.getRelatedNodeTypesAndObjectClasses found a relationship which did not match the original objectclassid" );
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

        #endregion Helper Functions

    } // class CswNbtWebServiceView

} // namespace ChemSW.Nbt.WebServices
