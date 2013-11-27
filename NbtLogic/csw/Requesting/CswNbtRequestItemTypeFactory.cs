using ChemSW.Exceptions;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Requesting
{
    /// <summary>
    /// Factory for RequestItemType definitions
    /// </summary>
    public class CswNbtRequestItemTypeFactory
    {
        public static CswNbtRequestItemType makeRequestItemType( CswNbtResources CswNbtResources, CswNbtObjClassRequestItem RequestItem )
        {
            string RequestItemType = RequestItem.Type.Value;
            CswNbtRequestItemType TypeDef;
            switch( RequestItemType )
            {
                case CswNbtObjClassRequestItem.Types.EnterprisePart:
                    TypeDef = new CswNbtRequestItemTypeEnterprisePart( CswNbtResources, RequestItem );
                    break;
                case CswNbtObjClassRequestItem.Types.MaterialCreate:
                    TypeDef = new CswNbtRequestItemTypeMaterialCreate( CswNbtResources, RequestItem );
                    break;
                case CswNbtObjClassRequestItem.Types.MaterialBulk:
                    TypeDef = new CswNbtRequestItemTypeMaterialByBulk( CswNbtResources, RequestItem );
                    break;
                case CswNbtObjClassRequestItem.Types.MaterialSize:
                    TypeDef = new CswNbtRequestItemTypeMaterialBySize( CswNbtResources, RequestItem );
                    break;
                case CswNbtObjClassRequestItem.Types.ContainerDispense:
                    TypeDef = new CswNbtRequestItemTypeContainerDispense( CswNbtResources, RequestItem );
                    break;
                case CswNbtObjClassRequestItem.Types.ContainerDispose:
                    TypeDef = new CswNbtRequestItemTypeContainerDispose( CswNbtResources, RequestItem );
                    break;
                case CswNbtObjClassRequestItem.Types.ContainerMove:
                    TypeDef = new CswNbtRequestItemTypeContainerMove( CswNbtResources, RequestItem );
                    break;
                default:
                throw new CswDniException( CswEnumErrorType.Error,
                                           "Unhandled item type: " + RequestItemType,
                                           "CswNbtRequestItemTypeFactory did not recognize item type: " + RequestItemType );
            }
            return TypeDef;
        }
    }
}
