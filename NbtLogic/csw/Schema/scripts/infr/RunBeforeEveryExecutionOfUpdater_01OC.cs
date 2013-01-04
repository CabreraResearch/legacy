using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for DDL changes
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_01OC: CswUpdateSchemaTo
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
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 1
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.SortOrder,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 2
            } );
            String FireHazardClassTypes = @"Aero-1,Aero-2,Aero-3,Carc,CF/D (balled),CF/D (loose),CL-II,CL-IIIA,CL-IIIB,
                    Corr,CRY-FG,CRY-OXY,Exp,FG (gaseous),FG (liquified),FL-1A,FL-1B,FL-1C,FL-Comb,FS,H.T.,Irr,OHH,
                    Oxy1,Oxy2,Oxy3,Oxy4,Oxy-Gas,Oxy-Gas (liquid),Perox-Det,Perox-I,Perox-II,Perox-III,Perox-IV,Perox-V,Pyro,
                    RAD-Alpha,RAD-Beta,RAD-Gamma,Sens,Tox,UR-1,UR-2,UR-3,UR-4,WR-1,WR-2,WR-3";
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.FireHazardClassType,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.List,
                SetValOnAdd = true,
                IsRequired = true,
                ListOptions = FireHazardClassTypes,
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
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.Material,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text,
                SetValOnAdd = true,
                DisplayColAdd = 1,
                DisplayRowAdd = 5
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.StorageGasExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedLiquidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.ClosedGasExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenSolidExemptFootnotes,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Text
            } );
            _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( FireClassExemptAmountOC )
            {
                PropName = CswNbtObjClassFireClassExemptAmount.PropertyName.OpenLiquidExemptAmount,
                FieldType = CswNbtMetaDataFieldType.NbtFieldType.Quantity
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
                    LocationNtp.SetFK( inFKValue : FkValue, inFKType : NbtViewRelatedIdType.ObjectClassId.ToString() );
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

        #region 28424

        private void _fixContainerLabelFormatView( CswDeveloper Dev, Int32 CaseNum )
        {
            _acceptBlame( Dev, CaseNum );

            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp LabelFormatOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.LabelFormat );
            CswNbtMetaDataObjectClass PrintLabelOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.PrintLabelClass );
            CswNbtMetaDataObjectClassProp NodeTypeOcp = PrintLabelOc.getObjectClassProp( CswNbtObjClassPrintLabel.PropertyName.NodeTypes );
            string LabelViewXml = null;
            foreach( CswNbtMetaDataNodeTypeProp LfNtp in LabelFormatOcp.getNodeTypeProps() )
            {
                CswNbtView View = _CswNbtSchemaModTrnsctn.restoreView( LfNtp.ViewId );
                View.Root.ChildRelationships.Clear();

                CswNbtViewRelationship LabelVr = View.AddViewRelationship( PrintLabelOc, IncludeDefaultFilters : false );
                View.AddViewPropertyAndFilter( LabelVr, NodeTypeOcp, "Container", FilterMode : CswNbtPropFilterSql.PropertyFilterMode.Contains );
                LabelViewXml = LabelViewXml ?? View.ToXml().ToString();
                View.save();
            }

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LabelFormatOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.viewxml, LabelViewXml );

            _resetBlame();
        }

        #endregion

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
            _fixLocationOcps( CswDeveloper.CF, 28255 );
            _addMaterialTierIIOCP( CswDeveloper.BV, 28247 );
            _fixContainerLabelFormatView( CswDeveloper.CF, 28424 );

            #endregion VIOLA


            //THIS GOES LAST!
            _CswNbtSchemaModTrnsctn.MetaData.makeMissingNodeTypeProps();
        }

        //Update()

    }//class RunBeforeEveryExecutionOfUpdater_01OC

}//namespace ChemSW.Nbt.Schema


