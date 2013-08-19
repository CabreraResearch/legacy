/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    /*
     * Curry Left method
    */
    Csw.register('curryLeft', function curryLeft(func) {
        var slice = Array.prototype.slice;
        var args = slice.call(arguments, 1);
        return function () {
            return func.apply(this, args.concat(slice.call(arguments, 0)));

        }
    });

    /*
     * Fold Left method
    */
    Csw.register('foldLeft', function foldLeft(func, newArray, oldArray) {
        var accumulation = newArray;
        Csw.each(oldArray, function (val) {
            accumulation = func(accumulation, val);
        });
        return accumulation;
    });

    /*
     * Map method
    */
    Csw.register('map', function map(func, array) {
        var onIteration = function (accumulation, val) {
            return accumulation.concat(func(val));
        };
        return foldLeft(onIteration, [], array)
    });

    /*
     * Filter method
    */
    Csw.register('filter', function filter(func, array) {
        var onIteration = function (accumulation, val) {
            if (func(val)) {
                return accumulation.concat(val);
            } else {
                return accumulation;
            }
        };
        return foldLeft(onIteration, [], array)
    });

    /*
     * Inserts a parameter into the position of the first argument, shifting all other arguments to "the right" by one position
    */
    Csw.register('shiftRight', function shiftRight(shiftFunc, firstParam, originalArguments, context) {
        context = context || this;
        var args = Array.prototype.slice.call(originalArguments, 0);
        args.unshift(firstParam);
        return shiftFunc.apply(context, args);
    });

    /*
     * Inserts a parameter into the position of the first argument, shifting all other arguments to "the right" by one position
    */
    Csw.register('apply', function apply(applyFunc, originalArguments, context) {
        context = context || this;
        var args = Array.prototype.slice.call(originalArguments, 0);
        return applyFunc.apply(context, args);
    });

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
                        n$.each(schemaScripts, function (script) {
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
        
        var ret = n$.object();
        ret.add('connect', connect);
        ret.add('disconnect', disconnect);
        ret.add('getDb', function () { return db; });

        ret.add('schemaScripts', schemaScripts);
        ret.add('tables', n$.object());

        ret.add('promises', n$.object());
        ret.promises.add('connect', connectPromise);

        ret.add('ddl', {
            createTable: function (tableName, tablePkColumnName, autoIncrement) {
                return n$.shiftRight(n$.db.table.create, ret, arguments, this);
                /*
                var args = Array.prototype.slice.call(arguments, 0);
                args.unshift(ret);
                return createTable.apply(this, args);*/
            },
            dropTable: function (tableName) {
                return n$.shiftRight(n$.db.index.drop, ret, arguments, this);
                /*var args = Array.prototype.slice.call(arguments, 0);
                args.unshift(ret);
                return createTable.apply(this, args);*/
            },
            createIndex: function (tableName, columnName, indexName, isUnique) {
                return n$.shiftRight(n$.db.index.create, ret, arguments, this);
                /*var args = Array.prototype.slice.call(arguments, 0);
                args.unshift(ret);
                return createIndex.apply(this, args);*/
            }
        });
        ret.add('insert', function () {
            return n$.shiftRight(n$.db.insert, ret, arguments, this);
            /*var args = Array.prototype.slice.call(arguments, 0);
            args.unshift(ret);
            return insert.apply(this, args);*/
        });
        return ret;
    };

    Csw.register('dbManager', dbManager);

   
} ());
