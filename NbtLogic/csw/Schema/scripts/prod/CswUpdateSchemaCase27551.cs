using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27551
    /// </summary>
    public class CswUpdateSchemaCase27551 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass materialCompnentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialComponentClass );

            //make mixture readonly
            CswNbtMetaDataObjectClassProp mixtureOCP = materialCompnentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, Tristate.True );

            //make constituent required
            CswNbtMetaDataObjectClassProp constituentOCP = materialCompnentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( constituentOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, Tristate.True );

            //add constituentNTP to add layout
            foreach( CswNbtMetaDataNodeType materialComponentNT in materialCompnentOC.getNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp constituentNTP = materialComponentNT.getNodeTypeProp( CswNbtObjClassMaterialComponent.PropertyName.Constituent );
                if( null != constituentNTP )
                {
                    constituentNTP.updateLayout( CswNbtMetaDataNodeTypeLayoutMgr.LayoutType.Add, constituentNTP, true ); //add constituent to add layout
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase27551

}//namespace ChemSW.Nbt.Schema