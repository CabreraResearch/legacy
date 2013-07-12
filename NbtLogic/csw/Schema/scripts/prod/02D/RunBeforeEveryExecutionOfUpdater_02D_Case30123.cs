using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30123
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02D_Case30123 : CswUpdateSchemaTo
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
            _createBalanceObjectClass();
            _addUserDefaultBalanceProp();
        }


        public void _createBalanceObjectClass()
        {

            CswNbtMetaDataObjectClass BalanceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );

            if( null == BalanceOC )
            {

                BalanceOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.BalanceClass, "harddrive.png", false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                    {
                        PropName = CswNbtObjClassBalance.PropertyName.Name,
                        FieldType = CswEnumNbtFieldType.Text,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                    {
                        PropName = CswNbtObjClassBalance.PropertyName.Quantity,
                        FieldType = CswEnumNbtFieldType.Quantity,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                    {
                        PropName = CswNbtObjClassBalance.PropertyName.LastActive,
                        FieldType = CswEnumNbtFieldType.DateTime,
                    } );


            }//if ( null == BalanceOC )

        }//createBalanceObjectClass()



        public void _addUserDefaultBalanceProp()
        {

            CswNbtMetaDataObjectClass BalanceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
            CswNbtMetaDataObjectClass UserOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );

            CswNbtMetaDataObjectClassProp DefaultBalanceOCP = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.DefaultBalance );

            if( null == DefaultBalanceOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( UserOC )
                    {
                        PropName = CswNbtObjClassUser.PropertyName.DefaultBalance,
                        FieldType = CswEnumNbtFieldType.Relationship,
                        IsFk = true,
                        FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                        FkValue = BalanceOC.ObjectClassId,
                        ServerManaged = false

                    } );

            }//if( null == DefaultBalanceOCP )


        }//addUserDefaultBalanceProp


}//class CswUpdateSchema_02C_Case30123

}//namespace ChemSW.Nbt.Schema