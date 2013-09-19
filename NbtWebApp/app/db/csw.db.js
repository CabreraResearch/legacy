/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';
    
    /*
     * Instance a DB Manager class which abstracts the mechanics for connecting to and selecting from an IndexedDb database
    */
    var dbManager = function (name, version) {
        var ret = Csw.object();
        ret.add('promises', Csw.object());
        
        var isNewConnectionRequired = false;
        var schemaScripts = [];

        /*
         * Initiate a promise to connect to a database. When that connection is established, the promise will be resolved.
        */
        var connect = function (dbName, dbVersion, dbOnUpgrade) {
            isNewConnectionRequired = (!ret.promises.connect || dbName !== name || dbVersion !== version);
            if (isNewConnectionRequired) {
                var deferred = Q.defer();

                ret.promises.connect = deferred.promise;

                version = dbVersion || 1;
                name = dbName;
                dbOnUpgrade = dbOnUpgrade || function () { };

                var request = window.indexedDB.open(name, version);

                request.onblocked = function (event) {
                    ret.IDB.close();
                    alert("A new version of this page is ready. Please reload!");
                };

                request.onerror = function (event) {
                    deferred.reject(new Error("Database error: " + event.target.errorCode));
                    if (ret.IDB) {
                        ret.IDB.close();
                    }
                };
                request.onsuccess = function (event) {
                    ret.IDB = ret.IDB || request.result;
                    deferred.resolve(ret.IDB);
                };
                request.onupgradeneeded = function (event) {
                    ret.IDB = ret.IDB || request.result;
                    if (schemaScripts.length > 0) {
                        Csw.iterate(schemaScripts, function (script) {
                            //debugger;
                            script(ret.IDB);
                        });
                    }
                    dbOnUpgrade(ret.IDB);
                };
            }
            return ret.promises.connect;
        };

        /*
         * Disconnect from a database
        */
        var disconnect = function () {
            if (ret.promises.connect.isFulfilled()) {
                ret.IDB.close();
            }
            else if (ret.IDB) {
                ret.promises.connect.done(ret.IDB.close);
            }
        };
        
        //Collect the methods into an API:
        ret.add('connect', connect);
        ret.add('disconnect', disconnect);
        ret.add('getDb', function () { return ret.IDB; });

        ret.add('schemaScripts', schemaScripts);
        ret.add('tables', Csw.object());
        
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

        ret.add('update', function () {
            return Csw.fun.shiftRight(Csw.db.update, ret, arguments, this);
        });

        var select = Csw.object();
        ret.add('select', select);
        
        select.add('all', function () {
            return Csw.fun.shiftRight(Csw.db.select.all, ret, arguments, this);
        });
        
        select.add('from', function () {
            return Csw.fun.shiftRight(Csw.db.select.from, ret, arguments, this);
        });

        //Connect to the DB automatically
        ret.connect(name, version);

        return ret;
    };
    
    Csw.db.register('dbManager', dbManager);

   
} ());
