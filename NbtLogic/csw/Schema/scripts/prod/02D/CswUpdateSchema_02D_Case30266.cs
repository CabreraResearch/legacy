using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02D_Case30266: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30266; }
        }

        public override void update()
        {
            //Fetch the data we need
            CswNbtMetaDataObjectClass SDSDocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.SDSDocumentClass );
            CswNbtMetaDataObjectClassProp fileTypeOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.FileType );
            CswNbtMetaDataObjectClassProp archivedOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Archived );
            CswNbtMetaDataObjectClassProp formatOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Format );
            CswNbtMetaDataObjectClassProp languageOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Language );
            CswNbtMetaDataObjectClassProp fileOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.File );
            CswNbtMetaDataObjectClassProp linkOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Link );
            CswNbtMetaDataObjectClassProp ownerOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.Owner );
            CswNbtMetaDataObjectClassProp revisionDateOCP = SDSDocOC.getObjectClassProp( CswNbtObjClassSDSDocument.PropertyName.RevisionDate );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp AssignedSDS_OCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.AssignedSDS );

            //Find the relationship to add the prop to
            CswNbtView AssignedSDSView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( AssignedSDS_OCP.ViewXml );
            AssignedSDSView.Clear();
            AssignedSDSView.SetVisibility( CswEnumNbtViewVisibility.Property, null, null );

            CswNbtViewRelationship chemLvl = AssignedSDSView.AddViewRelationship( ChemicalOC, false );
            CswNbtViewRelationship sdsLvl = AssignedSDSView.AddViewRelationship( chemLvl, CswEnumNbtViewPropOwnerType.Second, ownerOCP, true );

            AssignedSDSView.AddViewPropertyAndFilter( sdsLvl,
                                                        MetaDataProp : archivedOCP,
                                                        SubFieldName : CswEnumNbtSubFieldName.Checked,
                                                        Value : false.ToString(),
                                                        FilterMode : CswEnumNbtFilterMode.Equals,
                                                        ShowInGrid : false );

            AssignedSDSView.AddViewProperty( sdsLvl, revisionDateOCP, 1 );
            AssignedSDSView.AddViewProperty( sdsLvl, formatOCP, 5 );
            AssignedSDSView.AddViewProperty( sdsLvl, languageOCP, 4 );
            AssignedSDSView.AddViewProperty( sdsLvl, fileOCP, 2 );
            AssignedSDSView.AddViewProperty( sdsLvl, linkOCP, 3 );
            CswNbtViewProperty fileTypeVP = AssignedSDSView.AddViewProperty( sdsLvl, fileTypeOCP );
            fileTypeVP.ShowInGrid = false;

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( AssignedSDS_OCP, CswEnumNbtObjectClassPropAttributes.viewxml, AssignedSDSView.ToString() );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema