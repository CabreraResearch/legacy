
using ChemSW.Core;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27769
    /// </summary>
    public class CswUpdateSchema_01S_Case27769 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {
            CswNbtMetaDataNodeType TimeNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Unit (Time)" );
            if( null != TimeNT )
            {
                foreach( CswNbtObjClassUnitOfMeasure TimeNode in TimeNT.getNodes( false, false ) )
                {
                    if( "Months" == TimeNode.Name.Text )
                    {
                        //According to Google, 1 month = 30.4368 days (based on averages, I'm assuming)
                        TimeNode.ConversionFactor.Base = 3.285496;
                        TimeNode.ConversionFactor.Exponent = -2;
                        TimeNode.postChanges( false );
                    }
                }
            }

            CswNbtMetaDataObjectClass MaterialOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );
            foreach( CswNbtMetaDataNodeType MaterialNt in MaterialOc.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp SpecificGravityNtp = MaterialNt.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.SpecificGravity );
                if( null != SpecificGravityNtp )
                {
                    SpecificGravityNtp.Attribute1 = CswConvert.ToDbVal( true ).ToString();
                }
            }
        }

        public override CswDeveloper Author
        {
            get { return CswDeveloper.BV; }
        }

        public override int CaseNo
        {
            get { return 27769; }
        }

        //Update()

    }

}//namespace ChemSW.Nbt.Schema