using System.Linq;
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
                name = CswTools.UppercaseFirst( CswNbtKioskModeRuleName.Move._Name ),
                imgUrl = "Images/newicons/KioskMode/Move_code39.png"
            } );

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.Containers ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswNbtKioskModeRuleName.Owner._Name ),
                    imgUrl = "Images/newicons/KioskMode/Owner_code39.png"
                } );
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswNbtKioskModeRuleName.Transfer._Name ),
                    imgUrl = "Images/newicons/KioskMode/Transfer_code39.png"
                } );
                CswNbtPermit permissions = new CswNbtPermit( NbtResources );
                if( permissions.can( CswNbtActionName.DispenseContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( CswNbtKioskModeRuleName.Dispense._Name ),
                        imgUrl = "Images/newicons/KioskMode/Dispense_code39.png"
                    } );
                }
                if( permissions.can( CswNbtActionName.DisposeContainer ) )
                {
                    kioskModeData.AvailableModes.Add( new Mode
                    {
                        name = CswTools.UppercaseFirst( CswNbtKioskModeRuleName.Dispose._Name ),
                        imgUrl = "Images/newicons/KioskMode/Dispose_code39.png"
                    } );
                }
            }

            if( NbtResources.Modules.IsModuleEnabled( CswNbtModuleName.IMCS ) )
            {
                kioskModeData.AvailableModes.Add( new Mode
                {
                    name = CswTools.UppercaseFirst( CswNbtKioskModeRuleName.Status._Name ),
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
            if( _isModeScan( KioskModeData.OperationData.LastItemScanned, KioskModeData ) )
            {
                KioskModeData.OperationData.Mode = KioskModeData.OperationData.LastItemScanned;
                _setFields( NbtResources, KioskModeData.OperationData );
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

        private static void _setFields( CswNbtResources NbtResources, OperationData OpData )
        {
            CswNbtKioskModeRule rule = CswNbtKioskModeRuleFactory.Make( NbtResources, OpData.Mode );
            rule.SetFields( ref OpData );
        }

        private static bool _isModeScan( string ScannedMode, KioskModeData KMData )
        {
            bool Ret = KMData.AvailableModes.Any<Mode>( mode => mode.name.ToLower().Equals( ScannedMode.ToLower() ) );
            return Ret;
        }

        #endregion
    }

}