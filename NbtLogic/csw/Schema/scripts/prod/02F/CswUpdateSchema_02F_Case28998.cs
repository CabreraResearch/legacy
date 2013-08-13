using System.Collections.ObjectModel;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02F_Case28998 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28998; }
        }

        public override string ScriptName
        {
            get { return "02F_Case28998"; }
        }

        public override bool AlwaysRun
        {
            get { return false; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestClass );
            CswNbtMetaDataObjectClass RequestContainerDispenseOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestContainerDispenseClass );
            CswNbtMetaDataObjectClass RequestContainerUpdateOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RequestContainerUpdateClass );

            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp RequestsGridNtp = ContainerNt.getNodeTypeProp( "Submitted Requests" );
                if( null != RequestsGridNtp )
                {
                    CswNbtView GridView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( RequestsGridNtp.ViewId );
                    GridView.Root.ChildRelationships.Clear();
                    GridView.ViewName = ContainerNt.NodeTypeName + " Requested Items";
                    GridView.Visibility = CswEnumNbtViewVisibility.Property;
                    GridView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                    GridView.Category = "Requests";
                    CswNbtViewRelationship RootRel = GridView.AddViewRelationship( ContainerNt, false );

                    Collection<CswNbtMetaDataObjectClass> OcsInThisView = new Collection<CswNbtMetaDataObjectClass>
                    {
                        RequestContainerDispenseOc, 
                        RequestContainerUpdateOc
                    };

                    foreach( CswNbtMetaDataObjectClass Oc in OcsInThisView )
                    {
                        CswNbtViewRelationship RequestItemRel =
                            GridView.AddViewRelationship
                            (
                                RootRel,
                                CswEnumNbtViewPropOwnerType.Second,
                                Oc.getObjectClassProp( CswNbtObjClassRequestContainerDispense.PropertyName.Container ),
                                false
                            );
                        CswNbtViewRelationship RequestRel =
                            GridView.AddViewRelationship
                            (
                                RequestItemRel,
                                CswEnumNbtViewPropOwnerType.First,
                                Oc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Request ), false
                            );
                        CswNbtViewProperty CompletedVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.CompletedDate ), 3 );
                        CompletedVp.SortBy = true;
                        CompletedVp.SortMethod = CswEnumNbtViewPropertySortMethod.Descending;
                        CswNbtViewProperty SubmittedVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.SubmittedDate ), 2 );
                        SubmittedVp.SortMethod = CswEnumNbtViewPropertySortMethod.Descending;
                        CswNbtViewProperty NameVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Name ), 1 );
                        NameVp.SortMethod = CswEnumNbtViewPropertySortMethod.Descending;
                        CswNbtViewProperty RequestorVp = GridView.AddViewProperty( RequestRel, RequestOc.getObjectClassProp( CswNbtObjClassRequest.PropertyName.Requestor ), 4 );
                        CswNbtViewProperty TypeVp = GridView.AddViewProperty( RequestItemRel, Oc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Number ), 5 );
                        CswNbtViewProperty NumberVp = GridView.AddViewProperty( RequestItemRel, Oc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Description ), 6 );
                        CswNbtViewProperty OrderVp = GridView.AddViewProperty( RequestItemRel, Oc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.ExternalOrderNumber ), 7 );
                        CswNbtViewProperty StatusVp = GridView.AddViewProperty( RequestItemRel, Oc.getObjectClassProp( CswNbtPropertySetRequestItem.PropertyName.Status ), 8 );
                        GridView.AddViewPropertyFilter( StatusVp, CswEnumNbtSubFieldName.Value, CswEnumNbtFilterMode.Equals, CswNbtPropertySetRequestItem.Statuses.Submitted );
                    }
                    GridView.save();
                }
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema