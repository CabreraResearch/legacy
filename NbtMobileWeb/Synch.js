

function _waitForData() {

    setTimeout(_handleDataCheckTimer, 2000);

} //_waitForData() 

function _handleDataCheckTimer() {


    _DoSql("select * from changes where applied='0'", null, _processChanges );
} //_handleDataCheckTimer()


function _processChanges(transaction, result) {

    console.log( "totalrows: " + result.rows.length );
    for (var rowidx = 0; rowidx < result.rows.length; rowidx++) {

        console.log("iteration " + rowidx + ": change value: " + result.rows.item(rowidx)[ "newvalue" ]);

    } //iterate rows

    _waitForData(); 

} //_processChanges() 


