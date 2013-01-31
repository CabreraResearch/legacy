using System;
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC : CswUpdateSchemaTo
    {
        public static string Title = "Pre-Script: OC";

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

        private Int32 _CaseNo = 0;

        public override int CaseNo
        {
            get { return _CaseNo; }
        }


        private CswNbtMetaDataNodeTypeProp _createNewProp( CswNbtMetaDataNodeType Nodetype, string PropName, CswNbtMetaDataFieldType.NbtFieldType PropType, bool SetValOnAdd = true )
        {
            CswNbtMetaDataNodeTypeProp Prop = _CswNbtSchemaModTrnsctn.MetaData.makeNewProp( Nodetype, PropType, PropName, Nodetype.getFirstNodeTypeTab().TabId );
            if( SetValOnAdd )
            {
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                    CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add,
                    Nodetype.NodeTypeId,
                    Prop.PropId,
                    true,
                    Nodetype.getFirstNodeTypeTab().TabId
                    );
            }
            _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout(
                CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit,
                Nodetype.NodeTypeId,
                Prop.PropId,
                true,
                Nodetype.getFirstNodeTypeTab().TabId
                );

            return Prop;
        }

        #region Viola Methods

        #region Case 28283

        private void _addFireClassExemptAmountProps( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            #region FireClassExemptAmountSet

            CswNbtMetaDataObjectClass FireClassExemptAmountSetOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountSetClass );
            if( null == FireClassExemptAmountSetOC )
            {
                FireClassExemptAmountSetOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.FireClassExemptAmountSetClass, "explode.png", false );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, FireClassExemptAmountSetOC.ObjectClassId );
            }
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountSetOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmountSet.PropertyName.SetName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                IsUnique = true
            } );

            #endregion FireClassExemptAmountSet

            #region FireClassExemptAmount

            CswNbtMetaDataObjectClass FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass );
            if( null == FireClassExemptAmountOC )
            {
                FireClassExemptAmountOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.FireClassExemptAmountClass, "explode.png", false );
                _CswNbtSchemaModTrnsctn.createModuleObjectClassJunction( CswNbtModuleName.CISPro, FireClassExemptAmountOC.ObjectClassId );
            }
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.SetName,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                IsFk = true,
                FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                FkValue = FireClassExemptAmountSetOC.ObjectClassId,
                IsRequired = true,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 1
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.HazardCategory,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ReadOnly = true,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 2
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.Class,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                ReadOnly = true,
                SetValOnAdd = true,
                DisplayColAdd = 2,
                DisplayRowAdd = 2
            } );
            CswCommaDelimitedString FireHazardClassTypes = new CswCommaDelimitedString {
                "Aero-1",
                "Aero-2",
                "Aero-3",
                "Carc",
                "CF/D (baled)",
                "CF/D (loose)",
                "CL-II",
                "CL-IIIA",
                "CL-IIIB",
                "Corr",
                "CRY-FG",
                "CRY-OXY",
                "Exp",
                "FG (gaseous)",
                "FG (liquified)",
                "FL-1A",
                "FL-1B",
                "FL-1C",
                "FL-Comb",
                "FS",
                "HT",
                "Irr",
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
                "UR-1",
                "UR-2",
                "UR-3",
                "UR-4",
                "WR-1",
                "WR-2",
                "WR-3"
            };
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.HazardClass,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ReadOnly = true,
                SetValOnAdd = true,
                IsRequired = true,
                ListOptions = FireHazardClassTypes.ToString(),
                DisplayColAdd = 1,
                DisplayRowAdd = 3
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.HazardType,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                SetValOnAdd = true,
                ListOptions = "Physical,Health",
                DisplayColAdd = 1,
                DisplayRowAdd = 4
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.CategoryFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 5
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.SortOrder,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 6
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );

            #endregion FireClassExemptAmount

            _resetBlame();
        }

        #endregion Case 28283

        #region Case 28281
        private void _addHazardousReoprtingProp( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass MaterialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp HazardousReportingOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialComponentOC )
            {
                PropName = CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                IsRequired = true
            } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( HazardousReportingOCP, false );

            _resetBlame();
        }

        private void _addContainerFireReportingProps( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass ContainerOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.StoragePressure,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "1 = Atmospheric,2 = Pressurized,3 = Subatmospheric",
                SetValOnAdd = false
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.StorageTemperature,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "4 = Room Temperature,5 = Greater than Room Temperature,6 = Less than Room Temperature,7 = Cryogenic",
                SetValOnAdd = false
            } );

            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( ContainerOC )
            {
                PropName = CswNbtObjClassContainer.PropertyName.UseType,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                ListOptions = "Storage,Use Closed,Use Open",
                SetValOnAdd = false
            } );

            _resetBlame();
        }
        #endregion Case 28281

        #region Case 28282

        private void _addControlZoneNT( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass GenericOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GenericClass );
            if( null != GenericOc )
            {
                //ControlZone NodeType
                CswNbtMetaDataNodeType ControlZoneNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Control Zone" );
                if( null == ControlZoneNt )
                {
                    ControlZoneNt = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( GenericOc.ObjectClassId, "Control Zone", "Materials" );
                    _CswNbtSchemaModTrnsctn.createModuleNodeTypeJunction( CswNbtModuleName.CISPro, ControlZoneNt.NodeTypeId );

                    CswNbtMetaDataNodeTypeProp NameNTP = _createNewProp( ControlZoneNt, "Name", CswNbtMetaDataFieldType.NbtFieldType.Text );
                    NameNTP.IsRequired = true;
                    CswNbtMetaDataNodeTypeProp MAQOffsetNTP = _createNewProp( ControlZoneNt, "MAQ Offset %", CswNbtMetaDataFieldType.NbtFieldType.Number, false );
                    MAQOffsetNTP.DefaultValue.AsNumber.Value = 100;
                    CswNbtMetaDataObjectClass FCEASOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountSetClass );
                    CswNbtMetaDataNodeTypeProp FCEASNameNTP = _createNewProp( ControlZoneNt, "Fire Class Set Name", CswNbtMetaDataFieldType.NbtFieldType.Relationship );
                    FCEASNameNTP.SetFK( NbtViewRelatedIdType.ObjectClassId.ToString(), FCEASOC.ObjectClassId );

                    ControlZoneNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( "Name" ) );

                    //Update Location to include Control Zone
                    CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
                    _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( LocationOC )
                    {
                        PropName = CswNbtObjClassLocation.PropertyName.ControlZone,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                        IsFk = true,
                        FkType = NbtViewRelatedIdType.NodeTypeId.ToString(),
                        FkValue = ControlZoneNt.NodeTypeId
                    } );
                }
            }

            _resetBlame();
        }

        #endregion Case 28282

        #region Case 28408

        private void _addBarcodePropToUserOC( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UserClass );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UserOC )
            {
                PropName = CswNbtObjClassUser.PropertyName.Barcode,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Barcode,
                IsUnique = true
            } );

            _resetBlame();
        }

        #endregion

        #region Case 28255

        private void _fixLocationOcps( CswDeveloper Dev, Int32 CaseNo )
        {
            _acceptBlame( Dev, CaseNo );
            CswNbtMetaDataObjectClass LocationOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            foreach( CswNbtMetaDataObjectClassProp LocationOcp in _CswNbtSchemaModTrnsctn.MetaData.getObjectClassProps( CswNbtMetaDataFieldType.NbtFieldType.Location ) )
            {
                Int32 FkValue = LocationOcp.FKValue;
                if( Int32.MinValue == FkValue )
                {
                    FkValue = LocationOc.ObjectClassId;
                }

                if( string.IsNullOrEmpty( LocationOcp.FKType ) || Int32.MinValue == LocationOcp.FKValue )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fktype, NbtViewRelatedIdType.ObjectClassId.ToString() );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.fkvalue, FkValue );
                    _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LocationOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isfk, true );
                }
            }
            foreach( CswNbtMetaDataNodeTypeProp LocationNtp in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.Location ) )
            {
                Int32 FkValue = LocationNtp.FKValue;
                if( Int32.MinValue == FkValue )
                {
                    FkValue = LocationOc.ObjectClassId;
                }
                if( string.IsNullOrEmpty( LocationNtp.FKType ) || Int32.MinValue == LocationNtp.FKValue )
                {
                    LocationNtp.SetFK( inFKValue: FkValue, inFKType: NbtViewRelatedIdType.ObjectClassId.ToString() );
                }

            }
            _resetBlame();
        }

        #endregion Case 28255

        #region Case 28247

        private void _addMaterialTierIIOCP( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClassProp IsTierIIOCP =
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( MaterialOC )
                {
                    PropName = CswNbtObjClassMaterial.PropertyName.IsTierII,
                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Logical,
                    IsRequired = true
                } );
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( IsTierIIOCP, false );

            _resetBlame();
        }

        #endregion Case 28247


        #region Case 28145

        private void _correctSpellingOnStorageCompField( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass LocationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.LocationClass );
            //Using a string for the prop name because I already corrected the spelling of 'Compatibility' in the obj class
            //and looking for the word 'Compatibility' spelled correctly won't return anything
            CswNbtMetaDataObjectClassProp StorageCompatibilityOCP = LocationOC.getObjectClassProp( "Storage Compatability" );
            if( null != StorageCompatibilityOCP )
            {
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( StorageCompatibilityOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.propname, "Storage Compatibility" );
            }

            _resetBlame();
        }

        #endregion Case 28145

        #region 28424

        private void _fixContainerLabelFormatView( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp LabelFormatOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );

            _acceptBlame( CswDeveloper.CF, 24424 );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LabelFormatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp NodeTypeOcp = PrintLabelOc.getObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );
            string LabelViewXml = null;
            foreach( CswNbtMetaDataNodeTypeProp LfNtp in LabelFormatOcp.getNodeTypeProps() )
            {
                CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( LfNtp.ViewId );
                View.Root.ChildRelationships.Clear();

                CswNbtViewRelationship LabelVr = View.AddViewRelationship( PrintLabelOc, IncludeDefaultFilters: false );
                View.AddViewPropertyAndFilter( LabelVr, NodeTypeOcp, "Container", FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Contains );
                LabelViewXml = LabelViewXml ?? View.ToXml().ToString();
                View.save();
            }

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LabelFormatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, LabelViewXml );

            _resetBlame();
        }

        #endregion

        #region Case 27436

        private void _addGhsOC( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            if( null == _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.GHSClass ) )
            {
                CswNbtMetaDataObjectClass JurisdictionOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.JurisdictionClass );
                CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

                CswNbtMetaDataObjectClass GhsOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.GHSClass, "warning.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                          {
                                                                              PropName = CswNbtObjClassGHS.PropertyName.Jurisdiction,
                                                                              FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                                              FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                                              FkValue = JurisdictionOC.ObjectClassId,
                                                                              IsRequired = true
                                                                          } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                          {
                                                                              PropName = CswNbtObjClassGHS.PropertyName.Material,
                                                                              FieldType = CswNbtMetaDataFieldType.NbtFieldType.Relationship,
                                                                              FkType = NbtViewRelatedIdType.ObjectClassId.ToString(),
                                                                              FkValue = MaterialOC.ObjectClassId,
                                                                              IsRequired = true
                                                                          } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassGHS.PropertyName.LabelCodes,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.MultiList
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassGHS.PropertyName.ClassCodes,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.MultiList
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassGHS.PropertyName.LabelCodesGrid,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Grid
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                    {
                        PropName = CswNbtObjClassGHS.PropertyName.ClassCodesGrid,
                        FieldType = CswNbtMetaDataFieldType.NbtFieldType.Grid
                    } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                          {
                                                                              PropName = CswNbtObjClassGHS.PropertyName.SignalWord,
                                                                              FieldType = CswNbtMetaDataFieldType.NbtFieldType.List
                                                                          } );

                CswNbtMetaDataObjectClass GhsPhraseOC = _CswNbtSchemaModTrnsctn.createObjectClass( NbtObjectClass.GHSPhraseClass, "warning.png", false );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsPhraseOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                                {
                                                                                    PropName = CswNbtObjClassGHSPhrase.PropertyName.Code,
                                                                                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
                                                                                } );
                _CswNbtSchemaModTrnsctn.createObjectClassProp( GhsPhraseOC, new CswNbtWcfMetaDataModel.ObjectClassProp
                                                                                {
                                                                                    PropName = CswNbtObjClassGHSPhrase.PropertyName.Category,
                                                                                    FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                                                                                    ListOptions = "Physical,Health,Environmental"
                                                                                } );
            }
            _resetBlame();
        } // _addGhsOC()

        #endregion Case 27436

        #endregion Viola Methods

        /// <summary>
        /// The actual update call
        /// </summary>
        public override void update()
        {
            // This script is for adding object class properties, 
            // which often become required by other business logic and can cause prior scripts to fail.

            #region VIOLA

            _addFireClassExemptAmountProps( CswDeveloper.BV, 28283 );
            _addHazardousReoprtingProp( CswDeveloper.BV, 28281 );
            _addContainerFireReportingProps( CswDeveloper.BV, 28281 );
            _addControlZoneNT( CswDeveloper.BV, 28282 );
            _addBarcodePropToUserOC( CswDeveloper.CM, 28408 );
            _fixLocationOcps( CswDeveloper.CF, 28255 );
            _addMaterialTierIIOCP( CswDeveloper.BV, 28247 );
            _correctSpellingOnStorageCompField( CswDeveloper.CM, 28145 );
            _fixContainerLabelFormatView( CswDeveloper.CF, 28424 );
            _addGhsOC( CswDeveloper.SS, 27436 );

            #endregion VIOLA


            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01OC

}//namespace ChemSW.Nbt.Schema


