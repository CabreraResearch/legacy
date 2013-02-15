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

                CswNbtMetaDataNodeTypeProp UnitConversionNTP = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypePropByObjectClassProp( UoMNT.NodeTypeId, CswNbtObjClassUnitOfMeasure.PropertyName.UnitConversion );
                UnitConversionNTP.StaticText = @"Conversion Factor should be set to the number required to make the current unit equal to the base unit.<br/>
Example: <strong>g(1E3) = kg</strong><br/>where g is the current unit, kg is the base unit, and 1E3 is the conversion factor.";
                UnitConversionNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, true );
            }
            foreach( CswNbtObjClassUnitOfMeasure UoMNode in UnitOfMeasureOC.getNodes( false, false ) )
            {
                UoMNode.UnitConversion.StaticText = @"Conversion Factor should be set to the number required to make the current unit equal to the base unit.<br/>
Example: <strong>g(1E3) = kg</strong><br/>where g is the current unit, kg is the base unit, and 1E3 is the conversion factor.";
                UoMNode.postChanges( false );
            }
        } //Update()

    }//class CswUpdateSchema_01V_Case28650

}//namespace ChemSW.Nbt.Schema