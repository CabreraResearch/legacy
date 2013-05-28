using System;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29773
    /// </summary>
    public class CswUpdateSchema_02B_Case29773 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.NBT; }
        }

        public override int CaseNo
        {
            get { return 29773; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes())
            {
                CswNbtMetaDataNodeTypeProp NameNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.Name );

                // Add 'Name' property to Add layout
                if( null == NameNTP.getAddLayout() )
                {
                    NameNTP.updateLayout( CswEnumNbtLayoutType.Add, false );
                }

            } // foreach( CswNbtMetaDataNodeType MailReportNT in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypes( CswEnumNbtObjectClass.MailReportClass ) )
        } // update()

    }//class CswUpdateSchema_02B_Case29773

}//namespace ChemSW.Nbt.Schema