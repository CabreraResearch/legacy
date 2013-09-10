using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 29701
    /// </summary>
    public class RunBeforeEveryExecutionOfUpdater_02E_Case29701: CswUpdateSchemaTo
    {
        public override string Title { get { return "Pre-Script: Case 29701: OC Script"; } }
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29701; }
        }

        public override void update()
        {

            _makeProp( CswEnumNbtObjectClass.RequestMaterialDispenseClass );
            _makeProp( CswEnumNbtObjectClass.RequestMaterialCreateClass );

        } // update()

        private void _makeProp( CswEnumNbtObjectClass ObjClass )
        {
            CswNbtMetaDataObjectClass requestOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( ObjClass );

            CswNbtMetaDataObjectClassProp approvalLvlOCP = requestOC.getObjectClassProp( CswNbtObjClassRequestMaterialCreate.PropertyName.ApprovalLevel );

            if( null == approvalLvlOCP )
            {
                _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( requestOC )
                    {
                        PropName = CswNbtObjClassRequestMaterialCreate.PropertyName.ApprovalLevel,
                        FieldType = CswEnumNbtFieldType.List,
                        ListOptions = CswEnumNbtMaterialRequestApprovalLevel.All.ToString()
                    } );
            }
        }


    }//class RunBeforeEveryExecutionOfUpdater_02E_Case29701

}//namespace ChemSW.Nbt.Schema