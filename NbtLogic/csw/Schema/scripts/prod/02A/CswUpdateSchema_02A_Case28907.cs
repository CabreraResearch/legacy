﻿using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28907
    /// </summary>
    public class CswUpdateSchema_02A_Case28907 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28907; }
        }

        public override void update()
        {
            _CswNbtSchemaModTrnsctn.createAction( CswNbtActionName.Material_Approval, false, String.Empty, "Materials" );
            _CswNbtSchemaModTrnsctn.createModuleActionJunction( CswEnumNbtModuleName.Containers, CswNbtActionName.Material_Approval );

            CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass );
            foreach( CswNbtNode RoleNode in RoleOC.getNodes( false, true ) )
            {
                bool CanApprove = ( RoleNode.NodeName == "Administrator" || RoleNode.NodeName == "chemsw_admin_role" ||
                                    RoleNode.NodeName == "CISPro_Receiver" || RoleNode.NodeName == "CISPro_Admin" );
                _CswNbtSchemaModTrnsctn.Permit.set( CswNbtActionName.Material_Approval, RoleNode, CanApprove );
            }

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp MaterialIdProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.MaterialId );
            CswNbtMetaDataObjectClassProp TradeNameProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Tradename );
            CswNbtMetaDataObjectClassProp SupplierProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.Supplier );
            CswNbtMetaDataObjectClassProp PartNoProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.PartNumber );
            CswNbtMetaDataObjectClassProp CASNoProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.CasNo );
            CswNbtMetaDataObjectClassProp PhysicalStateProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.PhysicalState );
            CswNbtMetaDataObjectClassProp ApprovedForReceivingProp = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.ApprovedForReceiving );

            CswNbtView UnapprovedMaterialsView = _CswNbtSchemaModTrnsctn.restoreView( "Unapproved Materials", NbtViewVisibility.Global );
            if( null == UnapprovedMaterialsView )
            {
                UnapprovedMaterialsView = _CswNbtSchemaModTrnsctn.makeNewView( "Unapproved Materials", NbtViewVisibility.Global );
            }
            else
            {
                UnapprovedMaterialsView.Root.ChildRelationships.Clear();
            }
            UnapprovedMaterialsView.ViewMode = NbtViewRenderingMode.Grid;
            UnapprovedMaterialsView.Category = "Materials";

            CswNbtViewRelationship MatRel = UnapprovedMaterialsView.AddViewRelationship( MaterialOC, true );
            CswNbtViewProperty MaterialIdVP = UnapprovedMaterialsView.AddViewProperty( MatRel, MaterialIdProp );
            MaterialIdVP.Order = 1;
            CswNbtViewProperty TradeNameVP = UnapprovedMaterialsView.AddViewProperty( MatRel, TradeNameProp );
            TradeNameVP.Order = 2;
            CswNbtViewProperty SupplierVP = UnapprovedMaterialsView.AddViewProperty( MatRel, SupplierProp );
            SupplierVP.Order = 3;
            CswNbtViewProperty PartNoVP = UnapprovedMaterialsView.AddViewProperty( MatRel, PartNoProp );
            PartNoVP.Order = 4;
            CswNbtViewProperty CASNoVP = UnapprovedMaterialsView.AddViewProperty( MatRel, CASNoProp );
            CASNoVP.Order = 5;
            CswNbtViewProperty PhysicalStateVP = UnapprovedMaterialsView.AddViewProperty( MatRel, PhysicalStateProp );
            PhysicalStateVP.Order = 6;
            UnapprovedMaterialsView.AddViewPropertyAndFilter( MatRel, ApprovedForReceivingProp, Tristate.False.ToString(),
                                                                FilterMode: CswEnumNbtFilterMode.Equals,
                                                                ShowInGrid: false );
            UnapprovedMaterialsView.save();
        } // update()
    }//class CswUpdateSchema_02A_Case28907
}//namespace ChemSW.Nbt.Schema