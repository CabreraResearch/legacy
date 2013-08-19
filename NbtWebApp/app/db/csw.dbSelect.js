/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    Csw.db.register('select', Csw.makeNameSpace());

    var onError = function (eventObj) {
        Csw.debug.error(eventObj.target.error);
        return new Error(eventObj.target.error);
    };

    /*
    * Private implementation method to select all records from a table
    * @param dbManager {Csw.db.Manager} A DB Manager instance
    * @param tableName {String} The name of the table to select from
   */
    var selectAllImpl = function (dbManager, tableName, ret) {
        var deferred = Q.defer();
        var doSelect = function () {
            try {
                var transaction = dbManager.getDb().transaction([tableName]);

                var objectStore = transaction.objectStore(tableName);

                ret = ret || [];
                var selRequest = objectStore.openCursor();
                selRequest.onsuccess = function(event) {
                    var cursor = event.target.result;
                    if (cursor) {
                        ret.push(cursor.value);
                        cursor.continue();
                    } else {
                        deferred.resolve(ret);
                    }
                };

                selRequest.onerror = function(eventObj) {
                    deferred.reject(onError(eventObj));
                };

            }
            catch (e) {
                console.log(e, e.stack);
                deferred.reject(new Error('Could not select records', e));
            }
            
            return deferred.promise;
        };
        
        dbManager.promises.connect.then(doSelect);
        return deferred.promise;
    };

    /*
     * Public implementation method to select all records from a table
    * @param dbManager {Csw.db.Manager} A DB Manager instance
    * @param tableName {String} The name of the table to select from
    */
    var selectAll = function (dbWrapper, tableName) {
        var ret = [];
        var promise = selectAllImpl(dbWrapper, tableName, ret);
        promise.return = ret;
        return promise;
    };

    Csw.db.select.register('all', selectAll);
   

    /*
    * Private implementation method to select all records from a table
    * @param dbManager {Csw.db.Manager} A DB Manager instance
    * @param tableName {String} The name of the table to select from
    * @param indexName {String} The name of the index to select from
    * @param indexVal {String} The "where" clause: where indexName = indexVal
   */
    var selectFromImpl = function (dbManager, tableName, indexName, indexVal, ret) {
        var deferred = Q.defer();
        var doSelect = function () {
            try {
                var transaction = dbManager.getDb().transaction([tableName]);

                var objectStore = transaction.objectStore(tableName);
                var index = objectStore.index(indexName);

                ret = ret || [];
                var keyRange;
                if (indexVal) {
                    keyRange = IDBKeyRange.only(indexVal);
                }

                var selRequest = index.openCursor(keyRange);
                selRequest.onsuccess = function(event) {
                    var cursor = event.target.result;
                    if (cursor) {
                        ret.push(cursor.value);
                        cursor.continue();
                    } else {
                        deferred.resolve(ret);
                    }
                };

                selRequest.onerror = function(eventObj) {
                    deferred.reject(onError(eventObj));
                };
            } catch(e) {
                console.log(e, e.stack);
                deferred.reject(new Error('Could not select records', e));
            }
            return deferred.promise;
        };
        
        dbManager.promises.connect.then(doSelect);
        return deferred.promise;
    };

    /*
     * Public implementation method to select all records from a table
    * @param dbManager {Csw.db.Manager} A DB Manager instance
    * @param tableName {String} The name of the table to select from
    * @param indexName {String} The name of the index to select from
    * @param indexVal {String} The "where" clause: where indexName = indexVal
    */
    Csw.db.select.register('from', function selectFrom(dbWrapper, tableName, indexName, indexVal) {
        var ret = [];
        var promise = selectFromImpl(dbWrapper, tableName, indexName, indexVal, ret);
        promise.return = ret;
        return promise;
    });


} ());
