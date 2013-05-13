/*global Csw2:true,$Array:true,window:true,Number:true*/
(function() {

    Csw2.is.lift('bool', function(boolean) {
        'use strict';
        return (boolean	=== true || boolean	=== false);
    });

    Csw2.is.lift('arrayNullOrEmpty', function(arr) {
        'use strict';
        return (!Array.isArray(arr) || !arr || !arr.length || arr.length === 0 || !arr.push);
    });

    Csw2.is.lift('stringNullOrEmpty', function(str) {
        'use strict';
        return (str && ( !str.length || str.length === 0 || !str.trim || !str.trim() ));
    });

    Csw2.is.lift('numberNullOrEmpty', function(num) {
        'use strict';
        return (!num || isNaN(num) || !num.toPrecision);
    });

    Csw2.is.lift('dateNullOrEmpty', function(dt) {
        'use strict';
        return (!dt || !dt.getTime);
    });

    Csw2.is.lift('objectNullOrEmpty', function(obj) {
        'use strict';
        return (!obj || !Object.keys(obj) || Object.keys(obj).length === 0);
    });

    Csw2.is.lift('plainObject', function (obj) {
        'use strict';
        var ret = (Csw2['?'].isPlainObject(obj));
        return ret;
    });

    Csw2.is.lift('date', function(dt) {
        return (dt instanceof Date);
    });

    /**
        Determines if a value is an instance of a Number and not NaN*
    */
    Csw2.is.lift('number', function(num) {

        return (typeof num === 'number' &&
        false === (Csw2.number.isNaN(num) ||
            false === Csw2.number.isFinite(num) ||
            Csw2.number.MAX_VALUE === num ||
            Csw2.number.MIN_VALUE === num));
    });

    /**
        Determines if a value is convertable to a Number
    */
    Csw2.is.lift('numeric', function(num) {
        var ret = Csw2.is.number(num);
        if (!ret) {
            var nuNum = Csw2.to.number(num);
            ret = Csw2.is.number(nuNum);
        }
        return ret;
    });

    Csw2.is.lift('vendorObject', function (obj) {
        'use strict';
        var ret = (obj instanceof Csw2['?']);
        return ret;
    });

    Csw2.is.lift('elementInDom', function (elementId) {
            return false === Csw2.is.nullOrEmpty(document.getElementById(elementId));
        });

    Csw2.is.lift('generic', function (obj) {
        'use strict';
        var ret = (false === Csw2.is['function'](obj) && false === Csw2.hasLength(obj) && false === Csw2.is.plainObject(obj));
        return ret;
    });

    Csw2.is.lift('array', function(obj) {
        return Array.isArray(obj);
    });


    Csw2.is.lift('string', function(str) {
        return  null !== str &&
                (typeof str === 'string' || // covers any primitive assignment (e.g. var x = 'x')
                (typeof str === 'object' && str && str.valueOf && typeof str.valueOf() === 'string')); //covers any object assignment (e.g. var x = new String('x'))
    });

    Csw2.is.lift('true', function (obj) {
        'use strict';
        return (
            obj === true ||
            obj === 'true' ||
            obj === 1 ||
            obj === '1'
        );
    });

    Csw2.is.lift('false', function (obj) {
        'use strict';
        return (
            obj === false ||
            obj === 'false' ||
            obj === 0 ||
            obj === '0'
        );
    });

    Csw2.is.lift('trueOrFalse', function (obj) {
        'use strict';
        return ( Csw2.is['true'](obj) || Csw2.is['false'](obj) );
    });

    Csw2.is.lift('nullOrEmpty', function (obj, checkLength) {
        'use strict';
        var ret = false;
        if ((!obj && !Csw2.is.trueOrFalse(obj) && !Csw2.is.func(obj)) ||
            (checkLength && obj && (obj.length === 0 || (Object.keys(obj) && Object.keys(obj).length === 0)))) {
            ret = true;
        }
        return ret;
    });

    Csw2.is.lift('instanceof', function (name, obj) {
        'use strict';
        return (obj.type === name || obj instanceof name);
    });

    Csw2.is.lift('func', function(obj) {
        'use strict';
        return typeof(obj) === 'function';
    });


}());