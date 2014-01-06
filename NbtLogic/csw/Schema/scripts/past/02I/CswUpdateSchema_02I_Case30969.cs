using ChemSW.Nbt.csw.Dev;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update
    /// </summary>
    public class CswUpdateSchema_02I_Case30969: CswUpdateSchemaTo
    {
        public override CswEnumDeveloper Author
        {
            get { return CswEnumDeveloper.MB; }
        }

        public override int CaseNo
        {
            get { return 30969; }
        }

        public override string Title
        {
            get { return "Remove Legacy Id from all layouts and make it server managed"; }
        }

        public override void update()
        {

            foreach( CswNbtMetaDataObjectClass ObjClass in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses() )
            {
                CswNbtMetaDataObjectClassProp LegacyIdOCP = ObjClass.getObjectClassProp( CswNbtObjClass.PropertyName.LegacyId );
                _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( LegacyIdOCP, CswEnumNbtObjectClassPropAttributes.servermanaged, true );

                foreach( CswNbtMetaDataNodeTypeProp LegacyIdNTP in LegacyIdOCP.getNodeTypeProps() )
                {
                    LegacyIdNTP.removeFromAllLayouts();
                }
            }

        } // update()

    }

}//namespace ChemSW.Nbt.Schema