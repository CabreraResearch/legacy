using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27864
    /// </summary>
    public class CswUpdateSchema_01U_Case27864 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27864; }
        }

        public override void update()
        {
            CswNbtMetaDataObjectClass materialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialClass );

            #region Create and set the material Id for all material NTs
            CswSequenceName materialIDSequenceName = new CswSequenceName( "Material Id nbt" );

            if( false == _CswNbtSchemaModTrnsctn.doesSequenceExist( materialIDSequenceName ) )
            {
                int seqId = _CswNbtSchemaModTrnsctn.makeSequence( materialIDSequenceName, "M", "", 6, 0 );

                foreach( CswNbtMetaDataNodeType matNT in materialOC.getNodeTypes() )
                {
                    CswNbtMetaDataNodeTypeProp materialIdNTP = matNT.getNodeTypePropByObjectClassProp( CswNbtObjClassMaterial.PropertyName.MaterialId );
                    materialIdNTP.setSequence( seqId );
                }
            }
            #endregion
        }

        //Update()

    }//class CswUpdateSchemaCase27864

}//namespace ChemSW.Nbt.Schema