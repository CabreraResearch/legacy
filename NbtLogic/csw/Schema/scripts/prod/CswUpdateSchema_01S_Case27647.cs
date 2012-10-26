
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27647
    /// </summary>
    public class CswUpdateSchema_01S_Case27647 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataObjectClass SizeOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.SizeClass );

            // moved to RunBeforeEveryExecutionOfUpdater_01OC
            //CswNbtMetaDataObjectClassProp UnitCountOcp = _CswNbtSchemaModTrnsctn.createObjectClassProp( new CswNbtWcfMetaDataModel.ObjectClassProp( SizeOc )
            //{
            //    PropName = CswNbtObjClassSize.PropertyName.UnitCount,
            //    FieldType = CswNbtMetaDataFieldType.NbtFieldType.Number,
            //    IsRequired = true,
            //    SetValOnAdd = true,
            //    NumberMinValue = 1,
            //    NumberPrecision = 0
            //} );

            CswNbtMetaDataNodeType SizeNt = SizeOc.FirstNodeType;
            if( null != SizeNt )
            {
                CswNbtMetaDataNodeTypeProp UnitCountNtp = SizeNt.getNodeTypePropByObjectClassProp( CswNbtObjClassSize.PropertyName.UnitCount );
                UnitCountNtp.DefaultValue.AsNumber.Value = 1;
            }

            foreach( CswNbtObjClassSize SizeNode in SizeOc.getNodes( false, false ) )
            {
                SizeNode.UnitCount.Value = 1;
                SizeNode.postChanges( false );
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27647; }
        }

        //Update()

    }//class CswUpdateSchema_01S_Case27647

}//namespace ChemSW.Nbt.Schema