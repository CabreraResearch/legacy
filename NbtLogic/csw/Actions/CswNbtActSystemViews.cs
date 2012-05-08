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

                CswNbtViewProperty DueDateVp = Ret.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, CswNbtObjClassInspectionDesign.DatePropertyName );
                DueDateVp.SortBy = true;

                CswNbtViewProperty LocationVp = Ret.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, CswNbtObjClassInspectionDesign.LocationPropertyName );
                LocationVp.SortBy = true;

                Ret.AddViewPropertyByName( InspectionDesignVr, InspectionDesignOc, "Barcode" );

                CswNbtMetaDataObjectClassProp StatusOcp = InspectionDesignOc.getObjectClassProp( CswNbtObjClassInspectionDesign.StatusPropertyName );
                CswNbtViewProperty StatusVp = Ret.AddViewProperty( InspectionDesignVr, StatusOcp );
                string Completed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed );
                string Cancelled = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Cancelled );
                string CompletedLate = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Completed_Late );
                string Missed = CswNbtObjClassInspectionDesign.InspectionStatusAsString( CswNbtObjClassInspectionDesign.InspectionStatus.Missed );

                Ret.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Completed, false );
                Ret.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Cancelled, false );
                Ret.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, CompletedLate, false );
                Ret.AddViewPropertyFilter( StatusVp, StatusOcp.getFieldTypeRule().SubFields.Default.Name, CswNbtPropFilterSql.PropertyFilterMode.NotEquals, Missed, false );

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
                CswNbtMetaDataObjectClassProp NameOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.NamePropertyName );
                CswNbtViewProperty NameVp = Ret.AddViewProperty( LocationVr, NameOcp );
                NameVp.SortBy = true;
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
                CswNbtMetaDataObjectClassProp NameOcp = LocationOc.getObjectClassProp( CswNbtObjClassLocation.NamePropertyName );
                CswNbtViewProperty NameVp = Ret.AddViewProperty( LocationVr, NameOcp );
                NameVp.SortBy = true;
                Ret.save();
            }
            return Ret;
        }

        private CswNbtMetaDataObjectClass _EnforceObjectClassRelationship = null;

        #endregion Private, core methods

        #region Constructor

        public CswNbtActSystemViews( CswNbtResources CswNbtResources, SystemViewName ViewName, CswNbtMetaDataObjectClass EnforceObjectClassRelationship )
        {
            _CswNbtResources = CswNbtResources;
            _EnforceObjectClassRelationship = EnforceObjectClassRelationship;

            if( ViewName == SystemViewName.SILocationsList )
            {
                SystemView = _siLocationsListView();
            }
            else if( ViewName == SystemViewName.SILocationsTree )
            {
                SystemView = _siLocationsTreeView();
            }
            else if( ViewName != SystemViewName.Unknown )
            {
                SystemView = _getSiInspectionBaseView( ViewName );
            }
        }

        #endregion Constructor

        #region Public methods

        public class SystemViewPropFilterDefinition
        {
            public CswNbtMetaDataObjectClassProp ObjectClassProp { get; set; }
            public string FilterValue { get; set; }
            public CswNbtPropFilterSql.PropertyFilterMode FilterMode { get; set; }
        }

        public bool AddSystemViewFilter( SystemViewPropFilterDefinition FilterDefinition )
        {
            bool Ret = false;
            foreach( CswNbtViewRelationship PotentialInspectionDesignRelationship in SystemView.Root.ChildRelationships )
            {
                if( PotentialInspectionDesignRelationship.isExpectedMetaDataType( _EnforceObjectClassRelationship ) )
                {
                    Ret = true;
                    SystemView.AddViewPropertyAndFilter( PotentialInspectionDesignRelationship, FilterDefinition.ObjectClassProp, FilterDefinition.FilterValue, FilterMode: FilterDefinition.FilterMode );
                }
                if( PotentialInspectionDesignRelationship.ChildRelationships.Count > 0 )
                {
                    Ret = Ret || AddSystemViewFilter( FilterDefinition );
                }
            }
            return Ret;
        }

        #endregion Public methods




    }


}
