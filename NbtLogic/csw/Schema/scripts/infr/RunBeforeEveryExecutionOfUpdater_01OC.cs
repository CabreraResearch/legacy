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

        #region ASPEN Methods

        private void _addSaveProperty( UnitOfBlame Blamne )
        {
            _acceptBlame( Blamne );

            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp SaveOcp = ObjectClass.getObjectClassProp( CswNbtObjClass.PropertyName.Save );
                if( null == SaveOcp )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ObjectClass )
                        {
                            PropName = CswNbtObjClass.PropertyName.Save,
                            FieldType = CswEnumNbtFieldType.Button,
                            Extended = CswNbtNodePropButton.ButtonMode.button
                        } );
                }
            }

            _resetBlame();
        }

        private void _createAssemblyBarcodeProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = (CswNbtMetaDataObjectClassProp) assemblyOC.getBarcodeProperty();
            if( null == barcodeOCP )
            {
                barcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( assemblyOC )
                {
                    PropName = CswNbtObjClassEquipmentAssembly.PropertyName.Barcode,
                    FieldType = CswEnumNbtFieldType.Barcode,
                    IsUnique = true
                } );
            }

            _resetBlame();
        }

        private void _upgradeEquipmentBarcodeProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = (CswNbtMetaDataObjectClassProp) equipmentOC.getBarcodeProperty();
            if( null == barcodeOCP )
            {
                barcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( equipmentOC )
                {
                    PropName = CswNbtObjClassEquipment.PropertyName.EquipmentId,
                    FieldType = CswEnumNbtFieldType.Barcode,
                    IsUnique = true
                } );
            }

            _resetBlame();
        }

        private void _makeC3ProductIdProperty( CswEnumDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != MaterialOC )
            {
                // Add property to material object class
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                    {
                        PropName = CswNbtObjClassChemical.PropertyName.C3ProductId,
                        FieldType = CswEnumNbtFieldType.Text,
                        IsRequired = false,
                        ReadOnly = true,
                        ServerManaged = true
                    } );

                // Now add the property to all material nodetypes
                _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp C3ProductIdProp = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassChemical.PropertyName.C3ProductId );
                    C3ProductIdProp.removeFromAllLayouts();
                }

            }

            _resetBlame();
        }

        private void _upgradeAssemblyAndEquipmentLocationProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp locationOCP = equipmentOC.getObjectClassProp( CswNbtObjClassEquipment.PropertyName.Location );
            if( null == locationOCP )
            {
                locationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( equipmentOC )
                {
                    PropName = CswNbtObjClassEquipment.PropertyName.Location,
                    FieldType = CswEnumNbtFieldType.Location
                } );
            }

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass );
            locationOCP = assemblyOC.getObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Location );
            if( null == locationOCP )
            {
                locationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( assemblyOC )
                {
                    PropName = CswNbtObjClassEquipmentAssembly.PropertyName.Location,
                    FieldType = CswEnumNbtFieldType.Location
                } );
            }

            _resetBlame();
        }

        private void _upgradeAssemblyStatusProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass );
            CswNbtMetaDataObjectClassProp statusOCP = assemblyOC.getObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Status );
            if( null == statusOCP )
            {
                statusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( assemblyOC )
                {
                    PropName = CswNbtObjClassEquipmentAssembly.PropertyName.Status,
                    FieldType = CswEnumNbtFieldType.List,
                } );
            }

            _resetBlame();
        }

        private void _createReportInstructionsProp( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            CswNbtMetaDataObjectClass reportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp instructionsOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Instructions );
            if( null == instructionsOCP )
            {
                instructionsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( reportOC )
                {
                    PropName = CswNbtObjClassReport.PropertyName.Instructions,
                    FieldType = CswEnumNbtFieldType.Memo,
                    ServerManaged = true
                } );

                string txt = @"To create a parameterized report, enclose the name of the property in {}.  For example:
    and datecolumn < '{Date}'
will prompt the user to enter a Date. Parameters that match properties on the current User will be automatically filled in. For example:
    {Username} - The username of the user running the report.
    {Role} - The role of the user running the report.
    {userid} - The primary key of the user running the report.";

                _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( instructionsOCP, txt );

            }

            _resetBlame();
        }

        private void _fixHazardClassSpellingAndAddNewClasses( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.FireClassExemptAmountClass );
            if( null != FireClassExemptAmountOC )
            {
                CswNbtMetaDataObjectClassProp HazardClassOCP = FireClassExemptAmountOC.getObjectClassProp( CswNbtObjClassFireClassExemptAmount.PropertyName.HazardClass );
                if( null != HazardClassOCP )
                {
                    CswCommaDelimitedString FireHazardClassTypes = new CswCommaDelimitedString
                        {
                            "Aero-1",
                            "Aero-2",
                            "Aero-3",
                            "Carc",
                            "CF/D (bailed)",
                            "CF/D (loose)",
                            "CL-II",
                            "CL-IIIA",
                            "CL-IIIB",
                            "Corr",
                            "Corr (liquified gas)",
                            "CRY-FG",
                            "CRY-NFG",
                            "CRY-OXY",
                            "Exp",
                            "Exp-1.1",
                            "Exp-1.2",
                            "Exp-1.3",
                            "Exp-1.4",
                            "Exp-1.4G",
                            "Exp-1.5",
                            "Exp-1.6",
                            "FG (gaseous)",
                            "FG (liquified)",
                            "FL-1A",
                            "FL-1B",
                            "FL-1C",
                            "FL-Comb",
                            "FS",
                            "H.T.",
                            "H.T. (liquified gas)",
                            "Irr",
                            "N/R",
                            "NFG",
                            "NFG (liquified)",
                            "NFS",
                            "OHH",
                            "Oxy-1",
                            "Oxy-2",
                            "Oxy-3",
                            "Oxy-4",
                            "Oxy-Gas",
                            "Oxy-Gas (liquid)",
                            "Perox-Det",
                            "Perox-I",
                            "Perox-II",
                            "Perox-III",
                            "Perox-IV",
                            "Perox-V",
                            "Pyro",
                            "RAD-Alpha",
                            "RAD-Beta",
                            "RAD-Gamma",
                            "Sens",
                            "Tox",
                            "Tox (liquified gas)",
                            "UR-1",
                            "UR-2",
                            "UR-3",
                            "UR-4",
                            "WR-1",
                            "WR-2",
                            "WR-3"
                        };

                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( HazardClassOCP, CswEnumNbtObjectClassPropAttributes.listoptions, FireHazardClassTypes.ToString() );
                }

            }


            _resetBlame();
        }



        private void _createMaterialC3SyncDataProp( UnitOfBlame Blame )
        {
            // Add the C3SyncData property to the Material Object Class
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            if( null != MaterialOC )
            {
                CswNbtMetaDataObjectClassProp C3SyncDateOCP = MaterialOC.getObjectClassProp( CswNbtPropertySetMaterial.PropertyName.C3SyncDate );
                if( null == C3SyncDateOCP )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                        {
                            PropName = CswNbtPropertySetMaterial.PropertyName.C3SyncDate,
                            FieldType = CswEnumNbtFieldType.DateTime,
                            ServerManaged = true,
                            ReadOnly = true
                        } );
                }

                // Add the C3SyncData property to all Material NodeTypes
                _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

                // Remove from all layouts
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp C3SyncDateNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtPropertySetMaterial.PropertyName.C3SyncDate );
                    C3SyncDateNTP.removeFromAllLayouts();
                }
            }
        }


        private void _addAssignIvgButton( UnitOfBlame Blame )
        {

            _acceptBlame( Blame );
            CswNbtMetaDataObjectClass InventoryGroupOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryGroupClass );
            if( null != InventoryGroupOC )
            {
                CswNbtMetaDataObjectClassProp AssignLocationButtonOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( InventoryGroupOC, new CswNbtWcfMetaDataModel.ObjectClassProp()
                {
                    FieldType = CswEnumNbtFieldType.Button,
                    PropName = CswNbtObjClassInventoryGroup.PropertyName.AssignLocation
                } );

            }//if we found the ing ocp

            _resetBlame();

        }//_addAssignIvgButton()

        private void _createHazardClassProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp HazardClassOCP = MaterialOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.HazardClasses );
            if( null == HazardClassOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                {
                    PropName = CswNbtObjClassChemical.PropertyName.HazardClasses,
                    FieldType = CswEnumNbtFieldType.MultiList
                } );
            }

            _resetBlame();
        }

        #endregion ASPEN Methods

        #region BUCKEYE Methods
        
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
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ChemicalOC )
            {
                PropName = CswNbtObjClassChemical.PropertyName.Jurisdiction,
                FieldType = CswEnumNbtFieldType.ChildContents,
                IsFk = true,
                FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = GHSOC.ObjectClassId
            } );

            _resetBlame();
        }

        #endregion Case 28690

        #endregion BUCKEYE Methods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            //This ASPEN method has to be first
            _addSaveProperty( new UnitOfBlame( CswEnumDeveloper.CF, 27923 ) );

            #region ASPEN

            _makeC3ProductIdProperty( CswEnumDeveloper.CM, 28688 );
            _createAssemblyBarcodeProp( new UnitOfBlame( CswEnumDeveloper.MB, 29108 ) );
            _upgradeEquipmentBarcodeProp( new UnitOfBlame( CswEnumDeveloper.MB, 29108 ) );
            _upgradeAssemblyAndEquipmentLocationProp( new UnitOfBlame( CswEnumDeveloper.MB, 28648 ) );
            _upgradeAssemblyStatusProp( new UnitOfBlame( CswEnumDeveloper.MB, 28648 ) );
            _createReportInstructionsProp( new UnitOfBlame( CswEnumDeveloper.MB, 28950 ) );
            _fixHazardClassSpellingAndAddNewClasses( new UnitOfBlame( CswEnumDeveloper.CM, 29243 ) );
            _createMaterialC3SyncDataProp( new UnitOfBlame( CswEnumDeveloper.CM, 29246 ) );
            _addAssignIvgButton( new UnitOfBlame( CswEnumDeveloper.PG, 28927 ) );
            _createHazardClassProp( new UnitOfBlame( CswEnumDeveloper.CM, 29245 ) );

            #endregion ASPEN

            #region BUCKEYE
            
            _ghsPictos( new UnitOfBlame( CswEnumDeveloper.SS, 28778 ) );
            _createNonChemicalObjClass( new UnitOfBlame( CswEnumDeveloper.BV, 28690 ) );
            _promoteChemicalNTPsToOCPs( new UnitOfBlame( CswEnumDeveloper.BV, 28690 ) );
            _createMaterialPropertySet( new UnitOfBlame( CswEnumDeveloper.BV, 28690 ) );

            #endregion BUCKEYE

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()
    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema


