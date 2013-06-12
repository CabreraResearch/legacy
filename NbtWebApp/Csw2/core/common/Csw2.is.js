/*global nameSpace:true,$Array:true,window:true,Number:true*/
(function(nameSpace) {

    nameSpace.is.lift('bool', function(boolean) {
        'use strict';
        return (boolean	=== true || boolean	=== false);
    });

    nameSpace.is.lift('arrayNullOrEmpty', function(arr) {
        'use strict';
        return (!Array.isArray(arr) || !arr || !arr.length || arr.length === 0 || !arr.push);
    });

    nameSpace.is.lift('stringNullOrEmpty', function(str) {
        'use strict';
        return (str && ( !str.length || str.length === 0 || !str.trim || !str.trim() ));
    });

    nameSpace.is.lift('numberNullOrEmpty', function(num) {
        'use strict';
        return (!num || isNaN(num) || !num.toPrecision);
    });

    nameSpace.is.lift('dateNullOrEmpty', function(dt) {
        'use strict';
        return (!dt || !dt.getTime);
    });

    nameSpace.is.lift('objectNullOrEmpty', function(obj) {
        'use strict';
        return (!obj || !Object.keys(obj) || Object.keys(obj).length === 0);
    });

    nameSpace.is.lift('plainObject', function (obj) {
        'use strict';
        var ret = (nameSpace['?'].isPlainObject(obj));
        return ret;
    });

    nameSpace.is.lift('date', function(dt) {
        return (dt instanceof Date);
    });

    /**
        Determines if a value is an instance of a Number and not NaN*
    */
    nameSpace.is.lift('number', function(num) {

        return (typeof num === 'number' &&
        false === (nameSpace.number.isNaN(num) ||
            false === nameSpace.number.isFinite(num) ||
            nameSpace.number.MAX_VALUE === num ||
            nameSpace.number.MIN_VALUE === num));
    });

    /**
        Determines if a value is convertable to a Number
    */
    nameSpace.is.lift('numeric', function(num) {
        var ret = nameSpace.is.number(num);
        if (!ret) {
            var nuNum = nameSpace.to.number(num);
            ret = nameSpace.is.number(nuNum);
        }
        return ret;
    });

    nameSpace.is.lift('vendorObject', function (obj) {
        'use strict';
        var ret = (obj instanceof nameSpace['?']);
        return ret;
    });

    nameSpace.is.lift('elementInDom', function (elementId) {
            return false === nameSpace.is.nullOrEmpty(document.getElementById(elementId));
        });

    nameSpace.is.lift('generic', function (obj) {
        'use strict';
        var ret = (false === nameSpace.is['function'](obj) && false === nameSpace.hasLength(obj) && false === nameSpace.is.plainObject(obj));
        return ret;
    });

    nameSpace.is.lift('array', function(obj) {
        return Array.isArray(obj);
    });


    nameSpace.is.lift('string', function(str) {
        return  null !== str &&
                (typeof str === 'string' || // covers any primitive assignment (e.g. var x = 'x')
                (typeof str === 'object' && str && str.valueOf && typeof str.valueOf() === 'string')); //covers any object assignment (e.g. var x = new String('x'))
    });

    nameSpace.is.lift('true', function (obj) {
        'use strict';
        return (
            obj === true ||
            obj === 'true' ||
            obj === 1 ||
            obj === '1'
        );
    });

    nameSpace.is.lift('false', function (obj) {
        'use strict';
        return (
            obj === false ||
            obj === 'false' ||
            obj === 0 ||
            obj === '0'
        );
    });

    nameSpace.is.lift('trueOrFalse', function (obj) {
        'use strict';
        return ( nameSpace.is['true'](obj) || nameSpace.is['false'](obj) );
    });

    nameSpace.is.lift('nullOrEmpty', function (obj, checkLength) {
        'use strict';
        var ret = false;
        if ((!obj && !nameSpace.is.trueOrFalse(obj) && !nameSpace.is.func(obj)) ||
            (checkLength && obj && (obj.length === 0 || (Object.keys(obj) && Object.keys(obj).length === 0)))) {
            ret = true;
        }
        return ret;
    });

    nameSpace.is.lift('instanceof', function (name, obj) {
        'use strict';
        return (obj.type === name || obj instanceof name);
    });

    nameSpace.is.lift('func', function(obj) {
        'use strict';
        return typeof(obj) === 'function';
    });


}(window.$om$));