/* jshint undef: true, unused: true */
/* global Ext  */

(function () {

    /*
     * SQL Manager is defined but dormant until initialzed
    */
    var init = (function() {
        /*
         * SQL Manager exposes a connections array and a Csw2.actions.sql.select property
        */
        var manager = function () {
            var ret = Csw2.object();
            ret.add('connections', []);
            ret.add('select', Csw2.actions.sql.select());
            Csw2.actions.sql.lift('manager', ret);
            return ret;
        };
        return manager;

    }());
    Csw2.actions.sql.lift('init', init);

}());