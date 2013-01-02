using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using System;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28374
    /// </summary>
    public class CswUpdateSchema_01V_Case28374 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28374; }
        }

        private CswNbtMetaDataNodeType ChemicalNT;

        public override void update()
        {
            //3 - Add the following properties to the Chemical's Preview Layout: TradeName, Supplier, NFPA, PPE, GHS pictos (?), Storage Compatibility
            ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                _addToPreview( CswNbtObjClassMaterial.PropertyName.Tradename );
                _addToPreview( CswNbtObjClassMaterial.PropertyName.Supplier );
                _addToPreview( "NFPA" );
                _addToPreview( "PPE" );
                _addToPreview( "GHS Pictos" );
                _addToPreview( CswNbtObjClassMaterial.PropertyName.StorageCompatibility );
            }

            //5 - Create new "My Contianers" grid, which is essentially the Containers view with an "owner=me" filter
            CswNbtView MyContainersView = _CswNbtSchemaModTrnsctn.restoreView( "My Containers" ) ??
                                          _CswNbtSchemaModTrnsctn.makeNewView( "My Containers", NbtViewVisibility.Global );
            MyContainersView.ViewMode = NbtViewRenderingMode.Grid;            
            MyContainersView.Category = "Containers";
            MyContainersView.Root.ChildRelationships.Clear();

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtViewRelationship ContainerRel = MyContainersView.AddViewRelationship( ContainerOC, false );

            CswNbtViewProperty BarcodeVP = MyContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Barcode ) );
            BarcodeVP.Order = 1;
            CswNbtViewProperty StatusVP = MyContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Status ) );
            StatusVP.Order = 2;
            CswNbtViewProperty QuantityVP = MyContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Quantity ) );
            QuantityVP.Order = 3;
            CswNbtViewProperty LocationVP = MyContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Location ) );
            LocationVP.Order = 4;
            CswNbtViewProperty DisposedVP = MyContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Disposed ) );
            DisposedVP.Order = 5;
            MyContainersView.AddViewPropertyFilter( DisposedVP,
                                          CswNbtPropFilterSql.PropertyFilterConjunction.And,
                                          CswNbtPropFilterSql.FilterResultMode.Hide,
                                          CswNbtSubField.SubFieldName.Checked,
                                          CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                          "false");
            CswNbtViewProperty OwnerVP = MyContainersView.AddViewProperty( ContainerRel, ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner ) );
            OwnerVP.Order = 6;
            OwnerVP.ShowInGrid = false;
            MyContainersView.AddViewPropertyFilter( OwnerVP, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals, Value: "me" );
            MyContainersView.save();            

            //9 - rename MSDS to SDS everywhere
            CswNbtMetaDataObjectClass DocumentClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );
            foreach(CswNbtMetaDataNodeType DocumentNT in DocumentClass.getNodeTypes())
            {
                CswNbtMetaDataNodeTypeProp DocumentClassNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
                DocumentClassNTP.ListOptions = DocumentClassNTP.ListOptions.Replace( "MSDS", "SDS" );
                DocumentClassNTP.DefaultValue.AsList.Value = DocumentClassNTP.DefaultValue.AsList.Value.Replace( "MSDS", "SDS" );
                DocumentClassNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );

                CswNbtMetaDataNodeTypeProp LanguageNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
                LanguageNTP.setFilter( DocumentClassNTP, DocumentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.SDS );
                LanguageNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeProp FormatNTP = DocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
                FormatNTP.setFilter( DocumentClassNTP, DocumentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.SDS );
                FormatNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );
                CswNbtMetaDataNodeTypeProp IssueDateNTP = DocumentNT.getNodeTypeProp( "Issue Date" );                
                if( null != IssueDateNTP )
                {
                    IssueDateNTP.setFilter( DocumentClassNTP, DocumentClassNTP.getFieldTypeRule().SubFields.Default, CswNbtPropFilterSql.PropertyFilterMode.Equals, CswNbtObjClassDocument.DocumentClasses.SDS );
                    IssueDateNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true, DocumentNT.getFirstNodeTypeTab().TabId );
                }                                
            }
            foreach( CswNbtObjClassDocument DocumentNode in DocumentClass.getNodes( false, false ) )
            {
                if( DocumentNode.DocumentClass.Value == "MSDS" )
                {
                    DocumentNode.DocumentClass.Value = "SDS";
                    DocumentNode.postChanges( false );
                }
            }
            CswNbtView MSDSView = _CswNbtSchemaModTrnsctn.restoreView( "MSDS Expiring Next Month" );
            if( null != MSDSView )
            {
                MSDSView.ViewName = "SDS Expiring Next Month";
                MSDSView.save();
            }

        }//Update()

        private void _addToPreview( String NodeTypePropName )
        {
            CswNbtMetaDataNodeTypeProp ChemicalNTP = ChemicalNT.getNodeTypeProp( NodeTypePropName );
            if( null != ChemicalNTP )
            {
                ChemicalNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Preview, true );
            }
        }

    }//class CswUpdateSchemaCase_01V_28374

}//namespace ChemSW.Nbt.Schema