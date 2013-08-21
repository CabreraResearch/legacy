/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
   
    var onError = function (eventObj) {
        Csw.debug.error(eventObj.target.error);
        return new Error(eventObj.target.error);
    };
    
    /*
    * Private implementation method to insert new records into a table
    * @param dbManager {Csw.db.Manager} A DB Manager instance
    * @param tableName {String} The name of the table to insert into
    * @param records {Array} An Array of records (objects) to insert into the db
   */
    var insertImpl = function (dbManager, tableName, records) {
        var deferred = Q.defer();
        var doInsert = function () {
            try {
                var transaction = dbManager.getDb().transaction([tableName], 'readwrite');

                var objectStore = transaction.objectStore(tableName);
                Csw.iterate(records, function (rec) {
                    objectStore.add(rec);
                });

            }
            catch (e) {
                console.log(e, e.stack);
                deferred.reject(new Error('Could not insert records', e));
            }

            return deferred.resolve(true);
        };

        dbManager.promises.connect.then(doInsert);
        return deferred.promise;
    };

    /*
     * Public implementation method to insert new records into a table
     * @param dbManager {Csw.db.Manager} A DB Manager instance
     * @param tableName {String} The name of the table to insert into
     * @param records {Array} An Array of records (objects) to insert into the db
    */
    var insert = function (dbWrapper, tableName, records) {
        return insertImpl(dbWrapper, tableName, records);
    };

    Csw.db.register('insert', insert);
   
} ());
