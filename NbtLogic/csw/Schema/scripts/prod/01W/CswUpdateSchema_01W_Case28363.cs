using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.ObjectModel;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28363
    /// </summary>
    public class CswUpdateSchema_01W_Case28363 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 28363; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            //update the View SDS menu opts with the correct values
            foreach( CswNbtObjClassMaterial materialNode in materialOC.getNodes( false, false, false, true ) )
            {
                materialNode.ViewSDS.State = CswNbtObjClassMaterial.PropertyName.ViewSDS;
                materialNode.UpdateViewSDSButtonOpts();
                materialNode.postChanges( false );
            }

            //Move the View SDS button to the Identity tab for Chemicals and hide it for everything else
            foreach( CswNbtMetaDataNodeType materialNT in materialOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp viewSDS_NTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.ViewSDS );
                if( materialNT.NodeTypeName.Equals( "Chemical" ) )
                {
                    CswNbtMetaDataNodeTypeProp receiveNTP = materialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.Receive );
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, viewSDS_NTP, receiveNTP, true );
                }
                else
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( viewSDS_NTP );
                }

                //update Material Documents grid to only display Docs have a Doc Class of anything but SDS
                CswNbtMetaDataNodeTypeProp documentsNTP = materialNT.getNodeTypeProp( "Documents" );
                if( null != documentsNTP )
                {
                    CswNbtView documentsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( documentsNTP.ViewId );

                    CswNbtMetaDataNodeType materialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                    if( null != materialDocumentNT )
                    {
                        CswNbtMetaDataNodeTypeProp docClassNTP = materialDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
                        _addDocumentClassPropFilter( documentsView.Root.ChildRelationships, materialDocumentNT, docClassNTP, documentsView );
                    }

                }

            }

            //Change the Document.Issue Date NTP name to "Revision Date"
            CswNbtMetaDataObjectClass documentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.DocumentClass );

            foreach( CswNbtMetaDataNodeType documentNT in documentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp issueDateNTP = documentNT.getNodeTypeProp( "Issue Date" );
                if( null != issueDateNTP )
                {
                    issueDateNTP.PropName = "Revision Date";
                }
            }


        } //Update()

        private void _addDocumentClassPropFilter( Collection<CswNbtViewRelationship> childRelationships, CswNbtMetaDataNodeType materialDocumentNT, CswNbtMetaDataNodeTypeProp docClassNTP, CswNbtView docsView )
        {
            bool complete = false;
            foreach( CswNbtViewRelationship parent in childRelationships )
            {
                if( parent.SecondName.Equals( materialDocumentNT.NodeTypeName ) )
                {
                    bool foundDocClassProp = false;
                    foreach( CswNbtViewProperty viewProp in parent.Properties )
                    {
                        if( viewProp.NodeTypePropId == docClassNTP.PropId )
                        {
                            foundDocClassProp = true;
                            docsView.AddViewPropertyFilter( viewProp,
                                FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                                Value: CswNbtObjClassDocument.DocumentClasses.SDS,
                                SubFieldName: CswNbtSubField.SubFieldName.Value );
                        }
                    }
                    if( false == foundDocClassProp ) //if the Document Class prop does not exist, add it and a filter and do no show it in the grid
                    {
                        docsView.AddViewPropertyAndFilter( parent,
                            MetaDataProp: docClassNTP,
                            Value: CswNbtObjClassDocument.DocumentClasses.SDS,
                            SubFieldName: CswNbtSubField.SubFieldName.Value,
                            FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals,
                            ShowInGrid: false );
                    }
                    docsView.save();
                    complete = true; //we no longer need to recurse
                }
                else if( parent.ChildRelationships.Count > 0 && false == complete ) //continue searching only if we haven't found what we're looking for
                {
                    _addDocumentClassPropFilter( parent.ChildRelationships, materialDocumentNT, docClassNTP, docsView );
                }
            }
        }

    }//class CswUpdateSchema_01V_Case28363

}//namespace ChemSW.Nbt.Schema