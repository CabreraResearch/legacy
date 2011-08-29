using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using ChemSW.Nbt.PropTypes;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.TblDn;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassUnitOfMeasure : CswNbtObjClass
    {
        public static string NamePropertyName { get { return "Name"; } }
        public static string UnitTypePropertyName { get { return "Unit Type"; } }
        public static string ConversionFactorPropertyName { get { return "Conversion Factor"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public CswNbtObjClassUnitOfMeasure( CswNbtResources CswNbtResources )
            : base( CswNbtResources )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UnitOfMeasureClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode()
        {
            _CswNbtObjClassDefault.beforeCreateNode();
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode()
        {
            _CswNbtObjClassDefault.beforeWriteNode();
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText Name
        {
            get
            {
                return ( _CswNbtNode.Properties[NamePropertyName].AsText );
            }
        }

        public CswNbtNodePropScientific ConversionFactor
        {
            get
            {
                return ( _CswNbtNode.Properties[ConversionFactorPropertyName].AsScientific );
            }
        }

        public CswNbtNodePropRelationship UnitType
        {
            get
            {
                return ( _CswNbtNode.Properties[UnitTypePropertyName].AsRelationship );
            }
        }
        #endregion



    }//CswNbtObjClassUnitOfMeasure

}//namespace ChemSW.Nbt.ObjClasses
