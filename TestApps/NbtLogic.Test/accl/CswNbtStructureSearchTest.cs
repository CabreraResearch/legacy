using System.Collections.Generic;
using System.Linq;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using NUnit.Framework;

namespace ChemSW.Nbt.Test.ObjClasses
{
    [TestFixture]
    public class CswNbtStructureSearchTest
    {
        #region Mols

        //A mol file that doesn't have the three lines before the info line
        private const string ImproperlyFormattedMol = @" localdb 08011213052D 1   1.00000     0.00000     0

 11  3  0     0  0            999 V2000
    3.9667    2.5208    0.0000 Si  0  0  0  0  0  0           0  0  0
    4.6667    1.3083    0.0000 O   0  5  0  0  0  0           0  0  0
    2.5667    2.5208    0.0000 O   0  5  0  0  0  0           0  0  0
    4.6667    3.6875    0.0000 O   0  0  0  0  0  0           0  0  0
    3.5458    4.7125    0.0000 Na  0  3  0  0  0  0           0  0  0
    1.8667    3.6875    0.0000 Na  0  3  0  0  0  0           0  0  0
    0.0000    4.2458    0.0000 O   0  0  0  0  0  0           0  0  0
    6.0667    0.2833    0.0000 O   0  0  0  0  0  0           0  0  0
    1.6792    0.1417    0.0000 O   0  0  0  0  0  0           0  0  0
    3.4500    6.8125    0.0000 O   0  0  0  0  0  0           0  0  0
    7.3708    4.4792    0.0000 O   0  0  0  0  0  0           0  0  0
  2  1  1  0     0  0
  3  1  1  0     0  0
  4  1  2  0     0  0
M  CHG  4   2  -1   3  -1   5   1   6   1
M  END";

        private const string MolWithBenzeneRing1 = @"$$$$

  -ISIS-  09030813062D

 20 21  0  0  0  0  0  0  0  0999 V2000
    5.1750   -2.7500    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    4.4542   -2.3375    0.0000 C   0  0  3  0  0  0  0  0  0  0  0  0
    5.8917   -2.3292    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.7417   -2.7500    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    5.1750   -3.5792    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    6.6042   -2.7417    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    5.8917   -1.5042    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.0292   -2.3375    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.7417   -3.5792    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    4.4542   -1.5042    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    2.3167   -3.5792    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    7.3250   -1.5042    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    2.3167   -2.7500    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    7.3250   -2.3292    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.6042   -1.0917    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.0292   -3.9917    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    1.5917   -4.0000    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    8.0417   -1.0875    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    8.7667   -1.5000    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    0.8792   -3.5875    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
  2  1  1  0  0  0  0
  3  1  1  0  0  0  0
  4  2  1  0  0  0  0
  5  1  2  0  0  0  0
  6  3  2  0  0  0  0
  7  3  1  0  0  0  0
  8  4  1  0  0  0  0
  9  4  2  0  0  0  0
 10  2  1  0  0  0  0
 11 16  2  0  0  0  0
 12 15  1  0  0  0  0
 13  8  2  0  0  0  0
 14  6  1  0  0  0  0
 15  7  2  0  0  0  0
 16  9  1  0  0  0  0
 17 11  1  0  0  0  0
 18 12  1  0  0  0  0
 19 18  1  0  0  0  0
 20 17  1  0  0  0  0
 14 12  2  0  0  0  0
 13 11  1  0  0  0  0
M  END
";

        private const string MolWithBenzeneRing2 = @"$$$$

  -ISIS-  09030813062D

 17 20  0  0  0  0  0  0  0  0999 V2000
    5.2917   -5.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.0292   -5.4542    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.0292   -4.7167    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    4.5542   -5.4542    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    4.5542   -4.7167    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.7667   -5.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    5.2917   -6.5667    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.7667   -6.5667    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.0292   -6.9292    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    7.5042   -6.9292    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0
    6.7667   -4.3500    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.8167   -4.3500    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.8167   -5.8250    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    7.5042   -5.4542    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    7.5042   -4.7167    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.0792   -5.4542    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.0792   -4.7167    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
  2  1  1  0  0  0  0
  3  2  1  0  0  0  0
  4  1  1  0  0  0  0
  5  4  2  0  0  0  0
  6  2  2  0  0  0  0
  7  1  2  0  0  0  0
  8  9  2  0  0  0  0
  9  7  1  0  0  0  0
 10  8  1  0  0  0  0
 11  3  2  0  0  0  0
 12  5  1  0  0  0  0
 13  4  1  0  0  0  0
 14  6  1  0  0  0  0
 15 14  2  0  0  0  0
 16 13  2  0  0  0  0
 17 16  1  0  0  0  0
  5  3  1  0  0  0  0
  6  8  1  0  0  0  0
 12 17  2  0  0  0  0
 11 15  1  0  0  0  0
M  END
";

        private const string MolWithoutBenzeneRing = @"$$$$

  -ISIS-  09030813062D

 14 13  0  0  0  0  0  0  0  0999 V2000
    9.7417   -2.0458    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    9.7417   -1.2167    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
   10.4583   -2.4583    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0
    1.8833   -2.4583    0.0000 N   0  0  0  0  0  0  0  0  0  0  0  0
    9.0292   -2.4583    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    2.5958   -2.0458    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    8.3125   -2.0458    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    3.3125   -2.4583    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    4.7417   -2.4583    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    4.0250   -2.0458    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    7.6000   -2.4583    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.8833   -2.0458    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    6.1708   -2.4583    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    5.4542   -2.0458    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
  2  1  2  0  0  0  0
  3  1  1  0  0  0  0
  4  6  1  0  0  0  0
  5  1  1  0  0  0  0
  6  8  1  0  0  0  0
  7  5  1  0  0  0  0
  8 10  1  0  0  0  0
  9 14  1  0  0  0  0
 10  9  1  0  0  0  0
 11  7  1  0  0  0  0
 12 11  1  0  0  0  0
 13 12  1  0  0  0  0
 14 13  1  0  0  0  0
M  END";

        private const string Benzene = @" benzene
 ACD/Labs0812062058
 
  6  6  0  0  0  0  0  0  0  0  1 V2000
    1.9050   -0.7932    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    1.9050   -2.1232    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    0.7531   -0.1282    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
    0.7531   -2.7882    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
   -0.3987   -0.7932    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
   -0.3987   -2.1232    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0
  2  1  1  0  0  0  0
  3  1  2  0  0  0  0
  4  2  2  0  0  0  0
  5  3  1  0  0  0  0
  6  4  1  0  0  0  0
  6  5  2  0  0  0  0
 M  END
 $$$$";

        #endregion

        #region Setup and Teardown

        private TestData TestData;

        [SetUp]
        public void MyTestInitialize()
        {
            TestData = new TestData();
        }

        [TearDown]
        public void MyTestCleanup()
        {
            TestData.Destroy();
        }

        #endregion

        /// <summary>
        /// Verifies that the molecule manager does not choke on a mol file when generating an image
        /// </summary>
        [Test]
        public void generateImage()
        {
            byte[] img = TestData.CswNbtResources.MoleculeManager.GenerateImage( ImproperlyFormattedMol, false );
            Assert.Greater( img.Length, 0 );
        }

        /// <summary>
        /// Creates three Chemicals with 2 different MOL files, two of which contains BENZENE. Structure Search should return 2/3 chemicals
        /// </summary>
        [Test]
        public void structureSearch()
        {
            CswNbtMetaDataNodeType ChemicalNT = TestData.CswNbtResources.MetaData.getNodeType( "Chemical" );
            if( null == ChemicalNT )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Unexpected Error testing structure searches - not chemical NT exists", "Unexpected Error testing structure searches - not chemical NT exists" );
            }

            CswNbtNode n1Benzene = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, delegate( CswNbtNode NewNode )
                {
                    CswNbtObjClassChemical AsChemical = NewNode;
                    AsChemical.TradeName.Text = MolWithBenzeneRing1;
                    AsChemical.Structure.Mol = MolWithBenzeneRing1;
                    } );

            CswNbtNode n2Benzene = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassChemical AsChemical = NewNode;
                AsChemical.TradeName.Text = MolWithBenzeneRing2;
                AsChemical.Structure.Mol = MolWithBenzeneRing2;
            } );

            CswNbtNode n3NoBenzene = TestData.CswNbtResources.Nodes.makeNodeFromNodeTypeId( ChemicalNT.NodeTypeId, delegate( CswNbtNode NewNode )
            {
                CswNbtObjClassChemical AsChemical = NewNode;
                AsChemical.TradeName.Text = MolWithoutBenzeneRing;
                AsChemical.Structure.Mol = MolWithoutBenzeneRing;
            } );

            Dictionary<int, string> FoundMols = TestData.CswNbtResources.MoleculeManager.RunSearch( Benzene, false );

            bool ContainsNode1 = FoundMols.Keys.Any( key => key == n1Benzene.NodeId.PrimaryKey );
            bool ContainsNode2 = FoundMols.Keys.Any( key => key == n2Benzene.NodeId.PrimaryKey );
            bool ContainsNode3 = FoundMols.Keys.Any( key => key == n3NoBenzene.NodeId.PrimaryKey );

            Assert.IsTrue( ContainsNode1, "Structure search failed to return an expected mol (MolWithBenzeneRing1)" );
            Assert.IsTrue( ContainsNode2, "Structure search failed to return an expected mol (MolWithBenzeneRing2)" );
            Assert.IsFalse( ContainsNode3, "Structure search returned a mol when it shouldn't have (MolWithoutBenzeneRing)" );
        }
        
    }
}
