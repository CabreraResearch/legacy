using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.csw.Dev;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 28260
    /// </summary>
    public class CswUpdateSchema_01T_Case28260 : CswUpdateSchemaTo
    {
        public override CswDeveloper Author
        {
            get { return CswDeveloper.SS; }
        }

        public override int CaseNo
        {
            get { return 28260; }
        }

        public override void update()
        {
            // Fix servermanaged
            foreach( CswNbtMetaDataObjectClass objectClass in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses() )
            {
                foreach( CswNbtMetaDataObjectClassProp objectClassProp in objectClass.getObjectClassProps() )
                {
                    if( objectClassProp.ServerManaged )
                    {
                        foreach( CswNbtMetaDataNodeType nodeType in objectClass.getNodeTypes() )
                        {
                            CswNbtMetaDataNodeTypeProp nodeTypeProp = nodeType.getNodeTypePropByObjectClassProp( objectClassProp );
                            if( false == nodeTypeProp.ServerManaged )
                            {
                                nodeTypeProp.ServerManaged = objectClassProp.ServerManaged;
                            }
                        }
                    } // if(objectClassProp.ServerManaged)
                } // foreach(CswNbtMetaDataObjectClassProp objectClassProp in objectClass.getObjectClassProps())
            } // foreach(CswNbtMetaDataObjectClass objectClass in _CswNbtSchemaModTrnsctn.MetaData.getObjectClasses())

        } // Update()

    }//class CswUpdateSchema_01T_Case28260

}//namespace ChemSW.Nbt.Schema