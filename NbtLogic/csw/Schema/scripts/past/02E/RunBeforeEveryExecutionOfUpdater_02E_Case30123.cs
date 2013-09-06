using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 30123
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02E_Case30123 : CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 30123"; } }
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
            _createBalanceConfigurationObjectClass();
            _createBalanceObjectClass();
            _addUserDefaultBalanceProp();
        }


        public void _createBalanceConfigurationObjectClass()
        {
            CswNbtMetaDataObjectClass BalanceConfigurationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass );

            if( null == BalanceConfigurationOC )
            {
                BalanceConfigurationOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass, "options.png", false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                    {
                        PropName = CswNbtObjClassBalanceConfiguration.PropertyName.Name,
                        FieldType = CswEnumNbtFieldType.Text,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                    {
                        PropName = CswNbtObjClassBalanceConfiguration.PropertyName.RequestFormat,
                        FieldType = CswEnumNbtFieldType.Text,
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                {
                    PropName = CswNbtObjClassBalanceConfiguration.PropertyName.ResponseFormat,
                    FieldType = CswEnumNbtFieldType.Text,
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                {
                    PropName = CswNbtObjClassBalanceConfiguration.PropertyName.BaudRate,
                    FieldType = CswEnumNbtFieldType.Number,
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                {
                    PropName = CswNbtObjClassBalanceConfiguration.PropertyName.ParityBit,
                    FieldType = CswEnumNbtFieldType.Text,
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                {
                    PropName = CswNbtObjClassBalanceConfiguration.PropertyName.DataBits,
                    FieldType = CswEnumNbtFieldType.Number,
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                {
                    PropName = CswNbtObjClassBalanceConfiguration.PropertyName.StopBits,
                    FieldType = CswEnumNbtFieldType.Text,
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceConfigurationOC )
                {
                    PropName = CswNbtObjClassBalanceConfiguration.PropertyName.Handshake,
                    FieldType = CswEnumNbtFieldType.Text,
                } );

            }//if ( null == BalanceConfigurationOC )
        }//createBalanceConfigurationObjectClass()



        public void _createBalanceObjectClass()
        {

            CswNbtMetaDataObjectClass BalanceOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceClass );
            CswNbtMetaDataObjectClass BalanceConfigurationOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.BalanceConfigurationClass );

            if( null == BalanceOC && null != BalanceConfigurationOC )
            {

                BalanceOC = _CswNbtSchemaModTrnsctn.createObjectClass( CswEnumNbtObjectClass.BalanceClass, "harddrive.png", false );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                    {
                        PropName = CswNbtObjClassBalance.PropertyName.Name,
                        FieldType = CswEnumNbtFieldType.Text,
                        ServerManaged = true
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                    {
                        PropName = CswNbtObjClassBalance.PropertyName.Quantity,
                        FieldType = CswEnumNbtFieldType.Quantity,
                        ServerManaged = true
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                    {
                        PropName = CswNbtObjClassBalance.PropertyName.LastActive,
                        FieldType = CswEnumNbtFieldType.DateTime,
                        ServerManaged = true
                    } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                {
                    PropName = CswNbtObjClassBalance.PropertyName.Manufacturer,
                    FieldType = CswEnumNbtFieldType.Text,
                    ServerManaged = true
                } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                {
                    PropName = CswNbtObjClassBalance.PropertyName.Device,
                    FieldType = CswEnumNbtFieldType.Text,
                    ServerManaged = true
                } );
            
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                {
                    PropName = CswNbtObjClassBalance.PropertyName.Operational,
                    FieldType = CswEnumNbtFieldType.Logical,
                    ServerManaged = true
                 } );

                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( BalanceOC )
                 {
                     PropName = CswNbtObjClassBalance.PropertyName.BalanceConfiguration,
                     FieldType = CswEnumNbtFieldType.Relationship,
                     IsFk = true,
                     FkType = CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(),
                     FkValue = BalanceConfigurationOC.ObjectClassId,
                     ServerManaged = true
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