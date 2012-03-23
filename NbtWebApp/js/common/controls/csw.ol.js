/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function () {
    'use strict';

    Csw.controls.ol = Csw.controls.ol ||
        Csw.controls.register('ol', function(options) {
            /// <summary> Create an <ol /> </summary>
            /// <param name="options" type="Object">Options to define the ol.</param>
            /// <returns type="ol">A ol object</returns>
            var internal = {
                $parent: '',
                number: 1
            };
            var external = { };
            
            external.li = function(liOptions) {
                    /// <summary> Create a <li /> </summary>
                    /// <param name="options" type="Object">Options to define the li.</param>
                    /// <returns type="li">A li object</returns>
                    var liInternal = {
                        number: 1
                    };
                    var liExternal = { };

                    (function() {
                        var html = '<li></li>';
                        var $li;

                        $.extend(liInternal, liOptions);

                        $li = $(html);
                        Csw.controls.factory($li, liExternal);
                        external.append($li);
                    }());

                    return liExternal;
                };

            (function() {
                var html = '<ol></ol>';
                var $ol;

                $.extend(internal, options);

                $ol = $(html);
                Csw.controls.factory($ol, external);

                internal.$parent.append(external.$);
            }());


            return external;
        });

} ());

