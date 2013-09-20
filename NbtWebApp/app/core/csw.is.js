/*global Csw:true,$Array:true,window:true,Number:true*/
(function() {

    Csw.is.register('bool', function(boolean) {
        'use strict';
        return (boolean	=== true || boolean	=== false);
    });

    Csw.is.register('arrayNullOrEmpty', function(arr) {
        'use strict';
        return (!Array.isArray(arr) || !arr || !arr.length || arr.length === 0 || !arr.push);
    });

    Csw.is.register('stringNullOrEmpty', function(str) {
        'use strict';
        return (str && ( !str.length || str.length === 0 || !str.trim || !str.trim() ));
    });

    Csw.is.register('numberNullOrEmpty', function(num) {
        'use strict';
        return (!num || isNaN(num) || !num.toPrecision);
    });

    Csw.is.register('dateNullOrEmpty', function(dt) {
        'use strict';
        return (!dt || !dt.getTime);
    });

    Csw.is.register('objectNullOrEmpty', function(obj) {
        'use strict';
        return (!obj || !Object.keys(obj) || Object.keys(obj).length === 0);
    });

    Csw.is.register('plainObject', function (obj) {
        'use strict';
        var ret = (Csw['?'].isPlainObject(obj));
        return ret;
    });

    Csw.is.register('date', function(dt) {
        return (dt instanceof Date);
    });

    /**
        Determines if a value is an instance of a Number and not NaN*
    */
    Csw.is.register('number', function(num) {

        return (typeof num === 'number' &&
        false === (Csw.number.isNaN(num) ||
            false === Csw.number.isFinite(num) ||
            Csw.number.MAX_VALUE === num ||
            Csw.number.MIN_VALUE === num));
    });

    /**
        Determines if a value is convertable to a Number
    */
    Csw.is.register('numeric', function(num) {
        var ret = Csw.is.number(num);
        if (!ret) {
            var nuNum = Csw.to.number(num);
            ret = Csw.is.number(nuNum);
        }
        return ret;
    });

    Csw.is.register('vendorObject', function (obj) {
        'use strict';
        var ret = (obj instanceof Csw['?']);
        return ret;
    });

    Csw.is.register('elementInDom', function (elementId) {
            return false === Csw.is.nullOrEmpty(document.getElementById(elementId));
        });

    Csw.is.register('generic', function (obj) {
        'use strict';
        var ret = (false === Csw.is['function'](obj) && false === Csw.hasLength(obj) && false === Csw.is.plainObject(obj));
        return ret;
    });

    Csw.is.register('array', function(obj) {
        return Array.isArray(obj);
    });


    Csw.is.register('string', function(str) {
        return  null !== str &&
                (typeof str === 'string' || // covers any primitive assignment (e.g. var x = 'x')
                (typeof str === 'object' && str && str.valueOf && typeof str.valueOf() === 'string')); //covers any object assignment (e.g. var x = new String('x'))
    });

    Csw.is.register('true', function (obj) {
        'use strict';
        return (
            obj === true ||
            obj === 'true' ||
            obj === 1 ||
            obj === '1'
        );
    });

    Csw.is.register('false', function (obj) {
        'use strict';
        return (
            obj === false ||
            obj === 'false' ||
            obj === 0 ||
            obj === '0'
        );
    });

    Csw.is.register('trueOrFalse', function (obj) {
        'use strict';
        return ( Csw.is['true'](obj) || Csw.is['false'](obj) );
    });

    Csw.is.register('nullOrEmpty', function (obj, checkLength) {
        'use strict';
        var ret = false;
        if ((!obj && !Csw.is.trueOrFalse(obj) && !Csw.is.func(obj)) ||
            (checkLength && obj && (obj.length === 0 || (Object.keys(obj) && Object.keys(obj).length === 0)))) {
            ret = true;
        }
        return ret;
    });

    Csw.is.register('instanceof', function (name, obj) {
        'use strict';
        return (obj.type === name || obj instanceof name);
    });

    Csw.is.register('func', function(obj) {
        'use strict';
        return typeof(obj) === 'function';
    });


}(window.$nameSpace$));
