using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30590 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30590; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Inspection Details Tab Comes First"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass InspectionDesignOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass );
            foreach(CswNbtMetaDataNodeType InspectionNT in InspectionDesignOC.getNodeTypes())
            {
                CswNbtMetaDataNodeTypeTab DetailsTab = InspectionNT.getNodeTypeTab( "Details" );
                if( null != DetailsTab )
                {
                    DetailsTab.TabOrder = 0;
                }
            }
        }

    }

}//namespace ChemSW.Nbt.Schema