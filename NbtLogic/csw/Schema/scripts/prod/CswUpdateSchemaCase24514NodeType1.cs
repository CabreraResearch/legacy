using System;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 24514 and case 26954
    /// </summary>
    public class CswUpdateSchemaCase24514NodeType1 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass RequestOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.RequestClass );
            foreach( CswNbtMetaDataNodeType RequestNt in RequestOc.getLatestVersionNodeTypes() )
            {
                CswNbtMetaDataNodeTypeProp NameNtp = RequestNt.getNodeTypePropByObjectClassProp(CswNbtObjClassRequest.PropertyName.Name.ToString());
                CswNbtMetaDataNodeTypeProp UserNtp = RequestNt.getNodeTypePropByObjectClassProp(CswNbtObjClassRequest.PropertyName.Requestor.ToString());
                CswNbtMetaDataNodeTypeProp DateNtp = RequestNt.getNodeTypePropByObjectClassProp(CswNbtObjClassRequest.PropertyName.SubmittedDate.ToString());
                RequestNt.NameTemplateValue = "";
                RequestNt.addNameTemplateText(NameNtp.PropName);
                RequestNt.addNameTemplateText(UserNtp.PropName);
                RequestNt.addNameTemplateText(DateNtp.PropName);
            }


        }//Update()

    }//class CswUpdateSchemaCase24514NodeType

}//namespace ChemSW.Nbt.Schema