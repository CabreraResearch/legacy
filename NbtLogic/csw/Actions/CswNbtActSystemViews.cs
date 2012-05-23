using System;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActSystemViews
    {
        #region Public, Definitional props

        public CswNbtView SystemView { get; private set; }
        public static readonly string SiViewCategory = "SI Configuration";
        public sealed class SystemViewName : CswEnum<SystemViewName>
        {
            private SystemViewName( String Name ) : base( Name ) { }
            public static IEnumerable<SystemViewName> all { get { return All; } }
            public static explicit operator SystemViewName( string Str )
            {
                SystemViewName Ret = Parse( Str );
                return Ret ?? Unknown;
            }
            public static readonly SystemViewName SILocationsList = new SystemViewName( "SI Locations List" );
            public static readonly SystemViewName SILocationsTree = new SystemViewName( "SI Locations Tree" );
            public static readonly SystemViewName SIInspectionsbyDate = new SystemViewName( "SI Inspections by Date" );
            public static readonly SystemViewName SIInspectionsbyBarcode = new SystemViewName( "SI Inspections by Barcode" );
            public static readonly SystemViewName SIInspectionsbyLocation = new SystemViewName( "SI Inspections by Location" );
            public static readonly SystemViewName SIInspectionsbyUser = new SystemViewName( "SI Inspections by User" );
            public static readonly SystemViewName Unknown = new SystemViewName( "Unknown" );
        }
        #endregion Public, Definitional props

        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtView _getSystemView( SystemViewName ViewName )
        {
            List<CswNbtView> Views = _CswNbtResources.ViewSelect.restoreViews( ViewName.ToString(), NbtViewVisibility.Unknown, Int32.MinValue );
            CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            return Views.FirstOrDefault( View => View.Visibility == NbtViewVisibility.Role &&
                View.VisibilityRoleId == ChemSwAdminRoleNode.NodeId );
        }

        private CswNbtView _getSiInspectionBaseView( SystemViewName ViewName )
        {
            CswNbtView Ret = _getSystemView( ViewName );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( ViewName.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;

                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( InspectionDesignOc, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                Ret.save();
            }
            return Ret;
        }

        private void _addDefaultInspectionDesignViewPropsAndFilters( CswNbtView View, CswNbtViewRelationship InspectionDesignVr, CswNbtMetaDataObjectClass InspectionDesignOc )
        {
            CswNbtViewProperty DueDateVp = View.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, CswNbtObjClassInspectionDesign.DatePropertyName );
            DueDateVp.SortBy = true;

            CswNbtViewProperty LocationVp = View.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, CswNbtObjClassInspectionDesign.LocationPropertyName );
            LocationVp.SortBy = true;

            View.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, "Barcode" );

            CswNbtMetaDataObjectClassProp StatusOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
            CswNbtViewProperty StatusVp = View.AddViewProperty( InspectionDesignVr, StatusOcp );
            string Completed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed );
            string Cancelled = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled );
            string CompletedLate = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late );
            string Missed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed );

            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Completed, false );
            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Cancelled, false );
            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CompletedLate, false );
            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Missed, false );
        }

        private CswNbtView _getSiInspectionUserView()
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SIInspectionsbyUser );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SIInspectionsbyUser.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;

                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( InspectionDesignOc, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                CswNbtMetaDataObjectClassProp InspectorOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.InspectorPropertyName );
                Ret.AddViewPropertyAndFilter( InspectionDesignVr, InspectorOcp, "me" );

                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _getSiInspectionBarcodeView()
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SIInspectionsbyBarcode );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SIInspectionsbyBarcode.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;

                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                CswNbtMetaDataObjectClassProp InspectionTargetLocationOcp = InspectionTargetOc.getObjectClassProp( CswNbtObjClassInspectionTarget.LocationPropertyName );

                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtViewRelationship LocationTargetVr = Ret.AddViewRelationship( LocationVr, NbtViewPropOwnerType.Second, InspectionTargetLocationOcp, true );

                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtMetaDataObjectClassProp InspectionDesignTargetOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.TargetPropertyName );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( LocationTargetVr, NbtViewPropOwnerType.Second, InspectionDesignTargetOcp, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                CswNbtViewRelationship TargetVr = Ret.AddViewRelationship( InspectionTargetOc, true );
                CswNbtViewRelationship TargetDesignVr = Ret.AddViewRelationship( TargetVr, NbtViewPropOwnerType.Second, InspectionDesignTargetOcp, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, TargetDesignVr, InspectionDesignOc );

                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _siLocationsTreeView()
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SILocationsTree );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SILocationsTree.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.Tree;

                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtMetaDataObjectClassProp LocationLocationOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );
                CswNbtViewProperty LocationLocationVp = Ret.AddViewProperty( LocationVr, LocationLocationOcp );
                LocationLocationVp.SortBy = true;
                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _siLocationsListView()
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SILocationsList );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SILocationsList.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;

                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtMetaDataObjectClassProp LocationLocationOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.LocationPropertyName );
                CswNbtViewProperty LocationLocationVp = Ret.AddViewProperty( LocationVr, LocationLocationOcp );
                LocationLocationVp.SortBy = true;
                Ret.save();
            }
            return Ret;
        }

        private CswNbtMetaDataObjectClass _EnforceObjectClassRelationship = null;

        #endregion Private, core methods

        #region Constructor

        private CswNbtView _initView( SystemViewName ViewName, bool ReInit )
        {
            CswNbtView RetView = null;
            CswNbtView TmpView = null;
            if( ViewName == SystemViewName.SILocationsList )
            {
                TmpView = _siLocationsListView();
                if( ReInit )
                {
                    TmpView.Delete();
                    RetView = _siLocationsListView();
                }
                else
                {
                    RetView = TmpView;
                }
            }
            else if( ViewName == SystemViewName.SILocationsTree )
            {
                TmpView = _siLocationsTreeView();
                if( ReInit )
                {
                    TmpView.Delete();
                    RetView = _siLocationsTreeView();
                }
                else
                {
                    RetView = TmpView;
                }
            }
            else if( ViewName == SystemViewName.SIInspectionsbyUser )
            {
                TmpView = _getSiInspectionUserView();
                if( ReInit )
                {
                    TmpView.Delete();
                    RetView = _getSiInspectionUserView();
                }
                else
                {
                    RetView = TmpView;
                }
            }
            else if( ViewName == SystemViewName.SIInspectionsbyBarcode )
            {
                TmpView = _getSiInspectionBarcodeView();
                if( ReInit )
                {
                    TmpView.Delete();
                    RetView = _getSiInspectionBarcodeView();
                }
                else
                {
                    RetView = TmpView;
                }
            }
            else if( ViewName != SystemViewName.Unknown )
            {
                TmpView = _getSiInspectionBaseView( ViewName );
                if( ReInit )
                {
                    TmpView.Delete();
                    RetView = _getSiInspectionBaseView( ViewName );
                }
                else
                {
                    RetView = TmpView;
                }
            }
            return RetView;
        }

        public CswNbtActSystemViews( CswNbtResources CswNbtResources, SystemViewName ViewName, CswNbtMetaDataObjectClass EnforceObjectClassRelationship )
        {
            _CswNbtResources = CswNbtResources;
            _EnforceObjectClassRelationship = EnforceObjectClassRelationship;

            SystemView = _initView( ViewName, false );
        }

        #endregion Constructor

        #region Public methods

        public class SystemViewPropFilterDefinition
        {
            public CswNbtMetaDataObjectClassProp ObjectClassProp { get; set; }
            public string FilterValue { get; set; }
            public CswNbtPropFilterSql.PropertyFilterMode FilterMode { get; set; }
        }

        public SystemViewPropFilterDefinition makeSystemViewFilter( CswNbtMetaDataObjectClassProp ObjectClassProp, string FilterValue, CswNbtPropFilterSql.PropertyFilterMode FilterMode )
        {
            return new SystemViewPropFilterDefinition
                       {
                           ObjectClassProp = ObjectClassProp,
                           FilterValue = FilterValue,
                           FilterMode = FilterMode
                       };
        }

        private bool _addSystemViewFilterRecursive( IEnumerable<CswNbtViewRelationship> Relationships, SystemViewPropFilterDefinition FilterDefinition, CswNbtMetaDataObjectClass MatchObjectClass = null )
        {
            bool Ret = false;
            CswNbtMetaDataObjectClass ExpectedObjectClass = MatchObjectClass ?? _EnforceObjectClassRelationship;
            foreach( CswNbtViewRelationship PotentialInspectionDesignRelationship in Relationships )
            {
                if( PotentialInspectionDesignRelationship.isExpectedMetaDataType( ExpectedObjectClass ) )
                {
                    Ret = true;
                    SystemView.AddViewPropertyAndFilter( PotentialInspectionDesignRelationship, FilterDefinition.ObjectClassProp, FilterDefinition.FilterValue, FilterMode: FilterDefinition.FilterMode );
                }
                if( PotentialInspectionDesignRelationship.ChildRelationships.Count > 0 )
                {
                    Ret = Ret || _addSystemViewFilterRecursive( PotentialInspectionDesignRelationship.ChildRelationships, FilterDefinition, MatchObjectClass );
                }
            }
            return Ret;
        }

        public bool addSystemViewFilter( SystemViewPropFilterDefinition FilterDefinition, CswNbtMetaDataObjectClass MatchObjectClass = null )
        {
            return _addSystemViewFilterRecursive( SystemView.Root.ChildRelationships, FilterDefinition, MatchObjectClass );
        }

        public void reInitSystemView( SystemViewName ViewName )
        {
            SystemView = _initView( ViewName, true );
        }

        #endregion Public methods




    }


}
