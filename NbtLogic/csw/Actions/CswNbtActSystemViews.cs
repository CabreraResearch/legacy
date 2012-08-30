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
            public static readonly SystemViewName CISProRequestCart = new SystemViewName( "CISPro Request Cart" );
            public static readonly SystemViewName CISProRequestHistory = new SystemViewName( "CISPro Request History" );
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

        private CswNbtView _getSiInspectionBaseView( SystemViewName ViewName, bool ReInit )
        {
            CswNbtView Ret = _getSystemView( ViewName );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( ViewName.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
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

        private CswNbtView _getSiInspectionUserView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SIInspectionsbyUser );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SIInspectionsbyUser.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                CswNbtViewRelationship InspectionDesignVr = Ret.AddViewRelationship( InspectionDesignOc, true );

                _addDefaultInspectionDesignViewPropsAndFilters( Ret, InspectionDesignVr, InspectionDesignOc );

                CswNbtMetaDataObjectClassProp InspectorOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.InspectorPropertyName );
                Ret.AddViewPropertyAndFilter( InspectionDesignVr, InspectorOcp, "me" );

                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _getSiInspectionBarcodeView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SIInspectionsbyBarcode );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SIInspectionsbyBarcode.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                CswNbtMetaDataObjectClassProp InspectionTargetLocationOcp = InspectionTargetOc.getObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );

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

        private CswNbtView _siLocationsTreeView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( SystemViewName.SILocationsTree );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SILocationsTree.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.Tree;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
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
            CswNbtView Ret = _getSystemView( SystemViewName.SILocationsList );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode =
                    _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.SILocationsList.ToString(), NbtViewVisibility.Role,
                            ChemSwAdminRoleNode.NodeId );
                Ret.Category = SiViewCategory;
                Ret.ViewMode = NbtViewRenderingMode.List;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                CswNbtViewRelationship LocationVr = Ret.AddViewRelationship( LocationOc, true );
                CswNbtMetaDataObjectClassProp LocationLocationOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Location );
                CswNbtViewProperty LocationLocationVp = Ret.AddViewProperty( LocationVr, LocationLocationOcp );
                LocationLocationVp.SortBy = true;
                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _cisproRequestCartView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( SystemViewName.CISProRequestCart );
            if( null == Ret )
            {
                Ret = new CswNbtView( _CswNbtResources );
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret.makeNew( SystemViewName.CISProRequestCart.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Category = "Request Configuration";
                Ret.ViewMode = NbtViewRenderingMode.Grid;

                Ret.Root.ChildRelationships.Clear();

                CswNbtMetaDataObjectClass RequestItemOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestItemClass );
                CswNbtViewRelationship RequestItemVr = Ret.AddViewRelationship( RequestItemOc, true );

                CswNbtMetaDataObjectClassProp NumberOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Number );
                CswNbtMetaDataObjectClassProp TypeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                CswNbtMetaDataObjectClassProp QuantityOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Quantity );
                CswNbtMetaDataObjectClassProp CountOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Count );
                CswNbtMetaDataObjectClassProp SizeOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Size );
                CswNbtMetaDataObjectClassProp MaterialOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Material );
                CswNbtMetaDataObjectClassProp ContainerOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Container );
                CswNbtMetaDataObjectClassProp LocationOcp = RequestItemOc.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location );

                Ret.AddViewProperty( RequestItemVr, NumberOcp );
                Ret.AddViewProperty( RequestItemVr, TypeOcp );
                Ret.AddViewProperty( RequestItemVr, QuantityOcp );
                Ret.AddViewProperty( RequestItemVr, CountOcp );
                Ret.AddViewProperty( RequestItemVr, SizeOcp );
                Ret.AddViewProperty( RequestItemVr, MaterialOcp );
                Ret.AddViewProperty( RequestItemVr, ContainerOcp );
                Ret.AddViewProperty( RequestItemVr, LocationOcp );

                Ret.save();
            }
            return Ret;
        }

        private CswNbtView _cisproRequestHistoryView( bool ReInit )
        {
            CswNbtView Ret = _getSystemView( SystemViewName.CISProRequestHistory );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( SystemViewName.CISProRequestHistory.ToString(), NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = "Request Configuration";
                Ret.ViewMode = NbtViewRenderingMode.Tree;
                ReInit = true;
            }
            if( ReInit )
            {
                Ret.Root.ChildRelationships.Clear();
                CswNbtMetaDataObjectClass RequestOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
                CswNbtMetaDataObjectClassProp SubmittedDateOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString() );
                CswNbtMetaDataObjectClassProp NameOcp = RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name.ToString() );
                CswNbtViewRelationship RequestVr = Ret.AddViewRelationship( RequestOc, true ); //default filter says Requestor == me
                Ret.AddViewProperty( RequestVr, SubmittedDateOcp );
                Ret.AddViewPropertyAndFilter( RequestVr, NameOcp, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotNull );

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

            if( ViewName == SystemViewName.CISProRequestCart )
            {
                RetView = _cisproRequestCartView( ReInit );
            }
            else if( ViewName == SystemViewName.CISProRequestHistory )
            {
                RetView = _cisproRequestHistoryView( ReInit );
            }
            else if( ViewName == SystemViewName.SILocationsList )
            {
                RetView = _siLocationsListView( ReInit );
            }
            else if( ViewName == SystemViewName.SILocationsTree )
            {
                RetView = _siLocationsTreeView( ReInit );
            }
            else if( ViewName == SystemViewName.SIInspectionsbyUser )
            {
                RetView = _getSiInspectionUserView( ReInit );
            }
            else if( ViewName == SystemViewName.SIInspectionsbyBarcode )
            {
                RetView = _getSiInspectionBarcodeView( ReInit );
            }
            else if( ViewName != SystemViewName.Unknown )
            {
                RetView = _getSiInspectionBaseView( ViewName, ReInit );
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
            public CswNbtSubField.SubFieldName SubFieldName { get; set; }
            private bool _ShowInGrid = true;
            public bool ShowInGrid { get { return _ShowInGrid; } set { _ShowInGrid = value; } }
            public CswNbtMetaDataFieldType.NbtFieldType FieldType { get; set; }
        }

        public SystemViewPropFilterDefinition makeSystemViewFilter( CswNbtMetaDataObjectClassProp ObjectClassProp, string FilterValue, CswNbtPropFilterSql.PropertyFilterMode FilterMode, CswNbtSubField.SubFieldName SubFieldName = null, CswNbtMetaDataFieldType.NbtFieldType FieldType = null, bool ShowInGrid = true )
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

        private bool _addSystemViewFilterRecursive( IEnumerable<CswNbtViewRelationship> Relationships, SystemViewPropFilterDefinition FilterDefinition, CswNbtMetaDataObjectClass MatchObjectClass = null )
        {
            bool Ret = false;
            CswNbtMetaDataObjectClass ExpectedObjectClass = MatchObjectClass ?? _EnforceObjectClassRelationship;
            foreach( CswNbtViewRelationship PotentialSystemViewRelationship in Relationships )
            {
                if( null == ExpectedObjectClass || PotentialSystemViewRelationship.isExpectedMetaDataType( ExpectedObjectClass ) )
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
                    else if( FilterDefinition.FieldType == CswNbtMetaDataFieldType.NbtFieldType.Barcode )
                    {
                        ICswNbtMetaDataObject Object = PotentialSystemViewRelationship.SecondMetaDataObject();
                        SystemView.AddViewPropertyByFieldType( PotentialSystemViewRelationship, Object, FilterDefinition.FieldType );
                    }
                }
                if( PotentialSystemViewRelationship.ChildRelationships.Count > 0 )
                {
                    Ret = Ret || _addSystemViewFilterRecursive( PotentialSystemViewRelationship.ChildRelationships, FilterDefinition, MatchObjectClass );
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
