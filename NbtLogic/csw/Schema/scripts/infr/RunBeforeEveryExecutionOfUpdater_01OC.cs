using System;
using System.Data;
using ChemSW.Audit;
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
    public class RunBeforeEveryExecutionOfUpdater_01OC: CswUpdateSchemaTo
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
        } // _ghsPictos()

        #region Case 28690

        private void _createMaterialPropertySet( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataPropertySet MaterialPS = _CswNbtSchemaModTrnsctn.MetaData.makeNewPropertySet( CswEnumNbtPropertySetName.MaterialSet, "atom.png" );

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

        private void _listText( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            // Add 'Text' subfield for Lists
            CswTableUpdate JctUpdate = _CswNbtSchemaModTrnsctn.makeCswTableUpdate( "updateLists_jnp", "jct_nodes_props" );
            string WhereClause = @"where jctnodepropid in (select j.jctnodepropid
                                                             from jct_nodes_props j
                                                             join nodetype_props p on j.nodetypepropid = p.nodetypepropid
                                                             join field_types f on p.fieldtypeid = f.fieldtypeid
                                                            where f.fieldtype = 'List' and j.field2 is null)";
            DataTable JctTable = JctUpdate.getTable( WhereClause );
            foreach( DataRow JctRow in JctTable.Rows )
            {
                JctRow["field2"] = JctRow["field1"];
            }
            JctUpdate.update( JctTable );

            _resetBlame();
        }

        private void _designObjectClasses( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            if( null == _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass ) )
            {
                CswNbtMetaDataObjectClass NodeTypeOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignNodeTypeClass, "wrench.png", true );
                CswNbtMetaDataObjectClass PropOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignNodeTypePropClass, "wrench.png", true );
                CswNbtMetaDataObjectClass TabOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.DesignNodeTypeTabClass, "wrench.png", true );

                // DesignNodeType
                {
                    CswNbtMetaDataObjectClassProp AuditLevelOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.AuditLevel,
                            FieldType = CswEnumNbtFieldType.List,
                            ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumAuditLevel.NoAudit.ToString(),
                                    CswEnumAuditLevel.PlainAudit.ToString()
                                }.ToString(),
                            IsRequired = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.Category,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                    CswNbtMetaDataObjectClassProp DeferSearchToOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.DeferSearchTo,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsRequired = false,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = PropOC.ObjectClassId
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.IconFileName,
                            FieldType = CswEnumNbtFieldType.ImageList,
                            Extended = false.ToString(),
                            TextAreaRows = 16,
                            TextAreaColumns = 16,
                            IsRequired = true
                        } );
                    CswNbtMetaDataObjectClassProp LockedOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.Locked,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.NameTemplate,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.NameTemplateAdd,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = PropOC.ObjectClassId
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.NodeTypeName,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                    //_CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                    //{
                    //    PropName = CswNbtObjClassDesignNodeType.PropertyName.ObjectClassName,
                    //    FieldType = CswEnumNbtFieldType.Text,
                    //    ServerManaged = true
                    //} );
                    CswNbtMetaDataObjectClassProp ObjectClassValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( NodeTypeOC )
                        {
                            PropName = CswNbtObjClassDesignNodeType.PropertyName.ObjectClass,
                            FieldType = CswEnumNbtFieldType.List,
                            ReadOnly = true,
                            IsRequired = true
                        } );

                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AuditLevelOCP, CswEnumAuditLevel.NoAudit.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( LockedOCP, CswConvert.ToDbVal( CswEnumTristate.False.ToString() ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ObjectClassValueOCP, CswConvert.ToDbVal( _CswNbtSchemaModTrnsctn.MetaData.getObjectClassId( CswEnumNbtObjectClass.GenericClass ) ) );
                }

                // DesignNodeTypeProp
                {
                    CswNbtMetaDataObjectClassProp AuditLevelOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.AuditLevel,
                            FieldType = CswEnumNbtFieldType.List,
                            ListOptions = new CswCommaDelimitedString()
                                {
                                    CswEnumAuditLevel.NoAudit.ToString(),
                                    CswEnumAuditLevel.PlainAudit.ToString()
                                }.ToString(),
                            IsRequired = true
                        } );
                    CswNbtMetaDataObjectClassProp CompoundUniqueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.CompoundUnique,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );
                    CswNbtMetaDataObjectClassProp DisplayConditionFilterOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionFilter,
                            FieldType = CswEnumNbtFieldType.List
                        } );
                    CswNbtMetaDataObjectClassProp DisplayConditionPropertyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionProperty,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = PropOC.ObjectClassId
                        } );
                    CswNbtMetaDataObjectClassProp DisplayConditionSubfieldOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionSubfield,
                            FieldType = CswEnumNbtFieldType.List
                        } );
                    CswNbtMetaDataObjectClassProp DisplayConditionValueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.DisplayConditionValue,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.FieldType,
                            FieldType = CswEnumNbtFieldType.List,
                            ReadOnly = true,
                            IsRequired = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.HelpText,
                            FieldType = CswEnumNbtFieldType.Memo
                        } );
                    CswNbtMetaDataObjectClassProp NodeTypeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.NodeTypeValue,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = NodeTypeOC.ObjectClassId,
                            ReadOnly = true,
                            IsRequired = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.ObjectClassPropName,
                            FieldType = CswEnumNbtFieldType.List,
                            ServerManaged = true
                        } );
                    CswNbtMetaDataObjectClassProp PropNameOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.PropName,
                            FieldType = CswEnumNbtFieldType.Text
                        } );
                    CswNbtMetaDataObjectClassProp ReadOnlyOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.ReadOnly,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );
                    CswNbtMetaDataObjectClassProp RequiredOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.Required,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );
                    CswNbtMetaDataObjectClassProp UniqueOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.Unique,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );
                    CswNbtMetaDataObjectClassProp UseNumberingOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( PropOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeProp.PropertyName.UseNumbering,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );


                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( AuditLevelOCP, CswEnumAuditLevel.NoAudit.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( CompoundUniqueOCP, CswConvert.ToDbVal( CswEnumTristate.False.ToString() ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( UseNumberingOCP, CswConvert.ToDbVal( CswEnumTristate.False.ToString() ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ReadOnlyOCP, CswConvert.ToDbVal( CswEnumTristate.False.ToString() ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( RequiredOCP, CswConvert.ToDbVal( CswEnumTristate.False.ToString() ) );
                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( UniqueOCP, CswConvert.ToDbVal( CswEnumTristate.False.ToString() ) );

                    // Display condition view includes all properties on the same nodetype
                    CswNbtView DispCondView = _CswNbtSchemaModTrnsctn.makeView();
                    CswNbtViewRelationship PropRel1 = DispCondView.AddViewRelationship( PropOC, false );
                    CswNbtViewRelationship TypeRel2 = DispCondView.AddViewRelationship( PropRel1, CswEnumNbtViewPropOwnerType.First, NodeTypeOCP, false );
                    CswNbtViewRelationship PropRel3 = DispCondView.AddViewRelationship( TypeRel2, CswEnumNbtViewPropOwnerType.Second, NodeTypeOCP, false );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionPropertyOCP, CswEnumNbtObjectClassPropAttributes.viewxml, DispCondView.ToXml().InnerXml );

                    // Display condition filters rely on a property being selected
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionSubfieldOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, DisplayConditionPropertyOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionFilterOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, DisplayConditionPropertyOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionValueOCP, CswEnumNbtObjectClassPropAttributes.filterpropid, DisplayConditionPropertyOCP.ObjectClassPropId );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionSubfieldOCP, CswEnumNbtObjectClassPropAttributes.filter, "Field1_Fk|NotNull" );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionFilterOCP, CswEnumNbtObjectClassPropAttributes.filter, "Field1_Fk|NotNull" );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DisplayConditionValueOCP, CswEnumNbtObjectClassPropAttributes.filter, "Field1_Fk|NotNull" );
                }

                // DesignNodeTypeTab
                {
                    CswNbtMetaDataObjectClassProp IncludeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.IncludeInReport,
                            FieldType = CswEnumNbtFieldType.Logical,
                            IsRequired = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.NodeTypeValue,
                            FieldType = CswEnumNbtFieldType.Relationship,
                            IsFk = true,
                            FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                            FkValue = NodeTypeOC.ObjectClassId,
                            ReadOnly = true,
                            IsRequired = true
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.Order,
                            FieldType = CswEnumNbtFieldType.Number
                        } );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( TabOC )
                        {
                            PropName = CswNbtObjClassDesignNodeTypeTab.PropertyName.TabName,
                            FieldType = CswEnumNbtFieldType.Text
                        } );

                    _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IncludeOCP, CswConvert.ToDbVal( CswEnumTristate.True.ToString() ) );
                }
            }
            _resetBlame();
        } // _designObjectClasses()

        public void _metaDataListFieldType( UnitOfBlame blameMe )
        {
            _acceptBlame( blameMe );
            
            _CswNbtSchemaModTrnsctn.MetaData.makeNewFieldType( CswEnumNbtFieldType.MetaDataList, CswEnumNbtFieldTypeDataType.INTEGER );
            
            _resetBlame();
        }

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

            #endregion BUCKEYE

            #region CEDAR

            _makeLocationNameRequired( new UnitOfBlame( CswEnumDeveloper.BV, 29519 ) );
            _metaDataListFieldType( new UnitOfBlame( CswEnumDeveloper.SS, 29311 ) );
            _listText( new UnitOfBlame( CswEnumDeveloper.SS, 29311 ) );
            _designObjectClasses( new UnitOfBlame( CswEnumDeveloper.SS, 29311 ) );

            #endregion CEDAR

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()
    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema


