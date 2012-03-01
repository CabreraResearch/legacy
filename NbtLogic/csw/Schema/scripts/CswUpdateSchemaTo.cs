
namespace ChemSW.Nbt.Schema
{
    public abstract class CswUpdateSchemaTo
    {
        protected CswNbtSchemaModTrnsctn _CswNbtSchemaModTrnsctn = null;
        public CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn
        {
            set { _CswNbtSchemaModTrnsctn = value; }
        }

        //        public abstract CswSchemaVersion SchemaVersion { get; }
        //public abstract string Description { set; get; }

        private string _Description = string.Empty;
        public virtual string Description { set { _Description = value; } get { return ( _Description ); } }

        public abstract void update();
    }
}
