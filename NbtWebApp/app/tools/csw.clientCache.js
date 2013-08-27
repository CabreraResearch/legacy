/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    var cacheDbMgr = null;
    var thisCustomerId = '';
    var thisUserName = '';

    /**
     * All paramaters are required
    */
    var validate = function (customerId, userName, webServiceName) {
        thisCustomerId = customerId || (thisCustomerId = Csw.clientSession.currentAccessId());
        thisUserName = userName || (thisUserName = Csw.clientSession.currentUserName());

        if (!thisCustomerId) {
            throw new Error('Customer ID is required.');
        }
        if (!thisUserName) {
            throw new Error('User Name is required.');
        }
        if (!webServiceName) {
            throw new Error('Web Service Name is required.');
        }
    };

    /**
        * Make a cached call for insert
    */
    var makeCachedCall = function (webServiceName, data) {
        return { message: { dateTime: new Date(), cache: { customerId: thisCustomerId, userName: thisUserName, webServiceName: webServiceName }, data: data } };
    };

    var getCachedWebServiceCall = function (webServiceName, customerId, userName) {
        var deferred = Q.defer();
        var ret;
        if (null == cacheDbMgr) {
            deferred.resolve(Csw.object());
            ret = deferred.promise;
        } else {
            validate(customerId, userName, webServiceName);
            var promise = cacheDbMgr.select.from('CachedData', 'uniqueCalls', [webServiceName, thisUserName, thisCustomerId]);
            ret = promise.then(function (data) {
                //This is a bit of a dance, but promises are promises.
                //We want the return promise to massage the data into the right object, 
                //so what is one more promise between friends?
                Csw.debug.log('Get: ' + webServiceName);
                Csw.debug.log(data);

                if (data && data.length > 0) {
                    return data[0].data;
                }
            });
        }
        return ret;
    };

    Csw.register('getCachedWebServiceCall', getCachedWebServiceCall);

    var setCachedWebServiceCall = function (webServiceName, data, customerId, userName) {
        var deferred = Q.defer();

        Csw.debug.log('Set: ' + webServiceName);
        Csw.debug.log(data);

        var ret;
        if (null == cacheDbMgr) {
            deferred.resolve(Csw.object());
            ret = deferred.promise;
        } else {
            validate(customerId, userName, webServiceName);
            ret = cacheDbMgr.update('CachedData', 'uniqueCalls', [webServiceName, thisUserName, thisCustomerId], { data: data });
            ret.then(function (updatedRows) {
                if (!updatedRows || updatedRows.length === 0) {
                    var cachedCall = makeCachedCall(webServiceName, data);
                    return cacheDbMgr.insert('CachedData', cachedCall);
                }
            });
        }
        return ret;
    };

    Csw.register('setCachedWebServiceCall', setCachedWebServiceCall);

    //Wait until Main is loaded before initing
    Csw.main.onReady.then(function () {

        thisCustomerId = Csw.clientSession.currentAccessId() || 'offline';
        thisUserName = Csw.clientSession.currentUserName() || 'offline';
        
        if (window.Modernizr.indexeddb) {
            //Until we need to manage versions, there is only 1. Versioning either happens on connection, or it doesn't.

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
    });
}());
