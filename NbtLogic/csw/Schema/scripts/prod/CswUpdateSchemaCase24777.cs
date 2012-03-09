using System;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;
using ChemSW.Nbt.Sched;
using ChemSW.Audit;
using ChemSW.Nbt.PropTypes;



namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema for case 24777
    /// </summary>
    public class CswUpdateSchemaCase24777 : CswUpdateSchemaTo
    {

        public override void update()
        {
            // Fix property references to use firstpropversionid
            foreach( CswNbtMetaDataNodeTypeProp PropRefNTP in _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProps( CswNbtMetaDataFieldType.NbtFieldType.PropertyReference ) )
            {
                if( PropRefNTP.FKType == NbtViewPropIdType.NodeTypePropId.ToString() &&
                    PropRefNTP.ValuePropType == NbtViewPropIdType.NodeTypePropId.ToString() )
                {
                    CswNbtMetaDataNodeTypeProp RelationshipProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( PropRefNTP.FKValue );
                    CswNbtMetaDataNodeTypeProp ReferenceProp = _CswNbtSchemaModTrnsctn.MetaData.getNodeTypeProp( PropRefNTP.ValuePropId );
                    PropRefNTP.SetFK( PropRefNTP.FKType, RelationshipProp.FirstPropVersionId, PropRefNTP.ValuePropType, ReferenceProp.FirstPropVersionId );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase24777

}//namespace ChemSW.Nbt.Schema