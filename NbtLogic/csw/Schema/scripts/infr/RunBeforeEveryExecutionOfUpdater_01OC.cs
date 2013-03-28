using System;
using ChemSW.Core;
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

        private void _acceptBlame( CswDeveloper BlameMe, Int32 BlameCaseNo )
        {
            _Author = BlameMe;
            _CaseNo = BlameCaseNo;
        }

        private void _resetBlame()
        {
            _Author = CswDeveloper.NBT;
            _CaseNo = 0;
        }

        private CswDeveloper _Author = CswDeveloper.NBT;

        public override CswDeveloper Author
        {
            get { return _Author; }
        }

        private Int32 _CaseNo;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }

        #endregion Blame Logic

        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswNbtMetaDataFieldType.NbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        private static string _makeNodeTypePermissionValue( Int32 FirstVersionNodeTypeId, CswNbtPermit.NodeTypePermission Permission )
        {
            return "nt_" + FirstVersionNodeTypeId.ToString() + "_" + Permission.ToString();
        }

        #region Yorick Methods

        private void _updateUserFormats( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass UserOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            CswNbtMetaDataObjectClassProp DateFormatOcp = UserOc.getObjectClassProp( CswNbtObjClassUser.PropertyName.DateFormat );
            string ValidFormats = CswDateFormat.Mdyyyy + "," + CswDateFormat.dMyyyy + "," + CswDateFormat.yyyyMMdd_Dashes + "," + CswDateFormat.yyyyMd;
            ValidFormats += "," + CswDateFormat.ddMMMyyyy;

            if( DateFormatOcp.ListOptions != ValidFormats )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( DateFormatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, ValidFormats );
                foreach( CswNbtObjClassUser User in UserOc.getNodes( forceReInit: true, includeSystemNodes: false, IncludeDefaultFilters: false ) )
                {
                    if( false == string.IsNullOrEmpty( User.DateFormatProperty.Value ) &&
                        CswResources.UnknownEnum == (CswDateFormat) User.DateFormatProperty.Value )
                    {
                        User.DateFormatProperty.Value = CswDateTime.DefaultDateFormat.ToString();
                    }
                }
            }

            _resetBlame();
        }

        private void _makeUnCode( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            if( null != MaterialOC )
            {

                //first remove existing prop which is of type relationship
                CswNbtMetaDataObjectClassProp UNCodeOCPOld = _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProp( MaterialOC.ObjectClassId, CswNbtObjClassMaterial.PropertyName.UNCode );
                if( null != UNCodeOCPOld )
                {

                    _CswNbtSchemaModTrnsctn.MetaData.DeleteObjectClassProp( UNCodeOCPOld, true );

                }//if we have a un ocp


                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                {
                    PropName = CswNbtObjClassMaterial.PropertyName.UNCode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                    IsRequired = false
                } );

                //now add new prop which is of type text

            }//if we have a material oc

            _resetBlame();
        }

        private void _correctGeneratorTargetTypeProps( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp TargetTypeOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.TargetType );

            //This prop is already server managed, but I think this makes the intention explicit for the reader
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, false );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( TargetTypeOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, false );

            //To prevent the various behaviors associated with changing Owner, make it readonly
            CswNbtMetaDataObjectClassProp OwnerOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.readOnly, true );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.setvalonadd, true );

            _resetBlame();
        }

        #endregion Yorick Methods

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
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.Button,
                            Extended = CswNbtNodePropButton.ButtonMode.button
                        } );
                }
            }

            _resetBlame();
        }

        private void _createAssemblyBarcodeProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = assemblyOC.getBarcodeProp();
            if( null == barcodeOCP )
            {
                barcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( assemblyOC )
                {
                    PropName = CswNbtObjClassEquipmentAssembly.PropertyName.Barcode,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Barcode,
                    IsUnique = true
                } );
            }

            _resetBlame();
        }

        private void _upgradeEquipmentBarcodeProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp barcodeOCP = equipmentOC.getBarcodeProp();
            if( null == barcodeOCP )
            {
                barcodeOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( equipmentOC )
                {
                    PropName = CswNbtObjClassEquipment.PropertyName.EquipmentId,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Barcode,
                    IsUnique = true
                } );
            }

            _resetBlame();
        }

        private void _makeC3ProductIdProperty( CswDeveloper Dev, Int32 Case )
        {
            _acceptBlame( Dev, Case );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            if( null != MaterialOC )
            {
                // Add property to material object class
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                    {
                        PropName = CswNbtObjClassMaterial.PropertyName.C3ProductId,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                        IsRequired = false,
                        ReadOnly = true,
                        ServerManaged = true
                    } );

                // Now add the property to all material nodetypes
                _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp C3ProductIdProp = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.C3ProductId );
                    C3ProductIdProp.removeFromAllLayouts();
                }

            }

            _resetBlame();
        }

        private void _upgradeAssemblyAndEquipmentLocationProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass equipmentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentClass );
            CswNbtMetaDataObjectClassProp locationOCP = equipmentOC.getObjectClassProp( CswNbtObjClassEquipment.PropertyName.Location );
            if( null == locationOCP )
            {
                locationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( equipmentOC )
                {
                    PropName = CswNbtObjClassEquipment.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location
                } );
            }

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
            locationOCP = assemblyOC.getObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Location );
            if( null == locationOCP )
            {
                locationOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( assemblyOC )
                {
                    PropName = CswNbtObjClassEquipmentAssembly.PropertyName.Location,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Location
                } );
            }

            _resetBlame();
        }

        private void _upgradeAssemblyStatusProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass assemblyOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.EquipmentAssemblyClass );
            CswNbtMetaDataObjectClassProp statusOCP = assemblyOC.getObjectClassProp( CswNbtObjClassEquipmentAssembly.PropertyName.Status );
            if( null == statusOCP )
            {
                statusOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( assemblyOC )
                {
                    PropName = CswNbtObjClassEquipmentAssembly.PropertyName.Status,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                } );
            }

            _resetBlame();
        }

        private void _createReportInstructionsProp( UnitOfBlame BlameMe )
        {
            _acceptBlame( BlameMe );

            CswNbtMetaDataObjectClass reportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ReportClass );
            CswNbtMetaDataObjectClassProp instructionsOCP = reportOC.getObjectClassProp( CswNbtObjClassReport.PropertyName.Instructions );
            if( null == instructionsOCP )
            {
                instructionsOCP = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( reportOC )
                {
                    PropName = CswNbtObjClassReport.PropertyName.Instructions,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Memo,
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

            CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
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

                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( HazardClassOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.listoptions, FireHazardClassTypes.ToString() );
                }

            }


            _resetBlame();
        }

        private void _createMaterialC3SyncDataProp( UnitOfBlame Blame )
        {
            // Add the C3SyncData property to the Material Object Class
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            if( null != MaterialOC )
            {
                CswNbtMetaDataObjectClassProp C3SyncDateOCP = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.C3SyncDate );
                if( null == C3SyncDateOCP )
                {
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                        {
                            PropName = CswNbtObjClassMaterial.PropertyName.C3SyncDate,
                            FieldType = CswNbtMetaDataFieldType.NbtFieldType.DateTime,
                            ServerManaged = true,
                            ReadOnly = true
                        } );
                }

                // Add the C3SyncData property to all Material NodeTypes
                _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();

                // Remove from all layouts
                foreach( CswNbtMetaDataNodeType MaterialNT in MaterialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp C3SyncDateNTP = MaterialNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.C3SyncDate );
                    C3SyncDateNTP.removeFromAllLayouts();
                }
            }
        }

        private void _createHazardClassProp( UnitOfBlame Blame )
        {
            _acceptBlame( Blame );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp HazardClassOCP = MaterialOC.getObjectClassProp( CswNbtObjClassMaterial.PropertyName.HazardClasses );
            if( null == HazardClassOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                {
                    PropName = CswNbtObjClassMaterial.PropertyName.HazardClasses,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.MultiList
                } );
            }

            _resetBlame();
        }

        #endregion ASPEN Methods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            //This ASPEN method has to be first
            _addSaveProperty( new UnitOfBlame( CswDeveloper.CF, 27923 ) );


            #region YORICK

            //YORICK OC changes go here.


            _makeUnCode( CswDeveloper.PG, 28671 );
            _updateUserFormats( CswDeveloper.CF, 26574 );
            _correctGeneratorTargetTypeProps( CswDeveloper.CF, 29039 );

            #endregion YORICK

            #region ASPEN

            _makeC3ProductIdProperty( CswDeveloper.CM, 28688 );
            _createAssemblyBarcodeProp( new UnitOfBlame( CswDeveloper.MB, 29108 ) );
            _upgradeEquipmentBarcodeProp( new UnitOfBlame( CswDeveloper.MB, 29108 ) );
            _upgradeAssemblyAndEquipmentLocationProp( new UnitOfBlame( CswDeveloper.MB, 28648 ) );
            _upgradeAssemblyStatusProp( new UnitOfBlame( CswDeveloper.MB, 28648 ) );
            _createReportInstructionsProp( new UnitOfBlame( CswDeveloper.MB, 28950 ) );
            _fixHazardClassSpellingAndAddNewClasses( new UnitOfBlame( CswDeveloper.CM, 29243 ) );
            _createMaterialC3SyncDataProp( new UnitOfBlame( CswDeveloper.CM, 29246 ) );
            _createHazardClassProp( new UnitOfBlame( CswDeveloper.CM, 29245 ) );

            #endregion ASPEN

            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        } //Update()
    }//class RunBeforeEveryExecutionOfUpdater_01OC
}//namespace ChemSW.Nbt.Schema


