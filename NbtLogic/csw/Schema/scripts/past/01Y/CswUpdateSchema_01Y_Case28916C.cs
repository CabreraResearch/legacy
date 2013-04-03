using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28916
    /// </summary>
    public class CswUpdateSchema_01Y_Case28916C : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28916; }
        }

        public override void update()
        {
            CswNbtMetaDataNodeType SDSDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "SDS Document" );
            if( null != SDSDocumentNT )
            {
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp DocumentsNTP = MaterialNT.getNodeTypeProp("Documents");
                    if( null != DocumentsNTP )
                    {
                        //Material: Documents - MaterialClass->MaterialDocument (by Owner) - Title, Acquired Date, Expiration Date, Archived, File, Link (no filters)
                        CswNbtView DocumentsView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( DocumentsNTP.ViewId );
                        CswNbtMetaDataNodeType MaterialDocumentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Document" );
                        if( null != MaterialDocumentNT )
                        {
                            _removeDocumentClassPropAndFilters( DocumentsView, MaterialDocumentNT );
                        }
                    }
                    if( MaterialNT.NodeTypeName == "Chemical")
                    {
                        //Chemical: Assigned SDS - MaterialClass->SDS Document (by Owner) - Title, Language, Format, File, Link, Revision Date, Archived (equals 0, show at runtime)
                        CswNbtMetaDataNodeTypeProp AssignedSDSNTP = MaterialNT.getNodeTypeProp( "Assigned SDS" );
                        if( null != AssignedSDSNTP )
                        {
                            CswNbtView AssignedSDSView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( AssignedSDSNTP.ViewId );
                            AssignedSDSView.Root.ChildRelationships.Clear();
                            CswNbtViewRelationship RootRel = AssignedSDSView.AddViewRelationship( MaterialNT, false );
                            CswNbtMetaDataNodeTypeProp OwnerOCP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Owner );
                            CswNbtViewRelationship DocRel = AssignedSDSView.AddViewRelationship( RootRel, NbtViewPropOwnerType.Second, OwnerOCP, true );
                            CswNbtMetaDataNodeTypeProp ArchivedNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Archived );
                            AssignedSDSView.AddViewPropertyAndFilter( DocRel, ArchivedNTP, Tristate.False.ToString(),
                                                             FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals,
                                                             ShowAtRuntime: true, 
                                                             ShowInGrid: false );
                            CswNbtMetaDataNodeTypeProp TitleNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Title );
                            AssignedSDSView.AddViewProperty( DocRel, TitleNTP );
                            CswNbtMetaDataNodeTypeProp LanguageNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Language );
                            AssignedSDSView.AddViewProperty( DocRel, LanguageNTP );
                            CswNbtMetaDataNodeTypeProp FormatNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Format );
                            AssignedSDSView.AddViewProperty( DocRel, FormatNTP );
                            CswNbtMetaDataNodeTypeProp FileNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.File );
                            AssignedSDSView.AddViewProperty( DocRel, FileNTP );
                            CswNbtMetaDataNodeTypeProp LinkNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.Link );
                            AssignedSDSView.AddViewProperty( DocRel, LinkNTP );
                            CswNbtMetaDataNodeTypeProp RevisionDateNTP = SDSDocumentNT.getNodeTypeProp( "Revision Date" );
                            if( null != RevisionDateNTP )
                            {
                                AssignedSDSView.AddViewProperty( DocRel, RevisionDateNTP );
                            }
                            AssignedSDSView.save();
                        }
                    }
                }

                CswNbtView SDSView = _CswNbtSchemaModTrnsctn.restoreView("SDS Expiring Next Month");
                if( null != SDSView )
                {
                    //ViewSelect: SDS Expiring Next Month - SDS Document - Expiration Date (value <= today+30)
                    SDSView.Root.ChildRelationships.Clear();
                    CswNbtViewRelationship RootRel = SDSView.AddViewRelationship( SDSDocumentNT, false );
                    CswNbtMetaDataNodeTypeProp ExpirationDateNTP = SDSDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.ExpirationDate );
                    SDSView.AddViewPropertyAndFilter( RootRel, ExpirationDateNTP, "today+30",
                                                     FilterMode: CswNbtPropFilterSql.PropertyFilterMode.LessThanOrEquals);
                    SDSView.save();
                }
            }
        } //Update()

        private void _removeDocumentClassPropAndFilters( CswNbtView DocumentsView, CswNbtMetaDataNodeType MaterialDocumentNT )
        {
            IEnumerable<CswNbtViewRelationship> ChildRelationships = DocumentsView.Root.ChildRelationships[0].ChildRelationships;
            CswNbtMetaDataNodeTypeProp DocumentClassNTP = MaterialDocumentNT.getNodeTypePropByObjectClassProp( CswNbtObjClassDocument.PropertyName.DocumentClass );
            foreach( CswNbtViewRelationship Parent in ChildRelationships )
            {
                if( Parent.SecondName.Equals( MaterialDocumentNT.NodeTypeName ) )
                {
                    CswNbtViewProperty PropToDelete = null;
                    foreach( CswNbtViewProperty ViewProp in Parent.Properties )
                    {
                        if( null != ViewProp.Filters && ViewProp.Filters.Count > 0 )
                        {
                            ViewProp.removeFilter( (CswNbtViewPropertyFilter) ViewProp.Filters[0] );
                        }
                        if( ViewProp.NodeTypePropId == DocumentClassNTP.PropId )
                        {
                            PropToDelete = ViewProp; 
                        }  
                    }
                    if( null != PropToDelete )
                    {
                        Parent.removeProperty( PropToDelete );
                    }
                    DocumentsView.save();
                }
            }
        }
    }//class CswUpdateSchema_01Y_Case28916C
}//namespace ChemSW.Nbt.Schema