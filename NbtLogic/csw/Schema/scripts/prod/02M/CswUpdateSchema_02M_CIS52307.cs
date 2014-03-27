using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02M_CIS52307: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 52307; }
        }

        public override string Title
        {
            get { return "Configure Testing Lab add layout"; }
        }

        public override string AppendToScriptName()
        {
            return "B";
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass TestingLabOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.TestingLabClass );
            foreach( CswNbtMetaDataNodeType TestingLabNT in TestingLabOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp NameNTP = TestingLabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTestingLab.PropertyName.Name );
                CswNbtMetaDataNodeTypeProp SampleDeliveryLocationNTP = TestingLabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTestingLab.PropertyName.SampleDeliveryLocation );
                CswNbtMetaDataNodeTypeProp SampleDeliveryRequiredNTP = TestingLabNT.getNodeTypePropByObjectClassProp( CswNbtObjClassTestingLab.PropertyName.SampleDeliveryRequired );

                NameNTP.updateLayout( CswEnumNbtLayoutType.Add, true );
                SampleDeliveryLocationNTP.updateLayout( CswEnumNbtLayoutType.Add, true );
                SampleDeliveryRequiredNTP.updateLayout( CswEnumNbtLayoutType.Add, true );
            }
        }
    }
}