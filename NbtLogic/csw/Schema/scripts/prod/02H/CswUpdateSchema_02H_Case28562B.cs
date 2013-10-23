using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02H_Case28562B : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28562; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo + "B"; }
        }

        public override string Title
        {
            get { return "Report WebService property - layout"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass ReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportClass );
            foreach( CswNbtMetaDataNodeType ReportNT in ReportOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SqlNTP = ReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassReport.PropertyName.Sql );
                CswNbtMetaDataNodeTypeProp WebServiceNTP = ReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassReport.PropertyName.WebService );
                if( null != WebServiceNTP )
                {
                    WebServiceNTP.removeFromAllLayouts();
                    WebServiceNTP.updateLayout( CswEnumNbtLayoutType.Edit, SqlNTP, true );
                }
            }
        } // update()
    }
}//namespace ChemSW.Nbt.Schema