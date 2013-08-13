using System;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30123
    /// </summary>
    public class CswUpdateSchema_02E_Case30123 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30123; }
        }

        public override string ScriptName
        {
            get { throw new NotImplementedException(); }
        }

        public override bool AlwaysRun
        {
            get { throw new NotImplementedException(); }
        }

        public override void update()
        {
            _addBalanceNT();
            _addBalanceConfigurationNT();
            _updateUserLayout();

        } // update()



        public void _addBalanceNT()
        {
            CswNbtMetaDataObjectClass BalanceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
            if( null == BalanceOC.FirstNodeType )
            {
                CswNbtMetaDataNodeType BalanceNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( BalanceOC )
                {
                    NodeTypeName = "Balance",
                    Category = "System",
                } );

                BalanceNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassBalance.PropertyName.Name ) );
            }

        }//_addBalanceNT()


        public void _addBalanceConfigurationNT()
        {
            CswNbtMetaDataObjectClass BalanceConfigurationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass );
            if( null == BalanceConfigurationOC.FirstNodeType )
            {
                CswNbtMetaDataNodeType BalanceConfigurationNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( BalanceConfigurationOC )
                    {
                        NodeTypeName = "Balance Configuration",
                        Category = "System",
                    } );

                BalanceConfigurationNT.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassBalanceConfiguration.PropertyName.Name ) );
            }

        }//_addBalanceConfigurationNT()


        public void _updateUserLayout()
        {

            CswNbtMetaDataNodeType UserNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "User" );
            CswNbtMetaDataNodeTypeProp DefaultBalanceNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.DefaultBalance );
            CswNbtMetaDataNodeTypeProp DefaultPrinterNTP = UserNT.getNodeTypePropByObjectClassProp( CswNbtObjClassUser.PropertyName.DefaultPrinter );

            //insert the Balance prop to the layout after the Printer prop
            DefaultBalanceNTP.updateLayout( CswEnumNbtLayoutType.Add, DefaultPrinterNTP, true );
            DefaultBalanceNTP.updateLayout( CswEnumNbtLayoutType.Edit, DefaultPrinterNTP, true );

        }//_updateUserLayout()

    }//class CswUpdateSchema_02C_Case30123

}//namespace ChemSW.Nbt.Schema