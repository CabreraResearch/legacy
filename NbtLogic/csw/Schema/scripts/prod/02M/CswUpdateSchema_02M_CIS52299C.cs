using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.csw.Schema;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52299C : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 52299; }
        }

        public override string Title
        {
            get { return "Fix MaterialComponent nodetypes for new field type"; }
        }

        public override string AppendToScriptName()
        {
            return "C";
        }

        public override void update()
        {
            // Set default value of Units to %
            CswNbtMetaDataObjectClass ComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MaterialComponentClass );

            foreach( CswNbtMetaDataNodeType NodeType in ComponentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp PercentRangeNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
                PercentRangeNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleNumericRange.AttributeName.DefaultValue].SetSubFieldValue( CswNbtFieldTypeRuleNumericRange.SubFieldName.Units, "%" );
            }


            // Set layout on nodetypes
            CswNbtSchemaUpdateLayoutMgr AddLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.MaterialComponentClass, LayoutType: CswEnumNbtLayoutType.Add );
            AddLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            AddLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            AddLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
            AddLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Active );
            AddLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting );

            CswNbtSchemaUpdateLayoutMgr EditLayoutMgr = new CswNbtSchemaUpdateLayoutMgr( _CswNbtSchemaModTrnsctn, CswEnumNbtObjectClass.MaterialComponentClass, LayoutType: CswEnumNbtLayoutType.Edit );
            EditLayoutMgr.First.moveProp( Row: 1, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            EditLayoutMgr.First.moveProp( Row: 2, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            EditLayoutMgr.First.moveProp( Row: 3, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
            EditLayoutMgr.First.moveProp( Row: 4, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.Active );
            EditLayoutMgr.First.moveProp( Row: 5, Column: 1, PropName: CswNbtObjClassMaterialComponent.PropertyName.HazardousReporting );

            // Fix Chemical Components Grid
            CswNbtMetaDataNodeType ChemicalNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Chemical" );
            if( null != ChemicalNT )
            {
                CswNbtMetaDataNodeTypeProp ComponentsNTP = ChemicalNT.getNodeTypeProp( "Components" );
                if( null != ComponentsNTP )
                {
                    CswNbtView ComponentsView = _CswNbtSchemaModTrnsctn.restoreView( ComponentsNTP.ViewId );
                    CswNbtViewProperty percentageVP = ComponentsView.findPropertyByName( "Percentage" );
                    if( null != percentageVP )
                    {
                        CswNbtMetaDataObjectClassProp PercentageRangeOCP = ComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.PercentageRange );
                        percentageVP.Name = PercentageRangeOCP.PropName;
                        percentageVP.ObjectClassPropId = PercentageRangeOCP.ObjectClassPropId;
                        percentageVP.FieldType = CswEnumNbtFieldType.NumericRange.ToString();
                        ComponentsView.save();
                    }
                        /*
                         *<TreeView viewname="Components" version="1.0" iconfilename="Images/view/viewgrid.gif" selectable="true" mode="Grid" width="100" viewid="3293" category="" visibility="Property" 
                         *        visibilityroleid="" visibilityuserid="" groupbysiblings="false" included="true" isdemo="false" issystem="false" gridgroupbycol="">
                         *    <Relationship secondname="ChemicalClass" secondtype="ObjectClassId" secondid="147" secondiconfilename="atom.png" selectable="true" arbitraryid="root_OC_147" showintree="true" 
                         *                  allowaddchildren="True" allowview="True" allowedit="True" allowdelete="True" nodeidstofilterin="" nodeidstofilterout="">
                         *        <Relationship propid="1373" propname="Mixture" proptype="ObjectClassPropId" propowner="Second" firstname="ChemicalClass" firsttype="ObjectClassId" firstid="147" 
                         *                      secondname="MaterialComponentClass" secondtype="ObjectClassId" secondid="315" secondiconfilename="atom.png" selectable="true" arbitraryid="root_OC_147_OC_3151373" 
                         *                      showintree="true" allowaddchildren="True" allowview="True" allowedit="True" allowdelete="True" nodeidstofilterin="" nodeidstofilterout="">
                         *            <Property type="ObjectClassPropId" nodetypepropid="-2147483648" objectclasspropid="1374" name="Constituent" arbitraryid="root_OC_147_OC_3151373_OCP_1374" sortby="False" 
                         *                      sortmethod="Ascending" fieldtype="Relationship" order="1" width="" showingrid="True" />
                         *            <Property type="ObjectClassPropId" nodetypepropid="-2147483648" objectclasspropid="1364" name="Percentage" arbitraryid="root_OC_147_OC_3151373_OCP_1364" sortby="False" 
                         *                      sortmethod="Ascending" fieldtype="Number" order="3" width="" showingrid="True" />
                         *            <Relationship propid="1374" propname="Constituent" proptype="ObjectClassPropId" propowner="First" firstname="MaterialComponentClass" firsttype="ObjectClassId" 
                         *                      firstid="315" secondname="ChemicalClass" secondtype="ObjectClassId" secondid="147" secondiconfilename="atom.png" selectable="true" 
                         *                      arbitraryid="root_OC_147_OC_3151373_OC_1471374" showintree="true" allowaddchildren="True" allowview="True" allowedit="True" allowdelete="True" 
                         *                      nodeidstofilterin="" nodeidstofilterout="">
                         *                <Property type="NodeTypePropId" nodetypepropid="5583" objectclasspropid="-2147483648" name="CAS No" arbitraryid="root_OC_147_OC_3151373_OC_1471374_NTP_5583" 
                         *                          sortby="False" sortmethod="Ascending" fieldtype="CASNo" order="2" width="" showingrid="True" />
                         *            </Relationship>
                         *        </Relationship>
                         *    </Relationship>
                         *</TreeView>
                         */
                }
            }
        }
    }
}