using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28650
    /// </summary>
    public class CswUpdateSchema_01W_Case28650 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28650; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            foreach(CswNbtMetaDataNodeType UoMNT in UnitOfMeasureOC.getNodeTypes())
            {
                CswNbtMetaDataNodeTypeProp BaseUnitNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( UoMNT.NodeTypeId, CswNbtObjClassUnitOfMeasure.PropertyName.BaseUnit );
                BaseUnitNTP.StaticText = BaseUnitNTP.DefaultValue.AsText.Text;
                BaseUnitNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
                BaseUnitNTP.ServerManaged = false;
            }
        } //Update()

    }//class CswUpdateSchema_01V_Case28650

}//namespace ChemSW.Nbt.Schema