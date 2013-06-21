using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

        #region Blame Logic

        private void _acceptBlame( UnitOfBlame Blame )
        {
            _Author = Blame.Developer;
            _CaseNo = Blame.CaseNumber;
        }

        private void _acceptBlame( CswEnumDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswEnumDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswEnumDeveloper _Author = CswEnumDeveloper.NBT;

        public override CswEnumDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        #endregion Blame Logic

        #region Private helpers

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswEnumNbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswEnumNbtLayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswEnumNbtLayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        private static string _makeNodeTypePermissionValue( Int32 FirstVersionNodeTypeId, CswEnumNbtNodeTypePermission Permission )
        {
            return "nt_" + FirstVersionNodeTypeId.ToString() + "_" + Permission.ToString();
        }

        #endregion Private helpers

        #region CEDAR Methods

        private void _makeLocationNameRequired( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.LocationClass );
            CswNbtMetaDataObjectClassProp NameOCP = LocationOC.getObjectClassProp( CswNbtObjClassLocation.PropertyName.Name );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( NameOCP, CswEnumNbtObjectClassPropAttributes.isrequired, true );

            _resetBlame();
        }

        private void _updateGHSPhraseCategoriesAndLanguages( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass GHSPhraseOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSPhraseClass );
            CswNbtMetaDataObjectClassProp CategoryOCP = GHSPhraseOC.getObjectClassProp( CswNbtObjClassGHSPhrase.PropertyName.Category );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( CategoryOCP, CswEnumNbtObjectClassPropAttributes.listoptions, "Physical,Health,Environmental,Precaution" );

            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.English );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Danish );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Dutch );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Finnish );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.French );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.German );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Italian );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Portuguese );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Spanish );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Swedish );
            _addGHSPhraseLanguage( GHSPhraseOC, CswNbtObjClassGHSPhrase.PropertyName.Chinese );

            _resetBlame();
        }

        private void _addGHSPhraseLanguage( CswNbtMetaDataObjectClass GHSPhraseOC, String Language )
        {
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( GHSPhraseOC )
            {
                PropName = Language,
                FieldType = CswEnumNbtFieldType.Text
            } );
        }

        private void _updateContainerLabelFormatViewXML( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataNodeType ContainerNT = ContainerOC.FirstNodeType;
            CswNbtMetaDataObjectClassProp LabelFormatOCP = ContainerOC.getObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );
            CswNbtMetaDataNodeTypeProp LabelFormatNTP = ContainerNT.getNodeTypePropByObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );
            CswNbtView LabelFormatView = _CswNbtSchemaModTrnsctn.ViewSelect.restoreView( LabelFormatNTP.ViewId );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LabelFormatOCP, CswEnumNbtObjectClassPropAttributes.viewxml, LabelFormatView.ToString() );

            _resetBlame();
        }

        private void _updatePPEOptions( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp PPEOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.PPE );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( PPEOCP, CswEnumNbtObjectClassPropAttributes.listoptions, "Goggles,Gloves,Clothing,Fume Hood,Respirator" );

            _resetBlame();
        }

        private void _addViewCofAButtons( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.ViewCofA,
                FieldType = CswEnumNbtFieldType.Button,
                Extended = CswNbtNodePropButton.ButtonMode.menu
            } );

            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ReceiptLotOC )
            {
                PropName = CswNbtObjClassReceiptLot.PropertyName.ViewCofA,
                FieldType = CswEnumNbtFieldType.Button,
                Extended = CswNbtNodePropButton.ButtonMode.menu
            } );

            _resetBlame();
        }

        #region Case 29833

        private void _createDocumentObjClasses( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass ReceiptLotOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReceiptLotClass );
            _createDocObjClass( CswEnumNbtObjectClass.SDSDocumentClass, CswEnumNbtModuleName.SDS, ChemicalOC.ObjectClassId, "Safety Data Sheet" );
            _createDocObjClass( CswEnumNbtObjectClass.CofADocumentClass, CswEnumNbtModuleName.CofA, ReceiptLotOC.ObjectClassId, "Certificate Of Analysis" );

            _resetBlame();
        }

        private void _createDocObjClass( string ObjClassName, CswEnumNbtModuleName ModuleName, Int32 OwnerOCId, string DefaultTitle )
        {
            CswNbtMetaDataObjectClass DocOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            if( null == DocOC )
            {
                DocOC = _CswNbtSchemaModTrnsctn.createObjectClass( ObjClassName, "doc.png", false );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( ModuleName, DocOC.ObjectClassId );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.AcquiredDate,
                    FieldType = CswEnumNbtFieldType.DateTime
                } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.ArchiveDate,
                    FieldType = CswEnumNbtFieldType.DateTime
                } );
                CswNbtMetaDataObjectClassProp ArchivedOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.Archived,
                    FieldType = CswEnumNbtFieldType.Logical,
                    IsRequired = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ArchivedOCP, CswEnumTristate.False );
                CswNbtMetaDataObjectClassProp FileTypeOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.FileType,
                    FieldType = CswEnumNbtFieldType.List,
                    ListOptions = CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Options.ToString(),
                    IsRequired = true,
                    SetValOnAdd = true
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( FileTypeOCP, CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File );
                CswNbtMetaDataObjectClassProp FileOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.File,
                    FieldType = CswEnumNbtFieldType.File,
                    SetValOnAdd = true
                } );
                FileOCP.setFilter( FileTypeOCP, FileTypeOCP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswNbtPropertySetDocument.CswEnumDocumentFileTypes.File );
                CswNbtMetaDataObjectClassProp LinkOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.Link,
                    FieldType = CswEnumNbtFieldType.Link,
                    SetValOnAdd = true
                } );
                LinkOCP.setFilter( FileTypeOCP, FileTypeOCP.getFieldTypeRule().SubFields.Default, CswEnumNbtFilterMode.Equals, CswNbtPropertySetDocument.CswEnumDocumentFileTypes.Link );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.Owner,
                    FieldType = CswEnumNbtFieldType.Relationship,
                    IsFk = true,
                    FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                    FkValue = OwnerOCId,
                    IsRequired = true
                } );
                CswNbtMetaDataObjectClassProp TitleOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtPropertySetDocument.PropertyName.Title,
                    FieldType = CswEnumNbtFieldType.Text
                } );
                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( TitleOCP, DefaultTitle );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                {
                    PropName = CswNbtObjClassSDSDocument.PropertyName.RevisionDate,
                    FieldType = CswEnumNbtFieldType.DateTime,
                    SetValOnAdd = true
                } );
                if( ObjClassName == CswEnumNbtObjectClass.SDSDocumentClass )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                    {
                        PropName = CswNbtObjClassSDSDocument.PropertyName.Language,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = "en,fr,es,de",
                        SetValOnAdd = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( DocOC )
                    {
                        PropName = CswNbtObjClassSDSDocument.PropertyName.Format,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = CswNbtObjClassSDSDocument.CswEnumSDSDocumentFormats.Options.ToString(),
                        SetValOnAdd = true
                    } );
                }
            }
        }

        private void _createDocumentPropertySet( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataPropertySet DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.DocumentSet );
            if( null == DocumentPS )
            {
                DocumentPS = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropertySet( CswEnumNbtPropertySetName.DocumentSet, "doc.png" );

                //Update jct_propertyset_objectclass
                CswTableUpdate JctPSOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29833_jctpsoc_update", "jct_propertyset_objectclass" );
                DataTable JctPSOCTable = JctPSOCUpdate.getEmptyTable();
                _addObjClassToPropertySetDocument( JctPSOCTable, CswEnumNbtObjectClass.DocumentClass, DocumentPS.PropertySetId );
                _addObjClassToPropertySetDocument( JctPSOCTable, CswEnumNbtObjectClass.SDSDocumentClass, DocumentPS.PropertySetId );
                _addObjClassToPropertySetDocument( JctPSOCTable, CswEnumNbtObjectClass.CofADocumentClass, DocumentPS.PropertySetId );
                JctPSOCUpdate.update( JctPSOCTable );

                //Update jct_propertyset_ocprop
                CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "29833_jctpsocp_update", "jct_propertyset_ocprop" );
                DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();
                _addObjClassPropsToPropertySetDocument( JctPSOCPTable, CswEnumNbtObjectClass.DocumentClass, DocumentPS.PropertySetId );
                _addObjClassPropsToPropertySetDocument( JctPSOCPTable, CswEnumNbtObjectClass.SDSDocumentClass, DocumentPS.PropertySetId );
                _addObjClassPropsToPropertySetDocument( JctPSOCPTable, CswEnumNbtObjectClass.CofADocumentClass, DocumentPS.PropertySetId );
                JctPSOCPUpdate.update( JctPSOCPTable );
            }

            _resetBlame();
        }

        private void _addObjClassToPropertySetDocument( DataTable JctPSOCTable, string ObjClassName, int PropertySetId )
        {
            DataRow NewJctPSOCRow = JctPSOCTable.NewRow();
            NewJctPSOCRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( ObjClassName );
            NewJctPSOCRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
            JctPSOCTable.Rows.Add( NewJctPSOCRow );
        }

        private void _addObjClassPropsToPropertySetDocument( DataTable JctPSOCPTable, string ObjClassName, int PropertySetId )
        {
            CswNbtMetaDataObjectClass DocumentObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in DocumentObjectClass.getObjectClassProps() )
            {
                bool doInsert = ( ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.AcquiredDate ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.ArchiveDate ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.Archived ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.File ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.FileType ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.Link ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.Owner ||
                                    ObjectClassProp.PropName == CswNbtPropertySetDocument.PropertyName.Title
                                );
                if( doInsert )
                {
                    DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                    NewJctPSOCPRow["objectclasspropid"] = ObjectClassProp.PropId;
                    NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
                    JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
                }
            }
        }

        #endregion Case 29833

        private void _addIsConstituentProperty( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            foreach( CswNbtMetaDataObjectClass MaterialOC in _CswNbtSchemaModTrnsctn.MetaData.getObjectClassesByPropertySetId( MaterialPS.PropertySetId ) )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                    {
                        PropName = CswNbtPropertySetMaterial.PropertyName.IsConstituent,
                        FieldType = CswEnumNbtFieldType.Logical,
                        ServerManaged = true
                    } );
            }
            _resetBlame();
        }

        private void _addRegulatoryListCasNoOC( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
                if( null == RegListCasNoOC )
                {
                    RegListCasNoOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass, "doc.png", false );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo,
                        FieldType = CswEnumNbtFieldType.CASNo,
                        IsCompoundUnique = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.ErrorMessage,
                        FieldType = CswEnumNbtFieldType.Text,
                        ServerManaged = true
                    } );
                    //_CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    //{
                    //    PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.IsValid,
                    //    FieldType = CswEnumNbtFieldType.Logical
                    //} );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = RegListOC.ObjectClassId,
                        IsCompoundUnique = true
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.TPQ,
                        FieldType = CswEnumNbtFieldType.Quantity
                    } );

                    _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.RegulatoryLists, RegListCasNoOC.ObjectClassId );

                } // if( null == RegListCasNoOC )
            } // if( null != RegListOC )
            _resetBlame();
        } // _addRegulatoryListCasNoOC

        private void _addRegulatoryListMemberOC( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != ChemicalOC && null != RegListOC && null != UserOC )
            {
                CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
                if( null == RegListMemberOC )
                {
                    RegListMemberOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass, "doc.png", false );
                    //_CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                    //    {
                    //        PropName = CswNbtObjClassRegulatoryListMember.PropertyName.CASNo,
                    //        FieldType = CswEnumNbtFieldType.CASNo,
                    //        ServerManaged = true
                    //    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.Chemical,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = ChemicalOC.ObjectClassId,
                            ServerManaged = true
                        } );
                    //_CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                    //    {
                    //        PropName = CswNbtObjClassRegulatoryListMember.PropertyName.Exclusive,
                    //        FieldType = CswEnumNbtFieldType.Logical,
                    //        IsRequired = true,
                    //        ServerManaged = true
                    //    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.ByUser,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            ServerManaged = true,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = UserOC.ObjectClassId
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = RegListOC.ObjectClassId
                        } );
                    //_CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                    //    {
                    //        PropName = CswNbtObjClassRegulatoryListMember.PropertyName.Show,
                    //        FieldType = CswEnumNbtFieldType.Logical,
                    //        IsRequired = true,
                    //        ServerManaged = true
                    //    } );

                    _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.RegulatoryLists, RegListMemberOC.ObjectClassId );
                } // if( null == RegListMemberOC )
            } // if( null != ChemicalOC && null != RegListOC )
            _resetBlame();
        } // _addRegulatoryListMemberOC()

        private void _renameRegListCasNoMemo( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            string deprecatedPropName = "CAS Numbers";
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                CswNbtMetaDataObjectClassProp RegListCasNosOCP = RegListOC.getObjectClassProp( deprecatedPropName );
                if( null != RegListCasNosOCP )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RegListCasNosOCP, CswEnumNbtObjectClassPropAttributes.propname, CswNbtObjClassRegulatoryList.PropertyName.AddCASNumbers );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RegListCasNosOCP, CswEnumNbtObjectClassPropAttributes.isrequired, false );
                }
            }

            _resetBlame();
        } // _renameRegListCasNoMemo()


        private void _addRegListCasNosGrid( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                // Exclusive prop
                CswNbtMetaDataObjectClassProp RegListExclusiveOCP = RegListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.Exclusive );
                if( null == RegListExclusiveOCP )
                {
                    RegListExclusiveOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                        {
                            PropName = CswNbtObjClassRegulatoryList.PropertyName.Exclusive,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );

                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RegListExclusiveOCP, CswConvert.ToDbVal( CswEnumTristate.False ) );

                } // if( null == RegListExclusiveOCP )

                // Grid property
                CswNbtMetaDataObjectClassProp RegListCASNosGridOCP = RegListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.CASNosGrid );
                if( null == RegListCASNosGridOCP )
                {
                    RegListCASNosGridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                        {
                            PropName = CswNbtObjClassRegulatoryList.PropertyName.CASNosGrid,
                            FieldType = CswEnumNbtFieldType.Grid
                        } );

                    CswNbtMetaDataObjectClass RegListCasNoOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListCasNoClass );
                    if( null != RegListCasNoOC )
                    {
                        CswNbtMetaDataObjectClassProp RegListCasNoRegListOCP = RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList );

                        // Grid View
                        CswNbtView RegListCasNosView = _CswNbtSchemaModTrnsctn.makeView();
                        RegListCasNosView.ViewName = CswNbtObjClassRegulatoryList.PropertyName.CASNosGrid;
                        RegListCasNosView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                        CswNbtViewRelationship RegListRel = RegListCasNosView.AddViewRelationship( RegListOC, false );
                        CswNbtViewRelationship MemberRel = RegListCasNosView.AddViewRelationship( RegListRel, CswEnumNbtViewPropOwnerType.Second, RegListCasNoRegListOCP, true );
                        CswNbtViewProperty CasNoVP = RegListCasNosView.AddViewProperty( MemberRel, RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo ), 1 );
                        RegListCasNosView.AddViewProperty( MemberRel, RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.TPQ ), 2 );
                        CswNbtViewProperty ErrorVP = RegListCasNosView.AddViewProperty( MemberRel, RegListCasNoOC.getObjectClassProp( CswNbtObjClassRegulatoryListCasNo.PropertyName.ErrorMessage ), 3 );
                        RegListCasNosView.setSortProperty( ErrorVP, CswEnumNbtViewPropertySortMethod.Ascending );
                        RegListCasNosView.setSortProperty( CasNoVP, CswEnumNbtViewPropertySortMethod.Ascending, false );
                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RegListCASNosGridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, RegListCasNosView.ToString() );
                    }
                } // if( null == RegListCASNosGridOCP )
            } // if( null != RegListOC )
            _resetBlame();
        } // _addCasMembersGrid()

        private void _removeChemicalRegListsProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp ChemicalRegListGridOCP = ChemicalOC.getObjectClassProp( "Regulatory Lists" );
            if( null != ChemicalRegListGridOCP && ChemicalRegListGridOCP.getFieldTypeValue() == CswEnumNbtFieldType.Static )
            {
                _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( ChemicalRegListGridOCP, true );
            }

            _resetBlame();
        } // _removeChemicalRegListsProp

        private void _addRegListGridToChemical( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != ChemicalOC )
            {
                // Grid property
                CswNbtMetaDataObjectClassProp ChemicalRegListGridOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.RegulatoryListsGrid );
                if( null == ChemicalRegListGridOCP )
                {
                    ChemicalRegListGridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                        {
                            PropName = CswNbtObjClassChemical.PropertyName.RegulatoryListsGrid,
                            FieldType = CswEnumNbtFieldType.Grid,
                            Extended = CswEnumNbtGridPropMode.Small.ToString()
                        } );

                    CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
                    if( null != RegListMemberOC )
                    {
                        CswNbtMetaDataObjectClassProp RegListMemberChemicalOCP = RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.Chemical );

                        // Grid View
                        CswNbtView ChemicalRegListView = _CswNbtSchemaModTrnsctn.makeView();
                        ChemicalRegListView.ViewName = CswNbtObjClassRegulatoryList.PropertyName.CASNosGrid;
                        ChemicalRegListView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                        CswNbtViewRelationship RegListRel = ChemicalRegListView.AddViewRelationship( ChemicalOC, false );
                        CswNbtViewRelationship MemberRel = ChemicalRegListView.AddViewRelationship( RegListRel, CswEnumNbtViewPropOwnerType.Second, RegListMemberChemicalOCP, true );
                        ChemicalRegListView.AddViewProperty( MemberRel, RegListMemberOC.getObjectClassProp( CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList ), 1 );

                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ChemicalRegListGridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, ChemicalRegListView.ToString() );
                    }
                } // if( null == ChemicalRegListGridOCP )
            } // if( null != ChemicalOC )

            _resetBlame();
        } // _addRegListGridToChemical()

        private void _addSuppressedRegListsToChemical( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != ChemicalOC )
            {
                // Suppressed Regulatory Lists property
                CswNbtMetaDataObjectClassProp ChemicalSuppressedOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.SuppressedRegulatoryLists );
                if( null == ChemicalSuppressedOCP )
                {
                    ChemicalSuppressedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
                    {
                        PropName = CswNbtObjClassChemical.PropertyName.SuppressedRegulatoryLists,
                        FieldType = CswEnumNbtFieldType.Memo,
                        ServerManaged = true
                    } );

                } // if( null == ChemicalRegListGridOCP )
            } // if( null != ChemicalOC )

            _resetBlame();
        } // _addRegListGridToChemical()

        #endregion CEDAR Methods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region CEDAR

            _makeLocationNameRequired( new UnitOfBlame( CswEnumDeveloper.BV, 29519 ) );
            _updateGHSPhraseCategoriesAndLanguages( new UnitOfBlame( CswEnumDeveloper.BV, 29717 ) );
            _updateContainerLabelFormatViewXML( new UnitOfBlame( CswEnumDeveloper.BV, 29716 ) );
            _updatePPEOptions( new UnitOfBlame( CswEnumDeveloper.CM, 29566 ) );
            _addViewCofAButtons( new UnitOfBlame( CswEnumDeveloper.BV, 29563 ) );
            _createDocumentObjClasses( new UnitOfBlame( CswEnumDeveloper.BV, 29833 ) );
            _createDocumentPropertySet( new UnitOfBlame( CswEnumDeveloper.BV, 29833 ) );
            _addIsConstituentProperty( new UnitOfBlame( CswEnumDeveloper.SS, 29680 ) );
            _removeChemicalRegListsProp( new UnitOfBlame( CswEnumDeveloper.SS, 29612 ) );
            _addRegulatoryListCasNoOC( new UnitOfBlame( CswEnumDeveloper.SS, 29487 ) );
            _addRegulatoryListMemberOC( new UnitOfBlame( CswEnumDeveloper.SS, 29488 ) );
            _renameRegListCasNoMemo( new UnitOfBlame( CswEnumDeveloper.SS, 29610 ) );
            _addRegListCasNosGrid( new UnitOfBlame( CswEnumDeveloper.SS, 29610 ) );
            _addRegListGridToChemical( new UnitOfBlame( CswEnumDeveloper.SS, 29612 ) );
            _addSuppressedRegListsToChemical( new UnitOfBlame( CswEnumDeveloper.SS, 28303 ) );

            #endregion CEDAR

            #region DOGWOOD

            _addRegulatoryListListCodeOC( new UnitOfBlame( CswEnumDeveloper.CM, 30008 ) );
            _addRegListLOLIListCodesGrid( new UnitOfBlame( CswEnumDeveloper.CM, 30010 ) );
            _addRegListListModeProp( new UnitOfBlame( CswEnumDeveloper.CM, 30010 ) );
            _addPropFiltertoAddCASNosProp( new UnitOfBlame( CswEnumDeveloper.CM, 30010 ) );

            #endregion DOGWOOD

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()

        #region DOGWOOD Methods

        private void _addRegulatoryListListCodeOC( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
                if( null == RegListListCodeOC )
                {
                    // Create the object class
                    RegListListCodeOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass, "doc.png", false );

                    // Create the properties
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListListCodeOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = RegListOC.ObjectClassId,
                        IsCompoundUnique = true,
                        ReadOnly = true,
                        DisplayRowAdd = 1,
                        DisplayColAdd = 1
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListListCodeOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = "",
                        SetValOnAdd = true,
                        DisplayRowAdd = 2,
                        DisplayColAdd = 1
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListListCodeOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListCode,
                        FieldType = CswEnumNbtFieldType.Number,
                        ServerManaged = true
                    } );

                    // Tie to the Regulatory Lists module
                    _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.RegulatoryLists, RegListListCodeOC.ObjectClassId );

                } // if( null == RegListCasListCode )
            } // if( null != RegListOC )
            _resetBlame();
        } // _addRegulatoryListListCodeOC

        private void _addRegListLOLIListCodesGrid( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                // Grid property
                CswNbtMetaDataObjectClassProp RegListLOLIListCodesGridOCP = RegListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.LOLIListCodes );
                if( null == RegListLOLIListCodesGridOCP )
                {
                    RegListLOLIListCodesGridOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                    {
                        PropName = CswNbtObjClassRegulatoryList.PropertyName.LOLIListCodes,
                        FieldType = CswEnumNbtFieldType.Grid
                    } );

                    CswNbtMetaDataObjectClass RegListListCodeOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListListCodeClass );
                    if( null != RegListListCodeOC )
                    {
                        CswNbtMetaDataObjectClassProp RegListListCodeRegListOCP = RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.RegulatoryList );

                        // Grid View
                        CswNbtView RegListListCodesView = _CswNbtSchemaModTrnsctn.makeView();
                        RegListListCodesView.ViewName = CswNbtObjClassRegulatoryList.PropertyName.LOLIListCodes;
                        RegListListCodesView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                        CswNbtViewRelationship RegListRel = RegListListCodesView.AddViewRelationship( RegListOC, false );
                        CswNbtViewRelationship MemberRel = RegListListCodesView.AddViewRelationship( RegListRel, CswEnumNbtViewPropOwnerType.Second, RegListListCodeRegListOCP, true );
                        CswNbtViewProperty LOLIListNameVP = RegListListCodesView.AddViewProperty( MemberRel, RegListListCodeOC.getObjectClassProp( CswNbtObjClassRegulatoryListListCode.PropertyName.LOLIListName ), 1 );
                        _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( RegListLOLIListCodesGridOCP, CswEnumNbtObjectClassPropAttributes.viewxml, RegListListCodesView.ToString() );
                    }
                } // if( null == RegListListCodesGridOCP )
            } // if( null != RegListOC )
            _resetBlame();
        } // _addLoliCodesGrid()

        private void _addRegListListModeProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegListOC )
            {
                CswNbtMetaDataObjectClassProp FileTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListOC )
                {
                    PropName = CswNbtObjClassRegulatoryList.PropertyName.ListMode,
                    FieldType = CswEnumNbtFieldType.List,
                    ListOptions = CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.Options.ToString(),
                    IsRequired = true,
                    SetValOnAdd = true,

                } );
            }//if (null != RegListOC)

            _resetBlame();
        }// _addRegListListModeProp()

        private void _addPropFiltertoAddCASNosProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass RegulatoryListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != RegulatoryListOC )
            {
                CswNbtMetaDataObjectClassProp ListModeOCP = RegulatoryListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.ListMode );
                CswNbtMetaDataObjectClassProp AddCASNumbersOCP = RegulatoryListOC.getObjectClassProp( CswNbtObjClassRegulatoryList.PropertyName.AddCASNumbers );
                AddCASNumbersOCP.setFilter( FilterProp: ListModeOCP,
                                            SubField: ListModeOCP.getFieldTypeRule().SubFields.Default,
                                            FilterMode: CswEnumNbtFilterMode.Equals,
                                            FilterValue: CswNbtObjClassRegulatoryList.CswEnumRegulatoryListListModes.ManuallyManaged );

            }

            _resetBlame();
        }

        #endregion DOGWOOD Methods

    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema


