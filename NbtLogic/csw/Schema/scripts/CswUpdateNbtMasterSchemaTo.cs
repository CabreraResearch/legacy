
namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// A schema script that only will be applied to Nbt_Master
    /// </summary>
    public abstract class CswUpdateNbtMasterSchemaTo : CswUpdateSchemaTo
    {

        public sealed override void update()
        {
            if( _CswNbtSchemaModTrnsctn.isMaster() )
            {
                doUpdate();
            }
        }

        /// <summary>
        /// The operation to be applied to Nbt_master
        /// </summary>
        public abstract void doUpdate();

    }
}
