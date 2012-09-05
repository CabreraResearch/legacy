
/// <reference path="~app/CswApp-vsdoc.js" />

(function() {
    'use strict';

    function isArray(obj) {
        /// <summary> Returns true if the object is an array</summary>
        /// <param name="obj" type="Object"> Object to test</param>
        /// <returns type="Boolean" />
        var ret = ($.isArray(obj));
        return ret;
    }
    Csw.register('isArray', isArray);
    Csw.isArray = Csw.isArray || isArray;
    
    function array() {
        var retArray = [];
        if (arguments.length > 0) {
            retArray = Array.prototype.slice.call(arguments, 0);
        }

//        retArray.contains = retArray.contains || function(value) {
//            return retArray.indexOf(value) !== -1;
//        };

        return retArray;
    }
    Csw.register('array', array);
    Csw.array = Csw.array || array;

    function makeSequentialArray(start, end) {
        var ret = array(),
            i;
        end = +end;
        if (Csw.isNumber(start) &&
            Csw.isNumber(end)) {
            for (i = +start; i <= end; i += 1) {
                ret.push(i);
            }
        }
        return ret;
    }
    Csw.register('makeSequentialArray', makeSequentialArray);
    Csw.makeSequentialArray = Csw.makeSequentialArray || makeSequentialArray;

}());
