<html>
  <head>
    <title>Simple Jmol_10_pre12 example</title>
    <script src="Jmol.js"></script>
	  <script>jmolSetCodebase("");</script>
    <script language="JavaScript">
		
		function init(){
			var astr;
			astr=document.TheForm.amol.value;
		  jmolLoadInline(astr);
		  jmolSetAppletColor("white");
		
		}
		
		</script>
  </head>
  <body onload="init()">
    <h2>Simple Jmol_10_pre12 example</h2>
		<form name="TheForm"><input type="hidden" id="amol" name="amol" value="aspirina.mol     1  2  2        7  9  2 11 13  2


 21 21  0  0  0
   -2.2240   -1.4442   -0.4577 C   0  0  0  0  0
   -2.1657   -0.0545   -0.5349 C   0  0  0  0  0
   -0.9916    0.6085   -0.1694 C   0  0  0  0  0
    0.1471   -0.0738    0.2764 C   0  0  0  0  0
    0.0751   -1.4832    0.3390 C   0  0  0  0  0
   -1.1052   -2.1532   -0.0188 C   0  0  0  0  0
    1.2412   -2.2934    0.7925 C   0  0  0  0  0
    2.4223   -1.7619    1.1727 O   0  0  0  0  0
    1.1650   -3.5162    0.8364 O   0  0  0  0  0
    1.2795    0.6233    0.5954 O   0  0  0  0  0
    1.1005    1.7577    1.3258 C   0  0  0  0  0
    2.4429    2.3635    1.6825 C   0  0  0  0  0
    0.0255    2.2041    1.6578 O   0  0  0  0  0
   -3.1430   -1.9775   -0.7500 H   0  0  0  0  0
   -3.0382    0.5167   -0.8915 H   0  0  0  0  0
   -0.9608    1.7083   -0.2479 H   0  0  0  0  0
   -1.1740   -3.2520    0.0315 H   0  0  0  0  0
    2.9869   -2.5132    1.4166 H   0  0  0  0  0
    2.3142    3.3967    2.0773 H   0  0  0  0  0
    3.1051    2.4141    0.7884 H   0  0  0  0  0
    2.9391    1.7459    2.4657 H   0  0  0  0  0
  1  2  1  0  0  0
  1  6  2  0  0  0
  1 14  1  0  0  0
  2  3  2  0  0  0
  2 15  1  0  0  0
  3  4  1  0  0  0
  3 16  1  0  0  0
  4  5  2  0  0  0
  4 10  1  0  0  0
  5  6  1  0  0  0
  5  7  1  0  0  0
  6 17  1  0  0  0
  7  8  1  0  0  0
  7  9  2  0  0  0
  8 18  1  0  0  0
 10 11  1  1  0  0
 11 12  1  0  0  0
 11 13  2  0  0  0
 12 19  1  0  0  0
 12 20  1  6  0  0
 12 21  1  1  0  0" /></form>
 
<script>
		jmolSetAppletColor("white");
		jmolApplet(200, "");
    jmolCheckbox("spin on", "spin off", "spin");
</script> 

  </body>
</html>

