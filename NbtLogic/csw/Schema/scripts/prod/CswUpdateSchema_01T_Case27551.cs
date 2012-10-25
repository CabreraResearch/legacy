using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using System.Collections.Generic;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 27551
    /// </summary>
    public class CswUpdateSchema_01T_Case27551 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 27551; }
        }

        public override void update()
        {
            //part 1 - make Material Components Mixture server managed
            CswNbtMetaDataObjectClass materialComponentOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( NbtObjectClass.MaterialComponentClass );
            CswNbtMetaDataObjectClassProp mixtureOCP = materialComponentOC.getObjectClassProp( CswNbtObjClassMaterialComponent.PropertyName.Mixture );

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( mixtureOCP, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.servermanaged, true );

            //part 3 - mark the Components View as Demo
            List<CswNbtView> componentsViews = _CswNbtSchemaModTrnsctn.ViewSelect.restoreViews( "Components" );
            foreach( CswNbtView componentsView in componentsViews )
            {
                if( componentsView.Visibility.Equals( NbtViewVisibility.Global ) && componentsView.Category.Equals( "Materials" ) )
                {
                    componentsView.IsDemo = true;
                    componentsView.save();
                }
            }
        }

        //Update()

    }//class CswUpdateSchemaCase27551

}//namespace ChemSW.Nbt.Schema