/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    //Wait until Main is loaded before initing
    Csw.main.onReady.then(function () {

        cacheDbMgr = Csw.db.dbManager('CswLive', 1);

        //newDbMgr.ddl.dropTable(tableName);
        cacheDbMgr.ddl.createTable('CachedData', 'CachedDataId', true); //true == auto manage primary key
        cacheDbMgr.ddl.createIndex('CachedData', 'customerId', 'cache.customerId');
        cacheDbMgr.ddl.createIndex('CachedData', 'dateTimeId', 'dateTime');
        cacheDbMgr.ddl.createIndex('CachedData', 'userNameId', 'cache.userName');
        cacheDbMgr.ddl.createIndex('CachedData', 'webServiceNameId', 'cache.webServiceName');
        cacheDbMgr.ddl.createIndex('CachedData', 'uniqueCalls', ['cache.webServiceName', 'cache.userName', 'cache.customerId'], true);

        //Insert some data
        cacheDbMgr.insert('CachedData', { message: { dateTime: new Date(), cache: { customerId: 'dev', userName: 'admin', webServiceName: 'getDashbaord' }, data: { a: 1, b: 2, c: 3 } } });
        cacheDbMgr.insert('CachedData', { message: { dateTime: new Date(), cache: { customerId: 'qa', userName: 'bill', webServiceName: 'getWatermark' }, data: { a: 1, b: 2, c: 3 } } });

        var cacheDbMgr = null;
        if (window.Modernizr.indexeddb) {
            //Until we need to manage versions, there is only 1. Versioning either happens on creation, or it doesn't.

        }

        var getCachedWebServiceCall = function(customerId, userName, webServiceName) {
            var deferred = Q.defer();
            var ret;
            if (null == cacheDbMgr) {
                deferred.resolve(Csw.object());
                ret = deferred.promise;
            } else {
                debugger;
                ret = cacheDbMgr.select.all('CachedData');
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
                debugger;
                //need to do either an insert or an update
                if (true) {
                    cacheDbMgr.insert('CachedData', { message: { dateTime: new Date(), cache: { customerId: customerId, userName: userName, webServiceName: webServiceName }, data: data } });
                } else {
                    //do update
                }

                //ret = cacheDbMgr.select.all('CachedData');
            }
            return ret;
        };

        Csw.register('setCachedWebServiceCall', setCachedWebServiceCall);
    });
} ());
