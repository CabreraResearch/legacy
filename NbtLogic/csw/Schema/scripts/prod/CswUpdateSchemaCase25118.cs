using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Schema Update for case 25118
    /// </summary>
    public class CswUpdateSchemaCase25118 : CswUpdateSchemaTo
    {
        public override void update()
        {
            CswNbtMetaDataObjectClass MaterialOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass );
            CswNbtMetaDataObjectClass UnitOC = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass );

            // Material is missing Expiration Interval -- add it.
            _CswNbtSchemaModTrnsctn.createObjectClassProp( CswNbtMetaDataObjectClass.NbtObjectClass.MaterialClass,
                                                           CswNbtObjClassMaterial.ExpirationIntervalPropName,
                                                           CswNbtMetaDataFieldType.NbtFieldType.Quantity );

            // Add 'months' and 'years' as units
            IEnumerable<CswNbtMetaDataNodeType> UnitNTs = UnitOC.getNodeTypes();
            if( UnitNTs.Count() > 0 )
            {
                CswNbtMetaDataNodeType UnitNT = UnitNTs.First();
                if( UnitNT != null )
                {
                    CswNbtNode MonthsNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UnitNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    CswNbtObjClassUnitOfMeasure MonthsAsUnit = CswNbtNodeCaster.AsUnitOfMeasure( MonthsNode );
                    MonthsAsUnit.Name.Text = "months";
                    MonthsAsUnit.postChanges( true );

                    CswNbtNode YearsNode = _CswNbtSchemaModTrnsctn.Nodes.makeNodeFromNodeTypeId( UnitNT.NodeTypeId, CswNbtNodeCollection.MakeNodeOperation.WriteNode );
                    CswNbtObjClassUnitOfMeasure YearsAsUnit = CswNbtNodeCaster.AsUnitOfMeasure( YearsNode );
                    YearsAsUnit.Name.Text = "years";
                    YearsAsUnit.postChanges( true );
                }
            }

        }//Update()

    }//class CswUpdateSchemaCase25118

}//namespace ChemSW.Nbt.Schema
