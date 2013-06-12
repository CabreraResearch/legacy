/*global nameSpace:true*/
(function (nameSpace) {

    nameSpace.lift('hasLength',
        /**
         * True if the Object has an Array-like length property
        */
        function hasLength(obj) {
            'use strict';
            var ret = (nameSpace.is.array(obj) || nameSpace.is.jQuery(obj));
            return ret;
        });

    nameSpace.lift('contains',
        /*
         * True if the Object has a property by name, excluding the properties on the Object's prototype
        */
        function contains(object, index) {
            'use strict';
            var ret = false;
            if (false === nameSpace.is.nullOrUndefined(object)) {
                if (nameSpace.is.array(object)) {
                    ret = object.indexOf(index) !== -1;
                }
                if (false === ret && object.hasOwnProperty(index)) {
                    ret = true;
                }
            }
            return ret;
        });

    nameSpace.lift('clone',
        /**
         * Convert an Object to a String to an Object to get a dereferenced copy.
        */
        function (data) {
            'use strict';
            return nameSpace.deserialize(nameSpace.serialize(data));
        });

    nameSpace.lift('serialize',
        /**
         * Convert an Object to a String
        */
        function serialize(data) {
            'use strict';
            var ret = '';
            nameSpace.tryExec(function () { ret = JSON.stringify(data); });
            return ret;
        });


    nameSpace.lift('deserialize',
        /**
         * Convert a string into an Object
        */
        function deserialize(data) {
            'use strict';
            var ret = {};
            nameSpace.tryExec(function () { ret = nameSpace['?'].parseJSON(data); });
            if (nameSpace.is.nullOrEmpty(ret)) {
                ret = {};
            }
            return ret;
        });

    nameSpace.lift('params',
        /**
         * Convert an Object into a serialized parameter string
        */
        function params(data, delimiter) {
            'use strict';
            var ret = '';
            delimiter = delimiter || '&';
            if (delimiter === '&') {
                nameSpace.tryExec(function () { ret = nameSpace['?'].param(data); });
            } else {
                nameSpace.each(data, function (val, key) {
                    if (ret.length > 0) {
                        ret += delimiter;
                    }
                    ret += key + '=' + val;
                });
            }
            return nameSpace.string(ret);
        });

    nameSpace.lift('extend',
        /**
         * Extend the properties of one object with the properties of another. Deep copy to recurse and preserve references.
        */
        function extend(destObj, srcObj, deepCopy) {
            'use strict';
            var ret = destObj || {};
            if (arguments.length === 3) {
                ret = window.$.extend(nameSpace.bool(deepCopy), ret, srcObj);
            } else {
                ret = window.$.extend(ret, srcObj);
            }
            return ret;
        });

    nameSpace.lift('getArguments',
        /**
         * Take an arguments object and convert it into an Array
        */
        function (args, sliceAt) {
            'use strict';
            var slice = Array.prototype.slice;
            sliceAt = sliceAt || 0;

            var ret = slice.call(args, sliceAt);
            return ret;
        });

}(window.$om$));