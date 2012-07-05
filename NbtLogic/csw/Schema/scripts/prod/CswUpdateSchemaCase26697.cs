using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 26997
    /// </summary>
    public class CswUpdateSchemaCase26697 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataNodeType LabSafetyCheckListNt = _CswNbtSchemaModTrnsctn.MetaData.getNodeType( "Lab Safety Checklist" );
            if(null != LabSafetyCheckListNt)
            {
                CswNbtMetaDataNodeType LabSafetyLatest = LabSafetyCheckListNt.getNodeTypeLatestVersion();
                foreach ( CswNbtMetaDataNodeTypeProp NodeTypeProp in LabSafetyLatest.getNodeTypeProps() )
                {
                    if(NodeTypeProp.getFieldType().FieldType == CswNbtMetaDataFieldType.NbtFieldType.Question)
                    {
                        CswCommaDelimitedString AllowedAnswers = new CswCommaDelimitedString();
                        AllowedAnswers.FromString(NodeTypeProp.ListOptions);
                        if( AllowedAnswers.Contains( "Yes", false ) )
                        {
                            NodeTypeProp.Extended = "Yes";
                        }
                    }
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase26697

}//namespace ChemSW.Nbt.Schema