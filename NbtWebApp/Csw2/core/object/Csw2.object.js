/*global Csw2:true*/
(function() {

    Csw2.lift('hasLength', function (obj) {
        'use strict';
        var ret = (Csw2.is.array(obj) || Csw2.is.jQuery(obj));
        return ret;
    });

    Csw2.lift('contains', function (object, index) {
        'use strict';
        var ret = false;
        if (false === Csw2.is.nullOrUndefined(object)) {
            if (Csw2.is.array(object)) {
                ret = object.indexOf(index) !== -1;
            }
            if (false === ret && object.hasOwnProperty(index)) {
                ret = true;
            }
        }
        return ret;
    });

    Csw2.lift('renameProperty', function (obj, oldName, newName) {
        'use strict';
        if (false === Csw2.is.nullOrUndefined(obj) && Csw2.contains(obj, oldName)) {
            obj[newName] = obj[oldName];
            delete obj[oldName];
        }
        return obj;
    });

    Csw2.lift('clone', function (data) {
        'use strict';
        return Csw2.deserialize(Csw2.serialize(data));
    });

    Csw2.lift('serialize', function (data) {
        'use strict';
        var ret = '';
        Csw2.tryExec(function() { ret = JSON.stringify(data); });
        return ret;
    });


    Csw2.lift('deserialize', function (data) {
        'use strict';
        var ret = {};
        Csw2.tryExec(function () { ret = window.$.parseJSON(data); });
        if(Csw2.is.nullOrEmpty(ret)) {
			ret = {};
		}
		return ret;
    });

    Csw2.lift('params', function (data, delimiter) {
         'use strict';
        var ret = '';
        delimiter = delimiter || '&';
        if (delimiter === '&') {
            Csw2.tryExec(function() { ret = Csw2['?'].param(data); });
        } else {
            Csw2.each(data, function(val, key) {
                if(ret.length > 0) {
                    ret += delimiter;
                }
                ret += key + '=' + val;
            });
        }
        return Csw2.string(ret);
    });


    Csw2.lift('extend', function (destObj, srcObj, deepCopy) {
        'use strict';
        var ret = destObj || {};
        if(arguments.length === 3) {
            ret = window.$.extend(Csw2.bool(deepCopy), ret, srcObj);
        } else {
            ret = window.$.extend(ret, srcObj);
        }
        return ret;
    });

    Csw2.lift('each', function(object, callBack) {
         'use strict';
         if(Csw2.is.array(object) && object.length > 0) {
            object.forEach(callBack);
         }
         else if(object && Object.keys(object) && Object.keys(object).length > 0) {
            Object.keys(object).forEach(callBack);
         }
         return null;
    });

}());