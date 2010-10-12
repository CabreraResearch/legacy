//using System;
//using System.Collections.Generic;
//using System.Collections;
//using System.Text;
//using System.Data;
//using ChemSW.Nbt.PropTypes;
//using ChemSW.Exceptions;
//using ChemSW.Nbt.MetaData;
//

//namespace ChemSW.Nbt.ObjClasses
//{
//    public class CswNbtObjClassTestGroup : CswNbtObjClass
//    {
//        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

//        public CswNbtObjClassTestGroup(CswNbtResources CswNbtResources, CswNbtNode Node)
//            : base( CswNbtResources, Node )
//        {
//            _CswNbtObjClassDefault = new CswNbtObjClassDefault(_CswNbtResources, Node);
//        }//ctor()

//        public override CswNbtMetaDataObjectClass ObjectClass
//        {
//            get { return _CswNbtResources.MetaData.getObjectClass(CswNbtMetaDataObjectClass.NbtObjectClass.TestGroupClass); }
//        }

//        #region Inherited Events
//        public override void beforeWriteNode()
//        {
//            _CswNbtObjClassDefault.beforeWriteNode();
//        }//beforeWriteNode()

//        public override void afterWriteNode()
//        {
//            _CswNbtObjClassDefault.afterWriteNode();
//        }//afterWriteNode()

//        public override void beforeDeleteNode()
//        {
//            _CswNbtObjClassDefault.beforeDeleteNode();

//        }//beforeDeleteNode()

//        public override void afterDeleteNode()
//        {
//            _CswNbtObjClassDefault.afterDeleteNode();
//        }//afterDeleteNode()
        
//        public override void afterPopulateProps()
//        {
//            _CswNbtObjClassDefault.afterPopulateProps();
//        }//afterPopulateProps()        

//        #endregion

//        #region Object class specific properties

//        //Sic. (no props)

//        #endregion



//    }//CswNbtObjClassTestGroup

//}//namespace ChemSW.Nbt.ObjClasses
