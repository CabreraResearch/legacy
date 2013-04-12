using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.Test.ServiceDrivers;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.Security
{
    [TestFixture]
    public class CswNbtSdTabsAndProps_UnitTest : IDisposable
    {
        
        [SetUp]
        public void Init()
        {
            
        }

        [TearDown]
        public void Destroy()
        {
            
        }

        public void Dispose()
        {
            Destroy();
        }

        /// <summary>
        /// This test simply proves that when requesting the Int32 val for a new row:
        /// <para>a.) The row will be greater than zero.</para>
        /// <para>b.) The row/column values will not collide with any other.</para>
        /// <para>c.) The intended order of rows and columns is preserved as best as possible.</para>
        /// </summary>
        [Test]
        public void getUniqueRow_Test()
        {
            
            Dictionary<Int32, Collection<Int32>> RowsAndCols = new Dictionary<int, Collection<int>>();
            
            //(1,1)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( Int32.MinValue, Int32.MinValue, RowsAndCols ), 1 );
            //(2,1)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( Int32.MinValue, Int32.MinValue, RowsAndCols ), 2);
            //(3,1)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( Int32.MinValue, Int32.MinValue, RowsAndCols ), 3);
            //(4,1)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( 3, Int32.MinValue, RowsAndCols ), 4);
            //(5,1)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( Int32.MinValue, 1, RowsAndCols ), 5);
            //(1,2)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( Int32.MinValue, 2, RowsAndCols ), 1);
            //(2,2)
            Assert.AreEqual( CswNbtSdTabsAndProps.getUniqueRow( Int32.MinValue, 2, RowsAndCols ), 2);
            
        }


    }
}
