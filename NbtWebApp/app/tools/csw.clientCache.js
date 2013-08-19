/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    /**
     * All paramaters are required
    */
    var validate = function (customerId, userName, webServiceName) {
        if (!customerId) {
            throw new Error('Customer ID is required.');
        }
        if (!userName) {
            throw new Error('User Name is required.');
        }
        if (!webServiceName) {
            throw new Error('Web Service Name is required.');
        }
    };
    
    /**
     * Make a cached call for insert
    */
    var makeCachedCall = function (customerId, userName, webServiceName, data) {
        validate(customerId, userName, webServiceName);
        return { message: { dateTime: new Date(), cache: { customerId: customerId, userName: userName, webServiceName: webServiceName }, data: data } };
    };

    //Wait until Main is loaded before initing
    Csw.main.onReady.then(function () {

        var cacheDbMgr = null;
        if (window.Modernizr.indexeddb) {
            //Until we need to manage versions, there is only 1. Versioning either happens on creation, or it doesn't.

            cacheDbMgr = Csw.db.dbManager('CswLive', 1);

            //newDbMgr.ddl.dropTable(tableName);
            cacheDbMgr.ddl.createTable('CachedData', 'CachedDataId', true); //true == auto manage primary key
            cacheDbMgr.ddl.createIndex('CachedData', 'customerId', 'cache.customerId');
            cacheDbMgr.ddl.createIndex('CachedData', 'dateTimeId', 'dateTime');
            cacheDbMgr.ddl.createIndex('CachedData', 'userNameId', 'cache.userName');
            cacheDbMgr.ddl.createIndex('CachedData', 'webServiceNameId', 'cache.webServiceName');
            cacheDbMgr.ddl.createIndex('CachedData', 'uniqueCalls', ['cache.webServiceName', 'cache.userName', 'cache.customerId'], true);

            //Insert some demo data
            //cacheDbMgr.insert('CachedData', { message: { dateTime: new Date(), cache: { customerId: 'dev', userName: 'admin', webServiceName: 'getDashbaord' }, data: { a: 1, b: 2, c: 3 } } });
            //cacheDbMgr.insert('CachedData', { message: { dateTime: new Date(), cache: { customerId: 'qa', userName: 'bill', webServiceName: 'getWatermark' }, data: { a: 1, b: 2, c: 3 } } });
        }

        var getCachedWebServiceCall = function(customerId, userName, webServiceName) {
            var deferred = Q.defer();
            var ret;
            if (null == cacheDbMgr) {
                deferred.resolve(Csw.object());
                ret = deferred.promise;
            } else {
                validate(customerId, userName, webServiceName);
                ret = cacheDbMgr.select.from('CachedData', 'uniqueCalls', [webServiceName, userName, customerId]);
            }
            return ret;
        };

        Csw.register('getCachedWebServiceCall', getCachedWebServiceCall);
        
        var setCachedWebServiceCall = function(customerId, userName, webServiceName, data) {
            var deferred = Q.defer();
            var ret;
            if (null == cacheDbMgr) {
                deferred.resolve(Csw.object());
                ret = deferred.promise;
            } else {
                ret = cacheDbMgr.update('CachedData', 'uniqueCalls', [webServiceName, userName, customerId], data);
                ret.then(function(updatedRows) {
                    if (!updatedRows || updatedRows.length === 0) {
                        var cachedCall = makeCachedCall(customerId, userName, webServiceName);
                        return cacheDbMgr.insert('CachedData', cachedCall);
                    }
                });
            }
            return ret;
        };

        Csw.register('setCachedWebServiceCall', setCachedWebServiceCall);
    });
} ());
