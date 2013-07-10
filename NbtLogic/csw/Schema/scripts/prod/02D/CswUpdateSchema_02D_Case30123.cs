using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30123
    /// </summary>
    public class CswUpdateSchema_02D_Case30123: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MO; }
        }

        public override int CaseNo
        {
            get { return 30123; }
        }

        public override void update()
        {
            _addBalanceNT();
            _updateUserLayout();

        } // update()



        public void _addBalanceNT()
        {
            CswNbtMetaDataNodeType BalanceNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Balance" );
            if( null == BalanceNT )
            {

                CswNbtMetaDataObjectClass BalanceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
                BalanceNT = _CswNbtSchemaModTrnsctn.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( BalanceOC )
                {
                    NodeTypeName = "Balance",
                    Category = "System",
                } );

            }

        }//_addBalanceNT()


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