using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52296B: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52296; }
        }

        public override string Title
        {
            get { return "Create MLM CertDefSpecLevel NodeType"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass CertDefSpecLevelOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.CertDefSpecLevelClass );
            CswNbtMetaDataNodeType CertDefSpecLevelNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( CertDefSpecLevelOC )
                {
                    Category = "MLM",
                    IconFileName = "barchart.png",
                    NodeTypeName = "Cert Def Spec Level",
                    NameTemplate = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefSpecLevel.PropertyName.Level ) + " " + CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassCertDefSpecLevel.PropertyName.CertDefSpec )
                } );
            CswNbtMetaDataNodeTypeTab FirstTab = CertDefSpecLevelNT.getFirstNodeTypeTab();

            foreach( CswNbtMetaDataObjectClassProp ocp in CertDefSpecLevelOC.getObjectClassProps() )
            {
                CswNbtMetaDataNodeTypeProp ntp = CertDefSpecLevelNT.getNodeTypePropByObjectClassProp( ocp );
                ntp.removeFromAllLayouts();
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, CertDefSpecLevelNT.NodeTypeId, ntp, true, FirstTab.TabId );

                if( CswNbtObjClassCertDefSpecLevel.PropertyName.SampleSize != ocp.PropName )
                {
                    _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, CertDefSpecLevelNT.NodeTypeId, ntp, true );
                }

                if( CswNbtObjClassCertDefSpecLevel.PropertyName.ApprovalPeriod == ocp.PropName )
                {
                    CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
                    CswNbtMetaDataObjectClassProp NameOCP = UnitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );
                    CswNbtView MonthsUnitView = _CswNbtSchemaModTrnsctn.makeNewView( "MonthsViewForCertDefSpecApprovvalPeriodProp", CswEnumNbtViewVisibility.Hidden );
                    CswNbtViewRelationship parent = MonthsUnitView.AddViewRelationship( UnitOfMeasureOC, true );
                    MonthsUnitView.AddViewPropertyAndFilter( parent, NameOCP, "Months" );
                    MonthsUnitView.save();

                    ntp.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.UnitView].AsViewReference.ViewId = MonthsUnitView.ViewId;
                }
            }

            _makeSampleSizeView( CswNbtObjClassCertDefSpecLevel.GramsViewName, "g" );
            _makeSampleSizeView( CswNbtObjClassCertDefSpecLevel.EachViewName, "Each" );

        }

        private void _makeSampleSizeView( string ViewName, string FilterVal )
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UnitOfMeasureClass );
            CswNbtMetaDataObjectClassProp NameOCP = UnitOfMeasureOC.getObjectClassProp( CswNbtObjClassUnitOfMeasure.PropertyName.Name );

            CswNbtView SampleSizeUnitsView = _CswNbtSchemaModTrnsctn.makeSafeView( ViewName, CswEnumNbtViewVisibility.Hidden );
            CswNbtViewRelationship parent = SampleSizeUnitsView.AddViewRelationship( UnitOfMeasureOC, true );

            SampleSizeUnitsView.AddViewPropertyAndFilter( parent, NameOCP, FilterVal );
            SampleSizeUnitsView.save();
        }
    }
}