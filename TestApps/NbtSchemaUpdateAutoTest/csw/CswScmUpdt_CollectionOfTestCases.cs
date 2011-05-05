using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Text;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.MetaData;
using ChemSW.Exceptions;
//using ChemSW.RscAdo;
//using ChemSW.TblDn;
using ChemSW.Nbt.Schema;

namespace ChemSW.Nbt.Schema
{

    public class CswScmUpdt_CollectionOfTestCases
    {
        private List<CswScmUpdt_TstCse> _ItemList = new List<CswScmUpdt_TstCse>();
        CswNbtSchemaModTrnsctn CswNbtSchemaModTrnsctn = null;
        public CswScmUpdt_CollectionOfTestCases()
        {
//            _ItemList.Add( new CswScmUpdt_TstCse_Column_AddMultiple() );
//            _ItemList.Add( new CswTstUpdtSchema_002_01() );
            _ItemList.Add( new CswScmUpdt_TstCse_Column_RollbackDrop() );
            _ItemList.Add( new CswScmUpdt_TstCse_Constraint_AddMultiple() );
            _ItemList.Add( new CswScmUpdt_TstCse_Constraint_AddOnPendingDml() );
            _ItemList.Add( new CswScmUpdt_TstCse_Constraint_AddSingle() );
            _ItemList.Add( new CswScmUpdt_TstCse_Constraint_RollbackAdd() );
            _ItemList.Add( new CswScmUpdt_TstCse_Constraint_RollbackDrop() );
            _ItemList.Add( new CswScmUpdt_TstCse_NodetypeProp_RollbackAdd() );
            _ItemList.Add( new CswScmUpdt_TstCse_NodetypeProp_RollbackDelete() );
            _ItemList.Add( new CswScmUpdt_TstCse_Nodetype_RollbackAdd() );
            _ItemList.Add( new CswScmUpdt_TstCse_Nodetype_RollbackDelete() );
            _ItemList.Add( new CswScmUpdt_TstCse_Table_Add() );
            _ItemList.Add( new CswScmUpdt_TstCse_Table_Drop() );
            _ItemList.Add( new CswScmUpdt_TstCse_Table_RollbackAdd() );
            _ItemList.Add( new CswScmUpdt_TstCse_Table_RollbackDrop() );
            _ItemList.Add( new CswScmUpdt_TstCse_UniqueSequence_RollbackCreate() );
            _ItemList.Add( new CswScmUpdt_TstCse_UniqueSequence_RollbackDelete() );
            _ItemList.Add( new CswScmUpdt_TstCse_View_RollbackUpdate() );
            _ItemList.Add( new CswScmUpdt_TstCse_NodeType_PreserveUpdateAfterCommit() );
            _ItemList.Add( new CswScmUpdt_TstCse_Node_KeepPropCollectionUpToDate() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataDictionary_ReadTableColId() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataDictionary_ReadTableColIdAfterDdlOp() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackInsert() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataTable_Transaction_NukeRolledBackUpdate() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataTable_Transaction_NukeStaleCoumnlValue() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataTable_UpdateFromSelectedColumns() );
            _ItemList.Add( new CswScmUpdt_TstCse_DataTable_PreserveBlobColumns() );

            //bz # 8604 is on hold indenfintely. No need to enable this test case. 
            //_ItemList.Add( new CswScmUpdt_TstCse_DataDictionary_AddTwoColumns( ) );


        }//ctor

        private List<string> _Names = null;
        public List<string> Names
        {
            get
            {
                if ( null == _Names )
                {
                    _Names = new List<string>();

                    foreach ( CswScmUpdt_TstCse CurrentTest in _ItemList )
                    {
                        _Names.Add( CurrentTest.Name );
                    }
                }

                return ( _Names );

            }
        }

        public Int32 getCount( AppVenue AppVenue )
        {
            Int32 ReturnVal = 0;

            foreach ( CswScmUpdt_TstCse CurrentTestCase in _ItemList )
            {
                if ( AppVenue == CurrentTestCase.AppVenue )
                {
                    ReturnVal++;
                }
            }

            return ( ReturnVal );
        }


        public IEnumerator<CswScmUpdt_TstCse> GetEnumerator()
        {
            return ( new CswSchemaUpdaterTestCaseCollectionEnumerator( _ItemList ) );
        }

    }//CswSchemaUpdaterTestCaseCollection

    public class CswSchemaUpdaterTestCaseCollectionEnumerator : IEnumerator<CswScmUpdt_TstCse>
    {
        Int32 _Position = -1;

        List<CswScmUpdt_TstCse> _ItemList = null;
        public CswSchemaUpdaterTestCaseCollectionEnumerator( List<CswScmUpdt_TstCse> ItemList )
        {
            _ItemList = ItemList;
        }//ctor


        #region IEnumerator<CswSchemaUpdaterTestCase> Members

        CswScmUpdt_TstCse IEnumerator<CswScmUpdt_TstCse>.Current
        {
            get
            {
                try
                {
                    return _ItemList[ _Position ];
                }
                catch ( IndexOutOfRangeException )
                {
                    throw new InvalidOperationException();
                }
            }//get
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get
            {
                try
                {
                    return _ItemList[ _Position ];
                }
                catch ( IndexOutOfRangeException )
                {
                    throw new InvalidOperationException();
                }
            }//get
        }

        bool System.Collections.IEnumerator.MoveNext()
        {
            _Position++;
            return ( _Position < _ItemList.Count );
        }

        void System.Collections.IEnumerator.Reset()
        {
            _Position = -1;
        }
        #endregion

    }//

}//ChemSW.Nbt.Schema
