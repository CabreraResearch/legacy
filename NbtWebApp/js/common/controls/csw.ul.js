/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/ChemSW-vsdoc.js" />

(function _cswUl() {
    'use strict';


    Csw.controls.ul = Csw.controls.ul ||
        Csw.controls.register('ul', function ul(options) {
            /// <summary> Create a <ul /> </summary>
            /// <param name="options" type="Object">Options to define the ul.</param>
            /// <returns type="ul">A ul object</returns>
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
                var html = '<ul></ul>';
                var $ul;

                $.extend(internal, options);

                $ul = $(html);
                Csw.controls.factory($ul, external);

                internal.$parent.append(external.$);
            }());


            return external;
        });

} ());

