using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27720
    /// </summary>
    public class CswUpdateSchemaCase27720 : CswUpdateSchemaTo
    {
        public override void update()
        {
            
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MailReportClass );
            foreach( CswNbtMetaDataNodeType MailReportNT in MailReportOC.getNodeTypes() )
            {
                // remove 'Nodes to Report' from all layouts
                CswNbtMetaDataNodeTypeProp NodesToReportNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.NodesToReport );
                _CswNbtSchemaModTrnsctn.MetaData.NodeTypeLayout.removePropFromAllLayouts( NodesToReportNTP );

                // remove Run Status from Add layout
                CswNbtMetaDataNodeTypeProp RunStatusNTP = MailReportNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMailReport.PropertyName.RunStatus );
                RunStatusNTP.removeFromLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add );
            }

        }//Update()

    }//class CswUpdateSchemaCase27720

}//namespace ChemSW.Nbt.Schema