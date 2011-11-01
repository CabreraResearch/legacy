using System;
using System.Collections.ObjectModel;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01J-10
    /// </summary>
    public class CswUpdateSchemaTo01J10 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'J', 10 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {
            // case 24051
            CswNbtMetaDataNodeType MailReportNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Mail Report" );
            if( null != MailReportNt )
            {
                Collection<CswNbtMetaDataNodeTypeProp> Ntps = new Collection<CswNbtMetaDataNodeTypeProp>();
                CswNbtMetaDataNodeTypeTab MailReportTab = MailReportNt.getNodeTypeTab( "Mail Report" );
                if( null != MailReportTab )
                {
                    CswNbtMetaDataNodeTypeProp NameNtp = MailReportNt.getNodeTypeProp( "Name" );
                    if( null != NameNtp )
                    {
                        Ntps.Add( NameNtp );
                        NameNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 1, 1 );
                    }
                    
                    CswNbtMetaDataNodeTypeProp TypeNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.TypePropertyName );
                    Ntps.Add( TypeNtp );
                    TypeNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 2, 1 );
                    
                    CswNbtMetaDataNodeTypeProp MessageNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.MessagePropertyName );
                    Ntps.Add( MessageNtp );
                    MessageNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 3, 1 );

                    CswNbtMetaDataNodeTypeProp NoDataNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.NoDataNotificationPropertyName );
                    Ntps.Add( NoDataNtp );
                    NoDataNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 4, 1 );

                    CswNbtMetaDataNodeTypeProp NextDueNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.NextDueDatePropertyName );
                    Ntps.Add( NextDueNtp );
                    NextDueNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 5, 1 );

                    CswNbtMetaDataNodeTypeProp LastNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.LastProcessedPropertyName );
                    Ntps.Add( LastNtp );
                    LastNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 6, 1 );

                    CswNbtMetaDataNodeTypeProp RunNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.RunStatusPropertyName );
                    Ntps.Add( RunNtp );
                    RunNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 7, 1 );

                    CswNbtMetaDataNodeTypeProp EnabledNtp = MailReportNt.getNodeTypePropByObjectClassPropName( CswNbtObjClassMailReport.EnabledPropertyName );
                    Ntps.Add( EnabledNtp );
                    EnabledNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, 8, 1 );

                    Int32 I = 9;
                    foreach( CswNbtMetaDataNodeTypeProp MrNtp in MailReportTab.NodeTypeProps )
                    {
                        if( false == Ntps.Contains( MrNtp ) )
                        {
                            MrNtp.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Edit, MailReportTab, I, 1 );
                            I += 1;
                        }
                    }
                }
            }


        }//Update()

    }//class CswUpdateSchemaTo01J10

}//namespace ChemSW.Nbt.Schema


