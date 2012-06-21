using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Mail;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.ServiceDrivers
{
    public class CswNbtSdInventoryLevelMgr
    {
        private CswNbtResources _CswNbtResources;
        private CswNbtObjClassInventoryLevel _InventoryLevel;

        public CswNbtSdInventoryLevelMgr( CswNbtResources Resources )
        {
            _CswNbtResources = Resources;
        }
        public CswNbtSdInventoryLevelMgr( CswNbtResources Resources, CswNbtObjClassInventoryLevel InventoryLevel )
        {
            _CswNbtResources = Resources;
            _InventoryLevel = InventoryLevel;
        }

        #region Validation

        public bool doSendEmail()
        {
            return isLevelPastThreshhold() && _InventoryLevel.LastNotified.DateTimeValue <= DateTime.Now.AddDays( -1 );
        }

        public bool isLevelPastThreshhold()
        {
            bool Ret = false;

            switch( _InventoryLevel.Type.Value )
            {
                case CswNbtObjClassInventoryLevel.Types.Maximum:
                    Ret = ( _InventoryLevel.CurrentQuantity.Quantity > _InventoryLevel.Level.Quantity );
                    break;
                case CswNbtObjClassInventoryLevel.Types.Minimum:
                    Ret = ( _InventoryLevel.CurrentQuantity.Quantity < _InventoryLevel.Level.Quantity );
                    break;
            }

            return Ret;
        }

        #endregion Validation

        #region Email

        private void _sendEmail( string Recipient )
        {
            string Subject = "";
            string Message = "";

            Collection<CswMailMessage> EmailMessage = _CswNbtResources.makeMailMessages( Subject, Message,
                                                                                        Recipient );
            _CswNbtResources.sendEmailNotification( EmailMessage );
        }

        /// <summary>
        /// Sends notification email and returns Now
        /// </summary>
        /// <returns></returns>
        public DateTime sendPastThreshholdEmail()
        {
            foreach( CswNbtObjClassUser User in _InventoryLevel.Subscribe.SelectedUsers() )
            {
                if( false == string.IsNullOrEmpty( User.Email ) )
                {
                    _sendEmail( User.Email );
                }
            }
            return DateTime.Now;
        }

        #endregion Email

        #region Inventory

        public Collection<CswPrimaryKey> getInventoryLevelLocationIds()
        {
            Collection<CswPrimaryKey> Ret = new Collection<CswPrimaryKey>();
            try
            {
                CswNbtLocationTree LocationTree = new CswNbtLocationTree( _CswNbtResources, _InventoryLevel.Location, Int32.MaxValue );
                Ret = LocationTree.LocationIds;
            }
            catch( Exception Ex )
            {
                throw new CswDniException( "Failed to get Inventory Level locations", Ex );
            }
            return Ret;
        }

        private CswNbtView _QuantityView = null;
        public CswNbtView QuantityView
        {
            get
            {
                if( null == _QuantityView )
                {
                    _QuantityView = new CswNbtView( _CswNbtResources );
                    CswNbtMetaDataObjectClass LocationOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.LocationClass );
                    CswNbtViewRelationship LocationRel = _QuantityView.AddViewRelationship( LocationOc, false );

                    Collection<CswPrimaryKey> LocationIds = getInventoryLevelLocationIds();
                    if( LocationIds.Count > 0 )
                    {
                        LocationRel.NodeIdsToFilterIn = LocationIds;
                    }
                    else
                    {
                        LocationRel.NodeIdsToFilterIn.Add( _InventoryLevel.Location.NodeId );
                    }

                    CswNbtMetaDataObjectClass ContainerOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ContainerClass );
                    CswNbtMetaDataObjectClassProp LocationOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.LocationPropertyName );
                    CswNbtMetaDataObjectClassProp MaterialOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.MaterialPropertyName );
                    CswNbtMetaDataObjectClassProp DisposedOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.DisposedPropertyName );
                    CswNbtMetaDataObjectClassProp MissingOcp = ContainerOc.getObjectClassProp( CswNbtObjClassContainer.MissingPropertyName );

                    CswNbtViewRelationship ContainerRel = _QuantityView.AddViewRelationship( LocationRel, NbtViewPropOwnerType.Second, LocationOcp, false );
                    _QuantityView.AddViewPropertyAndFilter( ContainerRel, MaterialOcp, _InventoryLevel.Material.RelatedNodeId.PrimaryKey.ToString(), CswNbtSubField.SubFieldName.NodeID, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.Equals );
                    _QuantityView.AddViewPropertyAndFilter( ContainerRel, DisposedOcp, Tristate.False.ToString(), CswNbtSubField.SubFieldName.Checked, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
                    _QuantityView.AddViewPropertyAndFilter( ContainerRel, MissingOcp, Tristate.False.ToString(), CswNbtSubField.SubFieldName.Checked, FilterMode: CswNbtPropFilterSql.PropertyFilterMode.NotEquals );
                }
                return _QuantityView;
            }
        }



        public double getCurrentInventoryLevel()
        {
            double Ret = 0;
            
            return Ret;
        }

        #endregion Inventory
    } // public class CswNbtSdInventoryLevelMgr

} // namespace ChemSW.Nbt.WebServices
