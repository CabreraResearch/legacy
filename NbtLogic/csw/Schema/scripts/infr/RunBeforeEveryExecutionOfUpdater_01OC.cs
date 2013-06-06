using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
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

        #region BUCKEYE Methods

        private void _createUOMProp( CswEnumDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            if( null != UnitOfMeasureOC )
            {
                // Add property to the unit of measure object class
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UnitOfMeasureOC )
                {
                    PropName = CswNbtObjClassUnitOfMeasure.PropertyName.Aliases,
                    FieldType = CswEnumNbtFieldType.Memo,
                    ReadOnly = true
                } );
            }

            _resetBlame();
        }


        private void _correctPrinterEnabledDefaultValue( UnitOfBlame Blamne )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass PrinterOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.PrinterClass );
            CswNbtMetaDataObjectClassProp EnabledOcp = PrinterOc.getObjectClassProp( CswNbtObjClassPrinter.PropertyName.Enabled );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( EnabledOcp, CswEnumTristate.True );

            _resetBlame();

        }

        private void _ghsPictos( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            CswNbtMetaDataObjectClass GhsOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            if( null != GhsOc )
            {
                if( null == GhsOc.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Pictograms ) )
                {
                    CswDelimitedString PictoNames = new CswDelimitedString( '\n' )
                        {
                            "Oxidizer",
                            "Flammable",
                            "Explosive",
                            "Acute Toxicity (severe)",
                            "Corrosive",
                            "Gases Under Pressure",
                            "Target Organ Toxicity",
                            "Environmental Toxicity",
                            "Irritant"
                        };
                    CswDelimitedString PictoPaths = new CswDelimitedString( '\n' )
                        {
                            "Images/cispro/ghs/rondflam.jpg",
                            "Images/cispro/ghs/flamme.jpg",
                            "Images/cispro/ghs/explos.jpg",
                            "Images/cispro/ghs/skull.jpg",
                            "Images/cispro/ghs/acid.jpg",
                            "Images/cispro/ghs/bottle.jpg",
                            "Images/cispro/ghs/silhouet.jpg",
                            "Images/cispro/ghs/pollut.jpg",
                            "Images/cispro/ghs/exclam.jpg"
                        };

                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp()
                        {
                            ObjectClass = GhsOc,
                            FieldType = CswEnumNbtFieldType.ImageList,
                            PropName = CswNbtObjClassGHS.PropertyName.Pictograms,
                            ListOptions = PictoNames.ToString(),
                            ValueOptions = PictoPaths.ToString(),
                            Extended = "true",
                            TextAreaColumns = 77,
                            TextAreaRows = 77
                        } );
                } //  if( null != GhsOc )
            } // if( null == GhsOc.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Pictograms ) )

            _resetBlame();
        }

        #region Case 28690

        private void _createMaterialPropertySet( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.getPropertySet( CswEnumNbtPropertySetName.MaterialSet );
            if( null == MaterialPS )
            {
                MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropertySet( CswEnumNbtPropertySetName.MaterialSet, "atom.png" );

                //Update jct_propertyset_objectclass
                CswTableUpdate JctPSOCUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_jctpsoc_update", "jct_propertyset_objectclass" );
                DataTable JctPSOCTable = JctPSOCUpdate.getEmptyTable();
                _addObjClassToPropertySetMaterial( JctPSOCTable, CswEnumNbtObjectClass.ChemicalClass, MaterialPS.PropertySetId );
                _addObjClassToPropertySetMaterial( JctPSOCTable, CswEnumNbtObjectClass.NonChemicalClass, MaterialPS.PropertySetId );
                JctPSOCUpdate.update( JctPSOCTable );

                //Update jct_propertyset_ocprop
                CswTableUpdate JctPSOCPUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "28690_jctpsocp_update", "jct_propertyset_ocprop" );
                DataTable JctPSOCPTable = JctPSOCPUpdate.getEmptyTable();
                _addObjClassPropsToPropertySetMaterial( JctPSOCPTable, CswEnumNbtObjectClass.ChemicalClass, MaterialPS.PropertySetId );
                _addObjClassPropsToPropertySetMaterial( JctPSOCPTable, CswEnumNbtObjectClass.NonChemicalClass, MaterialPS.PropertySetId );
                JctPSOCPUpdate.update( JctPSOCPTable );
            }

            _resetBlame();
        }

        private void _addObjClassToPropertySetMaterial( DataTable JctPSOCTable, string ObjClassName, int PropertySetId )
        {
            DataRow NewJctPSOCRow = JctPSOCTable.NewRow();
            NewJctPSOCRow["objectclassid"] = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( ObjClassName );
            NewJctPSOCRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
            JctPSOCTable.Rows.Add( NewJctPSOCRow );
        }

        private void _addObjClassPropsToPropertySetMaterial( DataTable JctPSOCPTable, string ObjClassName, int PropertySetId )
        {
            CswNbtMetaDataObjectClass MaterialObjectClass = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClassName );
            foreach( CswNbtMetaDataObjectClassProp ObjectClassProp in MaterialObjectClass.getObjectClassProps() )
            {
                bool doInsert = ( ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.MaterialId ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.TradeName ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Supplier ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.PartNumber ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Receive ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.Request ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.C3ProductId ||
                                    ObjectClassProp.PropName == CswNbtPropertySetMaterial.PropertyName.C3SyncDate );
                if( doInsert )
                {
                    DataRow NewJctPSOCPRow = JctPSOCPTable.NewRow();
                    NewJctPSOCPRow["objectclasspropid"] = ObjectClassProp.PropId;
                    NewJctPSOCPRow["propertysetid"] = CswConvert.ToDbVal( PropertySetId );
                    JctPSOCPTable.Rows.Add( NewJctPSOCPRow );
                }
            }
        }

        private void _createNonChemicalObjClass( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass NonChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.NonChemicalClass );
            if( null == NonChemicalOC )
            {
                NonChemicalOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.NonChemicalClass, "atom.png", false );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswEnumNbtModuleName.CISPro, NonChemicalOC.ObjectClassId );
            }
            CswNbtMetaDataObjectClassProp ApprovedForReceivingOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.ApprovedForReceiving,
                FieldType = CswEnumNbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ApprovedForReceivingOCP, CswEnumTristate.False );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.MaterialId,
                FieldType = CswEnumNbtFieldType.Sequence,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.PartNumber,
                FieldType = CswEnumNbtFieldType.Text,
                IsRequired = true,
                IsCompoundUnique = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.Receive,
                FieldType = CswEnumNbtFieldType.Button
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.Request,
                FieldType = CswEnumNbtFieldType.Button,
                Extended = "menu"
            } );
            CswNbtMetaDataObjectClass VendorOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.Supplier,
                FieldType = CswEnumNbtFieldType.Relationship,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = VendorOC.ObjectClassId,
                IsRequired = true,
                IsCompoundUnique = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.TradeName,
                FieldType = CswEnumNbtFieldType.Text,
                IsRequired = true,
                IsCompoundUnique = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.C3ProductId,
                FieldType = CswEnumNbtFieldType.Text,
                ServerManaged = true
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NonChemicalOC )
            {
                PropName = CswNbtPropertySetMaterial.PropertyName.C3SyncDate,
                FieldType = CswEnumNbtFieldType.DateTime,
                ServerManaged = true
            } );

            _resetBlame();
        }

        private void _promoteChemicalNTPsToOCPs( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.NFPA,
                FieldType = CswEnumNbtFieldType.NFPA
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.PPE,
                FieldType = CswEnumNbtFieldType.MultiList,
                ListOptions = @"Goggles,Gloves,Clothing,Fume Hood",
                Extended = ","
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Hazardous,
                FieldType = CswEnumNbtFieldType.Logical
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Formula,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Structure,
                FieldType = CswEnumNbtFieldType.MOL
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.PhysicalDescription,
                FieldType = CswEnumNbtFieldType.Memo
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.MolecularWeight,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Formula,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.pH,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.BoilingPoint,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.MeltingPoint,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.AqueousSolubility,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.FlashPoint,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.VaporPressure,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.VaporDensity,
                FieldType = CswEnumNbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.StorageAndHandling,
                FieldType = CswEnumNbtFieldType.Memo
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Isotope,
                FieldType = CswEnumNbtFieldType.Text
            } );
            CswNbtMetaDataObjectClassProp MaterialTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.MaterialType,
                FieldType = CswEnumNbtFieldType.List,
                IsRequired = true,
                ListOptions = "Pure,Mixture"
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( MaterialTypeOCP, "Pure" );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.SpecialFlags,
                FieldType = CswEnumNbtFieldType.MultiList,
                ListOptions = "EHS,Waste,Not Reportable,Trade Secret"
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.HazardCategories,
                FieldType = CswEnumNbtFieldType.MultiList,
                ListOptions = "F = Fire,C = Chronic (delayed),I = Immediate (acute),R = Reactive,P = Pressure"
            } );
            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            CswNbtMetaDataObjectClassProp GHSMaterialProp = GHSOC.getObjectClassProp( CswNbtObjClassGHS.PropertyName.Material );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Jurisdiction,
                FieldType = CswEnumNbtFieldType.ChildContents,
                IsFk = true,
                FkType = CswEnumNbtViewPropIdType.ObjectClassPropId.ToString(),
                FkValue = GHSMaterialProp.PropId
            } );

            CswNbtMetaDataObjectClassProp PhysicalStateProp = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.PhysicalState );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( PhysicalStateProp, "liquid" );

            _resetBlame();
        }

        #endregion Case 28690

        #region Case 29630

        private void _addImageToInspDesign( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            CswNbtMetaDataObjectClass inspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            CswNbtMetaDataObjectClassProp picturesOCP = inspectionDesignOC.getObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Pictures );
            if( null == picturesOCP )
            {
                picturesOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( inspectionDesignOC )
                    {
                        PropName = CswNbtObjClassInspectionDesign.PropertyName.Pictures,
                        FieldType = CswEnumNbtFieldType.Image
                    } );
            }

            _resetBlame();
        }

        #endregion

        private void _mailReportNameProp( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp MailReportNameOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Name );
            if( null == MailReportNameOCP )
            {
                MailReportNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MailReportOC )
                    {
                        FieldType = CswEnumNbtFieldType.Text,
                        PropName = CswNbtObjClassMailReport.PropertyName.Name,
                        IsRequired = true
                    } );

                // Find and fix existing "Name of Report" properties manually, since they won't match
                CswTableUpdate NtpUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "01OC_updateNTP", "nodetype_props" );
                DataTable NtpTable = NtpUpdate.getTable( "where propname = 'Name of Report' and nodetypeid in (select nodetypeid from nodetypes where objectclassid = " + MailReportOC.ObjectClassId + ")" );
                foreach( DataRow NtpRow in NtpTable.Rows )
                {
                    NtpRow["objectclasspropid"] = MailReportNameOCP.ObjectClassPropId;
                }
                NtpUpdate.update( NtpTable );
            }

            _resetBlame();
        } // _mailReportNameProp()

        #endregion BUCKEYE Methods


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
                    RegListCasNoOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass, "doc.png", false );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.CASNo,
                        FieldType = CswEnumNbtFieldType.CASNo
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.ErrorMessage,
                        FieldType = CswEnumNbtFieldType.Memo
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.IsValid,
                        FieldType = CswEnumNbtFieldType.Logical
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.RegulatoryList,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = RegListOC.ObjectClassId
                    } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListCasNoOC )
                    {
                        PropName = CswNbtObjClassRegulatoryListCasNo.PropertyName.TPQ,
                        FieldType = CswEnumNbtFieldType.Number
                    } );
                }
            }
            _resetBlame();
        } // _addRegulatoryListCasNoOC

        private void _addRegulatoryListMemberOC( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );
            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClass RegListOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListClass );
            if( null != ChemicalOC && null != RegListOC )
            {
                CswNbtMetaDataObjectClass RegListMemberOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass );
                if( null == RegListMemberOC )
                {
                    RegListMemberOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.RegulatoryListMemberClass, "doc.png", false );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.CASNo,
                            FieldType = CswEnumNbtFieldType.CASNo,
                            ServerManaged = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.Chemical,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = ChemicalOC.ObjectClassId,
                            ServerManaged = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.Exclusive,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true,
                            ServerManaged = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.FromUser,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            ServerManaged = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.RegulatoryList,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = RegListOC.ObjectClassId,
                            ServerManaged = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( RegListMemberOC )
                        {
                            PropName = CswNbtObjClassRegulatoryListMember.PropertyName.Show,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true,
                            ServerManaged = true
                        } );
                }
            }
            _resetBlame();
        } // _addRegulatoryListMemberOC()


        #endregion CEDAR Methods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region BUCKEYE

            _createUOMProp( CswEnumDeveloper.CM, 29211 );
            _correctPrinterEnabledDefaultValue( new UnitOfBlame( CswEnumDeveloper.CF, 29397 ) );
            _ghsPictos( new UnitOfBlame( CswEnumDeveloper.SS, 28778 ) );
            _createNonChemicalObjClass( new UnitOfBlame( CswEnumDeveloper.BV, 28690 ) );
            _promoteChemicalNTPsToOCPs( new UnitOfBlame( CswEnumDeveloper.BV, 28690 ) );
            _createMaterialPropertySet( new UnitOfBlame( CswEnumDeveloper.BV, 28690 ) );
            _addImageToInspDesign( new UnitOfBlame( CswEnumDeveloper.MB, 29630 ) );
            _mailReportNameProp( new UnitOfBlame( CswEnumDeveloper.SS, 29773 ) );

            #endregion BUCKEYE

            #region CEDAR

            _makeLocationNameRequired( new UnitOfBlame( CswEnumDeveloper.BV, 29519 ) );
            _updateGHSPhraseCategoriesAndLanguages( new UnitOfBlame( CswEnumDeveloper.BV, 29717 ) );
            _updateContainerLabelFormatViewXML( new UnitOfBlame( CswEnumDeveloper.BV, 29716 ) );
            _updatePPEOptions( new UnitOfBlame( CswEnumDeveloper.CM, 29566 ) );
            _addIsConstituentProperty( new UnitOfBlame( CswEnumDeveloper.SS, 29680 ) );
            _addRegulatoryListCasNoOC( new UnitOfBlame( CswEnumDeveloper.SS, 29487 ) );
            _addRegulatoryListMemberOC( new UnitOfBlame( CswEnumDeveloper.SS, 29488 ) );

            #endregion CEDAR

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()
    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema


