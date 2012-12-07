using System.Collections.Generic;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassFireClassExemptAmount : CswNbtObjClass
    {
        #region Properties and ctor

        public sealed class PropertyName
        {
            public const string SetName = "Set Name";
            public const string SortOrder = "Sort Order";
            public const string FireHazardClassType = "Fire Hazard Class Type";
            public const string HazardType = "Hazard Type";
            public const string Material = "Material";
            public const string StorageSolidExemptAmount = "Storage Solid Exempt Amount";
            public const string StorageSolidExemptFootnotes = "Storage Solid Exempt Footnotes";
            public const string StorageLiquidExemptAmount = "Storage Liquid Exempt Amount";
            public const string StorageLiquidExemptFootnotes = "Storage Liquid Exempt Footnotes";
            public const string StorageGasExemptAmount = "Storage Gas Exempt Amount";
            public const string StorageGasExemptFootnotes = "Storage Gas Exempt Footnotes";
            public const string ClosedSolidExemptAmount = "Closed Solid Exempt Amount";
            public const string ClosedSolidExemptFootnotes = "Closed Solid Exempt Footnotes";
            public const string ClosedLiquidExemptAmount = "Closed Liquid Exempt Amount";
            public const string ClosedLiquidExemptFootnotes = "Closed Liquid Exempt Footnotes";
            public const string ClosedGasExemptAmount = "Closed Gas Exempt Amount";
            public const string ClosedGasExemptFootnotes = "Closed Gas Exempt Footnotes";
            public const string OpenSolidExemptAmount = "Open Solid Exempt Amount";
            public const string OpenSolidExemptFootnotes = "Open Solid Exempt Footnotes";
            public const string OpenLiquidExemptAmount = "Open Liquid Exempt Amount";
            public const string OpenLiquidExemptFootnotes = "Open Liquid Exempt Footnotes";
        }

        private CswNbtObjClassDefault _CswNbtObjClassDefault;

        public CswNbtObjClassFireClassExemptAmount( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.FireClassExemptAmountClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassFireClassExemptAmount
        /// </summary>
        public static implicit operator CswNbtObjClassFireClassExemptAmount( CswNbtNode Node )
        {
            CswNbtObjClassFireClassExemptAmount ret = null;
            if( null != Node && _Validate( Node, NbtObjectClass.FireClassExemptAmountClass ) )
            {
                ret = (CswNbtObjClassFireClassExemptAmount) Node.ObjClass;
            }
            return ret;
        }

        #endregion Properties and ctor

        #region Enums

        public sealed class FireHazardClassTypes : CswEnum<FireHazardClassTypes>
        {
            private FireHazardClassTypes( string Name ) : base( Name ) { }
            public static IEnumerable<FireHazardClassTypes> _All { get { return All; } }
            public static implicit operator FireHazardClassTypes( string str )
            {
                FireHazardClassTypes ret = Parse( str );
                return ret;
            }
            public static readonly FireHazardClassTypes Aero1 = new FireHazardClassTypes( "Aero-1" );
            public static readonly FireHazardClassTypes Aero2 = new FireHazardClassTypes( "Aero-2" );
            public static readonly FireHazardClassTypes Aero3 = new FireHazardClassTypes( "Aero-3" );
            public static readonly FireHazardClassTypes Carc = new FireHazardClassTypes( "Carc" );
            public static readonly FireHazardClassTypes CFDBalled = new FireHazardClassTypes( "CF/D (balled)" );
            public static readonly FireHazardClassTypes CFDLoose = new FireHazardClassTypes( "CF/D (loose)" );
            public static readonly FireHazardClassTypes CLII = new FireHazardClassTypes( "CL-II" );
            public static readonly FireHazardClassTypes CLIIIA = new FireHazardClassTypes( "CL-IIIA" );
            public static readonly FireHazardClassTypes CLIIIB = new FireHazardClassTypes( "CL-IIIB" );
            public static readonly FireHazardClassTypes Corr = new FireHazardClassTypes( "Corr" );
            public static readonly FireHazardClassTypes CRYFG = new FireHazardClassTypes( "CRY-FG" );
            public static readonly FireHazardClassTypes CRYOXY = new FireHazardClassTypes( "CRY-OXY" );
            public static readonly FireHazardClassTypes Exp = new FireHazardClassTypes( "Exp" );
            public static readonly FireHazardClassTypes FGGas = new FireHazardClassTypes( "FG (gaseous)" );
            public static readonly FireHazardClassTypes FGLiquid = new FireHazardClassTypes( "FG (liquified)" );
            public static readonly FireHazardClassTypes FL1A = new FireHazardClassTypes( "FL-1A" );
            public static readonly FireHazardClassTypes FL1B = new FireHazardClassTypes( "FL-1B" );
            public static readonly FireHazardClassTypes FL1C = new FireHazardClassTypes( "FL-1C" );
            public static readonly FireHazardClassTypes FLComb = new FireHazardClassTypes( "FL-Comb" );
            public static readonly FireHazardClassTypes FS = new FireHazardClassTypes( "FS" );
            public static readonly FireHazardClassTypes HT = new FireHazardClassTypes( "H.T." );
            public static readonly FireHazardClassTypes Irr = new FireHazardClassTypes( "Irr" );
            public static readonly FireHazardClassTypes OHH = new FireHazardClassTypes( "OHH" );
            public static readonly FireHazardClassTypes Oxy1 = new FireHazardClassTypes( "Oxy1" );
            public static readonly FireHazardClassTypes Oxy2 = new FireHazardClassTypes( "Oxy2" );
            public static readonly FireHazardClassTypes Oxy3 = new FireHazardClassTypes( "Oxy3" );
            public static readonly FireHazardClassTypes Oxy4 = new FireHazardClassTypes( "Oxy4" );
            public static readonly FireHazardClassTypes OxyGas = new FireHazardClassTypes( "Oxy-Gas" );
            public static readonly FireHazardClassTypes OxyGasLiquid = new FireHazardClassTypes( "Oxy-Gas (liquid)" );
            public static readonly FireHazardClassTypes PeroxDet = new FireHazardClassTypes( "Perox-Det" );
            public static readonly FireHazardClassTypes PeroxI = new FireHazardClassTypes( "Perox-I" );
            public static readonly FireHazardClassTypes PeroxII = new FireHazardClassTypes( "Perox-II" );
            public static readonly FireHazardClassTypes PeroxIII = new FireHazardClassTypes( "Perox-III" );
            public static readonly FireHazardClassTypes PeroxIV = new FireHazardClassTypes( "Perox-IV" );
            public static readonly FireHazardClassTypes PeroxV = new FireHazardClassTypes( "Perox-V" );
            public static readonly FireHazardClassTypes Pyro = new FireHazardClassTypes( "Pyro" );
            public static readonly FireHazardClassTypes RADAlpha = new FireHazardClassTypes( "RAD-Alpha" );
            public static readonly FireHazardClassTypes RADBeta = new FireHazardClassTypes( "RAD-Beta" );
            public static readonly FireHazardClassTypes RADGamma = new FireHazardClassTypes( "RAD-Gamma" );
            public static readonly FireHazardClassTypes Sens = new FireHazardClassTypes( "Sens" );
            public static readonly FireHazardClassTypes Tox = new FireHazardClassTypes( "Tox" );
            public static readonly FireHazardClassTypes UR1 = new FireHazardClassTypes( "UR-1" );
            public static readonly FireHazardClassTypes UR2 = new FireHazardClassTypes( "UR-2" );
            public static readonly FireHazardClassTypes UR3 = new FireHazardClassTypes( "UR-3" );
            public static readonly FireHazardClassTypes UR4 = new FireHazardClassTypes( "UR-4" );
            public static readonly FireHazardClassTypes WR1 = new FireHazardClassTypes( "WR-1" );
            public static readonly FireHazardClassTypes WR2 = new FireHazardClassTypes( "WR-2" );
            public static readonly FireHazardClassTypes WR3 = new FireHazardClassTypes( "WR-3" );
        }

        #endregion Enums

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );

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

        public override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData && null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship SetName { get { return _CswNbtNode.Properties[PropertyName.SetName]; } }
        public CswNbtNodePropNumber SortOrder { get { return _CswNbtNode.Properties[PropertyName.SortOrder]; } }
        public CswNbtNodePropList FireHazardClassType { get { return _CswNbtNode.Properties[PropertyName.FireHazardClassType]; } }
        public CswNbtNodePropList HazardType { get { return _CswNbtNode.Properties[PropertyName.HazardType]; } }
        public CswNbtNodePropText Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        public CswNbtNodePropQuantity StorageSolidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.StorageSolidExemptAmount]; } }
        public CswNbtNodePropText StorageSolidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.StorageSolidExemptFootnotes]; } }
        public CswNbtNodePropQuantity StorageLiquidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.StorageLiquidExemptAmount]; } }
        public CswNbtNodePropText StorageLiquidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.StorageLiquidExemptFootnotes]; } }
        public CswNbtNodePropQuantity StorageGasExemptAmount { get { return _CswNbtNode.Properties[PropertyName.StorageGasExemptAmount]; } }
        public CswNbtNodePropText StorageGasExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.StorageGasExemptFootnotes]; } }
        public CswNbtNodePropQuantity ClosedSolidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.ClosedSolidExemptAmount]; } }
        public CswNbtNodePropText ClosedSolidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.ClosedSolidExemptFootnotes]; } }
        public CswNbtNodePropQuantity ClosedLiquidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.ClosedLiquidExemptAmount]; } }
        public CswNbtNodePropText ClosedLiquidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.ClosedLiquidExemptFootnotes]; } }
        public CswNbtNodePropQuantity ClosedGasExemptAmount { get { return _CswNbtNode.Properties[PropertyName.ClosedGasExemptAmount]; } }
        public CswNbtNodePropText ClosedGasExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.ClosedGasExemptFootnotes]; } }
        public CswNbtNodePropQuantity OpenSolidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.OpenSolidExemptAmount]; } }
        public CswNbtNodePropText OpenSolidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.OpenSolidExemptFootnotes]; } }
        public CswNbtNodePropQuantity OpenLiquidExemptAmount { get { return _CswNbtNode.Properties[PropertyName.OpenLiquidExemptAmount]; } }
        public CswNbtNodePropText OpenLiquidExemptFootnotes { get { return _CswNbtNode.Properties[PropertyName.OpenLiquidExemptFootnotes]; } }

        #endregion

    }//CswNbtObjClassFireClassExemptAmount

}//namespace ChemSW.Nbt.ObjClasses
