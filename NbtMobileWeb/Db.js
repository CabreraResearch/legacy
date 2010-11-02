

var DbId = {
    DBShortName: 'main.html',
    DBVersion: '1.0',
    DBDisplayName: 'main.html',
    DBMaxSize: 65536
};


function _DoSql(sql, params, onSuccess) {

    if (window.openDatabase) {

        db = openDatabase(DbId.DBShortName, DbId.DBVersion, DbId.DBDisplayName, DbId.DBMaxSize);
        db.transaction(
                function (transaction) {
                    transaction.executeSql(sql, params, onSuccess, _errorHandler);
                }
            );
    } else 
    {
        console.log("database is not opened");
    }
} //_DoSql

function _errorHandler(transaction, error) {
    alert('Database Error: ' + error.message + ' (Code ' + error.code + ')');
    return true;
} //_errorHandler() 



function _initDB(doreset) {

        if (doreset) {
            _DoSql('DROP TABLE IF EXISTS sublevels; ');
            _DoSql('DROP TABLE IF EXISTS changes; ');
        }

        _createDB();

} //_initDb()



function _createDB() {
    _DoSql('CREATE TABLE IF NOT EXISTS sublevels ' +
            '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
            '   rootid TEXT NOT NULL, ' +
            '   rootname TEXT NOT NULL, ' +
            '   rootxml TEXT, ' +
            '   sublevelxml TEXT );'
            );

    _DoSql('CREATE TABLE IF NOT EXISTS changes ' +
            '  (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, ' +
            '   propid TEXT NOT NULL, ' +
            '   newvalue TEXT, ' +
            '   applied CHAR ); '
            );
}//_createDB()



