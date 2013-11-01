using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30680 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Move GHS Add Codes Properties to Tab Group"; } }

        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30680; }
        }

        public override string ScriptName
        {
            get { return "Case30680NT"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass GHSOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.GHSClass );
            foreach( CswNbtMetaDataNodeType GHSNT in GHSOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp AddClassCodesNTP = GHSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.AddClassCodes );
                AddClassCodesNTP.updateLayout( CswEnumNbtLayoutType.Add, true, TabGroup: "Classification" );
                AddClassCodesNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, GHSNT.getFirstNodeTypeTab().TabId, TabGroup: "Classification" );

                CswNbtMetaDataNodeTypeProp AddLabelCodesNTP = GHSNT.getNodeTypePropByObjectClassProp( CswNbtObjClassGHS.PropertyName.AddLabelCodes );
                AddLabelCodesNTP.updateLayout( CswEnumNbtLayoutType.Add, true, TabGroup: "Labeling" );
                AddLabelCodesNTP.updateLayout( CswEnumNbtLayoutType.Edit, true, GHSNT.getFirstNodeTypeTab().TabId, TabGroup: "Labeling" );
            }
        }
    }
}