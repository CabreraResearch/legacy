using System;
using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case30533C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30533; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override string Title
        {
            get { return "Organize Request Item NodeType Layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            foreach( CswNbtMetaDataNodeType RequestItemNT in RequestItemOC.getNodeTypes() )
            {
                //Set NewMaterialType default value
                CswNbtMetaDataNodeTypeProp NewMaterialTypeNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NewMaterialType );
                if( null != ChemicalOC.FirstNodeType && null != NewMaterialTypeNTP )
                {
                    NewMaterialTypeNTP.DefaultValue.AsNodeTypeSelect.SelectedNodeTypeIds.Add( ChemicalOC.FirstNodeType.NodeTypeId.ToString() );
                }
                CswNbtMetaDataNodeTypeProp DescriptionNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description );
                //Set RequestItem name template
                RequestItemNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( DescriptionNTP.PropName ) );
                //Hide Internal Flags and States
                CswNbtMetaDataNodeTypeProp TypeNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Type );
                TypeNTP.Hidden = true;
                CswNbtMetaDataNodeTypeProp IsFavoriteNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.IsFavorite );
                IsFavoriteNTP.Hidden = true;
                CswNbtMetaDataNodeTypeProp IsRecurringNTP = RequestItemNT.getNodeTypePropByObjectClassProp( CswNbtObjClassRequestItem.PropertyName.IsRecurring );
                IsRecurringNTP.Hidden = true;

                //Remove all props from all layouts
                foreach( CswNbtMetaDataNodeTypeProp Prop in RequestItemNT.getNodeTypeProps() )
                {
                    if( Prop.PropName != CswNbtObjClass.PropertyName.Save )
                    {
                        Prop.removeFromAllLayouts();
                    }
                }

                //Give CRUD permissions to applicable Roles and Users
                CswNbtMetaDataObjectClass RoleOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RoleClass ); 
                foreach( CswNbtObjClassRole RoleNode in RoleOC.getNodes( false, false, false, true ) )
                {
                    _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.View, RequestItemNT, RoleNode,
                        RoleNode.Administrator.Checked == CswEnumTristate.True || RoleNode.Name.Text.Contains( "CISPro" ) );
                    if( RoleNode.Administrator.Checked == CswEnumTristate.True || 
                      ( RoleNode.Name.Text.Contains( "CISPro" ) && RoleNode.Name.Text != "CISPro_View_Only" ) )
                    {
                        _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtActionName.Submit_Request, RoleNode, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Edit, RequestItemNT, RoleNode, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Create, RequestItemNT, RoleNode, true );
                        _CswNbtSchemaModTrnsctn.Permit.set( CswEnumNbtNodeTypePermission.Delete, RequestItemNT, RoleNode, true );
                    }
                }
            }

            _updateAddLayout();
            _updateEditLayout();
            _updatePreviewLayout();
            _updateTableLayout();

            _updateRequestViews();
        } // update()

        private void _updateAddLayout()
        {
            CswNbtSchemaUpdateLayoutMgr AddLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Add );

            AddLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.RequestedFor );
            AddLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialType );
            AddLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename );
            AddLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier );
            AddLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo );
            AddLayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Quantity );
            AddLayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Size );
            AddLayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.SizeCount );
            AddLayoutMgr.First.moveProp( Row: 9, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            AddLayoutMgr.First.moveProp( Row: 10, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NeededBy );
            AddLayoutMgr.First.moveProp( Row: 11, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            AddLayoutMgr.First.moveProp( Row: 12, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Location );
            AddLayoutMgr.First.moveProp( Row: 13, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.CertificationLevel );
        }

        private void _updateEditLayout()
        {
            CswNbtSchemaUpdateLayoutMgr LayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Edit );

            LayoutMgr.Identity.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.RequestType );
            LayoutMgr.Identity.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Description );
            LayoutMgr.Identity.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Material );
            LayoutMgr.Identity.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.EnterprisePart );
            LayoutMgr.Identity.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Container );
            LayoutMgr.Identity.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Fulfill );

            LayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ItemNumber );
            LayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.IsBatch );
            LayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Requestor );
            LayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Status );
            LayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename );
            LayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier );
            LayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Quantity );
            LayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Size );
            LayoutMgr.First.moveProp( Row: 9, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Location );
            LayoutMgr.First.moveProp( Row: 10, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NeededBy );
            LayoutMgr.First.moveProp( Row: 11, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.TotalDispensed );
            LayoutMgr.First.moveProp( Row: 12, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.RecurringFrequency );
            LayoutMgr.First.moveProp( Row: 13, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Comments );

            LayoutMgr.First.moveProp( Row: 1, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            LayoutMgr.First.moveProp( Row: 2, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.BatchNumber );
            LayoutMgr.First.moveProp( Row: 3, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.RequestedFor );
            LayoutMgr.First.moveProp( Row: 4, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.AssignedTo );
            LayoutMgr.First.moveProp( Row: 5, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo );
            LayoutMgr.First.moveProp( Row: 6, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialType );
            LayoutMgr.First.moveProp( Row: 7, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.CertificationLevel );
            LayoutMgr.First.moveProp( Row: 8, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.SizeCount );
            LayoutMgr.First.moveProp( Row: 9, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            LayoutMgr.First.moveProp( Row: 10, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.Priority );
            LayoutMgr.First.moveProp( Row: 11, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.TotalMoved );
            LayoutMgr.First.moveProp( Row: 12, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.NextReorderDate );
            LayoutMgr.First.moveProp( Row: 13, Column: 2, PropName: CswNbtObjClassRequestItem.PropertyName.FulfillmentHistory );

            //Receipt Lots tab

            LayoutMgr["Receipt Lots"].copyProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.GoodsReceived );
            LayoutMgr["Receipt Lots"].copyProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ReceiptLotToDispense );
            LayoutMgr["Receipt Lots"].copyProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived );
        }

        private void _updatePreviewLayout()
        {
            CswNbtSchemaUpdateLayoutMgr PreviewLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Preview );

            PreviewLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Description );
            PreviewLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            PreviewLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.EnterprisePart );
            PreviewLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Material );
            PreviewLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Container );
            PreviewLayoutMgr.First.moveProp( Row: 6, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialTradename );
            PreviewLayoutMgr.First.moveProp( Row: 7, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialSupplier );
            PreviewLayoutMgr.First.moveProp( Row: 8, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.NewMaterialPartNo );
            PreviewLayoutMgr.First.moveProp( Row: 9, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Quantity );
            PreviewLayoutMgr.First.moveProp( Row: 10, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.SizeCount );
            PreviewLayoutMgr.First.moveProp( Row: 11, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Size );
            PreviewLayoutMgr.First.moveProp( Row: 12, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Location );
        }

        private void _updateTableLayout()
        {
            CswNbtSchemaUpdateLayoutMgr TableLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.RequestItemClass, LayoutType: CswEnumNbtLayoutType.Table );

            TableLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ItemNumber );
            TableLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.ExternalOrderNumber );
            TableLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.InventoryGroup );
            TableLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassRequestItem.PropertyName.Fulfill );
        }

        private void _updateRequestViews()
        {
            CswNbtMetaDataObjectClass RequestItemOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestItemClass );
            CswNbtView SubmittedRequestsView = _CswNbtSchemaModTrnsctn.restoreView( "Submitted Requests" ) ?? 
                _CswNbtSchemaModTrnsctn.makeSafeView( "Submitted Requests", CswEnumNbtViewVisibility.Global );
            SubmittedRequestsView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
            SubmittedRequestsView.Category = "Requests";
            SubmittedRequestsView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship RootRel = SubmittedRequestsView.AddViewRelationship( RequestItemOC, false );
            CswNbtViewProperty DescriptionVP = SubmittedRequestsView.AddViewProperty( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Description ), 1 );
            DescriptionVP.Width = 50;
            SubmittedRequestsView.AddViewProperty( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.NeededBy ), 2 );
            CswNbtViewProperty LocationVP = SubmittedRequestsView.AddViewProperty( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Location ), 3 );
            LocationVP.Width = 40;
            CswNbtViewProperty InventoryGroupVP = SubmittedRequestsView.AddViewProperty( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.InventoryGroup ), 4 );
            InventoryGroupVP.Width = 20;
            SubmittedRequestsView.AddViewPropertyAndFilter( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.RequestedFor ), "me", ShowAtRuntime: true );
            SubmittedRequestsView.AddViewPropertyAndFilter( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Requestor ), "me", ShowAtRuntime: true );
            SubmittedRequestsView.AddViewPropertyAndFilter( RootRel, RequestItemOC.getObjectClassProp( CswNbtObjClassRequestItem.PropertyName.Status ), CswNbtObjClassRequestItem.Statuses.Submitted, ShowInGrid: false, ShowAtRuntime: true );
            SubmittedRequestsView.save();

            _makeGridView( CswEnumNbtObjectClass.RequestClass,
                CswNbtObjClassRequest.PropertyName.RequestItems,
                CswEnumNbtObjectClass.RequestItemClass,
                CswNbtObjClassRequestItem.PropertyName.Request,
                new List<string>
                    {
                        CswNbtObjClassRequestItem.PropertyName.Status,
                        CswNbtObjClassRequestItem.PropertyName.Description,
                        CswNbtObjClassRequestItem.PropertyName.NeededBy,
                        CswNbtObjClassRequestItem.PropertyName.Location,
                        CswNbtObjClassRequestItem.PropertyName.InventoryGroup,
                        CswNbtObjClassRequestItem.PropertyName.RequestedFor,
                        CswNbtObjClassRequestItem.PropertyName.ItemNumber
                    }
                );

            _makeGridView( CswEnumNbtObjectClass.ContainerClass,
                CswNbtObjClassContainer.PropertyName.SubmittedRequests,
                CswEnumNbtObjectClass.RequestItemClass,
                CswNbtObjClassRequestItem.PropertyName.Container,
                new List<string>
                    {
                        CswNbtObjClassRequestItem.PropertyName.Status,
                        CswNbtObjClassRequestItem.PropertyName.Description,
                        CswNbtObjClassRequestItem.PropertyName.NeededBy,
                        CswNbtObjClassRequestItem.PropertyName.InventoryGroup,
                        CswNbtObjClassRequestItem.PropertyName.Requestor,
                        CswNbtObjClassRequestItem.PropertyName.RequestedFor,
                        CswNbtObjClassRequestItem.PropertyName.ItemNumber
                    }
                );

            _makeGridView( CswEnumNbtObjectClass.RequestItemClass,
                CswNbtObjClassRequestItem.PropertyName.ReceiptLotsReceived,
                CswEnumNbtObjectClass.ReceiptLotClass,
                CswNbtObjClassReceiptLot.PropertyName.RequestItem,
                new List<string>
                    {
                        CswNbtObjClassReceiptLot.PropertyName.ExpirationDate,
                        CswNbtObjClassReceiptLot.PropertyName.InvestigationNotes,
                        CswNbtObjClassReceiptLot.PropertyName.Manufacturer,
                        CswNbtObjClassReceiptLot.PropertyName.UnderInvestigation
                    }
                );
        }

        private void _makeGridView( string RootOCName, string RootGridOCPName, string RelOCName, string RelOCPName, List<String> RelOCPNames )
        {
            CswNbtMetaDataObjectClass RootOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( RootOCName );
            CswNbtMetaDataObjectClass RelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( RelOCName );
            CswNbtMetaDataObjectClassProp GridOCP = RootOC.getObjectClassProp( RootGridOCPName );
            CswNbtMetaDataObjectClassProp RelOCP = RelOC.getObjectClassProp( RelOCPName );
            CswNbtView GridView = _CswNbtSchemaModTrnsctn.makeSafeView( RootGridOCPName, CswEnumNbtViewVisibility.Property );
            GridView.ViewName = RootGridOCPName;
            GridView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
            GridView.Root.ChildRelationships.Clear();
            CswNbtViewRelationship RootRel = GridView.AddViewRelationship( RootOC, false );
            CswNbtViewRelationship Rel = GridView.AddViewRelationship( RootRel, CswEnumNbtViewPropOwnerType.Second, RelOCP, true );
            for( int i = 0; i < RelOCPNames.Count; i++ )
            {
                GridView.AddViewProperty( Rel, RelOC.getObjectClassProp( RelOCPNames[i] ), i + 1 );
            }
            GridView.save();
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( GridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, GridView.ToString() );
            foreach( CswNbtMetaDataNodeType RootNT in RootOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp GridNTP = RootNT.getNodeTypePropByObjectClassProp( GridOCP );
                GridNTP.ViewId = GridView.ViewId;
            }
        }

    }
}//namespace ChemSW.Nbt.Schema