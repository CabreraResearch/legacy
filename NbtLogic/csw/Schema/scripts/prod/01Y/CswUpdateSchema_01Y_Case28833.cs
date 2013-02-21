using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28833
    /// </summary>
    public class CswUpdateSchema_01Y_Case28833 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 28833; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass UnitOfMeasureOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.UnitOfMeasureClass );
            foreach( CswNbtObjClassUnitOfMeasure UoMNode in UnitOfMeasureOC.getNodes( false, false ) )
            {
                switch( UoMNode.Name.Text )
                {
                    case "g":
                        UoMNode.ConversionFactor.Base = 1.0;
                        UoMNode.ConversionFactor.Exponent = -3;
                        break;
                    case "mg":
                        UoMNode.ConversionFactor.Base = 1.0;
                        UoMNode.ConversionFactor.Exponent = -6;
                        break;
                    case "lb":
                        UoMNode.ConversionFactor.Base = 4.5359237;
                        UoMNode.ConversionFactor.Exponent = -1;
                        break;
                    case "ounces":
                        UoMNode.ConversionFactor.Base = 2.83495231;
                        UoMNode.ConversionFactor.Exponent = -2;
                        break;
                    case "ml":
                        UoMNode.ConversionFactor.Base = 1.0;
                        UoMNode.ConversionFactor.Exponent = -3;
                        break;
                    case "µL":
                        UoMNode.ConversionFactor.Base = 1.0;
                        UoMNode.ConversionFactor.Exponent = -6;
                        break;
                    case "gal":
                        UoMNode.ConversionFactor.Base = 3.78541178;
                        UoMNode.ConversionFactor.Exponent = 0;
                        break;
                    case "fluid ounces":
                        UoMNode.ConversionFactor.Base = 2.95735296;
                        UoMNode.ConversionFactor.Exponent = -2;
                        break;
                    case "cu.ft.":
                        UoMNode.ConversionFactor.Base = 2.83168466;
                        UoMNode.ConversionFactor.Exponent = 1;
                        break;
                    case "Weeks":
                        UoMNode.ConversionFactor.Base = 7.0;
                        UoMNode.ConversionFactor.Exponent = 0;
                        break;
                    case "Years":
                        UoMNode.ConversionFactor.Base = 3.65;
                        UoMNode.ConversionFactor.Exponent = 2;
                        break;
                    case "Hours":
                        UoMNode.ConversionFactor.Base = 4.1666667;
                        UoMNode.ConversionFactor.Exponent = -2;
                        break;
                    case "Minutes":
                        UoMNode.ConversionFactor.Base = 6.94444444;
                        UoMNode.ConversionFactor.Exponent = -4;
                        break;
                    case "Months":
                        UoMNode.ConversionFactor.Base = 3.04166666;
                        UoMNode.ConversionFactor.Exponent = 1;
                        break;
                    case "Seconds":
                        UoMNode.ConversionFactor.Base = 1.15740741;
                        UoMNode.ConversionFactor.Exponent = -5;
                        break;
                    case "mCi":
                        UoMNode.ConversionFactor.Base = 1.0;
                        UoMNode.ConversionFactor.Exponent = -3;
                        break;
                    case "Bq":
                        UoMNode.ConversionFactor.Base = 2.702703;
                        UoMNode.ConversionFactor.Exponent = -11;
                        break;
                }
                UoMNode.postChanges( false );
            }
        } //Update()
    }//class CswUpdateSchema_01Y_Case28833
}//namespace ChemSW.Nbt.Schema