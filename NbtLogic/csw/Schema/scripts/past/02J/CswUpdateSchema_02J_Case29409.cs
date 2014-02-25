using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02J_Case29409: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 29409; }
        }

        public override string Title
        {
            get { return "Chemical Approved for Receiving not longer required"; }
        }

        public override void update()
        {

            CswNbtMetaDataObjectClass ChemicalOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ChemicalClass );
            CswNbtMetaDataObjectClassProp ApprovedForReceivingOCP = ChemicalOC.getObjectClassProp( CswNbtObjClassChemical.PropertyName.ApprovedForReceiving );

            //Prop is no longer required
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( ApprovedForReceivingOCP, CswEnumNbtObjectClassPropAttributes.isrequired, false );

            //Prop defaults to "?"
            _CswNbtSchemaModTrnsctn.MetaData.SetObjectClassPropDefaultValue( ApprovedForReceivingOCP, CswEnumTristate.Null );

        } // update()

    }

}//namespace ChemSW.Nbt.Schema