using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24519
    /// </summary>
    public class CswUpdateSchemaCase24519 : CswUpdateSchemaTo
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

        private void _updateNodeTypeProps( CswNbtMetaDataNodeType DocumentNt, CswNbtMetaDataObjectClass RelatedObjectClass )
        {
            string TemplateEntry = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassDocument.TitlePropertyName );
            DocumentNt.setNameTemplateText( TemplateEntry );

            CswNbtMetaDataNodeTypeProp AcquiredDateNtp = DocumentNt.getNodeTypePropByObjectClassProp( AcquiredDateOcp.PropName );
            AcquiredDateNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, Int32.MinValue, 1, 2 );

            CswNbtMetaDataNodeTypeProp ExpirationDateNtp = DocumentNt.getNodeTypePropByObjectClassProp( ExpirationDateOcp.PropName );
            ExpirationDateNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, Int32.MinValue, 2, 2 );

            CswNbtMetaDataNodeTypeProp OwnerNtp = DocumentNt.getNodeTypePropByObjectClassProp( OwnerOcp.PropName );
            OwnerNtp.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), RelatedObjectClass.ObjectClassId );
            OwnerNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, Int32.MinValue, 3, 2 );

            CswNbtMetaDataNodeTypeProp ArchivedNtp = DocumentNt.getNodeTypePropByObjectClassProp( ArchivedOcp.PropName );
            ArchivedNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, Int32.MinValue, 4, 2 );
        }

        private void _makeGridViewForDocumentsTab( CswNbtMetaDataNodeType NodeType, CswNbtMetaDataObjectClass ObjectClass, CswNbtMetaDataNodeType DocumentNodeType )
        {
            CswNbtMetaDataNodeTypeTab DocumentsTab = NodeType.getNodeTypeTab( "Documents" ) ??
                _CswNbtSchemaModTrnsctn.MetaData.makeNewTab( NodeType, "Documents", Int32.MinValue );
            CswNbtMetaDataNodeTypeProp DocumentsProp = NodeType.getNodeTypeProp( "Documents" ) ??
                _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( NodeType, CswNbtMetaDataFieldType.NbtFieldType.Grid, "Documents", DocumentsTab.TabId );
            CswNbtMetaDataNodeTypeProp OwnerProp = DocumentNodeType.getNodeTypePropByObjectClassProp( OwnerOcp.PropName );

            CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.restoreView( DocumentsProp.ViewId );
            DocumentsView.Root.ChildRelationships.Clear();
            DocumentsView.ViewMode = NbtViewRenderingMode.Grid;
            DocumentsView.Visibility = NbtViewVisibility.Property;

            CswNbtViewRelationship OwnerRelationship = DocumentsView.AddViewRelationship( ObjectClass, true );
            CswNbtViewRelationship DocumentRelationship = DocumentsView.AddViewRelationship( OwnerRelationship, NbtViewPropOwnerType.Second, OwnerProp, true );
            DocumentsView.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( TitleOcp.PropName ) );
            DocumentsView.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( AcquiredDateOcp.PropName ) );
            DocumentsView.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( ExpirationDateOcp.PropName ) );
            DocumentsView.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( DocumentClassOcp.PropName ) );
            DocumentsView.AddViewProperty( DocumentRelationship, DocumentNodeType.getNodeTypePropByObjectClassProp( ArchivedOcp.PropName ) );
            DocumentsView.save();
        }

        public override void update()
        {
            #region OC

            DocumentOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass, "page.gif", true, false );
            TitleOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.TitlePropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.Text,
                                                                      SetValOnAdd: true, DisplayRowAdd: 2, DisplayColAdd: 1 );

            FileTypeOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                          CswNbtObjClassDocument.FileTypePropertyName,
                                                          CswNbtMetaDataFieldType.NbtFieldType.List,
                                                          ListOptions: CswNbtObjClassDocument.AllowedFileTypes.ToString(),
                                                          SetValOnAdd: true, DisplayRowAdd: 3, DisplayColAdd: 1 );

            FileOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.FilePropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.File,
                                                                      SetValOnAdd: true, DisplayRowAdd: 4, DisplayColAdd: 1 );
            FileOcp.FilterObjectClassPropId = FileTypeOcp.PropId;
            char FilterDelimiter = '|';
            string FileFilterString = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.Equals + FilterDelimiter + "File";
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( FileOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filter, FileFilterString );

            LinkOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.LinkPropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.Link,
                                                                      SetValOnAdd: true, DisplayRowAdd: 5, DisplayColAdd: 1 );
            LinkOcp.FilterObjectClassPropId = FileTypeOcp.PropId;
            string LinkFilterString = CswNbtSubField.PropColumn.Field1.ToString() + FilterDelimiter + CswNbtPropFilterSql.PropertyFilterMode.Equals + FilterDelimiter + "Link";
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LinkOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.filter, LinkFilterString );

            DocumentClassOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.DocumentClassPropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.List,
                                                                      SetValOnAdd: true, DisplayRowAdd: 6, DisplayColAdd: 1 );

            //OpenOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
            //                                              CswNbtObjClassDocument.OpenPropertyName,
            //                                              CswNbtMetaDataFieldType.NbtFieldType.Button );

            OwnerOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.OwnerPropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                                      SetValOnAdd: true, IsRequired: true, DisplayRowAdd: 1, DisplayColAdd: 1 );

            AcquiredDateOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.AcquiredDatePropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.DateTime );

            ExpirationDateOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                      CswNbtObjClassDocument.ExpirationDatePropertyName,
                                                                      CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                                                                      SetValOnAdd: true, DisplayRowAdd: 7, DisplayColAdd: 1 );

            ArchivedOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.DocumentClass,
                                                                        CswNbtObjClassDocument.ArchivedPropertyName,
                                                                        CswNbtMetaDataFieldType.NbtFieldType.Logical );

            #endregion

            #region NT

            CswNbtMetaDataNodeType MaterialDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( DocumentOc.ObjectClassId, "Material Document", "Materials" );
            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            _updateNodeTypeProps( MaterialDocumentNt, MaterialOc );
            CswNbtMetaDataNodeTypeProp DocumentClassNtp = MaterialDocumentNt.getNodeTypePropByObjectClassProp( DocumentClassOcp.PropName );
            DocumentClassNtp.ListOptions = "[blank],MSDS";

            CswNbtMetaDataNodeType ContainerDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( DocumentOc.ObjectClassId, "Container Document", "Materials" );
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
            _updateNodeTypeProps( ContainerDocumentNt, ContainerOc );
            CswNbtMetaDataNodeTypeProp ContainerDocClassOwnerNtp = ContainerDocumentNt.getNodeTypePropByObjectClassProp( DocumentClassOcp.PropName );
            ContainerDocClassOwnerNtp.ListOptions = "[blank],C of A";

            CswNbtMetaDataNodeType EquipmentDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( DocumentOc.ObjectClassId, "Equipment Document", "Equipment" );
            CswNbtMetaDataObjectClass EquipmentOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass );
            _updateNodeTypeProps( EquipmentDocumentNt, EquipmentOc );

            CswNbtMetaDataNodeType AssemblyDocumentNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( DocumentOc.ObjectClassId, "Assembly Document", "Equipment" );
            CswNbtMetaDataObjectClass AssemblyOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass );
            _updateNodeTypeProps( AssemblyDocumentNt, AssemblyOc );
            #endregion

            #region Tabs and Views

            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                _makeGridViewForDocumentsTab( MaterialNt, MaterialOc, MaterialDocumentNt );
            }

            foreach( CswNbtMetaDataNodeType ContainerNt in ContainerOc.getNodeTypes() )
            {
                _makeGridViewForDocumentsTab( ContainerNt, ContainerOc, ContainerDocumentNt );
            }

            foreach( CswNbtMetaDataNodeType EquipmentNt in EquipmentOc.getNodeTypes() )
            {
                _makeGridViewForDocumentsTab( EquipmentNt, EquipmentOc, EquipmentDocumentNt );
            }

            foreach( CswNbtMetaDataNodeType AssemblyNt in AssemblyOc.getNodeTypes() )
            {
                _makeGridViewForDocumentsTab( AssemblyNt, AssemblyOc, AssemblyDocumentNt );
            }

            #endregion

            #region MSDS View

            if( _CswNbtSchemaModTrnsctn.MetaData._CswNbtMetaDataResources.CswNbtResources.ModulesEnabled().Contains( CswNbtResources.CswNbtModule.CISPro ) )
            {
                CswNbtView MSDSView = _CswNbtSchemaModTrnsctn.makeView();
                MSDSView.makeNew( "MSDS Expiring Next Month", NbtViewVisibility.Global );
                MSDSView.ViewMode = NbtViewRenderingMode.Tree;
                MSDSView.Category = "Materials";

                CswNbtViewRelationship DocumentVr = MSDSView.AddViewRelationship( MaterialDocumentNt, true );
                CswNbtMetaDataNodeTypeProp ExpirationDateNtp = MaterialDocumentNt.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.ExpirationDatePropertyName );
                MSDSView.AddViewPropertyAndFilter( DocumentVr, ExpirationDateNtp, "today+30", FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals );
                MSDSView.AddViewPropertyAndFilter( DocumentVr, DocumentClassNtp, "MSDS" );

                MSDSView.save();
            }

            #endregion


        }//Update()



    }//class CswUpdateSchemaCase24519

}//namespace ChemSW.Nbt.Schema