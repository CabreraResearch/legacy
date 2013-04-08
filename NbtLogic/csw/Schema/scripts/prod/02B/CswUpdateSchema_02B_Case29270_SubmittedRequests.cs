﻿using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case XXXXX
    /// </summary>
    public class CswUpdateSchema_02B_Case29270_SubmittedRequests : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 29270; }
        }

        public override void update()
        {
            CswNbtView Ret = _CswNbtSchemaModTrnsctn.makeSafeView( "Pending Requests", NbtViewVisibility.Global );
            
            Ret.ViewName = "Submitted Requests";
            Ret.Category = "Requests";
            Ret.ViewMode = NbtViewRenderingMode.Grid;

            foreach( NbtObjectClass Member in CswNbtPropertySetRequestItem.Members() )
            {
                CswNbtMetaDataObjectClass MemberOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( Member );
                CswNbtViewRelationship RequestItemRel = Ret.AddViewRelationship( MemberOc, IncludeDefaultFilters: false );

                Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ), CswNbtPropertySetRequestItem.Statuses.Submitted, ShowInGrid: false, ShowAtRuntime: true );
                if( MemberOc.ObjectClass == NbtObjectClass.RequestMaterialDispenseClass )
                {
                    Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsFavorite ),
                                                  FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                                  Value: CswNbtNodePropLogical.toLogicalGestalt( Tristate.True ), ShowInGrid: false );
                    Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtObjClassRequestMaterialDispense.PropertyName.IsRecurring ), Tristate.False.ToString(), ShowInGrid: false );
                }
                CswNbtViewProperty Vp2 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ) );
                Vp2.Width = 50;
                Vp2.Order = 2;
                CswNbtViewProperty Vp3 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.NeededBy ) );
                Vp3.Order = 3;
                CswNbtViewProperty Vp4 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Location ) );
                Vp4.Width = 40;
                Vp4.Order = 4;
                CswNbtViewProperty Vp5 = Ret.AddViewProperty( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.InventoryGroup ) );
                Vp5.Width = 20;
                Vp5.Order = 5;
                Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.RequestedFor ), ShowAtRuntime: true );
                Ret.AddViewPropertyAndFilter( RequestItemRel, MemberOc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Requestor ), ShowAtRuntime: true );
            }

            Ret.save();

        } //Update()

    }//class CswUpdateSchema_01Y_CaseXXXXX

}//namespace ChemSW.Nbt.Schema