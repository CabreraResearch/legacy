using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Core;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27864
    /// </summary>
    public class CswUpdateSchema_01U_Case27864_part2 : CswUpdateSchemaTo
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
            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );

            #region Fix existing Material Components

            foreach( CswNbtObjClassMaterialComponent matCompNode in materialComponentOC.getNodes( false, false, false, true ) )
            {
                if( Tristate.Null == matCompNode.Active.Checked )
                {
                    matCompNode.Active.Checked = Tristate.False;
                    matCompNode.postChanges( false );
                }
            }

            #endregion

            #region fix existing material NTs by setting the vars

            foreach( CswNbtNode materialNode in materialOC.getNodes( false, false, false, true ) )
            {
                if( string.IsNullOrEmpty( materialNode.Properties[CswNbtObjClassMaterial.PropertyName.MaterialId].AsSequence.Sequence ) )
                {
                    materialNode.Properties[CswNbtObjClassMaterial.PropertyName.MaterialId].AsSequence.setSequenceValue();
                }

                if( Tristate.Null == materialNode.Properties[CswNbtObjClassMaterial.PropertyName.Approved].AsLogical.Checked )
                {
                    materialNode.Properties[CswNbtObjClassMaterial.PropertyName.Approved].AsLogical.Checked = Tristate.False;
                }

                materialNode.postChanges( false );
            }

            #endregion

        }

        //Update()

    }//class CswUpdateSchemaCase27864

}//namespace ChemSW.Nbt.Schema