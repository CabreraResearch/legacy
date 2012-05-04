using System.Collections.Generic;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActSystemViews
    {
        CswNbtResources _CswNbtResources = null;

        public CswNbtActSystemViews( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        private CswNbtView _getSystemView( string ViewName )
        {
            List<CswNbtView> Views = _CswNbtResources.ViewSelect.restoreViews( ViewName );
            CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
            return Views.FirstOrDefault( View => View.Visibility == NbtViewVisibility.Role &&
                View.VisibilityRoleId == ChemSwAdminRoleNode.NodeId );
        }

        private CswNbtView _getSiInspectionBaseView( string ViewName )
        {
            CswNbtView Ret = _getSystemView( ViewName );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( ViewName, NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = "SI Configuration";
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

        public CswNbtView SiLocationsTreeView()
        {
            CswNbtView Ret = _getSystemView( "SI Location Tree" );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( "SI Locations Tree", NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = "SI Configuration";
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

        public CswNbtView SiLocationsListView()
        {
            CswNbtView Ret = _getSystemView( "SI Location List" );
            if( null == Ret )
            {
                CswNbtNode ChemSwAdminRoleNode = _CswNbtResources.Nodes.makeRoleNodeFromRoleName( CswNbtObjClassRole.ChemSWAdminRoleName );
                Ret = new CswNbtView( _CswNbtResources );
                Ret.makeNew( "SI Locations List", NbtViewVisibility.Role, ChemSwAdminRoleNode.NodeId );
                Ret.Category = "SI Configuration";
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

        public CswNbtView SiInspectionsByDateView()
        {
            return _getSiInspectionBaseView( "SI Inspections by Date" );
        }

        public CswNbtView SiInspectionsByUserView()
        {
            return _getSiInspectionBaseView( "SI Inspections by User" );
        }

        public CswNbtView SiInspectionsByBarcodeView()
        {
            return _getSiInspectionBaseView( "SI Inspections by Barcode" );
        }

        public CswNbtView SiInspectionsByLocationView()
        {
            return _getSiInspectionBaseView( "SI Inspections by Location" );
        }

    }


}
