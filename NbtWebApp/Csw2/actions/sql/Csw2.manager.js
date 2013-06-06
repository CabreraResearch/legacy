/* jshint undef: true, unused: true */
/* global Ext  */

(function () {

    var manager = function() {
        var ret = Csw2.object();
        ret.add('connections', []);
        ret.add('select', Csw2.actions.sql.select());
        return ret;
    };
    Csw2.actions.sql.lift('manager', manager);

}());