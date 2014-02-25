using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02K_Case31803: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 31803; }
        }

        public override string Title
        {
            get { return "Make Mail Report Name property unique"; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass MailReportOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass );
            CswNbtMetaDataObjectClassProp NameOCP = MailReportOC.getObjectClassProp( CswNbtObjClassMailReport.PropertyName.Name );

            //Make sure all Mail reports have unique names
            foreach( CswNbtObjClassMailReport MailReport in MailReportOC.getNodes( false, true, false, true ) )
            {
                _makeNameUnique( MailReportOC, NameOCP, MailReport, MailReport.Name.Text, 0 );
            }

            //Make the Name property unique
            foreach( CswNbtMetaDataNodeTypeProp NameNTP in NameOCP.getNodeTypeProps() )
            {
                CswNbtObjClassDesignNodeTypeProp NamePropertyNode = NameNTP.DesignNode;
                NamePropertyNode.Unique.Checked = CswEnumTristate.True;
                NamePropertyNode.postChanges( false );
            }

        } // update()

        private void _makeNameUnique( CswNbtMetaDataObjectClass MailReportOC, CswNbtMetaDataObjectClassProp NameOCP, CswNbtObjClassMailReport MailReport, string ProposedName, int Modifier )
        {
            if( false == _findMailReportByName( MailReportOC, NameOCP, ProposedName ) )
            {
                MailReport.Name.Text = ProposedName;
                MailReport.postChanges( false );
            }
            else
            {
                Modifier++;
                string NewName = ProposedName + Modifier;
                _makeNameUnique( MailReportOC, NameOCP, MailReport, NewName, Modifier );
            }
        }

        private bool _findMailReportByName( CswNbtMetaDataObjectClass MailReportOC, CswNbtMetaDataObjectClassProp NameOCP, string Name )
        {
            CswNbtView MailReportsView = _CswNbtSchemaModTrnsctn.makeNewView( "MailReports31838ByName", CswEnumNbtViewVisibility.Hidden );
            CswNbtViewRelationship Parent = MailReportsView.AddViewRelationship( MailReportOC, false );
            MailReportsView.AddViewPropertyAndFilter( Parent, NameOCP, Name );

            ICswNbtTree MailReportsTree = _CswNbtSchemaModTrnsctn.getTreeFromView( MailReportsView, true );
            return MailReportsTree.getChildNodeCount() > 1;
        }

    }

}//namespace ChemSW.Nbt.Schema