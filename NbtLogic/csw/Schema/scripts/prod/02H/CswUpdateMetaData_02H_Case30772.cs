using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateMetaData_02H_Case30772 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 30772; }
        }

        public override string ScriptName
        {
            get { return "02H_Case" + CaseNo; }
        }

        public override string Title
        {
            get { return "Only Mail Reports can set Hourly Schedules"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp DueDateIntervalNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.DueDateInterval );
                if( null != DueDateIntervalNTP )
                {
                    DueDateIntervalNTP.Attribute1 = CswConvert.ToDbVal( true ).ToString();
                }
            }
        }

    }

}//namespace ChemSW.Nbt.Schema