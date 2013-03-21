using System.Runtime.Serialization;
using ChemSW.Core;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.Actions.KioskMode;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceKioskMode
    {
        private static CswNbtResources _CswNbtResources;

        public CswNbtWebServiceKioskMode( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        #region Data Contracts

        [DataContract]
        public class KioskModeDataReturn: CswWebSvcReturn
        {
            public KioskModeDataReturn()
            {
                Data = new KioskModeData();
            }
            [DataMember]
            public KioskModeData Data;
        }

        #endregion

        public static void GetAvailableModes( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            CswNbtObjClassRole currentUserRoleNode = NbtResources.Nodes.makeRoleNodeFromRoleName( NbtResources.CurrentNbtUser.Rolename );

            KioskModeData kioskModeData = new KioskModeData();

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = CswTools.UppercaseFirst( Modes.Move ),
                imgUrl = "Images/newicons/KioskMode/Move_code39.png"
            } );

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Owner ),
                    imgUrl = "Images/newicons/KioskMode/Owner_code39.png"
                } );
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Transfer ),
                    imgUrl = "Images/newicons/KioskMode/Transfer_code39.png"
                } );
                CswNbtPermit permissions = new CswNbtPermit( NbtResources );
                if( permissions.can( CswNbtActionName.DispenseContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( Modes.Dispense ),
                        imgUrl = "Images/newicons/KioskMode/Dispense_code39.png"
                    } );
                }
                if( permissions.can( CswNbtActionName.DisposeContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( Modes.Dispose ),
                        imgUrl = "Images/newicons/KioskMode/Dispose_code39.png"
                    } );
                }
            }

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( Modes.Status ),
                    imgUrl = "Images/newicons/KioskMode/Status_code39.png"
                } );
            }

            kioskModeData.AvailableModes.Add( new Mode
            {
                name = "Reset",
                imgUrl = "Images/newicons/KioskMode/Reset_code39.png"
            } );

            Return.Data = kioskModeData;
        }

        public static void HandleScan( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            if( _isModeScan( KioskModeData.OperationData.LastItemScanned ) )
            {
                KioskModeData.OperationData.Mode = KioskModeData.OperationData.LastItemScanned;
                _setFields( KioskModeData.OperationData );
            }
            else
            {
                CswNbtKioskModeRule rule = CswNbtKioskModeRuleFactory.Make( NbtResources, KioskModeData.OperationData.Mode );
                if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field2.Value ) && false == KioskModeData.OperationData.Field2.ServerValidated )
                {
                    rule.ValidateFieldTwo( ref KioskModeData.OperationData );
                }
                else if( false == string.IsNullOrEmpty( KioskModeData.OperationData.Field1.Value ) && false == KioskModeData.OperationData.Field1.ServerValidated )
                {
                    rule.ValidateFieldOne( ref KioskModeData.OperationData );
                }
                else
                {
                    KioskModeData.OperationData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    KioskModeData.OperationData.ModeServerValidated = false;
                }
            }
            Return.Data = KioskModeData;
        }

        public static void CommitOperation( ICswResources CswResources, KioskModeDataReturn Return, KioskModeData KioskModeData )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            OperationData OpData = KioskModeData.OperationData;

            CswNbtKioskModeRule rule = CswNbtKioskModeRuleFactory.Make( NbtResources, OpData.Mode );
            rule.CommitOperation( ref OpData );

            KioskModeData.OperationData = OpData;
            Return.Data = KioskModeData;
        }

        #region Private Methods

        private static void _setFields( OperationData OpData )
        {
            OpData.ModeServerValidated = true;
            OpData.ModeStatusMsg = string.Empty;
            OpData.Field1 = new Field();
            OpData.Field2 = new Field();
            OpData.ScanTextLabel = "Scan a barcode:";
            string selectedOp = OpData.Mode.ToLower();
            switch( selectedOp )
            {
                case Modes.Move:
                    OpData.Field1.Name = "Location:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Owner:
                    OpData.Field1.Name = "User:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Transfer:
                    OpData.Field1.Name = "User:";
                    OpData.Field2.Name = "Item:";
                    break;
                case Modes.Dispense:
                    OpData.Field1.Name = "Container:";
                    OpData.Field2.Name = "Quantity:";
                    break;
                case Modes.Dispose:
                    OpData.Field1.Name = "Container:";
                    OpData.Field2.ServerValidated = true;
                    break;
                case Modes.Status:
                    OpData.Field1.Name = "Status:";
                    OpData.Field2.Name = "Item:";
                    break;
                default:
                    OpData.ModeStatusMsg = "Error: Scanned mode does not exist or is unavailable";
                    OpData.ModeServerValidated = false;
                    break;
            }
        }

        private static bool _isModeScan( string ScannedMode )
        {
            bool Ret = false;
            foreach( string mode in Modes.All )
            {
                if( mode.Equals( ScannedMode.ToLower() ) )
                {
                    Ret = true;
                }
            }
            return Ret;
        }

        #endregion
    }

}