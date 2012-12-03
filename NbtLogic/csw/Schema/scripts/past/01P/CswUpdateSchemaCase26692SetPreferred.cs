using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26692SetPreferred
    /// </summary>
    public class CswUpdateSchemaCase26692SetPreferred : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( InspectionDesignOc, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                                  {
                                                                                      FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                                                                                      PropName = CswNbtObjClassInspectionDesign.SetPreferredPropertyName
                                                                                  } );
            foreach( CswNbtMetaDataNodeType InspectionNt in InspectionDesignOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp Finished = InspectionNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.FinishPropertyName );
                if( null != Finished.FirstEditLayout )
                {
                    CswNbtMetaDataNodeTypeProp SetPreferred = InspectionNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.SetPreferredPropertyName );
                    SetPreferred.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, true, Finished.FirstEditLayout.TabId, 1, 1 );
                    Finished.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, SetPreferred, true );
                    CswNbtMetaDataNodeTypeProp Cancel = InspectionNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.CancelPropertyName );
                    if( null != Cancel.FirstEditLayout && Finished.FirstEditLayout.TabId == Cancel.FirstEditLayout.TabId )
                    {
                        Cancel.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, Finished, true );
                    }
                    CswNbtMetaDataNodeTypeProp CancelReason = InspectionNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.CancelReasonPropertyName );
                    if( null != CancelReason.FirstEditLayout && Finished.FirstEditLayout.TabId == CancelReason.FirstEditLayout.TabId )
                    {
                        CancelReason.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, Cancel, true );
                    }
                }

            }

        }//Update()

    }//class CswUpdateSchemaCase26692SetPreferred

}//namespace ChemSW.Nbt.Schema