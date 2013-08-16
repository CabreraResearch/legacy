/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    /*
     * Instance a DB Manager class which abstracts the mechanics for connecting to and selecting from an IndexedDb database
    */
    var dbManager = function () {
        var name, version, db, connectPromise, upgradeIsRequired = false;
        var schemaScripts = [];

        /*
         * Initiate a promise to connect to a database. When that connection is established, the promise will be resolved.
        */
        var connect = function (dbName, dbVersion, dbOnUpgrade) {
            upgradeIsRequired = (!connectPromise || dbName !== name || dbVersion !== version);
            if (upgradeIsRequired) {
                var deferred = Q.defer();

                connectPromise = deferred.promise;

                version = dbVersion || 1;
                name = dbName;
                dbOnUpgrade = dbOnUpgrade || function () { };

                var request = window.indexedDB.open(name, version);

                request.onblocked = function (event) {
                    db.close();
                    alert("A new version of this page is ready. Please reload!");
                };

                request.onerror = function (event) {
                    deferred.reject(new Error("Database error: " + event.target.errorCode));
                    if (db) {
                        db.close();
                    }
                };
                request.onsuccess = function (event) {
                    db = request.result;
                    deferred.resolve(db);
                };
                request.onupgradeneeded = function (event) {
                    if (schemaScripts.length > 0) {
                        Csw.iterate(schemaScripts, function (script) {
                            //debugger;
                            script(db);
                        });
                    }
                    dbOnUpgrade(db);
                };
            }
            return connectPromise;
        };

        /*
         * Disconnect from a database
        */
        var disconnect = function () {
            if (connectPromise.isFulfilled()) {
                db.close();
            }
            else if (db) {
                connectPromise.done(db.close);
            }
        };
        
        var ret = Csw.object();
        ret.add('connect', connect);
        ret.add('disconnect', disconnect);
        ret.add('getDb', function () { return db; });

        ret.add('schemaScripts', schemaScripts);
        ret.add('tables', Csw.object());

        ret.add('promises', Csw.object());
        ret.promises.add('connect', connectPromise);

        ret.add('ddl', {
            createTable: function (tableName, tablePkColumnName, autoIncrement) {
                return Csw.fun.shiftRight(Csw.db.table.create, ret, arguments, this);
            },
            dropTable: function (tableName) {
                return Csw.fun.shiftRight(Csw.db.index.drop, ret, arguments, this);
            },
            createIndex: function (tableName, columnName, indexName, isUnique) {
                return Csw.fun.shiftRight(Csw.db.index.create, ret, arguments, this);
            }
        });
        ret.add('insert', function () {
            return Csw.fun.shiftRight(Csw.db.insert, ret, arguments, this);
        });
        return ret;
    };

    Csw.register('db', Csw.makeNameSpace());

    Csw.db.register('dbManager', dbManager);

   
} ());
