/* jshint undef: true, unused: true */
/* global Ext  */

(function (nameSpace) {

    /*
     * SQL Manager is defined but dormant until initialzed
    */
    var init = (function() {
        /*
         * SQL Manager exposes a connections array and a nameSpace.actions.sql.select property
        */
        var manager = function () {
            var ret = nameSpace.object();
            ret.add('connections', []);
            ret.add('select', nameSpace.actions.sql.select());
            nameSpace.actions.sql.lift('manager', ret);
            return ret;
        };
        return manager;

    }());
    nameSpace.actions.sql.lift('init', init);

}(window.$om$));