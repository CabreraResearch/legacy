/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var onError = function(eventObj) {
        Csw.debug.error(eventObj.target.error);
        return new Error(eventObj.target.error);
    };

    /*
    * Private implementation method to update new records into a table
    * @param dbManager {Csw.db.Manager} A DB Manager instance
    * @param tableName {String} The name of the table to update into
    * @param indexName {String} The name of the index to select from
    * @param indexVal {String} The "where" clause: where indexName = indexVal
    * @param record {Object} An object to update/insert into the db
   */
    var updateImpl = function (dbManager, tableName, indexName, indexVal, ret, record) {
        var deferred = Q.defer();
        var doUpdate = function () {
            try {
                var transaction = dbManager.getDb().transaction([tableName], 'readwrite');

                var objectStore = transaction.objectStore(tableName);
                var index = objectStore.index(indexName);

                ret = ret || [];
                var keyRange = IDBKeyRange.only(indexVal);
                var selRequest = index.openCursor(keyRange);

                selRequest.onsuccess = function (event) {
                    var cursor = event.target.result;
                    if (cursor) {
                        var val = cursor.value;
                        var newRec = Csw.extend(val, record);

                        var updtRequest = cursor.update(newRec);
                        updtRequest.onerror = onError;
                        
                    } else {
                        deferred.resolve(ret);
                    }
                };

                selRequest.onerror = function (e) {
                    deferred.reject(onError(e));
                };

            } catch (e) {
                console.log(e, e.stack);
                deferred.reject(new Error('Could not select records', e));
            }
            return deferred.promise;
        };

        dbManager.promises.connect.then(doUpdate);
        return deferred.promise;
    };

    /*
     * Public implementation method to update (or insert new) record into a table
     * @param dbManager {Csw.db.Manager} A DB Manager instance
     * @param tableName {String} The name of the table to update into
     * @param indexName {String} The name of the index to select from
     * @param indexVal {String} The "where" clause: where indexName = indexVal
     * @param record {Object} An object to update/insert into the db
    */
    Csw.db.register('update', function update(dbWrapper, tableName, indexName, indexVal, record) {
        var ret = [];
        return updateImpl(dbWrapper, tableName, indexName, indexVal, ret, record);
    });
   
} ());
