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

        #endregion Public, Definitional props

        #region Private, core methods

        private CswNbtResources _CswNbtResources = null;
        private CswNbtView _getSystemView( CswEnumNbtSystemViewName ViewName )
        {
            List<CswNbtView> Views = _CswNbtResources.ViewSelect.restoreViews( ViewName.ToString(), NbtViewVisibility.Unknown, Int32.MinValue );
            CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            return Views.FirstOrDefault( View => View.Visibility == NbtViewVisibility.Role &&
                View.VisibilityRoleId == ChemSwAdminRoleNode.NodeId );
        }

        private CswNbtView _getSiInspectionBaseView( CswEnumNbtSystemViewName ViewName, bool ReInit )
        {
            CswNbtView Ret = _getSystemView( ViewName );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.saveNew( ViewName.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( InspectionDesignOc, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                Ret.save();
            }
            return Ret;
        }

        private void _addDefaultInspectionDesignViewPropsAndFilters( CswNbtView View, CswNbtViewRelationship InspectionDesignVr, CswNbtMetaDataObjectClass InspectionDesignOc )
        {
            CswNbtViewProperty DueDateVp = View.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, CswNbtObjClassInspectionDesign.PropertyName.DueDate );
            DueDateVp.SortBy = true;

            CswNbtViewProperty LocationVp = View.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, CswNbtObjClassInspectionDesign.PropertyName.Location );
            LocationVp.SortBy = true;

            View.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, "Barcode" );

            CswNbtMetaDataObjectClassProp StatusOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status );
            CswNbtViewProperty StatusVp = View.AddViewProperty( InspectionDesignVr, StatusOcp );
            string Completed = CswNbtObjClassInspectionDesign.InspectionStatus.Completed;
            string Cancelled = CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled;
            string CompletedLate = CswNbtObjClassInspectionDesign.InspectionStatus.CompletedLate;
            string Missed = CswNbtObjClassInspectionDesign.InspectionStatus.Missed;

            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Completed, false );
            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Cancelled, false );
            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CompletedLate, false );
            View.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Missed, false );
        }

        private CswNbtView _getSiInspectionUserView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( CswEnumNbtSystemViewName.SIInspectionsbyUser );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.saveNew( CswEnumNbtSystemViewName.SIInspectionsbyUser.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( InspectionDesignOc, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                CswNbtMetaDataObjectClassProp InspectorOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Inspector );
                Ret.AddViewPropertyAndFilter( InspectionDesignVr, InspectorOcp, "me" );

                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _getSiInspectionBarcodeView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( CswEnumNbtSystemViewName.SIInspectionsbyBarcode );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.saveNew( CswEnumNbtSystemViewName.SIInspectionsbyBarcode.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionTargetClass );
                CswNbtMetaDataObjectClassProp InspectionTargetLocationOcp = InspectionTargetOc.getObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );

                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtViewRelationship LocationTargetVr = Ret.AddViewRelationship( LocationVr, NbtViewPropOwnerType.Second, InspectionTargetLocationOcp, true );

                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.InspectionDesignClass );
                CswNbtMetaDataObjectClassProp InspectionDesignTargetOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( LocationTargetVr, NbtViewPropOwnerType.Second, InspectionDesignTargetOcp, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                CswNbtViewRelationship TargetVr = Ret.AddViewRelationship( InspectionTargetOc, true );
                CswNbtViewRelationship TargetDesignVr = Ret.AddViewRelationship( TargetVr, NbtViewPropOwnerType.Second, InspectionDesignTargetOcp, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, TargetDesignVr, InspectionDesignOc );

                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _siLocationsTreeView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( CswEnumNbtSystemViewName.SILocationsTree );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.saveNew( CswEnumNbtSystemViewName.SILocationsTree.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.Tree;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtMetaDataObjectClassProp LocationLocationOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );
                CswNbtViewProperty LocationLocationVp = Ret.AddViewProperty( LocationVr, LocationLocationOcp );
                LocationLocationVp.SortBy = true;
                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _siLocationsListView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( CswEnumNbtSystemViewName.SILocationsList );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode =
                    _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.saveNew( CswEnumNbtSystemViewName.SILocationsList.ToString(), NbtViewVisibility.Role,
                            ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtMetaDataObjectClassProp LocationLocationOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );
                CswNbtViewProperty LocationLocationVp = Ret.AddViewProperty( LocationVr, LocationLocationOcp );
                LocationLocationVp.SortBy = true;
                Ret.save();
            }
            return Ret;
        }

        private CswNbtMetaDataObjectClass _EnforceObjectClassRelationship = null;

        #endregion Private, core methods

        #region Constructor

        private CswNbtView _initView( CswEnumNbtSystemViewName ViewName, bool ReInit )
        {
            CswNbtView RetView = null;

            if( ViewName == CswEnumNbtSystemViewName.SILocationsList )
            {
                RetView = _siLocationsListView( ReInit );
            }
            else if( ViewName == CswEnumNbtSystemViewName.SILocationsTree )
            {
                RetView = _siLocationsTreeView( ReInit );
            }
            else if( ViewName == CswEnumNbtSystemViewName.SIInspectionsbyUser )
            {
                RetView = _getSiInspectionUserView( ReInit );
            }
            else if( ViewName == CswEnumNbtSystemViewName.SIInspectionsbyBarcode )
            {
                RetView = _getSiInspectionBarcodeView( ReInit );
            }
            else if( ViewName != CswEnumNbtSystemViewName.Unknown )
            {
                RetView = _getSiInspectionBaseView( ViewName, ReInit );
            }
            return RetView;
        }

        public CswNbtActSystemViews( CswNbtResources CswNbtResources, CswEnumNbtSystemViewName ViewName, CswNbtMetaDataObjectClass EnforceObjectClassRelationship )
        {
            _CswNbtResources = CswNbtResources;
            _EnforceObjectClassRelationship = EnforceObjectClassRelationship;

            SystemView = _initView( ViewName, false );
        }

        #endregion Constructor

        #region Public methods

        public class SystemViewPropFilterDefinition
        {
            public ICswNbtMetaDataProp ObjectClassProp { get; set; }
            public string FilterValue { get; set; }
            public CswNbtPropFilterSql.PropertyFilterMode FilterMode { get; set; }
            public CswEnumNbtSubFieldName SubFieldName { get; set; }
            private bool _ShowInGrid = true;
            public bool ShowInGrid { get { return _ShowInGrid; } set { _ShowInGrid = value; } }
            public CswEnumNbtFieldType FieldType { get; set; }
        }

        public SystemViewPropFilterDefinition makeSystemViewFilter( ICswNbtMetaDataProp ObjectClassProp, string FilterValue, CswNbtPropFilterSql.PropertyFilterMode FilterMode, CswEnumNbtSubFieldName SubFieldName = null, CswEnumNbtFieldType FieldType = null, bool ShowInGrid = true )
        {
            SubFieldName = SubFieldName ?? ObjectClassProp.getFieldTypeRule().SubFields.Default.Name;
            return new SystemViewPropFilterDefinition
                       {
                           ObjectClassProp = ObjectClassProp,
                           FilterValue = FilterValue,
                           FilterMode = FilterMode,
                           SubFieldName = SubFieldName,
                           FieldType = FieldType,
                           ShowInGrid = ShowInGrid
                       };
        }

        private bool _addSystemViewFilterRecursive( IEnumerable<CswNbtViewRelationship> Relationships, SystemViewPropFilterDefinition FilterDefinition, ICswNbtMetaDataDefinitionObject MatchObj = null )
        {
            bool Ret = false;
            ICswNbtMetaDataDefinitionObject ExpectedObjectClass = MatchObj ?? _EnforceObjectClassRelationship;
            foreach( CswNbtViewRelationship PotentialSystemViewRelationship in Relationships )
            {
                if( null == ExpectedObjectClass || PotentialSystemViewRelationship.SecondMatches( MatchObj ) )
                {
                    Ret = true;
                    if( null != FilterDefinition.ObjectClassProp )
                    {
                        SystemView.AddViewPropertyAndFilter( PotentialSystemViewRelationship,
                                                            FilterDefinition.ObjectClassProp,
                                                            FilterDefinition.FilterValue,
                                                            FilterMode: FilterDefinition.FilterMode,
                                                            SubFieldName: FilterDefinition.SubFieldName,
                                                            ShowInGrid: FilterDefinition.ShowInGrid );
                    }
                    else if( FilterDefinition.FieldType == CswEnumNbtFieldType.Barcode )
                    {
                        ICswNbtMetaDataObject Object = PotentialSystemViewRelationship.SecondMetaDataDefinitionObject();
                        SystemView.AddViewPropertyByFieldType( PotentialSystemViewRelationship, Object, FilterDefinition.FieldType );
                    }
                }
                if( PotentialSystemViewRelationship.ChildRelationships.Count > 0 )
                {
                    Ret = Ret || _addSystemViewFilterRecursive( PotentialSystemViewRelationship.ChildRelationships, FilterDefinition, MatchObj );
                }
            }
            return Ret;
        }

        public bool addSystemViewFilter( SystemViewPropFilterDefinition FilterDefinition, ICswNbtMetaDataDefinitionObject MatchObj = null )
        {
            return _addSystemViewFilterRecursive( SystemView.Root.ChildRelationships, FilterDefinition, MatchObj );
        }

        public void reInitSystemView( CswEnumNbtSystemViewName ViewName )
        {
            SystemView = _initView( ViewName, true );
        }

        #endregion Public methods
    }
}
