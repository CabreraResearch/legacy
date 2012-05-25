using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514
    /// </summary>
    public class CswUpdateSchemaCase24514 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.createObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass,
                                                          IconFileName: "request.png",
                                                          AuditLevel: true,
                                                          UseBatchEntry: false );
        }//Update()

    }//class CswUpdateSchemaCase24514

}//namespace ChemSW.Nbt.Schema