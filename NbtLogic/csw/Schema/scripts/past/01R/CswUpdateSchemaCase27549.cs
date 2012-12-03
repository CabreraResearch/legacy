using System;
using System.Linq;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27549
    /// </summary>
    public class CswUpdateSchemaCase27549 : CswUpdateSchemaTo
    {
        /// <summary>
        /// Update logic
        /// </summary>
        public override void update()
        {

            CswNbtMetaDataNodeType materialComponentNT = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Material Component" );
            if( null != materialComponentNT )
            {
                string templateText = CswNbtMetaData.MakeTemplateEntry( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
                materialComponentNT.setNameTemplateText( templateText );
            }


        }//Update()
    }

}//namespace ChemSW.Nbt.Schema