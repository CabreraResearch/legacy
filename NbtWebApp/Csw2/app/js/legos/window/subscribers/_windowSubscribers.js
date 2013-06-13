/* jshint undef: true, unused: true */
/* global n$:true, window:true, Ext:true, $: true */

(function _listenerIIFE(n$) {

    /**
     * Define the subscriber methods which are available to this class.
    */
    var windowSubscribers = n$.object();
    windowSubscribers.add('beforeclose', 'beforeclose');
    windowSubscribers.add('beforeshow', 'beforeshow');
    windowSubscribers.add('show', 'show');
    n$.constant(n$.okna, 'subscribers', windowSubscribers);

    n$.okna.subscribers.register('subscribers',
        /**
         * Create a new subscribers collection. This returns a subscribers object with an add method.
        */
        function subscribers() {
            'use strict';
            var ret = n$.makeSubscribers('windowSubscribers', 'okna');
            return ret;
        });


}(window.$nameSpace$));
