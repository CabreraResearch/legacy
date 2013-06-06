/*global Csw2:true*/
(function(){

	Csw2.lift('tryExec', function(func) {
        'use strict';
        var ret = false;
        try {
            if (Csw2.is.func(func)) {
                ret = func.apply(this, Array.prototype.slice.call(arguments, 1));
            }
        } catch(exception) {
            if ((exception.name !== 'TypeError' ||
                exception.type !== 'called_non_callable') &&
                exception.type !== 'non_object_property_load') { /* ignore errors failing to exec self-executing functions */
                Csw2.console.error(exception);
            }
        } finally {
            return ret;
        }
    });

    Csw2.lift('method', function(func) {
        'use strict';
        var that = this;
        return function() {
            var args = Array.prototype.slice.call(arguments, 0);
            args.unshift(func);
            return Csw2.tryExec.apply(that, args);
        };
    });

}());