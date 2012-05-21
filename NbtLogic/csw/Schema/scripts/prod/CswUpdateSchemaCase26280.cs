using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26280
    /// </summary>
    public class CswUpdateSchemaCase26280 : CswUpdateSchemaTo
    {
        private CswNbtMetaDataObjectClass DocumentOc;
        private CswNbtMetaDataObjectClassProp TitleOcp;
        private CswNbtMetaDataObjectClassProp AcquiredDateOcp;
        private CswNbtMetaDataObjectClassProp ExpirationDateOcp;
        private CswNbtMetaDataObjectClassProp FileOcp;
        private CswNbtMetaDataObjectClassProp LinkOcp;
        private CswNbtMetaDataObjectClassProp FileTypeOcp;
        private CswNbtMetaDataObjectClassProp DocumentClassOcp;
        //private CswNbtMetaDataObjectClassProp OpenOcp;
        private CswNbtMetaDataObjectClassProp OwnerOcp;
        private CswNbtMetaDataObjectClassProp ArchivedOcp;


        private CswNbtView _getDocumentsTabGridView( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataObjectClass ObjectClass, CswNbtMetaDataNodeType DocumentNodeType )
        {
            CswNbtMetaDataNodeTypeTab DocumentsTab = NodeType.getNodeTypeTab( "Documents" ) ??
                _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( NodeType, "Documents", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp DocumentsProp = NodeType.getNodeTypeProp( "Documents" );
            CswNbtView Ret = null;
            if( null == DocumentsProp )
            {
                DocumentsProp = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( NodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Documents", DocumentsTab.TabId );
                CswNbtMetaDataNodeTypeProp OwnerProp = DocumentNodeType.getNodeTypePropByObjectClassProp( OwnerOcp.PropName );

                Ret = _CswNbtSchemaModTrnsctn.restoreView( DocumentsProp.ViewId );
                Ret.Root.ChildRelationships.Clear();
                Ret.ViewMode = NbtViewRenderingMode.Grid;
                Ret.Visibility = NbtViewVisibility.Property;

                CswNbtViewRelationship OwnerRelationship = Ret.AddViewRelationship( ObjectClass, false ); /* Case 26280: we don't want default filters */
                CswNbtViewRelationship DocumentRelationship = Ret.AddViewRelationship( OwnerRelationship, NbtViewPropOwnerType.Second, OwnerProp, true );
                Ret.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( TitleOcp.PropName ) );
                Ret.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( AcquiredDateOcp.PropName ) );
                Ret.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( ExpirationDateOcp.PropName ) );
                Ret.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( ArchivedOcp.PropName ) );
                Ret.save();
            }
            else
            {
                Ret = _CswNbtSchemaModTrnsctn.restoreView( DocumentsProp.ViewId );
            }
            return Ret;
        }

        private CswNbtMetaDataNodeTypeProp _pruneDocumentForImcs( CswNbtMetaDataNodeType DocumentNt, CswNbtMetaDataNodeTypeProp OwnerNtp, string OwnerName )
        {
            CswNbtMetaDataNodeTypeProp DocClassNtp = DocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.DocumentClassPropertyName );
            OwnerNtp.PropName = OwnerName;
            DocClassNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            DocClassNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit );
            DocClassNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview );
            DocClassNtp.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Table );
            DocClassNtp.IsRequired = false;
            return DocClassNtp;
        }

        public override void update()
        {
            DocumentOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass );

            #region NT

            CswNbtMetaDataObjectClass EquipmentOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp EquipmentStatsOcp = EquipmentOc.getObjectClassProp( CswNbtObjClassEquipment.StatusPropertyName );
            CswNbtMetaDataObjectClass AssemblyOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
            #endregion

            #region Tabs and Views

            foreach( CswNbtMetaDataNodeType DocumentNt in DocumentOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp OwnerNtp = DocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.OwnerPropertyName );
                if( OwnerNtp.FKType.ToLower() == NbtViewRelatedIdType.ObjectClassId.ToString().ToLower() )
                {

                    if( OwnerNtp.FKValue == AssemblyOc.ObjectClassId )
                    {
                        CswNbtMetaDataNodeTypeProp DocumentClassNtp = _pruneDocumentForImcs( DocumentNt, OwnerNtp, "Assembly" );
                        foreach( CswNbtMetaDataNodeType AssemblyNt in AssemblyOc.getNodeTypes() )
                        {
                            CswNbtView DocView = _getDocumentsTabGridView( AssemblyNt, AssemblyOc, DocumentNt );
                            DocView.removeViewProperty( DocumentClassNtp );
                            DocView.save();
                        }

                    }
                    else if( OwnerNtp.FKValue == EquipmentOc.ObjectClassId )
                    {
                        CswNbtMetaDataNodeTypeProp DocumentClassNtp = _pruneDocumentForImcs( DocumentNt, OwnerNtp, "Equipment" );
                        foreach( CswNbtMetaDataNodeType EquipmentNt in EquipmentOc.getNodeTypes() )
                        {
                            CswNbtView DocView = _getDocumentsTabGridView( EquipmentNt, EquipmentOc, DocumentNt );
                            DocView.removeViewProperty( DocumentClassNtp );
                            CswNbtMetaDataNodeTypeProp StatusNtp = DocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassEquipment.StatusPropertyName );
                            DocView.removeViewProperty( StatusNtp );
                            DocView.removeViewProperty( EquipmentStatsOcp );
                            DocView.save();
                        }
                    }
                }
            }

            #endregion

        }//Update()



    }//class CswUpdateSchemaCase26280

}//namespace ChemSW.Nbt.Schema