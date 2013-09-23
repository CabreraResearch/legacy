using System.Collections.Generic;
using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02G_Case30756 : CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.CF; }
        }

        public override int CaseNo
        {
            get { return 30756; }
        }

        public override string ScriptName
        {
            get { return "02G_" + CaseNo; }
        }

        public override string Title
        {
            get { return "Correct My Containers View"; }
        }

        public override void update()
        {
            List<CswNbtView> MyContainersViews = _CswNbtSchemaModTrnsctn.restoreViews( "My Expiring Containers" );
            CswNbtMetaDataObjectClass ContainerOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswEnumNbtObjectClass.ContainerClass );
            CswNbtMetaDataObjectClassProp OwnerOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.PropertyName.Owner );

            foreach( CswNbtView MyContainersView in MyContainersViews )
            {
                if( MyContainersView.ViewMode == CswEnumNbtViewRenderingMode.Grid )
                {
                    CswNbtView View = MyContainersView;
                    MyContainersView.Root.eachRelationship( Relationship =>
                    {
                        if( Relationship.SecondMatches(ContainerOc) )
                        {
                            View.AddViewPropertyAndFilter( Relationship, OwnerOcp, "me" );
                            View.save();
                        }
                    }, null);
                }
            }


        } // update()

    }

}//namespace ChemSW.Nbt.Schema