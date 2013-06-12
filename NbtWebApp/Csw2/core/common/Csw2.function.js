/*global nameSpace:true*/
(function(nameSpace){

    nameSpace.lift('tryExec', function(func) {
        'use strict';
        var ret = false;
        try {
            if (nameSpace.is.func(func)) {
                ret = func.apply(this, Array.prototype.slice.call(arguments, 1));
            }
        } catch(exception) {
            if ((exception.name !== 'TypeError' ||
                exception.type !== 'called_non_callable') &&
                exception.type !== 'non_object_property_load') { /* ignore errors failing to exec self-executing functions */
                nameSpace.console.error(exception);
            }
        } finally {
            return ret;
        }
    });

    nameSpace.lift('method', function(func) {
        'use strict';
        var that = this;
        return function() {
            var args = Array.prototype.slice.call(arguments, 0);
            args.unshift(func);
            return nameSpace.tryExec.apply(that, args);
        };
    });

}(window.$om$));